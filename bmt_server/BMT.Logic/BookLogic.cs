using System.Text.RegularExpressions;
using BMT.Contract.Dtos;
using BMT.Contract.Dtos.Results;
using BMT.Contract.Logic;
using BMT.Database;
using BMT.Database.Entity;
using BMT.Database.Mappers;
using Microsoft.EntityFrameworkCore;

namespace BMT.Logic;

public class BookLogic(
    IValidationLogic _validation,
    IAuthorLogic _authorLogic,
    BmtDataContext _db
) : IBookLogic
{
    public async Task<ResultDto<Guid>> CreateBook(BookDto newBook)
    {
        try
        {
            if (newBook.AvailableCopies <= 0)
                return ErrorDto.BadRequest("Available copies must be at least 1");
            var validationResult = _validation.ValidateNewBook(newBook);
            if (!validationResult.IsSuccess)
                return validationResult.Error!;

            // NOTE: Due to possibility of ISBN having leading zeros and our DB model not really caring about that,
            // it would be possible to have two of the same ISBNs stored in database in two different formats and this condition would not trigger.
            // In a real world scenario, it would make sense to fix the length of the ISBN to 13 characters with potentially leading zeros
            // and do some research into ISBN formats, validation and uniqueness.
            // Therefore, this logic might be a little bit off, but for our purposes it will do...

            // Another possible way might be to actually count the current number of available copies from the Total copies and history of lends/returns.
            // That would be foolproof but potentially slow for large data.
            var checkExisting = await _db.Books.FirstOrDefaultAsync(b => b.Isbn == newBook.Isbn);
            if (checkExisting is not null)
                return ErrorDto.BadRequest("The provided ISBN is already in our database");

            var normalizedAuthor = Utility.NormalizeFullName(newBook.Author);
            var existingAuthor = await _db.Authors.FirstOrDefaultAsync(b => b.FullName == normalizedAuthor);

            if (existingAuthor is null)
            {
                var createAuthorResult = await _authorLogic.CreateAuthor(normalizedAuthor);
                if (!createAuthorResult.IsSuccess)
                    return ErrorDto.General(createAuthorResult.Error!.Message);

                existingAuthor = await _db.Authors.FirstOrDefaultAsync(a => a.Id == createAuthorResult.Payload!);
                if (existingAuthor is null) return ErrorDto.General("Could not fetch newly created author.");
            }

            var createBookResult = await _db.AddAsync(new BookEntity()
            {
                Id = Guid.NewGuid(),
                Title = newBook.Title,
                AuthorId = existingAuthor.Id,
                Isbn = newBook.Isbn,
                AvailableCopies = newBook.AvailableCopies,
                TotalCopies = newBook.AvailableCopies
            });

            await _db.SaveChangesAsync();
            return createBookResult.Entity.Id;
        }
        catch (Exception e)
        {
            return ErrorDto.ExceptionThrown(e);
        }
    }

    public async Task<ResultDto<List<BookDto>>> GetBooks()
    {
        try
        {
            return await _db.Books.Select(BookMapper.ToDtoExpr).ToListAsync();
        }
        catch (Exception e)
        {
            return ErrorDto.ExceptionThrown(e);
        }
    }

    public async Task<ResultDto<List<BookDto>>> GetBooksByAuthor(string authorSubstring)
    {
        try
        {
            return await _db.Books
            .Where(b => b.Author.FullName.ToLower().Contains(authorSubstring.ToLower()))//NOTE: It seems that SQLite doesn't support InvariantCulture comparision. Pitty...
            .Select(BookMapper.ToDtoExpr)
            .ToListAsync();
        }
        catch (Exception e)
        {
            return ErrorDto.ExceptionThrown(e);
        }
    }

    public async Task<ResultDto<BookDto>> GetBookByISBN(string isbn)
    {
        try
        {
            var validationResult = _validation.ValidateISBN(isbn);
            if (!validationResult.IsSuccess)
                return validationResult.Error!;
            var book = await _db.Books
            .Where(b => b.Isbn == isbn)
            .Select(BookMapper.ToDtoExpr)
            .FirstOrDefaultAsync();

            if (book is null)
                return ErrorDto.NotFound("Book not found");
            return book;
        }
        catch (Exception e)
        {
            return ErrorDto.ExceptionThrown(e);
        }
    }

    public async Task<ResultDto<List<BookDto>>> GetBooksByTitle(string titleSubstring)
    {
        try
        {
            return await _db.Books
            .Where(b => b.Title.ToLower().Contains(titleSubstring.ToLower())) //NOTE: It seems that SQLite doesn't support InvariantCulture comparision. Pitty...
            .Select(BookMapper.ToDtoExpr)
            .ToListAsync();
        }
        catch (Exception e)
        {
            return ErrorDto.ExceptionThrown(e);
        }
    }

    public async Task<ResultDto<List<BookTransactionDto>>> GetBookTransactionHistory()
    {
        try
        {
            return await _db.Transactions
            .Select(BookTransactionMapper.ToDtoExpr)
            .OrderByDescending(t => t.DateTime)
            .ToListAsync();
        }
        catch (Exception e)
        {
            return ErrorDto.ExceptionThrown(e);
        }
    }

    public async Task<ResultDto<int>> LendBook(string isbn)
    {
        try
        {
            var validationResult = _validation.ValidateISBN(isbn);
            if (!validationResult.IsSuccess)
                return validationResult.Error!;
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Isbn == isbn);
            if (book is null)
                return ErrorDto.NotFound("Book not found");
            if (book.AvailableCopies <= 0)
                return ErrorDto.BadRequest("Book has no available copies");

            // NOTE: in real world (I'm starting to repeat myself:-D) we would be wise to handle lost updates
            // by for example extra column [Timestamp] public byte[] RowVersion { get; set; }
            // in combination with catch (DbUpdateConcurrencyException ex).
            // Here I chose not to do that as for our purposes (simple CLI) it is not critical. (And I need to sleep sometimes too)
            book.AvailableCopies--;

            _db.Transactions.Add(new()
            {
                BookId = book.Id,
                DateTime = DateTime.Now,
                IsReturn = false,
                Id = Guid.NewGuid()
            });

            await _db.SaveChangesAsync();

            return book.AvailableCopies;
        }
        catch (Exception e)
        {
            return ErrorDto.ExceptionThrown(e);
        }
    }

    public async Task<ResultDto<int>> ReturnBook(string isbn)
    {
        try
        {
            var validationResult = _validation.ValidateISBN(isbn);
            if (!validationResult.IsSuccess)
                return validationResult.Error!;
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Isbn == isbn);
            if (book is null)
                return ErrorDto.NotFound("Book not found");
            if (book.AvailableCopies >= book.TotalCopies)
                return ErrorDto.BadRequest("Cannot return book - all copies are already returned");

            // NOTE: Ad LendBook note above
            book.AvailableCopies++;

            _db.Transactions.Add(new()
            {
                BookId = book.Id,
                DateTime = DateTime.Now,
                IsReturn = true,
                Id = Guid.NewGuid()
            });

            await _db.SaveChangesAsync();

            return book.AvailableCopies;
        }
        catch (Exception e)
        {
            return ErrorDto.ExceptionThrown(e);
        }
    }

    public Task<ResultDto<string>> GetSecurity()
    {
        return Task.FromResult(ResultDto.Success("The application is secured. I promise"));
    }
}
