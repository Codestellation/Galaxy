using System;
using System.Linq;
using Codestellation.Galaxy.Domain.Notifications;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.WebEnd.Models;
using MediatR;
using Nejdb.Queries;

namespace Codestellation.Galaxy.WebEnd.Controllers.HomepageManagement
{
    public class HomepageRequestHandler : IRequestHandler<HomepageModelRequest, HomepageModelResponse>
    {
        private readonly Repository _repository;

        public HomepageRequestHandler(Repository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public HomepageModelResponse Handle(HomepageModelRequest message)
        {
            var notifications = _repository.Notifications;
            var now = DateTime.UtcNow;
            var thirtyDaysAgo = now.AddDays(-30);
            var greaterThan = Criterions.GreaterThanOrEqual(thirtyDaysAgo);
            var criterion = Criterions.Field(nameof(Notification.CreatedAt), greaterThan);
            var builder = new QueryBuilder(criterion);
            var last30Days = notifications
                .PerformQuery<Notification>(builder)
                .ToList();
            //Sort in descending order
            last30Days.Sort((x, y) => y.CreatedAt.CompareTo(x.CreatedAt));

            var model = new HomepageModel(last30Days);

            return new HomepageModelResponse(model);
        }
    }
}