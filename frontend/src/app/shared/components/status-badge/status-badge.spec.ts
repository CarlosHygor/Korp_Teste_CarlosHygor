import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StatusBadgeComponent } from './status-badge';

describe('StatusBadgeComponent', () => {
  let component: StatusBadgeComponent;
  let fixture: ComponentFixture<StatusBadgeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StatusBadgeComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(StatusBadgeComponent);
    component = fixture.componentInstance;
  });

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  it('deve renderizar status Aberta corretamente por padrão', () => {
    component.status = 'Aberta';
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(component.statusText).toBe('Aberta');
    expect(component.badgeClass).toBe('badge-aberta');
    expect(compiled.textContent?.trim()).toContain('Aberta');
  });

  it('deve renderizar status Fechada corretamente', () => {
    component.status = 'Fechada';
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(component.statusText).toBe('Fechada');
    expect(component.badgeClass).toBe('badge-fechada');
    expect(compiled.textContent?.trim()).toContain('Fechada');
  });

  it('deve converter número enum 1 para Fechada', () => {
    component.status = 1;
    fixture.detectChanges();

    expect(component.statusText).toBe('Fechada');
  });
});
