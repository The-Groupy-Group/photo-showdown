import { UserPublicDetails } from "src/app/users/models/user-public-details.model";

export interface PictureSelected {
  id: number;
  picturePath: string;
  numOfVotes: number;
  selectedByUser: UserPublicDetails;
  usersVoted: UserPublicDetails[];
}
