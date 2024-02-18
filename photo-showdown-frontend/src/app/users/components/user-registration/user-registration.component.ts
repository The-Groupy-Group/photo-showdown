import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';

import { UsersService } from '../../services/users/users.service';
import { User } from 'src/app/users/models/user.model';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from 'src/app/shared/services/auth-service/auth.service';

@Component({
  selector: 'app-user-registration',
  templateUrl: './user-registration.component.html',
  styleUrls: ['./user-registration.component.css'],
})
export class UserRegistrationComponent {
  user?: User;
  isLoading = false;
  errorMessage?: string;

  constructor(
    private readonly usersService: UsersService,
    private readonly authService: AuthService
  ) {}
  onSubmit(form: NgForm) {
    const userToCreate: User = {
      username: form.value.username,
      email: form.value.email,
      firstName: form.value.firstName,
      lastName: form.value.lastName,
      password: form.value.password,
    };

    // Create the user
    this.usersService.createUser(userToCreate).subscribe({
      next: (response) => {
        this.errorMessage = undefined;
        // Log in the user
        this.authService
          .login(userToCreate.username, userToCreate.password!)
          .subscribe({
            next: (response) => {
              this.errorMessage = undefined;
            },
            error: (error: HttpErrorResponse) => {
              this.errorMessage = error.error.message;
            },
          });
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage = error.error.message;
      },
    });
  }
}
