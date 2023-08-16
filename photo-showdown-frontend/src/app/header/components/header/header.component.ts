import { Component } from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent
{
  state:string='logged-out';
  ngOnInit():void
  {
    if(localStorage.getItem('id_token'))
      this.state='logged-in';
    else
      this.state='logged-out';
  }
}
