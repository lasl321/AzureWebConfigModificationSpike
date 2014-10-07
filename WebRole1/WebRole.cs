using System.Linq;
using Microsoft.Web.Administration;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WebRole1
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // Only update web.config if running in an actual Azure web role (not emulated)
            if (RoleEnvironment.IsAvailable && !RoleEnvironment.IsEmulated)
            {
                using (var serverManager = new ServerManager())
                {
                    // "Web" is the site name in the service definition file (i.e.
                    // the "name" attribute under ServiceDefinition/WebRole/Sites/Site
                    // in the ServiceDefintion.csdef file).
                    var siteConfig = serverManager.Sites[RoleEnvironment.CurrentRoleInstance.Id + "_Web"];

                    // Update the appropriate setting under appSettings in web.config
                    var webConfig = siteConfig.GetWebConfiguration();
                    var appSettings = webConfig.GetSection("appSettings").GetCollection();

                    try
                    {
                        AddElement(appSettings,
                                   "NewRelic.AppName",
                                   RoleEnvironment.GetConfigurationSettingValue("NewRelic.AppName"));
                    }
                        // RoleEnvironmentException will be thrown if "NewRelic.AppName"
                        // is not found in the service configuration file.
                    catch (RoleEnvironmentException)
                    {
                    }

                    serverManager.CommitChanges();
                }
            }

            return base.OnStart();
        }

        private void AddElement(ConfigurationElementCollection appSettings, string key, string value)
        {
            if (appSettings.Any(t => t.GetAttributeValue("key").ToString() == key))
            {
                appSettings.Remove(appSettings.First(t => t.GetAttributeValue("key").ToString() == key));
            }

            var addElement = appSettings.CreateElement("add");
            addElement["key"] = key;
            addElement["value"] = value;
            appSettings.Add(addElement);
        }
    }
}