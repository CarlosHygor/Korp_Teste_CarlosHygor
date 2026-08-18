namespace Estoque.API.DTOs;

public record AbaterEstoqueLoteRequestDto(
    string? IdempotencyKey,
    List<AbaterItemEstoqueDto>? Itens
);
