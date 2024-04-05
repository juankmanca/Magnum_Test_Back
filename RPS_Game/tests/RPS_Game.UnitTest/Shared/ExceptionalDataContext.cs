using Microsoft.EntityFrameworkCore;
using RPS_Game.API.Data;

namespace RPS_Game.UnitTest.Shared
{
    public class ExceptionalDataContext : ApplicationDbContext
    {
        public ExceptionalDataContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Simulated exception");
        }
    }
}
