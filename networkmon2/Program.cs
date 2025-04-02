using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;

namespace networkmon2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Мониторинг процессов и сетевой активности...");

            // Выбор процесса для мониторинга (например, "chrome")
            string processName = "firefox";

            while (true)
            {
                // Получаем все процессы с указанным именем
                var processes = Process.GetProcessesByName(processName);

                if (processes.Length > 0)
                {
                    foreach (var process in processes)
                    {
                        // Выводим информацию о процессе
                        Console.WriteLine($"Процесс: {process.ProcessName} (ID: {process.Id})");
                        Console.WriteLine($"  CPU: {process.TotalProcessorTime.TotalMilliseconds} мс");
                        Console.WriteLine($"  Память: {process.WorkingSet64 / 1024 / 1024} МБ");

                        // Получаем сетевую активность для процесса
                       // var networkStats = GetIPv4GlobalStatistics(process.Id);
                       // if (networkStats != null)
                        {
                       //     Console.WriteLine($"  Сетевой трафик: {networkStats.BytesReceived / 1024} КБ получено, {networkStats.BytesSent / 1024} КБ отправлено");
                        }
                       // else
                        {
                       //     Console.WriteLine("  Сетевая активность не обнаружена.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Процесс {processName} не найден.");
                }

                // Пауза перед следующим обновлением
                Thread.Sleep(5000); // 5 секунд
            }
        }
    }
}
