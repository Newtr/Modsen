

using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;

public class CarContext : DbContext
{
    public CarContext(DbContextOptions<CarContext> options) : base(options)
    {

    }

    public DbSet<Car> Cars {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>().HasIndex(car => car.CarNumber).IsUnique();
    }
}