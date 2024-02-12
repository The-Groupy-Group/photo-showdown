import { Injectable } from '@angular/core';
import {
  WebSocketSubject,
  webSocket,
  WebSocketSubjectConfig,
} from 'rxjs/webSocket';
import { Observable } from 'rxjs';
import { AuthService } from 'src/app/shared/auth-service/auth.service';
import { WebSocketMessage } from '../models/web-socket-message.model';
import { WebSocketMessageType } from '../models/web-socket-message-type.enum';

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

  receiveMessages(): Observable<WebSocketMessage> {
    return this.socket$.asObservable();
  }
  onWebSocketEvent(
    type: WebSocketMessageType,
    f: (wsMessage: WebSocketMessage) => void
  ): void {
    this.receiveMessages().subscribe((message) => {
      console.log(JSON.stringify(message));
      if (message.type === type) {
        f(message);
      }
    });
  }
}
