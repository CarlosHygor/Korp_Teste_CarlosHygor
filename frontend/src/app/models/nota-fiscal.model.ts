export type StatusNotaFiscal = 'Aberta' | 'Fechada';

export interface ItemNotaFiscal {
  id: number;
  codigoProduto: string;
  descricaoProduto?: string;
  quantidade: number;
}

export interface NotaFiscal {
  id: number;
  numeracao: number;
  status: StatusNotaFiscal;
  dataCriacao: string;
  itens: ItemNotaFiscal[];
}

export interface CreateItemNotaFiscalDto {
  codigoProduto: string;
  descricaoProduto?: string;
  quantidade: number;
}

export interface CreateNotaFiscalDto {
  itens: CreateItemNotaFiscalDto[];
}
