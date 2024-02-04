import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../shared/auth-service/auth.service';
import { Router } from '@angular/router';
@Component({
  selector: 'app-user-login',
  templateUrl: './user-login.component.html',
  styleUrls: ['./user-login.component.css'],
})
export class UserLoginComponent {
  username: string = '';
  password: string = '';
  errorMessage?: string;
  isLoading = false;

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  onSubmit(form: NgForm) {
    this.username = form.value.username;
    this.password = form.value.password;

    this.isLoading = true;
    this.authService.login(this.username, this.password).subscribe({
      next: (response) => {
        this.errorMessage = undefined;
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage = error.error.message;
      },
    });
  }
}
