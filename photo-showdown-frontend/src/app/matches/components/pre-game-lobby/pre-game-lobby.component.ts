import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NotifierService } from 'angular-notifier';
import { MatchesService } from '../../services/matches.service';
import { Match } from '../../models/match.model';
import { WebSocketService } from '../../services/web-socket.service';
import {
  PlayerJoinedWebSocketMessage,
  PlayerLeftWebSocketMessage,
  WebSocketMessageType,
} from '../../models/web-socket-message.model';

@Component({
  selector: 'app-pre-game-lobby',
  templateUrl: './pre-game-lobby.component.html',
  styleUrls: ['./pre-game-lobby.component.css'],
})
export class PreGameLobbyComponent implements OnInit {
  match: Match | undefined;
  @Input() matchId!: number;
  @Output() onDisconnect: EventEmitter<undefined> = new EventEmitter();

  constructor(
    private readonly notifier: NotifierService,
    private readonly matchesService: MatchesService,
    private readonly webSocketService: WebSocketService
  ) {}

  ngOnInit() {
    this.matchesService.getMatchById(this.matchId).subscribe({
      next: (response) => {
        this.match = response.data;
      },
    });
    this.webSocketService.onWebSocketEvent<PlayerJoinedWebSocketMessage>(
      WebSocketMessageType.playerJoined,
      (wsMessage) => {
        this.match?.userNames.push(wsMessage.userName);
      }
    );
    this.webSocketService.onWebSocketEvent<PlayerLeftWebSocketMessage>(
      WebSocketMessageType.playerLeft,
      (wsMessage) => {
        const index = this.match?.userNames.indexOf(wsMessage.userName);
        if (index != undefined) this.match?.userNames.splice(index, 1);
      }
    );
  }

  disconnect() {
    this.matchesService.leaveMatch(this.matchId).subscribe({
      next: (response) => {
        this.onDisconnect.emit();
      },
      error: (response) => {
        this.notifier.notify('error', response.error.message);
      },
    });
  }
}
