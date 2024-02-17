export interface MatchSettings {
  matchId: number;
  sentences: string[];
  pictureSelectionTimeSeconds?: number;
  voteTimeSeconds?: number;
  numOfVotesToWin?: number;
  numOfRounds?: number;
}
