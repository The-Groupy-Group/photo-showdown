import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatchConnectionService } from '../../services/match-connections.service';
import { ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { MatchesService } from '../../services/matches.service';
import { Match } from '../../models/match.model';
import { AuthService } from 'src/app/shared/auth-service/auth.service';
@Component({
  selector: 'app-pre-game-lobby',
  templateUrl: './pre-game-lobby.component.html',
  styleUrls: ['./pre-game-lobby.component.css'],
})
export class PreGameLobbyComponent implements OnInit {
  matchId!: number;
  userId!: number;
  isLeavingMatch = false;
  match: Match | undefined;
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly matchConnectionService: MatchConnectionService,
    private readonly route: ActivatedRoute,
    private readonly notifier: NotifierService,
    private readonly matchService: MatchesService
  ) {}

  ngOnInit() {
    this.isLeavingMatch = false;
    this.route.params.subscribe((params) => {
      this.matchId = params['matchId'];
    });

    const idFromToken = this.authService.getUserId();

    if (idFromToken != undefined) {
      this.userId = parseInt(idFromToken);
    }
    this.matchService.getMatchById(this.matchId).subscribe({
      next: (response) => {
        this.match = response.data;
      },
    });
  }

  disconnect() {
    this.router.navigate(['/matches']);
  }
}
