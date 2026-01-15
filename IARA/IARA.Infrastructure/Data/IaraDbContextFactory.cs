using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace IARA.Infrastructure.Data;

public class IaraDbContextFactory : IDesignTimeDbContextFactory<IaraDbContext>
{
    public IaraDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IaraDbContext>();
        
 
        optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=IaraDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
        
        return new IaraDbContext(optionsBuilder.Options);
    }
}