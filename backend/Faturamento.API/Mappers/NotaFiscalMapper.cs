using Faturamento.API.Builders;
using Faturamento.API.DTOs;
using Faturamento.API.Models;

namespace Faturamento.API.Mappers;

public static class NotaFiscalMapper
{
    // Converte CreateNotaFiscalDto para a entidade NotaFiscal utilizando os Builders
    public static NotaFiscal ToEntity(this CreateNotaFiscalDto dto)
    {
        var builder = new NotaFiscalBuilder()
            .ComStatus(StatusNotaFiscal.Aberta)
            .ComDataCriacao(DateTime.UtcNow);

        if (dto.Itens != null)
        {
            foreach (var itemDto in dto.Itens)
            {
                builder.ComItem(itemDto.ToEntity());
            }
        }

        return builder.Build();
    }

    // Converte CreateItemNotaFiscalDto para a entidade ItemNotaFiscal utilizando o ItemNotaFiscalBuilder
    public static ItemNotaFiscal ToEntity(this CreateItemNotaFiscalDto dto)
    {
        return new ItemNotaFiscalBuilder()
            .ComCodigoProduto(dto.CodigoProduto.Trim())
            .ComDescricaoProduto(dto.DescricaoProduto.Trim())
            .ComQuantidade(dto.Quantidade)
            .Build();
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
