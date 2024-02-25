import { UserPublicDetails } from 'src/app/users/models/user-public-details.model';

export interface Match {
  id: number;
  matchState: MatchStates;
  owner: UserPublicDetails;
  users: UserPublicDetails[];
}

export enum MatchStates {
  notStarted = 'notStarted',
  inProgress = 'inProgress',
  ended = 'ended',
}
