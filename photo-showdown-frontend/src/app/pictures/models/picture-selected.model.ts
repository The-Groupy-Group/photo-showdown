import { UserPublicDetails } from 'src/app/users/models/user-public-details.model';
import { Picture } from './picture.model';

export interface PictureSelected{
  id: number;
  picturePath: string;
  numOfVotes: number;
  selectedByUser?: UserPublicDetails;
  usersVoted: UserPublicDetails[];
  pictureId: number;
}
