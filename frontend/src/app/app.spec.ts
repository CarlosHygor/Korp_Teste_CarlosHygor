import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';
import { routes } from './app.routes';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideRouter(routes)]
    }).compileComponents();
  });

  it('deve criar a aplicação principal', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('deve renderizar o título do sistema no header', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.brand-title')?.textContent).toContain('Gestão de Faturamento & Estoque');
  });

  it('deve possuir as abas de navegação para Produtos e Notas Fiscais', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    const navTabs = compiled.querySelectorAll('.nav-tab');
    expect(navTabs.length).toBe(2);
    expect(navTabs[0].textContent).toContain('Produtos');
    expect(navTabs[1].textContent).toContain('Notas Fiscais');
  });
});
