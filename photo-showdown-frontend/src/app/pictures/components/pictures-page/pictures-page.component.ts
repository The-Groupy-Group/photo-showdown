
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


  file:any
  errorMessage?: string;

  getFile(event:any){
  this.file=event.target.files[0]
  }
  onSubmit(form: NgForm)
  {

    let formData=new FormData();
    console.log(this.file);
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
