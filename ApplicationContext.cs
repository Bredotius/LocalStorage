using System.Data.Entity;

namespace LocalStorage
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection"){}
        public DbSet<Doc> Docs { get; set; }
    }
}
