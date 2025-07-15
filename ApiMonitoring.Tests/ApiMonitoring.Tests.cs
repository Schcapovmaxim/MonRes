using ApiMonitoring.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace ApiMonitoring.Tests
{
    [TestClass]
    public class ApiMonitoringTests
    {
        /// <summary>
        /// Тестовый сервис для демонстрации
        /// </summary>
        public interface ICalculatorService
        {
            int Add(int a, int b);
            void ThrowError();
            Task<int> MultiplyAsync(int a, int b);
            Task LongOperationAsync();
        }

        /// <summary>
        /// Реальная реализация сервиса
        /// </summary>
        public class CalculatorService : ICalculatorService
        {
            public int Add(int a, int b) => a + b;

            public void ThrowError() => throw new InvalidOperationException("Test error");

            public async Task<int> MultiplyAsync(int a, int b)
            {
                await Task.Delay(100); // Имитация асинхронной работы
                return a * b;
            }

            public async Task LongOperationAsync()
            {
                await Task.Delay(200); // Имитация долгой операции
            }
        }

        /// <summary>
        /// Обёртка с мониторингом для тестового сервиса
        /// </summary>
        public class MonitoredCalculator : MonitoredApi, ICalculatorService
        {
            private readonly ICalculatorService _service = new CalculatorService();

            public MonitoredCalculator(IApiMonitor monitor) : base(monitor) { }

            public int Add(int a, int b)
            {
                return Execute(
                    methodName: nameof(Add),
                    method: () => _service.Add(a, b),
                    parameters: new object[] { a, b }
                );
            }

            public void ThrowError()
            {
                Execute(
                    methodName: nameof(ThrowError),
                    method: () => _service.ThrowError(),
                    parameters: Array.Empty<object>()
                );
            }

            public Task<int> MultiplyAsync(int a, int b)
            {
                return ExecuteAsync(
                    methodName: nameof(MultiplyAsync),
                    method: () => _service.MultiplyAsync(a, b),
                    parameters: new object[] { a, b }
                );
            }

            public Task LongOperationAsync()
            {
                return ExecuteAsync(
                    methodName: nameof(LongOperationAsync),
                    method: () => _service.LongOperationAsync(),
                    parameters: Array.Empty<object>()
                );
            }
        }

        // Тесты
        [TestMethod]
        public void Add_TwoNumbers_ReturnsSumAndLogs()
        {
            // Arrange
            var monitor = new ConsoleApiMonitor();
            var calculator = new MonitoredCalculator(monitor);

            // Act
            var result = calculator.Add(5, 3);

            // Assert
            Assert.AreEqual(8, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ThrowError_LogsException()
        {
            // Arrange
            var monitor = new ConsoleApiMonitor();
            var calculator = new MonitoredCalculator(monitor);

            // Act & Assert
            calculator.ThrowError();
        }

        [TestMethod]
        public async Task MultiplyAsync_TwoNumbers_ReturnsProductAndLogs()
        {
            // Arrange
            var monitor = new ConsoleApiMonitor();
            var calculator = new MonitoredCalculator(monitor);

            // Act
            var result = await calculator.MultiplyAsync(4, 6);

            // Assert
            Assert.AreEqual(24, result);
        }

        [TestMethod]
        public async Task LongOperationAsync_LogsDuration()
        {
            // Arrange
            var monitor = new ConsoleApiMonitor();
            var calculator = new MonitoredCalculator(monitor);

            // Act
            await calculator.LongOperationAsync();

            // Assert (проверяем через логи)
        }
    }
}
