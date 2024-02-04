import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Match } from 'src/app/matches/models/match.model';
import { MatchesService } from 'src/app/matches/services/matches.service';
import { MatchConnectionService } from '../../services/match-connections.service';
import { NotifierService } from 'angular-notifier';

@Component({
  selector: 'app-match-list-item',
  templateUrl: './match-list-item.component.html',
  styleUrls: ['./match-list-item.component.css'],
})
export class MatchListItemComponent {
  @Input() match!: Match;
  @Output() onJoinMatch: EventEmitter<number> = new EventEmitter();

  constructor(
    private readonly matchConnectionService: MatchConnectionService,
    private readonly notifier: NotifierService
  ) {}

  joinMatch() {
    let matchId: number = this.match.id;

    this.matchConnectionService.joinMatch(matchId).subscribe({
      next: (response) => {
        this.onJoinMatch.emit(this.match.id);
      },
      error: (response) => {
        this.notifier.notify('error', response.error.message);
      },
    });
  }
}
