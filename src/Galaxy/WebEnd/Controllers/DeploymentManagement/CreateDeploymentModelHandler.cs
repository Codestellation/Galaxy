using System.Collections.Generic;
using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.WebEnd.Models.DeploymentManangement;
using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Controllers.DeploymentManagement
{
    public class CreateDeploymentModelHandler : IRequestHandler<CreateDeploymentModelRequest, CreateDeploymentModelResponse>
    {
        private readonly Repository _repository;

        public CreateDeploymentModelHandler(Repository repository)
        {
            _repository = repository;
        }
        public CreateDeploymentModelResponse Handle(CreateDeploymentModelRequest message)
        {
            var allFeeds = _repository
                .Feeds
                .PerformQuery<NugetFeed>()
                .Select(x => new KeyValuePair<ObjectId, string>(x.Id, x.Name))
                .ToList();
            
            var model = new DeploymentEditModel { AllFeeds = allFeeds };
            return new CreateDeploymentModelResponse(model);
        }
    }
}