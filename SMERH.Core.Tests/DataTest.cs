using SMERH.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using System;
using System.Threading;
namespace SMERH.Core.Tests
{
    [TestClass]
    public class CoreServiceTests
    {
        private static readonly string TestKeyPath =
            @"Software\SMERH_Test_" + Guid.NewGuid().ToString("N");

        private RegistryKey _testKey;
        private CoreService _coreService;

        [TestInitialize]
        public void TestInitialize()
        {
            // Явно создаем уникальный тестовый ключ
            _testKey = Registry.CurrentUser.CreateSubKey(TestKeyPath);

            // Проверяем, что ключ действительно создан
            if (_testKey == null)
            {
                throw new InvalidOperationException(
                    $"Не удалось создать тестовый ключ: {TestKeyPath}");
            }

            _coreService = new CoreService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _coreService?.Dispose();

            try
            {
                // Удаляем только наш тестовый ключ
                if (_testKey != null)
                {
                    _testKey.Close();
                    Registry.CurrentUser.DeleteSubKeyTree(
                        TestKeyPath.Split('\\').Last(),
                        throwOnMissingSubKey: false);
                }
            }
            catch (Exception ex)
            {
                // Логируем, но не прерываем
                Console.WriteLine($"Ошибка очистки: {ex.Message}");
            }
        }

        [TestMethod]
        public void ShouldDetectRegistryChanges()
        {
            // Arrange
            var eventSignal = new ManualResetEventSlim(false);
            string changedPath = null;

            _coreService.RegistryChanged += (s, e) =>
            {
                changedPath = e.KeyPath;
                eventSignal.Set();
            };

            // Act
            _coreService.AddKeyToMonitor(_testKey);
            _coreService.StartMonitoring();

            // Даем время на инициализацию
            Thread.Sleep(150);

            // Вносим изменение
            _testKey.SetValue("TestValue", "Data", RegistryValueKind.String);

            // Assert
            Assert.IsTrue(
                eventSignal.Wait(TimeSpan.FromSeconds(3)),
                "Событие не получено за 3 секунды");

            Assert.AreEqual(
                _testKey.Name.ToUpperInvariant(),
                changedPath?.ToUpperInvariant(),
                $"Пути не совпадают. Ожидалось: {_testKey.Name}, Получено: {changedPath}");
        }
    }
}

