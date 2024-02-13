export interface WebSocketMessage {
  type: WebSocketMessageType;
}

export interface PlayerJoinedWebSocketMessage extends WebSocketMessage {
  userName: string;
}

export interface PlayerLeftWebSocketMessage extends WebSocketMessage {
  userName: string;
}

export enum WebSocketMessageType {
  playerJoined = 'playerJoined',
  playerLeft = 'playerLeft',
}
