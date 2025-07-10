namespace VovaDataService.Test;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class SystemLoadMonitorTests
{
    [TestMethod]
    public void TestInitialization()
    {
        var monitor = new SystemLoadMonitor();

        Assert.AreEqual(60, monitor.MaxReadingsHistory);
        Assert.AreEqual(1.0f, monitor.SamplingIntervalSeconds);
        Assert.AreEqual(80f, monitor.CpuWarningThreshold);
        Assert.AreEqual(500f, monitor.RamWarningThresholdMB);
    }

    [TestMethod]
    public void TestConfigurableThresholds()
    {
        var monitor = new SystemLoadMonitor
        {
            CpuWarningThreshold = 70f,
            RamWarningThresholdMB = 300f,
            MemoryLeakSlopeThreshold = 0.2f
        };

        Assert.AreEqual(70f, monitor.CpuWarningThreshold);
        Assert.AreEqual(300f, monitor.RamWarningThresholdMB);
        Assert.AreEqual(0.2f, monitor.MemoryLeakSlopeThreshold);
    }

    [TestMethod]
    public void TestReadingUpdates()
    {
        var monitor = new SystemLoadMonitor();
        string report = monitor.GetSystemLoadReport();

        Assert.IsTrue(report.Contains("CPU:"));
        Assert.IsTrue(report.Contains("RAM:"));
        Assert.IsTrue(monitor.CurrentCpuUsage >= 0);
        Assert.IsTrue(monitor.CurrentRamUsageMB >= 0);
    }

    [TestMethod]
    public void TestMaxValuesTracking()
    {
        var monitor = new SystemLoadMonitor();
        float initialMaxCpu = monitor.MaxObservedCpuUsage;
        float initialMaxRam = monitor.MaxObservedRamUsageMB;

        // Имитируем несколько замеров
        for (int i = 0; i < 5; i++)
        {
            monitor.GetSystemLoadReport();
        }

        Assert.IsTrue(monitor.MaxObservedCpuUsage >= initialMaxCpu);
        Assert.IsTrue(monitor.MaxObservedRamUsageMB >= initialMaxRam);
    }

    [TestMethod]
    public void TestMemoryLeakDetection()
    {
        var monitor = new SystemLoadMonitor
        {
            MaxReadingsHistory = 10,
            MinReadingsForAnalysis = 5,
            MemoryLeakSlopeThreshold = 0.1f
        };

        // Имитируем утечку памяти (линейный рост)
        for (int i = 0; i < 10; i++)
        {
            // Используем reflection для подмены значений в очереди
            var ramField = typeof(SystemLoadMonitor)
                .GetField("_ramReadings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var queue = (Queue<float>)ramField.GetValue(monitor);
            queue.Enqueue(i * 20f); // Линейный рост 0, 20, 40... MB

            monitor.GetSystemLoadReport();
        }

        Assert.IsTrue(monitor.PossibleMemoryLeak);
    }

    [TestMethod]
    public void TestAnomalyDetection()
    {
        var monitor = new SystemLoadMonitor
        {
            MaxReadingsHistory = 10,
            MinReadingsForAnalysis = 5
        };

        // Заполняем историю стабильными значениями
        for (int i = 0; i < 9; i++)
        {
            monitor.GetSystemLoadReport();
        }

        // Сохраняем текущий отчет
        string normalReport = monitor.GetSystemLoadReport();

        // Имитируем аномальный скачок CPU
        var cpuField = typeof(SystemLoadMonitor)
            .GetField("_cpuReadings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var queue = (Queue<float>)cpuField.GetValue(monitor);
        queue.Enqueue(95f); // Аномально высокое значение

        string anomalyReport = monitor.GetSystemLoadReport();

        Assert.IsFalse(normalReport.Contains("АНОМАЛИЯ"));
        Assert.IsTrue(anomalyReport.Contains("АНОМАЛИЯ"));
    }

    [TestMethod]
    public void TestThresholdAlerts()
    {
        var monitor = new SystemLoadMonitor
        {
            CpuWarningThreshold = 50f,
            CpuCriticalThreshold = 70f,
            RamWarningThresholdMB = 200f,
            RamCriticalThresholdMB = 300f
        };

        // Подменяем значения
        var cpuField = typeof(SystemLoadMonitor)
            .GetField("_cpuReadings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var ramField = typeof(SystemLoadMonitor)
            .GetField("_ramReadings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var cpuQueue = (Queue<float>)cpuField.GetValue(monitor);
        var ramQueue = (Queue<float>)ramField.GetValue(monitor);

        // Тест предупреждения CPU
        cpuQueue.Enqueue(55f);
        string warningReport = monitor.GetSystemLoadReport();
        Assert.IsTrue(warningReport.Contains("Высокая нагрузка!"));

        // Тест критического CPU
        cpuQueue.Enqueue(75f);
        string criticalReport = monitor.GetSystemLoadReport();
        Assert.IsTrue(criticalReport.Contains("КРИТИЧЕСКАЯ НАГРУЗКА!"));
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void TestUnderRealLoad()
    {
        var monitor = new SystemLoadMonitor
        {
            SamplingIntervalSeconds = 0.5f,
            MaxReadingsHistory = 20
        };

        // Создаем нагрузку
        var stopwatch = Stopwatch.StartNew();
        var loadThread = new Thread(() => {
            while (stopwatch.Elapsed.TotalSeconds < 5)
            {
                // Создаем CPU нагрузку
                double result = 0;
                for (int i = 0; i < 1000000; i++)
                {
                    result += Math.Sqrt(i);
                }

                // Создаем RAM нагрузку
                byte[] buffer = new byte[1024 * 1024]; // 1MB
                Thread.Sleep(100);
            }
        });

        loadThread.Start();

        // Мониторим в течение 5 секунд
        int warnings = 0;
        while (stopwatch.Elapsed.TotalSeconds < 5)
        {
            string report = monitor.GetSystemLoadReport();
            if (report.Contains("Высокая нагрузка") || report.Contains("КРИТИЧЕСКАЯ"))
                warnings++;

            Console.WriteLine(report);
            Thread.Sleep(500);
        }

        loadThread.Join();

        Assert.IsTrue(warnings > 0, "Должен был быть хотя бы один warning при нагрузке");
        Assert.IsTrue(monitor.MaxObservedCpuUsage > 10, "CPU нагрузка должна быть больше 10%");
        Assert.IsTrue(monitor.MaxObservedRamUsageMB > 50, "RAM использование должно быть больше 50MB");
    }
}
