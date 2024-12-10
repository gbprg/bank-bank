using Microsoft.EntityFrameworkCore;
using transfer_bank.Models;

namespace transfer_bank.Data
{
  // Configura o contexto do banco de dados e define os modelos mapeados
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    // Configurações adicionais de mapeamento para as tabelas do banco
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Transaction>(entity =>
      {
        entity.ToTable("Transactions");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.PaymentMethod)
        .HasConversion<string>();
      });

      modelBuilder.Entity<User>(entity =>
      {
        entity.ToTable("Users");
        entity.HasKey(e => e.Id);
      });
    }
  }
}