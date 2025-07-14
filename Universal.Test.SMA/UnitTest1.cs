using System.Diagnostics;

namespace Universal.Test.SMA
{
    [TestClass]
    public class UnitTest1
    {
        // ���� � ��������� ����� (����� �������� �� ���)
        private static readonly string TestFilePath = @"C:\Users\Haier\Desktop\txt\17.txt";
        private static Process _startedProcess; // ���������� ��� �������� ��������

        // ���� ����� ����� ����������� ����� ����� �������
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            // ���������, ���������� �� ����
            if (!File.Exists(TestFilePath))
            {
                // ������ ����, ���� ��� ���
                File.WriteAllText(TestFilePath, "��� �������� ����, ��������� �������������.");
                Console.WriteLine($"�������� ���� ������: {TestFilePath}");
            }
            else
            {
                Console.WriteLine($"������������ ������������ �������� ����: {TestFilePath}");
            }
            try
            {
                _startedProcess = Process.Start(new ProcessStartInfo(TestFilePath) { UseShellExecute = true });
                Console.WriteLine($"���� �������: {TestFilePath}");
                bool isRunning = !_startedProcess.HasExited;
                if (isRunning)
                {
                    Console.WriteLine($"������� {_startedProcess.Id} ��������");
                }
                else { Console.WriteLine($"������� �� ��������"); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ ��� ������� �����: {ex.Message}");
            }
        }
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}