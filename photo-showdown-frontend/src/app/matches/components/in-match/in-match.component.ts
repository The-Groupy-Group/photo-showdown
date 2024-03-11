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
  MatchEndedWSMessage,
  WebSocketMessage,
  WebSocketMessageType,
} from '../../models/web-socket-message.model';
import { PicturesService } from 'src/app/pictures/services/pictures.service';
import { Picture } from 'src/app/pictures/models/picture.model';
import { DateTimeUtils } from 'src/app/shared/utils/date-time-utils';
import { Observable, timer, map, takeWhile } from 'rxjs';
import { UrlUtils } from 'src/app/shared/utils/url-utils';
import { Match, MatchStates } from '../../models/match.model';
import { UserPublicDetails } from 'src/app/users/models/user-public-details.model';
import { environment } from 'src/environments/environment';
import { PictureSelected } from 'src/app/pictures/models/picture-selected.model';
import { AuthService } from 'src/app/shared/services/auth-service/auth.service';

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
  usersPictures: Picture[] = [];
  userPictureIds: Set<number> = new Set();
  selectedPicture?: Picture;
  countdown$?: Observable<number>;
  score = new Map<number, number>(); // TODO: https://groupy-group.atlassian.net/browse/PHSH-153 (score should only be received from the server)
  lockedInUserIds: Set<number> = new Set(); // TODO: https://groupy-group.atlassian.net/browse/PHSH-153 (lock ins should only be received from the server)
  userId: number = 0;

  @Input({ required: true }) matchId!: number;
  @Output() matchLeft = new EventEmitter<void>();

  readonly RoundStates = RoundStates;
  readonly MatchStates = MatchStates;
  constructor(
    private readonly webSocketService: WebSocketService,
    private readonly matchesService: MatchesService,
    private readonly picturesService: PicturesService,
    private readonly notifier: NotifierService,
    private readonly cd: ChangeDetectorRef,
    authService: AuthService
  ) {
    this.userId = authService.getUserId();
  }

  ngOnInit()
  {
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

    // Listen for players locking in their picture
    this.webSocketService.onWebSocketEvent<WebSocketMessage<number>>(
      WebSocketMessageType.userLockedIn,
      (wsMessage) => {
        this.lockedInUserIds.add(wsMessage.data);
        this.cd.detectChanges();
      }
    );

    this.webSocketService.onWebSocketEvent<MatchEndedWSMessage>(
      WebSocketMessageType.matchEnded,
      (wsMessage) => {
        this.match!.matchState=MatchStates.ended;
      }
    )

  }

  leaveMatch() {
    this.matchesService.leaveMatch(this.matchId).subscribe({
      error: (response) => {
        if (!environment.production) {
          console.error(response.error);
        }
      },
    });
    this.matchLeft.emit();
  }

  onLockedInPicture() {
    this.lockedInUserIds.add(this.userId);
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

    this.match!.currentRound = round;

    // Reset locked in user ids
    this.lockedInUserIds.clear();
  }

  private setTimer(seconds: number) {
    return timer(0, 1000).pipe(
      map((x) => seconds - x),
      takeWhile((x) => x >= 0)
    );
  }
}
