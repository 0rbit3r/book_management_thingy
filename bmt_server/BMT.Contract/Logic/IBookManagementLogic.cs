using BMT.Contract.Dtos;
using BMT.Contract.Dtos.Results;

namespace BMT.Contract.Logic;

public interface IBookManagementLogic
{
    /// <summary>
    /// Gets all the books without paging, filtering or other fancy stuff
    /// </summary>
    Task<ResultDto<List<BookDto>>> GetBooks();
    /// <summary>
    /// Will create a new book and store it in database
    /// </summary>
    /// <param name="newBook">New book with required fields: 
    ///     Title, Author, ISBN and publish Date</param>
    /// <returns>Result with guid of the created book (if successful)</returns>
    Task<ResultDto<Guid>> CreateBook(BookDto newBook);

    /// <summary>
    /// Will return books with author whose name contains the given substring
    /// </summary>
    /// <param name="author">Substring to look for in authors name/surname</param>
    /// <returns></returns>
    Task<ResultDto<List<BookDto>>> GetBooksByAuthor(string authorSubstring);

    /// <summary>
    /// Will return books with author whose title contains the given substring
    /// </summary>
    /// <param name="author">Substring to look for in the book's title</param>
    /// <returns></returns>
    Task<ResultDto<List<BookDto>>> GetBooksByTitle(string titleSubstring);

    /// <summary>
    /// Returns book by specifying its ICBM
    /// </summary>
    /// <param name="ISBM">Valid ISBM string</param>
    /// <returns>Either a book matching the given ISBM or NotFound Error if no such one is found</returns>
    Task<ResultDto<BookDto>> GetBooksByICBM(string ISBM);

    /// <summary>
    /// Lends given book - practically this only decrements number of available copies and stores a row about the borrowing
    /// </summary>
    /// <param name="id">Id of the lent book</param>
    /// <param name="borrowerName">Optional name of the person borrowing - is stored in history</param>
    /// <returns>Result of the operation</returns>
    Task<ResultDto> LendBook(Guid id);
    /// <summary>
    /// Lends given book - practically this only inccrements number of available copies and stores a row about the return
    /// </summary>
    /// <param name="id">Id of the book being returned</param>
    Task<ResultDto> ReturnsBook(Guid id);

    /// <summary>
    /// This will return the entire history of book transactions
    /// </summary>
    /// <returns>Result with the transaction history sorted in descending order by date (or error if unsuccessful)</returns>
    Task<ResultDto<List<BookTransactionDto>>> GetBookTransactionHistory();
 
    /// <summary>
    /// THis will return the security state of the application
    /// </summary>
    Task<ResultDto<string>> GetSecurity();
}
