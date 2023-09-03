import { Component } from '@angular/core';
import { Match } from '../../models/match.model';

@Component({
  selector: 'app-match-list',
  templateUrl: './match-list.component.html',
  styleUrls: ['./match-list.component.css']
})
export class MatchListComponent
{
  matches:Match[]=[];
  
}
