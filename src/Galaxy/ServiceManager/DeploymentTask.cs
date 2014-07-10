using Codestellation.Galaxy.Domain;
using System.Collections.Generic;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager
{
    public class DeploymentTask
    {
        private readonly List<OperationBase> _operations; 

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

        public IReadOnlyList<OperationBase> Operations
        {
            get { return _operations; }
        }

        public void Add(OperationBase operation)
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
            _operations =new List<OperationBase>();
        }

        public DeploymentTask(string name, Deployment deployment, NugetFeed feed, string targetPath) : this()
        {
            _name = name;
            _deployment = deployment;
            _feed = feed;
            _targetPath = targetPath;
        }

        public DeploymentTask(string name, Deployment deployment, NugetFeed feed, string targetPath, IEnumerable<OperationBase> operations) :
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
