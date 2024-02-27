import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Picture } from 'src/app/pictures/models/picture.model';
import { MatchesService } from '../../services/matches.service';
import { Round } from '../../models/round.model';

/**
 * A component that displays the picture selection view in a match.
 */
@Component({
  selector: 'app-in-match-picture-selection',
  templateUrl: './in-match-picture-selection.component.html',
  styleUrls: ['./in-match-picture-selection.component.css'],
})
export class InMatchPictureSelectionComponent {
  selectedPictureId?: number;
  lockedIn = false;

  @Input() pictures: Picture[] = [];
  @Input() currentRound!: Round;

  constructor(private readonly matchesService: MatchesService) {}

  selectPicture(picture: Picture) {
    if (this.lockedIn) {
      return;
    }
    this.selectedPictureId = picture.id;
  }

  lockIn() {
    this.lockedIn = true;
    this.matchesService
      .selectPictureForRound(
        this.currentRound!.matchId,
        this.currentRound!.roundIndex,
        this.selectedPictureId!
      )
      .subscribe();
  }
}
