using Dentigo.Application.Interfaces.Repositories;
using Dentigo.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Dentigo.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IToothRepository, ToothRepository>();
            services.AddScoped<IClinicalFindingRepository, ClinicalFindingRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
