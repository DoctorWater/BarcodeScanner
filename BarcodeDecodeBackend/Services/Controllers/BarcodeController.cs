using System.Net;
using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeLib.Models.Dtos.Messages;
using BarcodeDecodeLib.Models.Dtos.Messages.Barcode;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BarcodeDecodeBackend.Services.Controllers;

/// <summary>
/// Контроллер для обработки штрихкодов.
/// </summary>
[Controller]
[Route("api/[controller]")]
[SwaggerTag("Операции по декодированию штрихкодов в пакетном режиме")]
public class BarcodeController : ControllerBase
{
    private readonly IBarcodeMessageHandler _barcodeMessageHandler;
    private readonly ILogger<BarcodeController> _logger;

    public BarcodeController(IBarcodeMessageHandler barcodeMessageHandler, ILogger<BarcodeController> logger)
    {
        _barcodeMessageHandler = barcodeMessageHandler;
        _logger = logger;
    }

    /// <summary>
    /// Обработка пакета штрихкодов.
    /// </summary>
    /// <param name="request">
    /// Объект запроса, содержащий:
    /// <list type="bullet">
    ///   <item><c>CorrelationId</c> — уникальный Guid-идентификатор для трассировки;</item>
    ///   <item><c>Messages</c> — коллекция сообщений с полем <c>BarcodeText</c>.</item>
    /// </list>
    /// </param>
    /// <returns>
    /// Объект <see cref="BarcodeResponseMessageBatch"/>, содержащий ТСУ и ордера, соответствующие штрихкоду,
    /// и копию <c>CorrelationId</c>.
    /// </returns>
    /// <response code="200">Пакет успешно обработан и возвращён результат.</response>
    /// <response code="400">Запрос некорректен: отсутствуют данные или Messages = null.</response>
    [HttpPost("batch")]
    [SwaggerOperation(
        Summary = "Обработка пакета штрихкодов",
        Description = "Декодирует все штрихкоды из запроса и возвращает пакет с результатами.")]
    [SwaggerResponse(200, "Успешно декодирован пакет", typeof(BarcodeResponseMessageBatch))]
    [SwaggerResponse(400, "BadRequest — некорректный запрос")]
    public async Task<ActionResult<BarcodeResponseMessageBatch>> ProcessBarcodeBatch(
        [FromBody] BarcodeRequestMessageBatch request)
    {
        _logger.LogInformation("Barcode batch request was received. Request: {request}", request);
        if (request == null || request.Messages == null)
        {
            _logger.LogWarning("Barcode batch request is invalid. Request: {request}", request);
            return BadRequest("Некорректный запрос");
        }
        
        var decoded = await _barcodeMessageHandler.HandleBarcodes(
            request.CorrelationId ?? Guid.NewGuid(), request.Messages.Select(x => x.BarcodeText));
        _logger.LogInformation("Barcode batch request with id {correlationId} was processed successfully.", request.CorrelationId);
        
        return Ok(decoded);
    }
}