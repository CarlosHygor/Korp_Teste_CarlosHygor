import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { ProdutoListComponent } from './produto-list';
import { ProdutoService } from '../../../../services/produto.service';
import { PagedResult } from '../../../../models/paged-result.model';
import { Produto } from '../../../../models/produto.model';

describe('ProdutoListComponent', () => {
  let component: ProdutoListComponent;
  let fixture: ComponentFixture<ProdutoListComponent>;
  let produtoServiceMock: any;

  const mockPagedResult: PagedResult<Produto> = {
    itens: [
      { id: 1, codigo: 'PROD-001', descricao: 'Monitor 27', saldo: 10 },
      { id: 2, codigo: 'PROD-002', descricao: 'Teclado RGB', saldo: 0 }
    ],
    paginaAtual: 1,
    tamanhoPagina: 10,
    totalRegistros: 2,
    totalPaginas: 1,
    temPaginaAnterior: false,
    temProximaPagina: false
  };

  beforeEach(async () => {
    produtoServiceMock = {
      getPaginated: vi.fn().mockReturnValue(of(mockPagedResult)),
      delete: vi.fn().mockReturnValue(of(undefined))
    };

    await TestBed.configureTestingModule({
      imports: [ProdutoListComponent, HttpClientTestingModule, FormsModule],
      providers: [
        { provide: ProdutoService, useValue: produtoServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProdutoListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  it('deve carregar produtos paginados ao inicializar', () => {
    expect(produtoServiceMock.getPaginated).toHaveBeenCalledWith(1, 10, null, '');
    expect(component.pagedResult).toEqual(mockPagedResult);
  });

  it('deve retornar classe e texto de status de saldo corretos', () => {
    expect(component.obterTextoStatusSaldo(0)).toBe('Sem Estoque');
    expect(component.obterClasseStatusSaldo(0)).toBe('stock-zero');

    expect(component.obterTextoStatusSaldo(3)).toBe('Poucas Unidades');
    expect(component.obterClasseStatusSaldo(3)).toBe('stock-low');

    expect(component.obterTextoStatusSaldo(15)).toBe('Em Estoque');
    expect(component.obterClasseStatusSaldo(15)).toBe('stock-ok');
  });

  it('deve abrir o modal de cadastro ao clicar no botão', () => {
    component.abrirModalCadastro();
    expect(component.exibirModalForm).toBe(true);
    expect(component.produtoEmEdicao).toBeNull();
  });
});
