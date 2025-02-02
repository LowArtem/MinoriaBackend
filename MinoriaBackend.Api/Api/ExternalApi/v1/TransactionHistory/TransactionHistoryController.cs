using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinoriaBackend.Api.Attributes;
using MinoriaBackend.Api.Extensions.Api;
using MinoriaBackend.Core.Dto.TransactionHistory.Get;
using MinoriaBackend.Core.Dto.TransactionHistory.Update;
using MinoriaBackend.Data.Services.TransactionHistory;
using Swashbuckle.AspNetCore.Annotations;

namespace MinoriaBackend.Api.Api.ExternalApi.v1.TransactionHistory;

/// <summary>
/// История транзакций
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[SetRoute("transactions")]
[Authorize]
public class TransactionHistoryController : ControllerBase
{
    private readonly TransactionHistoryService _transactionHistoryService;
    private readonly IValidator<TransactionHistoryRequest> _transactionHistoryValidator;
    private readonly IValidator<TransactionUpdateRequest> _transactionUpdateValidator;
    private readonly ILogger<TransactionHistoryController> _logger;

    public TransactionHistoryController(TransactionHistoryService transactionHistoryService,
        ILogger<TransactionHistoryController> logger, IValidator<TransactionHistoryRequest> transactionHistoryValidator,
        IValidator<TransactionUpdateRequest> transactionUpdateValidator)
    {
        _transactionHistoryService = transactionHistoryService;
        _logger = logger;
        _transactionHistoryValidator = transactionHistoryValidator;
        _transactionUpdateValidator = transactionUpdateValidator;
    }
    
    /// <summary>
    /// История транзакций
    /// </summary>
    /// <param name="request">запрос</param>
    /// <param name="token">токен отмены</param>
    /// <returns>список транзакций пользователя</returns>
    [HttpGet("")]
    [SwaggerResponse(200, "История транзакций", typeof(TransactionHistoryResponse))]
    [SwaggerResponse(400, "Неверные параметры запроса", typeof(List<ValidationFailure>))]
    [SwaggerResponse(401, "Ошибка авторизации")]
    [SwaggerResponse(403, "Доступ запрещен")]
    [SwaggerResponse(500, "Внутренняя ошибка")]
    public async Task<IActionResult> GetTransactionHistory([FromQuery] TransactionHistoryRequest request, CancellationToken token)
    {
        var validationResult = await _transactionHistoryValidator.ValidateAsync(request, token);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var userId = User.GetUserId();
        
        try
        {
            var result = await _transactionHistoryService.GetTransactionHistory(userId, request, token);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception in GetTransactionHistory: {Message}", e.Message);
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Обновить элемент истории транзакций
    /// </summary>
    /// <param name="transactionId">Id транзакции</param>
    /// <param name="request">запрос</param>
    /// <param name="token">токен отмены</param>
    /// <returns>код статуса</returns>
    [HttpPut("{transactionId:guid}")]
    [SwaggerResponse(200, "Элемент успешно обновлён")]
    [SwaggerResponse(400, "Неверные параметры запроса", typeof(List<ValidationFailure>))]
    [SwaggerResponse(401, "Ошибка авторизации")]
    [SwaggerResponse(403, "Доступ запрещен")]
    [SwaggerResponse(500, "Внутренняя ошибка")]
    public async Task<IActionResult> UpdateTransaction([FromRoute] Guid transactionId, [FromBody] TransactionUpdateRequest request,
        CancellationToken token)
    {
        var validationResult = await _transactionUpdateValidator.ValidateAsync(request, token);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            await _transactionHistoryService.UpdateTransactionHistory(transactionId, request, token);
            _logger.LogInformation("Updated transaction {TransactionId}", transactionId);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception in UpdateTransaction: {Message}", e.Message);
            return StatusCode(500);
        }
    }
    
    /// <summary>
    /// Удалить элемент истории транзакций
    /// </summary>
    /// <param name="transactionId">Id транзакции</param>
    /// <returns>код статуса</returns>
    [HttpDelete("{transactionId:guid}")]
    [SwaggerResponse(200, "Элемент успешно удалён")]
    [SwaggerResponse(401, "Ошибка авторизации")]
    [SwaggerResponse(403, "Доступ запрещен")]
    [SwaggerResponse(500, "Внутренняя ошибка")]
    public IActionResult DeleteTransaction([FromRoute] Guid transactionId)
    {
        try
        {
            _transactionHistoryService.DeleteTransaction(transactionId);
            _logger.LogInformation("Deleted (or not found) transaction {TransactionId}", transactionId);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception in DeleteTransaction: {Message}", e.Message);
            return StatusCode(500);
        }
    }
}