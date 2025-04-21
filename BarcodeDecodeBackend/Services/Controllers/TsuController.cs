using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;
using Microsoft.AspNetCore.Mvc;

namespace BarcodeDecodeBackend.Services.Controllers;

[Controller]
[Route("api/[controller]")]
public class TsuController : ControllerBase
{
    private readonly ITsuMessageHandler _tsuMessageHandler;

    public TsuController(ITsuMessageHandler tsuMessageHandler)
    {
        _tsuMessageHandler = tsuMessageHandler;
    }

    [HttpPost("change")]
    public async Task<ActionResult<TsuResponseDto>> ProcessTsuChange([FromBody] TsuChangeMessage request)
    {
        var updateResult =await _tsuMessageHandler.HandleTsuChange(request);
        if(updateResult is null)
            return NotFound("Tsu not found");
        return Ok(new TsuResponseDto(updateResult));
    }
}