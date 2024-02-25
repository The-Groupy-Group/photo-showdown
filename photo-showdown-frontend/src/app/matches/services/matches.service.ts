import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  APIResponse,
  EmptyAPIResponse,
} from 'src/app/shared/models/api-response.model';
import { Match, MatchStates } from '../models/match.model';
import { Observable } from 'rxjs';
import { Entity } from 'src/app/shared/models/entity.model';
import { environment } from 'src/environments/environment';
import { MatchSettings } from '../models/match-settings.model';
import { Round } from '../models/round.model';

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

  // ------------------- Match API Calls ------------------- //
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
  getAllMatches(state?: MatchStates): Observable<APIResponse<Match[]>> {
    let url = this.apiURL + '/GetAllMatches';
    if (state) {
      url += `?state=${state}`;
    }
    return this.http.get<APIResponse<Match[]>>(url, this.httpOptions);
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
  startMatch(matchSettings: MatchSettings): Observable<EmptyAPIResponse> {
    return this.http.post<EmptyAPIResponse>(
      this.apiURL + `/StartMatch`,
      matchSettings,
      this.httpOptions
    );
  }

  // ------------------- In Match API Calls ------------------- //
  /**
   *
   * @returns the current round
   */
  getCurrentRound(matchId: number): Observable<APIResponse<Round>> {
    return this.http.get<APIResponse<Round>>(
      `${this.apiURL}/GetCurrentRound?matchId=${matchId}`,
      this.httpOptions
    );
  }

  selectPictureForRound(
    matchId: number,
    roundIndex: number,
    pictureId: number
  ): Observable<EmptyAPIResponse> {
    return this.http.post<EmptyAPIResponse>(
      this.apiURL + `/SelectPictureForRound`,
      {
        matchId,
        roundIndex,
        pictureId,
      },
      this.httpOptions
    );
  }

  voteForPicture(
    matchId: number,
    roundIndex: number,
    pictureSelectedId: number
  ): Observable<EmptyAPIResponse> {
    return this.http.post<EmptyAPIResponse>(
      this.apiURL + `/VoteForSelectedPicture`,
      {
        matchId,
        roundIndex,
        roundPictureId: pictureSelectedId,
      },
      this.httpOptions
    );
  }
}
