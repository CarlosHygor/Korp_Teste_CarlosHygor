import { Component, Output, EventEmitter, Input, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { ProdutoService } from '../../../../services/produto.service';
import { Produto, ProdutoDto } from '../../../../models/produto.model';
import { LoadingSpinnerComponent } from '../../../../shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-produto-form-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LoadingSpinnerComponent],
  template: `
    <div class="modal-backdrop" (click)="fechar()">
      <div class="modal-card" (click)="$event.stopPropagation()">
        
        <div class="modal-header">
          <h3>📦 {{ produtoEdicao ? 'Editar Produto' : 'Cadastrar Novo Produto' }}</h3>
          <button class="btn-close" (click)="fechar()">✕</button>
        </div>

        <form [formGroup]="form" (ngSubmit)="salvar()">
          <div class="modal-body">
            <app-loading-spinner *ngIf="salvando" [isOverlay]="true" mensagem="Salvando no estoque..."></app-loading-spinner>

            <div *ngIf="erroApi" class="alert-error">
              ⚠️ {{ erroApi }}
            </div>

            <!-- Campo Código -->
            <div class="form-group">
              <label for="codigo">Código do Produto <span class="required">*</span></label>
              <input 
                id="codigo" 
                type="text" 
                formControlName="codigo" 
                placeholder="Ex: PROD-001" 
                class="form-control"
                [class.invalid]="campoInvalido('codigo')"
              />
              <div *ngIf="campoInvalido('codigo')" class="field-error">
                <span *ngIf="form.get('codigo')?.errors?.['required']">O código é obrigatório.</span>
                <span *ngIf="form.get('codigo')?.errors?.['minlength']">Mínimo de 2 caracteres.</span>
              </div>
            </div>

            <!-- Campo Descrição -->
            <div class="form-group">
              <label for="descricao">Descrição (Nome) <span class="required">*</span></label>
              <input 
                id="descricao" 
                type="text" 
                formControlName="descricao" 
                placeholder="Ex: Teclado Mecânico RGB" 
                class="form-control"
                [class.invalid]="campoInvalido('descricao')"
              />
              <div *ngIf="campoInvalido('descricao')" class="field-error">
                <span *ngIf="form.get('descricao')?.errors?.['required']">A descrição é obrigatória.</span>
                <span *ngIf="form.get('descricao')?.errors?.['minlength']">Mínimo de 3 caracteres.</span>
              </div>
            </div>

            <!-- Campo Saldo Inicial -->
            <div class="form-group">
              <label for="saldo">Saldo em Estoque (Quantidade) <span class="required">*</span></label>
              <input 
                id="saldo" 
                type="number" 
                formControlName="saldo" 
                placeholder="0" 
                min="0"
                class="form-control"
                [class.invalid]="campoInvalido('saldo')"
              />
              <div *ngIf="campoInvalido('saldo')" class="field-error">
                <span *ngIf="form.get('saldo')?.errors?.['required']">O saldo inicial é obrigatório.</span>
                <span *ngIf="form.get('saldo')?.errors?.['min']">O saldo não pode ser menor que zero.</span>
              </div>
            </div>
          </div>

          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" (click)="fechar()" [disabled]="salvando">Cancelar</button>
            <button type="submit" class="btn btn-primary" [disabled]="form.invalid || salvando">
              {{ produtoEdicao ? 'Atualizar Produto' : 'Salvar Produto' }}
            </button>
          </div>
        </form>

      </div>
    </div>
  `,
  styles: [`
    .modal-backdrop {
      position: fixed;
      inset: 0;
      background: rgba(15, 23, 42, 0.8);
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
      border-radius: 1rem;
      width: 100%;
      max-width: 500px;
      overflow: hidden;
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.5);
      position: relative;
    }

    .modal-header {
      padding: 1.25rem 1.5rem;
      background: #162832;
      border-bottom: 1px solid #294656;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .modal-header h3 {
      margin: 0;
      color: #f8fafc;
      font-size: 1.125rem;
      font-weight: 700;
    }

    .btn-close {
      background: transparent;
      border: none;
      color: #94a3b8;
      font-size: 1.25rem;
      cursor: pointer;
      padding: 0.25rem;
      border-radius: 0.375rem;
      transition: color 0.2s;
    }
    .btn-close:hover { color: #f8fafc; }

    .modal-body {
      padding: 1.5rem;
      display: flex;
      flex-direction: column;
      gap: 1.25rem;
      position: relative;
    }

    .alert-error {
      background: rgba(244, 63, 94, 0.15);
      border: 1px solid rgba(244, 63, 94, 0.3);
      color: #fb7185;
      padding: 0.75rem 1rem;
      border-radius: 0.5rem;
      font-size: 0.875rem;
      font-weight: 500;
    }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: 0.375rem;
    }

    .form-group label {
      font-size: 0.875rem;
      font-weight: 600;
      color: #cbd5e1;
    }

    .required { color: #f43f5e; }

    .form-control {
      background: #0f1c23;
      border: 1px solid #294656;
      border-radius: 0.5rem;
      padding: 0.625rem 0.875rem;
      color: #f8fafc;
      font-size: 0.9375rem;
      outline: none;
      transition: border-color 0.2s, box-shadow 0.2s;
    }

    .form-control:focus {
      border-color: #0ea5e9;
      box-shadow: 0 0 0 3px rgba(14, 165, 233, 0.15);
    }

    .form-control.invalid {
      border-color: #f43f5e;
    }

    .field-error {
      color: #fb7185;
      font-size: 0.75rem;
      margin-top: 0.125rem;
    }

    .modal-footer {
      padding: 1rem 1.5rem;
      background: #162832;
      border-top: 1px solid #294656;
      display: flex;
      justify-content: flex-end;
      gap: 0.75rem;
    }

    .btn-primary {
      background-color: #0ea5e9;
      color: #ffffff;
      border: none;
      padding: 0.625rem 1.25rem;
      border-radius: 0.5rem;
      font-size: 0.875rem;
      font-weight: 600;
      cursor: pointer;
      box-shadow: 0 4px 12px rgba(14, 165, 233, 0.3);
      transition: all 0.2s ease;
    }

    .btn-primary:hover:not(:disabled) {
      background-color: #0284c7;
      transform: translateY(-1px);
    }

    .btn-primary:disabled {
      opacity: 0.5;
      cursor: not-allowed;
      transform: none;
    }

    .btn-secondary {
      background-color: #243f4e;
      color: #f8fafc;
      border: 1px solid #294656;
      padding: 0.625rem 1.25rem;
      border-radius: 0.5rem;
      font-size: 0.875rem;
      font-weight: 600;
      cursor: pointer;
    }

    @keyframes fadeIn {
      from { opacity: 0; transform: scale(0.95); }
      to { opacity: 1; transform: scale(1); }
    }
  `]
})
export class ProdutoFormModalComponent {
  @Input() produtoEdicao: Produto | null = null;
  @Output() aoFechar = new EventEmitter<void>();
  @Output() aoSalvar = new EventEmitter<Produto>();

  form: FormGroup;
  salvando: boolean = false;
  erroApi: string | null = null;

  constructor(
    private readonly fb: FormBuilder,
    private readonly produtoService: ProdutoService,
    private readonly cdr: ChangeDetectorRef
  ) {
    this.form = this.fb.group({
      codigo: ['', [Validators.required, Validators.minLength(2)]],
      descricao: ['', [Validators.required, Validators.minLength(3)]],
      saldo: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    if (this.produtoEdicao) {
      this.form.patchValue({
        codigo: this.produtoEdicao.codigo,
        descricao: this.produtoEdicao.descricao,
        saldo: this.produtoEdicao.saldo
      });
    }
  }

  campoInvalido(campo: string): boolean {
    const control = this.form.get(campo);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.salvando = true;
    this.erroApi = null;
    this.cdr.markForCheck();

    const dto: ProdutoDto = this.form.value;

    if (this.produtoEdicao) {
      this.produtoService.update(this.produtoEdicao.id, dto)
        .pipe(
          finalize(() => {
            this.salvando = false;
            this.cdr.markForCheck();
          })
        )
        .subscribe({
          next: () => {
            this.aoSalvar.emit({ ...this.produtoEdicao!, ...dto });
            this.fechar();
          },
          error: (err) => {
            this.erroApi = err.error?.mensagem || 'Falha ao atualizar produto.';
            this.cdr.markForCheck();
          }
        });
    } else {
      this.produtoService.create(dto)
        .pipe(
          finalize(() => {
            this.salvando = false;
            this.cdr.markForCheck();
          })
        )
        .subscribe({
          next: (produtoCriado) => {
            this.aoSalvar.emit(produtoCriado);
            this.fechar();
          },
          error: (err) => {
            if (err.status === 409) {
              this.erroApi = `O código de produto '${dto.codigo}' já está em uso no estoque.`;
            } else {
              this.erroApi = err.error?.mensagem || 'Falha ao cadastrar produto no estoque.';
            }
            this.cdr.markForCheck();
          }
        });
    }
  }

  fechar(): void {
    this.aoFechar.emit();
  }
}
