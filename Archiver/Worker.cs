using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Archiver
{
    public class Worker : BackgroundService
    {
        private string _archivedPath;
        private DateTime _archiveLimit;
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

            _archiveLimit = DateTime.UtcNow.AddDays(-3);
            _archivedPath = @"C:\0_Playground\0_Locker\DailyManager";

            return base.StartAsync(cancellationToken);
        }

        // TODO: Need to move this logic to StartAsync.
        //       Also no point using the BackgroundServices base class.
        //       Switch to IHostedService interface instead.
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("On execution the archiving worker service will wait for 5 mins before archiving");

                // Moving the minute delay from 5 to 1 minute
                await Task.Delay(1 * 60 * 1000, stoppingToken);

                //Get directory info
                DirectoryInfo directoryInfo = new DirectoryInfo(_archivedPath);

                //Get directory to archive based on archive limit datatime
                var directories = directoryInfo.EnumerateDirectories().Where(x => !x.Name.StartsWith('_') && x.CreationTime < _archiveLimit);

                foreach(var directory in directories)
                {
                    directory.MoveTo(_archivedPath + @"\_Archived\" + directory.Name);
                }

                _logger.LogInformation("Archiving process completed");

                // Create folder structure for current day, skip if sat & sun
                DateTime today = DateTime.Now;
                string date = today.ToString("yyyyMMdd");
                DirectoryInfo taskDirectoryInfo = new DirectoryInfo($"{_archivedPath}\\{date}");

                if (today.DayOfWeek != DayOfWeek.Saturday
                    && today.DayOfWeek != DayOfWeek.Sunday
                    && !taskDirectoryInfo.Exists)
                {
                    taskDirectoryInfo.Create();
                    File.Create($"{taskDirectoryInfo.FullName}\\{today.ToString("yyyyMMddHHmm")}.md").Dispose();
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Archiving worker service has stopped");
            return base.StopAsync(cancellationToken);
        }
    }
}
