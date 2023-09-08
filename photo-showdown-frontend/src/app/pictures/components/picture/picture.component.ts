import { PicturesService } from './../../services/pictures.service';
import { Picture } from './../../models/picture.model';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-picture',
  templateUrl: './picture.component.html',
  styleUrls: ['./picture.component.css']
})
export class PictureComponent
{
  constructor(
    private readonly picturesService: PicturesService,
  ) {}

  @Input() picture!:Picture;
  @Input() allowDelete: boolean = true;
  @Output() onDeletePicture: EventEmitter<number> = new EventEmitter();

  onDelete()
  {
    this.onDeletePicture.emit(this.picture.id);
    this.picturesService.deletePicture(this.picture.id).subscribe()
  }
}

