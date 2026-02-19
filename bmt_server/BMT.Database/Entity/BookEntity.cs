using System.ComponentModel.DataAnnotations;

namespace BMT.Database.Entity;

public class BookEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Isbn { get; set; } = null!;
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public Guid AuthorId { get; set; }
    public DateOnly PublishDate { get; set; }
    public AuthorEntity Author { get; set; } = null!;
    public ICollection<BookTransactionEntity> Lends { get; set; } = [];
}