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
using Elsa_Workflow.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace Elsa_Workflow.Activities
{
    [ActivityDefinition(Category = "Users", Description = "Create a User", Icon = "fas fa-user-plus", Outcomes = new[] { OutcomeNames.Done })]
    public class CreateUser : Activity
    {
        private readonly ILogger<CreateUser> _logger;
        private readonly IMongoCollection<User> _store;
        private readonly IIdGenerator _idGenerator;
        private readonly IDistributedCache _redisCache;
        private readonly IWorkflowInvoker _workflowInvoker;
        private Workflow HttpWorkflow { get; } 
        public CreateUser(
            ILogger<CreateUser> logger,
            IMongoCollection<User> store,
            IIdGenerator idGenerator, IDistributedCache redisCache, IWorkflowInvoker workflowInvoker, Workflow httpWorkflow)
        {
            _logger = logger;
            _store = store;
            _idGenerator = idGenerator;
            _redisCache = redisCache;
            _workflowInvoker = workflowInvoker;
            HttpWorkflow = httpWorkflow;
        }
        
        [ActivityProperty(Hint = "Enter an expression that evaluates to the alias of the user to create.")]
        public WorkflowExpression<string> Alias
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
                TestNumber = _idGenerator.Generate(),
                IsActive = false
            };

            try
            { 
               var record = await _redisCache.GetRecordAsync<User>(user.Alias);
               if (record is null)
               {
                   await _redisCache.SetRecordAsync(user.Alias, user);
               }
               
               // await _store.InsertOneAsync(user, cancellationToken: cancellationToken);
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
