using Microsoft.Data.Sqlite;
using ClosedXML.Excel;
using Test.Models;
using Test.Infrastructure;

namespace Test.Data
{
    public class SqliteDataManager
    {
        private readonly string _connectionString = "Data Source=schedule.db";
        private readonly Logger _logger;
        public SqliteDataManager(Logger logger)
        {
            _logger = logger;
            InitDatabase();
        }
        private void InitDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Lessons (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        GroupName TEXT,
                        DayInfo TEXT,
                        PairNumber TEXT,
                        LessonName TEXT,
                        Auditory TEXT
                    )";
                command.ExecuteNonQuery();
            }
        }

        public void ClearAll()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var cmd = new SqliteCommand("DELETE FROM Lessons", connection);
                cmd.ExecuteNonQuery();
            }
        }

        public void ImportFromExcel(string filePath)
        {
            if (!File.Exists(filePath)) return;
            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    using (var connection = new SqliteConnection(_connectionString))
                    {
                        connection.Open();
                        using (var transaction = connection.BeginTransaction())
                        {
                            var dropCmd = connection.CreateCommand();
                            dropCmd.Transaction = transaction;
                            dropCmd.CommandText = "DELETE FROM Lessons";
                            dropCmd.ExecuteNonQuery();
                            int count = 0;
                            foreach (var groupName in AppConfiguration.AvailableGroups)
                            {
                                int groupCol = FindGroupColumn(worksheet, groupName, 3);
                                if (groupCol == -1) continue;
                                string currentDay = "";
                                foreach (var row in worksheet.RowsUsed().Where(r => r.RowNumber() >= 5))
                                {
                                    var dayVal = row.Cell(1).GetValue<string>()?.Trim();
                                    if (!string.IsNullOrEmpty(dayVal))
                                    {
                                        currentDay = dayVal;
                                    }
                                    string pair = row.Cell(2).GetValue<string>()?.Trim() ?? "";
                                    string lesson = row.Cell(groupCol).GetValue<string>()?.Trim() ?? "";
                                    string aud = row.Cell(groupCol + 1).GetValue<string>()?.Trim() ?? "";
                                    if (!string.IsNullOrEmpty(lesson))
                                    {
                                        var insertCmd = connection.CreateCommand();
                                        insertCmd.Transaction = transaction;
                                        insertCmd.CommandText = "INSERT INTO Lessons (GroupName, DayInfo, PairNumber, LessonName, Auditory) VALUES (@g, @d, @p, @l, @a)";
                                        insertCmd.Parameters.AddWithValue("@g", groupName);
                                        insertCmd.Parameters.AddWithValue("@d", currentDay);
                                        insertCmd.Parameters.AddWithValue("@p", pair);
                                        insertCmd.Parameters.AddWithValue("@l", lesson);
                                        insertCmd.Parameters.AddWithValue("@a", aud);
                                        insertCmd.ExecuteNonQuery();
                                        count++;
                                    }
                                }
                            }
                            transaction.Commit();
                            _logger?.Log($"✅ База обновлена! Групп найдено: {AppConfiguration.AvailableGroups.Length}, Занятий загружено: {count}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.Log($"❌ Ошибка импорта: {ex.Message}");
            }
        }

        private int FindGroupColumn(IXLWorksheet ws, string group, int rowNum)
        {
            var row = ws.Row(rowNum);
            foreach (var cell in row.CellsUsed())
            {
                if (cell.GetString().Trim().Equals(group, StringComparison.OrdinalIgnoreCase))
                    return cell.Address.ColumnNumber;
            }
            return -1;
        }

        public List<string[]> GetLessons(string group)
        {
            var list = new List<string[]>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var cmd = new SqliteCommand("SELECT DayInfo, PairNumber, LessonName, Auditory FROM Lessons WHERE GroupName = @g", connection);
                cmd.Parameters.AddWithValue("@g", group);
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(new string[] { r.GetString(0), r.GetString(1), r.GetString(2), r.GetString(3) });
                }
            }
            return list;
        }
    }
}