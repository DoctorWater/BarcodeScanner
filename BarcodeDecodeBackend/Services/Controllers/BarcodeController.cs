using System.Net;
using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeLib.Models.Messages;
using Microsoft.AspNetCore.Mvc;

namespace BarcodeDecodeBackend.Services.Controllers;

[Controller]
[Route("api/[controller]")]
public class BarcodeController : ControllerBase
{
    private readonly IBarcodeMessageHandler _barcodeMessageHandler;

    public BarcodeController(IBarcodeMessageHandler barcodeMessageHandler)
    {
        _barcodeMessageHandler = barcodeMessageHandler;
    }
    
    [HttpPost("batch")]
    public async Task<ActionResult<BarcodeResponseMessageBatch>> ProcessBarcodeBatch([FromBody] BarcodeRequestMessageBatch request)
    {
        if (request == null || request.Messages == null)
        {
            return BadRequest("Некорректный запрос");
        }
            
        BarcodeResponseMessageBatch response = await _barcodeMessageHandler.HandleBarcodes(request.Messages.Select(x => x.Text));
                
        return Ok(response);
    }
}