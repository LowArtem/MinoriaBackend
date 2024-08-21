using MinoriaBackend.Core.Model.Enum;

namespace MinoriaBackend.Core.Model.Accounts;

/// <summary>
/// Обычный счёт пользователя (соответствует его реальному банковскому счёту)
/// </summary>
public class Account : BaseAccount
{
    /// <summary>
    /// Зарезервированная сумма (для виртуального счёта)
    /// </summary>
    public decimal AmountReserved { get; set; } = 0;

    /// <summary>
    /// Тип счёта (обычный, кредитный, вклад)
    /// </summary>
    public AccountTypeEnum AccountType { get; set; } = AccountTypeEnum.DEFAULT;

    /// <summary>
    /// Является ли счёт основным (счёт по умолчанию)
    /// </summary>
    public bool IsPrimary { get; set; } = false;
}