namespace ConsoleApp2
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\path\to\your\program.exe"; // Укажите путь к файлу
            string arguments = ""; // Аргументы командной строки

            try
            {
                // Проверяем, существует ли файл
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Файл не найден!");
                    return;
                }

                // Запускаем процесс
                Process process = new Process();
                process.StartInfo.FileName = filePath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                Console.WriteLine($"Процесс запущен (PID: {process.Id})");

                // Запускаем мониторинг в отдельном потоке
                Thread monitorThread = new Thread(() => MonitorSystemResources(process));
                monitorThread.Start();

                process.WaitForExit();
                Console.WriteLine($"Процесс завершен. Код выхода: {process.ExitCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void MonitorSystemResources(Process process)
        {
            try
            {
                // Инициализация счетчиков производительности
                PerformanceCounter cpuCounter = new PerformanceCounter(
                    "Process", "% Processor Time", process.ProcessName, true);
                PerformanceCounter ramCounter = new PerformanceCounter(
                    "Process", "Working Set", process.ProcessName, true);
                PerformanceCounter diskReadCounter = new PerformanceCounter(
                    "Process", "IO Read Bytes/sec", process.ProcessName, true);
                PerformanceCounter diskWriteCounter = new PerformanceCounter(
                    "Process", "IO Write Bytes/sec", process.ProcessName, true);
                PerformanceCounter networkSentCounter = new PerformanceCounter(
                    "Network Interface", "Bytes Sent/sec", GetNetworkInterfaceName());
                PerformanceCounter networkReceivedCounter = new PerformanceCounter(
                    "Network Interface", "Bytes Received/sec", GetNetworkInterfaceName());

                // Первый вызов NextValue() возвращает 0, поэтому пропускаем его
                cpuCounter.NextValue();
                diskReadCounter.NextValue();
                diskWriteCounter.NextValue();
                networkSentCounter.NextValue();
                networkReceivedCounter.NextValue();
                Thread.Sleep(1000); // Ждем для актуальных данных

                while (!process.HasExited)
                {
                    float cpuUsage = cpuCounter.NextValue() / Environment.ProcessorCount;
                    float ramUsage = ramCounter.NextValue() / 1024 / 1024; // в МБ
                    float diskRead = diskReadCounter.NextValue() / 1024; // в КБ/с
                    float diskWrite = diskWriteCounter.NextValue() / 1024; // в КБ/с
                    float networkSent = networkSentCounter.NextValue() / 1024; // в КБ/с
                    float networkReceived = networkReceivedCounter.NextValue() / 1024; // в КБ/с

                    Console.WriteLine(
                        $"[Загрузка системы]\n" +
                        $"CPU: {cpuUsage:0.0}%\n" +
                        $"RAM: {ramUsage:0.0} MB\n" +
                        $"Диск: Чтение {diskRead:0.0} КБ/с | Запись {diskWrite:0.0} КБ/с\n" +
                        $"Сеть: Отправка {networkSent:0.0} КБ/с | Получение {networkReceived:0.0} КБ/с\n" +
                        new string('-', 40));

                    Thread.Sleep(1000); // Интервал обновления (1 сек)
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка мониторинга: {ex.Message}");
            }
        }

        // Получаем имя активного сетевого интерфейса
        static string GetNetworkInterfaceName()
        {
            var interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            foreach (var ni in interfaces)
            {
                if (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                {
                    return ni.Name;
                }
            }
            return "*"; // Если интерфейс не найден, используем общий счетчик
        }
    }
}