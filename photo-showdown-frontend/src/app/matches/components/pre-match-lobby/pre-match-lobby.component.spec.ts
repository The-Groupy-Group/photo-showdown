import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PreMatchLobbyComponent } from './pre-match-lobby.component';

describe('PreMatchLobbyComponent', () => {
  let component: PreMatchLobbyComponent;
  let fixture: ComponentFixture<PreMatchLobbyComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PreMatchLobbyComponent]
    });
    fixture = TestBed.createComponent(PreMatchLobbyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
