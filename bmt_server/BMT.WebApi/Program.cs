using BMT.Contract.Dtos;
using BMT.Contract.Dtos.Results;
using BMT.Contract.Logic;
using BMT.Database;
using BMT.Logic;
using Microsoft.EntityFrameworkCore;

//This file was scaffolded by AI

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BmtDataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.RegisterLogicModule();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<BmtDataContext>().Database.EnsureCreated();
}

// -------------------------------------------------------------------------
// Books
// -------------------------------------------------------------------------

app.MapGet("/books", async (IBookLogic bookLogic, ILogger<Program> logger) =>
{
    var result = await bookLogic.GetBooks();
    if (!result.IsSuccess)
    {
        logger.LogError("GetBooks failed: {Error}", result.Error);
        return ToErrorResponse(result.Error!);
    }
    return Results.Ok(result.Payload);
});

app.MapGet("/books/isbn/{isbn}", async (string isbn, IBookLogic bookLogic, ILogger<Program> logger) =>
{
    var result = await bookLogic.GetBookByISBN(isbn);
    if (!result.IsSuccess)
    {
        logger.LogError("GetBookByISBN failed for ISBN {Isbn}: {Error}", isbn, result.Error);
        return ToErrorResponse(result.Error!);
    }
    return Results.Ok(result.Payload);
});

app.MapGet("/books/author/{authorSubstring}", async (string authorSubstring, IBookLogic bookLogic, ILogger<Program> logger) =>
{
    var result = await bookLogic.GetBooksByAuthor(authorSubstring);
    if (!result.IsSuccess)
    {
        logger.LogError("GetBooksByAuthor failed for query '{Query}': {Error}", authorSubstring, result.Error);
        return ToErrorResponse(result.Error!);
    }
    return Results.Ok(result.Payload);
});

app.MapGet("/books/title/{titleSubstring}", async (string titleSubstring, IBookLogic bookLogic, ILogger<Program> logger) =>
{
    var result = await bookLogic.GetBooksByTitle(titleSubstring);
    if (!result.IsSuccess)
    {
        logger.LogError("GetBooksByTitle failed for query '{Query}': {Error}", titleSubstring, result.Error);
        return ToErrorResponse(result.Error!);
    }
    return Results.Ok(result.Payload);
});

app.MapPost("/books", async (BookDto newBook, IBookLogic bookLogic, ILogger<Program> logger) =>
{
    var result = await bookLogic.CreateBook(newBook);
    if (!result.IsSuccess)
    {
        logger.LogError("CreateBook failed: {Error}", result.Error);
        return ToErrorResponse(result.Error!);
    }
    return Results.Created($"/books/isbn/{newBook.Isbn}", result.Payload);
});

// -------------------------------------------------------------------------
// Lending
// -------------------------------------------------------------------------

app.MapPost("/books/isbn/{isbn}/lend", async (string isbn, IBookLogic bookLogic, ILogger<Program> logger) =>
{
    var result = await bookLogic.LendBook(isbn);
    if (!result.IsSuccess)
    {
        logger.LogError("LendBook failed for ISBN {Isbn}: {Error}", isbn, result.Error);
        return ToErrorResponse(result.Error!);
    }
    logger.LogInformation("Book {Isbn} lent. Remaining copies: {Copies}", isbn, result.Payload);
    return Results.Ok(new { AvailableCopies = result.Payload });
});

app.MapPost("/books/isbn/{isbn}/return", async (string isbn, IBookLogic bookLogic, ILogger<Program> logger) =>
{
    var result = await bookLogic.ReturnBook(isbn);
    if (!result.IsSuccess)
    {
        logger.LogError("ReturnBook failed for ISBN {Isbn}: {Error}", isbn, result.Error);
        return ToErrorResponse(result.Error!);
    }
    logger.LogInformation("Book {Isbn} returned. Available copies: {Copies}", isbn, result.Payload);
    return Results.Ok(new { AvailableCopies = result.Payload });
});

// -------------------------------------------------------------------------
// Transactions
// -------------------------------------------------------------------------

app.MapGet("/books/transactions", async (IBookLogic bookLogic, ILogger<Program> logger) =>
{
    var result = await bookLogic.GetBookTransactionHistory();
    if (!result.IsSuccess)
    {
        logger.LogError("GetBookTransactionHistory failed: {Error}", result.Error);
        return ToErrorResponse(result.Error!);
    }
    return Results.Ok(result.Payload);
});

// -------------------------------------------------------------------------
// Security (bonus points, apparently)
// -------------------------------------------------------------------------

app.MapGet("/security", async (IBookLogic bookLogic) =>
{
    var result = await bookLogic.GetSecurity();
    return Results.Ok(result.Payload);
});

// -------------------------------------------------------------------------

try
{
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "An error occurred while running the application");
}

// -------------------------------------------------------------------------
// Helpers
// -------------------------------------------------------------------------

static IResult ToErrorResponse(ErrorDto error) => error.Code switch
{
    ErrorCode.BadRequest => Results.BadRequest(error),
    ErrorCode.NotFound => Results.NotFound(error),
    ErrorCode.General => Results.StatusCode(500), // no message to client
    _ => Results.StatusCode(500)
};