using BarcodeDecodeBackend.Services.Processing;
using BarcodeDecodeDataAccess.Interfaces;
using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;
using Moq;

namespace BarcodeDecodeUnitTests.UnitTests;

[TestFixture]
public class TsuMessageHandlerTests
{
    private Mock<ITransportStorageUnitRepository> _tsuRepoMock;
    private TsuMessageHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _tsuRepoMock = new Mock<ITransportStorageUnitRepository>();
        _handler = new TsuMessageHandler(_tsuRepoMock.Object);
    }

    [Test]
    public async Task HandleTsuChange_ShouldReturnTsu_WhenRepositoryReturnsTsu()
    {
        // Arrange
        var msg = new TsuChangeMessage(1)
        {
        };
        var expectedTsu = new TransportStorageUnit("M231445");
        _tsuRepoMock
            .Setup(r => r.Update(msg))
            .ReturnsAsync(expectedTsu);

        // Act
        var result = await _handler.HandleTsuChange(msg);

        // Assert
        Assert.That(result, Is.EqualTo(expectedTsu));
        _tsuRepoMock.Verify(r => r.Update(msg), Times.Once);
    }

    [Test]
    public async Task HandleTsuChange_ShouldReturnNull_WhenRepositoryReturnsNull()
    {
        // Arrange
        var msg = new TsuChangeMessage(1);
        _tsuRepoMock
            .Setup(r => r.Update(msg))
            .ReturnsAsync((TransportStorageUnit?)null);

        // Act
        var result = await _handler.HandleTsuChange(msg);

        // Assert
        Assert.That(result, Is.Null);
        _tsuRepoMock.Verify(r => r.Update(msg), Times.Once);
    }
}