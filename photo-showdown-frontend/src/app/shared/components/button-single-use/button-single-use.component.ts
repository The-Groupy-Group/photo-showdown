import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-button-single-use',
  templateUrl: './button-single-use.component.html',
  styleUrls: ['./button-single-use.component.css']
})
export class ButtonSingleUseComponent {
  isDisabled = false;

  @Output() buttonClick = new EventEmitter<void>();

  onClick() {
    // Disable the button after clicking
    this.isDisabled = true;

    // Emit the buttonClick event
    this.buttonClick.emit();
  }
}
