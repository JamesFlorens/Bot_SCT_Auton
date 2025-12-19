namespace Test.Models
{
    public static class GroupHelper
    {
        public static string? FindGroup(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            string normalizedInput = input.Replace(" ", "").Replace("-", "").ToUpper();

            foreach (var group in ScheduleConfig.AvailableGroups)
            {
                string normalizedTarget = group.Replace(" ", "").Replace("-", "").ToUpper();

                if (normalizedInput == normalizedTarget)
                {
                    return group; 
                }
            }
            return null;
        }
    }
}