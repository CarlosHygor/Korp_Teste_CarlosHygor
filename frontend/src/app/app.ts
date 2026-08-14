import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private http = inject(HttpClient);

  mensagemBackend = signal<string>('Carregando resposta da API...');
  statusConexao = signal<'sucesso' | 'erro' | 'carregando'>('carregando');

  ngOnInit(): void {
    this.testarConexao();
  }

  testarConexao(): void {
    this.statusConexao.set('carregando');
    this.http.get<{ status: string; mensagem: string }>('http://localhost:5000/api/ping').subscribe({
      next: (res) => {
        this.mensagemBackend.set(res.mensagem);
        this.statusConexao.set('sucesso');
      },
      error: () => {
        this.mensagemBackend.set('Não foi possível conectar com a API em http://localhost:5000/api/ping');
        this.statusConexao.set('erro');
      }
    });
  }
}
