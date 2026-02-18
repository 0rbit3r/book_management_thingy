using System.ComponentModel.DataAnnotations;

namespace BMT.Contract.Dtos;

public class BookDto
{
    public Guid Id { get; set; }
    [Required]
    public required string Name { get; set; } 
    [Required]
    public required string Author { get; set; }
    [Required]
    public DateTime PublishDate { get; set; }
    [Required]
    public required string Isbn { get; set; }
    public int AvailableCopies { get; set; }
}