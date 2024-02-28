import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map, pipe } from 'rxjs';
import {
  APIResponse,
  EmptyAPIResponse,
} from 'src/app/shared/models/api-response.model';
import { Picture } from '../models/picture.model';
import { environment } from 'src/environments/environment';
import { UrlUtils } from 'src/app/shared/utils/url-utils';

@Injectable({
  providedIn: 'root',
})
export class PicturesService {
  private readonly apiURL = environment.apiUrl + '/Pictures';
  constructor(private http: HttpClient) {}
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  };
  httpUploadOptions = {
    headers: new HttpHeaders(),
  };
  /**
   *
   * @returns an array of pictures the user posses
   */
  getMyPictures(): Observable<APIResponse<Picture[]>> {
    return this.http
      .get<APIResponse<Picture[]>>(
        this.apiURL + '/GetMyPictures',
        this.httpOptions
      )
      .pipe(
        map((response) => {
          response.data.forEach((picture) => {
            picture.picturePath = UrlUtils.getPictureURL(picture.picturePath);
          });
          return response;
        })
      );
  }
  /**
   *deletes a picture
   *
   * @param id id of the picture to the list
   * @returns isSuccess and message
   */
  deletePicture(id: number): Observable<EmptyAPIResponse> {
    return this.http.delete<APIResponse<Picture[]>>(
      this.apiURL + '/DeletePicture/' + id,
      this.httpOptions
    );
  }
  /**
   * Uploads pictures to the server for the user
   * @param pictureFiles the picture files
   * @returns the pictures the user uploaded
   */
  uploadPictures(pictureFiles: FileList): Observable<APIResponse<Picture[]>> {
    const formData = new FormData();
    for (let i = 0; i < pictureFiles.length; i++) {
      formData.append('pictureFiles', pictureFiles[i]);
    }
    return this.http.post<APIResponse<Picture[]>>(
      this.apiURL + '/UploadPicture',
      formData,
      this.httpUploadOptions
    );
  }
}
