using Codestellation.Galaxy.Domain;
using System.Collections.Generic;

namespace Codestellation.Galaxy.ServiceManager
{
    public class DeploymentTask
    {
        private readonly List<ServiceOperation> _operations; 

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

        public IReadOnlyList<ServiceOperation> Operations
        {
            get { return _operations; }
        }

        public void Add(ServiceOperation operation)
        {
            _operations.Add(operation);
        }

        public Deployment Deployment
        {
            get { return _deployment; }
        }

        public string Name
        {
            get { return _name; }
        }

        private DeploymentTask()
        {
            _operations =new List<ServiceOperation>();
        }

        public DeploymentTask(string name, Deployment deployment, NugetFeed feed, string targetPath) : this()
        {
            _name = name;
            _deployment = deployment;
            _feed = feed;
            _targetPath = targetPath;
        }

        public DeploymentTask(string name, Deployment deployment, NugetFeed feed, string targetPath, IEnumerable<ServiceOperation> operations) :
            this(name, deployment, feed, targetPath)
        {
            _operations.AddRange(operations);
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
