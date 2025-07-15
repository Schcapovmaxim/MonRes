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
        public void Сложение_ДваЧисла_ВозвращаетСуммуИЛогирует()
        {
            // Конфигурация с пользовательским путем
            var config = new FileMonitorConfig
            {
                LogDirectory = Path.Combine(TestContext.TestRunDirectory, "ApiLogs")
            };

            // Arrange
            var monitor = new FileApiMonitor("Калькулятор", config);
            TestContext.WriteLine("Начало теста сложения...");
            var calculator = new ТестовыйКалькулятор(monitor);

            // Act
            var result = calculator.Сложить(5, 3);

            // Assert
            Assert.AreEqual(8, result);
            TestContext.WriteLine("Тест сложения завершен успешно!");

            // Проверяем создание файла логов
            string logFile = Path.Combine(config.LogDirectory, $"Калькулятор_{DateTime.Now:yyyyMMdd}.log");
            Assert.IsTrue(File.Exists(logFile), "Файл логов не создан");
            TestContext.WriteLine($"Логи сохранены в: {logFile}");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ОшибочныйМетод_ЛогируетИсключение()
        {
            // Arrange
            var monitor = new FileApiMonitor("Калькулятор");
            TestContext.WriteLine("Начало теста ошибки...");
            var calculator = new ТестовыйКалькулятор(monitor);

            // Act & Assert
            calculator.ВызватьОшибку();
        }

        // Вспомогательные классы на русском
        private interface IКалькулятор
        {
            int Сложить(int a, int b);
            void ВызватьОшибку();
        }

        private class ТестовыйКалькулятор : MonitoredApi, IКалькулятор
        {
            public ТестовыйКалькулятор(IApiMonitor monitor) : base(monitor) { }

            public int Сложить(int a, int b)
            {
                return Выполнить(
                    methodName: nameof(Сложить),
                    method: () => a + b,
                    parameters: new object[] { a, b }
                );
            }

            public void ВызватьОшибку()
            {
                Выполнить(
                    methodName: nameof(ВызватьОшибку),
                    method: () => throw new InvalidOperationException("Тестовая ошибка"),
                    parameters: Array.Empty<object>()
                );
            }
        }
    }
}