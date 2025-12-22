using Test.Models;
using Test.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection; 
using Test.Data;
using Test.Infrastructure;
using Test.Postgres;

namespace Test
{
    public partial class Form1 : Form, IUpdateHandler
    {
        private TelegramBotEngine? _botService;
        private ResponseFormatter? _scheduleService;
        private UserActionHandler? _updateHandler;
        private StatusReporter? _reporter;
        private Logger _logger;
        private ExcelFileProvider _downloadService;
        private string _scheduleFilePath = "";
        private ScheduleMonitor _monitor;
        public Form1()
        {
            InitializeComponent();
            _logger = new Logger(listBox1);
            Test.Postgres.DatabaseInitializer.EnsureTablesCreated(_logger);
            _downloadService = new ExcelFileProvider(_logger);
            _monitor = new ScheduleMonitor(_downloadService, _logger);
            _monitor.OnScheduleUpdated += (path) =>
            {
                if (_scheduleService != null)
                {
                    _scheduleService.WatchFile(path);
                    _logger.Log("✅ Расписание успешно синхронизировано с базой.");
                }
            };
            this.Load += Form1_Load;
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            SetVersionInTitle();
            try
            {
                _logger.Log("🔍 Проверка наличия обновлений...");
                UpdateService updateService = new UpdateService(_logger);
                await updateService.CheckForUpdatesAsync();
            }
            catch (Exception ex)
            {
                _logger.Log($"⚠️ Ошибка при автопроверке: {ex.Message}");
            }
        }
        private void SetVersionInTitle()
        {
            this.Text = $"Bot Panel [{AppInfoService.GetFormattedVersion()}]";
        }
        #region Управление ботом
        private void StartBot()
        {
            string key = textBoxApiKey.Text.Trim();
            if (string.IsNullOrEmpty(key))
            {
                MessageBox.Show("Введите API ключ!");
                return;
            }
            try
            {
                _botService = new TelegramBotEngine();
                _botService.Start(key, this);
                _scheduleService = new ResponseFormatter(_botService.Client!, _logger);
                _updateHandler = new UserActionHandler(_scheduleService, _logger);
                _reporter = new StatusReporter(_botService, _logger);
                _reporter.Start();
                _downloadService.SetReporter(_reporter);
                _monitor.Start();

                label1.Text = "Статус: Работает";
                _logger.Log("🚀 Бот запущен и готов к работе.");
            }
            catch (Exception ex)
            {
                _logger.Log("❌ Ошибка запуска: " + ex.Message);
            }
        }
        private void StopBot()
        {
            _monitor?.Stop();
            _reporter?.Stop();
            _botService?.Stop();
            label1.Text = "Статус: Остановлен";
            _logger.Log("⏹ Бот полностью остановлен.");
        }
        #endregion
        #region Методы IUpdateHandler (События Telegram)
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var handler = new TelegramHandlerService(_logger, _updateHandler);
            await handler.HandleUpdateAsync(botClient, update, ct);
        }
        public Task HandleErrorAsync(ITelegramBotClient b, Exception e, HandleErrorSource s, CancellationToken ct)
        {
            var handler = new TelegramHandlerService(_logger, null);
            return handler.HandleErrorAsync(b, e, s, ct);
        }
        #endregion
        #region Кнопки интерфейса
        private void button1_Click(object sender, EventArgs e) => StartBot();
        private void button2_Click(object sender, EventArgs e) => StopBot();
        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Excel Files|*.xlsx;*.xls" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _scheduleFilePath = ofd.FileName;
                    if (_scheduleService == null && _botService?.Client != null)
                    {
                        _scheduleService = new ResponseFormatter(_botService.Client, _logger);
                    }
                    _scheduleService?.WatchFile(_scheduleFilePath);
                    _logger.Log($"📄 Файл выбран вручную: {Path.GetFileName(_scheduleFilePath)}");
                }
            }
        }
        private void button4_Click(object sender, EventArgs e) => Application.Exit();
        private void buttonSelectLogPath_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "Text files|*.txt" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    _logger?.SetFile(sfd.FileName);
                    _logger?.Log($"📝 Логи будут дублироваться в: {sfd.FileName}");
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            _logger?.Log("🔄 Выполняется перезапуск бота...");
            StopBot();
            StartBot();
        }
        #endregion

        private void textBoxApiKey_TextChanged(object sender, EventArgs e)
        {

        }
    }
}