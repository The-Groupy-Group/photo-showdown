import { Component } from '@angular/core';
import { AuthService } from '../../../shared/services/auth-service/auth.service';
import { Router } from '@angular/router';
@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
})
export class HeaderComponent {
  username: string | undefined;
  isLoggedIn: boolean = false;

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.setStatus();
  }

  logout() {
    this.authService.logout();
    window.location.reload();
    // this.setStatus();
    // this.router.navigate(['/login']);
  }

  setStatus(): void {
    this.isLoggedIn = this.authService.isLoggedIn();
    this.username = this.isLoggedIn
      ? this.authService.getUsername()
      : undefined;
  }
}
