// See https://aka.ms/new-console-template for more information
using BMT.Contract.Dtos;
using BMT.Contract.Logic;
using BMT.Database;
using BMT.Logic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string mainScreen =
@"
    Book Managament Thingy

Usage:
    bmt <command>

Commands:
    list                                                List all books

    list -a <search-query>                              List book whose author's name contains the query

    list -t <search-query>                              List book whose title contains the query

    get <isbn>                                          Get a book specified by its ISBN

    add <title> <author> <isbn> <published> <copies>    Add a new book to the system
                                                        - <isbn> takes format of 'ddd-d-ddddd-ddd-d' or 'd-ddddd-ddd-d'
                                                        - <published> takes format of 'yyyy-MM-dd'

    lend <isbn>                                         Lend a book

    return <isbn>                                       Return a book

    history                                             Print history of transactions
";

HostApplicationBuilder builder = new HostApplicationBuilder();
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false)
    .Build();

// Add modules
builder.Services.RegisterLogicModule();

builder.Services.AddDbContext<BmtDataContext>(options =>
    options.UseSqlite(config.GetConnectionString("DefaultConnection"))); // With IConfiguration we could use whatever path, but I decided not to to keep it simple

var app = builder.Build();

app.Services.GetRequiredService<BmtDataContext>().Database.EnsureCreated();

if (args.Length == 0) return PrintHelpAndExitWithOne();

switch (args[0])
{
    case "add":
        if (args.Length != 6) return PrintHelpAndExitWithOne();
        if (!DateOnly.TryParseExact(args[4], "yyyy-MM-dd", out var publishDate))
        { Console.WriteLine("Wrong date format"); return 1; }
        if (!int.TryParse(args[5], out int copies))
        { Console.WriteLine("Copies argument must be an integer"); return 1; }
        var addBookResult = await app.Services.GetRequiredService<IBookLogic>().CreateBook(
            new() { Title = args[1], Author = args[2], Isbn = args[3], PublishDate = publishDate, AvailableCopies = copies });
        Console.WriteLine(addBookResult.IsSuccess
            ? "Book added."
            : addBookResult.Error!.Message);
        break;
    case "list":
        if (args.Length == 1)
        {
            var listAllResult = await app.Services.GetRequiredService<IBookLogic>().GetBooks();
            if (!listAllResult.IsSuccess) { Console.WriteLine(listAllResult.Error!.Message); return 1; }
            foreach (var book in listAllResult.Payload!)
                PrintBook(book);
        }
        else if (args.Length == 3 && args[1] == "-a")
        {
            var listByAuthorResult = await app.Services.GetRequiredService<IBookLogic>().GetBooksByAuthor(args[2]);
            if (!listByAuthorResult.IsSuccess) { Console.WriteLine(listByAuthorResult.Error!.Message); return 1; }
            foreach (var book in listByAuthorResult.Payload!)
                PrintBook(book);
        }
        else if (args.Length == 3 && args[1] == "-t")
        {
            var listByAuthorResult = await app.Services.GetRequiredService<IBookLogic>().GetBooksByTitle(args[2]);
            if (!listByAuthorResult.IsSuccess) { Console.WriteLine(listByAuthorResult.Error!.Message); return 1; }
            foreach (var book in listByAuthorResult.Payload!)
                PrintBook(book);
        }
        else
            return PrintHelpAndExitWithOne();
        break;
    case "get":
        if (args.Length != 2)
            return PrintHelpAndExitWithOne();
        var getByIsbnResult = await app.Services.GetRequiredService<IBookLogic>().GetBookByISBN(args[1]);
        if (!getByIsbnResult.IsSuccess) { Console.WriteLine(getByIsbnResult.Error!.Message); return 1; }
        PrintBook(getByIsbnResult.Payload!);
        break;
    case "lend":
        if (args.Length != 2)
            return PrintHelpAndExitWithOne();
        var lendResult = await app.Services.GetRequiredService<IBookLogic>().LendBook(args[1]);
        if (!lendResult.IsSuccess) { Console.WriteLine(lendResult.Error!.Message); return 1; }
        Console.WriteLine($"Book lent successfully. Remaining copies: {lendResult.Payload!}");
        break;
    case "return":
        if (args.Length != 2)
            return PrintHelpAndExitWithOne();
        var returnResult = await app.Services.GetRequiredService<IBookLogic>().ReturnBook(args[1]);
        if (!returnResult.IsSuccess) { Console.WriteLine(returnResult.Error!.Message); return 1; }
        Console.WriteLine($"Book returned successfully. Remaining copies: {returnResult.Payload!}");
        break;
    case "history":
        if (args.Length != 1)
            return PrintHelpAndExitWithOne();
        var historyResult = await app.Services.GetRequiredService<IBookLogic>().GetBookTransactionHistory();
        if (!historyResult.IsSuccess) { Console.WriteLine(historyResult.Error!.Message); return 1; }
        foreach (var transaction in historyResult.Payload!)
            PrintTransaction(transaction);
        break;
    default:
        return PrintHelpAndExitWithOne();
}

return 0;

void PrintBook(BookDto book)
{
    Console.WriteLine($"{book.Title,-50}{$" by {book.Author}",-30}ISBN: {book.Isbn,-18} Copies: {book.AvailableCopies}");
}

void PrintTransaction(BookTransactionDto transaction)
{
    Console.WriteLine($"{transaction.DateTime,-20:yyyy-MM-dd HH:mm} {transaction.BookIdentification,-40}{(transaction.IsReturn ? "returned" : "lent")}");

}

int PrintHelpAndExitWithOne()
{
    Console.WriteLine(mainScreen);
    return 1;
}