namespace MinoriaBackend.Data.Repositories;

/// <summary>
/// Класс для работы с транзакциями
/// </summary>
public class ApplicationUnitOfWork : UnitOfWork<ApplicationContext>
{
    public ApplicationUnitOfWork(ApplicationContext dbContext) : base(dbContext)
    {
    }
}