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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связи один ко многим между MyEvent и Member
            modelBuilder.Entity<MyEvent>()
                .HasMany(e => e.EventMembers)
                .WithMany(m => m.MemberEvents)
                .UsingEntity(j => j.ToTable("EventsAndMembers")); // Вспомогательная таблица для связи многие ко многим
        }
    }
}
