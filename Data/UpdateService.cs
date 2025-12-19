using System;
using System.Net.Http;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Test.Infrastructure;

namespace Test.Services
{
    public class UpdateService
    {
        private const string VersionUrl = "https://raw.githubusercontent.com/JamesFlorens/Bot_SCT_Auton/refs/heads/main/version.txt";
        private const string ReleaseUrl = "https://github.com/JamesFlorens/Bot_SCT_Auton/releases";
        private const string AppUserAgent = "C#_Bot_Updater_v1";
        private readonly Logger _logger;
        public UpdateService(Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CheckForUpdatesAsync()
        {
            _logger.Log("🌐 Соединение с сервером обновлений...");
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", AppUserAgent);
                client.Timeout = TimeSpan.FromSeconds(10);
                string remoteContent = await client.GetStringAsync(VersionUrl);
                if (string.IsNullOrWhiteSpace(remoteContent))
                {
                    _logger.Log("⚠️ Файл версии на сервере пуст.");
                    return;
                }

                if (!Version.TryParse(remoteContent.Trim(), out Version? remoteVersion))
                {
                    _logger.Log($"⚠️ Не удалось распознать формат версии: {remoteContent}");
                    return;
                }
                Version localVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);
                if (remoteVersion > localVersion)
                {
                    NotifyUpdateAvailable(localVersion, remoteVersion);
                }
                else
                {
                    _logger.Log($"✅ У вас установлена актуальная версия ({localVersion})");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.Log($"🌐 Ошибка сети: Не удалось связаться с GitHub ({ex.StatusCode})");
            }
            catch (Exception ex)
            {
                _logger.Log($"⚠️ Непредвиденная ошибка обновления: {ex.Message}");
            }
        }

        private void NotifyUpdateAvailable(Version local, Version remote)
        {
            _logger.Log($"🚀 Найдена новая версия: {remote} (текущая: {local})");
            string message = $"Доступно обновление ПО!\n\n" + $"Текущая версия: {local}\n" + $"Новая версия: {remote}\n\n" + $"Желаете перейти на страницу загрузки?";
            var result = MessageBox.Show(message,"Обновление системы",MessageBoxButtons.YesNo,MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                try
                {
                    Process.Start(new ProcessStartInfo(ReleaseUrl) { UseShellExecute = true });
                    _logger.Log("🔗 Открыта страница релизов в браузере.");
                }
                catch (Exception ex)
                {
                    _logger.Log($"❌ Не удалось открыть ссылку: {ex.Message}");
                }
            }
        }
    }
}