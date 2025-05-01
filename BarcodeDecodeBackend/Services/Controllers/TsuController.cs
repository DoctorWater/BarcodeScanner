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
    private readonly ILogger<TsuController> _logger;

    public TsuController(ITsuMessageHandler tsuMessageHandler, ILogger<TsuController> logger)
    {
        _tsuMessageHandler = tsuMessageHandler;
        _logger = logger;
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
        _logger.LogInformation("Tsu change request was received. Request: {request}", request);
        var updateResult = await _tsuMessageHandler.HandleTsuChange(request);
        if (updateResult is null)
        {
            _logger.LogWarning("Tsu with id {tsuId} was not found", request.Id);
            return NotFound("Tsu not found");
        }

        _logger.LogInformation("Tsu was changed. New tsu: {result}", updateResult);

        return Ok(new TsuResponseMessage(request.CorrelationId, updateResult));
    }
}