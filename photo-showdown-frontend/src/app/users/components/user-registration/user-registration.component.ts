
import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';

import {UsersService } from '../../services/users.service'
import { User } from 'src/app/users/models/user.model';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-user-registration',
  templateUrl: './user-registration.component.html',
  styleUrls: ['./user-registration.component.css']

})
export class UserRegistrationComponent
{
  user?:User;
  errorMessage?:string;
  constructor(private readonly usersService: UsersService) {}
  onSubmit(form:NgForm)
  {
      const user:User=
      {
        username:form.value.username,
        email:form.value.email,
        firstName:form.value.firstName,
        lastName:form.value.lastName,
        password:form.value.password
      }

      console.log(JSON.stringify(user));

      this.usersService.createUser(user).subscribe({
        next:(response)=>
        {
          this.user=response.data;
          this.errorMessage=undefined;
          console.log(user);
        },
        error:((error:HttpErrorResponse)=>
        {
          this.errorMessage=error.error.message;
        })

      });
  }
}
