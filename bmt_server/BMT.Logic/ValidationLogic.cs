using System.Text;
using System.Text.RegularExpressions;
using BMT.Contract.Dtos;
using BMT.Contract.Dtos.Results;
using BMT.Contract.Logic;

public class ValidationLogic : IValidationLogic
{
    public ResultDto ValidateNewBook(BookDto book)
    {
        var validationErrors = new StringBuilder();
        if (string.IsNullOrWhiteSpace(book.Title))
            validationErrors.AppendLine("Title cannot be empty");
        if (string.IsNullOrWhiteSpace(book.Author))
            validationErrors.AppendLine("Author cannot be empty");
        var icbnValidationResult = ValidateISBN(book.Isbn);
        if (!icbnValidationResult.IsSuccess)
            validationErrors.AppendLine(icbnValidationResult.Error!.Message);
        if (book.PublishDate == DateOnly.MinValue)
            validationErrors.AppendLine("PublishedDate was not provided");

        return validationErrors.Length == 0
            ? ResultDto.Success()
            : ErrorDto.BadRequest(validationErrors.ToString());
    }

    public ResultDto ValidateISBN(string isbn)
    {
        // https://stackoverflow.com/questions/41271613/use-regex-to-verify-an-isbn-number
        // potentially not entirely correct, but for our purposes it will do
        var match = Regex.IsMatch(isbn, @"^(?=(?:\D*\d){10}(?:(?:\D*\d){3})?$)[\d-]+$");
        if (!match)
            return ErrorDto.BadRequest("The provided ISBN is not valid");
        return ResultDto.Success();
    }

    // This should methid limit author names to reasonable values
    public ResultDto ValidateFullName(string fullName)
    {
        var match = Regex.IsMatch(fullName, @"[a-z,A-Z,á,é,í,ó,ú,â,ê,ô,ã,õ,ç,Á,É,Í,Ó,Ú,Â,Ê,Ô,Ã,Õ,Ç,ü,ñ,Ü,Ñ,' ']+");
        if (!match)
            return ErrorDto.BadRequest("The full name seems to be malformed. Remove any special characters and use up to three words.");
        return ResultDto.Success();
    }
}
