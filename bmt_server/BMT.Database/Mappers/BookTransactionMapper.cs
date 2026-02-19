using System.Linq.Expressions;
using BMT.Contract.Dtos;
using BMT.Database.Entity;

namespace BMT.Database.Mappers;

public static class BookTransactionMapper
{
    // mapping expression that can be used in linq expressions
    public static Expression<Func<BookTransactionEntity, BookTransactionDto>> ToDtoExpr = (BookTransactionEntity entity) =>
        new BookTransactionDto
        {
            IsReturn = entity.IsReturn,
            BookIdentification = entity.Book.Title + " (by " + entity.Book.Author.FullName +")",
            DateTime = entity.DateTime
        };

    // Do not use this inside linq expressions (due to usage of compile)
    public static BookTransactionDto ToDtoFull(this BookTransactionEntity entity)
    {
        return ToDtoExpr.Compile()(entity);
    }
}