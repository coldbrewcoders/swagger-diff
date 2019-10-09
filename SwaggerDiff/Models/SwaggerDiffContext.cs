using Microsoft.EntityFrameworkCore;

namespace SwaggerDiff.Models
{
  public class SwaggerDiffContext : DbContext
  {
    public SwaggerDiffContext(DbContextOptions<SwaggerDiffContext> options) : base(options)
    {
    }

    public DbSet<SwaggerItem> SwaggerItems { get; set; }
  }
}
