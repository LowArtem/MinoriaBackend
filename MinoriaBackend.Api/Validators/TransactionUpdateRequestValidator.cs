using FluentValidation;
using MinoriaBackend.Core.Dto.TransactionHistory.Update;
using MinoriaBackend.Core.Model.Enum;

namespace MinoriaBackend.Api.Validators;

/// <summary>
/// Валидация для <see cref="TransactionUpdateRequest"/>
/// </summary>
public class TransactionUpdateRequestValidator : AbstractValidator<TransactionUpdateRequest>
{
    /// <inheritdoc />
    public TransactionUpdateRequestValidator()
    {
        RuleFor(x => x.AccountTo).NotNull()
            .When(x => x.TransactionType == TransactionTypeEnum.RESERVATION || x.TransactionType == TransactionTypeEnum.TRANSFER)
            .WithMessage("Счёт на которой осуществлён перевод не может быть пустым");

        RuleFor(x => x.AccountFrom).NotNull();
        RuleFor(x => x.Amount).NotNull();
        RuleFor(x => x.Category).NotNull();
        RuleFor(x => x.Fee).NotNull().GreaterThanOrEqualTo(0);
        RuleFor(x => x.Date).NotNull();
        RuleFor(x => x.TransactionType).NotNull().IsInEnum();
    }
}