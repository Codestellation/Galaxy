using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Agents;
using Codestellation.Galaxy.WebEnd.Models;
using Nancy.ModelBinding;
using Nancy.Responses;

namespace Codestellation.Galaxy.WebEnd
{
    public class AgentModule : CrudModule
    {
        private AgentBoard _agentBoard;
        public const string Path = "agent";

        public AgentModule(AgentBoard agentBoard)
            : base(Path)
        {
            _agentBoard = agentBoard;
        }

        protected override CrudOperations SupportedOperations
        {
            get { return CrudOperations.GetList | CrudOperations.GetCreate | CrudOperations.PostCreate; }
        }

        protected override object GetList(dynamic parameters)
        {
            var agents = _agentBoard.Agents;
            return new AgentListModel(agents);
        }

        protected override object GetCreate(dynamic parameters)
        {
            return new CreateAgentModel();
        }

        protected override object PostCreate(dynamic parameters)
        {
            var model = this.Bind<CreateAgentModel>();

            var enpoint = new AgentEndpoint {Host = model.Host, Port = model.Port};

            _agentBoard.Create(enpoint);

            return new RedirectResponse("/" + Path);
        }
    }
}