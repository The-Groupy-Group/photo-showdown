import { inject } from '@angular/core';
import {  MatchConnectionService } from './../../services/match-connections.service';
import { CanDeactivateFn, Router } from '@angular/router';
import { PreGameLobbyComponent } from '../../components/pre-game-lobby/pre-game-lobby.component';
import { Observable } from 'rxjs/internal/Observable';

export const leaveMatchGuard: CanDeactivateFn<PreGameLobbyComponent> =
(component, currentRoute, currentState, nextState): boolean | Observable<boolean> =>
{

  if(component.isLeavingMatch)
    return true;
  
    const matchConnectionService=inject(MatchConnectionService);
    matchConnectionService.leaveMatch(component.userId,component.matchId).subscribe();
    return true;


};
