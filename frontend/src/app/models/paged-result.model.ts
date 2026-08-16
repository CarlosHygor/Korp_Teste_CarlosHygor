export interface PagedResult<T> {
  itens: T[];
  paginaAtual: number;
  tamanhoPagina: number;
  totalRegistros: number;
  totalPaginas: number;
  temPaginaAnterior: boolean;
  temProximaPagina: boolean;
}
