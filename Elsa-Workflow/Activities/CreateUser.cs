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
using System.Threading;
using System.Threading.Tasks;

namespace Elsa_Workflow.Activities
{
    [ActivityDefinition(Category = "Users", Description = "Create a User", Icon = "fas fa-user-plus", Outcomes = new[] { OutcomeNames.Done })]
    public class CreateUser : Activity
    {
        private readonly ILogger<CreateUser> _logger;
        private readonly IMongoCollection<User> _store;
        private readonly IIdGenerator _idGenerator;

        public CreateUser(
            ILogger<CreateUser> logger,
            IMongoCollection<User> store,
            IIdGenerator idGenerator)
        {
            _logger = logger;
            _store = store;
            _idGenerator = idGenerator;
        }

        [ActivityProperty(Hint = "Enter an expression that evaluates to the alias of the user to create.")]
        public WorkflowExpression<string> Alias
        {
            get => GetState<WorkflowExpression<string>>();
            set => SetState(value);
        }

        [ActivityProperty(Hint = "Enter an expression that evaluates to the COVID test number of the user to create.")]
        public WorkflowExpression<string> TestNumber
        {
            get => GetState<WorkflowExpression<string>>();
            set => SetState(value);
        }
        
        
        protected override async Task<ActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context, CancellationToken cancellationToken)
        {
            // Create and persist the new user
            var user = new User
            {
                Id = _idGenerator.Generate(),
                Alias = await context.EvaluateAsync(Alias, cancellationToken),
                TestNumber = await context.EvaluateAsync(TestNumber, cancellationToken),
                IsActive = false
            };

            try
            {
                await _store.InsertOneAsync(user, cancellationToken: cancellationToken);
                // Set the info that will be available through Output
                Output.SetVariable("User", user);
                _logger.LogInformation($"New user created: {user.Id}, {user.Alias}");
                return Done();
            } catch (Exception ex)
            {
                _logger.LogError(ex, $"Error persisting user: {user.Id}, {user.Alias}");
                return Outcome("New user not persisted");
            }
        }
    }
}
