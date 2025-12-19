using System.Diagnostics;
using System.Net.Http;

namespace Test.Models 
{
    public class BotManager
    {
        private readonly Logger _logger;
        private DateTime _lastUpdateCheck = DateTime.MinValue;

        public BotManager(Logger logger)
        {
            _logger = logger;
        }
        public async Task CheckForUpdates()
        {
            if (DateTime.Now.Hour == 1 && DateTime.Now.Minute == 0 && _lastUpdateCheck.Date != DateTime.Today)
            {
                _lastUpdateCheck = DateTime.Today; 

                try
                {
                    _logger.Log("🚀 Наступило время обслуживания (01:00). Проверка обновлений...");

                    // URL, где лежит твой скомпилированный новый .exe
                    string updateUrl = "https://your-site.com/Bot_New.exe";
                    string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bot_Update.tmp");

                    using (var client = new HttpClient())
                    {
                        // Скачиваем файл
                        var data = await client.GetByteArrayAsync(updateUrl);
                        await File.WriteAllBytesAsync(tempPath, data);
                    }

                    _logger.Log("✅ Файл обновления скачан. Запуск процесса замены...");

                    // Путь к текущему запущенному файлу
                    string currentExe = Process.GetCurrentProcess().MainModule.FileName;

                    // Запускаем Updater.exe (Пункт 2)
                    // Передаем аргументы: текущий путь и путь к скачанному файлу
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "Updater.exe",
                        Arguments = $"\"{currentExe}\" \"{tempPath}\"",
                        UseShellExecute = true
                    };

                    Process.Start(psi);

                    // Закрываем основное приложение, чтобы Updater смог заменить файл
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    _logger.Log($"❌ Ошибка самообновления: {ex.Message}");
                }
            }
        }
    }
}