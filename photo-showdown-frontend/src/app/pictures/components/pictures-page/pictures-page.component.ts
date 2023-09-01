
import { NgForm } from '@angular/forms';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { PicturesService } from '../../services/pictures.service';

@Component({
  selector: 'app-pictures-page',
  templateUrl: './pictures-page.component.html',
  styleUrls: ['./pictures-page.component.css']
})
export class PicturesPageComponent {
  constructor(
    private readonly picturesService: PicturesService,
    private readonly router: Router
  ) {}


  file?:File;
  errorMessage?: string;

  getFile(event:Event)
  {
    const inputElement = event.target as HTMLInputElement;
    if (inputElement.files && inputElement.files.length > 0)
      this.file = inputElement.files[0];
  }
  onSubmit(form: NgForm)
  {

    let formData=new FormData();
    console.log(this.file);
    if(this.file==undefined)
      {
        this.errorMessage='Please choose a photo.'
        return;
      }
    formData.append("pictureFile",this.file);
    this.picturesService.uploadPicture(formData).subscribe({
      next: (response) => {
        this.errorMessage = undefined;
        console.log(response);
        //TODO let user view the image once uploaded
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage = error.message;
      },
    });
  }
}
