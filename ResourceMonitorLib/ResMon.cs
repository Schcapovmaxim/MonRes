using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ResourceMonitorLib
{
    public class ResourceMonitor
    {
        public float MaxCpuPercentage { get; }
        public float MaxMemoryPercentage { get; }

        public ResourceMonitor(float maxCpuPercentage, float maxMemoryPercentage)
        {
            MaxCpuPercentage = maxCpuPercentage;
            MaxMemoryPercentage = maxMemoryPercentage;
        }

        public (bool isCpuAnomaly, bool isMemoryAnomaly) CheckSystemAnomalies()
        {
            float cpuUsage = GetSystemCpuUsage().Result;
            float memoryUsage = GetSystemMemoryUsage();

            return (
                cpuUsage > MaxCpuPercentage,
                memoryUsage > MaxMemoryPercentage
            );
        }

        public bool CheckForMemoryLeak(string processName, int sampleCount = 5, float increaseThreshold = 0.1f)
        {
            var processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0) return false;

            var process = processes[0];
            var samples = new List<long>();

            for (int i = 0; i < sampleCount; i++)
            {
                process.Refresh();
                samples.Add(process.WorkingSet64);
                Thread.Sleep(1000);
            }

            long initial = samples.First();
            long final = samples.Last();

            return (final - initial) > (initial * increaseThreshold);
        }

        private async Task<float> GetSystemCpuUsage()
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetProcesses().Sum(p => {
                try { return p.TotalProcessorTime.TotalMilliseconds; }
                catch { return 0; }
            });

            await Task.Delay(1000);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetProcesses().Sum(p => {
                try { return p.TotalProcessorTime.TotalMilliseconds; }
                catch { return 0; }
            });

            var cpuUsedMs = endCpuUsage - startCpuUsage;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

            return (float)cpuUsageTotal * 100;
        }

        private float GetSystemMemoryUsage()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return GetWindowsMemoryUsage();
            }
            else
            {
                // Для Linux/macOS используем альтернативный метод
                return GetUnixMemoryUsage();
            }
        }

        private float GetWindowsMemoryUsage()
        {
            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(memStatus))
            {
                return (float)memStatus.dwMemoryLoad;
            }
            return 0;
        }

        private float GetUnixMemoryUsage()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    string[] memInfo = File.ReadAllLines("/proc/meminfo");
                    var memTotal = ParseMemInfoValue(memInfo[0]);
                    var memAvailable = ParseMemInfoValue(memInfo[2]);
                    return 100f * (memTotal - memAvailable) / memTotal;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    // Для macOS используем системную команду vm_stat
                    var process = Process.Start(new ProcessStartInfo("vm_stat")
                    {
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    });
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    // Парсинг вывода vm_stat
                    return ParseMacMemory(output);
                }
            }
            catch { }
            return 0;
        }

        private long ParseMemInfoValue(string line)
        {
            return long.Parse(line.Split(':')[1].Trim().Split(' ')[0]) * 1024;
        }

        private float ParseMacMemory(string output)
        {
            // Упрощенный парсинг для демонстрации
            var lines = output.Split('\n');
            var freeLine = lines.FirstOrDefault(l => l.Contains("Pages free"));
            if (freeLine != null)
            {
                var freePages = long.Parse(freeLine.Split(':')[1].Trim().Split('.')[0]);
                return 10f; // Примерное значение
            }
            return 0;
        }

        #region Windows Native Methods
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
        #endregion
    }
}