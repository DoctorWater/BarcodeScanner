using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Enums;

namespace BarcodeDecodeUnitTests.UnitTests;

public class TestBase
{
    protected TransportOrder CreateTransportOrder(string barcode)
    {
        return new TransportOrder(barcode, Guid.NewGuid().ToString(), new List<int>(){1, 2}, DateTimeOffset.Now, TransportOrderStatusEnum.Created);
    }
}