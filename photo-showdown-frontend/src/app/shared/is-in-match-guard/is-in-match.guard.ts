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
  if (!previousUrl.includes('/lobby')) {
    return true;
  }

  if (!window.confirm('Are you sure?')) {
    return false;
  }

  const userIdString = authService.getUserId();
  const userId = parseInt(userIdString!);

  const splitUrl = previousUrl.split('/');
  const matchId = parseInt(splitUrl[splitUrl.length - 1]);
  
  matchConnectionService.leaveMatch(userId, matchId).subscribe();

  return true;
};
