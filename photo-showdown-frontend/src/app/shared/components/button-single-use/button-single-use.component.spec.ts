import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ButtonSingleUseComponent } from './button-single-use.component';

describe('ButtonSingleUseComponent', () => {
  let component: ButtonSingleUseComponent;
  let fixture: ComponentFixture<ButtonSingleUseComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ButtonSingleUseComponent]
    });
    fixture = TestBed.createComponent(ButtonSingleUseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
