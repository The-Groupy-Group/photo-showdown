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
  errorMessage?:string;
  matches:Match[]=[];
  constructor(private readonly matchesService: MatchesService){}


  ngOnInit(): void
  {
    this.loadMatches();
  }
  createMatch()
  {
    this.errorMessage=undefined;
    this.matchesService.createNewMatch().subscribe({
      next:(response)=>{
        console.log(response)
        //TODO - redicrect to matchroom instead of reload matches
        this.loadMatches();
      },
      error:(response)=>
      {
          console.log(response);
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
      response.data.forEach(match=>{
        this.matches.push({...match});
      })
    }
  })
  }
}
