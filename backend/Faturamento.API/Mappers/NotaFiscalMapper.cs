using Faturamento.API.DTOs;
using Faturamento.API.Models;

namespace Faturamento.API.Mappers;

public static class NotaFiscalMapper
{
    // Converte CreateNotaFiscalDto para a entidade NotaFiscal
    public static NotaFiscal ToEntity(this CreateNotaFiscalDto dto)
    {
        return new NotaFiscal
        {
            Status = StatusNotaFiscal.Aberta,
            DataCriacao = DateTime.UtcNow,
            Itens = dto.Itens.Select(i => i.ToEntity()).ToList()
        };
    }

    // Converte CreateItemNotaFiscalDto para a entidade ItemNotaFiscal
    public static ItemNotaFiscal ToEntity(this CreateItemNotaFiscalDto dto)
    {
        return new ItemNotaFiscal
        {
            CodigoProduto = dto.CodigoProduto.Trim(),
            DescricaoProduto = dto.DescricaoProduto.Trim(),
            Quantidade = dto.Quantidade
        };
    }

    // Converte a entidade NotaFiscal para NotaFiscalResponseDto
    public static NotaFiscalResponseDto ToResponseDto(this NotaFiscal entity)
    {
        return new NotaFiscalResponseDto(
            entity.Id,
            entity.Numeracao,
            entity.Status.ToString(),
            entity.DataCriacao,
            entity.Itens.Select(i => i.ToResponseDto()).ToList()
        );
    }

    // Converte a entidade ItemNotaFiscal para ItemNotaFiscalResponseDto
    public static ItemNotaFiscalResponseDto ToResponseDto(this ItemNotaFiscal item)
    {
        return new ItemNotaFiscalResponseDto(
            item.Id,
            item.CodigoProduto,
            item.DescricaoProduto,
            item.Quantidade
        );
    }

    // Converte uma lista de entidades NotaFiscal para uma lista de NotaFiscalResponseDto
    public static IEnumerable<NotaFiscalResponseDto> ToResponseDtoList(this IEnumerable<NotaFiscal> entities)
    {
        return entities.Select(e => e.ToResponseDto());
    }
}
