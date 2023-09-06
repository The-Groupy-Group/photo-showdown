import { Component, OnInit } from '@angular/core';
import { MatchesService } from '../../services/matches.service';
import { Match } from '../../models/match.model';


@Component({
  selector: 'app-match-list',
  templateUrl: './match-list.component.html',
  styleUrls: ['./match-list.component.css']
})
export class MatchListComponent implements OnInit
{
  matches:Match[]=[];
  constructor(private readonly matchesService: MatchesService){}


  ngOnInit(): void
  {
    this.loadMatches();
  }
  createMatch()
  {

    this.matchesService.createNewMatch().subscribe({
      next:(response)=>{
        //TODO - redicrect to matchroom instead of reload matches
        this.loadMatches();
      },
      error:(response)=>
      {
          alert(response.error.message);
      }
    })
  }
  loadMatches()
  {
    this.matches=[];
    this.matchesService.getAllOpenMatches().subscribe({
      next:(response)=>
      {
       this.matches=response.data;
      }
  })
  }
}
