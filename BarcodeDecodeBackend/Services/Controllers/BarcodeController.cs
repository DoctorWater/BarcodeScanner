using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeBackend.Services.Processing;
using BarcodeDecodeLib.Models.Dtos.Messages.Barcode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
using Swashbuckle.AspNetCore.Annotations;

namespace BarcodeDecodeBackend.Services.Controllers;

/// <summary>Контроллер для обработки штрих‑кодов.</summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
[SwaggerTag("Операции по декодированию штрихкодов в пакетном режиме")]
public sealed class BarcodeController : ControllerBase
{
    private readonly IBarcodeMessageHandler _barcodeMessageHandler;
    private readonly ILogger<BarcodeController> _logger;

    public BarcodeController(
        IBarcodeMessageHandler barcodeMessageHandler,
        ILogger<BarcodeController> logger)
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
    [SwaggerResponse(StatusCodes.Status200OK,
        "Успешно декодирован пакет", typeof(BarcodeResponseMessageBatch))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Некорректный запрос")]
    public async Task<ActionResult<BarcodeResponseMessageBatch>> ProcessBarcodeBatch(
        [FromBody] BarcodeRequestMessageBatch request)
    {
        _logger.LogInformation("Barcode batch request received. Request: {@request}", request);

        using var timer = MetricsRegistry.BarcodeBatchDuration.NewTimer();
        MetricsRegistry.BarcodeBatchRequestsTotal.Inc();
        if (request is null || request.Messages is null)
        {
            _logger.LogWarning("Invalid barcode batch request: {@request}", request);
            MetricsRegistry.BarcodeBatchErrorsTotal.Inc();
            return BadRequest("Некорректный запрос");
        }

        var decoded = await _barcodeMessageHandler.HandleBarcodes(
            request.CorrelationId ?? Guid.NewGuid(),
            request.Messages.Select(x => x.BarcodeText));

        _logger.LogInformation(
            "Barcode batch with id {CorrelationId} processed.", request.CorrelationId);
        
        MetricsRegistry.BarcodeBatchItemsProcessedTotal.Inc(decoded.Messages.Count);

        return Ok(decoded);
    }
}
