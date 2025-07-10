using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMERH.Core;
using System;
using System.Diagnostics;
using System.Threading;
namespace VovaResourceMonitorTest
{
    [TestClass]
    public class ResourceMonitorTests
    {
            private TestableResourceMonitor _monitor;

            // Тестовый класс для доступа к protected методам
            private class TestableResourceMonitor : ResourceMonitor
            {
                public TestableResourceMonitor(string processName = null) : base(processName) { }

                public bool PublicCheckForAnomaly(List<float> readings, float threshold, float anomalyFactor)
                    => CheckForAnomaly(readings, threshold, anomalyFactor);

                public bool PublicCheckForMemoryLeak(List<float> ramReadings)
                    => CheckForMemoryLeak(ramReadings);
            }

            [TestInitialize]
            public void TestInitialize()
            {
                _monitor = new TestableResourceMonitor();
            }

            [TestMethod]
            public void CheckForAnomaly_DetectsHighCpuUsage()
            {
                // Arrange
                var readings = new List<float> { 10, 12, 15, 18, 20, 75 };

                // Act
                bool hasAnomaly = _monitor.PublicCheckForAnomaly(readings, 50, 2);

                // Assert
                Assert.IsTrue(hasAnomaly);
            }

            [TestMethod]
            public void CheckForMemoryLeak_DetectsIncreasingTrend()
            {
                // Arrange
                var readings = new List<float> { 100, 120, 150, 180, 220, 270, 330 };

                // Act
                bool hasLeak = _monitor.PublicCheckForMemoryLeak(readings);

                // Assert
                Assert.IsTrue(hasLeak);
            }

            // Остальные тесты остаются без изменений
            [TestMethod]
            public void GetCurrentMetrics_ReturnsValidValues()
            {
                var result = _monitor.GetCurrentMetrics();
                Assert.IsTrue(result.CpuUsage >= 0 && result.CpuUsage <= 100);
                Assert.IsTrue(result.RamUsage > 0);
            }
        }
    }
