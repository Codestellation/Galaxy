using Nancy;
using Nancy.Testing;
using NUnit.Framework;

namespace Codestellation.Galaxy.Tests.WebEnd
{
    [TestFixture, Ignore("Does not work at the moment")]
    public class FeedModuleTest
    {
        private Bootstrapper _bootstrapper;
        private Browser _browser;

        [SetUp]
        public void Create_browser_and_log_in()
        {
            _bootstrapper = new Bootstrapper();
            _browser = new Browser(_bootstrapper);

            var loginResponse = _browser.Post("/login", with =>
            {

                with.HttpRequest();
                with.FormValue("Login", "admin");
                with.FormValue("Password", "admin");
            });
        }


        [Test]
        public void Shows_all_feeds_from_dashboard()
        {
            var response = _browser.Get("/feed", with => with.HttpRequest());

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}