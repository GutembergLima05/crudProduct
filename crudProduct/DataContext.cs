using Microsoft.EntityFrameworkCore;

namespace crudProduct
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseNpgsql("Server=tuffi.db.elephantsql.com;Port=5432;Database=htfoikvs;User Id=htfoikvs;Password=bdV1JHvvVMtx8p5S9C82mUiPH4W_Dbv5;");
    }
}
