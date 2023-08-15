import { LoginResponse } from './../../models/login-response.model';
import { Injectable } from '@angular/core';
import { UsersService } from '../users/users.service';
import { Observable, shareReplay, tap } from 'rxjs';
import { APIResponse } from 'src/app/shared/models/api-response.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';


@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {

  constructor(private http: HttpClient) {}
  readonly apiURL = 'https://localhost:7222/api/Users';
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  };
  public login(username: string,password: string):Observable<APIResponse<LoginResponse>>
  {
    return this.http.post<APIResponse<LoginResponse>>(
      this.apiURL + '/Login',
      { username, password },
      this.httpOptions
    )
    .pipe(tap((res)=>this.setSession(res)),
    shareReplay());
  }
  private setSession(authResult:APIResponse<LoginResponse>)
  {
    localStorage.setItem('id_token', authResult.data.token);
  }
  public logout()
  {
    localStorage.removeItem("id_token");
  }
  public isLoggedIn():boolean
  {
      return localStorage.getItem("id_token")!==undefined;
  }
  public isLoggedOut():boolean
  {
    return !this.isLoggedIn();
  }
}
