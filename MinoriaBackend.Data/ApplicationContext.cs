using MinoriaBackend.Core.Model.Auth;
using Microsoft.EntityFrameworkCore;
using MinoriaBackend.Core.Model;
using MinoriaBackend.Core.Model.Accounts;

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

    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BaseAccount> BaseAccounts { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<VirtualAccount> VirtualAccounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region User constraints

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        #endregion

        #region Account constraints

        modelBuilder.Entity<BaseAccount>()
            .HasDiscriminator<string>("AccountVariant")
            .HasValue<Account>("Account")
            .HasValue<VirtualAccount>("VirtualAccount");
        
        #endregion
        
        base.OnModelCreating(modelBuilder);
    }
}