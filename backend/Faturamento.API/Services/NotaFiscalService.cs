using Faturamento.API.Models;
using Faturamento.API.Repositories;

namespace Faturamento.API.Services;

public class NotaFiscalService : INotaFiscalService
{
    private readonly INotaFiscalRepository _notaFiscalRepository;
    private readonly IItemNotaFiscalService _itemNotaFiscalService;

    public NotaFiscalService(INotaFiscalRepository notaFiscalRepository, IItemNotaFiscalService itemNotaFiscalService)
    {
        _notaFiscalRepository = notaFiscalRepository;
        _itemNotaFiscalService = itemNotaFiscalService;
    }

    public async Task<IEnumerable<NotaFiscal>> GetAllAsync()
    {
        return await _notaFiscalRepository.GetAllAsync();
    }

    public async Task<NotaFiscal?> GetByIdAsync(int id)
    {
        return await _notaFiscalRepository.GetByIdAsync(id);
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

        // Percorre a Lista e delega a responsabilidade de abater cada item para o ItemNotaFiscalService 
        foreach (var item in notaFiscal.Itens)
        {
            await _itemNotaFiscalService.AbaterEstoqueAsync(item);
        }

        // Atualiza status da nota para Fechada após confirmação da baixa de estoque dos itens
        notaFiscal.Status = StatusNotaFiscal.Fechada;
        await _notaFiscalRepository.UpdateAsync(notaFiscal);

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