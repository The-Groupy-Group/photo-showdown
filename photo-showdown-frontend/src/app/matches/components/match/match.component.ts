import { Component,Input } from '@angular/core';
import { Match } from 'src/app/matches/models/match.model';
import { MatchesService } from 'src/app/matches/services/matches.service';
@Component({
  selector: 'app-match',
  templateUrl: './match.component.html',
  styleUrls: ['./match.component.css']
})
export class MatchComponent
{
  constructor(
    private readonly matchesService: MatchesService,
  ) {}
  @Input() match!:Match;
}
