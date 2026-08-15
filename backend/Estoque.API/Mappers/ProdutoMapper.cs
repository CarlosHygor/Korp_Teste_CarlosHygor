using Estoque.API.DTOs;
using Estoque.API.Models;

namespace Estoque.API.Mappers;

public static class ProdutoMapper
{
    // Converte ProdutoDto (record) para a entidade Produto
    public static Produto ToEntity(this ProdutoDto dto)
    {
        return new Produto
        {
            Codigo = dto.Codigo.Trim(),
            Descricao = dto.Descricao.Trim(),
            Saldo = dto.Saldo
        };
    }

    // Converte a entidade Produto para ProdutoResponseDto (record)
    public static ProdutoResponseDto ToResponseDto(this Produto entity)
    {
        return new ProdutoResponseDto(
            entity.Id,
            entity.Codigo,
            entity.Descricao,
            entity.Saldo
        );
    }

    // Converte uma coleção de entidades Produto para uma lista de ProdutoResponseDto (record)
    public static IEnumerable<ProdutoResponseDto> ToResponseDtoList(this IEnumerable<Produto> entities)
    {
        return entities.Select(e => e.ToResponseDto());
    }
}
