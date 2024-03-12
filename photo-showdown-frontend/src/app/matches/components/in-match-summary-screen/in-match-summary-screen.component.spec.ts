import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InMatchSummaryScreenComponent } from './in-match-summary-screen.component';

describe('InMatchSummaryScreenComponent', () => {
  let component: InMatchSummaryScreenComponent;
  let fixture: ComponentFixture<InMatchSummaryScreenComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [InMatchSummaryScreenComponent]
    });
    fixture = TestBed.createComponent(InMatchSummaryScreenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
