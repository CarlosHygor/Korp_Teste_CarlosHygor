import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { NotaFiscalService } from '../../../../services/nota-fiscal.service';
import { NotaFiscal, CreateNotaFiscalDto } from '../../../../models/nota-fiscal.model';
import { PagedResult } from '../../../../models/paged-result.model';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner';
import { StatusBadgeComponent } from '../../../../shared/components/status-badge/status-badge';
import { ErrorModalComponent, ErroDetalhado } from '../../../../shared/components/error-modal/error-modal';
import { SuccessModalComponent, MensagemSucesso } from '../../../../shared/components/success-modal/success-modal';
import { NotaFormModalComponent } from '../nota-form-modal/nota-form-modal';

interface NotaFiscalView extends NotaFiscal {
  processando?: boolean;
  expandida?: boolean;
}

@Component({
  selector: 'app-nota-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LoadingSpinnerComponent,
    StatusBadgeComponent,
    ErrorModalComponent,
    SuccessModalComponent,
    NotaFormModalComponent
  ],
  template: `
    <!-- Modal de Cadastro Reativo (FormArray) -->
    <app-nota-form-modal
      *ngIf="exibirModalForm"
      (aoFechar)="fecharModalForm()"
      (aoSalvar)="aoSalvarNota($event)"
    ></app-nota-form-modal>

    <!-- Modal de Erro Rico & Resiliência -->
    <app-error-modal
      [erro]="erroDetalhado"
      (aoFechar)="erroDetalhado = null"
    ></app-error-modal>

    <!-- Modal de Sucesso -->
    <app-success-modal
      [dados]="mensagemSucessoModal"
      (aoFechar)="mensagemSucessoModal = null"
    ></app-success-modal>

    <!-- Header da Página -->
    <div class="page-header">
      <div class="page-title">
        <h2>📄 Gestão de Notas Fiscais (Faturamento)</h2>
        <p>Emita, filtre e processe a impressão distribuída de Notas Fiscais com baixa no Estoque.</p>
      </div>

      <button class="btn btn-primary" (click)="abrirModalCadastro()">
        <svg class="btn-svg" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
          <line x1="12" y1="5" x2="12" y2="19"></line>
          <line x1="5" y1="12" x2="19" y2="12"></line>
        </svg>
        Nova Nota Fiscal
      </button>
    </div>

    <!-- Barra de Filtros por Status e Ordenação -->
    <div class="filters-bar">
      <div class="tabs-container">
        <button 
          class="filter-tab" 
          [class.active]="statusFiltro === 'Todas'"
          (click)="alterarFiltroStatus('Todas')"
        >
          <span>Todas as Notas</span>
        </button>
        <button 
          class="filter-tab" 
          [class.active]="statusFiltro === 'Aberta'"
          (click)="alterarFiltroStatus('Aberta')"
        >
          <span class="tab-dot dot-aberta"></span>
          <span>Abertas (Pendentes)</span>
        </button>
        <button 
          class="filter-tab" 
          [class.active]="statusFiltro === 'Fechada'"
          (click)="alterarFiltroStatus('Fechada')"
        >
          <span class="tab-dot dot-fechada"></span>
          <span>Fechadas (Impressas)</span>
        </button>
      </div>

      <div class="sort-selector">
        <label for="sortNota">Ordenar por:</label>
        <select id="sortNota" [ngModel]="ordenacao" (ngModelChange)="alterarOrdenacao($event)" class="select-sort">
          <option value="data_desc">Mais Recentes Primeiro (▼)</option>
          <option value="data_asc">Mais Antigas Primeiro (▲)</option>
          <option value="itens_desc">Maior Qtd de Itens (▼)</option>
          <option value="itens_asc">Menor Qtd de Itens (▲)</option>
        </select>
      </div>
    </div>

    <!-- Container da Tabela / Card Principal -->
    <div class="card-container">
      <app-loading-spinner *ngIf="carregando" [isOverlay]="true" mensagem="Carregando notas fiscais do faturamento..."></app-loading-spinner>

      <div class="table-responsive" *ngIf="!carregando && pagedResult">
        <table class="data-table" *ngIf="pagedResult.itens && pagedResult.itens.length > 0">
          <thead>
            <tr>
              <th style="width: 40px;"></th>
              <th>Numeração</th>
              <th>Data de Emissão</th>
              <th>Itens</th>
              <th>Status</th>
              <th class="text-right">Ações de Impressão</th>
            </tr>
          </thead>
          <tbody>
            <ng-container *ngFor="let nota of notasView">
              <!-- Linha Principal da Nota -->
              <tr [class.row-expanded]="nota.expandida" [class.row-fechada]="nota.status === 'Fechada'">
                <td>
                  <button class="btn-expand" (click)="toggleExpansao(nota)" title="Ver itens da nota">
                    <svg [class.rotate-90]="nota.expandida" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                      <polyline points="9 18 15 12 9 6"></polyline>
                    </svg>
                  </button>
                </td>
                <td class="td-codigo">
                  <code>#{{ formatarNumeracao(nota.numeracao) }}</code>
                </td>
                <td class="td-data">
                  {{ nota.dataCriacao | date:'dd/MM/yyyy HH:mm' }}
                </td>
                <td class="td-itens">
                  <span class="itens-badge">
                    <strong>{{ nota.itens.length }}</strong> {{ nota.itens.length === 1 ? 'item' : 'itens' }}
                  </span>
                </td>
                <td>
                  <app-status-badge [status]="nota.status"></app-status-badge>
                </td>
                <td class="text-right actions-cell">
                  <!-- Botão Imprimir Nota (Apenas Status Aberta) -->
                  <button 
                    *ngIf="nota.status === 'Aberta'" 
                    class="btn-action btn-print" 
                    [disabled]="nota.processando" 
                    (click)="imprimirNota(nota)"
                    title="Processar impressão e baixar estoque"
                  >
                    <svg *ngIf="!nota.processando" class="action-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                      <polyline points="6 9 6 2 18 2 18 9"></polyline>
                      <path d="M6 18H4a2 2 0 0 1-2-2v-5a2 2 0 0 1 2-2h16a2 2 0 0 1 2 2v5a2 2 0 0 1-2 2h-2"></path>
                      <rect x="6" y="14" width="12" height="8"></rect>
                    </svg>
                    <app-loading-spinner *ngIf="nota.processando" size="sm"></app-loading-spinner>
                    <span>{{ nota.processando ? 'Imprimindo...' : 'Imprimir Nota' }}</span>
                  </button>

                  <!-- Badge Nota Fechada -->
                  <span *ngIf="nota.status === 'Fechada'" class="printed-label">
                    <svg class="check-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                      <polyline points="20 6 9 17 4 12"></polyline>
                    </svg>
                    Finalizada
                  </span>

                  <!-- Botão Excluir Nota (Apenas Aberta) -->
                  <button 
                    *ngIf="nota.status === 'Aberta'" 
                    class="btn-action btn-delete" 
                    [disabled]="nota.processando"
                    (click)="confirmarExclusao(nota)" 
                    title="Excluir Nota Fiscal em Aberto"
                  >
                    <svg class="action-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                      <polyline points="3 6 5 6 21 6"></polyline>
                      <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                    </svg>
                  </button>
                </td>
              </tr>

              <!-- Linha Expansível de Detalhes dos Itens -->
              <tr *ngIf="nota.expandida" class="details-row">
                <td colspan="6">
                  <div class="items-details-container">
                    <h5>Itens da Nota Fiscal #{{ formatarNumeracao(nota.numeracao) }}</h5>
                    <table class="inner-table">
                      <thead>
                        <tr>
                          <th>Código do Produto</th>
                          <th>Descrição</th>
                          <th class="text-right">Quantidade Solicitada</th>
                        </tr>
                      </thead>
                      <tbody>
                        <tr *ngFor="let item of nota.itens">
                          <td><code>{{ item.codigoProduto }}</code></td>
                          <td>{{ item.descricaoProduto || item.codigoProduto }}</td>
                          <td class="text-right font-weight-bold">{{ item.quantidade }} un</td>
                        </tr>
                      </tbody>
                    </table>
                  </div>
                </td>
              </tr>
            </ng-container>
          </tbody>
        </table>

        <!-- Estado Vazio -->
        <div *ngIf="!pagedResult.itens || pagedResult.itens.length === 0" class="empty-state">
          <div class="empty-icon">📄</div>
          <h3>Nenhuma Nota Fiscal Encontrada</h3>
          <p *ngIf="statusFiltro === 'Todas'">Ainda não há notas fiscais cadastradas no sistema.</p>
          <p *ngIf="statusFiltro !== 'Todas'">Não existem notas fiscais com o status <strong>{{ statusFiltro }}</strong> no momento.</p>
          <button class="btn btn-primary" (click)="abrirModalCadastro()">Emitir Primeira Nota Fiscal</button>
        </div>

        <!-- Rodapé de Paginação -->
        <div class="pagination-footer" *ngIf="pagedResult.totalPaginas > 1">
          <span class="pagination-info">
            Página {{ pagedResult.paginaAtual }} de {{ pagedResult.totalPaginas }} (Total: {{ pagedResult.totalRegistros }} notas)
          </span>
          <div class="pagination-controls">
            <button 
              class="btn-page" 
              [disabled]="pagedResult.paginaAtual <= 1"
              (click)="mudarPagina(pagedResult.paginaAtual - 1)"
            >
              ◀ Anterior
            </button>
            <button 
              class="btn-page" 
              [disabled]="pagedResult.paginaAtual >= pagedResult.totalPaginas"
              (click)="mudarPagina(pagedResult.paginaAtual + 1)"
            >
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
    }

    .page-actions {
      display: flex;
      align-items: center;
      gap: 0.875rem;
      flex-wrap: wrap;
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

    .btn {
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0.625rem 1.25rem;
      border-radius: 0.5rem;
      font-weight: 600;
      font-size: 0.875rem;
      cursor: pointer;
      border: none;
      transition: all 0.15s ease-in-out;
    }

    .btn-primary {
      background: #38bdf8;
      color: #0f172a;
    }

    .btn-primary:hover {
      background: #0284c7;
      color: #ffffff;
    }

    .btn-svg {
      width: 1.125rem;
      height: 1.125rem;
    }

    /* Filters Bar */
    .filters-bar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.25rem;
      gap: 1rem;
      flex-wrap: wrap;
    }

    .tabs-container {
      display: inline-flex;
      background: #0d171d;
      padding: 0.25rem;
      border-radius: 0.75rem;
      border: 1px solid #294656;
      gap: 0.25rem;
    }

    .filter-tab {
      background: transparent;
      border: none;
      color: #94a3b8;
      padding: 0.5rem 1rem;
      border-radius: 0.5rem;
      font-size: 0.875rem;
      font-weight: 500;
      cursor: pointer;
      display: flex;
      align-items: center;
      gap: 0.5rem;
      transition: all 0.15s;
    }

    .filter-tab:hover {
      color: #f8fafc;
    }

    .filter-tab.active {
      background: #1a2f3a;
      color: #38bdf8;
      font-weight: 600;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.2);
    }

    .tab-dot {
      width: 0.5rem;
      height: 0.5rem;
      border-radius: 50%;
    }

    .dot-aberta { background: #f59e0b; }
    .dot-fechada { background: #10b981; }

    /* Card Container */
    .card-container {
      background: #1a2f3a;
      border: 1px solid #294656;
      border-radius: 0.875rem;
      position: relative;
      overflow: hidden;
      box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.3);
    }

    .table-responsive {
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
      font-size: 0.75rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      padding: 1rem 1.25rem;
      border-bottom: 1px solid #294656;
      vertical-align: middle;
    }

    .data-table td {
      padding: 1rem 1.25rem;
      border-bottom: 1px solid rgba(41, 70, 86, 0.5);
      color: #f8fafc;
      font-size: 0.875rem;
      vertical-align: middle;
    }

    .data-table tr:hover {
      background: rgba(56, 189, 248, 0.04);
    }

    .row-expanded {
      background: rgba(56, 189, 248, 0.03) !important;
    }

    .btn-expand {
      background: transparent;
      border: none;
      color: #94a3b8;
      cursor: pointer;
      padding: 0.25rem;
      border-radius: 0.25rem;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .btn-expand svg {
      width: 1.125rem;
      height: 1.125rem;
      transition: transform 0.2s;
    }

    .rotate-90 {
      transform: rotate(90deg);
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

    .itens-badge {
      display: inline-flex;
      align-items: center;
      gap: 0.375rem;
      background: #0f1c23;
      color: #cbd5e1;
      border: 1px solid #294656;
      padding: 0.25rem 0.625rem;
      border-radius: 0.375rem;
      font-size: 0.8125rem;
      font-weight: 500;
    }

    .itens-badge strong {
      color: #38bdf8;
      font-weight: 700;
    }

    .badge-icon {
      width: 0.875rem;
      height: 0.875rem;
      color: #38bdf8;
    }

    .text-right { text-align: right; }

    .actions-cell {
      display: flex;
      justify-content: flex-end;
      align-items: center;
      gap: 0.5rem;
    }

    .btn-action {
      display: inline-flex;
      align-items: center;
      gap: 0.375rem;
      padding: 0.375rem 0.75rem;
      border-radius: 0.375rem;
      font-size: 0.8125rem;
      font-weight: 600;
      border: none;
      cursor: pointer;
      transition: all 0.15s;
    }

    .btn-print {
      background: rgba(56, 189, 248, 0.15);
      color: #38bdf8;
      border: 1px solid rgba(56, 189, 248, 0.3);
    }

    .btn-print:hover:not(:disabled) {
      background: #38bdf8;
      color: #0f172a;
    }

    .btn-print:disabled {
      opacity: 0.6;
      cursor: wait;
    }

    .btn-delete {
      background: rgba(244, 63, 94, 0.1);
      color: #f43f5e;
      border: 1px solid rgba(244, 63, 94, 0.2);
      padding: 0.375rem 0.5rem;
    }

    .btn-delete:hover:not(:disabled) {
      background: #f43f5e;
      color: #ffffff;
    }

    .printed-label {
      display: inline-flex;
      align-items: center;
      gap: 0.25rem;
      color: #34d399;
      font-size: 0.8125rem;
      font-weight: 600;
      padding: 0.375rem 0.75rem;
    }

    .check-icon {
      width: 1rem;
      height: 1rem;
    }

    .action-icon {
      width: 1rem;
      height: 1rem;
    }

    /* Details row */
    .details-row td {
      background: #0f172a;
      padding: 1.25rem;
      border-bottom: 1px solid #334155;
    }

    .items-details-container {
      background: #1e293b;
      border: 1px solid #334155;
      border-radius: 0.5rem;
      padding: 1rem;
    }

    .items-details-container h5 {
      margin: 0 0 0.75rem 0;
      color: #38bdf8;
      font-size: 0.875rem;
      font-weight: 600;
    }

    .inner-table {
      width: 100%;
      border-collapse: collapse;
      font-size: 0.8125rem;
    }

    .inner-table th {
      background: #0f172a;
      color: #94a3b8;
      padding: 0.5rem 0.75rem;
      text-transform: none;
      letter-spacing: normal;
    }

    .inner-table td {
      padding: 0.5rem 0.75rem;
      border-bottom-color: #334155;
    }

    .inner-table code {
      color: #38bdf8;
    }

    .font-weight-bold {
      font-weight: 700;
    }

    /* Empty state */
    .empty-state {
      padding: 3rem 1.5rem;
      text-align: center;
      color: #94a3b8;
    }

    .empty-icon {
      font-size: 3rem;
      margin-bottom: 1rem;
    }

    .empty-state h3 {
      color: #f8fafc;
      margin: 0 0 0.5rem 0;
    }

    .empty-state p {
      margin: 0 0 1.5rem 0;
    }

    /* Pagination */
    .pagination-footer {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1rem 1.25rem;
      background: #0f172a;
      border-top: 1px solid #334155;
      font-size: 0.875rem;
      color: #94a3b8;
    }

    .pagination-controls {
      display: flex;
      gap: 0.5rem;
    }

    .btn-page {
      background: #1e293b;
      border: 1px solid #334155;
      color: #e2e8f0;
      padding: 0.375rem 0.75rem;
      border-radius: 0.375rem;
      font-size: 0.8125rem;
      cursor: pointer;
    }

    .btn-page:hover:not(:disabled) {
      background: #334155;
      color: #ffffff;
    }

    .btn-page:disabled {
      opacity: 0.4;
      cursor: not-allowed;
    }
  `]
})
export class NotaFiscalListComponent implements OnInit {
  pagedResult: PagedResult<NotaFiscal> | null = null;
  notasView: NotaFiscalView[] = [];
  carregando: boolean = false;

  paginaAtual: number = 1;
  tamanhoPagina: number = 10;
  statusFiltro: string = 'Todas';
  ordenacao: string = 'data_desc';

  exibirModalForm: boolean = false;
  erroDetalhado: ErroDetalhado | null = null;
  mensagemSucessoModal: MensagemSucesso | null = null;

  constructor(
    private readonly notaFiscalService: NotaFiscalService,
    private readonly cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.carregarNotas();
  }

  carregarNotas(): void {
    this.carregando = true;

    const statusParam = this.statusFiltro === 'Todas' ? undefined : this.statusFiltro;

    this.notaFiscalService.getPaginated(this.paginaAtual, this.tamanhoPagina, statusParam, this.ordenacao)
      .pipe(finalize(() => {
        this.carregando = false;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: (res) => {
          this.pagedResult = res;
          this.notasView = (res.itens || []).map(n => ({ ...n, processando: false, expandida: false }));
        },
        error: (err) => {
          this.erroDetalhado = {
            titulo: 'Erro ao carregar Notas Fiscais',
            mensagem: err.error?.mensagem || 'Não foi possível carregar a lista de notas fiscais.'
          };
        }
      });
  }

  alterarOrdenacao(novaOrdenacao: string): void {
    if (this.ordenacao !== novaOrdenacao) {
      this.ordenacao = novaOrdenacao;
      this.paginaAtual = 1;
      this.carregarNotas();
    }
  }

  alterarFiltroStatus(status: string): void {
    if (this.statusFiltro !== status) {
      this.statusFiltro = status;
      this.paginaAtual = 1;
      this.carregarNotas();
    }
  }

  mudarPagina(pagina: number): void {
    this.paginaAtual = pagina;
    this.carregarNotas();
  }

  toggleExpansao(nota: NotaFiscalView): void {
    nota.expandida = !nota.expandida;
  }

  formatarNumeracao(numeracao: number): string {
    return numeracao.toString().padStart(4, '0');
  }

  abrirModalCadastro(): void {
    this.exibirModalForm = true;
  }

  fecharModalForm(): void {
    this.exibirModalForm = false;
  }

  aoSalvarNota(dto: CreateNotaFiscalDto): void {
    this.notaFiscalService.create(dto).subscribe({
      next: (notaCriada) => {
        this.fecharModalForm();
        this.mensagemSucessoModal = {
          titulo: 'Nota Fiscal Criada!',
          mensagem: `A Nota Fiscal #${this.formatarNumeracao(notaCriada.numeracao)} foi emitida com sucesso no status ABERTA.`
        };
        this.carregarNotas();
      },
      error: (err) => {
        this.erroDetalhado = {
          titulo: 'Falha ao Criar Nota Fiscal',
          mensagem: err.error?.mensagem || 'Ocorreu um erro ao emitir a nota fiscal.'
        };
      }
    });
  }

  imprimirNota(nota: NotaFiscalView): void {
    nota.processando = true;

    this.notaFiscalService.imprimir(nota.id)
      .pipe(finalize(() => {
        nota.processando = false;
        this.cdr.detectChanges();
      }))
      .subscribe({
        next: (notaAtualizada) => {
          nota.status = notaAtualizada.status;
          this.mensagemSucessoModal = {
            titulo: 'Nota Fiscal Impressa & Fechada!',
            mensagem: `A Nota Fiscal #${this.formatarNumeracao(nota.numeracao)} foi finalizada com sucesso. O estoque dos produtos utilizados foi abatido.`
          };
          this.carregarNotas();
        },
        error: (err) => {
          const status = err.status;
          const erroBody = err.error;

          if (status === 503) {
            // Tratamento de Resiliência: Microsserviço de Estoque Indisponível
            this.erroDetalhado = {
              titulo: 'Serviço de Estoque Indisponível (HTTP 503)',
              mensagem: 'Não foi possível conectar ao microsserviço de Estoque para baixa dos produtos. A Nota Fiscal permaneceu ABERTA e poderá ser impressa novamente assim que o serviço estabilizar.',
              isResiliencia: true
            };
          } else if (status === 422) {
            // Erro de Negócio: Saldo Insuficiente
            this.erroDetalhado = {
              titulo: 'Saldo Insuficiente no Estoque (HTTP 422)',
              mensagem: erroBody?.mensagem || 'Estoque insuficiente para faturamento.',
              codigoProduto: erroBody?.codigoProduto,
              saldoAtual: erroBody?.saldoAtual,
              quantidadeSolicitada: erroBody?.quantidadeSolicitada
            };
          } else {
            // Erros genéricos (400, 404, 500)
            this.erroDetalhado = {
              titulo: `Erro ao Processar Impressão (HTTP ${status})`,
              mensagem: erroBody?.mensagem || 'Falha ao processar a impressão da nota fiscal.'
            };
          }
        }
      });
  }

  confirmarExclusao(nota: NotaFiscalView): void {
    if (confirm(`Tem certeza que deseja cancelar e excluir a Nota Fiscal #${this.formatarNumeracao(nota.numeracao)}?`)) {
      this.notaFiscalService.delete(nota.id).subscribe({
        next: () => {
          this.mensagemSucessoModal = {
            titulo: 'Nota Fiscal Cancelada',
            mensagem: `A Nota Fiscal #${this.formatarNumeracao(nota.numeracao)} foi removida.`
          };
          this.carregarNotas();
        },
        error: (err) => {
          this.erroDetalhado = {
            titulo: 'Erro ao Cancelar Nota Fiscal',
            mensagem: err.error?.mensagem || 'Não foi possível excluir a nota fiscal.'
          };
        }
      });
    }
  }
}
