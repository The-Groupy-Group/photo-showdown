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
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { UrlUtils } from 'src/app/shared/utils/path-utils';

@Component({
  selector: 'app-in-match',
  templateUrl: './in-match.component.html',
  styleUrls: ['./in-match.component.css'],
})
export class InMatchComponent {
  usersPictures: Picture[] = [];
  userPictureIds: Set<number> = new Set();
  currentRound?: Round;
  selectedPicture?: Picture;
  countdown$?: Observable<number>;

  @Input() matchId!: number;
  @Output() onLeaveMatch: EventEmitter<undefined> = new EventEmitter();

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

    this.matchesService.getCurrentRound(this.matchId).subscribe((response) => {
      this.handleRoundStateChange(response.data);
      this.cd.detectChanges();
    });

    // Listen for new round started
    this.webSocketService.onWebSocketEvent<WebSocketMessage<Round>>(
      WebSocketMessageType.roundStateChange,
      (wsMessage) => {
        this.handleRoundStateChange(wsMessage.data);
        this.cd.detectChanges();
      }
    );
  }

  onLeaveMatchClicked() {
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
        break;
    }
    round.picturesSelected.forEach((picture) => {
      picture.picturePath = `${UrlUtils.getBasePicturesURL()}/${picture.picturePath}`;
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
