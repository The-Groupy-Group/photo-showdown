import { Picture } from './../../models/picture.model';

import { NgForm } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { PicturesService } from '../../services/pictures.service';

@Component({
  selector: 'app-pictures-page',
  templateUrl: './pictures-page.component.html',
  styleUrls: ['./pictures-page.component.css'],
})
export class PicturesPageComponent implements OnInit {
  pictures: Picture[] = [];
  file?: File;
  errorMessage?: string;
  imageSrc?: string;
  isDeletable: boolean = true;

  constructor(
    private readonly picturesService: PicturesService,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.loadList();
  }

  removePicture(id: number) {
    this.pictures.find((picture) => picture.id === id);
    this.pictures = [...this.pictures.filter((picture) => id !== picture.id)];
  }
  showUpload(imagePath: string) {
    this.imageSrc = imagePath;
    this.file = undefined;
  }
  getFile(event: Event) {
    const inputElement = event.target as HTMLInputElement;
    if (inputElement.files && inputElement.files.length > 0)
      this.file = inputElement.files[0];
  }
  onSubmit(form: NgForm) {
    let formData = new FormData();
    if (this.file == undefined) {
      this.errorMessage = 'Please choose a photo.';
      return;
    }
    formData.append('pictureFile', this.file);
    this.picturesService.uploadPicture(formData).subscribe({
      next: (response) => {
        this.errorMessage = undefined;
        this.showUpload(response.data.picturePath);
        form.resetForm();
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage = error.message;
      },
    });
  }

  loadList() {
    this.pictures = [];
    this.picturesService.getMyPictures().subscribe({
      next: (response) => {
        response.data.forEach((picture) => {
          this.pictures.push({ ...picture });
        });
      },
    });
  }
}
