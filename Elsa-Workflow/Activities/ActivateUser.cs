using Elsa;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using Elsa_Workflow.Models;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;
using Elsa_Workflow.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace Elsa_Workflow.Activities
{
    [ActivityDefinition(Category = "Users", Description = "Activate a User", Icon = "fas fa-user-check", Outcomes = new[] { OutcomeNames.Done, "Not Found" })]
    public class ActivateUser : Activity
    {
        private readonly IMongoCollection<User> _store;
        private readonly IDistributedCache _redisCache;

        public ActivateUser(IMongoCollection<User> store, IDistributedCache redisCache)
        {
            _store = store;
            _redisCache = redisCache;
        }
        
        [ActivityProperty(Hint = "Enter an expression that evaluates to the alias of the user to create.")]
        public WorkflowExpression<string> Alias
        {
            get => GetState<WorkflowExpression<string>>();
            set => SetState(value);
        }

        protected override async Task<ActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context, CancellationToken cancellationToken)
        {
            // Retrieves user from db
            var recordKey  = await context.EvaluateAsync(Alias, cancellationToken);
            var user = await _redisCache.GetRecordAsync<User>(recordKey);
            
            if (user == null)
            {
                return Outcome("Not Found");
            }

            // Updates user info on redirs 
            user.IsActive = true; 
            await _redisCache.SetRecordAsync(recordKey, user);
            return Done();
        }
    }
}
