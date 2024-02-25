import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
  HttpStatusCode,
} from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { environment } from 'src/environments/environment';
import { AuthService } from '../../services/auth-service/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly notifier: NotifierService
  ) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const idToken = this.authService.getJwtToken();
    if (idToken) {
      request = request.clone({
        headers: request.headers.set('Authorization', 'Bearer ' + idToken),
      });
    }

    // Handle the request
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === HttpStatusCode.Unauthorized) {
          // Redirect to login page
          this.authService.logout();
          this.router.navigate(['/login']);
        } else if (error.status === HttpStatusCode.InternalServerError) {
          this.notifier.notify('error', error.error.message);
        }
        return throwError(() => error);
      })
    );
  }
}
