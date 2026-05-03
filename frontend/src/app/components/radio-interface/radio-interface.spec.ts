import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RadioInterface } from './radio-interface';

describe('RadioInterface', () => {
  let component: RadioInterface;
  let fixture: ComponentFixture<RadioInterface>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RadioInterface],
    }).compileComponents();

    fixture = TestBed.createComponent(RadioInterface);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
