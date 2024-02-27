import { ChangeDetectorRef, Component } from '@angular/core';
import { MatchesService } from '../../services/matches.service';
import { MatchStates } from '../../models/match.model';

/**
 * The main screen for the game. It is responsible for redirecting the user to the correct match state.
 */
@Component({
  selector: 'app-game-main-screen',
  templateUrl: './game-main-screen.component.html',
  styleUrls: ['./game-main-screen.component.css'],
})
export class GameMainScreenComponent {
  matchState?: MatchStates = undefined;
  matchId?: number;

  readonly MatchStates = MatchStates;

  constructor(
    private matchesService: MatchesService,
    private readonly cd: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.handleIsInMatch();
  }

  redirectToLobby(matchId: number) {
    this.matchId = matchId;
    this.matchState = MatchStates.notStarted;
  }

  redirectToMatch() {
    this.matchState = MatchStates.inProgress;
  }

  redirectToMatchList() {
    this.matchId = undefined;
    this.matchState = undefined;
  }

  /**
   * Redirects to the correct match state if the user is in a match.
   */
  private handleIsInMatch() {
    this.matchesService.getCurrentMatch().subscribe((response) => {
      if (!response.data) {
        // Not in a match
        this.matchState = undefined;
      } else if (response.data.matchState === MatchStates.inProgress) {
        // In a match
        this.matchId = response.data.id;
        this.redirectToMatch();
      } else {
        // In a match lobby
        this.redirectToLobby(response.data.id);
      }
      this.cd.detectChanges();
    });
  }
}
