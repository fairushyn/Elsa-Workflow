using Elsa_Workflow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elsa_Workflow.Business
{
    public interface IUserBusiness
    {
        Task UserRegistration(RegistrationModel request);
    }
}
