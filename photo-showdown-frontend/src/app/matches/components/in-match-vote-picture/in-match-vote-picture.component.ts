import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PictureSelected } from 'src/app/pictures/models/picture-selected.model';
import { MatchesService } from '../../services/matches.service';
import { Round } from '../../models/round.model';
import { AuthService } from 'src/app/shared/services/auth-service/auth.service';

@Component({
  selector: 'app-in-match-vote-picture',
  templateUrl: './in-match-vote-picture.component.html',
  styleUrls: ['./in-match-vote-picture.component.css'],
})
export class InMatchVotePictureComponent {
  votedPictureId: number = 0;
  userId: number;
  lockedIn = false;

  @Input() picturesToVote?: PictureSelected[] = [];
  @Input() currentRound?: Round;

  constructor(
    private readonly matchesService: MatchesService,
    private readonly authService: AuthService
  ) {
    this.userId = this.authService.getUserId();
  }

  onPictureVoted(picture: PictureSelected) {
    // Cant vote to your own picture
    if (this.userId === picture.selectedByUser.id) {
      return;
    }

    this.votedPictureId = picture.id;
  }

  onLockIn() {
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
