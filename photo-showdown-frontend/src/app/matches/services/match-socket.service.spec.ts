import { TestBed } from '@angular/core/testing';

import { MatchSocketService } from './match-socket.service';

describe('MatchSocketService', () => {
  let service: MatchSocketService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MatchSocketService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
