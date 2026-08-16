using System.ComponentModel.DataAnnotations;

namespace Estoque.API.DTOs;

public record ProdutoDto(
    [Required(ErrorMessage = "O código do produto é obrigatório.")]
    [StringLength(50, ErrorMessage = "O código não pode exceder 50 caracteres.")]
    string Codigo,

    [Required(ErrorMessage = "A descrição do produto é obrigatória.")]
    [StringLength(200, ErrorMessage = "A descrição não pode exceder 200 caracteres.")]
    string Descricao,

    [Range(0, int.MaxValue, ErrorMessage = "O saldo do produto não pode ser negativo.")]
    int Saldo
);
