import { Component, OnInit, Output, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { ProdutoService } from '../../../../services/produto.service';
import { Produto } from '../../../../models/produto.model';
import { CreateNotaFiscalDto } from '../../../../models/nota-fiscal.model';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-nota-form-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LoadingSpinnerComponent],
  template: `
    <div class="modal-backdrop" (click)="fechar()">
      <div class="modal-card" (click)="$event.stopPropagation()">
        
        <!-- Header do Modal -->
        <div class="modal-header">
          <div class="header-title">
            <svg class="header-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7Z"/>
              <path d="M14 2v4a2 2 0 0 0 2 2h4"/>
              <line x1="12" y1="11" x2="12" y2="17"/>
              <line x1="9" y1="14" x2="15" y2="14"/>
            </svg>
            <div>
              <h3>Nova Nota Fiscal</h3>
              <p>Adicione produtos e quantidades para gerar o faturamento</p>
            </div>
          </div>
          <button class="btn-close" (click)="fechar()" title="Fechar (Esc)">✕</button>
        </div>

        <!-- Body do Modal com Overlay de Loading da Aplicação -->
        <div class="modal-body-container">
          <app-loading-spinner 
            *ngIf="carregandoProdutos" 
            [isOverlay]="true" 
            mensagem="Carregando produtos do estoque..."
          ></app-loading-spinner>

          <!-- Formulário Reativo Dinâmico -->
          <form [formGroup]="notaForm" (ngSubmit)="salvar()" class="modal-body" *ngIf="!carregandoProdutos">
            <div class="form-section-header">
              <h4>Itens da Nota Fiscal</h4>
              <span class="itens-count">{{ itensArray.length }} {{ itensArray.length === 1 ? 'item' : 'itens' }}</span>
            </div>

            <!-- Lista Dinâmica de Itens (FormArray) -->
            <div formArrayName="itens" class="itens-list">
              <div *ngFor="let itemGroup of itensArray.controls; let i = index" [formGroupName]="i" class="item-block">
                <div class="item-row-header">
                  <span class="item-number">Item #{{ i + 1 }}</span>
                  
                  <!-- Botão Remover Item -->
                  <button 
                    type="button" 
                    class="btn-remove" 
                    (click)="removerItem(i)" 
                    [disabled]="itensArray.length <= 1" 
                    title="Remover item da nota"
                  >
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                      <polyline points="3 6 5 6 21 6"></polyline>
                      <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                    </svg>
                    <span>Remover</span>
                  </button>
                </div>

                <div class="item-fields">
                  <!-- Seletor do Produto -->
                  <div class="form-group flex-2">
                    <label>Selecione o Produto (por Código ou Descrição) *</label>
                    <select formControlName="codigoProduto" class="form-control" [class.is-invalid]="isFieldInvalid(i, 'codigoProduto')">
                      <option value="">Selecione um produto...</option>
                      <option *ngFor="let prod of produtosDisponiveis" [value]="prod.codigo" [disabled]="prod.saldo === 0">
                        [{{ prod.codigo }}] {{ truncarDescricao(prod.descricao) }} {{ prod.saldo === 0 ? '⚠️ (Sem Estoque)' : '(' + prod.saldo + ' un disponível)' }}
                      </option>
                    </select>
                  </div>

                  <!-- Quantidade -->
                  <div class="form-group flex-1">
                    <label>Quantidade *</label>
                    <input 
                      type="number" 
                      formControlName="quantidade" 
                      min="1" 
                      class="form-control" 
                      [class.is-invalid]="isFieldInvalid(i, 'quantidade')"
                      placeholder="Ex: 5"
                    />
                  </div>
                </div>

                <!-- Card de Detalhes do Produto Selecionado -->
                <div *ngIf="obterProdutoSelecionado(i) as prod" class="selected-product-card">
                  <div class="prod-badge-code"><code>{{ prod.codigo }}</code></div>
                  <div class="prod-full-desc" [title]="prod.descricao">{{ prod.descricao }}</div>
                  <div class="prod-stock-info">
                    <span class="stock-label">Saldo em estoque:</span>
                    <span class="stock-value" [class.text-danger]="prod.saldo === 0">{{ prod.saldo }} un</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Alerta de Duplicidade se houver -->
            <div *ngIf="erroDuplicidade" class="alert alert-warning">
              ⚠️ {{ erroDuplicidade }}
            </div>

            <!-- Botão Adicionar Mais Itens (Validação Defensiva) -->
            <div class="add-item-bar">
              <button 
                type="button" 
                class="btn btn-secondary" 
                (click)="adicionarItem()" 
                [disabled]="!podeAdicionarItem"
                [title]="podeAdicionarItem ? 'Adicionar novo item à nota' : 'Preencha o produto e a quantidade do item atual antes de adicionar outro'"
              >
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <line x1="12" y1="5" x2="12" y2="19"></line>
                  <line x1="5" y1="12" x2="19" y2="12"></line>
                </svg>
                Adicionar Outro Produto
              </button>
            </div>

            <!-- Footer do Form Modal -->
            <div class="modal-footer">
              <button type="button" class="btn btn-cancel" (click)="fechar()">Cancelar</button>
              <button type="submit" class="btn btn-primary" [disabled]="notaForm.invalid || salvando">
                <span *ngIf="!salvando">Emitir Nota Fiscal (Aberta)</span>
                <span *ngIf="salvando">Emitindo...</span>
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .modal-backdrop {
      position: fixed;
      inset: 0;
      background: rgba(18, 34, 42, 0.85);
      backdrop-filter: blur(4px);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 100;
      padding: 1rem;
      animation: fadeIn 0.2s ease-out;
    }

    .modal-card {
      background: #1a2f3a;
      border: 1px solid #294656;
      border-radius: 0.875rem;
      width: 100%;
      max-width: 680px;
      max-height: 90vh;
      display: flex;
      flex-direction: column;
      box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.6);
      overflow: hidden;
    }

    .modal-header {
      padding: 1.25rem 1.5rem;
      border-bottom: 1px solid #294656;
      display: flex;
      justify-content: space-between;
      align-items: center;
      background: #162832;
    }

    .header-title {
      display: flex;
      align-items: center;
      gap: 0.75rem;
    }

    .header-icon {
      width: 1.75rem;
      height: 1.75rem;
      color: #38bdf8;
    }

    .header-title h3 {
      margin: 0;
      color: #f8fafc;
      font-size: 1.125rem;
      font-weight: 700;
    }

    .header-title p {
      margin: 0;
      color: #94a3b8;
      font-size: 0.8125rem;
    }

    .btn-close {
      background: transparent;
      border: none;
      color: #94a3b8;
      font-size: 1.25rem;
      cursor: pointer;
      padding: 0.25rem 0.5rem;
      border-radius: 0.375rem;
      transition: all 0.15s;
    }

    .btn-close:hover {
      color: #f8fafc;
      background: #243f4e;
    }

    .modal-body-container {
      position: relative;
      min-height: 240px;
      display: flex;
      flex-direction: column;
      flex: 1;
      overflow-y: auto;
    }

    .modal-body {
      padding: 1.5rem;
      display: flex;
      flex-direction: column;
      gap: 1.25rem;
    }

    .form-section-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding-bottom: 0.5rem;
      border-bottom: 1px solid #294656;
    }

    .form-section-header h4 {
      margin: 0;
      color: #f8fafc;
      font-size: 0.9375rem;
      font-weight: 600;
    }

    .itens-count {
      background: rgba(56, 189, 248, 0.15);
      color: #38bdf8;
      border: 1px solid rgba(56, 189, 248, 0.3);
      padding: 0.125rem 0.625rem;
      border-radius: 9999px;
      font-size: 0.75rem;
      font-weight: 700;
    }

    .itens-list {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .item-block {
      background: #0f1c23;
      padding: 1rem;
      border: 1px solid #294656;
      border-radius: 0.625rem;
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }

    .item-row-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .item-number {
      font-weight: 700;
      color: #38bdf8;
      font-size: 0.8125rem;
    }

    .btn-remove {
      background: rgba(244, 63, 94, 0.1);
      border: 1px solid rgba(244, 63, 94, 0.2);
      color: #f43f5e;
      padding: 0.25rem 0.5rem;
      border-radius: 0.375rem;
      font-size: 0.75rem;
      font-weight: 600;
      display: flex;
      align-items: center;
      gap: 0.25rem;
      cursor: pointer;
      transition: all 0.15s;
    }

    .btn-remove:hover:not(:disabled) {
      background: #f43f5e;
      color: #ffffff;
    }

    .btn-remove:disabled {
      opacity: 0.3;
      cursor: not-allowed;
    }

    .btn-remove svg {
      width: 0.875rem;
      height: 0.875rem;
    }

    .item-fields {
      display: flex;
      gap: 0.75rem;
      flex-wrap: wrap;
    }

    .flex-2 { flex: 2; min-width: 240px; }
    .flex-1 { flex: 1; min-width: 120px; }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: 0.375rem;
    }

    .form-group label {
      color: #cbd5e1;
      font-size: 0.8125rem;
      font-weight: 500;
    }

    .form-control {
      background: #162832;
      border: 1px solid #294656;
      color: #f8fafc;
      padding: 0.5rem 0.75rem;
      border-radius: 0.5rem;
      font-size: 0.875rem;
      outline: none;
      max-width: 100%;
      text-overflow: ellipsis;
      white-space: nowrap;
      overflow: hidden;
      transition: border-color 0.15s;
    }

    .form-control:focus {
      border-color: #38bdf8;
      box-shadow: 0 0 0 2px rgba(56, 189, 248, 0.2);
    }

    .form-control.is-invalid {
      border-color: #f43f5e;
    }

    .selected-product-card {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      background: #162832;
      border: 1px solid #294656;
      padding: 0.5rem 0.75rem;
      border-radius: 0.5rem;
      font-size: 0.8125rem;
    }

    .prod-badge-code code {
      background: #0f1c23;
      color: #38bdf8;
      padding: 0.2rem 0.4rem;
      border-radius: 0.25rem;
      font-family: monospace;
      font-size: 0.75rem;
      border: 1px solid #294656;
    }

    .prod-full-desc {
      flex: 1;
      color: #f8fafc;
      font-weight: 500;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .prod-stock-info {
      display: flex;
      gap: 0.25rem;
      align-items: center;
    }

    .stock-label { color: #94a3b8; font-size: 0.75rem; }
    .stock-value { color: #34d399; font-weight: 700; font-size: 0.8125rem; }
    .stock-value.text-danger { color: #f43f5e; }

    .alert {
      padding: 0.75rem 1rem;
      border-radius: 0.5rem;
      font-size: 0.875rem;
    }

    .alert-warning {
      background: rgba(245, 158, 11, 0.15);
      border: 1px solid rgba(245, 158, 11, 0.3);
      color: #fbbf24;
    }

    .add-item-bar {
      display: flex;
      justify-content: flex-start;
    }

    .btn-secondary {
      background: rgba(255, 255, 255, 0.04);
      border: 1px dashed #294656;
      color: #e2e8f0;
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

    .btn-secondary:hover:not(:disabled) {
      background: rgba(255, 255, 255, 0.08);
      border-color: #38bdf8;
      color: #38bdf8;
    }

    .btn-secondary:disabled {
      opacity: 0.4;
      cursor: not-allowed;
      border-color: #294656;
    }

    .btn-secondary svg {
      width: 1rem;
      height: 1rem;
    }

    .modal-footer {
      padding-top: 1rem;
      border-top: 1px solid #294656;
      display: flex;
      justify-content: flex-end;
      gap: 0.75rem;
    }

    .btn-cancel {
      background: transparent;
      border: 1px solid #294656;
      color: #cbd5e1;
      padding: 0.5rem 1.25rem;
      border-radius: 0.5rem;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.15s;
    }

    .btn-cancel:hover {
      background: #243f4e;
      color: #f8fafc;
    }

    .btn-primary {
      background: #0ea5e9;
      color: #ffffff;
      border: none;
      padding: 0.5rem 1.5rem;
      border-radius: 0.5rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.15s;
    }

    .btn-primary:hover:not(:disabled) {
      background: #0284c7;
    }

    .btn-primary:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    @keyframes fadeIn {
      from { opacity: 0; transform: scale(0.97); }
      to { opacity: 1; transform: scale(1); }
    }
  `]
})
export class NotaFormModalComponent implements OnInit {
  @Output() aoFechar = new EventEmitter<void>();
  @Output() aoSalvar = new EventEmitter<CreateNotaFiscalDto>();

  notaForm!: FormGroup;
  produtosDisponiveis: Produto[] = [];
  carregandoProdutos: boolean = false;
  salvando: boolean = false;
  erroDuplicidade: string | null = null;

  constructor(
    private readonly fb: FormBuilder,
    private readonly produtoService: ProdutoService,
    private readonly cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.iniciarFormulario();
    this.carregarProdutos();
  }

  iniciarFormulario(): void {
    this.notaForm = this.fb.group({
      itens: this.fb.array([this.criarItemFormGroup()])
    });
  }

  get itensArray(): FormArray {
    return this.notaForm.get('itens') as FormArray;
  }

  criarItemFormGroup(): FormGroup {
    return this.fb.group({
      codigoProduto: ['', Validators.required],
      quantidade: [1, [Validators.required, Validators.min(1)]]
    });
  }

  get podeAdicionarItem(): boolean {
    if (this.itensArray.length === 0) return true;
    const ultimoItem = this.itensArray.at(this.itensArray.length - 1);
    const codigoValido = !!ultimoItem.get('codigoProduto')?.value;
    const qtdValida = Number(ultimoItem.get('quantidade')?.value) > 0;
    return codigoValido && qtdValida && !this.carregandoProdutos;
  }

  adicionarItem(): void {
    if (this.podeAdicionarItem) {
      this.itensArray.push(this.criarItemFormGroup());
    }
  }

  removerItem(index: number): void {
    if (this.itensArray.length > 1) {
      this.itensArray.removeAt(index);
    }
  }

  carregarProdutos(): void {
    this.carregandoProdutos = true;
    this.cdr.detectChanges();

    this.produtoService.getPaginated(1, 100).subscribe({
      next: (res) => {
        this.produtosDisponiveis = res.itens || [];
        this.carregandoProdutos = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.carregandoProdutos = false;
        this.cdr.detectChanges();
      }
    });
  }

  truncarDescricao(descricao: string, maxLen: number = 38): string {
    if (!descricao) return '';
    return descricao.length > maxLen ? descricao.substring(0, maxLen) + '...' : descricao;
  }

  obterProdutoSelecionado(index: number): Produto | null {
    const itemGroup = this.itensArray.at(index);
    const codigo = itemGroup?.get('codigoProduto')?.value;
    if (!codigo) return null;
    return this.produtosDisponiveis.find(p => p.codigo === codigo) || null;
  }

  isFieldInvalid(index: number, fieldName: string): boolean {
    const itemGroup = this.itensArray.at(index);
    const field = itemGroup?.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  fechar(): void {
    this.aoFechar.emit();
  }

  salvar(): void {
    if (this.notaForm.invalid) {
      this.notaForm.markAllAsTouched();
      return;
    }

    this.erroDuplicidade = null;
    const formVal = this.notaForm.value;

    // Verificar itens duplicados
    const codigos = formVal.itens.map((i: { codigoProduto: string }) => i.codigoProduto);
    const temDuplicados = new Set(codigos).size !== codigos.length;
    if (temDuplicados) {
      this.erroDuplicidade = 'Você selecionou o mesmo produto mais de uma vez na nota. Agrupe as quantidades em um único item.';
      return;
    }

    const dto: CreateNotaFiscalDto = {
      itens: formVal.itens.map((i: { codigoProduto: string; quantidade: number }) => {
        const prod = this.produtosDisponiveis.find(p => p.codigo === i.codigoProduto);
        return {
          codigoProduto: i.codigoProduto,
          descricaoProduto: prod?.descricao || i.codigoProduto,
          quantidade: Number(i.quantidade)
        };
      })
    };

    this.aoSalvar.emit(dto);
  }
}
