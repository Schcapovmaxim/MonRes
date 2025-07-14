using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMERH.Data;
using System.Management; // Для WMI
namespace SMERH.Core
{
    public class CoreService  //SMERH.Core
    {
        static void Main(string[] args)
        {
            // Создаем запрос WMI, отслеживающий запуск новых процессов
            // "SELECT * FROM __InstanceCreationEvent" означает: событие создания экземпляра
            // "WITHIN 1" — проверять каждую секунду
            // "TargetInstance ISA 'Win32_Process'" — нас интересуют процессы
            string query = "SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'";

            // Подключаемся к WMI-объектам на локальной машине
            ManagementEventWatcher watcher = new ManagementEventWatcher(new WqlEventQuery(query));

            Console.WriteLine("⏳ Мониторинг дочерних процессов начат...");

            // Устанавливаем обработчик события
            watcher.EventArrived += new EventArrivedEventHandler(ProcessStarted);

            // Запускаем слежение
            watcher.Start();

            // Ждём нажатия клавиши, чтобы завершить
            Console.ReadLine();

            // Останавливаем отслеживание
            watcher.Stop();
        }

        // Обработчик событий запуска процесса
        private static void ProcessStarted(object sender, EventArrivedEventArgs e)
        {
            // Получаем информацию о процессе из события
            ManagementBaseObject process = (ManagementBaseObject)e.NewEvent["TargetInstance"];

            // Извлекаем PID, имя и родительский PID
            string name = process["Name"]?.ToString();
            string pid = process["ProcessId"]?.ToString();
            string parentPid = process["ParentProcessId"]?.ToString();

            Console.WriteLine($"🚀 Новый процесс: {name} (PID: {pid}), родитель: {parentPid}");
        }
    }
}