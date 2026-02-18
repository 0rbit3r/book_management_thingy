using BMT.Contract.Dtos.Results;

namespace BMT.Contract.Logic;

public interface IAuthorLogic
{  
    Task<ResultDto<Guid>> CreateAuthor(string fullName);
}