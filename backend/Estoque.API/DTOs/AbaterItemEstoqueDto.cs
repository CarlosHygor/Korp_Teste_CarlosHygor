using System.ComponentModel.DataAnnotations;

namespace Estoque.API.DTOs;

public record AbaterItemEstoqueDto(
    [Required(ErrorMessage = "O código do produto é obrigatório.")] string CodigoProduto,
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")] int Quantidade
);
