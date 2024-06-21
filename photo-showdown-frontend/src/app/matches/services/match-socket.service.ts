import { Injectable } from "@angular/core";
import { WebSocketService } from "../../web-sockets/services/web-socket.service";
import { WebSocketSubject } from "rxjs/webSocket";
import { Match, MatchStates } from "../models/match.model";
import {
	EmptyWebSocketMessage,
	MatchEndedWSMessage,
	WebSocketMessage,
	WebSocketMessageType
} from "../../web-sockets/models/web-socket-message.model";
import { UserInMatch } from "../../users/models/user-public-details.model";
import { Round, RoundStates } from "../models/round.model";

@Injectable({
	providedIn: "root"
})
export class MatchSocketService {
	public match$: WebSocketSubject<Match>;
	public matchStarted$: WebSocketSubject<void>;
	public roundStateChanged$: WebSocketSubject<Round>;

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
			this.match.matchState = MatchStates.inProgress;
			this.match$.next(this.match);
			this.matchStarted$.next();
		});

		// Listen for round state change
		this.webSocketService.onWebSocketEvent<WebSocketMessage<Round>>(WebSocketMessageType.roundStateChange, (wsMessage) => {
			// Update the current round
			this.match.currentRound = wsMessage.data;
			// Reset the locked in state for all users
			this.match.users.forEach((user) => {
				user.isLockedIn = false;
			});
			this.match$.next(this.match);
			this.roundStateChanged$.next(wsMessage.data);
		});

		// Listen for players locking in their picture
		this.webSocketService.onWebSocketEvent<WebSocketMessage<number>>(WebSocketMessageType.userLockedIn, (wsMessage) => {
			const user = this.match.users.find((u) => u.id === wsMessage.data);
			if (user) {
				user.isLockedIn = true;
			}
			this.match$.next(this.match);
		});

		this.webSocketService.onWebSocketEvent<MatchEndedWSMessage>(WebSocketMessageType.matchEnded, () => {
			this.match.matchState = MatchStates.ended;
		});
	}
}
