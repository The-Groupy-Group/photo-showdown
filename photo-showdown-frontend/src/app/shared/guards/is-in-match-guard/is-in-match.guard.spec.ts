import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { isInMatchGuard } from './is-in-match.guard';

describe('isInMatchGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => isInMatchGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
