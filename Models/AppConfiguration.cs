namespace Test.Models
{
    public static class AppConfiguration
    {
        public static readonly string[] AvailableGroups =
        {
            "ИС-40","ПС-31","ПС-32","ИС-31","ИС-32",
            "ГД-31","Ю-21","Ю-22","БД-21","ИС-21",
            "ГД-21","Ю-11","Ю-12","БД-11","ИС-11","ГД-11"
        };
        public static string? FindGroup(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            string normalizedInput = input.Replace(" ", "").Replace("-", "").ToUpper();
            foreach (var group in AvailableGroups)
            {
                string normalizedGroup = group.Replace(" ", "").Replace("-", "").ToUpper();
                if (normalizedInput == normalizedGroup)
                {
                    return group; 
                }
            }
            return null;
        }
        public static readonly string[] LessonEmojis = { "1️⃣", "2️⃣", "3️⃣", "4️⃣" };
        public static readonly TimeSpan[] WeekdayTimes =
        {
            new TimeSpan(9,0,0), new TimeSpan(10,35,0),
            new TimeSpan(10,45,0), new TimeSpan(12,20,0),
            new TimeSpan(13,0,0), new TimeSpan(14,35,0),
            new TimeSpan(14,45,0), new TimeSpan(16,20,0)
        };
        public static readonly TimeSpan[] SaturdayTimes =
        {
            new TimeSpan(9,0,0), new TimeSpan(10,0,0),
            new TimeSpan(10,10,0), new TimeSpan(11,10,0),
            new TimeSpan(11,20,0), new TimeSpan(12,20,0),
            new TimeSpan(12,30,0), new TimeSpan(13,30,0)
        };
    }
}
