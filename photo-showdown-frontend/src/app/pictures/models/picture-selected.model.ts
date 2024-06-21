import { UserInMatch } from "../../users/models/user-public-details.model";

export interface PictureSelected {
	id: number;
	picturePath: string;
	numOfVotes: number;
	selectedByUserId?: number;
	selectedByUser?: UserInMatch;
	usersVoted: number[];
	pictureId: number;
}
