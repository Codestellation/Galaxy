using System.Threading.Tasks;
using System.Web.Http;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Deployments;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.WebEnd.Models;
using Nejdb;

namespace Codestellation.Galaxy.WebEnd.Api.Feeds
{
    public class FeedsController : ApiController
    {
        private readonly FeedBoard _feedBoard;
        private readonly DeploymentBoard _deploymentBoard;
        private Collection _feeds;

        public FeedsController(FeedBoard feedBoard, Repository repository, DeploymentBoard deploymentBoard)
        {
            _feedBoard = feedBoard;
            _deploymentBoard = deploymentBoard;
            _feeds = repository.GetCollection<NugetFeed>();
        }

        public async Task<FeedListModel> Get()
        {
            return new FeedListModel(_feedBoard, _deploymentBoard);
        }
    }
}