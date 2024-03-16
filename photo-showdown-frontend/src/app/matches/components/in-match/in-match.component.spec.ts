import { ComponentFixture, TestBed } from "@angular/core/testing";

import { InMatchComponent } from "./in-match.component";

describe("InMatchComponent", () => {
	let component: InMatchComponent;
	let fixture: ComponentFixture<InMatchComponent>;

	beforeEach(() => {
		TestBed.configureTestingModule({
			declarations: [InMatchComponent]
		});
		fixture = TestBed.createComponent(InMatchComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it("should create", () => {
		expect(component).toBeTruthy();
	});
});
