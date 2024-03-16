import { Component, Input } from "@angular/core";
import { UserPublicDetails } from "src/app/users/models/user-public-details.model";

@Component({
	selector: "app-in-match-summary-screen",
	templateUrl: "./in-match-summary-screen.component.html",
	styleUrls: ["./in-match-summary-screen.component.css"]
})
export class InMatchSummaryScreenComponent {
	@Input({ required: true }) users!: UserPublicDetails[];
	@Input({ required: true }) score!: Map<number, number>;
	winnerUsername = "";

	ngOnInit() {
		this.winnerUsername = this.calculateWinner().toUpperCase();
	}
	calculateWinner(): string {
		let max = 0;
		let winner: UserPublicDetails = this.users[0];
		for (let i = 0; i < this.users.length; i++) {
			const temp = this.score.get(this.users[i].id)!;
			if (max < temp) {
				max = temp;
				winner = this.users[i];
			}
		}
		return winner.username;
	}
}
