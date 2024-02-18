import { Component, Input } from '@angular/core';
import { Picture } from '../../models/picture.model';

@Component({
  selector: 'app-picture-item',
  templateUrl: './picture-item.component.html',
  styleUrls: ['./picture-item.component.css'],
})
export class PictureItemComponent {
  @Input() picture!: Picture;
}
