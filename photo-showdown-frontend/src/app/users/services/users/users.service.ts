import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { User } from "../../models/user.model";
import { APIResponse } from "src/app/shared/models/api-response.model";
import { environment } from "src/environments/environment";

@Injectable({
	providedIn: "root"
})
export class UsersService {
	readonly apiURL = environment.apiUrl + "/Users";
	constructor(private http: HttpClient) {}
	httpOptions = {
		headers: new HttpHeaders({
			"Content-Type": "application/json"
		})
	};

	createUser(user: User): Observable<APIResponse<User>> {
		return this.http.post<APIResponse<User>>(this.apiURL + "/Register", user, this.httpOptions);
	}
}
