import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  APIResponse,
  EmptyAPIResponse,
} from 'src/app/shared/models/api-response.model';
import { Match } from '../models/match.model';
import { Observable } from 'rxjs';
import { Entity } from 'src/app/shared/models/entity.model';

@Injectable({
  providedIn: 'root',
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
  createNewMatch(): Observable<APIResponse<Entity>> {
    return this.http.post<APIResponse<Entity>>(
      this.apiURL + '/CreateNewMatch',
      this.httpOptions
    );
  }
  /**
   *
   * @returns all open matches id's their owners and users
   */
  getAllOpenMatches(): Observable<APIResponse<Match[]>> {
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
  getMatchById(matchId: number): Observable<APIResponse<Match>> {
    return this.http.get<APIResponse<Match>>(
      this.apiURL + `/GetMatchById/${matchId}`,
      this.httpOptions
    );
  }
  /**
   *
   * joins the pre-game-lobby
   * @param matchId the matches being joined id
   * @returns failed/worked
   */
  joinMatch(matchId: number): Observable<EmptyAPIResponse> {
    return this.http.post<EmptyAPIResponse>(
      this.apiURL + `/JoinMatch/${matchId}`,
      this.httpOptions
    );
  }
  /**
   *
   * leaves the pre-game-lobby
   * @param matchId the matches being joined id
   * @returns failed/worked
   */
  leaveMatch(matchId: number) {
    return this.http.delete<EmptyAPIResponse>(
      this.apiURL + `/LeaveMatch/${matchId}`,
      this.httpOptions
    );
  }
}
