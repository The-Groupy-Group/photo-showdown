import { UserPublicDetails } from 'src/app/users/models/user-public-details.model';

export interface Match {
  id: number;
  hasStarted: boolean;
  owner: UserPublicDetails;
  users: UserPublicDetails[];
}
