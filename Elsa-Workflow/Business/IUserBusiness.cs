using Elsa_Workflow.Models;
using System.Threading.Tasks;

namespace Elsa_Workflow.Business
{
    public interface IUserBusiness
    {
        Task UserRegistration(RegistrationModel request);
    }
}
