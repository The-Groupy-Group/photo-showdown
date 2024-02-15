import { UserPublicDetails } from "src/app/users/models/user-public-details.model";

export interface WebSocketMessage {
  type: WebSocketMessageType;
}

export interface PlayerJoinedWebSocketMessage extends WebSocketMessage {
  user: UserPublicDetails;
}

export interface PlayerLeftWebSocketMessage extends WebSocketMessage {
  user: UserPublicDetails;
}

export interface NewOwnerWebSocketMessage extends WebSocketMessage {
  user: UserPublicDetails;
}

export enum WebSocketMessageType {
  playerJoined = 'playerJoined',
  playerLeft = 'playerLeft',
  newOwner = 'newOwner',
}
