using System.ComponentModel.DataAnnotations;

namespace BMT.Database.Entity;

public class AuthorEntity
{
    [Key]
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public ICollection<BookEntity> Books = [];
}