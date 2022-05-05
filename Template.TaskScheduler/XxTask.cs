using Microsoft.Extensions.Logging;
using Polly;
using Quartz;

namespace Template.TaskScheduler
{
    internal class XxTask : IJob
    {
        private readonly XxContext _dbContext;
        private readonly ILogger<XxTask> _logger;
        public XxTask(XxContext erpContext, ILogger<XxTask> logger)
        {
            _dbContext = erpContext;
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            if (context.ScheduledFireTimeUtc == null)
            {
                throw new Exception("quartzNet ScheduledFireTimeUtc is null");
            }
            var ScheduledfireTimeLocal = context.ScheduledFireTimeUtc.Value.ToLocalTime().DateTime;

            await Policy
              .Handle<Exception>()
              .WaitAndRetryForeverAsync(x => TimeSpan.FromHours(1))
              .ExecuteAsync(async () =>
              {
                  _logger.LogInformation($"Begin task,ScheduledfireTimeLocal:{ScheduledfireTimeLocal:yyyy-MM-dd}");

                  // Write your Bussiness logic

                  await _dbContext.SaveChangesAsync();
              });
        }

    }
}
