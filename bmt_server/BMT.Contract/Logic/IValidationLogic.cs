using System.Text;
using BMT.Contract.Dtos;
using BMT.Contract.Dtos.Results;

namespace BMT.Contract.Logic;

public interface IValidationLogic
{
    ResultDto ValidateNewBook(BookDto book);

    ResultDto ValidateISBN(string isbn);

    ResultDto ValidateFullName(string fullName);
}
