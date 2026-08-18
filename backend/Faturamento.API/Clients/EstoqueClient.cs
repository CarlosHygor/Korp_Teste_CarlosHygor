using System.Net;
using System.Net.Http.Json;
using Faturamento.API.Clients.DTOs;
using Faturamento.API.Exceptions;

namespace Faturamento.API.Clients;

public class EstoqueClient : IEstoqueClient
{
    private readonly HttpClient _httpClient;

    public EstoqueClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task AbaterEstoqueAsync(string codigoProduto, int quantidade)
    {
        try
        {
            var payload = new { quantidade };
            var response = await _httpClient.PostAsJsonAsync($"api/produtos/{Uri.EscapeDataString(codigoProduto)}/abater", payload);

            if (response.IsSuccessStatusCode)
            {
                return;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"Produto com código '{codigoProduto}' não foi encontrado no estoque.");
            }

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new NotaFiscalStatusInvalidoException($"Saldo insuficiente ou dados inválidos no produto '{codigoProduto}': {content}");
            }

            throw new ServicoEstoqueIndisponivelException($"Serviço de estoque retornou erro (HTTP Status: {(int)response.StatusCode}).");
        }
        catch (HttpRequestException ex) when (ex is not ServicoEstoqueIndisponivelException)
        {
            throw new ServicoEstoqueIndisponivelException($"Falha de comunicação com o Estoque.API ao tentar abater o produto '{codigoProduto}'. A nota fiscal permanece ABERTA.", ex);
        }
    }

    public async Task AbaterEstoqueLoteAsync(IEnumerable<ItemAbateEstoqueDto> itens, string? idempotencyKey = null)
    {
        try
        {
            var payload = new
            {
                idempotencyKey,
                itens
            };
            var response = await _httpClient.PostAsJsonAsync("api/produtos/abater-lote", payload);

            if (response.IsSuccessStatusCode)
            {
                return;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new KeyNotFoundException($"Falha de produto no estoque: {content}");
            }

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new NotaFiscalStatusInvalidoException($"Saldo insuficiente para um ou mais produtos da nota: {content}");
            }

            throw new ServicoEstoqueIndisponivelException($"Serviço de estoque retornou erro (HTTP Status: {(int)response.StatusCode}).");
        }
        catch (HttpRequestException ex) when (ex is not ServicoEstoqueIndisponivelException)
        {
            throw new ServicoEstoqueIndisponivelException("Falha de comunicação com o Estoque.API ao tentar abater o lote de produtos. A nota fiscal permanece ABERTA.", ex);
        }
    }

    public async Task EstornarEstoqueLoteAsync(IEnumerable<ItemAbateEstoqueDto> itens)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/produtos/estornar-lote", itens);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new ServicoEstoqueIndisponivelException($"Erro ao estornar lote no estoque (HTTP Status {(int)response.StatusCode}): {content}");
            }
        }
        catch (HttpRequestException ex) when (ex is not ServicoEstoqueIndisponivelException)
        {
            throw new ServicoEstoqueIndisponivelException("Falha de comunicação com o Estoque.API durante a Ação Compensatória de estorno.", ex);
        }
    }
}
