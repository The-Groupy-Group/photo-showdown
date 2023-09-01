import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { APIResponse, EmptyAPIResponse } from 'src/app/shared/models/api-response.model';
import { Picture } from '../models/picture.model';

@Injectable({
  providedIn: 'root'
})
export class PicturesService {

  readonly apiURL = 'https://localhost:7222/api/Users';
  constructor(private http: HttpClient) {}
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  };
/**
 *
 * @returns an array of pictures the user posses
 */
  getMyPicutres():Observable<APIResponse<Picture[]>>
  {
    return this.http.get<APIResponse<Picture[]>>(
      this.apiURL + '/GetMyPictures',
      this.httpOptions
    );
  }
/**
 *deletes a picture
 *
 * @param id id of the picture to the list
 * @returns isSuccess and message
 */
  deletePicture(id:number):Observable<EmptyAPIResponse>
  {
    return this.http.post<APIResponse<Picture[]>>(
      this.apiURL + '/DeletePicture/'+id,
      this.httpOptions
    );
  }
/**
 * uploads a user picture to db
 * @param formData the picture's data
 * @returns the picture the user uploaded
 */
  uploadPicture(formData:FormData):Observable<APIResponse<Picture>>{
    return this.http.post<APIResponse<Picture>>(
      this.apiURL + '/UploadPicture',
      formData,
      this.httpOptions
    );

  }
  }

