import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import {UsersService } from '../../services/users/users.service'
import { HttpErrorResponse } from '@angular/common/http';
import { AuthorizationService } from '../../../shared/authorization/authorization.service';
import { Router } from '@angular/router';
@Component({
  selector: 'app-user-login',
  templateUrl: './user-login.component.html',
  styleUrls: ['./user-login.component.css']
})

export class UserLoginComponent
{
  username:string='';
  password:string='';
  errorMessage?:string;
  constructor(private readonly authorizationService: AuthorizationService,private readonly router:Router) {}
   onSubmit(form:NgForm)
  {
      this.username=form.value.username;
      this.password=form.value.password;

      console.log(this.username)
      console.log(this.password)
      this.authorizationService.login(this.username,this.password).subscribe({
        next:(response)=>
        {
          this.errorMessage=undefined;
          console.log(response);
        this.router.navigate(['/']).then(()=>{window.location.reload();})
        },
        error:((error:HttpErrorResponse)=>
        {
          this.errorMessage=error.error.message;
        })


      })


  }
}
