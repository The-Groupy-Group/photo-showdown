// src/enums/GameState.ts

export enum GameState {
  PictureSelection = 'pictureselection',
  Voting = 'voting',
  Ended = 'ended',
  InProgress = 'inprogress',
  NotStarted = 'notstarted'
}


export const parseMatchState = (state: any): GameState => {
    if (state === 1) return GameState.InProgress;
    if (state === 2) return GameState.Ended;
    

    if (typeof state === 'string') return state.toLowerCase() as GameState;
    
    return GameState.NotStarted;
};


export const parseRoundState = (state: any): GameState => {
    if (state === 0) return GameState.PictureSelection;
    if (state === 1) return GameState.Voting;
    if (state === 2) return GameState.Ended;
    
    if (typeof state === 'string') return state.toLowerCase() as GameState;
    
    return GameState.PictureSelection; 
};