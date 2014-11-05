using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.WebEnd.Models;

namespace Codestellation.Galaxy.WebEnd
{
    public class AgentModule : CrudModule
    {
        private readonly DashBoard _dashBoard;
        public const string Path = "agent";

        public AgentModule(DashBoard dashBoard) : base(Path)
        {
            _dashBoard = dashBoard;
        }


        protected override CrudOperations SupportedOperations
        {
            get { return CrudOperations.GetList; }
        }

        protected override object GetList(dynamic parameters)
        {
            return new AgentListModel();
        }
    }
}