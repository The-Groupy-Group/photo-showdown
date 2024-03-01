import { Component, Input } from '@angular/core';
import { Picture } from '../../models/picture.model';
import { PictureSelected } from '../../models/picture-selected.model';

@Component({
  selector: 'app-picture-item',
  templateUrl: './picture-item.component.html',
  styleUrls: ['./picture-item.component.css'],
})
export class PictureItemComponent {
  @Input() picture!: Picture | PictureSelected;
  @Input() selected: boolean = false;
  @Input() voted: boolean = false;
  @Input() winning: boolean = false;
  @Input() disabled: boolean = false;
}
