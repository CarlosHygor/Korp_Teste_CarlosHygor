import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Produto, ProdutoDto, AbaterEstoqueDto, AbaterItemEstoqueDto } from '../models/produto.model';
import { PagedResult } from '../models/paged-result.model';

@Injectable({
  providedIn: 'root'
})
export class ProdutoService {
  private readonly apiUrl = `${environment.estoqueApiUrl}/produtos`;

  constructor(private readonly http: HttpClient) {}

  /**
   * Retorna a lista paginada de produtos do Estoque.API.
   */
  getPaginated(pagina: number = 1, tamanhoPagina: number = 10): Observable<PagedResult<Produto>> {
    const params = new HttpParams()
      .set('pagina', pagina.toString())
      .set('tamanhoPagina', tamanhoPagina.toString());

    return this.http.get<PagedResult<Produto>>(this.apiUrl, { params });
  }

  /**
   * Obtém um produto pelo seu ID único.
   */
  getById(id: number): Observable<Produto> {
    return this.http.get<Produto>(`${this.apiUrl}/${id}`);
  }

  /**
   * Obtém um produto pelo seu código de referência.
   */
  getByCodigo(codigo: string): Observable<Produto> {
    return this.http.get<Produto>(`${this.apiUrl}/codigo/${encodeURIComponent(codigo)}`);
  }

  /**
   * Cadastra um novo produto no estoque.
   */
  create(dto: ProdutoDto): Observable<Produto> {
    return this.http.post<Produto>(this.apiUrl, dto);
  }

  /**
   * Atualiza os dados de um produto existente.
   */
  update(id: number, dto: ProdutoDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  /**
   * Remove um produto do estoque.
   */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Abate a quantidade de saldo de um produto em estoque.
   */
  abaterEstoque(codigo: string, quantidade: number): Observable<{ mensagem: string }> {
    const body: AbaterEstoqueDto = { quantidade };
    return this.http.post<{ mensagem: string }>(`${this.apiUrl}/${encodeURIComponent(codigo)}/abater`, body);
  }

  /**
   * Abate o saldo de múltiplos produtos em lote de forma atômica.
   */
  abaterEstoqueLote(itens: AbaterItemEstoqueDto[]): Observable<{ mensagem: string }> {
    return this.http.post<{ mensagem: string }>(`${this.apiUrl}/abater-lote`, itens);
  }
}
