import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PictureSelected } from 'src/app/pictures/models/picture-selected.model';

@Component({
  selector: 'app-in-match-vote-picture',
  templateUrl: './in-match-vote-picture.component.html',
  styleUrls: ['./in-match-vote-picture.component.css']
})
export class InMatchVotePictureComponent {
  selectedPictureId: number = 0;
  @Input() pictures: PictureSelected[] = [];
  @Output() onVotePicture: EventEmitter<number> = new EventEmitter();

  onPictureSelected(picture: PictureSelected) {
    this.selectedPictureId = picture.id;
    this.onVotePicture.emit(picture.id);
  }
}
