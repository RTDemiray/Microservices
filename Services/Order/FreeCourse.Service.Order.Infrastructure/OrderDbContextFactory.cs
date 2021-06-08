using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FreeCourse.Service.Order.Infrastructure
{
    public class OrderDbContextFactory : IDesignTimeDbContextFactory<OrderDbContext>
    {
        public OrderDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost,1444;Database=OrderDb;Uid=sa;Pwd=Password12*");

            return new OrderDbContext(optionsBuilder.Options);
        }
    }
}