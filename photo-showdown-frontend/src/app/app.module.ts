import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { MatToolbarModule } from '@angular/material/toolbar';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { UserRegistrationComponent } from './users/components/user-registration/user-registration.component';
import { UserLoginComponent } from './users/components/user-login/user-login.component';
import { AuthInterceptor } from './shared/interceptors/auth-interceptor/auth.interceptor';
import { HeaderComponent } from './header/components/header/header.component';
import { HomepageComponent } from './homepage/homepage/homepage.component';
import { PicturesPageComponent } from './pictures/components/pictures-page/pictures-page.component';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { PictureGridItemComponent } from './pictures/components/picture-grid-item/picture-grid-item.component';
import { MatchListComponent } from './matches/components/match-list/match-list.component';
import { MatchListItemComponent } from './matches/components/match-list-item/match-list-item.component';
import { NotifierModule } from 'angular-notifier';
import { PreGameLobbyComponent } from './matches/components/pre-game-lobby/pre-game-lobby.component';
import { MatDialogModule } from '@angular/material/dialog';
import { GameComponent } from './matches/components/game/game.component';
import { InMatchComponent } from './matches/components/in-match/in-match.component';
import { PictureItemComponent } from './pictures/components/picture-item/picture-item.component';

@NgModule({
  declarations: [
    AppComponent,
    UserRegistrationComponent,
    UserLoginComponent,
    HeaderComponent,
    HomepageComponent,
    PicturesPageComponent,
    PictureGridItemComponent,
    MatchListComponent,
    MatchListItemComponent,
    PreGameLobbyComponent,
    GameComponent,
    InMatchComponent,
    PictureItemComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatButtonModule,
    HttpClientModule,
    MatToolbarModule,
    MatProgressBarModule,
    NotifierModule,
    MatDialogModule,
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
