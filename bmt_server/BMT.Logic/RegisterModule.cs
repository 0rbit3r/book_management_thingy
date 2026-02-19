using BMT.Contract.Logic;
using Microsoft.Extensions.DependencyInjection;

namespace BMT.Logic;

public static class RegisterModule
{
    public static void RegisterLogicModule(this IServiceCollection services)
    {
        services.AddScoped<IBookLogic, BookLogic>();
        services.AddScoped<IValidationLogic, ValidationLogic>();
        services.AddScoped<IAuthorLogic, AuthorLogic>();
    }
}