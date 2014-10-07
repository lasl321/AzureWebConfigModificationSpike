using System.Diagnostics;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebRole1.Startup))]
namespace WebRole1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Trace.TraceInformation("Startup.Configuration");
            ConfigureAuth(app);
        }
    }
}
