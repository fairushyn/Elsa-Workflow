using Elsa_Workflow.Activities;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa_Workflow.Extensions
{
    public static class UserServiceCollectionExtensions
    {
        public static IServiceCollection AddUserActivities(this IServiceCollection services)
        {
            return services
                .AddActivity<CreateUser>()
                .AddActivity<ActivateUser>()
                .AddActivity<SendDataToSSI>() // TODO: needs implementation;
                .AddActivity<DeleteUser>();
        }
    }
}
