using Microsoft.EntityFrameworkCore;

namespace Raf.Data;

// Tablolar (DbSet'ler) varlık modelleriyle birlikte eklenecek.
public class RafDbContext : DbContext
{
    public RafDbContext(DbContextOptions<RafDbContext> options) : base(options)
    {
    }
}
