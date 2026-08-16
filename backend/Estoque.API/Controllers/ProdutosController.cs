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

    /// <summary>A
    /// Retorna a lista de todos os produtos cadastrados no estoque.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProdutoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var produtos = await _produtoService.GetAllAsync();
        return Ok(produtos.ToResponseDtoList());
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
        try
        {
            var entidade = dto.ToEntity();
            var produtoCriado = await _produtoService.CreateAsync(entidade);
            var responseDto = produtoCriado.ToResponseDto();

            return CreatedAtAction(nameof(GetById), new { id = responseDto.Id }, responseDto);
        }
        catch (CodigoProdutoDuplicadoException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
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
        try
        {
            var entidade = dto.ToEntity();
            await _produtoService.UpdateAsync(id, entidade);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (CodigoProdutoDuplicadoException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Remove um produto do estoque pelo ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _produtoService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
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
        try
        {
            await _produtoService.AbaterEstoqueAsync(codigo, dto.Quantidade);
            return Ok(new { mensagem = $"Estoque do produto '{codigo}' abatido em {dto.Quantidade} unidade(s) com sucesso." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (EstoqueInsuficienteException ex)
        {
            return UnprocessableEntity(new
            {
                mensagem = ex.Message,
                codigoProduto = ex.CodigoProduto,
                saldoAtual = ex.SaldoAtual,
                quantidadeSolicitada = ex.QuantidadeSolicitada
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
