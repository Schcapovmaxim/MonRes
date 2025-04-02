using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            

            var adapters = NetworkInterface.GetAllNetworkInterfaces();
            Console.WriteLine($"Обнаружено {adapters.Length} устройств");
            foreach (NetworkInterface adapter in adapters)
            {
                Console.WriteLine("=====================================================================");
                Console.WriteLine();
                Console.WriteLine($"ID устройства: ------------- {adapter.Id}");
                Console.WriteLine($"Имя устройства: ------------ {adapter.Name}");
                Console.WriteLine($"Описание: ------------------ {adapter.Description}");
                Console.WriteLine($"Тип интерфейса: ------------ {adapter.NetworkInterfaceType}");
                Console.WriteLine($"Физический адрес: ---------- {adapter.GetPhysicalAddress()}");
                Console.WriteLine($"Статус: -------------------- {adapter.OperationalStatus}");
                Console.WriteLine($"Скорость: ------------------ {adapter.Speed}");

                IPInterfaceStatistics stats = adapter.GetIPStatistics();
                Console.WriteLine($"Получено: ----------------- {stats.BytesReceived}");
                Console.WriteLine($"Отправлено: --------------- {stats.BytesSent}");
            }
            Console.ReadKey();
        }
    }
}
