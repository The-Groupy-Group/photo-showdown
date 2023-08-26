import { Component } from '@angular/core';
import { AuthService } from '../../../shared/auth-service/auth.service';
import { Router } from '@angular/router';
@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
})
export class HeaderComponent {

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
    this.setStatus();
    this.router.navigate(['/login']);
  }

  setStatus(): void {
    this.isLoggedIn = this.authService.isLoggedIn();
  }
}
