import { UserPublicDetails } from 'src/app/users/models/user-public-details.model';

/**
 * This interface represents a message sent over a WebSocket connection.
 */
export interface EmptyWebSocketMessage {
  type: WebSocketMessageType;
}

/**
 * This interface represents a message sent over a WebSocket connection.
 */
export interface WebSocketMessage<T> extends EmptyWebSocketMessage {
  data: T;
}

export enum WebSocketMessageType {
  playerJoined = 'playerJoined',
  playerLeft = 'playerLeft',
  newOwner = 'newOwner',
}
