import { EventEmitter, Injectable } from "@angular/core";
import { WebSocketService } from "../../web-sockets/services/web-socket.service";
import { Match, MatchStates } from "../models/match.model";
import {
	EmptyWebSocketMessage,
	MatchEndedWSMessage,
	WebSocketMessage,
	WebSocketMessageType
} from "../../web-sockets/models/web-socket-message.model";
import { UserInMatch } from "../../users/models/user-public-details.model";
import { Round } from "../models/round.model";
import { BehaviorSubject, Observable } from "rxjs";
import { MatchesService } from "./matches.service";

@Injectable({
	providedIn: "root"
})
export class MatchSocketService {
	private isConnectionOpen = false;
	private match: Match = {
		id: 0,
		users: [],
		owner: {} as UserInMatch,
		matchState: MatchStates.notStarted,
		currentRound: {} as Round
	} as Match;
	private matchSubject = new BehaviorSubject<Match>(this.match);
	private roundSubject = new BehaviorSubject<Round>(this.match.currentRound ?? ({} as Round));

	public match$: Observable<Match> = this.matchSubject.asObservable();
	public matchStarted$: EventEmitter<void> = new EventEmitter<void>();
	public roundStateChanged$: Observable<Round> = this.roundSubject.asObservable();

	constructor(
		private readonly webSocketService: WebSocketService,
		private readonly matchesService: MatchesService
	) {}

	/**
	 * Opens a connection to the match socket
	 * @param matchId
	 */
	openConnection(matchId: number): void {
		if (this.isConnectionOpen) {
			return;
		}

		// Get the match details
		this.matchesService.getMatchById(matchId).subscribe((response) => {
			this.match = response.data;
			this.matchSubject.next(this.match);
		});

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

	/**
	 * A inner function to listen for all events received from the web socket server
	 */
	private listenForAllEvents() {
		// Listen for player joined events
		this.webSocketService.onWebSocketEvent<WebSocketMessage<UserInMatch>>(WebSocketMessageType.playerJoined, (wsMessage) => {
			this.match.users.push(wsMessage.data);
			this.matchSubject.next(this.match);
		});

		// Listen for player left events
		this.webSocketService.onWebSocketEvent<WebSocketMessage<UserInMatch>>(WebSocketMessageType.playerLeft, (wsMessage) => {
			const newUserLists = this.match?.users.filter((u) => u.id !== wsMessage.data.id);
			if (this.match) {
				this.match.users = newUserLists || [];
			}
			this.matchSubject.next(this.match);
		});

		// Listen for new owner events
		this.webSocketService.onWebSocketEvent<WebSocketMessage<UserInMatch>>(WebSocketMessageType.newOwner, (wsMessage) => {
			if (this.match) {
				this.match.owner = wsMessage.data;
			}
			this.matchSubject.next(this.match);
		});

		// Listen for match start events
		this.webSocketService.onWebSocketEvent<EmptyWebSocketMessage>(WebSocketMessageType.matchStarted, () => {
			this.match.matchState = MatchStates.inProgress;
			this.matchSubject.next(this.match);
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
			this.matchSubject.next(this.match);
			this.roundSubject.next(wsMessage.data);
		});

		// Listen for players locking in their picture
		this.webSocketService.onWebSocketEvent<WebSocketMessage<number>>(WebSocketMessageType.userLockedIn, (wsMessage) => {
			const user = this.match.users.find((u) => u.id === wsMessage.data);
			if (user) {
				user.isLockedIn = true;
			}
			this.matchSubject.next(this.match);
		});

		this.webSocketService.onWebSocketEvent<MatchEndedWSMessage>(WebSocketMessageType.matchEnded, () => {
			this.match.matchState = MatchStates.ended;
		});
	}
}
