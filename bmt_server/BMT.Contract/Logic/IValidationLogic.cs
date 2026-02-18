using System.Text;
using BMT.Contract.Dtos;
using BMT.Contract.Dtos.Results;

namespace BMT.Contract.Logic;

public interface IValidationLogic
{
    public ResultDto ValidateNewBook(BookDto book);

    public ResultDto ValidateISBN(string isbn);
}
