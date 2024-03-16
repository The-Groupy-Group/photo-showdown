import { ComponentFixture, TestBed } from "@angular/core/testing";

import { PicturesPageComponent } from "./pictures-page.component";

describe("PicturesPageComponent", () => {
	let component: PicturesPageComponent;
	let fixture: ComponentFixture<PicturesPageComponent>;

	beforeEach(() => {
		TestBed.configureTestingModule({
			declarations: [PicturesPageComponent]
		});
		fixture = TestBed.createComponent(PicturesPageComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it("should create", () => {
		expect(component).toBeTruthy();
	});
});
