using Microsoft.EntityFrameworkCore;
using PustokTask.Models;

namespace PustokTask.Data
{
    public class PustokDbContex : DbContext
    {
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BookImage> BookImages { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BookTag> BookTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public PustokDbContex(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PustokDbContex).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
