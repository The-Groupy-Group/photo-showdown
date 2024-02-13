import { LoginResponse } from '../../users/models/login-response.model';
import { Injectable } from '@angular/core';
import jwt_decode from 'jwt-decode';
import { Observable, shareReplay, tap } from 'rxjs';
import { APIResponse } from 'src/app/shared/models/api-response.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { JwtPayload } from '../models/jwt-payload.model';

@Injectable({
  providedIn: 'root',
})
/**
 * this service handles authentication.
 */
export class AuthService {
  constructor(private http: HttpClient, private readonly router: Router) {}
  readonly apiURL = 'https://localhost:7222/api/Users';
  readonly localStorageTokenKey = 'id_token';
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  };

  /**
   *this function logs in the user
   * @param username username to log in
   * @param password password to log in
   * @returns Observable with APIResponse with the given token
   */
  public login(
    username: string,
    password: string
  ): Observable<APIResponse<LoginResponse>> {
    return this.http
      .post<APIResponse<LoginResponse>>(
        this.apiURL + '/Login',
        { username, password },
        this.httpOptions
      )
      .pipe(
        tap((res) => {
          localStorage.setItem(this.localStorageTokenKey, res.data.token);

          this.router.navigate(['/']).then(() => {
            window.location.reload();
          });
        }),
        shareReplay()
      );
  }

  public logout() {
    localStorage.removeItem(this.localStorageTokenKey);
  }
  public isLoggedIn(): boolean {
    return (
      localStorage.getItem(this.localStorageTokenKey) != undefined &&
      this.getExpirationDate().valueOf() > Date.now().valueOf()
    );
  }
  public isLoggedOut(): boolean {
    return !this.isLoggedIn();
  }

  /**
   *decodes the token_id from local storage
   * @returns string of the user's id
   */
  public getUserId(): number {
    var JwtPayload = this.getJwtPayload();
    return Number(JwtPayload.Id);
  }

  public getUsername(): string {
    var JwtPayload = this.getJwtPayload();
    return JwtPayload.Username;
  }

  public getExpirationDate(): Date {
    var JwtPayload = this.getJwtPayload();
    return new Date(JwtPayload.exp * 1000);
  }

  /**
   *decodes the token_id from local storage
   * @returns string of the user's username
   */
  public getJwtPayload(): JwtPayload {
    const jwt = localStorage.getItem('id_token');
    if (!jwt) {
      throw new Error('No JWT found');
    }
    return jwt_decode(jwt);
  }

  public getJwtToken(): string {
    return localStorage.getItem('id_token') || '';
  }
}
