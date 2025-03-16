using Microsoft.EntityFrameworkCore;
using BookStore.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BookStore.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Books)
                .WithOne(b => b.Category)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Cascade); // Eğer bir kategori silinirse ilişkili kitaplar da silinecek

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Book)
                .WithMany()
                .HasForeignKey(o => o.BookId)
                .OnDelete(DeleteBehavior.Restrict); // Kitap silinirse siparişler silinmez
        }
    }
}
