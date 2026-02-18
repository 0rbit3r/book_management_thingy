using BMT.Contract.Enums;

namespace BMT.Contract.Dtos;

public class BookTransactionDto
{
    // specifies whether this transaction is a loan or a return
    public bool IsReturn { get; set; }
    public DateTime DateTime { get; set; }
    public required string BookIdentification { get; set; }
}