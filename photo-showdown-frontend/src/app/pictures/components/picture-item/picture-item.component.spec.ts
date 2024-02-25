import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PictureItemComponent } from './picture-item.component';

describe('PictureItemComponent', () => {
  let component: PictureItemComponent;
  let fixture: ComponentFixture<PictureItemComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PictureItemComponent]
    });
    fixture = TestBed.createComponent(PictureItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
