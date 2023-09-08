import { Component,Input } from '@angular/core';
import { Route, Router } from '@angular/router';
import { Match } from 'src/app/matches/models/match.model';
import { MatchesService } from 'src/app/matches/services/matches.service';
import { JwtService } from 'src/app/shared/jwt-service/jwt.service';
import { MatchConnectionService } from '../../services/match-connections.service';
import { NotifierService } from 'angular-notifier';

@Component({
  selector: 'app-match',
  templateUrl: './match.component.html',
  styleUrls: ['./match.component.css']
})
export class MatchComponent
{
  constructor
  (
    private readonly jwtService:JwtService,
    private readonly router:Router,
    private readonly matchConnectionService:MatchConnectionService,
    private readonly notifier:NotifierService
  ) {}

joinMatch()
{
  const token=this.jwtService.getTokenId();
  if(token!=undefined)
  {
    let userId:number=parseInt(token);
    let matchId:number=this.match.id;
    this.matchConnectionService.joinMatch(userId,matchId).subscribe({
      next:(response)=>
      {
        this.router.navigate(['/lobby/'+this.match.id]);
      },
      error:(response)=>
      {
        this.notifier.notify('error',response.error.message)
      }
    });
}
}

  @Input() match!:Match;
}
