using Faturamento.API.DTOs;
using Faturamento.API.Mappers;
using Faturamento.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Faturamento.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class NotasFiscaisController : ControllerBase
{
    private readonly INotaFiscalService _notaFiscalService;

    public NotasFiscaisController(INotaFiscalService notaFiscalService)
    {
        _notaFiscalService = notaFiscalService;
    }

    /// <summary>
    /// Retorna a lista paginada de todas as notas fiscais cadastradas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<NotaFiscalResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10)
    {
        var resultadoPaginado = await _notaFiscalService.GetPaginatedAsync(pagina, tamanhoPagina);

        var dtoPaginado = new PagedResultDto<NotaFiscalResponseDto>(
            resultadoPaginado.Itens.ToResponseDtoList(),
            resultadoPaginado.PaginaAtual,
            resultadoPaginado.TamanhoPagina,
            resultadoPaginado.TotalRegistros,
            resultadoPaginado.TotalPaginas,
            resultadoPaginado.TemPaginaAnterior,
            resultadoPaginado.TemProximaPagina
        );

        return Ok(dtoPaginado);
    }

    /// <summary>
    /// Obtém os detalhes de uma nota fiscal pelo seu ID único.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(NotaFiscalResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var nota = await _notaFiscalService.GetByIdAsync(id);
        if (nota == null)
        {
            return NotFound(new { mensagem = $"Nota Fiscal com ID {id} não foi encontrada." });
        }

        return Ok(nota.ToResponseDto());
    }

    /// <summary>
    /// Obtém os detalhes de uma nota fiscal pela sua numeração sequencial única.
    /// </summary>
    [HttpGet("numeracao/{numeracao:long}")]
    [ProducesResponseType(typeof(NotaFiscalResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByNumeracao(long numeracao)
    {
        var nota = await _notaFiscalService.GetByNumeracaoAsync(numeracao);
        if (nota == null)
        {
            return NotFound(new { mensagem = $"Nota Fiscal com numeração '{numeracao}' não foi encontrada." });
        }

        return Ok(nota.ToResponseDto());
    }

    /// <summary>
    /// Cadastra uma nova nota fiscal com numeração sequencial e status inicial Aberta.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(NotaFiscalResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateNotaFiscalDto dto)
    {
        var entidade = dto.ToEntity();
        var notaCriada = await _notaFiscalService.CreateAsync(entidade);
        var responseDto = notaCriada.ToResponseDto();

        return CreatedAtAction(nameof(GetById), new { id = responseDto.Id }, responseDto);
    }

    /// <summary>
    /// Processa a impressão da nota fiscal, abatendo o estoque dos produtos e alterando o status para Fechada.
    /// </summary>
    [HttpPost("{id:int}/imprimir")]
    [ProducesResponseType(typeof(NotaFiscalResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Imprimir(int id)
    {
        var notaImpressa = await _notaFiscalService.ImprimirAsync(id);
        return Ok(notaImpressa.ToResponseDto());
    }

    /// <summary>
    /// Remove uma nota fiscal pelo ID (apenas notas no status Aberta).
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _notaFiscalService.DeleteAsync(id);
        return NoContent();
    }
}
