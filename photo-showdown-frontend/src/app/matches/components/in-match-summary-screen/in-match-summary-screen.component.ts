import { Component, Input, OnInit } from "@angular/core";
import { UserInMatch } from "src/app/users/models/user-public-details.model";

@Component({
	selector: "app-in-match-summary-screen",
	templateUrl: "./in-match-summary-screen.component.html",
	styleUrls: ["./in-match-summary-screen.component.css"]
})
export class InMatchSummaryScreenComponent implements OnInit {
	@Input({ required: true }) users!: UserInMatch[];
	winnerUsername = "";

	ngOnInit() {
		this.winnerUsername = this.calculateWinner().toUpperCase();
	}
	calculateWinner(): string {
		let maxScore = 0;
		let winner: UserInMatch = this.users[0];
		for (let i = 0; i < this.users.length; i++) {
			const currScore = this.users[i].score;
			if (maxScore < currScore) {
				maxScore = currScore;
				winner = this.users[i];
			}
		}
		return winner.username;
	}
}
