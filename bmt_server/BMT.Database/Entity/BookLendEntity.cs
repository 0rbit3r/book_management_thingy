using System.ComponentModel.DataAnnotations;

namespace BMT.Database.Entity;

public class BookTransactionEntity
{
    [Key]
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public Guid BookId { get; set; }
    public BookEntity Book { get; set; } = null!;
    public bool IsReturn { get; set; }
}