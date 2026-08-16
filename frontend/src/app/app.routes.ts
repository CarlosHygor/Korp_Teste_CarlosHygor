import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'produtos', pathMatch: 'full' },
  { 
    path: 'produtos', 
    loadComponent: () => import('./features/produtos/componentes/produto-list/produto-list').then(m => m.ProdutoListComponent) 
  },
  { 
    path: 'notas-fiscais', 
    loadComponent: () => import('./features/notas-fiscais/componentes/nota-list/nota-list').then(m => m.NotaFiscalListComponent) 
  }
];
