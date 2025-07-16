using SMERH.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace SMERH.Core
{
    public class ResourceMonitor
    {
        private readonly PerformanceCounter _cpuCounter;
        private readonly PerformanceCounter _ramCounter;
        private readonly List<float> _cpuReadings = new List<float>();
        private readonly List<float> _ramReadings = new List<float>();

        // Эталонные значения (можно настраивать)
        public float CpuThreshold { get; set; } = 80.0f; // %
        public float RamThreshold { get; set; } = 1024.0f; // MB
        public float CpuAnomalyFactor { get; set; } = 3.0f;
        public float RamAnomalyFactor { get; set; } = 2.0f;
        public int SampleSize { get; set; } = 10;

        public ResourceMonitor(string processName = null)
        {
            if (string.IsNullOrEmpty(processName))
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _ramCounter = new PerformanceCounter("Process", "Working Set", "_Total");
            }
            else
            {
                _cpuCounter = new PerformanceCounter("Process", "% Processor Time", processName);
                _ramCounter = new PerformanceCounter("Process", "Working Set", processName);
            }

            // Первое значение всегда 0, нужно пропустить
            _cpuCounter.NextValue();
        }

        public MonitoringResult GetCurrentMetrics()
        {
            float cpuUsage = _cpuCounter.NextValue() / Environment.ProcessorCount;
            float ramUsageMB = _ramCounter.NextValue() / (1024 * 1024);

            // Добавляем текущие показания
            _cpuReadings.Add(cpuUsage);
            _ramReadings.Add(ramUsageMB);

            // Ограничиваем размер выборки
            if (_cpuReadings.Count > SampleSize)
            {
                _cpuReadings.RemoveAt(0);
                _ramReadings.RemoveAt(0);
            }

            return new MonitoringResult
            {
                CpuUsage = cpuUsage,
                RamUsage = ramUsageMB,
                HasCpuAnomaly = CheckForAnomaly(_cpuReadings, CpuThreshold, CpuAnomalyFactor),
                HasRamAnomaly = CheckForAnomaly(_ramReadings, RamThreshold, RamAnomalyFactor),
                HasMemoryLeak = CheckForMemoryLeak(_ramReadings)
            };
        }

        // Делаем методы protected internal для тестирования
        protected internal bool CheckForAnomaly(List<float> readings, float threshold, float anomalyFactor)
        {
            if (readings.Count < 3) return false;

            float current = readings[readings.Count - 1];
            if (current < threshold) return false;

            float sum = 0;
            for (int i = 0; i < readings.Count - 1; i++)
            {
                sum += readings[i];
            }
            float average = sum / (readings.Count - 1);

            return current > average * anomalyFactor;
        }

        protected internal bool CheckForMemoryLeak(List<float> ramReadings)
        {
            if (ramReadings.Count < SampleSize) return false;

            float sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;
            for (int i = 0; i < ramReadings.Count; i++)
            {
                sumX += i;
                sumY += ramReadings[i];
                sumXY += i * ramReadings[i];
                sumX2 += i * i;
            }

            float n = ramReadings.Count;
            float slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);

            return slope > 10.0f;
        }
    }

    public class MonitoringResult
    {
        public float CpuUsage { get; set; }
        public float RamUsage { get; set; }
        public bool HasCpuAnomaly { get; set; }
        public bool HasRamAnomaly { get; set; }
        public bool HasMemoryLeak { get; set; }
    }
}

