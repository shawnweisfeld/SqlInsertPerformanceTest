using System.Data.Entity;

namespace SqlInsertPerformanceTest
{
    public class TestDataContext : DbContext
    {
        public TestDataContext()
        {
            Database.SetInitializer(new TestDataContextInitializer());
        }

        public DbSet<Customer> Customers { get; set; }
    }
}