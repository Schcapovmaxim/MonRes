using System.Diagnostics;

namespace Universal.Test.SMA
{
    [TestClass]
    public class UnitTest1
    {
        // Путь к тестовому файлу (можно изменить на ваш)
        private static readonly string TestFilePath = @"C:\Users\Haier\Desktop\txt\17.txt";
        private static Process _startedProcess; // Переменная для хранения процесса

        // Этот метод будет запускаться перед всеми тестами
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            // Проверяем, существует ли файл
            if (!File.Exists(TestFilePath))
            {
                // Создаём файл, если его нет
                File.WriteAllText(TestFilePath, "Это тестовый файл, созданный автоматически.");
                Console.WriteLine($"Тестовый файл создан: {TestFilePath}");
            }
            else
            {
                Console.WriteLine($"Используется существующий тестовый файл: {TestFilePath}");
            }
            try
            {
                _startedProcess = Process.Start(new ProcessStartInfo(TestFilePath) { UseShellExecute = true });
                Console.WriteLine($"Файл запущен: {TestFilePath}");
                bool isRunning = !_startedProcess.HasExited;
                if (isRunning)
                {
                    Console.WriteLine($"Процесс {_startedProcess.Id} работает");
                }
                else { Console.WriteLine($"Процесс не работает"); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запуске файла: {ex.Message}");
            }
        }
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}