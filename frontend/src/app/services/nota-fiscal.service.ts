import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { NotaFiscal, CreateNotaFiscalDto } from '../models/nota-fiscal.model';
import { PagedResult } from '../models/paged-result.model';

@Injectable({
  providedIn: 'root'
})
export class NotaFiscalService {
  private readonly apiUrl = `${environment.faturamentoApiUrl}/notasfiscais`;

  constructor(private readonly http: HttpClient) {}

  /**
   * Retorna a lista paginada de Notas Fiscais do Faturamento.API.
   */
  getPaginated(pagina: number = 1, tamanhoPagina: number = 10): Observable<PagedResult<NotaFiscal>> {
    const params = new HttpParams()
      .set('pagina', pagina.toString())
      .set('tamanhoPagina', tamanhoPagina.toString());

    return this.http.get<PagedResult<NotaFiscal>>(this.apiUrl, { params });
  }

  /**
   * Obtém uma Nota Fiscal pelo seu ID único.
   */
  getById(id: number): Observable<NotaFiscal> {
    return this.http.get<NotaFiscal>(`${this.apiUrl}/${id}`);
  }

  /**
   * Obtém uma Nota Fiscal pela sua numeração sequencial.
   */
  getByNumeracao(numeracao: number): Observable<NotaFiscal> {
    return this.http.get<NotaFiscal>(`${this.apiUrl}/numeracao/${numeracao}`);
  }

  /**
   * Cadastra uma nova Nota Fiscal com numeração sequencial e status inicial 'Aberta'.
   */
  create(dto: CreateNotaFiscalDto): Observable<NotaFiscal> {
    return this.http.post<NotaFiscal>(this.apiUrl, dto);
  }

  /**
   * Processa a impressão da Nota Fiscal, efetuando baixa no Estoque e atualizando o status para 'Fechada'.
   */
  imprimir(id: number): Observable<NotaFiscal> {
    return this.http.post<NotaFiscal>(`${this.apiUrl}/${id}/imprimir`, {});
  }

  /**
   * Remove uma Nota Fiscal (apenas notas no status 'Aberta').
   */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
