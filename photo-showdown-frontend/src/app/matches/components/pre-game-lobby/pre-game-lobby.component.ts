import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NotifierService } from 'angular-notifier';
import { MatchesService } from '../../services/matches.service';
import { Match } from '../../models/match.model';
import { WebSocketService } from '../../services/web-socket.service';
import {
  NewOwnerWebSocketMessage,
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
    // Get the match details
    this.matchesService.getMatchById(this.matchId).subscribe({
      next: (response) => {
        this.match = response.data;
      },
    });
    // Listen for player joined events
    this.webSocketService.onWebSocketEvent<PlayerJoinedWebSocketMessage>(
      WebSocketMessageType.playerJoined,
      (wsMessage) => {
        this.match?.users.push(wsMessage.user);
      }
    );
    // Listen for player left events
    this.webSocketService.onWebSocketEvent<PlayerLeftWebSocketMessage>(
      WebSocketMessageType.playerLeft,
      (wsMessage) => {
        const newUserLists = this.match?.users.filter(u => u.id !== wsMessage.user.id);
        if (this.match) {
          this.match.users = newUserLists || [];
        }
      }
    );
    // Listen for new owner events
    this.webSocketService.onWebSocketEvent<NewOwnerWebSocketMessage>(
      WebSocketMessageType.newOwner,
      (wsMessage) => {
        if (this.match) {
          this.match.owner = wsMessage.user;
        }
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
