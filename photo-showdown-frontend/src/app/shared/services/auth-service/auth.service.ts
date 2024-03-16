import { LoginResponse } from "../../../users/models/login-response.model";
import { Injectable } from "@angular/core";
import jwt_decode from "jwt-decode";
import { Observable, shareReplay, tap } from "rxjs";
import { APIResponse } from "src/app/shared/models/api-response.model";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Router } from "@angular/router";
import { JwtPayload } from "../../models/jwt-payload.model";
import { environment } from "src/environments/environment";

@Injectable({
	providedIn: "root"
})
/**
 * this service handles authentication.
 */
export class AuthService {
	private readonly apiURL = environment.apiUrl + "/Users";
	private readonly TOKEN_KEY = "auth-token";
	private readonly httpOptions = {
		headers: new HttpHeaders({
			"Content-Type": "application/json"
		})
	};

	constructor(
		private http: HttpClient,
		private readonly router: Router
	) {}

	/**
	 * this function logs in the user
	 * @param username username to log in
	 * @param password password to log in
	 * @returns Observable with APIResponse with the given token
	 */
	public login(username: string, password: string): Observable<APIResponse<LoginResponse>> {
		window.sessionStorage.clear();
		return this.http.post<APIResponse<LoginResponse>>(this.apiURL + "/Login", { username, password }, this.httpOptions).pipe(
			tap((res) => {
				this.setJwtToken(res.data.token);
			}),
			shareReplay()
		);
	}

	public logout() {
		window.sessionStorage.removeItem(this.TOKEN_KEY);
	}

	public isLoggedIn(): boolean {
		return this.getJwtToken() != undefined && this.getExpirationDate().valueOf() > Date.now().valueOf();
	}

	/**
	 * decodes the token_id from local storage
	 * @returns string of the user's id
	 */
	public getUserId(): number {
		return Number(this.getJwtPayload().Id);
	}

	public getUsername(): string {
		return this.getJwtPayload().Username;
	}

	public getJwtToken(): string | null {
		return window.sessionStorage.getItem(this.TOKEN_KEY);
	}

	public setJwtToken(token: string) {
		window.sessionStorage.setItem(this.TOKEN_KEY, token);
	}

	public getJwtPayload(): JwtPayload {
		const jwt = window.sessionStorage.getItem(this.TOKEN_KEY);
		if (!jwt) {
			throw new Error("No JWT found");
		}
		return jwt_decode(jwt);
	}

	private getExpirationDate(): Date {
		//return new Date(jwt_decode<JwtPayload>(token)!.exp * 1000);
		return new Date(this.getJwtPayload().exp * 1000);
	}
}
