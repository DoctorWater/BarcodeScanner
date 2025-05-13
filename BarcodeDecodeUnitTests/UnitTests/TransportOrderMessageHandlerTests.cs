using BarcodeDecodeBackend.Services.Processing;
using BarcodeDecodeDataAccess.Interfaces;
using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using Moq;

namespace BarcodeDecodeUnitTests.UnitTests;

[TestFixture]
public class TransportOrderMessageHandlerTests : TestBase
{
    private Mock<ITransportOrderRepository> _orderRepoMock;
    private TransportOrderMessageHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _orderRepoMock = new Mock<ITransportOrderRepository>();
        _handler = new TransportOrderMessageHandler(_orderRepoMock.Object);
    }

    [Test]
    public async Task HandleOrderChange_ShouldReturnOrder_WhenRepositoryReturnsOrder()
    {
        // Arrange
        var msg = new TransportOrderChangeMessage(1);
        var expectedOrder = CreateTransportOrder("M230991");
        _orderRepoMock
            .Setup(r => r.Update(msg))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _handler.HandleOrderChange(msg);

        // Assert
        Assert.That(result, Is.EqualTo(expectedOrder));
        _orderRepoMock.Verify(r => r.Update(msg), Times.Once);
    }

    [Test]
    public async Task HandleOrderChange_ShouldReturnNull_WhenRepositoryReturnsNull()
    {
        // Arrange
        var msg = new TransportOrderChangeMessage(1);
        _orderRepoMock
            .Setup(r => r.Update(msg))
            .ReturnsAsync((TransportOrder?)null);

        // Act
        var result = await _handler.HandleOrderChange(msg);

        // Assert
        Assert.That(result, Is.Null);
        _orderRepoMock.Verify(r => r.Update(msg), Times.Once);
    }

    [Test]
    public async Task HandleOrderRelaunch_ShouldReturnTrue_WhenRepositoryReturnsTrue()
    {
        // Arrange
        var msg = new TransportOrderRelaunchMessage("M231445")
        {
        };
        _orderRepoMock
            .Setup(r => r.Relaunch(msg))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.HandleOrderRelaunch(msg);

        // Assert
        Assert.That(result, Is.True);
        _orderRepoMock.Verify(r => r.Relaunch(msg), Times.Once);
    }

    [Test]
    public async Task HandleOrderRelaunch_ShouldReturnFalse_WhenRepositoryReturnsFalse()
    {
        // Arrange
        var msg = new TransportOrderRelaunchMessage("M231445");
        _orderRepoMock
            .Setup(r => r.Relaunch(msg))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.HandleOrderRelaunch(msg);

        // Assert
        Assert.That(result, Is.False);
        _orderRepoMock.Verify(r => r.Relaunch(msg), Times.Once);
    }
}