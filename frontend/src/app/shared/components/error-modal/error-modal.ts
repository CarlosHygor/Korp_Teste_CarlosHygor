import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface ErroDetalhado {
  titulo?: string;
  mensagem: string;
  codigoProduto?: string;
  saldoAtual?: number;
  quantidadeSolicitada?: number;
  isResiliencia?: boolean;
}

@Component({
  selector: 'app-error-modal',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="modal-backdrop" *ngIf="erro" (click)="fechar()">
      <div class="modal-card" (click)="$event.stopPropagation()">
        <div class="modal-header" [class.resiliencia]="erro.isResiliencia">
          <div class="icon-circle">
            <span *ngIf="erro.isResiliencia">⚡</span>
            <span *ngIf="!erro.isResiliencia">⚠️</span>
          </div>
          <h3>{{ erro.titulo || (erro.isResiliencia ? 'Aviso de Resiliência' : 'Atenção') }}</h3>
        </div>

        <div class="modal-body">
          <p class="mensagem-principal">{{ erro.mensagem }}</p>

          <div *ngIf="erro.codigoProduto" class="detalhes-estoque">
            <div class="detalhe-item">
              <span class="label">Produto:</span>
              <span class="valor valor-codigo">{{ erro.codigoProduto }}</span>
            </div>
            <div class="detalhe-item">
              <span class="label">Saldo em Estoque:</span>
              <span class="valor valor-disponivel">{{ erro.saldoAtual }} un</span>
            </div>
            <div class="detalhe-item">
              <span class="label">Qtd. Solicitada na Nota:</span>
              <span class="valor valor-solicitado">{{ erro.quantidadeSolicitada }} un</span>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button class="btn btn-primary" (click)="fechar()">Entendi</button>
        </div>
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
      background: #1e293b;
      border: 1px solid #334155;
      border-radius: 1rem;
      width: 100%;
      max-width: 480px;
      overflow: hidden;
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.5);
    }

    .modal-header {
      padding: 1.25rem 1.5rem;
      background: rgba(239, 68, 68, 0.1);
      border-bottom: 1px solid rgba(239, 68, 68, 0.2);
      display: flex;
      align-items: center;
      gap: 0.75rem;
    }

    .modal-header.resiliencia {
      background: rgba(245, 158, 11, 0.1);
      border-bottom-color: rgba(245, 158, 11, 0.2);
    }

    .icon-circle {
      font-size: 1.25rem;
    }

    .modal-header h3 {
      margin: 0;
      color: #f8fafc;
      font-size: 1.125rem;
      font-weight: 600;
    }

    .modal-body {
      padding: 1.5rem;
    }

    .mensagem-principal {
      color: #cbd5e1;
      margin: 0 0 1rem 0;
      line-height: 1.5;
      font-size: 0.9375rem;
    }

    .detalhes-estoque {
      background: #0f172a;
      border: 1px solid #334155;
      border-radius: 0.5rem;
      padding: 1rem;
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .detalhe-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      font-size: 0.875rem;
    }

    .detalhe-item .label {
      color: #94a3b8;
    }

    .detalhe-item .valor {
      font-weight: 600;
    }

    .valor-codigo {
      color: #38bdf8;
      font-family: monospace;
    }

    .valor-disponivel {
      color: #f59e0b;
    }

    .valor-solicitado {
      color: #f43f5e;
    }

    .modal-footer {
      padding: 1rem 1.5rem;
      background: #0f172a;
      border-top: 1px solid #334155;
      display: flex;
      justify-content: flex-end;
    }

    .btn-primary {
      background: #6366f1;
      color: #ffffff;
      border: none;
      padding: 0.5rem 1.25rem;
      border-radius: 0.5rem;
      font-weight: 600;
      cursor: pointer;
      transition: background-color 0.2s;
    }

    .btn-primary:hover {
      background: #4f46e5;
    }

    @keyframes fadeIn {
      from { opacity: 0; transform: scale(0.95); }
      to { opacity: 1; transform: scale(1); }
    }
  `]
})
export class ErrorModalComponent {
  @Input() erro: ErroDetalhado | null = null;
  @Output() aoFechar = new EventEmitter<void>();

  fechar(): void {
    this.erro = null;
    this.aoFechar.emit();
  }
}
