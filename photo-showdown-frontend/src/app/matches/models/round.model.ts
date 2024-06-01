import { PictureSelected } from "src/app/pictures/models/picture-selected.model";

export interface Round {
	matchId: number;
	roundIndex: number;
	roundState: RoundStates;
	sentence: string;
	picturesSelected: PictureSelected[];
	roundWinnerId?: number;
	startDate: Date;
	pictureSelectionEndDate: Date;
	votingEndDate: Date;
	roundEndDate: Date;
}

/**
 * The possible states of a round
 */
export enum RoundStates {
	pictureSelection = "pictureSelection",
	voting = "voting",
	ended = "ended"
}
