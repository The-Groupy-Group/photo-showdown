import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Picture } from 'src/app/pictures/models/picture.model';

@Component({
  selector: 'app-in-match-picture-selection',
  templateUrl: './in-match-picture-selection.component.html',
  styleUrls: ['./in-match-picture-selection.component.css'],
})
export class InMatchPictureSelectionComponent {
  @Input() usersPictures: Picture[] = [];
  @Output() selectedPicture: EventEmitter<Picture> =
    new EventEmitter<Picture>();
}
