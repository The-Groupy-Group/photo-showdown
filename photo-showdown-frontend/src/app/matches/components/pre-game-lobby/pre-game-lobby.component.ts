import { Component, OnDestroy, OnInit, HostListener } from '@angular/core';
import { Router } from '@angular/router';
import { JwtService } from 'src/app/shared/jwt-service/jwt.service';
import { MatchConnectionService } from '../../services/match-connections.service';
import { ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { MatDialog } from '@angular/material/dialog';
import { CanComponentDeactivate } from 'src/app/shared/can-deactivate/can-deactivate.guard';
@Component({
  selector: 'app-pre-game-lobby',
  templateUrl: './pre-game-lobby.component.html',
  styleUrls: ['./pre-game-lobby.component.css']
})
export class PreGameLobbyComponent implements OnInit, CanComponentDeactivate
{

  matchId!:number;
  userId!:number;
  isLeavingMatch=false;

  constructor(
    private dialog: MatDialog,
    private readonly jwtService:JwtService,
    private readonly router:Router,
    private readonly matchConnectionService:MatchConnectionService,
    private readonly route: ActivatedRoute,
    private readonly notifier: NotifierService
    ){}

    ngOnInit()
    {
      this.isLeavingMatch = false;
      this.route.params.subscribe(params => 
      {
        this.matchId = params['matchId'];
      });
  
      const idFromToken=this.jwtService.getTokenId();
  
      if(idFromToken!=undefined)
      {
        this.userId=parseInt(idFromToken);
      }
    }

    canDeactivate()
    {
      if(this.isLeavingMatch)
      {
        return true;
      }
  
      if(!window.confirm('Leaving this page will quit the match, Are you sure?'))
      {
        return false;
      }
      
      this.matchConnectionService.leaveMatch(this.userId,this.matchId).subscribe();
      return true;
    }
  
  
    disconnect()
    {
      if(!window.confirm('Are you sure?'))
      {
        return;
      }

      this.matchConnectionService.leaveMatch(this.userId,this.matchId).subscribe(
        {
          next:()=>
          {
            this.isLeavingMatch=true;
            this.router.navigate(['/matches']);
          },
          error:(response)=>
          {
            this.notifier.notify('error',response.error.message)
          }
        })
    }
}
