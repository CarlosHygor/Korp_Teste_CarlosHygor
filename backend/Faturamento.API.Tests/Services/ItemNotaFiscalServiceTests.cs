using Faturamento.API.Clients;
using Faturamento.API.Models;
using Faturamento.API.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Faturamento.API.Tests.Services;

public class ItemNotaFiscalServiceTests
{
    private readonly Mock<IEstoqueClient> _estoqueClientMock;
    private readonly ItemNotaFiscalService _service;

    public ItemNotaFiscalServiceTests()
    {
        _estoqueClientMock = new Mock<IEstoqueClient>();
        _service = new ItemNotaFiscalService(_estoqueClientMock.Object);
    }

    [Fact]
    public async Task AbaterEstoqueAsync_DeveInvocarEstoqueClient_QuandoItemForValido()
    {
        // Arrange
        var item = new ItemNotaFiscal { CodigoProduto = "PROD-001", Quantidade = 5 };

        _estoqueClientMock
            .Setup(c => c.AbaterEstoqueAsync("PROD-001", 5))
            .Returns(Task.CompletedTask);

        // Act
        await _service.AbaterEstoqueAsync(item);

        // Assert
        _estoqueClientMock.Verify(c => c.AbaterEstoqueAsync("PROD-001", 5), Times.Once);
    }

    [Fact]
    public async Task AbaterEstoqueAsync_DeveLancarArgumentException_QuandoQuantidadeForZeroOuNegativa()
    {
        // Arrange
        var itemInvalido = new ItemNotaFiscal { CodigoProduto = "PROD-001", Quantidade = 0 };

        // Act
        var act = async () => await _service.AbaterEstoqueAsync(itemInvalido);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*maior que zero*");

        _estoqueClientMock.Verify(c => c.AbaterEstoqueAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }
}
