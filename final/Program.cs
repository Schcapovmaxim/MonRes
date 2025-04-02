using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;


namespace final
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\Users\Haier\Desktop\Tor Browser\Browser"; // Укажите путь к программе
            string arguments = ""; // Аргументы командной строки (если нужны)

            Process process = new Process();
            process.StartInfo.FileName = filePath;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            try
            {
                process.Start();
                Console.WriteLine($"Процесс запущен (PID: {process.Id})");

                // Запускаем мониторинг сети в отдельном потоке
                Thread networkMonitorThread = new Thread(() => MonitorNetwork(process));
                networkMonitorThread.Start();

                process.WaitForExit();
                Console.WriteLine($"Процесс завершился с кодом: {process.ExitCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            Console.ReadKey();
        }

        static void MonitorNetwork(Process process)
        {
            try
            {
                PerformanceCounter sentCounter = new PerformanceCounter(
                    "Process", "IO Data Bytes/sec", process.ProcessName);
                PerformanceCounter receivedCounter = new PerformanceCounter(
                    "Process", "IO Read Bytes/sec", process.ProcessName);

                while (!process.HasExited)
                {
                    float bytesSent = sentCounter.NextValue();
                    float bytesReceived = receivedCounter.NextValue();

                    Console.WriteLine($"Сеть: Отправлено ≈ {bytesSent / 1024:0.00} КБ/с | Получено ≈ {bytesReceived / 1024:0.00} КБ/с");
                    Thread.Sleep(1000); // Пауза 1 сек
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка мониторинга сети: {ex.Message}");
            }
            Console.ReadKey();
        }
    }
    
}
