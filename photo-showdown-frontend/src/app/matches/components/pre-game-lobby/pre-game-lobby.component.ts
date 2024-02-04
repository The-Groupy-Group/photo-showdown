import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
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
    private readonly notifier: NotifierService,
    private readonly matchesService: MatchesService
  ) {}

  ngOnInit() {
    this.matchesService.getMatchById(this.matchId).subscribe({
      next: (response) => {
        this.match = response.data;
      },
    });
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
