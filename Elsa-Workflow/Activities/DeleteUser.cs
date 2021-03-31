using Elsa;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using Elsa_Workflow.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elsa_Workflow.Activities
{
    [ActivityDefinition(Category = "Users", Description = "Delete a User", Icon = "fas fa-user-minus", Outcomes = new[] { OutcomeNames.Done, "Not Found" })]
    public class DeleteUser : Activity
    {
        private readonly ILogger<DeleteUser> _logger;
        private readonly IMongoCollection<User> _store;

        public DeleteUser(
            ILogger<DeleteUser> logger,
            IMongoCollection<User> store)
        {
            _logger = logger;
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
            // Removes user from db
            var userId = await context.EvaluateAsync(UserId, cancellationToken);
            var result = await _store.DeleteOneAsync(x => x.Id == userId, cancellationToken);

            if (result.DeletedCount == 0)
            {
                _logger.LogError($"User {userId} could not be deleted");
                return Outcome("Not found");
            } else
            {
                _logger.LogInformation($"User {userId} succesfully deleted");
                return Done();
            }
        }
    }
}
