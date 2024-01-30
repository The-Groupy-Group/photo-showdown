import { JwtService } from './../jwt-service/jwt.service';
import { inject } from '@angular/core';
import { MatchConnectionService } from './../../matches/services/match-connections.service';
import { CanActivateFn, Router } from '@angular/router';

export const isInMatchGuard: CanActivateFn = (route, state) =>
{
  const jwtService=inject(JwtService);
  const matchConnectionService=inject(MatchConnectionService);
  const router=inject(Router);
  const previousUrl = router.routerState.snapshot.url;

 if(previousUrl.includes('/lobby'))
 {
  if(window.confirm("Are you sure?"))
  {

    const token=jwtService.getTokenId();
    if(token!=undefined)
    {
      const parsedToken=parseInt(token);
      const parts = previousUrl.split('/');
      const matchId = parseInt(parts[parts.length - 1]);
      matchConnectionService.leaveMatch(parsedToken,matchId).subscribe();
    }
  }
  else
    {
     return false;
    }
  }
  return true;
};
