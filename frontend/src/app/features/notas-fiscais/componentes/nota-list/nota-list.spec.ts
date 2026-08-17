import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { NotaFiscalListComponent } from './nota-list';
import { NotaFiscalService } from '../../../../services/nota-fiscal.service';
import { ProdutoService } from '../../../../services/produto.service';
import { PagedResult } from '../../../../models/paged-result.model';
import { NotaFiscal } from '../../../../models/nota-fiscal.model';

describe('NotaFiscalListComponent', () => {
  let component: NotaFiscalListComponent;
  let fixture: ComponentFixture<NotaFiscalListComponent>;
  let notaFiscalServiceMock: any;
  let produtoServiceMock: any;

  const mockPagedResult: PagedResult<NotaFiscal> = {
    itens: [
      {
        id: 1,
        numeracao: 1,
        status: 'Aberta',
        dataCriacao: '2026-08-17T10:00:00Z',
        itens: [{ id: 10, codigoProduto: 'PROD-001', descricaoProduto: 'Teclado', quantidade: 2 }]
      },
      {
        id: 2,
        numeracao: 2,
        status: 'Fechada',
        dataCriacao: '2026-08-17T11:00:00Z',
        itens: [{ id: 11, codigoProduto: 'PROD-002', descricaoProduto: 'Mouse', quantidade: 5 }]
      }
    ],
    paginaAtual: 1,
    tamanhoPagina: 10,
    totalRegistros: 2,
    totalPaginas: 1,
    temPaginaAnterior: false,
    temProximaPagina: false
  };

  beforeEach(async () => {
    notaFiscalServiceMock = {
      getPaginated: vi.fn().mockReturnValue(of(mockPagedResult)),
      imprimir: vi.fn().mockReturnValue(of({ ...mockPagedResult.itens[0], status: 'Fechada' })),
      delete: vi.fn().mockReturnValue(of(undefined)),
      create: vi.fn().mockReturnValue(of(mockPagedResult.itens[0]))
    };

    produtoServiceMock = {
      getPaginated: vi.fn().mockReturnValue(of({ itens: [] }))
    };

    await TestBed.configureTestingModule({
      imports: [NotaFiscalListComponent, HttpClientTestingModule, FormsModule],
      providers: [
        { provide: NotaFiscalService, useValue: notaFiscalServiceMock },
        { provide: ProdutoService, useValue: produtoServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(NotaFiscalListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  it('deve carregar notas fiscais paginadas ao inicializar', () => {
    expect(notaFiscalServiceMock.getPaginated).toHaveBeenCalledWith(1, 10, undefined, 'data_desc');
    expect(component.pagedResult).toEqual(mockPagedResult);
    expect(component.notasView.length).toBe(2);
  });

  it('deve formatar a numeração com 4 dígitos', () => {
    expect(component.formatarNumeracao(1)).toBe('0001');
    expect(component.formatarNumeracao(42)).toBe('0042');
  });

  it('deve alternar a expansão da linha de itens', () => {
    const nota = component.notasView[0];
    expect(nota.expandida).toBe(false);
    component.toggleExpansao(nota);
    expect(nota.expandida).toBe(true);
  });

  it('deve filtrar por status ao clicar na aba', () => {
    component.alterarFiltroStatus('Aberta');
    expect(component.statusFiltro).toBe('Aberta');
    expect(notaFiscalServiceMock.getPaginated).toHaveBeenCalledWith(1, 10, 'Aberta', 'data_desc');
  });
});
