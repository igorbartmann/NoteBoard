using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoteBoard.Data.Persistence;
using NoteBoard.Data.Persistence.Constants;
using NoteBoard.Data.Persistence.Repositories;
using NoteBoard.Data.Persistence.UoW;
using NoteBoard.Domain.Repositories;

namespace NoteBoard.Data.DependencyInjection
{
    public static class DataDependencyInjection
    {
        public static IServiceCollection AddDataDependencies(this IServiceCollection services, IConfiguration configuraton)
        {
            AddDbContext(services, configuraton);
            AddUnitOfWork(services);
            AddRepositories(services);

            return services;
        }

        private static void AddDbContext(IServiceCollection services, IConfiguration configuraton)
        {
            var connectionString = configuraton.GetConnectionString(ConnectionStringNames.NoteBoardConnectionString);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Connection string {ConnectionStringNames.NoteBoardConnectionString} was not provided in the configuration.");
            }            

            services.AddDbContext<NoteBoardDbContext>(options => options.UseSqlServer(connectionString));
        }

        private static void AddUnitOfWork(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<INoteRepository, NoteRepository>();
        }
    }
}