import { UserPublicDetails } from "src/app/users/models/user-public-details.model";

export interface PictureSelected {
  id: number;
  picturePath: string;
  selectedByUser: UserPublicDetails;
  usersVoted: UserPublicDetails[];
}
