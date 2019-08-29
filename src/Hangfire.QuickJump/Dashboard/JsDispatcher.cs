using System.Text;
using System.Threading.Tasks;
using Hangfire.Dashboard;

namespace Hangfire.QuickJump.Dashboard
{
    internal class DynamicJsDispatcher : IDashboardDispatcher
    {
        public Task Dispatch(DashboardContext context)
        {
            var builder = new StringBuilder();

            builder.Append(@"(function (hangfire) {")
                .Append("hangfire.config = hangfire.config || {};")
                .AppendFormat("hangfire.config.pathBase = '{0}';", context.Request.PathBase)
                .Append("})(window.Hangfire = window.Hangfire || {});")
                .AppendLine();

            return context.Response.WriteAsync(builder.ToString());
        }
    }
}
