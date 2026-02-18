using BMT.Contract.Enums;

namespace BMT.Contract.Dtos;

public class BookTransactionDto
{
    public BookTransactionType Type { get; set; }
    public DateTime DateTime { get; set; }
    public required string BookIdentification {get; set;}
}