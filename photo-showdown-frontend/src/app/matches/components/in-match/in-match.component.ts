import { ChangeDetectorRef, Component, Input } from '@angular/core';
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

@Component({
  selector: 'app-in-match',
  templateUrl: './in-match.component.html',
  styleUrls: ['./in-match.component.css'],
})
export class InMatchComponent {
  readonly RoundStates = RoundStates;
  usersPictures: Picture[] = [];
  currentRound?: Round;
  selectedPicture?: Picture;
  countdown$?: Observable<number>;

  constructor(
    private readonly webSocketService: WebSocketService,
    private readonly matchesService: MatchesService,
    private readonly picturesService: PicturesService,
    private readonly cd: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.countdown$ = this.setTimer(10);
    // Get all pictures for the current user
    this.picturesService.getMyPictures().subscribe((response) => {
      this.usersPictures = response.data;
      this.cd.detectChanges();
    });

    // Listen for new round started
    this.webSocketService.onWebSocketEvent<WebSocketMessage<Round>>(
      WebSocketMessageType.roundStateChange,
      (wsMessage) => {
        this.currentRound = wsMessage.data;
        // Convert dates to local time
        this.currentRound.startDate = DateTimeUtils.convertUtcToLocal(
          this.currentRound.startDate
        );
        this.currentRound.pictureSelectionEndDate = DateTimeUtils.convertUtcToLocal(
          this.currentRound.pictureSelectionEndDate
        );
        this.currentRound.votingEndDate = DateTimeUtils.convertUtcToLocal(
          this.currentRound.votingEndDate
        );
        this.currentRound.roundEndDate = DateTimeUtils.convertUtcToLocal(
          this.currentRound.roundEndDate
        );
        // Set the timer based on the current round state
        switch (this.currentRound.roundState) {
          case RoundStates.pictureSelection:
            this.countdown$ = this.setTimer(
              DateTimeUtils.getSecondsUntil(this.currentRound.pictureSelectionEndDate)
            );
            break;
          case RoundStates.voting:
            this.countdown$ = this.setTimer(
              DateTimeUtils.getSecondsUntil(this.currentRound.votingEndDate)
            );
            break;
          case RoundStates.ended:
            this.countdown$ = this.setTimer(
              DateTimeUtils.getSecondsUntil(this.currentRound.roundEndDate)
            );
            break;
        }
        this.cd.detectChanges();
      }
    );
  }


  setTimer(seconds: number) {
    return timer(0, 1000).pipe(
      map((x) => seconds - x),
      takeWhile((x) => x >= 0)
    );
  }
}
