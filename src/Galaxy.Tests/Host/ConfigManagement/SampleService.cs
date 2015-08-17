﻿using Codestellation.Galaxy.Host;
using Codestellation.Galaxy.Host.ConfigManagement;

namespace Codestellation.Galaxy.Tests.Host.ConfigManagement
{
    public class SampleService : IService, IConsulConfigAware<SampleConfig>
    {
        public SampleConfig Config { get; private set; }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public ValidationResult Accept(SampleConfig config)
        {
            Config = config;
            return new ValidationResult();
        }
    }
}