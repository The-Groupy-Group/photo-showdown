import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Match } from 'src/app/matches/models/match.model';
import { MatchesService } from 'src/app/matches/services/matches.service';
import { NotifierService } from 'angular-notifier';

/**
 * A component that displays a single match in a list of matches.
 */
@Component({
  selector: 'app-match-list-item',
  templateUrl: './match-list-item.component.html',
  styleUrls: ['./match-list-item.component.css'],
})
export class MatchListItemComponent {
  @Input() match!: Match;
  @Output() matchJoined = new EventEmitter<number>();

  constructor(
    private readonly matchesService: MatchesService,
    private readonly notifier: NotifierService
  ) {}

  joinMatch() {
    let matchId: number = this.match.id;

    this.matchesService.joinMatch(matchId).subscribe({
      next: () => {
        this.matchJoined.emit(this.match.id);
      },
      error: (response) => {
        this.notifier.notify('error', response.error.message);
      },
    });
  }
}
