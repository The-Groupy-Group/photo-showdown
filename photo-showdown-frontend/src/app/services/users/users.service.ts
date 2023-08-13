import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {User} from '../../models/user.model'
import { Observable } from 'rxjs';
import { APIResponse } from 'src/app/models/api-response.model';
import { LoginResponse } from 'src/app/models/users/login-response.model';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  apiURL = 'http://localhost:7222/api/Users';
  constructor(private http:HttpClient){}
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  };


  createUser(user:User): Observable<APIResponse<User>>
  {
    return this.http
      .post<APIResponse<User>>(this.apiURL+'/Register',
        JSON.stringify(user),
        this.httpOptions)
  }

  login(username:string,password:string): Observable<APIResponse<LoginResponse>>
  {
    return this.http
      .post<APIResponse<LoginResponse>>(this.apiURL+'/Login',
        {username,password},
        this.httpOptions)
  }
  
}
