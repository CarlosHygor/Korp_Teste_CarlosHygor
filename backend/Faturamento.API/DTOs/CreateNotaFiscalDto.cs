using System.ComponentModel.DataAnnotations;

namespace Faturamento.API.DTOs;

public record CreateNotaFiscalDto(
    [Required(ErrorMessage = "A nota fiscal deve conter ao menos um item.")]
    [MinLength(1, ErrorMessage = "A nota fiscal deve conter ao menos um item.")]
    List<CreateItemNotaFiscalDto> Itens
);
