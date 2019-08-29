using System;
using Hangfire.Common;
using Hangfire.Storage;

namespace Hangfire.QuickJump.Utils
{
    internal static class StorageExtensions
    {
        public static RecurringJobDto GetRecurringJobDto(this IStorageConnection connection, string recurringJobId)
        {
            var hash = connection.GetAllEntriesFromHash($"recurring-job:{recurringJobId}");
            if (hash == null) return null;

            var dto = new RecurringJobDto
            {
                Id = recurringJobId,
                Cron = hash["Cron"]
            };

            try
            {
                if (hash.TryGetValue("Job", out var payload) && !String.IsNullOrWhiteSpace(payload))
                {
                    var invocationData = InvocationData.DeserializePayload(payload);
                    dto.Job = invocationData.DeserializeJob();
                }
            }
            catch (JobLoadException ex)
            {
                dto.LoadException = ex;
            }

            if (hash.ContainsKey("NextExecution"))
            {
                dto.NextExecution = JobHelper.DeserializeNullableDateTime(hash["NextExecution"]);
            }

            if (hash.ContainsKey("LastJobId") && !string.IsNullOrWhiteSpace(hash["LastJobId"]))
            {
                dto.LastJobId = hash["LastJobId"];

                var stateData = connection.GetStateData(dto.LastJobId);
                if (stateData != null)
                {
                    dto.LastJobState = stateData.Name;
                }
            }

            if (hash.ContainsKey("Queue"))
            {
                dto.Queue = hash["Queue"];
            }

            if (hash.ContainsKey("LastExecution"))
            {
                dto.LastExecution = JobHelper.DeserializeNullableDateTime(hash["LastExecution"]);
            }

            if (hash.ContainsKey("TimeZoneId"))
            {
                dto.TimeZoneId = hash["TimeZoneId"];
            }

            if (hash.ContainsKey("CreatedAt"))
            {
                dto.CreatedAt = JobHelper.DeserializeNullableDateTime(hash["CreatedAt"]);
            }

            if (hash.TryGetValue("Error", out var error))
            {
                /*dto.Error
                dto.Error = error;*/
            }

            return dto;
        }
    }
}
