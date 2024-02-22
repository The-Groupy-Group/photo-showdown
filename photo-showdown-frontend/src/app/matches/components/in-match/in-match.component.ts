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
        this.currentRound.startDate = DateTimeUtils.convertUtcToLocal(
          this.currentRound.startDate
        );
        this.countdown$ = this.setTimer(10);
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

  setTimer(seconds: number) {
    return timer(0, 1000).pipe(
      map((x) => seconds - x),
      takeWhile((x) => x >= 0)
    );
  }
}
