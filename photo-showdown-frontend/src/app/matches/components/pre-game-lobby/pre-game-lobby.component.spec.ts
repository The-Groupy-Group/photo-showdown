import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PreGameLobbyComponent } from './pre-game-lobby.component';

describe('PreGameLobbyComponent', () => {
  let component: PreGameLobbyComponent;
  let fixture: ComponentFixture<PreGameLobbyComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PreGameLobbyComponent]
    });
    fixture = TestBed.createComponent(PreGameLobbyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
