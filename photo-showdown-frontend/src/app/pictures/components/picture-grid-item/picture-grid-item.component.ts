import { PicturesService } from '../../services/pictures.service';
import { Picture } from '../../models/picture.model';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-picture-grid-item',
  templateUrl: './picture-grid-item.component.html',
  styleUrls: ['./picture-grid-item.component.css'],
})
export class PictureGridItemComponent {
  @Input() picture!: Picture;
  @Input() allowDelete: boolean = true;
  @Output() onDeletePicture: EventEmitter<number> = new EventEmitter();

  constructor(private readonly picturesService: PicturesService) {}

  onDelete() {
    this.onDeletePicture.emit(this.picture.id);
    this.picturesService.deletePicture(this.picture.id).subscribe();
  }
}
