import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatchConnectionService } from '../../services/match-connections.service';
import { NotifierService } from 'angular-notifier';
import { MatchesService } from '../../services/matches.service';
import { Match } from '../../models/match.model';

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
    private readonly matchConnectionService: MatchConnectionService,
    private readonly notifier: NotifierService,
    private readonly matchService: MatchesService
  ) {}

  ngOnInit() {
    this.matchService.getMatchById(this.matchId).subscribe({
      next: (response) => {
        this.match = response.data;
      },
    });
  }

  disconnect() {
    this.matchConnectionService.leaveMatch(this.matchId).subscribe({
      next: (response) => {
        this.onDisconnect.emit();
      },
      error: (response) => {
        this.notifier.notify('error', response.error.message);
      },
    });
  }
}
