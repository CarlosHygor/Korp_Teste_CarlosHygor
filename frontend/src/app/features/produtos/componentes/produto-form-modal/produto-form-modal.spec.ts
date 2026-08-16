import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { ProdutoFormModalComponent } from './produto-form-modal';
import { ProdutoService } from '../../../../services/produto.service';

describe('ProdutoFormModalComponent', () => {
  let component: ProdutoFormModalComponent;
  let fixture: ComponentFixture<ProdutoFormModalComponent>;
  let produtoServiceMock: any;

  beforeEach(async () => {
    produtoServiceMock = {
      create: vi.fn(),
      update: vi.fn()
    };

    await TestBed.configureTestingModule({
      imports: [ProdutoFormModalComponent, ReactiveFormsModule, HttpClientTestingModule],
      providers: [
        { provide: ProdutoService, useValue: produtoServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProdutoFormModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  it('deve validar formulário como inválido quando campos estiverem vazios', () => {
    component.form.patchValue({ codigo: '', descricao: '', saldo: -1 });
    expect(component.form.valid).toBe(false);
  });

  it('deve chamar produtoService.create ao enviar formulário válido', () => {
    const novoProduto = { id: 1, codigo: 'PROD-001', descricao: 'Cadeira Gamer', saldo: 10 };
    produtoServiceMock.create.mockReturnValue(of(novoProduto));

    component.form.patchValue({ codigo: 'PROD-001', descricao: 'Cadeira Gamer', saldo: 10 });
    
    let produtoSalvo: any = null;
    component.aoSalvar.subscribe(p => produtoSalvo = p);

    component.salvar();

    expect(produtoServiceMock.create).toHaveBeenCalledWith({
      codigo: 'PROD-001',
      descricao: 'Cadeira Gamer',
      saldo: 10
    });
    expect(produtoSalvo).toEqual(novoProduto);
  });

  it('deve exibir mensagem de erro HTTP 409 quando o código for duplicado', () => {
    const errorResponse = { status: 409, error: { mensagem: 'Código duplicado' } };
    produtoServiceMock.create.mockReturnValue(throwError(() => errorResponse));

    component.form.patchValue({ codigo: 'PROD-DUPLICADO', descricao: 'Mouse sem fio', saldo: 5 });
    component.salvar();

    expect(component.erroApi).toContain('já está em uso no estoque');
  });
});
