import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashLandComponent } from './dash-land.component';

describe('DashLandComponent', () => {
  let component: DashLandComponent;
  let fixture: ComponentFixture<DashLandComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [DashLandComponent]
    });
    fixture = TestBed.createComponent(DashLandComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
