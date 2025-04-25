using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BarcodeDecodeBackend.Services.Controllers;

/// <summary>
/// Операции по управлению TSU (Transport Storage Unit).
/// </summary>
[Controller]
[Route("api/[controller]")]
public class TsuController : ControllerBase
{
    private readonly ITsuMessageHandler _tsuMessageHandler;

    public TsuController(ITsuMessageHandler tsuMessageHandler)
    {
        _tsuMessageHandler = tsuMessageHandler;
    }

    /// <summary>
    /// Изменение параметров TSU.
    /// </summary>
    /// <remarks>
    /// Принимает DTO с данными для изменения TSU.  
    /// Если TSU с указанными параметрами не найден, возвращает 404 Not Found.
    /// </remarks>
    /// <param name="request">DTO <see cref="TsuChangeMessage"/>, содержащий информацию об изменениях.</param>
    /// <returns>
    /// Обновлённый объект <see cref="TsuResponseMessage"/> при успешном обновлении,  
    /// 404 Not Found, если TSU не найден.
    /// </returns>
    [HttpPost("change")]
    [SwaggerOperation(
        Summary     = "Изменить TSU",
        Description = "Обрабатывает запрос на изменение TSU и возвращает обновлённый объект.",
        Tags        = new[] { "TSU" }
    )]
    [SwaggerResponse(200, "TSU успешно изменён", typeof(TsuResponseMessage))]
    [SwaggerResponse(404, "TSU не найден")]
    public async Task<ActionResult<TsuResponseMessage>> ProcessTsuChange(
        [FromBody] TsuChangeMessage request)
    {
        var updateResult = await _tsuMessageHandler.HandleTsuChange(request);
        if (updateResult is null)
            return NotFound("Tsu not found");

        return Ok(new TsuResponseMessage(request.CorrelationId, updateResult));
    }
}