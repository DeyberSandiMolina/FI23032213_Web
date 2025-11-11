using Microsoft.EntityFrameworkCore;
using MyConsole.Entities;

namespace MyConsole.Context
{
    public class BooksContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TitleTag> TitleTags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "data");
            Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, "books.db");
            optionsBuilder.UseSqlite($"Data Source={path}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TitleTag>().ToTable("TitlesTags");

            modelBuilder.Entity<TitleTag>()
                .HasOne(tt => tt.Title)
                .WithMany(t => t.TitleTags)
                .HasForeignKey(tt => tt.TitleId);

            modelBuilder.Entity<TitleTag>()
                .HasOne(tt => tt.Tag)
                .WithMany(t => t.TitleTags)
                .HasForeignKey(tt => tt.TagId);
        }
    }
}
