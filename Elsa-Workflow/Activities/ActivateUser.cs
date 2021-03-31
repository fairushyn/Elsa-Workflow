using Elsa;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using Elsa_Workflow.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa_Workflow.Activities
{
    [ActivityDefinition(Category = "Users", Description = "Activate a User", Icon = "fas fa-user-check", Outcomes = new[] { OutcomeNames.Done, "Not Found" })]
    public class ActivateUser : Activity
    {
        private readonly IMongoCollection<User> _store;

        public ActivateUser(IMongoCollection<User> store)
        {
            _store = store;
        }

        [ActivityProperty(Hint = "Enter an expression that evaluates to the ID of the user to activate.")]
        public WorkflowExpression<string> UserId
        {
            get => GetState<WorkflowExpression<string>>();
            set => SetState(value);
        }

        protected override async Task<ActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context, CancellationToken cancellationToken)
        {
            // Retrieves user from db
            var userId = await context.EvaluateAsync(UserId, cancellationToken);
            var user = await _store.AsQueryable().FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

            if (user == null)
            {
                return Outcome("Not Found");
            }

            // Updates user info on db
            user.IsActive = true;
            await _store.ReplaceOneAsync(x => x.Id == userId, user, cancellationToken: cancellationToken);

            return Done();
        }
    }
}
