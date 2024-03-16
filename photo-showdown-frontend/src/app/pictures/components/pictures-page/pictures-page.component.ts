import { Picture } from "./../../models/picture.model";

import { NgForm } from "@angular/forms";
import { Component, OnInit } from "@angular/core";
import { HttpErrorResponse } from "@angular/common/http";
import { PicturesService } from "../../services/pictures.service";
import { NotifierService } from "angular-notifier";

@Component({
	selector: "app-pictures-page",
	templateUrl: "./pictures-page.component.html",
	styleUrls: ["./pictures-page.component.css"]
})
export class PicturesPageComponent implements OnInit {
	usersPictures: Picture[] = [];
	picturesToUpload?: FileList;
	isDeletable = true;

	constructor(
		private readonly picturesService: PicturesService,
		private readonly notifier: NotifierService
	) {}

	ngOnInit(): void {
		this.loadList();
	}

	onPictureDeleted(id: number) {
		this.usersPictures.find((picture) => picture.id === id);
		this.usersPictures = this.usersPictures.filter((picture) => id !== picture.id);
	}

	onFileChange(event: Event) {
		const inputElement = event.target as HTMLInputElement;
		if (inputElement.files && inputElement.files.length > 0) {
			this.picturesToUpload = inputElement.files;
		}
	}

	uploadPictures() {
		if (!this.picturesToUpload || this.picturesToUpload.length === 0) {
			return;
		}
		this.picturesService.uploadPictures(this.picturesToUpload).subscribe({
			next: () => {
				this.picturesToUpload = undefined;
				this.loadList();
				//form.resetForm();
			},
			error: (error: HttpErrorResponse) => {
				this.notifier.notify("error", error.error.message);
			}
		});
	}

	cancelUpload() {
		this.picturesToUpload = undefined;
	}

	private loadList() {
		this.usersPictures = [];
		this.picturesService.getMyPictures().subscribe({
			next: (response) => {
				response.data.forEach((picture) => {
					this.usersPictures.push({ ...picture });
				});
			}
		});
	}
}
