import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { NotifierService } from "angular-notifier";
import { MatchesService } from "../../services/matches.service";
import { Match } from "../../models/match.model";
import { WebSocketService } from "../../../web-sockets/services/web-socket.service";
import { EmptyWebSocketMessage, WebSocketMessage, WebSocketMessageType } from "../../../web-sockets/models/web-socket-message.model";
import { UserInMatch } from "src/app/users/models/user-public-details.model";
import { AuthService } from "src/app/shared/services/auth-service/auth.service";
import { MatchSettings } from "../../models/match-settings.model";
import { environment } from "src/environments/environment";

/**
 * A component that displays the pre-match lobby.
 */
@Component({
	selector: "app-pre-match-lobby",
	templateUrl: "./pre-match-lobby.component.html",
	styleUrls: ["./pre-match-lobby.component.css"]
})
export class PreMatchLobbyComponent implements OnInit {
	userId: number;
	match?: Match;
	allSentences = "";
	isLoading = true;
	matchSettings: MatchSettings = {
		matchId: 0,
		sentences: [],
		numOfRounds: 20,
		numOfVotesToWin: 10,
		pictureSelectionTimeSeconds: 30,
		voteTimeSeconds: 35
	};

	@Input({ required: true }) matchId!: number;
	@Output() matchLeft = new EventEmitter<void>();
	@Output() matchStarted = new EventEmitter<void>();

	constructor(
		private readonly matchesService: MatchesService,
		private readonly webSocketService: WebSocketService,
		private readonly notifier: NotifierService,
		private readonly cd: ChangeDetectorRef,
		authService: AuthService
	) {
		this.userId = authService.getUserId();
	}

	ngOnInit() {
		// Get the match details
		this.matchesService.getMatchById(this.matchId).subscribe((response) => {
			this.match = response.data;
			this.matchSettings.matchId = this.match.id;
			this.isLoading = false;
			this.cd.detectChanges();
		});

		// Listen for player joined events
		this.webSocketService.onWebSocketEvent<WebSocketMessage<UserInMatch>>(WebSocketMessageType.playerJoined, (wsMessage) => {
			this.match?.users.push(wsMessage.data);
			this.cd.detectChanges();
		});

		// Listen for player left events
		this.webSocketService.onWebSocketEvent<WebSocketMessage<UserInMatch>>(WebSocketMessageType.playerLeft, (wsMessage) => {
			const newUserLists = this.match?.users.filter((u) => u.id !== wsMessage.data.id);
			if (this.match) {
				this.match.users = newUserLists || [];
			}
			this.cd.detectChanges();
		});

		// Listen for new owner events
		this.webSocketService.onWebSocketEvent<WebSocketMessage<UserInMatch>>(WebSocketMessageType.newOwner, (wsMessage) => {
			if (this.match) {
				this.match.owner = wsMessage.data;
			}
			this.cd.detectChanges();
		});

		// Listen for match start events
		this.webSocketService.onWebSocketEvent<EmptyWebSocketMessage>(WebSocketMessageType.matchStarted, (wsMessage) => {
			this.matchStarted.emit();
		});
	}

	startMatch() {
		if (!this.match || !(this.match.owner.id === this.userId)) {
			return;
		}

		// Split the sentences by new line and remove empty strings
		this.matchSettings.sentences = this.allSentences.length > 0 ? this.allSentences.split("\n").filter((s) => s.length > 0) : [];

		this.matchesService.startMatch(this.matchSettings).subscribe({
			error: (response) => {
				this.notifier.notify("error", response.error.message);
			}
		});
	}

	leaveMatch() {
		this.matchesService.leaveMatch(this.matchId).subscribe({
			next: () => {
				this.matchLeft.emit();
			},
			error: (response) => {
				if (!environment.production) {
					console.error(response.error.message);
				}
			}
		});
	}
}
