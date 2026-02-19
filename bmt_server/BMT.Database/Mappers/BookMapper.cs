using System.Linq.Expressions;
using BMT.Contract.Dtos;
using BMT.Database.Entity;

namespace BMT.Database.Mappers;

public static class BookMapper
{
    // mapping expression that can be used in linq expressions
    public static Expression<Func<BookEntity, BookDto>> ToDtoExpr = (BookEntity entity) =>
        new BookDto
        {
            Title = entity.Title,
            Author = entity.Author.FullName,
            Isbn = entity.Isbn,
            AvailableCopies = entity.AvailableCopies,
            PublishDate = entity.PublishDate
        };

    // Do not use this inside linq expressions (due to usage of compile)
    public static BookDto ToDtoFull(this BookEntity entity)
    {
        return ToDtoExpr.Compile()(entity);
    }
}
