using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.WebEnd.Models;
using Nancy.ModelBinding;
using Nancy.Responses;
using Nancy.Security;
using Nejdb;

namespace Codestellation.Galaxy.WebEnd
{
    public class OptionsModule : ModuleBase
    {
        private readonly Options _options;
        private readonly Collection _optionsCollection;

        public const string Path = "option";

        public OptionsModule(Repository repository, Options options) : base(Path)
        {
            this.RequiresAuthentication();

            _options = options;
            _optionsCollection = repository.GetCollection<Options>();
            
            Get["/", true] = (parameters, token) => ProcessRequest(GetOptions, token);
            Post["/", true] = (parameters, token) => ProcessRequest(PostOptions, token);
        }

        private object GetOptions()
        {
            return new OptionsModel(_options);
        }

        private object PostOptions()
        {
            var model = this.Bind<OptionsModel>();

            model.Update(_options);

            _optionsCollection.Save(_options, false);

            return new RedirectResponse("/");
        }
    }
}