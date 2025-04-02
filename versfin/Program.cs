namespace versfin
{
    using System;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using Microsoft.Win32.SafeHandles;
    using System.IO;
    using System.Windows.Forms;
   // using System.Timers;
  
    using System.Diagnostics.PerformanceData;

    namespace ProcessMonitorApp
    {
        class Program
        {
            // Импорт Windows API
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern SafeFileHandle CreateFile(
                string lpFileName,
                uint dwDesiredAccess,
                uint dwShareMode,
                IntPtr lpSecurityAttributes,
                uint dwCreationDisposition,
                uint dwFlagsAndAttributes,
                IntPtr hTemplateFile);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern bool ReadDirectoryChangesW(
                SafeFileHandle hDirectory,
                IntPtr lpBuffer,
                uint nBufferLength,
                bool bWatchSubtree,
                uint dwNotifyFilter,
                out uint lpBytesReturned,
                IntPtr lpOverlapped,
                IntPtr lpCompletionRoutine);

            const uint FILE_LIST_DIRECTORY = 0x0001;
            const uint FILE_SHARE_READ = 0x00000001;
            const uint FILE_SHARE_WRITE = 0x00000002;
            const uint OPEN_EXISTING = 3;
            const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;

            // Для мониторинга производительности
            private static PerformanceCounter cpuCounter;
            private static PerformanceCounter ramCounter;
            private static Process monitoredProcess;
            private static System.Timers.Timer monitoringTimer;

            [STAThread]
            static void Main(string[] args)
            {
                try
                {
                    Console.WriteLine("=== Process Monitor ===");
                    Console.WriteLine("Мониторинг CPU, памяти и файловой активности");

                    string exePath = args.Length > 0 ? args[0] : SelectExecutableFile();
                    if (string.IsNullOrEmpty(exePath))
                    {
                        Console.WriteLine("Файл не выбран. Выход.");
                        return;
                    }

                    Console.WriteLine($"\nМониторинг процесса: {exePath}");
                    Console.WriteLine("Нажмите Ctrl+C для остановки...\n");

                    using (var process = StartProcess(exePath))
                    {
                        monitoredProcess = process;
                        InitializePerformanceCounters(process);

                        // Запускаем таймер для мониторинга CPU и памяти (каждые 2 секунды)
                       monitoringTimer = new System.Timers.Timer(2000);

                        // Мониторинг файловой активности
                        MonitorFileActivity(process);

                        process.WaitForExit();
                        monitoringTimer.Dispose();
                        Console.WriteLine("\nПроцесс завершен.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Критическая ошибка: {ex.Message}");
                }
            }

            static void InitializePerformanceCounters(Process process)
            {
                // Счетчик CPU (требует админских прав)
                cpuCounter = new PerformanceCounter(
                    "Process",
                    "% Processor Time",
                    process.ProcessName,
                    true);

                // Сбрасываем счетчик для первого значения
                cpuCounter.NextValue();

                // Счетчик памяти
                ramCounter = new PerformanceCounter(
                    "Process",
                    "Working Set",
                    process.ProcessName,
                    true);
            }

            static void MonitorPerformance(object state)
            {
                try
                {
                    if (monitoredProcess.HasExited)
                        return;

                    // Получаем значения
                    float cpuUsage = cpuCounter.NextValue();
                    float ramUsage = ramCounter.NextValue() / (1024 * 1024); // в МБ

                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] CPU: {cpuUsage:F2}% | RAM: {ramUsage:F2} MB");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка мониторинга производительности: {ex.Message}");
                }
            }

            static string SelectExecutableFile()
            {
                using (var openFileDialog = new OpenFileDialog()
                {
                    Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*",
                    Title = "Select executable file",
                    CheckFileExists = true,
                    Multiselect = false
                })
                {
                    return openFileDialog.ShowDialog() == DialogResult.OK
                        ? openFileDialog.FileName
                        : null;
                }
            }

            static Process StartProcess(string executablePath)
            {
                if (!File.Exists(executablePath))
                    throw new FileNotFoundException("Файл не существует", executablePath);

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = executablePath,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    },
                    EnableRaisingEvents = true
                };

                process.OutputDataReceived += (sender, e) =>
                { if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine($"[Output] {e.Data}"); };
                process.ErrorDataReceived += (sender, e) =>
                { if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine($"[Error] {e.Data}"); };

                if (!process.Start())
                    throw new Win32Exception($"Не удалось запустить процесс (код: {Marshal.GetLastWin32Error()})");

                Console.WriteLine($"Процесс запущен (PID: {process.Id}, Имя: {process.ProcessName})");
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                return process;
            }

            static void MonitorFileActivity(Process process)
            {
                try
                {
                    string processDir = Path.GetDirectoryName(process.MainModule.FileName);
                    if (string.IsNullOrEmpty(processDir))
                    {
                        Console.WriteLine("Не удалось определить рабочую директорию процесса.");
                        return;
                    }

                    using (var dirHandle = CreateFile(
                        processDir,
                        FILE_LIST_DIRECTORY,
                        FILE_SHARE_READ | FILE_SHARE_WRITE,
                        IntPtr.Zero,
                        OPEN_EXISTING,
                        FILE_FLAG_BACKUP_SEMANTICS,
                        IntPtr.Zero))
                    {
                        if (dirHandle.IsInvalid)
                            throw new Win32Exception(Marshal.GetLastWin32Error());

                        const int bufferSize = 4096;
                        IntPtr buffer = Marshal.AllocHGlobal(bufferSize);

                        try
                        {
                            while (!process.HasExited)
                            {
                                if (ReadDirectoryChangesW(
                                    dirHandle,
                                    buffer,
                                    bufferSize,
                                    false,
                                    0xFF, // Все основные события
                                    out _,
                                    IntPtr.Zero,
                                    IntPtr.Zero))
                                {
                                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Файловая активность в: {processDir}");
                                }
                                Thread.Sleep(500);
                            }
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(buffer);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка мониторинга файлов: {ex.Message}");
                }
            }
        }
    }
}
