using Microsoft.EntityFrameworkCore;
using ModsenPractice.Entity;

namespace ModsenPractice.Data
{
    public class ModsenPracticeContext : DbContext
    {
    public ModsenPracticeContext(DbContextOptions<ModsenPracticeContext> options) : base(options) {}

    public DbSet<MyEvent> Events { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    public DbSet<EventImage> EventImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MyEvent>()
            .HasMany(e => e.EventMembers)
            .WithMany(m => m.MemberEvents)
            .UsingEntity<Dictionary<string, object>>(
                "EventsAndMembers",
                j => j
                    .HasOne<Member>()
                    .WithMany()
                    .HasForeignKey("MemberID")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<MyEvent>()
                    .WithMany()
                    .HasForeignKey("EventID")
                    .OnDelete(DeleteBehavior.Cascade)
            );


        modelBuilder.Entity<EventImage>()
            .HasOne(e => e.MyEvent)
            .WithMany(m => m.EventImages)
            .HasForeignKey(e => e.EventId);

        modelBuilder.Entity<User>()
        .HasOne(u => u.Role)
        .WithMany()
        .HasForeignKey(u => u.RoleId);

        }
    }
}
