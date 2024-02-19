import { ChangeDetectorRef, Component } from '@angular/core';
import { MatchesService } from '../../services/matches.service';
import { HttpStatusCode } from '@angular/common/http';
import { MatchStates } from '../../models/match.model';

@Component({
  selector: 'app-game-main-screen',
  templateUrl: './game-main-screen.component.html',
  styleUrls: ['./game-main-screen.component.css'],
})
export class GameMainScreenComponent {
  readonly MatchStates = MatchStates;

  matchState?: MatchStates = undefined;
  matchId?: number;

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

  disconnect() {
    this.matchId = undefined;
    this.matchState = undefined;
  }

  private handleIsInMatch() {
    this.matchesService.getCurrentMatch().subscribe((response) => {
      if (!response.data) { // Not in a match
        this.matchState = undefined;
      } else if (response.data.matchState === MatchStates.inProgress) { // In a match
        this.matchId = response.data.id;
        this.redirectToMatch();
      } else { // In a match lobby
        this.redirectToLobby(response.data.id);
      }
      this.cd.detectChanges();
    });
  }
}
