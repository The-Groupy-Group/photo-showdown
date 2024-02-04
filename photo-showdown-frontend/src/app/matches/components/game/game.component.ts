import { Component } from '@angular/core';
import { GameState } from 'src/app/shared/models/game-state.enum';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css'],
})
export class GameComponent {
  gameState: GameState = GameState.NotStarted;
  GameState = GameState;
  matchId: number | undefined;

  redirectToLobby(matchId: number) {
    this.matchId = matchId;
    this.gameState = GameState.InLobby;
  }
  disconnect() {
    this.matchId = undefined;
    this.gameState = GameState.NotStarted;
  }
}
