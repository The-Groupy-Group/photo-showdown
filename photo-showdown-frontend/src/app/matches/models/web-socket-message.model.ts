import { UserInMatch } from "src/app/users/models/user-public-details.model";
import { Round } from "./round.model";

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

export interface PlayerJoinedWSMessage extends WebSocketMessage<UserInMatch> {
	type: WebSocketMessageType.playerJoined;
}

export interface PlayerLeftWSMessage extends WebSocketMessage<UserInMatch> {
	type: WebSocketMessageType.playerLeft;
}

export interface NewOwnerWSMessage extends WebSocketMessage<UserInMatch> {
	type: WebSocketMessageType.newOwner;
}

export interface MatchStartedWSMessage extends EmptyWebSocketMessage {
	type: WebSocketMessageType.matchStarted;
}

export interface RoundStateChangeWSMessage extends WebSocketMessage<Round> {
	type: WebSocketMessageType.roundStateChange;
}

export interface UserLockedInWSMessage extends WebSocketMessage<number> {
	type: WebSocketMessageType.userLockedIn;
}
export interface MatchEndedWSMessage extends EmptyWebSocketMessage {
	type: WebSocketMessageType.matchEnded;
}

/**
 * This enum represents the different types of messages that can be sent over a WebSocket connection.
 */
export enum WebSocketMessageType {
	playerJoined = "playerJoined",
	playerLeft = "playerLeft",
	newOwner = "newOwner",
	matchStarted = "matchStarted",
	roundStateChange = "roundStateChange",
	userLockedIn = "userLockedIn",
	matchEnded = "matchEnded"
}
