using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;
using Microsoft.AspNetCore.Mvc;

namespace BarcodeDecodeBackend.Services.Controllers;

[Controller]
[Route("api/order")]
public class TransportOrderController : ControllerBase
{
    private readonly ITransportOrderMessageHandler _orderMessageHandler;

    public TransportOrderController(ITransportOrderMessageHandler tsuMessageHandler)
    {
        _orderMessageHandler = tsuMessageHandler;
    }

    [HttpPost("change")]
    public async Task<ActionResult<TsuResponseDto>> ProcessTsuChange([FromBody] TransportOrderChangeMessage request)
    {
        var updateResult =await _orderMessageHandler.HandleOrderChange(request);
        if(updateResult is null)
            return NotFound("Order not found");
        return Ok(new TransportOrderResponseDto(updateResult));
    }
}