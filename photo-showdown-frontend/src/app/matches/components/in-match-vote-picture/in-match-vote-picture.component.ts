import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PictureSelected } from 'src/app/pictures/models/picture-selected.model';
import { MatchesService } from '../../services/matches.service';
import { Round, RoundStates } from '../../models/round.model';
import { AuthService } from 'src/app/shared/services/auth-service/auth.service';

/**
 * A component that displays the picture selection view in a match.
 */
@Component({
  selector: 'app-in-match-vote-picture',
  templateUrl: './in-match-vote-picture.component.html',
  styleUrls: ['./in-match-vote-picture.component.css'],
})
export class InMatchVotePictureComponent {
  votedPictureId: number = 0;
  userId: number;
  lockedIn = false;

  @Input() picturesToVote: PictureSelected[] = [];
  @Input() userPictureIds = new Set<number>();
  @Input({ required: true }) currentRound!: Round;

  readonly RoundStates = RoundStates;

  constructor(
    private readonly matchesService: MatchesService,
    authService: AuthService
  ) {
    this.userId = authService.getUserId();
  }

  selectPicture(picture: PictureSelected) {
    // Cant vote to your own picture
    if (this.userId === picture.selectedByUser?.id) {
      return;
    }

    if (this.lockedIn) {
      return;
    }

    this.votedPictureId = picture.id;
  }

  lockIn() {
    // Cant vote if no picture was selected
    if (this.votedPictureId === 0) {
      return;
    }

    this.lockedIn = true;
    this.matchesService
      .voteForPicture(
        this.currentRound!.matchId,
        this.currentRound!.roundIndex,
        this.votedPictureId
      )
      .subscribe();
  }
}
