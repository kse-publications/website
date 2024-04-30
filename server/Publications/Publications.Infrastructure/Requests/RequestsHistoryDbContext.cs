using Microsoft.EntityFrameworkCore;
using Publications.Domain.Requests;

namespace Publications.Infrastructure.Requests;

public class RequestsHistoryDbContext: DbContext
{
    public DbSet<Request> Requests { get; set; } = null!;

    public RequestsHistoryDbContext(
        DbContextOptions<RequestsHistoryDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Request>()
            .HasKey(request => request.Id);
    }
}