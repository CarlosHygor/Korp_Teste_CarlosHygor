namespace Estoque.API.DTOs;

public record ProdutoResponseDto(
    int Id,
    string Codigo,
    string Descricao,
    int Saldo
);
