import { Component } from '@angular/core';
import { AuthService } from '../../../shared/services/auth-service/auth.service';
import { Router } from '@angular/router';
import { MatchesService } from 'src/app/matches/services/matches.service';
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
    private readonly matchesService: MatchesService //private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.setStatus();
  }

  logout() {
    // Leave match if user is in a match that has not started
    this.matchesService.getCurrentMatch().subscribe((response) => {
      if (response.data?.hasStarted === false) {
        this.matchesService.leaveMatch(response.data.id).subscribe(() => {
          this.authService.logout();
          window.location.reload();
        });
      }
    });

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
