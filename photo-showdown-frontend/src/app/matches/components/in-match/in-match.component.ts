import { Component, Input } from '@angular/core';
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

@Component({
  selector: 'app-in-match',
  templateUrl: './in-match.component.html',
  styleUrls: ['./in-match.component.css'],
})
export class InMatchComponent {
  readonly RoundStates = RoundStates;
  currentRound!: Round;
  usersPictures: Picture[] = [];

  /**
   *
   */
  constructor(
    private readonly webSocketService: WebSocketService,
    private readonly matchesService: MatchesService,
    private readonly picturesService: PicturesService,
    private readonly notifier: NotifierService,
  ) {}

  ngOnInit() {
    // Get all pictures for the current user
    this.picturesService.getMyPictures().subscribe((response) => {
      this.usersPictures = response.data;
    });

    // Listen for new round started
    this.webSocketService.onWebSocketEvent<WebSocketMessage<Round>>(
      WebSocketMessageType.newRoundStarted,
      (wsMessage) => {
        this.currentRound = wsMessage.data;
      }
    );
  }
}
