using Elsa;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using Elsa_Workflow.Models;
using Elsa_Workflow.Services;
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
        private readonly IPasswordHasher _passwordHasher;

        public CreateUser(
            ILogger<CreateUser> logger,
            IMongoCollection<User> store,
            IIdGenerator idGenerator,
            IPasswordHasher passwordHasher)
        {
            _logger = logger;
            _store = store;
            _idGenerator = idGenerator;
            _passwordHasher = passwordHasher;
        }

        [ActivityProperty(Hint = "Enter an expression that evaluates to the name of the user to create.")]
        public WorkflowExpression<string> UserName
        {
            get => GetState<WorkflowExpression<string>>();
            set => SetState(value);
        }

        [ActivityProperty(Hint = "Enter an expression that evaluates to the email address of the user to create.")]
        public WorkflowExpression<string> Email
        {
            get => GetState<WorkflowExpression<string>>();
            set => SetState(value);
        }

        [ActivityProperty(Hint = "Enter an expression that evaluates to the password of the user to create.")]
        public WorkflowExpression<string> Password
        {
            get => GetState<WorkflowExpression<string>>();
            set => SetState(value);
        }

        protected override async Task<ActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context, CancellationToken cancellationToken)
        {
            var password = await context.EvaluateAsync(Password, cancellationToken);
            var hashedPassword = _passwordHasher.HashPassword(password);

            // Create and persist the new user
            var user = new User
            {
                Id = _idGenerator.Generate(),
                Name = await context.EvaluateAsync(UserName, cancellationToken),
                Email = await context.EvaluateAsync(Email, cancellationToken),
                Password = hashedPassword.Hashed,
                PasswordSalt = hashedPassword.Salt,
                IsActive = false
            };

            try
            {
                await _store.InsertOneAsync(user, cancellationToken: cancellationToken);
                // Set the info that will be available through Output
                Output.SetVariable("User", user);
                _logger.LogInformation($"New user created: {user.Id}, {user.Name}");
                return Done();
            } catch (Exception ex)
            {
                _logger.LogError(ex, $"Error persisting user: {user.Id}, {user.Name}");
                return Outcome("New user not persisted");
            }
        }
    }
}
