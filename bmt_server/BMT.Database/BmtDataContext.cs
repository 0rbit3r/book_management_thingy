using BMT.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace BMT.Database;

public class BmtDataContext : DbContext
{
    public DbSet<BookEntity> Books { get; set; }
    public DbSet<AuthorEntity> Authors { get; set; }
    public DbSet<BookTransactionEntity> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookEntity>()
            .HasIndex(b => b.Isbn);
        modelBuilder.Entity<BookEntity>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId);
        modelBuilder.Entity<BookEntity>()
            .HasMany(b => b.Lends)
            .WithOne(l => l.Book)
            .HasForeignKey(l => l.BookId);
    }
}
