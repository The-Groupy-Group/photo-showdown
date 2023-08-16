import { CanActivateFn, Router } from '@angular/router';
import { AuthorizationService } from '../authorization/authorization.service';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) =>
{
  const authService=inject(AuthorizationService);
  const router=inject(Router);
  if (!authService.isLoggedIn())
  {
    router.navigate(['/login']);
    return (false);
  }
  
  return (true);
};
