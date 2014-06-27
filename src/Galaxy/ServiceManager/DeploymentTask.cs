using Codestellation.Galaxy.Domain;
using System.Collections.Generic;

namespace Codestellation.Galaxy.ServiceManager
{
    public class DeploymentTask
    {
        private readonly LinkedList<ServiceOperation> _operations =
            new LinkedList<ServiceOperation>();

        private readonly string _name;
        private readonly Deployment _deployment;
        private readonly NugetFeed _feed;
        private readonly string _targetPath;

        public NugetFeed Feed
        {
            get { return _feed; }
        }

        public string TargetPath
        {
            get { return _targetPath; }
        } 

        public IEnumerable<ServiceOperation> Operations
        {
            get { return _operations; }
        }

        public void Add(ServiceOperation operation)
        {
            _operations.AddLast(operation);
        }

        public Deployment Deployment
        {
            get { return _deployment; }
        }
        public string Name
        {
            get { return _name; }
        } 

        public DeploymentTask(string name, Deployment deployment, NugetFeed feed, string targetPath)
        {
            _name = name;
            _deployment = deployment;
            _feed = feed;
            _targetPath = targetPath;
        }

        public DeploymentTask(string name, Deployment deployment, NugetFeed feed, string targetPath, 
            IEnumerable<ServiceOperation> operations) :
            this(name, deployment, feed, targetPath)
        {
            foreach (var op in operations)
	        {
                _operations.AddLast(op);
	        }            
        }
    }
}
