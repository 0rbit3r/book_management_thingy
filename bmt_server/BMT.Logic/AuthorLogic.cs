using BMT.Contract.Dtos.Results;
using BMT.Contract.Logic;
using BMT.Database;
using BMT.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace BMT.Logic;

public class AuthorLogic(
    IValidationLogic _validation,
    BmtDataContext _db
) : IAuthorLogic
{
    public async Task<ResultDto<Guid>> CreateAuthor(string fullName)
    {
        try
        {
            var normalizedName = Utility.NormalizeFullName(fullName);
            var validationResult = _validation.ValidateFullName(normalizedName);
            if (!validationResult.IsSuccess)
                return validationResult.Error!;

            var existingAuthor = await _db.Authors.FirstOrDefaultAsync(a => a.FullName == normalizedName);
            if (existingAuthor is not null)
                return ErrorDto.BadRequest("Author with this name already exists");

            var newGuid = Guid.NewGuid();
            _db.Authors.Add(new AuthorEntity()
            {
                Id = newGuid,
                FullName = normalizedName
            });
            await _db.SaveChangesAsync();
            return ResultDto.Success(newGuid);
        }
        catch (Exception e)
        {
            return ErrorDto.ExceptionThrown(e);
        }
    }
}