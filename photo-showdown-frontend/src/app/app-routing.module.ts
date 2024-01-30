import { NgModule } from '@angular/core';
import { RouterModule, Routes, CanActivate } from '@angular/router';
import { UserLoginComponent } from './users/components/user-login/user-login.component';
import { UserRegistrationComponent } from './users/components/user-registration/user-registration.component';
import { HomepageComponent } from './homepage/homepage/homepage.component';
import { authGuard } from './shared/auth-guard/auth.guard';
import { PicturesPageComponent } from './pictures/components/pictures-page/pictures-page.component';
import { MatchListComponent } from './matches/components/match-list/match-list.component';
import { PreGameLobbyComponent } from './matches/components/pre-game-lobby/pre-game-lobby.component';
import { canDeactivateGuard } from './shared/can-deactivate/can-deactivate.guard';
import { isInMatchGuard } from './shared/is-in-match-guard/is-in-match.guard';


const routes: Routes = [
  {
    component:HomepageComponent,
    path:'',
    canActivate: [authGuard,isInMatchGuard]
  },
  {
    component:UserLoginComponent,
    path:'login',
    canActivate:[isInMatchGuard]
  },
  {
    component:PreGameLobbyComponent,
    path:'lobby/:matchId',
    canActivate: [authGuard],
  },
  {
    component:MatchListComponent,
    path:'matches',
    canActivate: [authGuard,isInMatchGuard]
  },
  {
    component:PicturesPageComponent,
    path:'pictures',
    canActivate: [authGuard,isInMatchGuard]
  },
  {
    component:UserRegistrationComponent,
    path:'register',
    canActivate:[isInMatchGuard]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
