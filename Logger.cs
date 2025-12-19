namespace Test
{
    public class Logger
    {
        private readonly ListBox _listBox;
        private string _filePath;

        public Logger(ListBox listBox)
        {
            _listBox = listBox;
        }
        public void SetFile(string path) => _filePath = path;
        public void Log(string text)
        {
            string msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {text}";

            // Проверяем, создан ли дескриптор окна
            if (_listBox.IsHandleCreated)
            {
                _listBox.Invoke((MethodInvoker)(() =>
                {
                    _listBox.Items.Add(msg);
                    // Автопрокрутка вниз
                    _listBox.TopIndex = _listBox.Items.Count - 1;
                }));
            }
            else
            {
                // Если окно еще не готово, просто пишем в консоль или файл (если задан)
                Console.WriteLine(msg);
            }

            if (!string.IsNullOrEmpty(_filePath))
                File.AppendAllText(_filePath, msg + Environment.NewLine);
        }
    }
}
