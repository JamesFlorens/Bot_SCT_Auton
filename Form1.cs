using Test.Models;
using Test.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Test
{
    public partial class Form1 : Form, IUpdateHandler
    {
        private BotService? _botService;
        private ScheduleService? _scheduleService;
        private UpdateHandlerService? _updateHandler;
        private StatusReporter? _reporter;
        private Logger _logger;
        private DownloadService _downloadService;
        private System.Windows.Forms.Timer _autoUpdateTimer;
        private string _scheduleFilePath = ""; 
        private ScheduleMonitor _monitor;

        public Form1()
        {
            InitializeComponent();
            _logger = new Logger(listBox1);
            _downloadService = new DownloadService(_logger);
            _monitor = new ScheduleMonitor(_downloadService, _logger);
            _monitor.OnScheduleUpdated += (path) =>
            {
                if (_scheduleService != null)
                {
                    _scheduleService.WatchFile(path);
                    _logger.Log("✅ Расписание успешно синхронизировано с базой.");
                }
            };
            _monitor.Start(); 
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
                _botService = new BotService();
                _botService.Start(key, this);
                _scheduleService = new ScheduleService(_botService.Client!, _logger);
                _updateHandler = new UpdateHandlerService(_scheduleService, _logger);
                _reporter = new StatusReporter(_botService);
                _reporter.Start();
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
            _reporter?.Stop();
            _botService?.Stop();
            label1.Text = "Статус: Остановлен";
            _logger.Log("⏹ Бот полностью остановлен.");
        }
        #endregion

        #region Методы IUpdateHandler (События Telegram)
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (_updateHandler != null)
                await _updateHandler.HandleUpdate(botClient, update);
        }

        public Task HandleErrorAsync(ITelegramBotClient b, Exception e, HandleErrorSource s, CancellationToken ct)
        {
            _logger.Log("⚠️ Ошибка Telegram API: " + e.Message);
            return Task.CompletedTask;
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
                        _scheduleService = new ScheduleService(_botService.Client, _logger);
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
    }
}