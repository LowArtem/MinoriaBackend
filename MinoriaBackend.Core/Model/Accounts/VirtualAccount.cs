namespace MinoriaBackend.Core.Model.Accounts;

/// <summary>
/// Виртуальный счёт для накопления на заданную цель
/// </summary>
public class VirtualAccount : BaseAccount
{
    /// <summary>
    /// Целевая сумма накопления
    /// </summary>
    public decimal SavingGoal { get; set; } = 0;
    
    /// <summary>
    /// Дата достижения цели
    /// </summary>
    public DateTime AchievementDate { get; set; }

    /// <summary>
    /// Ссылка на цель накопления
    /// </summary>
    public string? GoalLink { get; set; } = null;
}