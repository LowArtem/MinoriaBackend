using MinoriaBackend.Core.Model.Auth;
using Microsoft.EntityFrameworkCore;

namespace MinoriaBackend.Data;

/// <summary>
/// Контекст базы данных приложения
/// </summary>
public class ApplicationContext : DbContext
{
    /// <inheritdoc />
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    /// <inheritdoc />
    public ApplicationContext()
    {
    }

    #region Auth

    public DbSet<User> Users { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}