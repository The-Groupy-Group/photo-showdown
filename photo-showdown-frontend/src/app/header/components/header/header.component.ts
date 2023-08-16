import { Component } from '@angular/core';
import {AuthorizationService} from '../../../users/services/authorization/authorization.service'
@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent
{
   isLoggedIn:boolean=false;
  constructor(private readonly authorizationService:AuthorizationService){}
  ngOnInit():void
  {
    this.setStatus();
  }

  setStatus():void
  {
    this.isLoggedIn=this.authorizationService.isLoggedIn();
  }
}

