import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { User } from '../../models/user.model';
import { APIResponse } from 'src/app/shared/models/api-response.model';
import { LoginResponse } from 'src/app/users/models/login-response.model';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  readonly apiURL = 'https://localhost:7222/api/Users';
  constructor(private http: HttpClient) {}
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  };

  createUser(user: User): Observable<APIResponse<User>> {
    return this.http.post<APIResponse<User>>(
      this.apiURL + '/Register',
      user,
      this.httpOptions
    );
  }
}
