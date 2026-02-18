using System.ComponentModel.DataAnnotations;

namespace BMT.Database.Entity;

public class AuthorEntity
{
    [Key]
    public Guid Id { get; set; }
    // NOTE: In a real world, this would of course be two (or more) separete columns
    // But here I will simply use full name to make my life easier for fulltext search
    // Also, I'm assuming that no two authors have the same name which is, again, nonsense.
    // But since there are no "unique author identifiers" besides name in the assignment, I will treat full name as one.
    public string FullName { get; set; } = null!;
    public ICollection<BookEntity> Books = [];
}