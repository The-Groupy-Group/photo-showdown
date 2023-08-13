import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';

import { User } from 'src/app/models/user.model';

@Component({
  selector: 'app-user-registration',
  templateUrl: './user-registration.component.html',
  styleUrls: ['./user-registration.component.css']

})
export class UserRegistrationComponent
{
  onSubmit(form:NgForm)
  {
      const user:User=
      {
        username:form.value.userName,
        email:form.value.email,
        firstName:form.value.firstName,
        lastName:form.value.lastName,
        password:form.value.password
      }
      console.log(JSON.stringify(user))
  }


}
