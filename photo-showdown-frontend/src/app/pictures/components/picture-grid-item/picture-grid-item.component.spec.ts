import { ComponentFixture, TestBed } from "@angular/core/testing";

import { PictureGridItemComponent } from "./picture-grid-item.component";

describe("PictureGridItemComponent", () => {
	let component: PictureGridItemComponent;
	let fixture: ComponentFixture<PictureGridItemComponent>;

	beforeEach(() => {
		TestBed.configureTestingModule({
			declarations: [PictureGridItemComponent]
		});
		fixture = TestBed.createComponent(PictureGridItemComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it("should create", () => {
		expect(component).toBeTruthy();
	});
});
