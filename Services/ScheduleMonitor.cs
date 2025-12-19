using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers; 
namespace Test.Services
{
    public class ScheduleMonitor
    {
        private readonly DownloadService _downloadService;
        private readonly Logger _logger;
        private readonly System.Timers.Timer _timer;
        public event Action<string>? OnScheduleUpdated;
        public ScheduleMonitor(DownloadService downloadService, Logger logger)
        {
            _downloadService = downloadService;
            _logger = logger;
            _timer = new System.Timers.Timer(3600000);
            _timer.Elapsed += async (s, e) => await PerformUpdate();
            _timer.AutoReset = true;
        }
        public void Start()
        {
            _timer.Start();
            _logger.Log("🕒 Мониторинг сайта запущен (проверка каждый час).");
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                await PerformUpdate();
            });
        }
        public void Stop() => _timer.Stop();
        private async Task PerformUpdate()
        {
            try
            {
                _logger.Log("🌐 Проверка сайта колледжа на обновление расписания...");
                string? path = await _downloadService.UpdateSchedule();

                if (path != null && File.Exists(path))
                {
                    OnScheduleUpdated?.Invoke(Path.GetFullPath(path));
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"❌ Ошибка мониторинга: {ex.Message}");
            }
        }
    }
}