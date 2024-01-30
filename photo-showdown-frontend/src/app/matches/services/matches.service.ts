import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { APIResponse } from 'src/app/shared/models/api-response.model';
import { Match } from '../models/match.model';
import { MatchId } from '../models/match-id.model';
import { Observable } from 'rxjs';

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
  createNewMatch():Observable<APIResponse<MatchId>>
  {
    return this.http.post<APIResponse<MatchId>>(
      this.apiURL + '/CreateNewMatch',
      this.httpOptions
    );
  }
/**
 *
 * @returns all open matches id's their owners and users
 */
  getAllOpenMatches():Observable<APIResponse<Match[]>>
  {
    return this.http.get<APIResponse<Match[]>>(
      this.apiURL + '/GetAllOpenMatches',
      this.httpOptions
    );
  }
/**
 *Use match id to get match details
 *
 * @returns match details
 */
  getMatchById(matchId:number):Observable<APIResponse<Match>>
  {
    return this.http.get<APIResponse<Match>>(
      this.apiURL+`/GetMatchById?matchId=${matchId}`,
      this.httpOptions
    );
  }
}
