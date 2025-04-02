using System.Net.NetworkInformation;
namespace Control_Sum
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("              Контрольая сумма пакетов данных:          ");



            var ipProps = IPGlobalProperties.GetIPGlobalProperties();
            var ipStats = ipProps.GetIPv4GlobalStatistics();
            Console.WriteLine($"Входящие пакеты: {ipStats.ReceivedPackets}");
            Console.WriteLine($"Исходящие пакеты: {ipStats.OutputPacketRequests}");
        }
    }
}
