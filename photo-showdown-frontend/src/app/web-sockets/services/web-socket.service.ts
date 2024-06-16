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
	readonly wsURL = UrlUtils.getWebSocketUrl();

	constructor(private readonly authService: AuthService) {
		this.socket$ = this.initSocket();
	}

	/**
	 * Sends a message to the server
	 * @param message
	 */
	sendMessage(message: unknown): void {
		this.socket$.next(message);
	}

	/**
	 * Listens to messages from the server
	 * @returns
	 */
	onWebSocketEvent<T extends EmptyWebSocketMessage = EmptyWebSocketMessage>(type: WebSocketMessageType, f: (wsMessage: T) => void): void {
		this.socket$.asObservable().subscribe({
			next: (message) => {
				if (message && typeof message === "object" && (message as T).type === type) {
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
	 * Closes the connection
	 */
	closeConnection(): void {
		this.socket$.complete();
	}

	private initSocket(): WebSocketSubject<unknown> {
		const token = this.authService.getJwtToken();
		return new WebSocketSubject(this.wsURL + "?jwt=" + token);
	}
}
