﻿using BarcodeDecodeLib.Models.Dtos.Messages.Barcode;

namespace BarcodeDecodeBackend.Services.Interfaces;

public interface IBarcodeMessageHandler
{
    Task<BarcodeResponseMessageBatch> HandleBarcodes(Guid correlationId, IEnumerable<string> barcodes);
}