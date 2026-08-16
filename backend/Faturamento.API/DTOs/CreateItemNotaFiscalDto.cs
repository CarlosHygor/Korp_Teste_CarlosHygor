using System.ComponentModel.DataAnnotations;

namespace Faturamento.API.DTOs;

public record CreateItemNotaFiscalDto(
    [Required(ErrorMessage = "O código do produto é obrigatório.")]
    [StringLength(50, ErrorMessage = "O código do produto não pode exceder 50 caracteres.")]
    string CodigoProduto,

    [Required(ErrorMessage = "A descrição do produto é obrigatória.")]
    [StringLength(200, ErrorMessage = "A descrição do produto não pode exceder 200 caracteres.")]
    string DescricaoProduto,

    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    int Quantidade
);
