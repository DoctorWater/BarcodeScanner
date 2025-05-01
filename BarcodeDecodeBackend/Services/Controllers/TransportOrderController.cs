using BarcodeDecodeBackend.Services.Interfaces;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BarcodeDecodeBackend.Services.Controllers;

/// <summary>
/// Операции по управлению заказами.
/// </summary>
[Controller]
[Route("api/[controller]")]
public class TransportOrderController : ControllerBase
{
    private readonly ITransportOrderMessageHandler _orderMessageHandler;
    private readonly ILogger<TransportOrderController> _logger;

    public TransportOrderController(ITransportOrderMessageHandler orderMessageHandler, ILogger<TransportOrderController> logger)
    {
        _orderMessageHandler = orderMessageHandler;
        _logger = logger;
    }

    /// <summary>
    /// Изменить параметры существующего заказа.
    /// </summary>
    /// <remarks>
    /// Принимает DTO с информацией об изменениях.  
    /// Возвращает обновлённую сущность заказа.
    /// </remarks>
    /// <param name="request">DTO с данными для изменения заказа.</param>
    /// <returns>Обновлённый заказ в теле ответа.</returns>
    [HttpPost("change")]
    [SwaggerOperation(
        Summary = "Изменить заказ",
        Description = "Обрабатывает запрос на изменение заказа и возвращает обновлённый объект заказа.",
        Tags = new[] { "TransportOrder" }
    )]
    [SwaggerResponse(200, "Заказ успешно изменён", typeof(TransportOrderResponseMessage))]
    [SwaggerResponse(404, "Заказ не найден")]
    public async Task<ActionResult<TransportOrderResponseMessage>> ProcessTransportOrderChange(
        [FromBody] TransportOrderChangeMessage request)
    {
        _logger.LogInformation("Transport order change request was received. Request: {request}", request);
        var updateResult = await _orderMessageHandler.HandleOrderChange(request);
        if (updateResult is null)
        {
            _logger.LogWarning("Transport order with id {barcodeId} was not found", request.Id);
            return NotFound("Order not found");
        }
        _logger.LogInformation("Transport order was changed. New order: {result}", updateResult);
        return Ok(new TransportOrderResponseMessage(request.CorrelationId, updateResult));
    }

    /// <summary>
    /// Повторный запуск (relaunch) заказа.
    /// </summary>
    /// <remarks>
    /// Принимает DTO для повторного запуска.  
    /// Возвращает 200 OK при успехе или 500 Problem в случае ошибки.
    /// </remarks>
    /// <param name="request">DTO с данными для релонча заказа.</param>
    /// <returns>Статус выполнения операции.</returns>
    [HttpPost("relaunch")]
    [SwaggerOperation(
        Summary = "Повторно запустить заказ",
        Description = "Пробует повторно запустить обработку заказа. Возвращает 200 OK при успехе.",
        Tags = new[] { "TransportOrder" }
    )]
    [SwaggerResponse(200, "Заказ успешно перезапущен")]
    [SwaggerResponse(500, "Не удалось перезапустить заказ")]
    public async Task<ActionResult> ProcessTransportOrderRelaunch(
        [FromBody] TransportOrderRelaunchMessage request)
    {
        _logger.LogInformation("Transport order relaunch request was received. Request: {request}", request);
        bool relaunchResult = await _orderMessageHandler.HandleOrderRelaunch(request);
        if (relaunchResult is false) return Problem("Ошибка при повторном запуске заказа");
        _logger.LogInformation("Transport order relaunch with correlation id {request.CorrelationId} was successful.", request.CorrelationId);
        return Ok();
    }
}