using Microsoft.EntityFrameworkCore;
using ModsenPractice.Entity;

namespace ModsenPractice.Data
{
    public class ModsenPracticeContext : DbContext
    {
 public ModsenPracticeContext(DbContextOptions<ModsenPracticeContext> options) : base(options) {}

    // Таблицы для сущностей
    public DbSet<MyEvent> Events { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<EventImage> EventImages { get; set; } // Обратите внимание, что класс должен называться EventImage, а не EventImages

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка связи многие ко многим между MyEvent и Member
        modelBuilder.Entity<MyEvent>()
            .HasMany(e => e.EventMembers)
            .WithMany(m => m.MemberEvents)
            .UsingEntity(j => j.ToTable("EventsAndMembers")); // Вспомогательная таблица для связи многие ко многим

        // Настройка связи один ко многим между MyEvent и EventImage
        modelBuilder.Entity<EventImage>()
            .HasOne(e => e.MyEvent)
            .WithMany(m => m.EventImages)
            .HasForeignKey(e => e.EventId);
        }
    }
}
