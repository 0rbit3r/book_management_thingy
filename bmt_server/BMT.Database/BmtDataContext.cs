using BMT.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace BMT.Database;

public class BmtDataContext : DbContext
{
    DbSet<BookEntity> Books { get; set; }
    DbSet<AuthorEntity> Authors { get; set; }
    DbSet<BookLendEntity> Lends { get; set; }
    DbSet<BookReturnEntity> Returns { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookEntity>()
            .HasIndex(b => b.Icbm);
        modelBuilder.Entity<BookEntity>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId);
        modelBuilder.Entity<BookEntity>()
            .HasMany(b => b.Lends)
            .WithOne(l => l.Book)
            .HasForeignKey(l => l.BookId);
        modelBuilder.Entity<BookEntity>()
            .HasMany(b => b.Returns)
            .WithOne(l => l.Book)
            .HasForeignKey(l => l.BookId);
    }
}
