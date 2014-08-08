using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.WebEnd.Models;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;
using Nejdb;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd
{
    public class FeedModule : CrudModule
    {
        private readonly DashBoard _dashBoard;
        private readonly Collection _feeds;
        public const string Path = "feed";

        public FeedModule(DashBoard dashBoard, Repository repository)
            : base(Path)
        {
            _dashBoard = dashBoard;
            _feeds = repository.GetCollection<NugetFeed>();
        }

        protected override CrudOperations SupportedOperations
        {
            get { return CrudOperations.GetList | CrudOperations.GetCreate | CrudOperations.PostCreate | CrudOperations.GetEdit | CrudOperations.PostEdit | CrudOperations.PostDelete; }
        }

        protected override object GetList(dynamic parameters)
        {
            return View["list", new FeedListModel(_dashBoard)];
        }

        protected override object GetCreate(dynamic parameters)
        {
            return View["Edit", new FeedModel()];
        }

        protected override object PostCreate(dynamic parameters)
        {
            FeedModel model = this.Bind();
            var feed = model.ToFeed();
            _feeds.Save(feed, false);

            _dashBoard.AddFeed(feed);

            return new RedirectResponse("/feed");
        }

        protected override object GetEdit(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var feed = _dashBoard.GetFeed(id);
            var model = new FeedModel(feed, false);
            return View["Edit", model];
        }

        protected override object PostEdit(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            FeedModel model = this.Bind();

            var currentFeed = _dashBoard.GetFeed(id);
            var updatedFeed = model.ToFeed();

            currentFeed.Merge(updatedFeed);

            _feeds.Save(currentFeed, false);

            return new RedirectResponse("/feed");
        }

        protected override object PostDelete(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            var feedInUse = _dashBoard.Deployments.Any(x => x.FeedId.Equals(id));

            if (feedInUse)
            {
                return new TextResponse(HttpStatusCode.BadRequest, "Feed use. Remove it from deployments to delete");
            }
        
            _dashBoard.RemoveFeed(id);
            _feeds.Delete(id);

            return "Ok";
        }
    }
}