using System.Data.Entity;

namespace LKapp.WebJob
{
    public class SQLContext : System.Data.Entity.DbContext
    {
        public SQLContext(string connectionString)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<SQLContext>());
            this.Database.Connection.ConnectionString = connectionString;
        }
        public DbSet<EventEntity> Events { get; set; }
    }
}