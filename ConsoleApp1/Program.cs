using System; // Стандартная библиотека C#: работа с базовыми типами, консолью и т.д.
using System.Collections.Generic; // Используется для HashSet — коллекции уникальных значений
using System.Diagnostics; // Позволяет работать с процессами (Process)
using System.Runtime.InteropServices; // Для взаимодействия с WinAPI
using System.Timers; // Таймер, вызывающий функцию по времени
using SMERH.Data;
class Program
{
    static void Main(int[] args) 
    {
        DataService cs = new DataService();
        int pid = int.Parse( Console.ReadLine());


        Console.WriteLine(cs.GetAllDescendantProcesses(pid));

    }

}
