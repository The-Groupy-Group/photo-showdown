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
  pictures: Picture[] = [];
  pictureFile?: File;
  imageSrc?: string;
  isDeletable: boolean = true;

  constructor(
    private readonly picturesService: PicturesService,
    private readonly notifier: NotifierService,
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
    this.pictureFile = undefined;
  }

  getFile(event: Event) {
    const inputElement = event.target as HTMLInputElement;
    if (inputElement.files && inputElement.files.length > 0) {
      this.showUpload(URL.createObjectURL(inputElement.files[0]));
      this.pictureFile = inputElement.files[0];
    }
  }

  onSubmit(form: NgForm) {
    let formData = new FormData();
    if (this.pictureFile == undefined) {
      throw new Error('No file selected');
    }
    formData.append('pictureFile', this.pictureFile);
    this.picturesService.uploadPicture(formData).subscribe({
      next: () => {
        this.imageSrc = undefined;
        this.pictureFile = undefined;
        this.loadList();
        form.resetForm();
      },
      error: (error: HttpErrorResponse) => {
        this.notifier.notify('error', error.error.message);
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
