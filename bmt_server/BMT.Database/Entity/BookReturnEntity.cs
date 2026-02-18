using System.ComponentModel.DataAnnotations;

namespace BMT.Database.Entity;

public class BookReturnEntity
{
    [Key]
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public Guid BookId { get; set; }
    public BookEntity Book { get; set; } = null!;

    // Currently there is no way of associating which lend the return corresponds to
    // as it was not in specification.
    // An obvious next step would be to pair the lends to some id (most likely a customerId)
    // and pair every return with the lend it corresponds tos
}