export interface Round {
  matchId: number;
  roundIndex: number;
  roundState: RoundStates;
  startDate: Date;
  sentence: string;
}

/**
 * The possible states of a round
 */
export enum RoundStates {
  pictureSelection = 'pictureSelection',
  voting = 'voting',
  ended = 'ended',
}
