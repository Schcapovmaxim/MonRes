using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResourceMonitorLib;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ResourceMonitorTests
{
    [TestClass]
    public class ResourceMonitorTests
    {
        [TestMethod]
        public async Task CheckSystemAnomalies_UnderLoad_ReturnsAnomalies()
        {
            // Arrange
            var monitor = new ResourceMonitor(80, 80);

            // Act
            var (cpuAnomaly, memoryAnomaly) = monitor.CheckSystemAnomalies();

            // Assert
            Assert.IsFalse(cpuAnomaly, "CPU anomaly detected");
            Assert.IsFalse(memoryAnomaly, "Memory anomaly detected");
        }

        [TestMethod]
        public void CheckForMemoryLeak_WithFileAccess_ReturnsFalse()
        {
            // Arrange
            string testFile = @"C:\Users\Пользователь\Desktop\Github.txt";
            var monitor = new ResourceMonitor(80, 80);

            // Start process
            var process = Process.Start("notepad.exe", testFile);
            Thread.Sleep(2000);

            try
            {
                // Act
                bool leakDetected = monitor.CheckForMemoryLeak("notepad");

                // Assert
                Assert.IsFalse(leakDetected, "Memory leak detected");
            }
            finally
            {
                process?.Kill();
            }
        }

        [TestMethod]
        public void CheckForMemoryLeak_WithRealLeak_ReturnsTrue()
        {
            // Arrange
            var monitor = new ResourceMonitor(80, 80);
            var leakProcess = CreateMemoryLeakProcess();
            Thread.Sleep(3000);

            try
            {
                // Act
                bool leakDetected = monitor.CheckForMemoryLeak("MemoryLeaker");

                // Assert
                Assert.IsTrue(leakDetected, "Memory leak not detected");
            }
            finally
            {
                leakProcess?.Kill();
            }
        }

        private Process CreateMemoryLeakProcess()
        {
            string code = @"
using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static List<byte[]> _leak = new List<byte[]>();
    
    static void Main()
    {
        Console.WriteLine(""MemoryLeaker started"");
        while(true)
        {
            _leak.Add(new byte[1024 * 1024]); // 1MB
            Thread.Sleep(100);
        }
    }
}";
            File.WriteAllText("MemoryLeaker.cs", code);

            // Компиляция для текущей платформы
            string compiler = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "dotnet.exe"
                : "dotnet";

            var compileProcess = Process.Start(compiler, "build MemoryLeaker.cs -o output");
            compileProcess.WaitForExit();

            return Process.Start(Path.Combine("output", "MemoryLeaker.exe"));
        }
    }
}