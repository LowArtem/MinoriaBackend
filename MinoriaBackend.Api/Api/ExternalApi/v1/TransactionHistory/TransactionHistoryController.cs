using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinoriaBackend.Api.Attributes;
using MinoriaBackend.Api.Extensions.Api;
using MinoriaBackend.Core.Dto.TransactionHistory;
using MinoriaBackend.Data.Services.TransactionHistory;
using Swashbuckle.AspNetCore.Annotations;

namespace MinoriaBackend.Api.Api.ExternalApi.v1.TransactionHistory;

/// <summary>
/// История транзакций
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[SetRoute]
[Authorize]
public class TransactionHistoryController : ControllerBase
{
    private readonly TransactionHistoryService _transactionHistoryService;
    private readonly IValidator<TransactionHistoryRequest> _validator;
    private readonly ILogger<TransactionHistoryController> _logger;

    public TransactionHistoryController(TransactionHistoryService transactionHistoryService, ILogger<TransactionHistoryController> logger, IValidator<TransactionHistoryRequest> validator)
    {
        _transactionHistoryService = transactionHistoryService;
        _logger = logger;
        _validator = validator;
    }
    
    /// <summary>
    /// История транзакций
    /// </summary>
    /// <param name="request">запрос</param>
    /// <param name="token">токен отмены</param>
    /// <returns>список транзакций пользователя</returns>
    [HttpGet("transactions")]
    [SwaggerResponse(200, "История транзакций", typeof(TransactionHistoryResponse))]
    [SwaggerResponse(400, "Неверные параметры запроса", typeof(List<ValidationFailure>))]
    [SwaggerResponse(401, "Ошибка авторизации")]
    [SwaggerResponse(403, "Доступ запрещен")]
    [SwaggerResponse(500, "Внутренняя ошибка")]
    public async Task<IActionResult> GetTransactionHistory([FromQuery] TransactionHistoryRequest request, CancellationToken token)
    {
        var validationResult = await _validator.ValidateAsync(request, token);
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
}