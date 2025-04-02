using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace fin
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\Users\Haier\Desktop\Tor Browser\Browser"; // Укажите путь к вашему файлу
            string arguments = ""; // Аргументы командной строки, если нужны

            // Создаем и запускаем процесс
            Process process = new Process();
            process.StartInfo.FileName = filePath;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            try
            {
                process.Start();
                Console.WriteLine($"Процесс запущен (ID: {process.Id})");

                // Мониторим нагрузку в отдельном потоке
                Thread monitoringThread = new Thread(() => MonitorProcess(process));
                monitoringThread.Start();

                // Ждем завершения процесса
                process.WaitForExit();
                Console.WriteLine($"Процесс завершен с кодом: {process.ExitCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            Console.ReadKey();
        }

        static void MonitorProcess(Process process)
        {
            try
            {
                while (!process.HasExited)
                {
                    // Получаем текущую нагрузку
                    float cpuUsage = GetCpuUsage(process);
                    long memoryUsage = process.WorkingSet64 / 1024 / 1024; // в МБ
                    TimeSpan cpuTime = process.TotalProcessorTime;

                    Console.WriteLine($"CPU: {cpuUsage:0.0}%, Memory: {memoryUsage} MB, CPU Time: {cpuTime}");

                    Thread.Sleep(1000); // Пауза между замерами (1 секунда)
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка мониторинга: {ex.Message}");
            }
            Console.ReadKey();
        }

        static float GetCpuUsage(Process process)
        {
            // Для точного замера CPU нужно два замера с интервалом
            var startTime = DateTime.Now;
            var startCpuUsage = process.TotalProcessorTime;

            Thread.Sleep(500); // Интервал замера (0.5 секунды)

            var endTime = DateTime.Now;
            var endCpuUsage = process.TotalProcessorTime;

            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;

            // Учитываем все ядра процессора
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

            return (float)cpuUsageTotal * 100;
        }
     
    }

}
    

