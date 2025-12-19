using HtmlAgilityPack;


namespace Test.Services
{
    public class SchedulerParser
    {
        private readonly string _pageUrl = "https://sktkomi.ru/ochnoe-otdelenie/";
        private readonly Logger _logger;
        public SchedulerParser(Logger logger) => _logger = logger;
        public async Task<string?> GetLatestScheduleUrl()
        {
            try
            {
                var web = new HtmlWeb();
                var doc = await web.LoadFromWebAsync(_pageUrl);
                var link = doc.DocumentNode.Descendants("a")
                    .Select(a => a.GetAttributeValue("href", ""))
                    .FirstOrDefault(href => href.Contains(".xls") || href.Contains(".xlsx"));
                if (string.IsNullOrEmpty(link)) return null;
                if (!link.StartsWith("http"))
                {
                    link = "https://sktkomi.ru" + (link.StartsWith("/") ? "" : "/") + link;
                }
                return link;
            }
            catch (Exception ex)
            {
                _logger.Log($"❌ Ошибка парсинга сайта: {ex.Message}");
                return null;
            }
        }
    }
}