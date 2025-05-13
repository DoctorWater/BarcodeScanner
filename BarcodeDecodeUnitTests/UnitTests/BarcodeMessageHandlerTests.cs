using BarcodeDecodeBackend.Services.Processing;
using BarcodeDecodeDataAccess.Interfaces;
using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;
using BarcodeDecodeLib.Models.Enums;
using Moq;
using NUnit.Framework.Legacy;

namespace BarcodeDecodeUnitTests.UnitTests;

[TestFixture]
public class BarcodeMessageHandlerTests : TestBase
{
    private Mock<ITransportStorageUnitRepository> _storageRepoMock;
    private Mock<ITransportOrderRepository> _orderRepoMock;
    private BarcodeMessageHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _storageRepoMock = new Mock<ITransportStorageUnitRepository>();
        _orderRepoMock = new Mock<ITransportOrderRepository>();
        _handler = new BarcodeMessageHandler(
            _storageRepoMock.Object,
            _orderRepoMock.Object
        );
    }

    [Test]
    public async Task HandleBarcodes_ShouldReturnEmptyBatch_WhenNoBarcodes()
    {
        var batch = await _handler.HandleBarcodes(Guid.NewGuid(), Enumerable.Empty<string>());

        Assert.That(batch, Is.Not.Null);
        Assert.That(batch.Messages, Is.Empty);
    }

    [Test]
    public async Task HandleBarcodes_ShouldReturnModelsWithEmptyCollections_WhenReposReturnNothing()
    {
        var barcodes = new[] { "BC1", "BC2" };
        _storageRepoMock
            .Setup(r => r.GetByBarcode(It.IsAny<string>()))
            .Returns(Enumerable.Empty<TransportStorageUnit>());
        _orderRepoMock
            .Setup(r => r.GetByBarcode(It.IsAny<string>()))
            .Returns(Enumerable.Empty<TransportOrder>());

        var batch = await _handler.HandleBarcodes(Guid.NewGuid(), barcodes);

        Assert.Multiple(() =>
        {
            Assert.That(batch.Messages, Has.Count.EqualTo(2));
            foreach (var model in batch.Messages)
            {
                Assert.That(model.TransportStorageUnits, Is.Empty);
                Assert.That(model.TransportOrders, Is.Empty);
            }
        });
    }

    [Test]
    public async Task HandleBarcodes_ShouldMapDataFromReposToResponseMessages()
    {
        var cid = Guid.NewGuid();
        const string _barcode = "BC123";

        var tsuList = new[]
        {
            new TransportStorageUnit(_barcode),
            new TransportStorageUnit(_barcode) { }
        };
        var orderList = new[]
        {
            CreateTransportOrder(_barcode)
        };

        _storageRepoMock.Setup(r => r.GetByBarcode(_barcode)).Returns(tsuList);
        _orderRepoMock.Setup(r => r.GetByBarcode(_barcode)).Returns(orderList);

        var batch = await _handler.HandleBarcodes(cid, new[] { _barcode });

        Assert.Multiple(() =>
        {
            Assert.That(batch.Messages, Has.Count.EqualTo(1));
            var model = batch.Messages.Single();

            Assert.That(model.TransportStorageUnits.Count(), Is.EqualTo(tsuList.Length));
            Assert.That(model.TransportOrders.Count(), Is.EqualTo(orderList.Length));

            foreach (var tsuMsg in model.TransportStorageUnits.OfType<TsuResponseMessage>())
                Assert.That(tsuMsg.CorrelationId, Is.EqualTo(cid));

            foreach (var orderMsg in model.TransportOrders.OfType<TransportOrderResponseMessage>())
                Assert.That(orderMsg.CorrelationId, Is.EqualTo(cid));
        });
    }

    [Test]
    public async Task HandleBarcodes_ShouldProcessMultipleBarcodes_Independently()
    {
        var cid = Guid.NewGuid();
        var barcode1 = "B1";
        var barcode2 = "B2";

        _storageRepoMock.Setup(r => r.GetByBarcode(barcode1))
            .Returns(new[] { new TransportStorageUnit(barcode1)});
        _orderRepoMock.Setup(r => r.GetByBarcode(barcode1))
            .Returns(Enumerable.Empty<TransportOrder>());

        _storageRepoMock.Setup(r => r.GetByBarcode(barcode2))
            .Returns(Enumerable.Empty<TransportStorageUnit>());
        _orderRepoMock.Setup(r => r.GetByBarcode(barcode2))
            .Returns(new[] { CreateTransportOrder(barcode2) });

        var batch = await _handler.HandleBarcodes(cid, new[] { barcode1, barcode2 });
        Assert.Multiple(() =>
        {
            Assert.That(batch.Messages, Has.Count.EqualTo(2));

            var first = batch.Messages[0];
            var second = batch.Messages[1];
        
            Assert.That(first.TransportStorageUnits.Count(), Is.EqualTo(1));
            Assert.That(first.TransportOrders, Is.Empty);

            Assert.That(second.TransportStorageUnits, Is.Empty);
            Assert.That(second.TransportOrders.Count(), Is.EqualTo(1)); 
        });
        
    }
}