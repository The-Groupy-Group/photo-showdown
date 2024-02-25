import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InMatchVotePictureComponent } from './in-match-vote-picture.component';

describe('InMatchVotePictureComponent', () => {
  let component: InMatchVotePictureComponent;
  let fixture: ComponentFixture<InMatchVotePictureComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [InMatchVotePictureComponent]
    });
    fixture = TestBed.createComponent(InMatchVotePictureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
