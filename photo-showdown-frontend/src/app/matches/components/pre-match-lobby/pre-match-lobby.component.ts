import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { NotifierService } from 'angular-notifier';
import { MatchesService } from '../../services/matches.service';
import { Match } from '../../models/match.model';
import { WebSocketService } from '../../services/web-socket.service';
import {
  EmptyWebSocketMessage,
  WebSocketMessage,
  WebSocketMessageType,
} from '../../models/web-socket-message.model';
import { UserPublicDetails } from 'src/app/users/models/user-public-details.model';
import { AuthService } from 'src/app/shared/services/auth-service/auth.service';
import { MatchSettings } from '../../models/match-settings.model';
import { NgModel } from '@angular/forms';

@Component({
  selector: 'app-pre-match-lobby',
  templateUrl: './pre-match-lobby.component.html',
  styleUrls: ['./pre-match-lobby.component.css'],
})
export class PreMatchLobbyComponent implements OnInit {
  userId: number = this.authService.getUserId();
  isOwner = false;
  match?: Match;
  allSentences = '';
  errorMessage = '';
  isLoading = true;
  matchSettings: MatchSettings = {
    matchId: 0,
    sentences: [],
    numOfRounds: 20,
    numOfVotesToWin: 10,
    pictureSelectionTimeSeconds: 30,
    voteTimeSeconds: 35,
  };

  @Input() matchId!: number;
  @Output() onDisconnect: EventEmitter<undefined> = new EventEmitter();
  @Output() onMatchStart: EventEmitter<undefined> = new EventEmitter();

  constructor(
    private readonly notifier: NotifierService,
    private readonly matchesService: MatchesService,
    private readonly authService: AuthService,
    private readonly webSocketService: WebSocketService,
    private readonly cd: ChangeDetectorRef
  ) {}

  ngOnInit() {
    // Get the match details
    this.matchesService.getMatchById(this.matchId).subscribe((response) => {
      this.match = response.data;
      this.matchSettings.matchId = this.match.id;
      this.isOwner = this.match.owner.id === this.userId;
      this.isLoading = false;
      this.cd.detectChanges();
    });
    // Listen for player joined events
    this.webSocketService.onWebSocketEvent<WebSocketMessage<UserPublicDetails>>(
      WebSocketMessageType.playerJoined,
      (wsMessage) => {
        this.match?.users.push(wsMessage.data);
        this.cd.detectChanges();
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
        this.cd.detectChanges();
      }
    );
    // Listen for new owner events
    this.webSocketService.onWebSocketEvent<WebSocketMessage<UserPublicDetails>>(
      WebSocketMessageType.newOwner,
      (wsMessage) => {
        if (this.match) {
          this.match.owner = wsMessage.data;
          this.isOwner = this.match.owner.id === this.userId;
        }
        this.cd.detectChanges();
      }
    );
    // Listen for match start events
    this.webSocketService.onWebSocketEvent<EmptyWebSocketMessage>(
      WebSocketMessageType.matchStarted,
      (wsMessage) => {
        this.onMatchStart.emit();
      }
    );
  }

  startMatch() {
    if (!this.match || !this.isOwner) {
      return;
    }
    this.matchSettings.sentences =
      this.allSentences.length > 0 ? this.allSentences.split('\n') : [];
      
    this.matchesService.startMatch(this.matchSettings).subscribe({
      error: (response) => {
        this.errorMessage = response.error.message;
      },
    });
  }

  onLeaveMatchClicked() {
    this.matchesService.leaveMatch(this.matchId).subscribe({
      next: () => {
        this.onDisconnect.emit();
      },
      error: (response) => {
        this.notifier.notify('error', response.error.message);
      },
    });
  }
}
