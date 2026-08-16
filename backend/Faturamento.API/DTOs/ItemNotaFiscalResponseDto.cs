namespace Faturamento.API.DTOs;

public record ItemNotaFiscalResponseDto(
    int Id,
    string CodigoProduto,
    string DescricaoProduto,
    int Quantidade
);
