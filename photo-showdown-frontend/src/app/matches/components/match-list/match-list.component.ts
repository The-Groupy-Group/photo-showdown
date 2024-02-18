import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { MatchesService } from '../../services/matches.service';
import { Match, MatchStates } from '../../models/match.model';
import { Router } from '@angular/router';
import { NotifierService } from 'angular-notifier';

@Component({
  selector: 'app-match-list',
  templateUrl: './match-list.component.html',
  styleUrls: ['./match-list.component.css'],
})
export class MatchListComponent implements OnInit {
  matches: Match[] = [];
  @Output() onJoinMatch: EventEmitter<number> = new EventEmitter();

  constructor(
    private readonly matchesService: MatchesService,
    private readonly router: Router,
    private readonly notifier: NotifierService
  ) {}

  ngOnInit(): void {
    this.loadMatches();
  }

  createMatch() {
    this.matchesService.createNewMatch().subscribe({
      next: (response) => {
        {
          this.onJoinMatch.emit(response.data.id);
        }
      },
      error: (response) => {
        this.notifier.notify('error', response.error.message);
      },
    });
  }

  loadMatches() {
    this.matches = [];
    this.matchesService.getAllMatches(MatchStates.notStarted).subscribe({
      next: (response) => {
        this.matches = response.data;
      },
    });
  }

  joinedMatch(matchId: number) {
    this.onJoinMatch.emit(matchId);
  }
}
