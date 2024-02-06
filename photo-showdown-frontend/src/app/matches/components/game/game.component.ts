import { Component } from '@angular/core';
import { GameState } from 'src/app/shared/models/game-state.enum';
import { MatchesService } from '../../services/matches.service';
import { HttpStatusCode } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css'],
})
export class GameComponent {
  gameState: GameState = GameState.NotStarted;
  GameState = GameState;
  matchId: number | undefined;

  constructor(
    private matchesService: MatchesService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.handleIsInMatch();
  }

  redirectToLobby(matchId: number) {
    this.matchId = matchId;
    this.gameState = GameState.InLobby;
  }

  redirectToMatch(matchId: number) {
    this.matchId = matchId;
    this.gameState = GameState.InMatch;
  }

  disconnect() {
    this.matchId = undefined;
    this.gameState = GameState.NotStarted;
  }

  private handleIsInMatch() {
    this.matchesService.getCurrentMatch().subscribe({
      next: (response) => {
        if (response.data.hasStarted) {
          this.redirectToMatch(response.data.id);
        }
        this.redirectToLobby(response.data.id);
      },
      error: (err) => {
        if (err.status !== HttpStatusCode.NotFound) {
          console.error(err);
          return;
        }
      },
    });
  }
}
