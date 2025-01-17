import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { MatchesService } from "../../services/matches.service";
import { Match, MatchStates } from "../../models/match.model";
import { NotifierService } from "angular-notifier";

/**
 * A component that displays a list of matches.
 */
@Component({
	selector: "app-match-list",
	templateUrl: "./match-list.component.html",
	styleUrls: ["./match-list.component.css"]
})
export class MatchListComponent implements OnInit {
	matches: Match[] = [];
	@Output() joinedMatch = new EventEmitter<number>();

	constructor(
		private readonly matchesService: MatchesService,
		private readonly notifier: NotifierService
	) {}

	ngOnInit(): void {
		this.loadMatches();
	}

	createMatch() {
		this.matchesService.createNewMatch().subscribe({
			next: (response) => {
				this.onMatchJoined(response.data.id);
			},
			error: (response) => {
				this.notifier.notify("error", response.error.message);
			}
		});
	}

	loadMatches() {
		this.matches = [];
		this.matchesService.getAllMatches(MatchStates.notStarted).subscribe({
			next: (response) => {
				this.matches = response.data;
			}
		});
	}

	onMatchJoined(matchId: number) {
		this.joinedMatch.emit(matchId);
	}
}
