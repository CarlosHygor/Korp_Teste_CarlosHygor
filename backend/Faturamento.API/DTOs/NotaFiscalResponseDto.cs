namespace Faturamento.API.DTOs;

public record NotaFiscalResponseDto(
    int Id,
    long Numeracao,
    string Status,
    DateTime DataCriacao,
    List<ItemNotaFiscalResponseDto> Itens
);
