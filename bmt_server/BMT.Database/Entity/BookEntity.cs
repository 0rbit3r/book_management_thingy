using System.ComponentModel.DataAnnotations;

namespace BMT.Database.Entity;

public class BookEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public int Icbm { get; set; }
    public int AvailableCopies { get; set; }
    public Guid AuthorId { get; set; }
    public AuthorEntity Author { get; set; } = null!;
    public ICollection<BookLendEntity> Lends {get; set;} = []; 
    public ICollection<BookReturnEntity> Returns {get; set;} = [];
}