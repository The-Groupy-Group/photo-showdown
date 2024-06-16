import { Injectable } from "@angular/core";
import { WebSocketService } from "../../web-sockets/services/web-socket.service";
import { WebSocketSubject } from "rxjs/webSocket";
import { Match } from "../models/match.model";
import { EmptyWebSocketMessage, WebSocketMessage, WebSocketMessageType } from "../../web-sockets/models/web-socket-message.model";
import { UserInMatch } from "../../users/models/user-public-details.model";

@Injectable({
	providedIn: "root"
})
export class MatchSocketService {
	public match$: WebSocketSubject<Match>;
	public matchStarted$: WebSocketSubject<void>;

	private isConnectionOpen = false;
	private match: Match;

	constructor(private webSocketService: WebSocketService) {}

	/**
	 * Opens a connection to the match socket
	 * @param matchId
	 */
	openConnection(): void {
		if (this.isConnectionOpen) {
			return;
		}
		this.webSocketService.openConnection();
		this.isConnectionOpen = true;

		this.listenForAllEvents();
	}

	/**
	 * Closes the connection
	 */
	closeConnection(): void {
		if (!this.isConnectionOpen) {
			return;
		}
		this.webSocketService.closeConnection();
		this.isConnectionOpen = false;
	}

	private listenForAllEvents() {
		// Listen for player joined events
		this.webSocketService.onWebSocketEvent<WebSocketMessage<UserInMatch>>(WebSocketMessageType.playerJoined, (wsMessage) => {
			this.match.users.push(wsMessage.data);
			this.match$.next(this.match);
		});

		// Listen for player left events
		this.webSocketService.onWebSocketEvent<WebSocketMessage<UserInMatch>>(WebSocketMessageType.playerLeft, (wsMessage) => {
			const newUserLists = this.match?.users.filter((u) => u.id !== wsMessage.data.id);
			if (this.match) {
				this.match.users = newUserLists || [];
			}
			this.match$.next(this.match);
		});

		// Listen for new owner events
		this.webSocketService.onWebSocketEvent<WebSocketMessage<UserInMatch>>(WebSocketMessageType.newOwner, (wsMessage) => {
			if (this.match) {
				this.match.owner = wsMessage.data;
			}
			this.match$.next(this.match);
		});

		// Listen for match start events
		this.webSocketService.onWebSocketEvent<EmptyWebSocketMessage>(WebSocketMessageType.matchStarted, () => {
			this.matchStarted$.next();
		});
	}
}
