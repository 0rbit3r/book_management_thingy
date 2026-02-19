using BMT.Contract.Logic;
using BMT.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BMT.Logic;
using BMT.Contract.Dtos;
using BMT.Database.Entity;
using BMT.Contract.Dtos.Results;

namespace BMT.Tests;

public class BookLogicTests : IDisposable
{
    private readonly ServiceProvider _services;
    private readonly IBookLogic _sut;

    public BookLogicTests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var services = new ServiceCollection();

        services.AddDbContext<BmtDataContext>(options =>
            options.UseSqlite(connection));

        services.RegisterLogicModule();

        _services = services.BuildServiceProvider();

        var context = _services.GetRequiredService<BmtDataContext>();
        context.Database.EnsureCreated();
        Seed(context);

        _sut = _services.GetRequiredService<IBookLogic>();
    }

    private void Seed(BmtDataContext ctx)
    {
        ctx.Books.AddRange(
            new BookEntity
            {
                Id = Guid.NewGuid(),
                Title = "Clean Code",
                Author = new AuthorEntity { Id = Guid.NewGuid(), FullName = "Robert C. Martin" },
                Isbn = "978-0-13208-906-8",
                AvailableCopies = 2,
                PublishDate = DateOnly.Parse("2007-01-01")
            },
            new BookEntity
            {
                Id = Guid.NewGuid(),
                Title = "Dirty Code",
                Author = new AuthorEntity { Id = Guid.NewGuid(), FullName = "Evil Martin" },
                Isbn = "0-13208-906-9",
                AvailableCopies = 3,
                PublishDate = DateOnly.Parse("1666-12-16")
            }
        );
        ctx.SaveChanges();
    }

    [Fact]
    public async Task GetBooks_ReturnsAllBooks()
    {
        var result = await _sut.GetBooks();
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Payload!.Count);
    }

    [Theory]
    [InlineData(" ", 2)]
    [InlineData("Martin", 2)]
    [InlineData("evil", 1)]
    [InlineData("Evil", 1)]
    [InlineData("<[{5}]", 0)]
    [InlineData("mar", 2)]
    public async Task GetBooksByAuthor_ReturnsFilteredBooks(string authorQuery, int expectedCount)
    {
        var result = await _sut.GetBooksByAuthor(authorQuery);
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedCount, result.Payload!.Count);
    }

    [Theory]
    [InlineData(" ", 2)]
    [InlineData("code", 2)]
    [InlineData("Code", 2)]
    [InlineData("dirty", 1)]
    [InlineData("Dirt", 1)]
    [InlineData("clean ", 1)]
    [InlineData("Clean", 1)]
    public async Task GetBooksByTitle_ReturnsFilteredBooks(string titleQuery, int expectedCount)
    {
        var result = await _sut.GetBooksByTitle(titleQuery);
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedCount, result.Payload!.Count);
    }

    [Theory]
    [InlineData("", "author", "1999-02-02", "178-0-13208-906-8", 5)]         // empty title
    [InlineData("title", "", "1999-02-02", "178-0-13208-906-8", 5)]          // empty author
    [InlineData("title", "author", "1999-02-02", "not-an-isbn", 5)]          // invalid isbn format
    [InlineData("title", "author", "1999-02-02", "178-0-13208-906-8", 0)]    // zero copies
    [InlineData("title", "author", "1999-02-02", "178-0-13208-906-8", -1)]   // negative copies
    [InlineData("title", "author", "0001-01-01", "178-0-13208-906-8", 5)]    // DateOnly.MinValue
    [InlineData("title", "author", "1999-02-02", "978-0-13208-906-8", 5)]    // ISBN already in db
    public async Task CreateBook_ValidatesWrongInput(string title, string author, string date, string isbn, int availableCopies)
    {
        var dateOnly = DateOnly.Parse(date);
        var result = await _sut.CreateBook(new() { Title = title, Author = author, Isbn = isbn, AvailableCopies = availableCopies, PublishDate = dateOnly });
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.BadRequest, result.Error!.Code);
    }

    [Theory]
    [InlineData("0-13208-906-9")]
    [InlineData("978-0-13208-906-8")]
    public async Task GetBoolByIsbn_ReturnsBook(string isbn)
    {
        var result = await _sut.GetBookByISBN(isbn);
        Assert.True(result.IsSuccess);
        Assert.Equal(isbn, result.Payload!.Isbn);
    }

    [Theory]
    [InlineData("178-0-13208-906-D", ErrorCode.BadRequest)]
    [InlineData("178-0-13208-906-1-486", ErrorCode.BadRequest)]
    [InlineData("0-13208-906-5", ErrorCode.NotFound)]
    [InlineData("978-0-13208-222-2", ErrorCode.NotFound)]
    public async Task GetBoolByIsbn_HandlesBadOrNonexistentInput(string isbn, ErrorCode expectedErrorCode)
    {
        var result = await _sut.GetBookByISBN(isbn);
        Assert.False(result.IsSuccess);
        Assert.Equal(expectedErrorCode, result.Error!.Code);
    }

    public void Dispose() => _services.Dispose();
}