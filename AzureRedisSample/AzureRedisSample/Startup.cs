using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AzureRedisSample.Startup))]
namespace AzureRedisSample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
