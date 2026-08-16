using System.ComponentModel.DataAnnotations;

namespace Estoque.API.DTOs;

public record AbaterEstoqueDto(
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade a abater deve ser de no mínimo 1 unidade.")]
    int Quantidade
);