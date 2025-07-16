using ApiMonitoring.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ApiMonitoring.Tests
{
    [TestClass]
    public class ApiMonitoringTests
    {

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Add_TwoNumbers_ReturnsSumAndLogs()
        {
            // Конфигурация с пользовательским путем
            var config = new FileMonitorConfig
            {
                LogDirectory = Path.Combine(TestContext.TestRunDirectory, "ApiLogs")
            };

            // Arrange
            var monitor = new FileApiMonitor("Calculator", config);
            TestContext.WriteLine("Начало теста сложения...");
            var calculator = new CalculatorService(monitor);

            // Act
            var result = calculator.Add(5, 3);

            // Assert
            Assert.AreEqual(8, result);
            TestContext.WriteLine("Тест сложения завершен успешно!");

            // Проверяем создание файла логов
            string logFile = Path.Combine(config.LogDirectory, $"Calculator_{DateTime.Now:yyyyMMdd}.log");
            Assert.IsTrue(File.Exists(logFile), "Файл логов не создан");
            TestContext.WriteLine($"Логи сохранены в: {logFile}");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ErrorMethod_LogsException()
        {
            // Arrange
            var monitor = new FileApiMonitor("Calculator");
            TestContext.WriteLine("Начало теста ошибки...");
            var calculator = new CalculatorService(monitor);

            // Act & Assert
            calculator.ErrorMethod();
        }

        // Вспомогательные классы
        private interface ICalculator
        {
            int Add(int a, int b);
            void ErrorMethod();
        }

        private class CalculatorService : MonitoredApi, ICalculator
        {
            public CalculatorService(IApiMonitor monitor) : base(monitor) { }

            public int Add(int a, int b)
            {
                return Execute(
                    methodName: nameof(Add),
                    method: () => a + b,
                    parameters: new object[] { a, b }
                );
            }

            public void ErrorMethod()
            {
                Execute(
                    methodName: nameof(ErrorMethod),
                    method: () => throw new InvalidOperationException("Тестовая ошибка"),
                    parameters: Array.Empty<object>()
                );
            }
        }
    }
}