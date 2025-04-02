namespace finalversion
{
    using System;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using Microsoft.Win32.SafeHandles;
    using System.IO;
    using System.Windows.Forms;

    namespace ProcessMonitorApp
    {
        class Program
        {
            // Импорт Windows API для мониторинга файлов
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

            [STAThread] // Необходим для OpenFileDialog
            static void Main(string[] args)
            {
                Console.WriteLine("=== Process File Activity Monitor ===");

                // Выбор файла через диалог
                string exePath = SelectExecutableFile();
                if (string.IsNullOrEmpty(exePath))
                {
                    Console.WriteLine("Файл не выбран. Выход.");
                    return;
                }

                Console.WriteLine($"\nМониторинг файловой активности для: {exePath}");
                Console.WriteLine("Нажмите Ctrl+C для остановки...\n");

                MonitorProcess(exePath);
            }

            static string SelectExecutableFile()
            {
                var openFileDialog = new OpenFileDialog()
                {
                    Filter = "Исполняемые файлы (*.exe)|*.exe|Все файлы (*.*)|*.*",
                    Title = "Выберите исполняемый файл",
                    CheckFileExists = true
                };

                return openFileDialog.ShowDialog() == DialogResult.OK
                    ? openFileDialog.FileName
                    : null;
            }

            static void MonitorProcess(string executablePath)
            {
                if (!File.Exists(executablePath))
                {
                    Console.WriteLine($"Ошибка: файл не существует - {executablePath}");
                    return;
                }

                try
                {
                    using (var process = new Process())
                    {
                        process.StartInfo = new ProcessStartInfo
                        {
                            FileName = executablePath,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        };

                        process.Start();
                        Console.WriteLine($"Процесс запущен (PID: {process.Id})");

                        // Мониторинг файловой активности
                        MonitorFileActivity(process);

                        process.WaitForExit();
                        Console.WriteLine("\nПроцесс завершен.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
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
                                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Обнаружена активность в: {processDir}");
                                }
                                System.Threading.Thread.Sleep(500);
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
                    Console.WriteLine($"Ошибка мониторинга: {ex.Message}");
                }
            }
        }
    }
}
