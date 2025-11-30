import { Injectable } from "@angular/core";
import { WebSocketSubject } from "rxjs/webSocket";
import { AuthService } from "src/app/shared/services/auth-service/auth.service";
import { EmptyWebSocketMessage } from "../models/web-socket-message.model";
import { WebSocketMessageType } from "../models/web-socket-message.model";
import { environment } from "src/environments/environment";
import { UrlUtils } from "src/app/shared/utils/url-utils";

@Injectable({
	providedIn: "root"
})
export class WebSocketService {
	private socket$: WebSocketSubject<unknown>;
	private isConnectionOpen = false;
	readonly wsURL = UrlUtils.getWebSocketUrl();

	constructor(private readonly authService: AuthService) {}

	/**
	 * Sends a message to the server
	 * @param message
	 */
	sendMessage(message: unknown): void {
		if (!this.isConnectionOpen) {
			throw new Error("Connection is not open");
		}
		this.socket$.next(message);
	}

	/**
	 * Listens to messages from the server
	 * @returns
	 */
	onWebSocketEvent<T extends EmptyWebSocketMessage = EmptyWebSocketMessage>(type: WebSocketMessageType, f: (wsMessage: T) => void): void {
		if (!this.isConnectionOpen) {
			throw new Error("Connection is not open");
		}
		this.socket$.asObservable().subscribe({
			next: (message) => {
				if (message && typeof message === "object" && (message as T).type === type) {
					console.log(`Received message: ${JSON.stringify(message, null, 2)}`);
					f(message as T);
				}
			},
			error: (error) => {
				if (!environment.production) {
					console.error("WebSocket error: ", error);
				}
			}
		});
	}

	/**
	 * Opens a connection to the server
	 * @returns
	 */
	openConnection(): void {
		if (this.isConnectionOpen) {
			return;
		}
		console.log("Opening connection to: ", this.wsURL);
		const token = this.authService.getJwtToken();
		this.socket$ = new WebSocketSubject(this.wsURL + "?jwt=" + token);
		this.isConnectionOpen = true;
	}

	/**
	 * Closes the connection
	 */
	closeConnection(): void {
		if (!this.isConnectionOpen) {
			return;
		}
		console.log("Closing connection");
		this.socket$.complete();
		this.isConnectionOpen = false;
	}
}
