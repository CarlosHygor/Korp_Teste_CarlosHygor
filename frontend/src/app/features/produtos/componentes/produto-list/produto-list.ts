import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ProdutoService } from '../../../../services/produto.service';
import { Produto } from '../../../../models/produto.model';
import { PagedResult } from '../../../../models/paged-result.model';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner';
import { ProdutoFormModalComponent } from '../produto-form-modal/produto-form-modal';
import { ErrorModalComponent, ErroDetalhado } from '../../../../shared/components/error-modal/error-modal';
import { SuccessModalComponent, MensagemSucesso } from '../../../../shared/components/success-modal/success-modal';

@Component({
  selector: 'app-produto-list',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    LoadingSpinnerComponent, 
    ProdutoFormModalComponent, 
    ErrorModalComponent,
    SuccessModalComponent
  ],
  template: `
    <!-- Modal de Cadastro / Edição -->
    <app-produto-form-modal
      *ngIf="exibirModalForm"
      [produtoEdicao]="produtoEmEdicao"
      (aoFechar)="fecharModalForm()"
      (aoSalvar)="aoSalvarProduto($event)"
    ></app-produto-form-modal>

    <!-- Modal de Erro Rico -->
    <app-error-modal
      [erro]="erroDetalhado"
      (aoFechar)="erroDetalhado = null"
    ></app-error-modal>

    <!-- Modal de Confirmação de Exclusão de Produto -->
    <div class="confirm-overlay" *ngIf="produtoParaExcluir" (click)="produtoParaExcluir = null">
      <div class="confirm-dialog" (click)="$event.stopPropagation()">
        <div class="confirm-icon">⚠️</div>
        <h3>Confirmar Exclusão</h3>
        <p>Deseja realmente remover o produto <strong>'{{ produtoParaExcluir.codigo }}'</strong> ({{ produtoParaExcluir.descricao }}) do estoque?</p>
        <div class="confirm-actions">
          <button class="btn-cancel" (click)="produtoParaExcluir = null">Cancelar</button>
          <button class="btn-confirm-delete" (click)="executarExclusaoProduto()">Sim, Excluir Produto</button>
        </div>
      </div>
    </div>

    <!-- Modal de Sucesso -->
    <app-success-modal
      [dados]="mensagemSucessoModal"
      (aoFechar)="mensagemSucessoModal = null"
    ></app-success-modal>

    <!-- Header da Página -->
    <div class="page-header">
      <div class="page-title">
        <h2>📦 Gerenciamento de Produtos (Estoque)</h2>
        <p>Cadastre novos itens e acompanhe a disponibilidade física em tempo real.</p>
      </div>

      <button class="btn btn-primary" (click)="abrirModalCadastro()">
        <svg class="btn-svg" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
          <line x1="12" y1="5" x2="12" y2="19"></line>
          <line x1="5" y1="12" x2="19" y2="12"></line>
        </svg>
        Cadastrar Produto
      </button>
    </div>

    <!-- Barra de Ferramentas: Busca e Ordenação -->
    <div class="toolbar-bar">
      <div class="search-box">
        <svg class="search-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <circle cx="11" cy="11" r="8"></circle>
          <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
        </svg>
        <input 
          type="text" 
          [ngModel]="termoBusca" 
          (ngModelChange)="aoDigitarBusca($event)" 
          placeholder="Buscar código ou descrição..." 
          class="input-search"
        />
      </div>

      <div class="sort-selector">
        <label for="sortSaldo">Ordenar Saldo:</label>
        <select id="sortSaldo" [ngModel]="ordenarPorSaldo" (ngModelChange)="alterarOrdenacaoSaldo($event)" class="select-sort">
          <option [value]="null">Padrão (ID)</option>
          <option value="asc">Menor Saldo Primeiro (▲)</option>
          <option value="desc">Maior Saldo Primeiro (▼)</option>
        </select>
      </div>
    </div>

    <!-- Tabela de Produtos / Card Principal -->
    <div class="card-container">
      <app-loading-spinner *ngIf="carregando" [isOverlay]="true" mensagem="Carregando produtos do estoque..."></app-loading-spinner>

      <div class="table-responsive" *ngIf="!carregando && pagedResult">
        <table class="data-table" *ngIf="pagedResult.itens && pagedResult.itens.length > 0">
          <thead>
            <tr>
              <th>Código</th>
              <th>Descrição do Produto</th>
              <th class="text-right">Saldo em Estoque</th>
              <th>Status do Saldo</th>
              <th class="text-right">Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let p of pagedResult.itens">
              <td class="td-codigo"><code>{{ p.codigo }}</code></td>
              <td class="td-descricao">{{ p.descricao }}</td>
              <td class="td-saldo text-right"><strong>{{ p.saldo }}</strong> un</td>
              <td>
                <span class="stock-badge" [ngClass]="obterClasseStatusSaldo(p.saldo)">
                  <span class="dot"></span>
                  {{ obterTextoStatusSaldo(p.saldo) }}
                </span>
              </td>
              <td class="text-right">
                <div class="actions-cell">
                  <button class="btn-action btn-edit" title="Editar Produto" (click)="abrirModalEdicao(p)">
                    <svg class="action-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                      <path d="M17 3a2.828 2.828 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z"/>
                    </svg>
                    <span>Editar</span>
                  </button>
                  <button class="btn-action btn-delete" title="Excluir Produto" (click)="confirmarExclusao(p)">
                    <svg class="action-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                      <polyline points="3 6 5 6 21 6"></polyline>
                      <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                    </svg>
                    <span>Excluir</span>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- Empty State -->
        <div class="empty-state" *ngIf="!pagedResult.itens || pagedResult.itens.length === 0">
          <div class="empty-icon">📦</div>
          <h4>Nenhum produto cadastrado no estoque</h4>
          <p>Clique no botão acima para realizar o primeiro cadastro de produto.</p>
        </div>
      </div>

      <!-- Controles de Paginação -->
      <div class="pagination-footer" *ngIf="pagedResult && pagedResult.totalRegistros > 0">
        <div class="pagination-info">
          Exibindo {{ pagedResult.itens.length }} de {{ pagedResult.totalRegistros }} produtos cadastrados
        </div>

        <div class="pagination-controls">
          <div class="page-size-selector">
            <label>Itens por página:</label>
            <select [ngModel]="tamanhoPagina" (ngModelChange)="alterarTamanhoPagina($event)" class="select-size">
              <option [value]="5">5</option>
              <option [value]="10">10</option>
              <option [value]="20">20</option>
            </select>
          </div>

          <div class="page-buttons">
            <button 
              class="btn-page" 
              [disabled]="!pagedResult.temPaginaAnterior || carregando"
              (click)="mudarPagina(paginaAtual - 1)">
              ◀ Anterior
            </button>

            <span class="page-indicator">Página <strong>{{ pagedResult.paginaAtual }}</strong> de <strong>{{ pagedResult.totalPaginas }}</strong></span>

            <button 
              class="btn-page" 
              [disabled]="!pagedResult.temProximaPagina || carregando"
              (click)="mudarPagina(paginaAtual + 1)">
              Próxima ▶
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.5rem;
      gap: 1rem;
      flex-wrap: wrap;
    }

    .page-title h2 {
      margin: 0 0 0.25rem 0;
      color: #f8fafc;
      font-size: 1.5rem;
      font-weight: 700;
    }

    .page-title p {
      margin: 0;
      color: #94a3b8;
      font-size: 0.875rem;
    }

    .toolbar-bar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.25rem;
      gap: 1rem;
      flex-wrap: wrap;
    }

    .search-box {
      position: relative;
      display: flex;
      align-items: center;
    }

    .search-icon {
      position: absolute;
      left: 0.75rem;
      width: 1rem;
      height: 1rem;
      color: #94a3b8;
      pointer-events: none;
    }

    .input-search {
      background: #0f1c23;
      color: #f8fafc;
      border: 1px solid #294656;
      border-radius: 0.5rem;
      padding: 0.5rem 0.75rem 0.5rem 2.25rem;
      font-size: 0.875rem;
      outline: none;
      width: 250px;
      transition: all 0.15s;
    }

    .input-search:focus {
      border-color: #38bdf8;
      box-shadow: 0 0 0 2px rgba(56, 189, 248, 0.2);
    }

    .sort-selector {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      font-size: 0.875rem;
      color: #94a3b8;
      background: #1a2f3a;
      padding: 0.375rem 0.75rem;
      border-radius: 0.5rem;
      border: 1px solid #294656;
    }

    .select-sort {
      background: #0f1c23;
      color: #38bdf8;
      border: 1px solid #294656;
      border-radius: 0.375rem;
      padding: 0.375rem 0.625rem;
      font-size: 0.875rem;
      font-weight: 600;
      outline: none;
      cursor: pointer;
    }

    .select-sort:focus {
      border-color: #38bdf8;
    }

    .btn-svg {
      width: 1.125rem;
      height: 1.125rem;
    }

    .card-container {
      background: #1a2f3a;
      border: 1px solid #294656;
      border-radius: 0.875rem;
      position: relative;
      min-height: 320px;
      box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.3);
      overflow: hidden;
    }

    .table-responsive {
      width: 100%;
      overflow-x: auto;
    }

    .data-table {
      width: 100%;
      border-collapse: collapse;
      text-align: left;
      font-size: 0.9375rem;
    }

    .data-table th {
      background: #162832;
      color: #94a3b8;
      padding: 1rem 1.25rem;
      font-weight: 700;
      font-size: 0.75rem;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      border-bottom: 1px solid #294656;
    }

    .data-table td {
      padding: 1rem 1.25rem;
      border-bottom: 1px solid rgba(41, 70, 86, 0.5);
      color: #f8fafc;
    }

    .data-table tbody tr {
      transition: background-color 0.2s ease;
    }

    .data-table tbody tr:hover {
      background-color: rgba(56, 189, 248, 0.04);
    }

    .td-codigo code {
      background: #0f1c23;
      color: #38bdf8;
      padding: 0.25rem 0.5rem;
      border-radius: 0.375rem;
      font-family: monospace;
      font-size: 0.875rem;
      border: 1px solid #294656;
    }

    .td-descricao {
      font-weight: 600;
    }

    .td-saldo strong {
      font-size: 1.05rem;
    }

    .stock-badge {
      display: inline-flex;
      align-items: center;
      gap: 0.375rem;
      padding: 0.25rem 0.625rem;
      border-radius: 9999px;
      font-size: 0.75rem;
      font-weight: 700;
    }

    .stock-badge .dot {
      width: 0.375rem;
      height: 0.375rem;
      border-radius: 50%;
    }

    .stock-ok {
      background: rgba(16, 185, 129, 0.15);
      color: #34d399;
      border: 1px solid rgba(16, 185, 129, 0.3);
    }
    .stock-ok .dot { background: #10b981; }

    .stock-low {
      background: rgba(245, 158, 11, 0.15);
      color: #fbbf24;
      border: 1px solid rgba(245, 158, 11, 0.3);
    }
    .stock-low .dot { background: #f59e0b; }

    .stock-zero {
      background: rgba(244, 63, 94, 0.15);
      color: #fb7185;
      border: 1px solid rgba(244, 63, 94, 0.3);
    }
    .stock-zero .dot { background: #f43f5e; }

    .text-right { text-align: right; }

    .actions-cell {
      display: flex;
      justify-content: flex-end;
      gap: 0.5rem;
    }

    .btn-action {
      display: inline-flex;
      align-items: center;
      gap: 0.375rem;
      background: #0f1c23;
      border: 1px solid #294656;
      border-radius: 0.375rem;
      padding: 0.375rem 0.625rem;
      cursor: pointer;
      font-size: 0.8125rem;
      font-weight: 600;
      color: #cbd5e1;
      transition: all 0.2s;
    }

    .action-icon {
      width: 0.875rem;
      height: 0.875rem;
    }

    .btn-edit:hover {
      background: rgba(56, 189, 248, 0.15);
      border-color: rgba(56, 189, 248, 0.4);
      color: #38bdf8;
    }

    .btn-delete:hover {
      background: rgba(244, 63, 94, 0.15);
      border-color: rgba(244, 63, 94, 0.4);
      color: #fb7185;
    }

    .empty-state {
      padding: 4rem 2rem;
      text-align: center;
    }

    .empty-icon {
      font-size: 3rem;
      margin-bottom: 1rem;
    }

    .empty-state h4 {
      color: #f8fafc;
      font-size: 1.125rem;
      margin-bottom: 0.5rem;
    }

    .empty-state p {
      color: #94a3b8;
      font-size: 0.875rem;
    }

    /* Rodapé da Paginação */
    .pagination-footer {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1rem 1.5rem;
      background: #162832;
      border-top: 1px solid #294656;
      flex-wrap: wrap;
      gap: 1rem;
    }

    .pagination-info {
      color: #94a3b8;
      font-size: 0.875rem;
    }

    .pagination-controls {
      display: flex;
      align-items: center;
      gap: 1.5rem;
    }

    .page-size-selector {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      font-size: 0.875rem;
      color: #94a3b8;
    }

    .select-size {
      background: #0f1c23;
      color: #f8fafc;
      border: 1px solid #294656;
      border-radius: 0.375rem;
      padding: 0.25rem 0.5rem;
      font-size: 0.875rem;
      outline: none;
    }

    .page-buttons {
      display: flex;
      align-items: center;
      gap: 0.75rem;
    }

    .btn-page {
      background: #0f1c23;
      color: #f8fafc;
      border: 1px solid #294656;
      padding: 0.375rem 0.875rem;
      border-radius: 0.375rem;
      font-size: 0.875rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.2s;
    }

    .btn-page:hover:not(:disabled) {
      background: #243f4e;
      border-color: #38bdf8;
    }

    .btn-page:disabled {
      opacity: 0.4;
      cursor: not-allowed;
    }

    .page-indicator {
      font-size: 0.875rem;
      color: #94a3b8;
    }

    .page-indicator strong {
      color: #f8fafc;
    }

    .confirm-overlay {
      position: fixed;
      top: 0;
      left: 0;
      width: 100vw;
      height: 100vh;
      background: rgba(10, 18, 23, 0.85);
      backdrop-filter: blur(4px);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 9999;
      padding: 1rem;
    }

    .confirm-dialog {
      background: #172832;
      border: 1px solid #e11d48;
      border-radius: 0.75rem;
      padding: 1.75rem;
      max-width: 480px;
      width: 100%;
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.5);
      text-align: center;
    }

    .confirm-icon {
      font-size: 2.5rem;
      margin-bottom: 0.5rem;
    }

    .confirm-dialog h3 {
      margin: 0 0 0.5rem 0;
      color: #f8fafc;
      font-size: 1.25rem;
    }

    .confirm-dialog p {
      color: #cbd5e1;
      font-size: 0.95rem;
      line-height: 1.5;
      margin-bottom: 1.5rem;
    }

    .confirm-actions {
      display: flex;
      justify-content: flex-end;
      gap: 0.75rem;
    }

    .btn-cancel {
      background: #0f1c23;
      color: #94a3b8;
      border: 1px solid #294656;
      padding: 0.5rem 1rem;
      border-radius: 0.375rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.2s;
    }

    .btn-cancel:hover {
      background: #243f4e;
      color: #fff;
    }

    .btn-confirm-delete {
      background: #e11d48;
      color: #fff;
      border: none;
      padding: 0.5rem 1.25rem;
      border-radius: 0.375rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.2s;
    }

    .btn-confirm-delete:hover {
      background: #be123c;
    }
  `]
})
export class ProdutoListComponent implements OnInit {
  pagedResult: PagedResult<Produto> | null = null;
  carregando: boolean = false;
  paginaAtual: number = 1;
  tamanhoPagina: number = 10;

  exibirModalForm: boolean = false;
  produtoEmEdicao: Produto | null = null;

  erroDetalhado: ErroDetalhado | null = null;
  mensagemSucessoModal: MensagemSucesso | null = null;

  ordenarPorSaldo: 'asc' | 'desc' | null = null;
  termoBusca: string = '';
  private readonly termoBuscaSubject = new Subject<string>();

  constructor(
    private readonly produtoService: ProdutoService,
    private readonly cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.carregarProdutos();

    this.termoBuscaSubject.pipe(
      debounceTime(350),
      distinctUntilChanged()
    ).subscribe((termo) => {
      this.termoBusca = termo;
      this.paginaAtual = 1;
      this.carregarProdutos();
    });
  }

  aoDigitarBusca(termo: string): void {
    this.termoBuscaSubject.next(termo);
  }

  carregarProdutos(): void {
    this.carregando = true;
    this.cdr.markForCheck();

    this.produtoService.getPaginated(this.paginaAtual, this.tamanhoPagina, this.ordenarPorSaldo, this.termoBusca).subscribe({
      next: (resultado) => {
        this.pagedResult = resultado;
        this.carregando = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.carregando = false;
        this.erroDetalhado = {
          titulo: 'Erro ao Carregar Produtos',
          mensagem: err.error?.mensagem || 'Não foi possível conectar à Estoque.API em http://localhost:5000.'
        };
        this.cdr.markForCheck();
      }
    });
  }

  alterarOrdenacaoSaldo(ordem: any): void {
    this.ordenarPorSaldo = !ordem || ordem === 'null' ? null : (ordem as 'asc' | 'desc');
    this.paginaAtual = 1;
    this.carregarProdutos();
  }

  mudarPagina(novaPagina: number): void {
    if (novaPagina < 1 || (this.pagedResult && novaPagina > this.pagedResult.totalPaginas)) return;
    this.paginaAtual = novaPagina;
    this.carregarProdutos();
  }

  alterarTamanhoPagina(novoTamanho: number): void {
    this.tamanhoPagina = Number(novoTamanho);
    this.paginaAtual = 1;
    this.carregarProdutos();
  }

  abrirModalCadastro(): void {
    this.produtoEmEdicao = null;
    this.exibirModalForm = true;
  }

  abrirModalEdicao(produto: Produto): void {
    this.produtoEmEdicao = produto;
    this.exibirModalForm = true;
  }

  fecharModalForm(): void {
    this.exibirModalForm = false;
    this.produtoEmEdicao = null;
  }

  aoSalvarProduto(produto: Produto): void {
    const acao = this.produtoEmEdicao ? 'atualizado' : 'cadastrado';
    this.mensagemSucessoModal = {
      titulo: `Produto ${this.produtoEmEdicao ? 'Atualizado' : 'Cadastrado'}!`,
      mensagem: `O produto '${produto.codigo}' (${produto.descricao}) foi ${acao} com sucesso no estoque.`,
      detalhe: `Saldo disponível atual: ${produto.saldo} unidades.`
    };
    this.carregarProdutos();
  }

  produtoParaExcluir: Produto | null = null;

  confirmarExclusao(produto: Produto): void {
    this.produtoParaExcluir = produto;
  }

  executarExclusaoProduto(): void {
    if (!this.produtoParaExcluir) return;

    const prod = this.produtoParaExcluir;
    this.produtoParaExcluir = null;

    this.produtoService.delete(prod.id).subscribe({
      next: () => {
        this.mensagemSucessoModal = {
          titulo: 'Produto Excluído',
          mensagem: `O produto '${prod.codigo}' foi removido do estoque com sucesso.`
        };
        this.carregarProdutos();
      },
      error: (err) => {
        this.erroDetalhado = {
          titulo: 'Erro ao Remover Produto',
          mensagem: err.error?.mensagem || 'Falha ao remover o produto do estoque.'
        };
        this.cdr.markForCheck();
      }
    });
  }

  obterTextoStatusSaldo(saldo: number): string {
    if (saldo === 0) return 'Sem Estoque';
    if (saldo <= 5) return 'Poucas Unidades';
    return 'Em Estoque';
  }

  obterClasseStatusSaldo(saldo: number): string {
    if (saldo === 0) return 'stock-zero';
    if (saldo <= 5) return 'stock-low';
    return 'stock-ok';
  }
}
