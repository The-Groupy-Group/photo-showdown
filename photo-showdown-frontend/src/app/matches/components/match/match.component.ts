import { Component,Input } from '@angular/core';
import { Route, Router } from '@angular/router';
import { Match } from 'src/app/matches/models/match.model';
import { MatchesService } from 'src/app/matches/services/matches.service';
import { JwtService } from 'src/app/shared/jwt-service/jwt.service';
import { MatchConnectionService } from '../../services/match-connections.service';

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
    private readonly matchConnectionService:MatchConnectionService
  ) {}

joinMatch()
{
  const token=this.jwtService.getTokenId();
  if(token!=undefined)
  {
    let userId:number=parseInt(token);
    let matchId:number=this.match.id;
    console.log(userId);
    this.matchConnectionService.joinMatch(userId,matchId).subscribe({
      next:(response)=>
      {
        this.router.navigate(['/lobby']);
      }
    });
}
}

  @Input() match!:Match;
}
