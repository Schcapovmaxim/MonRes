using System; // Стандартная библиотека C#: работа с базовыми типами, консолью и т.д.
using System.Collections.Generic; // Используется для HashSet — коллекции уникальных значений
using System.Diagnostics; // Позволяет работать с процессами (Process)
using System.Runtime.InteropServices; // Для взаимодействия с WinAPI
using SMERH.Data;

public class Program
{
    public static void Main(string[] args)
    {
        Console.Write("Введите PID процесса: ");
        int pid = int.Parse(Console.ReadLine());

        var processes = DataService.GetAllDescendantProcesses(pid);//вот здесь применяется метод йоу

        Console.WriteLine($"Найдено дочерних процессов: {processes.Count}");
        foreach (var (childPid, name) in processes)
        {
            Console.WriteLine($"🧒 {name} (PID: {childPid})");
        }
    }
}

