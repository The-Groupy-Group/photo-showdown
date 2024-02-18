import { Component } from '@angular/core';
import { GameState } from 'src/app/shared/models/game-state.enum';
import { MatchesService } from '../../services/matches.service';
import { HttpStatusCode } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { Round } from '../../models/round.model';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css'],
})
export class GameComponent {
  readonly GameState = GameState;

  gameState: GameState = GameState.NotStarted;
  matchId?: number;

  constructor(private matchesService: MatchesService) {}

  ngOnInit(): void {
    this.handleIsInMatch();
  }

  redirectToLobby(matchId: number) {
    this.matchId = matchId;
    this.gameState = GameState.InLobby;
  }

  redirectToMatch() {
    this.gameState = GameState.InMatch;
  }

  disconnect() {
    this.matchId = undefined;
    this.gameState = GameState.NotStarted;
  }

  private handleIsInMatch() {
    this.matchesService.getCurrentMatch().subscribe({
      next: (response) => {
        this.matchId = response.data.id;
        if (response.data.matchState) {
          this.redirectToMatch(); // TODO: fetch first round
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
