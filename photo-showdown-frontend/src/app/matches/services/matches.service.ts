import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  APIResponse,
  EmptyAPIResponse,
} from 'src/app/shared/models/api-response.model';
import { Match } from '../models/match.model';
import { Observable } from 'rxjs';
import { Entity } from 'src/app/shared/models/entity.model';
import { environment } from 'src/environments/environment';
import { MatchSettings } from '../models/match-settings.model';

@Injectable({
  providedIn: 'root',
})
export class MatchesService {
  readonly apiURL = environment.apiUrl + '/Matches';
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
  leaveMatch(matchId: number): Observable<EmptyAPIResponse> {
    return this.http.delete<EmptyAPIResponse>(
      this.apiURL + `/LeaveMatch/${matchId}`,
      this.httpOptions
    );
  }
  /**
   *
   * starts the match
   * @param matchId the matches being joined id
   * @returns failed/worked
   */
  getCurrentMatch(): Observable<APIResponse<Match>> {
    return this.http.get<APIResponse<Match>>(
      this.apiURL + '/GetCurrentMatch',
      this.httpOptions
    );
  }
  /**
   *
   * starts the match
   * @param matchId the matches being joined id
   * @returns failed/worked
   */
  startMatch(
    matchId: number,
    matchSettings: MatchSettings
  ): Observable<EmptyAPIResponse> {
    return this.http.post<EmptyAPIResponse>(
      this.apiURL + `/StartMatch/${matchId}`,
      matchSettings,
      this.httpOptions
    );
  }
}
