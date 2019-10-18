using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Archiver
{
    public class Worker : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<Worker> _logger;

        public Worker(IConfiguration config,
            ILogger<Worker> logger)
        {
            _config = config;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Archiving worker service has started");
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("On execution the archiving worker service will wait for 5 mins before archiving");
                await Task.Delay(5 * 60 * 1000, stoppingToken);

                //Code logic will go here
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Archiving worker service has stopped");
            return base.StopAsync(cancellationToken);
        }
    }
}
