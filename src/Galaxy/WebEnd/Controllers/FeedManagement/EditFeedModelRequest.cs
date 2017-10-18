using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Controllers.FeedManagement
{
    public class EditFeedModelRequest : IRequest<EditFeedModelResponse>
    {
        public ObjectId Id { get; }

        public EditFeedModelRequest(ObjectId id)
        {
            Id = id;
        }
    }
}