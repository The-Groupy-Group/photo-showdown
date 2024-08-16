import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { Round, RoundStates } from "../../models/round.model";
import { NotifierService } from "angular-notifier";
import { MatchesService } from "../../services/matches.service";
import { PicturesService } from "src/app/pictures/services/pictures.service";
import { Picture } from "src/app/pictures/models/picture.model";
import { DateTimeUtils } from "src/app/shared/utils/date-time-utils";
import { Observable, map, takeWhile, timer } from "rxjs";
import { UrlUtils } from "src/app/shared/utils/url-utils";
import { Match, MatchStates } from "../../models/match.model";
import { environment } from "src/environments/environment";
import { AuthService } from "src/app/shared/services/auth-service/auth.service";
import { MatchSocketService } from "../../services/match-socket.service";

/**
 * A component that displays the in-match view.
 */
@Component({
	selector: "app-in-match",
	templateUrl: "./in-match.component.html",
	styleUrls: ["./in-match.component.css"]
})
export class InMatchComponent implements OnInit {
	match?: Match;
	usersPictures: Picture[] = [];
	userPictureIds: Set<number> = new Set();
	selectedPicture?: Picture;
	countdown$?: Observable<number>;
	userId = 0;
	roundWinnerUserName?: string;

	@Input({ required: true }) matchId!: number;
	@Output() matchLeft = new EventEmitter<void>();

	readonly RoundStates = RoundStates;
	readonly MatchStates = MatchStates;
	constructor(
		private readonly matchSocketService: MatchSocketService,
		private readonly matchesService: MatchesService,
		private readonly picturesService: PicturesService,
		private readonly notifier: NotifierService,
		private readonly cd: ChangeDetectorRef,
		authService: AuthService
	) {
		this.userId = authService.getUserId();
	}

	async ngOnInit() {
		// Get all pictures for the current user
		this.picturesService.getMyPictures().subscribe((response) => {
			this.usersPictures = response.data;
			this.userPictureIds = new Set(this.usersPictures.map((p) => p.id));
			this.cd.detectChanges();
		});

		// Get the current match
		this.matchesService.getCurrentMatch().subscribe((response) => {
			this.match = response.data;
			// Get the current round
			if (response.data.currentRound) {
				this.handleRoundStateChange(response.data.currentRound);
			}
		});

		this.matchSocketService.openConnection(this.matchId);

		this.matchSocketService.match$.subscribe((match) => {
			this.match = match;
			this.cd.detectChanges();
		});

		// Listen for round state change
		this.matchSocketService.roundStateChanged$.subscribe((round) => {
			this.handleRoundStateChange(round);
			this.cd.detectChanges();
		});
	}

	leaveMatch() {
		this.matchesService.leaveMatch(this.matchId).subscribe({
			error: (response) => {
				if (!environment.production) {
					console.error(response.error);
				}
			}
		});
		this.matchLeft.emit();
	}

	onLockedInPicture() {
		if (!this.match) {
			return;
		}
		const user = this.match.users.find((u) => u.id === this.userId);
		if (!user) {
			return;
		}
		user.isLockedIn = true;
	}

	private handleRoundStateChange(round: Round) {
		// Convert dates to local time
		round.startDate = DateTimeUtils.convertUtcToLocal(round.startDate);
		round.pictureSelectionEndDate = DateTimeUtils.convertUtcToLocal(round.pictureSelectionEndDate);
		round.votingEndDate = DateTimeUtils.convertUtcToLocal(round.votingEndDate);
		round.roundEndDate = DateTimeUtils.convertUtcToLocal(round.roundEndDate);

		// Handle state specific logic
		// Set the timer based on the current round state
		switch (round.roundState) {
			case RoundStates.pictureSelection:
				this.countdown$ = this.setTimer(DateTimeUtils.getSecondsUntil(round.pictureSelectionEndDate));
				break;
			case RoundStates.voting:
				this.countdown$ = this.setTimer(DateTimeUtils.getSecondsUntil(round.votingEndDate));
				break;
			case RoundStates.ended:
				this.countdown$ = this.setTimer(DateTimeUtils.getSecondsUntil(round.roundEndDate));
				break;
		}

		// Set the base URL for the pictures
		round.picturesSelected.forEach((picture) => {
			picture.picturePath = UrlUtils.getPictureURL(picture.picturePath);
		});

		this.roundWinnerUserName = this.match?.users.find((user) => user.id === round.roundWinnerId)?.username;
	}

	private setTimer(seconds: number) {
		return timer(0, 1000).pipe(
			map((x) => seconds - x),
			takeWhile((x) => x >= 0)
		);
	}
}
