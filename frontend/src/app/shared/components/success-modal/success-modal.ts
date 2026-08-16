import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface MensagemSucesso {
  titulo: string;
  mensagem: string;
  detalhe?: string;
}

@Component({
  selector: 'app-success-modal',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="modal-backdrop" *ngIf="dados" (click)="fechar()">
      <div class="modal-card" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <div class="success-icon-circle">
            <svg class="check-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round">
              <polyline points="20 6 9 17 4 12"></polyline>
            </svg>
          </div>
          <h3>{{ dados.titulo }}</h3>
        </div>

        <div class="modal-body">
          <p class="mensagem-principal">{{ dados.mensagem }}</p>
          <p *ngIf="dados.detalhe" class="mensagem-detalhe">{{ dados.detalhe }}</p>
        </div>

        <div class="modal-footer">
          <button class="btn btn-success" (click)="fechar()">OK, Continuar</button>
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
      background: #1a2f3a;
      border: 1px solid #294656;
      border-radius: 1rem;
      width: 100%;
      max-width: 440px;
      overflow: hidden;
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.5);
      text-align: center;
    }

    .modal-header {
      padding: 1.5rem 1.5rem 0.5rem 1.5rem;
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 0.75rem;
    }

    .success-icon-circle {
      width: 3.5rem;
      height: 3.5rem;
      background: rgba(16, 185, 129, 0.15);
      border: 2px solid rgba(16, 185, 129, 0.4);
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      color: #34d399;
      box-shadow: 0 0 15px rgba(16, 185, 129, 0.25);
    }

    .check-icon {
      width: 1.75rem;
      height: 1.75rem;
    }

    .modal-header h3 {
      margin: 0;
      color: #f8fafc;
      font-size: 1.25rem;
      font-weight: 700;
    }

    .modal-body {
      padding: 0.75rem 1.5rem 1.5rem 1.5rem;
    }

    .mensagem-principal {
      color: #cbd5e1;
      margin: 0 0 0.5rem 0;
      line-height: 1.5;
      font-size: 0.9375rem;
    }

    .mensagem-detalhe {
      color: #94a3b8;
      font-size: 0.8125rem;
      margin: 0;
    }

    .modal-footer {
      padding: 1rem 1.5rem;
      background: #162832;
      border-top: 1px solid #294656;
      display: flex;
      justify-content: center;
    }

    .btn-success {
      background: #10b981;
      color: #ffffff;
      border: none;
      padding: 0.625rem 1.75rem;
      border-radius: 0.5rem;
      font-weight: 600;
      cursor: pointer;
      font-size: 0.9375rem;
      transition: background-color 0.2s, transform 0.1s;
    }

    .btn-success:hover {
      background: #059669;
      transform: translateY(-1px);
    }

    @keyframes fadeIn {
      from { opacity: 0; transform: scale(0.95); }
      to { opacity: 1; transform: scale(1); }
    }
  `]
})
export class SuccessModalComponent {
  @Input() dados: MensagemSucesso | null = null;
  @Output() aoFechar = new EventEmitter<void>();

  fechar(): void {
    this.dados = null;
    this.aoFechar.emit();
  }
}
