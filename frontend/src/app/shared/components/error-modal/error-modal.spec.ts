import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ErrorModalComponent, ErroDetalhado } from './error-modal';

describe('ErrorModalComponent', () => {
  let component: ErrorModalComponent;
  let fixture: ComponentFixture<ErrorModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ErrorModalComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(ErrorModalComponent);
    component = fixture.componentInstance;
  });

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  it('não deve exibir modal se erro for nulo', () => {
    component.erro = null;
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.modal-backdrop')).toBeNull();
  });

  it('deve exibir mensagem de erro e fechar ao clicar no botão', () => {
    const mockErro: ErroDetalhado = {
      titulo: 'Erro de Validação',
      mensagem: 'Mensagem de teste'
    };

    component.erro = mockErro;
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h3')?.textContent).toBe('Erro de Validação');
    expect(compiled.querySelector('.mensagem-principal')?.textContent).toBe('Mensagem de teste');

    let emitido = false;
    component.aoFechar.subscribe(() => emitido = true);

    const button = compiled.querySelector('button') as HTMLButtonElement;
    button.click();

    expect(emitido).toBe(true);
    expect(component.erro).toBeNull();
  });

  it('deve renderizar detalhes de estoque insuficiente quando fornecidos', () => {
    component.erro = {
      mensagem: 'Estoque insuficiente',
      codigoProduto: 'PROD-123',
      saldoAtual: 3,
      quantidadeSolicitada: 10
    };
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.valor-codigo')?.textContent).toBe('PROD-123');
    expect(compiled.querySelector('.valor-disponivel')?.textContent).toContain('3 un');
    expect(compiled.querySelector('.valor-solicitado')?.textContent).toContain('10 un');
  });
});
