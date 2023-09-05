import { Component, OnInit } from '@angular/core';
import { MatchesService } from '../../services/matches.service';
import { Match } from '../../models/match.model';
import { MatchConnectionService } from '../../services/match-connections.service';
import { Router } from '@angular/router';
import { JwtService } from 'src/app/shared/jwt-service/jwt.service';


@Component({
  selector: 'app-match-list',
  templateUrl: './match-list.component.html',
  styleUrls: ['./match-list.component.css']
})
export class MatchListComponent implements OnInit
{
  errorMessage?:string;
  matches:Match[]=[];
  constructor(private readonly matchesService: MatchesService,
    private readonly router:Router){}


  ngOnInit(): void
  {
    this.loadMatches();
  }
  createMatch()
  {
    this.errorMessage=undefined;
    this.matchesService.createNewMatch().subscribe({
      next:(response)=>{
        {
            this.router.navigate(['/lobby']);
      }},
      error:(response)=>
      {
          this.errorMessage=response.error.message;
      }
    })
  }
  loadMatches()
  {
    this.matches=[];
    this.matchesService.getAllOpenMatches().subscribe({

      next:(response)=>{
        console.log(response);
       this.matches=response.data;
    }
  })
  }
}
