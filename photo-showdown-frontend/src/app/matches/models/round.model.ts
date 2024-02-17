export interface Round {
  matchId: number;
  roundIndex: number;
  roundState: roundStates;
  startDate: Date;
  sentence: string;
}

/**
 * The possible states of a round
 */
export enum roundStates {
  notStarted = 'notStarted',
  pictureSelection = 'pictureSelection',
  voting = 'voting',
  ended = 'ended',
}
