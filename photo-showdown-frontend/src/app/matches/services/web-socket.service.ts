import { Injectable } from '@angular/core';
import { WebSocketSubject, webSocket, WebSocketSubjectConfig } from 'rxjs/webSocket';
import { Observable } from 'rxjs';
import { AuthService } from 'src/app/shared/auth-service/auth.service';

@Injectable({
  providedIn: 'root',
})
export class WebSocketService {
  private socket$: WebSocketSubject<any>;
  readonly wsURL = 'wss://localhost:7222';
  constructor(readonly authService: AuthService) {
    let token = authService.getJwtToken();
    this.socket$ = new WebSocketSubject(this.wsURL + '?jwt=' + token);
  }

  sendMessage(message: any): void {
    this.socket$.next(message);
  }

  receiveMessages(): Observable<any|string> {
    return this.socket$.asObservable();
  }
}
