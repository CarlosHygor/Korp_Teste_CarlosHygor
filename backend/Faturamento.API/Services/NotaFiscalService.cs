using Faturamento.API.Clients;
using Faturamento.API.Clients.DTOs;
using Faturamento.API.Models;
using Faturamento.API.Repositories;

namespace Faturamento.API.Services;

public class NotaFiscalService : INotaFiscalService
{
    private readonly INotaFiscalRepository _notaFiscalRepository;
    private readonly IEstoqueClient _estoqueClient;

    public NotaFiscalService(INotaFiscalRepository notaFiscalRepository, IEstoqueClient estoqueClient)
    {
        _notaFiscalRepository = notaFiscalRepository;
        _estoqueClient = estoqueClient;
    }

    public async Task<IEnumerable<NotaFiscal>> GetAllAsync()
    {
        return await _notaFiscalRepository.GetAllAsync();
    }

    public async Task<NotaFiscal?> GetByIdAsync(int id)
    {
        return await _notaFiscalRepository.GetByIdAsync(id);
    }

    public async Task<NotaFiscal?> GetByNumeracaoAsync(long numeracao)
    {
        return await _notaFiscalRepository.GetByNumeracaoAsync(numeracao);
    }

    public async Task<NotaFiscal> CreateAsync(NotaFiscal notaFiscal)
    {
        if (notaFiscal.Itens == null || !notaFiscal.Itens.Any())
        {
            throw new ArgumentException("A nota fiscal deve conter ao menos um item.", nameof(notaFiscal.Itens));
        }

        foreach (var item in notaFiscal.Itens)
        {
            if (string.IsNullOrWhiteSpace(item.CodigoProduto))
            {
                throw new ArgumentException("O código do produto é obrigatório para todos os itens da nota fiscal.");
            }

            if (item.Quantidade <= 0)
            {
                throw new ArgumentException($"A quantidade do produto '{item.CodigoProduto}' deve ser maior que zero.");
            }
        }

        // Garantir status inicial 'Aberta' e data UTC de criação
        notaFiscal.Status = StatusNotaFiscal.Aberta;
        notaFiscal.DataCriacao = DateTime.UtcNow;

        return await _notaFiscalRepository.CreateAsync(notaFiscal);
    }

    public async Task<NotaFiscal> ImprimirAsync(int id)
    {
        var notaFiscal = await _notaFiscalRepository.GetByIdAsync(id);
        if (notaFiscal == null)
        {
            throw new KeyNotFoundException($"Nota Fiscal com ID {id} não foi encontrada.");
        }

        // Não permitir impressão de notas que não estejam com status Aberta
        if (notaFiscal.Status != StatusNotaFiscal.Aberta)
        {
            throw new InvalidOperationException($"A Nota Fiscal nº {notaFiscal.Numeracao} já está com status '{notaFiscal.Status}' e não pode ser impressa novamente.");
        }

        if (notaFiscal.Itens == null || !notaFiscal.Itens.Any())
        {
            throw new InvalidOperationException($"A Nota Fiscal nº {notaFiscal.Numeracao} não possui itens para impressão.");
        }

        // Mapeia os itens da Nota Fiscal para a requisição de lote do Estoque.API
        var itensAbate = notaFiscal.Itens
            .Select(i => new ItemAbateEstoqueDto(i.CodigoProduto, i.Quantidade))
            .ToList();

        // 1. Envia a requisição em lote atômica para o Estoque.API
        await _estoqueClient.AbaterEstoqueLoteAsync(itensAbate);

        try
        {
            // 2. Atualiza status da nota para Fechada após confirmação atômica da baixa de estoque do lote
            notaFiscal.Status = StatusNotaFiscal.Fechada;
            await _notaFiscalRepository.UpdateAsync(notaFiscal);
        }
        catch (Exception ex)
        {
            // 3. Ação Compensatória: Se a gravação no banco do Faturamento falhar, estorna a baixa de estoque no Estoque.API
            await _estoqueClient.EstornarEstoqueLoteAsync(itensAbate);
            throw new InvalidOperationException($"Falha ao atualizar o status da Nota Fiscal no banco de dados. O abate de estoque foi revertido com sucesso.", ex);
        }

        return notaFiscal;
    }

    public async Task UpdateAsync(int id, NotaFiscal notaFiscalAtualizada)
    {
        var notaFiscalExistente = await _notaFiscalRepository.GetByIdAsync(id);
        if (notaFiscalExistente == null)
        {
            throw new KeyNotFoundException($"Nota Fiscal com ID {id} não foi encontrada.");
        }

        if (notaFiscalExistente.Status == StatusNotaFiscal.Fechada)
        {
            throw new InvalidOperationException($"Nota Fiscal com ID {id} está FECHADA e não pode ser alterada.");
        }

        notaFiscalExistente.Status = notaFiscalAtualizada.Status;
        notaFiscalExistente.Itens = notaFiscalAtualizada.Itens;

        await _notaFiscalRepository.UpdateAsync(notaFiscalExistente);
    }

    public async Task DeleteAsync(int id)
    {
        var notaFiscal = await _notaFiscalRepository.GetByIdAsync(id);
        if (notaFiscal == null)
        {
            throw new KeyNotFoundException($"Nota Fiscal com ID {id} não foi encontrada.");
        }

        if (notaFiscal.Status == StatusNotaFiscal.Fechada)
        {
            throw new InvalidOperationException($"Nota Fiscal com ID {id} está FECHADA e não pode ser excluída.");
        }

        await _notaFiscalRepository.DeleteAsync(notaFiscal);
    }
}