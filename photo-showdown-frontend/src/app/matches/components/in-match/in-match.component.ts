import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  Output,
} from '@angular/core';
import { Round, RoundStates } from '../../models/round.model';
import { WebSocketService } from '../../services/web-socket.service';
import { NotifierService } from 'angular-notifier';
import { MatchesService } from '../../services/matches.service';
import {
  WebSocketMessage,
  WebSocketMessageType,
} from '../../models/web-socket-message.model';
import { PicturesService } from 'src/app/pictures/services/pictures.service';
import { Picture } from 'src/app/pictures/models/picture.model';
import { DateTimeUtils } from 'src/app/shared/utils/date-time-utils';
import { Observable, timer, map, takeWhile } from 'rxjs';
import { UrlUtils } from 'src/app/shared/utils/url-utils';
import { Match } from '../../models/match.model';
import { UserPublicDetails } from 'src/app/users/models/user-public-details.model';

/**
 * A component that displays the in-match view.
 */
@Component({
  selector: 'app-in-match',
  templateUrl: './in-match.component.html',
  styleUrls: ['./in-match.component.css'],
})
export class InMatchComponent {
  match?: Match;
  currentRound?: Round;
  usersPictures: Picture[] = [];
  userPictureIds: Set<number> = new Set();
  selectedPicture?: Picture;
  countdown$?: Observable<number>;
  score = new Map<number, number>(); // TODO: https://groupy-group.atlassian.net/browse/PHSH-153

  @Input({ required: true }) matchId!: number;
  @Output() onLeaveMatch = new EventEmitter<void>();

  readonly RoundStates = RoundStates;

  constructor(
    private readonly webSocketService: WebSocketService,
    private readonly matchesService: MatchesService,
    private readonly picturesService: PicturesService,
    private readonly notifier: NotifierService,
    private readonly cd: ChangeDetectorRef
  ) {}

  ngOnInit() {
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
      // TODO: https://groupy-group.atlassian.net/browse/PHSH-153
      this.matchesService
        .getCurrentRound(this.matchId)
        .subscribe((response) => {
          this.handleRoundStateChange(response.data);
          this.match!.currentRound = this.currentRound;
          this.cd.detectChanges();
        });

      this.match.users.forEach((user) => {
        this.score.set(user.id, 0);
      });
    });

    // Listen for round state change
    this.webSocketService.onWebSocketEvent<WebSocketMessage<Round>>(
      WebSocketMessageType.roundStateChange,
      (wsMessage) => {
        this.handleRoundStateChange(wsMessage.data);
        this.cd.detectChanges();
      }
    );

    // Listen for players leaving the match
    this.webSocketService.onWebSocketEvent<WebSocketMessage<UserPublicDetails>>(
      WebSocketMessageType.playerLeft,
      (wsMessage) => {
        const newUserLists = this.match?.users.filter(
          (u) => u.id !== wsMessage.data.id
        );
        if (this.match) {
          this.match.users = newUserLists || [];
        }
        this.score.delete(wsMessage.data.id);
        this.cd.detectChanges();
      }
    );
  }

  leaveMatch() {
    this.matchesService.leaveMatch(this.matchId).subscribe({
      next: () => {
        this.onLeaveMatch.emit();
      },
      error: (response) => {
        this.notifier.notify('error', response.error.message);
      },
    });
  }

  private handleRoundStateChange(round: Round) {
    // Convert dates to local time
    round.startDate = DateTimeUtils.convertUtcToLocal(round.startDate);
    round.pictureSelectionEndDate = DateTimeUtils.convertUtcToLocal(
      round.pictureSelectionEndDate
    );
    round.votingEndDate = DateTimeUtils.convertUtcToLocal(round.votingEndDate);
    round.roundEndDate = DateTimeUtils.convertUtcToLocal(round.roundEndDate);

    // Handle state specific logic
    // Set the timer based on the current round state
    switch (round.roundState) {
      case RoundStates.pictureSelection:
        this.countdown$ = this.setTimer(
          DateTimeUtils.getSecondsUntil(round.pictureSelectionEndDate)
        );
        break;
      case RoundStates.voting:
        this.countdown$ = this.setTimer(
          DateTimeUtils.getSecondsUntil(round.votingEndDate)
        );
        break;
      case RoundStates.ended:
        this.countdown$ = this.setTimer(
          DateTimeUtils.getSecondsUntil(round.roundEndDate)
        );
        if (round.roundWinner?.id) {
          this.score.set(
            round.roundWinner!.id,
            this.score.get(round.roundWinner!.id)! + 1
          );
        }
        break;
    }

    // Set the base URL for the pictures
    round.picturesSelected.forEach((picture) => {
      picture.picturePath = UrlUtils.getPictureURL(picture.picturePath);
    });

    this.currentRound = round;
  }

  private setTimer(seconds: number) {
    return timer(0, 1000).pipe(
      map((x) => seconds - x),
      takeWhile((x) => x >= 0)
    );
  }
}
