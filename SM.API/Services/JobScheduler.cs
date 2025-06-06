using Hangfire;

namespace SM.API.Services
{
    public class JobScheduler : IHostedService
    {
        private readonly IServiceProvider _provider;

        public JobScheduler(IServiceProvider provider)
        {
            _provider = provider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _provider.CreateScope();
            RecurringJob.AddOrUpdate<Jobs>(
                "Update_Member_Status",
                job => job.UpdateMemberStatus(),
                "0 0,12 * * *");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
