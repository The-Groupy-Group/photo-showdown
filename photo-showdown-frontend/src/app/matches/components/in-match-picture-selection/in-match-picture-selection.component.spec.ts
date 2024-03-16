import { ComponentFixture, TestBed } from "@angular/core/testing";

import { InMatchPictureSelectionComponent } from "./in-match-picture-selection.component";

describe("InMatchPictureSelectionComponent", () => {
	let component: InMatchPictureSelectionComponent;
	let fixture: ComponentFixture<InMatchPictureSelectionComponent>;

	beforeEach(() => {
		TestBed.configureTestingModule({
			declarations: [InMatchPictureSelectionComponent]
		});
		fixture = TestBed.createComponent(InMatchPictureSelectionComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it("should create", () => {
		expect(component).toBeTruthy();
	});
});
