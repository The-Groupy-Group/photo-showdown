import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { APIResponse } from 'src/app/shared/models/api-response.model';
import { MyMatch } from '../models/my-match.model';
import { Match } from '../models/match.model';

@Injectable({
  providedIn: 'root'
})
export class MatchesService {

  readonly apiURL = 'https://localhost:7222/api/Matches';
  constructor(private http: HttpClient) {}
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  };

/**
 *
 *
 * @returns the created match's id
 */
  createNewMatch()
  {
    return this.http.post<APIResponse<MyMatch>>(
      this.apiURL + '/CreateNewMatch',
      this.httpOptions
    );
  }
/**
 *
 * @returns all open matches id's their owners and users
 */
  getAllOpenMatches()
  {
    return this.http.get<APIResponse<Match[]>>(
      this.apiURL + '/GetAllOpenMatches',
      this.httpOptions
    );
  }
}
