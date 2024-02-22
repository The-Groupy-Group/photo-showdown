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
import { Observable, Subscription, tap, timer } from 'rxjs';

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
  secondsCountDown: number = 10;
  private timerSubscription?: Subscription;

  constructor(
    private readonly webSocketService: WebSocketService,
    private readonly matchesService: MatchesService,
    private readonly picturesService: PicturesService,
    private readonly notifier: NotifierService,
    private readonly cd: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.setCountDown(10);
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
        this.currentRound.startDate = DateTimeUtils.convertUtcToLocal(
          this.currentRound.startDate
        );
        this.setCountDown(10);
        this.cd.detectChanges();
      }
    );
  }

  onPictureSelected(picture: Picture) {
    this.selectedPicture = picture;
    this.matchesService
      .selectPictureForRound(
        this.currentRound!.matchId,
        this.currentRound!.roundIndex,
        picture.id
      )
      .subscribe();
  }

  setCountDown(seconds: number) {
    this.timerSubscription?.unsubscribe();
    this.timerSubscription = timer(0, 1000)
      .pipe(
        tap((x) => {
          let remaining = seconds - x;
          if (remaining < 0) {
            remaining = 0;
            this.timerSubscription!.unsubscribe();
          }
          this.secondsCountDown = remaining;
          this.cd.detectChanges();
        })
      )
      .subscribe();
  }
}
