import { TestBed } from '@angular/core/testing';

import { MatchConnectionsService } from './match-connections.service';

describe('MatchConnectionsService', () => {
  let service: MatchConnectionsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MatchConnectionsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
