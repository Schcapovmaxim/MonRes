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
            // ���� ������� ���������� �������� ����
            _testKey = Registry.CurrentUser.CreateSubKey(TestKeyPath);

            // ���������, ��� ���� ������������� ������
            if (_testKey == null)
            {
                throw new InvalidOperationException(
                    $"�� ������� ������� �������� ����: {TestKeyPath}");
            }

            _coreService = new CoreService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _coreService?.Dispose();

            try
            {
                // ������� ������ ��� �������� ����
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
                // ��������, �� �� ���������
                Console.WriteLine($"������ �������: {ex.Message}");
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

            // ���� ����� �� �������������
            Thread.Sleep(150);

            // ������ ���������
            _testKey.SetValue("TestValue", "Data", RegistryValueKind.String);

            // Assert
            Assert.IsTrue(
                eventSignal.Wait(TimeSpan.FromSeconds(3)),
                "������� �� �������� �� 3 �������");

            Assert.AreEqual(
                _testKey.Name.ToUpperInvariant(),
                changedPath?.ToUpperInvariant(),
                $"���� �� ���������. ���������: {_testKey.Name}, ��������: {changedPath}");
        }
    }
}

