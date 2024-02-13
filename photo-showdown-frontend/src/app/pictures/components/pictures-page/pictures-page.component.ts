import { Picture } from './../../models/picture.model';

import { NgForm } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { PicturesService } from '../../services/pictures.service';
import { NotifierService } from 'angular-notifier';

@Component({
  selector: 'app-pictures-page',
  templateUrl: './pictures-page.component.html',
  styleUrls: ['./pictures-page.component.css'],
})
export class PicturesPageComponent implements OnInit {
  usersPictures: Picture[] = [];
  pictureFileToUpload?: File;
  pictureDisplayURL?: string;
  isDeletable: boolean = true;

  constructor(
    private readonly picturesService: PicturesService,
    private readonly notifier: NotifierService
  ) {}

  ngOnInit(): void {
    this.loadList();
  }

  removePicture(id: number) {
    this.usersPictures.find((picture) => picture.id === id);
    this.usersPictures = [...this.usersPictures.filter((picture) => id !== picture.id)];
  }

  showUpload(imagePath: string) {
    this.pictureDisplayURL = imagePath;
    this.pictureFileToUpload = undefined;
  }

  getFile(event: Event) {
    const inputElement = event.target as HTMLInputElement;
    if (inputElement.files && inputElement.files.length > 0) {
      this.showUpload(URL.createObjectURL(inputElement.files[0]));
      this.pictureFileToUpload = inputElement.files[0];
    }
  }

  onSubmit(form: NgForm) {
    let formData = new FormData();
    if (this.pictureFileToUpload == undefined) {
      throw new Error('No file selected');
    }
    formData.append('pictureFile', this.pictureFileToUpload);
    this.picturesService.uploadPicture(formData).subscribe({
      next: () => {
        this.pictureDisplayURL = undefined;
        this.pictureFileToUpload = undefined;
        this.loadList();
        form.resetForm();
      },
      error: (error: HttpErrorResponse) => {
        this.notifier.notify('error', error.error.message);
      },
    });
  }

  loadList() {
    this.usersPictures = [];
    this.picturesService.getMyPictures().subscribe({
      next: (response) => {
        response.data.forEach((picture) => {
          this.usersPictures.push({ ...picture });
        });
      },
    });
  }
}
