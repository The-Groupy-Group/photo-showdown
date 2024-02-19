import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Picture } from 'src/app/pictures/models/picture.model';

@Component({
  selector: 'app-in-match-picture-selection',
  templateUrl: './in-match-picture-selection.component.html',
  styleUrls: ['./in-match-picture-selection.component.css'],
})
export class InMatchPictureSelectionComponent {
  selectedPictureId: number = 0;
  @Input() pictures: Picture[] = [];
  @Output() onSelectedPicture: EventEmitter<Picture> = new EventEmitter();

  onPictureSelected(picture: Picture) {
    this.selectedPictureId = picture.id;
    this.onSelectedPicture.emit(picture);
  }
}
