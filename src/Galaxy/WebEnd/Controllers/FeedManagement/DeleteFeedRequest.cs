using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Controllers.FeedManagement
{
    public class DeleteFeedRequest : IRequest<DeleteFeedResponse>
    {
        public ObjectId Id { get; }

        public DeleteFeedRequest(ObjectId id)
        {
            Id = id;
        }
    }
}