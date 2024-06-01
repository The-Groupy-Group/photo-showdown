export interface PictureSelected {
	id: number;
	picturePath: string;
	numOfVotes: number;
	selectedByUserId?: number;
	usersVoted: number[];
	pictureId: number;
}
