import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GameMainScreenComponent } from './game-main-screen.component';

describe('GameMainScreenComponent', () => {
  let component: GameMainScreenComponent;
  let fixture: ComponentFixture<GameMainScreenComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [GameMainScreenComponent]
    });
    fixture = TestBed.createComponent(GameMainScreenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
