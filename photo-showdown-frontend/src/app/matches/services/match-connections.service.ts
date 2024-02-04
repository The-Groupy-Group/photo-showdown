import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from 'src/app/shared/auth-service/auth.service';
import { EmptyAPIResponse } from 'src/app/shared/models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class MatchConnectionService {

  readonly apiURL = 'https://localhost:7222/api/MatchConnections';
  userId!: number;
  constructor(private http: HttpClient,
    private authService: AuthService) {
    this.userId = this.authService.getUserId();
    }
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }),
  };

  /**
   *
   * joins the pre-game-lobby
   * @param userId id of the user joining
   * @param matchId the matches being joined id
   * @returns failed/worked
   */
  joinMatch(matchId:number):Observable<EmptyAPIResponse>
  {
    return this.http.post<EmptyAPIResponse>(
      this.apiURL + `/JoinMatch?userId=${this.userId}&matchId=${matchId}`,
      this.httpOptions
    );
  }
  /**
   *
   * leaves the pre-game-lobby
   * @param userId id of the user joining
   * @param matchId the matches being joined id
   * @returns failed/worked
   */
  leaveMatch(matchId:number)
  {
    return this.http.delete<EmptyAPIResponse>(
      this.apiURL + `/LeaveMatch?userId=${this.userId}&matchId=${matchId}`,
      this.httpOptions
    );
  }
}
