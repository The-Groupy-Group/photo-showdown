import { inject } from '@angular/core';
import { MatchConnectionService } from './../../matches/services/match-connections.service';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth-service/auth.service';

export const isInMatchGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const matchConnectionService = inject(MatchConnectionService);
  const router = inject(Router);
  const previousUrl = router.routerState.snapshot.url;

  // If the user is not navigating from a match, allow the navigation
  if (!previousUrl.includes('/game')) {
    return true;
  }

  if (!window.confirm('Are you sure?')) {
    return false;
  }

  const userId = authService.getUserId();

  const splitUrl = previousUrl.split('/');
  const matchId = parseInt(splitUrl[splitUrl.length - 1]);
  
  matchConnectionService.leaveMatch(matchId).subscribe();

  return true;
};
