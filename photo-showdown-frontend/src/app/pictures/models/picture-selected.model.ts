import { UserPublicDetails } from 'src/app/users/models/user-public-details.model';
import { Picture } from './picture.model';

export interface PictureSelected extends Picture {
  numOfVotes: number;
  selectedByUser: UserPublicDetails;
  usersVoted: UserPublicDetails[];
}
