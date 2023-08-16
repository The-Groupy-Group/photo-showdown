import { Component } from '@angular/core';
import {AuthorizationService} from '../../../shared/authorization/authorization.service'
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
  constructor(private readonly authorizationService:AuthorizationService,private readonly router:Router){}
  ngOnInit():void
  {
    this.setStatus();
  }

  setStatus():void
  {
    this.isLoggedIn=this.authorizationService.isLoggedIn();
  }
}

