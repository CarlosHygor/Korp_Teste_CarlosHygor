export interface Produto {
  id: number;
  codigo: string;
  descricao: string;
  saldo: number;
}

export interface ProdutoDto {
  codigo: string;
  descricao: string;
  saldo: number;
}

export interface AbaterEstoqueDto {
  quantidade: number;
}

export interface AbaterItemEstoqueDto {
  codigoProduto: string;
  quantidade: number;
}
