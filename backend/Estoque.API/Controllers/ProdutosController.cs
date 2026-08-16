using Estoque.API.DTOs;
using Estoque.API.Exceptions;
using Estoque.API.Mappers;
using Estoque.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    // Injeção de Dependência do IProdutoService via construtor
    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    /// <summary>
    /// Retorna a lista paginada de produtos cadastrados no estoque.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<ProdutoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10)
    {
        var resultadoPaginado = await _produtoService.GetPaginatedAsync(pagina, tamanhoPagina);

        var dtoPaginado = new PagedResultDto<ProdutoResponseDto>(
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
    /// Obtém os detalhes de um produto pelo seu ID único.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var produto = await _produtoService.GetByIdAsync(id);
        if (produto == null)
        {
            return NotFound(new { mensagem = $"Produto com ID {id} não foi encontrado." });
        }

        return Ok(produto.ToResponseDto());
    }

    /// <summary>
    /// Obtém os detalhes de um produto pelo seu código de referência.
    /// </summary>
    [HttpGet("codigo/{codigo}")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCodigo(string codigo)
    {
        var produto = await _produtoService.GetByCodigoAsync(codigo);
        if (produto == null)
        {
            return NotFound(new { mensagem = $"Produto com código '{codigo}' não foi encontrado." });
        }

        return Ok(produto.ToResponseDto());
    }

    /// <summary>
    /// Cadastra um novo produto no estoque.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] ProdutoDto dto)
    {
        var entidade = dto.ToEntity();
        var produtoCriado = await _produtoService.CreateAsync(entidade);
        var responseDto = produtoCriado.ToResponseDto();

        return CreatedAtAction(nameof(GetById), new { id = responseDto.Id }, responseDto);
    }

    /// <summary>
    /// Atualiza os dados de um produto existente.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] ProdutoDto dto)
    {
        var entidade = dto.ToEntity();
        await _produtoService.UpdateAsync(id, entidade);
        return NoContent();
    }

    /// <summary>
    /// Remove um produto do estoque pelo ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _produtoService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Abate a quantidade de saldo de um produto em estoque (Utilizado na impressão de Notas Fiscais).
    /// </summary>
    [HttpPost("{codigo}/abater")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AbaterEstoque(string codigo, [FromBody] AbaterEstoqueDto dto)
    {
        await _produtoService.AbaterEstoqueAsync(codigo, dto.Quantidade);
        return Ok(new { mensagem = $"Estoque do produto '{codigo}' abatido em {dto.Quantidade} unidade(s) com sucesso." });
    }

    /// <summary>
    /// Abate o saldo de múltiplos produtos em lote com garantia de transação atômica (Tudo ou Nada).
    /// </summary>
    [HttpPost("abater-lote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AbaterEstoqueLote([FromBody] List<AbaterItemEstoqueDto> itens)
    {
        await _produtoService.AbaterEstoqueLoteAsync(itens);
        return Ok(new { mensagem = $"Abate em lote de {itens?.Count ?? 0} produto(s) realizado com sucesso no estoque." });
    }

    /// <summary>
    /// Reverte/Estorna o saldo de múltiplos produtos em lote (Ação Compensatória em caso de falha no Faturamento).
    /// </summary>
    [HttpPost("estornar-lote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EstornarEstoqueLote([FromBody] List<AbaterItemEstoqueDto> itens)
    {
        await _produtoService.EstornarEstoqueLoteAsync(itens);
        return Ok(new { mensagem = $"Estorno de estoque em lote de {itens?.Count ?? 0} produto(s) realizado com sucesso." });
    }
}
