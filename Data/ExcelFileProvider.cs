using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Test.Services;
using Test.Infrastructure;

namespace Test.Data
{
    public class ExcelFileProvider
    {
        private readonly Logger _logger;
        private readonly SchedulerParser _parser;
        private StatusReporter? _reporter;
        private readonly string _localXls = "downloaded_schedule.xls";
        private readonly string _localXlsx = "auto_schedule.xlsx";

        public ExcelFileProvider(Logger logger)
        {
            _logger = logger;
            _parser = new SchedulerParser(logger);
        }
        public void SetReporter(StatusReporter reporter) => _reporter = reporter;
        public async Task<string?> UpdateSchedule()
        {
            try
            {
                _logger.Log("🌐 Подключение к сайту колледжа для проверки расписания...");

                string? url = await _parser.GetLatestScheduleUrl();
                if (string.IsNullOrEmpty(url))
                {
                    _logger.Log("status: Проверка завершена. Изменений на сайте нет.");
                    return null;
                }
                _logger.Log($"✨ Найдено актуальное расписание: {Path.GetFileName(url)}");
                using var client = new HttpClient();
                var data = await client.GetByteArrayAsync(url);
                if (url.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    await File.WriteAllBytesAsync(_localXlsx, data);
                    _logger.Log("🗄 База данных (.xlsx) успешно обновлена.");
                    return _localXlsx;
                }
                await File.WriteAllBytesAsync(_localXls, data);
                _logger.Log("🔄 Запуск конвертации .xls -> .xlsx...");
                ConvertXlsToXlsx(_localXls, _localXlsx);
                _logger.Log("🗄 База данных успешно обновлена после конвертации.");
                return _localXlsx;
            }
            catch (Exception ex)
            {
                _logger.Log($"❌ Ошибка в DownloadService: {ex.Message}");
                return null;
            }
        }
        private void ConvertXlsToXlsx(string source, string dest)
        {
            try
            {
                using var fs = new FileStream(source, FileMode.Open, FileAccess.Read);
                IWorkbook hssfwb = new HSSFWorkbook(fs);
                IWorkbook xssfwb = new XSSFWorkbook();
                var sheet = hssfwb.GetSheetAt(0);
                var outSheet = xssfwb.CreateSheet(sheet.SheetName);
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
                        outCell.SetCellValue(cell.ToString());
                    }
                }
                using var outFs = new FileStream(dest, FileMode.Create);
                xssfwb.Write(outFs);
                _logger.Log("✅ Конвертация завершена успешно.");
            }
            catch (Exception ex)
            {
                _logger.Log($"❌ Ошибка конвертации: {ex.Message}");
                throw;
            }
        }
        public string GetFinalPath() => Path.GetFullPath(_localXlsx);
    }
}