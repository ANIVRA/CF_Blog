using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(aNewBlog.Startup))]
namespace aNewBlog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
