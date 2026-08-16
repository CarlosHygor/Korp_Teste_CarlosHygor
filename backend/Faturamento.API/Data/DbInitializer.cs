using Faturamento.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Faturamento.API.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(FaturamentoDbContext context)
    {
        if (await context.NotasFiscais.AnyAsync())
        {
            return; // Se a tabela já possui notas fiscais, não faz nada
        }

        var baseDate = DateTime.UtcNow;

        var notas = new List<NotaFiscal>
        {
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-30),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-001", DescricaoProduto = "Notebook Dell Inspiron 15 (16GB RAM, 512GB SSD)", Quantidade = 1 },
                    new() { CodigoProduto = "PROD-002", DescricaoProduto = "Mouse Sem Fio Logitech MX Master 3S", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-29),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-003", DescricaoProduto = "Teclado Mecânico Keychron K2 RGB", Quantidade = 2 },
                    new() { CodigoProduto = "PROD-004", DescricaoProduto = "Monitor LG UltraWide 29 IPS Full HD", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Fechada,
                DataCriacao = baseDate.AddDays(-28),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-005", DescricaoProduto = "Cadeira de Escritório Ergonômica Comfy Ergofit", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-27),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-006", DescricaoProduto = "Headset Gamer HyperX Cloud II Red", Quantidade = 2 },
                    new() { CodigoProduto = "PROD-007", DescricaoProduto = "Webcam Full HD Logitech C920s Pro", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-26),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-008", DescricaoProduto = "SSD Kingston NV2 1TB M.2 NVMe", Quantidade = 3 },
                    new() { CodigoProduto = "PROD-009", DescricaoProduto = "Memória RAM Corsair Vengeance 16GB DDR4 3200MHz", Quantidade = 2 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Fechada,
                DataCriacao = baseDate.AddDays(-25),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-010", DescricaoProduto = "Processador AMD Ryzen 7 5700X", Quantidade = 1 },
                    new() { CodigoProduto = "PROD-015", DescricaoProduto = "Placa-Mãe ASUS TUF Gaming B550M-Plus", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-24),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-011", DescricaoProduto = "Placa de Vídeo RTX 4060 Ventus 8GB", Quantidade = 1 },
                    new() { CodigoProduto = "PROD-012", DescricaoProduto = "Fonte Corsair CV650 650W 80 Plus Bronze", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-23),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-013", DescricaoProduto = "Gabinete Gamer NZXT H5 Flow Black", Quantidade = 1 },
                    new() { CodigoProduto = "PROD-014", DescricaoProduto = "Water Cooler DeepCool LE520 240mm ARGB", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Fechada,
                DataCriacao = baseDate.AddDays(-22),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-016", DescricaoProduto = "Fone de Ouvido Bluetooth Sony WH-1000XM5", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-21),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-017", DescricaoProduto = "Mousepad Gamer Extra Grande 90x40cm Black", Quantidade = 3 },
                    new() { CodigoProduto = "PROD-018", DescricaoProduto = "Suporte Articulado para Monitor F80N ELG", Quantidade = 2 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-20),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-019", DescricaoProduto = "Filtro de Linha 8 Tomadas Clamper iClamper Energia 8", Quantidade = 4 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Fechada,
                DataCriacao = baseDate.AddDays(-19),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-020", DescricaoProduto = "Nobreak Intelbras Attiv 600VA 120V", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-18),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-021", DescricaoProduto = "Impressora Multifuncional Epson EcoTank L3250", Quantidade = 1 },
                    new() { CodigoProduto = "PROD-024", DescricaoProduto = "Cabo HDMI 2.1 4K 120Hz 2 Metros", Quantidade = 2 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-17),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-022", DescricaoProduto = "Roteador Wi-Fi 6 TP-Link Archer AX12", Quantidade = 2 },
                    new() { CodigoProduto = "PROD-023", DescricaoProduto = "Switch TP-Link 8 Portas Gigabit TL-SG108", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Fechada,
                DataCriacao = baseDate.AddDays(-16),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-025", DescricaoProduto = "Hub USB-C 7 em 1 Baseus Dual Type-C", Quantidade = 2 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-15),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-026", DescricaoProduto = "Mesa Gamer Com Regulagem de Altura Elétrica 140x70", Quantidade = 1 },
                    new() { CodigoProduto = "PROD-027", DescricaoProduto = "Luminária de Monitor Baseus i-Wok Stepless Dimming", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-14),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-028", DescricaoProduto = "HD Externo Portátil Seagate Expansion 2TB", Quantidade = 2 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Fechada,
                DataCriacao = baseDate.AddDays(-13),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-029", DescricaoProduto = "Pendrive SanDisk Ultra Flair 64GB USB 3.0", Quantidade = 5 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-12),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-030", DescricaoProduto = "Adaptador Bluetooth 5.0 USB TP-Link UB500", Quantidade = 3 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-11),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-031", DescricaoProduto = "Microfone Condensador Fifine K669B USB", Quantidade = 1 },
                    new() { CodigoProduto = "PROD-032", DescricaoProduto = "Braço Articulado para Microfone Neewer NB-35", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Fechada,
                DataCriacao = baseDate.AddDays(-10),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-033", DescricaoProduto = "Ring Light 10 Polegadas Com Tripé 1.60m", Quantidade = 2 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-9),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-034", DescricaoProduto = "Caixa de Som Edifier R1000T4 Bivolt 24W RMS", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-8),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-035", DescricaoProduto = "Organizador de Cabos Espiral 2 Metros Preto", Quantidade = 5 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Fechada,
                DataCriacao = baseDate.AddDays(-7),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-001", DescricaoProduto = "Notebook Dell Inspiron 15 (16GB RAM, 512GB SSD)", Quantidade = 1 },
                    new() { CodigoProduto = "PROD-008", DescricaoProduto = "SSD Kingston NV2 1TB M.2 NVMe", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-6),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-002", DescricaoProduto = "Mouse Sem Fio Logitech MX Master 3S", Quantidade = 2 },
                    new() { CodigoProduto = "PROD-003", DescricaoProduto = "Teclado Mecânico Keychron K2 RGB", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-5),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-006", DescricaoProduto = "Headset Gamer HyperX Cloud II Red", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-4),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-009", DescricaoProduto = "Memória RAM Corsair Vengeance 16GB DDR4 3200MHz", Quantidade = 2 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-3),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-012", DescricaoProduto = "Fonte Corsair CV650 650W 80 Plus Bronze", Quantidade = 1 },
                    new() { CodigoProduto = "PROD-015", DescricaoProduto = "Placa-Mãe ASUS TUF Gaming B550M-Plus", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-2),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-018", DescricaoProduto = "Suporte Articulado para Monitor F80N ELG", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddDays(-1),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-024", DescricaoProduto = "Cabo HDMI 2.1 4K 120Hz 2 Metros", Quantidade = 3 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddHours(-12),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-001", DescricaoProduto = "Notebook Dell Inspiron 15 (16GB RAM, 512GB SSD)", Quantidade = 1 },
                    new() { CodigoProduto = "PROD-004", DescricaoProduto = "Monitor LG UltraWide 29 IPS Full HD", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddHours(-6),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-007", DescricaoProduto = "Webcam Full HD Logitech C920s Pro", Quantidade = 1 },
                    new() { CodigoProduto = "PROD-031", DescricaoProduto = "Microfone Condensador Fifine K669B USB", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddHours(-3),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-010", DescricaoProduto = "Processador AMD Ryzen 7 5700X", Quantidade = 1 },
                    new() { CodigoProduto = "PROD-011", DescricaoProduto = "Placa de Vídeo RTX 4060 Ventus 8GB", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate.AddHours(-1),
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-017", DescricaoProduto = "Mousepad Gamer Extra Grande 90x40cm Black", Quantidade = 2 },
                    new() { CodigoProduto = "PROD-002", DescricaoProduto = "Mouse Sem Fio Logitech MX Master 3S", Quantidade = 1 }
                }
            },
            new()
            {
                Status = StatusNotaFiscal.Aberta,
                DataCriacao = baseDate,
                Itens = new List<ItemNotaFiscal>
                {
                    new() { CodigoProduto = "PROD-003", DescricaoProduto = "Teclado Mecânico Keychron K2 RGB", Quantidade = 1 },
                    new() { CodigoProduto = "PROD-006", DescricaoProduto = "Headset Gamer HyperX Cloud II Red", Quantidade = 1 }
                }
            }
        };

        for (int i = 0; i < notas.Count; i++)
        {
            notas[i].Numeracao = 1001 + i;
        }

        await context.NotasFiscais.AddRangeAsync(notas);
        await context.SaveChangesAsync();
    }
}
