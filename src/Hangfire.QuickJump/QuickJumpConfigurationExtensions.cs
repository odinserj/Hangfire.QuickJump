using System;
using System.Reflection;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Hangfire.Dashboard.Extensions;
using Hangfire.QuickJump.Dashboard;

namespace Hangfire.QuickJump
{
    public static class QuickJumpConfigurationExtensions
    {
        public static IGlobalConfiguration UseQuickJump([NotNull] this IGlobalConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            DashboardRoutes.Routes.Add("/quick-jump", new QuickJumpDispatcher());
            DashboardRoutes.Routes.AddRazorPage("/recurring/details/(?<JobId>.+)", x => new RecurringJobDetails(x.Groups["JobId"].Value));

            var assembly = typeof(QuickJumpConfigurationExtensions).GetTypeInfo().Assembly;

            var jsPath = DashboardRoutes.Routes.Contains("/js[0-9]+") ? "/js[0-9]+" : "/js[0-9]{3}";
            DashboardRoutes.Routes.Append(jsPath, new EmbeddedResourceDispatcher(assembly, "Hangfire.QuickJump.Dashboard.quick-jump.js"));
            DashboardRoutes.Routes.Append(jsPath, new DynamicJsDispatcher());

            var cssPath = DashboardRoutes.Routes.Contains("/css[0-9]+") ? "/css[0-9]+" : "/css[0-9]{3}";
            DashboardRoutes.Routes.Append(cssPath, new EmbeddedResourceDispatcher(assembly, "Hangfire.QuickJump.Dashboard.quick-jump.css"));

            return configuration;
        }
    }
}