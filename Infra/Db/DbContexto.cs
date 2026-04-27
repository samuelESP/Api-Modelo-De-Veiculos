using Dominios.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Infra.Db;

public class DbContexto: DbContext
{
    public DbContexto(DbContextOptions<DbContexto> options) : base(options){}
    

    public DbSet<Usuario> Usuarios { get; set; } = default!;
    public DbSet<Veiculo> Veiculos { get; set; } = default!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
}
