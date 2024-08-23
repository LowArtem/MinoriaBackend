using FluentValidation;
using MinoriaBackend.Core.Dto.TransactionHistory;
using MinoriaBackend.Core.Dto.TransactionHistory.Get;

namespace MinoriaBackend.Api.Validators;

/// <summary>
/// Валидация для <see cref="TransactionHistoryRequest"/>
/// </summary>
public class TransactionHistoryRequestValidator : AbstractValidator<TransactionHistoryRequest>
{
    /// <inheritdoc />
    public TransactionHistoryRequestValidator()
    {
        RuleFor(x => x.Count).GreaterThan(0);
        RuleFor(x => x.From).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DateFrom).LessThanOrEqualTo(x => x.DateTo).When(x => x.DateFrom != null && x.DateTo != null);
        RuleFor(x => x.TransactionType).IsInEnum().When(x => x.TransactionType != null);
    }
}