import { UserInMatch } from "src/app/users/models/user-public-details.model";
import { Round } from "./round.model";

export interface Match {
	id: number;
	matchState: MatchStates;
	owner: UserInMatch;
	users: UserInMatch[];
	currentRound?: Round;
}

export enum MatchStates {
	notStarted = "notStarted",
	inProgress = "inProgress",
	ended = "ended"
}
