import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NotifierService } from 'angular-notifier';
import { MatchesService } from '../../services/matches.service';
import { Match } from '../../models/match.model';
import { WebSocketService } from '../../services/web-socket.service';
import {
  WebSocketMessage,
  WebSocketMessageType,
} from '../../models/web-socket-message.model';
import { UserPublicDetails } from 'src/app/users/models/user-public-details.model';
import { AuthService } from 'src/app/shared/services/auth-service/auth.service';
import { MatchSettings } from '../../models/match-settings.model';

@Component({
  selector: 'app-pre-game-lobby',
  templateUrl: './pre-game-lobby.component.html',
  styleUrls: ['./pre-game-lobby.component.css'],
})
export class PreGameLobbyComponent implements OnInit {
  match?: Match;
  isOwner = false;
  matchSettings: MatchSettings = { sentences: [] };
  @Input() matchId!: number;
  @Output() onDisconnect: EventEmitter<undefined> = new EventEmitter();

  constructor(
    private readonly notifier: NotifierService,
    private readonly matchesService: MatchesService,
    private readonly authService: AuthService,
    private readonly webSocketService: WebSocketService
  ) {}

  ngOnInit() {
    // Get the match details
    this.matchesService.getMatchById(this.matchId).subscribe({
      next: (response) => {
        this.match = response.data;
        this.isOwner = this.match.owner.id === this.authService.getUserId();
      },
    });
    // Listen for player joined events
    this.webSocketService.onWebSocketEvent<WebSocketMessage<UserPublicDetails>>(
      WebSocketMessageType.playerJoined,
      (wsMessage) => {
        this.match?.users.push(wsMessage.data);
      }
    );
    // Listen for player left events
    this.webSocketService.onWebSocketEvent<WebSocketMessage<UserPublicDetails>>(
      WebSocketMessageType.playerLeft,
      (wsMessage) => {
        const newUserLists = this.match?.users.filter(
          (u) => u.id !== wsMessage.data.id
        );
        if (this.match) {
          this.match.users = newUserLists || [];
        }
      }
    );
    // Listen for new owner events
    this.webSocketService.onWebSocketEvent<WebSocketMessage<UserPublicDetails>>(
      WebSocketMessageType.newOwner,
      (wsMessage) => {
        if (this.match) {
          this.match.owner = wsMessage.data;
        }
      }
    );
  }

  startMatch() {
    if (!this.match || !this.isOwner) {
      return;
    }
    this.matchesService
      .startMatch(this.match.id, this.matchSettings)
      .subscribe();
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
