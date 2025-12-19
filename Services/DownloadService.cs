using NPOI.HSSF.UserModel; // Для работы с .xls
using NPOI.XSSF.UserModel; // Для работы с .xlsx
using NPOI.SS.UserModel;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Test.Services
{
    public class DownloadService
    {
        private readonly Logger _logger;
        private readonly SchedulerParser _parser;

        // Имена временных файлов
        private readonly string _localXls = "downloaded_schedule.xls";
        private readonly string _localXlsx = "auto_schedule.xlsx";

        public DownloadService(Logger logger)
        {
            _logger = logger;
            _parser = new SchedulerParser(logger);
        }

        /// <summary>
        /// Основной метод: находит, скачивает и (если надо) конвертирует расписание.
        /// </summary>
        public async Task<string?> UpdateSchedule()
        {
            try
            {
                // 1. Пытаемся получить ссылку с сайта
                string? url = await _parser.GetLatestScheduleUrl();
                if (string.IsNullOrEmpty(url))
                {
                    _logger.Log("⚠️ Не удалось найти ссылку на расписание на сайте.");
                    return null;
                }

                _logger.Log($"🔗 Найдено актуальное расписание: {Path.GetFileName(url)}");

                // 2. Скачиваем файл в память
                using var client = new HttpClient();
                var data = await client.GetByteArrayAsync(url);

                // 3. Если файл уже в формате .xlsx
                if (url.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    await File.WriteAllBytesAsync(_localXlsx, data);
                    return _localXlsx;
                }

                // 4. Если файл в старом формате .xls
                await File.WriteAllBytesAsync(_localXls, data);

                _logger.Log("🔄 Запуск конвертации .xls -> .xlsx...");
                ConvertXlsToXlsx(_localXls, _localXlsx);

                return _localXlsx;
            }
            catch (Exception ex)
            {
                _logger.Log($"❌ Ошибка в DownloadService: {ex.Message}");
                return null;
            }
        }
        public string GetFinalPath()
        {
            return Path.GetFullPath(_localXlsx);
        }
        private void ConvertXlsToXlsx(string source, string dest)
        {
            try
            {
                using var fs = new FileStream(source, FileMode.Open, FileAccess.Read);
                IWorkbook hssfwb = new HSSFWorkbook(fs); // Открываем XLS
                IWorkbook xssfwb = new XSSFWorkbook();   // Создаем XLSX

                var sheet = hssfwb.GetSheetAt(0);
                var outSheet = xssfwb.CreateSheet(sheet.SheetName);

                // Построчное копирование данных
                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    var row = sheet.GetRow(i);
                    var outRow = outSheet.CreateRow(i);
                    if (row == null) continue;

                    for (int j = 0; j < row.LastCellNum; j++)
                    {
                        var cell = row.GetCell(j);
                        if (cell == null) continue;

                        var outCell = outRow.CreateCell(j);

                        // Копируем значение как строку (для расписания этого достаточно)
                        outCell.SetCellValue(cell.ToString());
                    }
                }

                // Сохраняем результат
                using var outFs = new FileStream(dest, FileMode.Create);
                xssfwb.Write(outFs);

                _logger.Log("✅ Конвертация завершена успешно.");
            }
            catch (Exception ex)
            {
                _logger.Log($"❌ Ошибка конвертации: {ex.Message}");
                throw; // Пробрасываем ошибку выше
            }
        }
    }
}