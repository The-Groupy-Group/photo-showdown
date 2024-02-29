import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InMatchPicturesGridComponent } from './in-match-pictures-grid.component';

describe('InMatchPicturesGridComponent', () => {
  let component: InMatchPicturesGridComponent;
  let fixture: ComponentFixture<InMatchPicturesGridComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [InMatchPicturesGridComponent]
    });
    fixture = TestBed.createComponent(InMatchPicturesGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
