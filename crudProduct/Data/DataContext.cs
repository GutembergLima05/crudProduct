using crudProduct.Models;
using Microsoft.EntityFrameworkCore;

namespace crudProduct.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseNpgsql("Server=babar.db.elephantsql.com;Port=5432;Database=ydrmlyru;User Id=ydrmlyru;Password=HCjf_vwV1YOzO_2GGQQ8qFjkHhwvCeF-;");
    }
}
