using System;
using Microsoft.Extensions.DependencyInjection;
using NoteBoard.Application.Interfaces.Mappers;
using NoteBoard.Application.Interfaces.Normalizers;
using NoteBoard.Application.Interfaces.Queries;
using NoteBoard.Application.Interfaces.Services;
using NoteBoard.Application.Interfaces.Validators;
using NoteBoard.Application.LoggedUserManager;
using NoteBoard.Application.Mappers;
using NoteBoard.Application.Normalizers;
using NoteBoard.Application.Queries;
using NoteBoard.Application.Services;
using NoteBoard.Application.Validators;

namespace NoteBoard.Application.DependencyInjection
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
        {
            AddNormalizers(services);
            AddValidators(services);
            AddMappers(services);
            AddQueries(services);
            AddServices(services);
            AddLoggedUserManager(services);
            
            return services;
        }

        private static void AddNormalizers(IServiceCollection services)
        {
            services.AddScoped<INoteNormalizer, NoteNormalizer>();
            services.AddScoped<IUserNormalizer, UserNormalizer>();
        }

        private static void AddValidators(IServiceCollection services)
        {
            services.AddScoped<INoteValidator, NoteValidator>();
            services.AddScoped<IUserValidator, UserValidator>();
        }

        private static void AddMappers(IServiceCollection services)
        {
            services.AddScoped<INoteMapper, NoteMapper>();
            services.AddScoped<IUserMapper, UserMapper>();
        }

        private static void AddQueries(IServiceCollection services)
        {
            services.AddScoped<INoteQuery, NoteQuery>();
            services.AddScoped<IUserQuery, UserQuery>();
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddScoped<INoteService, NoteService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
        }

        private static void AddLoggedUserManager(IServiceCollection services)
        {
            services.AddScoped<ILoggedUserManager, LoggedUserManager.LoggedUserManager>();
        }
    }
}