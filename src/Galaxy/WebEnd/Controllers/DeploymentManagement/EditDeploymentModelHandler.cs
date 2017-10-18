using System;
using System.Collections.Generic;
using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.WebEnd.Models.DeploymentManangement;
using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Controllers.DeploymentManagement
{
    public class EditDeploymentModelHandler : IRequestHandler<EditDeploymentModelRequest, EditDeploymentModelResponse>
    {
        private readonly Repository _repository;

        public EditDeploymentModelHandler(Repository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public EditDeploymentModelResponse Handle(EditDeploymentModelRequest message)
        {
            var deployment = _repository
                .Deployments
                .Load<Domain.Deployment>(message.Id);

            var feeds = _repository
                .Feeds
                .PerformQuery<NugetFeed>()
                .Select(x => new KeyValuePair<ObjectId, string>(x.Id, x.Name))
                .ToList();

            var model = new DeploymentEditModel(deployment)
            {
                AllFeeds = feeds
            };

            return new EditDeploymentModelResponse(model);
        }
    }
}