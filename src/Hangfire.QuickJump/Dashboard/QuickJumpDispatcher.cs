using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hangfire.Common;
using Hangfire.Dashboard;

namespace Hangfire.QuickJump.Dashboard
{
    internal sealed class QuickJumpDispatcher : IDashboardDispatcher
    {
        public async Task Dispatch(DashboardContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (!"POST".Equals(request.Method, StringComparison.OrdinalIgnoreCase))
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return;
            }

            var jobIds = await context.Request.GetFormValuesAsync("jobid").ConfigureAwait(false);
            var jobId = jobIds.FirstOrDefault()?.Trim();

            string link = null;

            if (!String.IsNullOrEmpty(jobId))
            {
                try
                {
                    var monitoringApi = context.Storage.GetMonitoringApi();
                    var normalizedId = jobId.StartsWith("#") ? jobId.Substring(1) : jobId;

                    var jobDetails = monitoringApi.JobDetails(normalizedId);
                    if (jobDetails != null)
                    {
                        var urlHelper = new UrlHelper(context);
                        link = urlHelper.JobDetails(normalizedId);
                    }
                }
                catch
                {
                    //
                }

                if (link == null)
                {
                    using (var connection = context.Storage.GetConnection())
                    {
                        var recurringJob = connection.GetAllEntriesFromHash($"recurring-job:{jobId}");
                        if (recurringJob != null && recurringJob.Count != 0)
                        {
                            link = "/recurring";
                        }
                    }
                }
            }

            response.StatusCode = link != null ? 200 : 404;

#pragma warning disable 618
            await response.WriteAsync(JobHelper.ToJson(new Dictionary<string, string>
#pragma warning restore 618
            {
                { "location", link }
            }));
        }
    }
}