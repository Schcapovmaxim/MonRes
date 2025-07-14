using System; // Стандартная библиотека C#: работа с базовыми типами, консолью и т.д.
using System.Collections.Generic; // Используется для HashSet — коллекции уникальных значений
using System.Diagnostics; // Позволяет работать с процессами (Process)
using System.Runtime.InteropServices; // Для взаимодействия с WinAPI
using System.Timers; // Таймер, вызывающий функцию по времени

class Program
{
    // PID процесса, чьи дочерние процессы будем отслеживать
    static int targetParentPid;

    // Множество уже замеченных дочерних PID — чтобы не выводить их повторно
    static HashSet<int> knownChildPids = new HashSet<int>();

    // Таймер, который будет запускать проверку каждую секунду
    static System.Timers.Timer timer;

    static void Main(string[] args)
    {
        // Запрашиваем у пользователя PID процесса, за которым хотим наблюдать
        Console.WriteLine("Введите PID процесса, за дочерними процессами которого вы хотите следить:");

        // Пробуем прочитать и преобразовать ввод в число (PID)
        if (!int.TryParse(Console.ReadLine(), out targetParentPid))
        {
            Console.WriteLine("Ошибка: неверный PID."); // Если введено не число — ошибка
            return;
        }

        // Проверяем, существует ли процесс с указанным PID
        if (!ProcessExists(targetParentPid))
        {
            Console.WriteLine($"Процесс с PID {targetParentPid} не найден."); // Если процесса нет — выходим
            return;
        }

        Console.WriteLine($"🔍 Отслеживаются дочерние процессы PID {targetParentPid}...");

        // Создаём таймер: он сработает каждые 1000 миллисекунд = 1 секунда
        timer = new System.Timers.Timer(1000);

        // Подписываемся на событие: при срабатывании таймера будет вызвана функция CheckChildProcesses
        timer.Elapsed += CheckChildProcesses;

        // Устанавливаем автоматический перезапуск таймера
        timer.AutoReset = true;

        // Запускаем таймер
        timer.Start();

        // Ожидаем нажатия клавиши Enter, чтобы завершить программу
        Console.WriteLine("Нажмите Enter для завершения.");
        Console.ReadLine();
    }

    // Метод проверяет, существует ли процесс с заданным PID
    static bool ProcessExists(int pid)
    {
        try
        {
            Process.GetProcessById(pid); // Если процесс найден — всё хорошо
            return true;
        }
        catch
        {
            return false; // Если исключение — процесс не существует
        }
    }

    // Получаем PID родительского процесса для заданного PID
    static int GetParentPid(int pid)
    {
        // Структура, в которую будет записана информация о процессе
        PROCESS_BASIC_INFORMATION pbi = new PROCESS_BASIC_INFORMATION();

        int returnLength = 0; // Переменная для получения длины возвращённой информации

        // Получаем дескриптор процесса, чтобы можно было читать его свойства
        IntPtr hProcess = OpenProcess(ProcessAccessFlags.QueryInformation, false, pid);

        if (hProcess == IntPtr.Zero)
            return -1; // Не удалось открыть процесс

        // Вызываем WinAPI NtQueryInformationProcess, чтобы получить структуру с родительским PID
        int status = NtQueryInformationProcess(hProcess, 0, ref pbi, Marshal.SizeOf(pbi), ref returnLength);

        // Закрываем дескриптор после использования
        CloseHandle(hProcess);

        // Если запрос успешен (status == 0), возвращаем родительский PID
        return (status == 0) ? pbi.InheritedFromUniqueProcessId.ToInt32() : -1;
    }

    // Метод, вызываемый таймером каждую секунду: проверяет, появились ли новые дочерние процессы
    static void CheckChildProcesses(object sender, ElapsedEventArgs e)
    {
        // Получаем список всех процессов, запущенных в системе
        Process[] allProcesses = Process.GetProcesses();

        foreach (var proc in allProcesses)
        {
            try
            {
                // Пропускаем процесс, если он уже был найден ранее
                if (knownChildPids.Contains(proc.Id))
                    continue;

                // Получаем PID родителя текущего процесса
                int parentPid = GetParentPid(proc.Id);

                // Если родитель совпадает с наблюдаемым PID — это наш дочерний процесс
                if (parentPid == targetParentPid)
                {
                    // Добавляем процесс в список известных, чтобы не повторяться
                    knownChildPids.Add(proc.Id);

                    // Выводим информацию о дочернем процессе
                    Console.WriteLine($"🧒 Дочерний процесс: {proc.ProcessName} (PID: {proc.Id})");
                }
            }
            catch
            {
                // Некоторые процессы могут быть недоступны (например, системные) — игнорируем ошибки
            }
        }
    }

    // -------- WinAPI: ниже идут определения, чтобы использовать функции Windows напрямую --------

    // Структура, в которую WinAPI запишет данные о процессе (в том числе — родительский PID)
    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_BASIC_INFORMATION
    {
        public IntPtr Reserved1;
        public IntPtr PebBaseAddress;
        public IntPtr Reserved2_0;
        public IntPtr Reserved2_1;
        public IntPtr UniqueProcessId;
        public IntPtr InheritedFromUniqueProcessId; // <-- Именно это поле содержит родительский PID
    }

    // Объявление функции NtQueryInformationProcess из библиотеки ntdll.dll
    [DllImport("ntdll.dll")]
    public static extern int NtQueryInformationProcess(
        IntPtr processHandle,
        int processInformationClass,
        ref PROCESS_BASIC_INFORMATION processInformation,
        int processInformationLength,
        ref int returnLength);

    // Объявление функции OpenProcess из kernel32.dll — открывает процесс по PID
    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(
        ProcessAccessFlags dwDesiredAccess,
        bool bInheritHandle,
        int dwProcessId);

    // Объявление функции CloseHandle — закрывает дескриптор
    [DllImport("kernel32.dll")]
    public static extern bool CloseHandle(IntPtr hObject);

    // Флаги доступа для OpenProcess — в нашем случае только QueryInformation (чтение свойств)
    [Flags]
    public enum ProcessAccessFlags : uint
    {
        QueryInformation = 0x400 // Получение базовой информации о процессе
    }
}
