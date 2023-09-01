import { NgModule } from '@angular/core';
import { RouterModule, Routes, CanActivate } from '@angular/router';
import { UserLoginComponent } from './users/components/user-login/user-login.component';
import { UserRegistrationComponent } from './users/components/user-registration/user-registration.component';
import { HomepageComponent } from './homepage/homepage/homepage.component';
import { authGuard } from './shared/auth-guard/auth.guard';
import { PicturesPageComponent } from './pictures/components/pictures-page/pictures-page.component';


const routes: Routes = [
  {
  component:HomepageComponent,
  path:'',
  canActivate: [authGuard]
  },
  {
    component:UserLoginComponent,
    path:'login'
  },
  {
    component:PicturesPageComponent,
    path:'pictures',
    canActivate: [authGuard]
  },
  {
    component:UserRegistrationComponent,
    path:'register'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
