using System;
using Codestellation.Galaxy.WebEnd.Models;

namespace Codestellation.Galaxy.WebEnd.Controllers.HomepageManagement
{
    public class HomepageModelResponse
    {
        public HomepageModel Model { get; }

        public HomepageModelResponse(HomepageModel model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }
    }
}