import { Component } from '@angular/core';
import {AuthService} from '../../../shared/auth-service/auth.service'
import { Router } from '@angular/router';
@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent
{
logout()
{
  this.authorizationService.logout();
  this.setStatus();
  this.router.navigate(['/login']);
}
   isLoggedIn:boolean=false;
  constructor(private readonly authorizationService:AuthService,private readonly router:Router){}
  ngOnInit():void
  {
    this.setStatus();
  }

  setStatus():void
  {
    this.isLoggedIn=this.authorizationService.isLoggedIn();
  }
}

