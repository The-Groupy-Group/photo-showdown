import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserLoginComponent } from './users/components/user-login/user-login.component';
import { UserRegistrationComponent } from './users/components/user-registration/user-registration.component';

const routes: Routes = [
  {
    component:UserLoginComponent,
    path:'login'
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
