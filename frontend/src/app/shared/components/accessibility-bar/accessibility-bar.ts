import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-accessibility-bar',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="accessibility-bar" role="region" aria-label="Ferramentas de acessibilidade">
      <!-- Ajuste de Escala de Fonte -->
      <div class="font-controls" aria-label="Controle de tamanho de fonte">
        <button 
          class="btn-acc" 
          (click)="diminuirFonte()" 
          [disabled]="escalaFonte <= 85" 
          title="Diminuir tamanho da fonte (A-)"
          aria-label="Diminuir tamanho da fonte"
        >
          A-
        </button>
        
        <button 
          class="btn-acc btn-reset" 
          (click)="restaurarFonte()" 
          title="Tamanho de fonte normal (A)"
          aria-label="Restaurar tamanho de fonte normal"
        >
          A
        </button>
        
        <button 
          class="btn-acc" 
          (click)="aumentarFonte()" 
          [disabled]="escalaFonte >= 130" 
          title="Aumentar tamanho da fonte (A+)"
          aria-label="Aumentar tamanho da fonte"
        >
          A+
        </button>
      </div>

      <div class="divider"></div>

      <!-- Toggle de Alto Contraste -->
      <button 
        class="btn-acc btn-contrast" 
        [class.active]="altoContraste"
        [attr.aria-pressed]="altoContraste"
        (click)="toggleAltoContraste()" 
        title="Alternar Modo Alto Contraste (WCAG 2.2)"
        aria-label="Alternar modo de alto contraste"
      >
        <svg class="contrast-svg" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <circle cx="12" cy="12" r="10"></circle>
          <path d="M12 18a6 6 0 0 0 0-12v12z" fill="currentColor"></path>
        </svg>
        <span>Alto Contraste</span>
      </button>
    </div>
  `,
  styles: [`
    .accessibility-bar {
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
      background: #0f1c23;
      border: 1px solid #294656;
      padding: 0.25rem 0.5rem;
      border-radius: 0.5rem;
    }

    .font-controls {
      display: flex;
      align-items: center;
      gap: 0.125rem;
    }

    .btn-acc {
      background: transparent;
      border: none;
      color: #94a3b8;
      font-size: 0.75rem;
      font-weight: 700;
      padding: 0.25rem 0.5rem;
      border-radius: 0.25rem;
      cursor: pointer;
      transition: all 0.15s ease-in-out;
      display: inline-flex;
      align-items: center;
      gap: 0.375rem;
    }

    .btn-acc:hover:not(:disabled) {
      color: #38bdf8;
      background: rgba(56, 189, 248, 0.1);
    }

    .btn-acc:focus-visible {
      outline: 2px solid #38bdf8;
      outline-offset: 1px;
    }

    .btn-acc:disabled {
      opacity: 0.3;
      cursor: not-allowed;
    }

    .btn-reset {
      font-size: 0.8125rem;
      color: #f8fafc;
    }

    .divider {
      width: 1px;
      height: 1rem;
      background-color: #294656;
    }

    .btn-contrast {
      color: #cbd5e1;
    }

    .btn-contrast.active {
      background: #ffff00;
      color: #000000;
      font-weight: 800;
      box-shadow: 0 0 10px rgba(255, 255, 0, 0.4);
    }

    .contrast-svg {
      width: 1rem;
      height: 1rem;
    }
  `]
})
export class AccessibilityBarComponent {
  escalaFonte: number = 100;
  altoContraste: boolean = false;

  aumentarFonte(): void {
    if (this.escalaFonte < 130) {
      this.escalaFonte += 10;
      this.aplicarEscalaFonte();
    }
  }

  diminuirFonte(): void {
    if (this.escalaFonte > 85) {
      this.escalaFonte -= 10;
      this.aplicarEscalaFonte();
    }
  }

  restaurarFonte(): void {
    this.escalaFonte = 100;
    this.aplicarEscalaFonte();
  }

  private aplicarEscalaFonte(): void {
    document.documentElement.style.fontSize = `${this.escalaFonte}%`;
  }

  toggleAltoContraste(): void {
    this.altoContraste = !this.altoContraste;
    if (this.altoContraste) {
      document.body.classList.add('high-contrast');
    } else {
      document.body.classList.remove('high-contrast');
    }
  }
}
