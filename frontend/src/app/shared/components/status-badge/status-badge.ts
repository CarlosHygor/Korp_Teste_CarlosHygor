import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span class="badge" [ngClass]="badgeClass">
      <span class="badge-dot"></span>
      {{ statusText }}
    </span>
  `,
  styles: [`
    .badge {
      display: inline-flex;
      align-items: center;
      gap: 0.375rem;
      padding: 0.25rem 0.625rem;
      border-radius: 9999px;
      font-size: 0.75rem;
      font-weight: 600;
      letter-spacing: 0.025em;
      text-transform: uppercase;
    }

    .badge-dot {
      width: 0.375rem;
      height: 0.375rem;
      border-radius: 50%;
    }

    .badge-aberta {
      background-color: rgba(245, 158, 11, 0.15);
      color: #fbbf24;
      border: 1px solid rgba(245, 158, 11, 0.3);
    }
    .badge-aberta .badge-dot {
      background-color: #f59e0b;
      box-shadow: 0 0 6px #f59e0b;
    }

    .badge-fechada {
      background-color: rgba(16, 185, 129, 0.15);
      color: #34d399;
      border: 1px solid rgba(16, 185, 129, 0.3);
    }
    .badge-fechada .badge-dot {
      background-color: #10b981;
      box-shadow: 0 0 6px #10b981;
    }
  `]
})
export class StatusBadgeComponent {
  @Input() status: string | number = 'Aberta';

  get statusText(): string {
    const s = String(this.status).toLowerCase();
    if (s === '1' || s === 'fechada') return 'Fechada';
    return 'Aberta';
  }

  get badgeClass(): string {
    return this.statusText === 'Fechada' ? 'badge-fechada' : 'badge-aberta';
  }
}
