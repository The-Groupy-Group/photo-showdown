import { WebSocketMessageType } from './web-socket-message-type.enum';

export interface WebSocketMessage {
  type: WebSocketMessageType;
  data: any;
}
