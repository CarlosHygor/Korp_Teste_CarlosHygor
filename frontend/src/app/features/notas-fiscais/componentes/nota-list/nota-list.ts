import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-nota-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="page-header">
      <div class="page-title">
        <h2>📄 Emissão de Notas Fiscais (Faturamento)</h2>
        <p>Gerencie, filtre e processe a impressão distribuída de Notas Fiscais com baixa de estoque.</p>
      </div>
    </div>

    <div style="background: #1e293b; padding: 2rem; border-radius: 0.75rem; border: 1px solid #334155; text-align: center; color: #94a3b8;">
      <p>💡 Layout Base e Componentes Compartilhados configurados! O Módulo completo de Notas Fiscais será implementado nos <strong>Passos 3 e 4</strong>.</p>
    </div>
  `
})
export class NotaFiscalListComponent {}
