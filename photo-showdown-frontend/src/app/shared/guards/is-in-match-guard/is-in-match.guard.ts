import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { MatchesService } from "src/app/matches/services/matches.service";

export const isInMatchGuard: CanActivateFn = (route, state) => {
	const matchesService = inject(MatchesService);
	const router = inject(Router);
	const previousUrl = router.routerState.snapshot.url;

	// If the user is not navigating from a match, allow the navigation
	if (!previousUrl.includes("/game")) {
		return true;
	}

	if (!window.confirm("This action will leave the match, Are you sure?")) {
		return false;
	}

	matchesService.getCurrentMatch().subscribe((response) => {
		matchesService.leaveMatch(response.data.id).subscribe();
	});

	return true;
};
