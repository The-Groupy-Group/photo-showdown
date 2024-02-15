import { Injectable } from '@angular/core';
import {
  WebSocketSubject,
  webSocket,
  WebSocketSubjectConfig,
} from 'rxjs/webSocket';
import { Observable } from 'rxjs';
import { AuthService } from 'src/app/shared/services/auth-service/auth.service';
import { WebSocketMessage } from '../models/web-socket-message.model';
import { WebSocketMessageType } from '../models/web-socket-message.model';

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

  receiveMessages<
    T extends WebSocketMessage = WebSocketMessage
  >(): Observable<T> {
    return this.socket$.asObservable();
  }
  onWebSocketEvent<T extends WebSocketMessage = WebSocketMessage>(
    type: WebSocketMessageType,
    f: (wsMessage: T) => void
  ): void {
    this.receiveMessages<T>().subscribe((message) => {
      if (message.type === type) {
        f(message);
      }
    });
  }
}
