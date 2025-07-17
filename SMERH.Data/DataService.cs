using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SMERH.Data
{
    public class DataService
    {

        // Основной метод: возвращает всех потомков указанного PID
        public static List<(int Pid, string Name)> GetAllDescendantProcesses(int rootPid)
        {
            var descendants = new List<(int, string)>();
            var known = new HashSet<int>();        // уже добавленные
            var toCheck = new Queue<int>();        // очередь на обработку

            toCheck.Enqueue(rootPid);              // начинаем с корня

            // Сначала соберём список всех процессов и их родителей
            var allProcesses = new List<(int pid, int parentPid, string name)>();
            foreach (var proc in Process.GetProcesses())
            {
                try
                {
                    allProcesses.Add((proc.Id, GetParentPid(proc.Id), proc.ProcessName));
                }
                catch { }
            }

            while (toCheck.Count > 0)
            {
                int currentPid = toCheck.Dequeue();

                foreach (var proc in allProcesses)
                {
                    if (!known.Contains(proc.pid) &&
                        (proc.parentPid == currentPid || known.Contains(proc.parentPid)))
                    {
                        known.Add(proc.pid);
                        descendants.Add((proc.pid, proc.name));
                        toCheck.Enqueue(proc.pid); // проверить его детей
                    }
                }
            }

            return descendants;
        }

        // Получает родительский PID через WinAPI
        private static int GetParentPid(int pid)
        {
            PROCESS_BASIC_INFORMATION pbi = new PROCESS_BASIC_INFORMATION();

            int returnLength = 0;

            IntPtr handle = OpenProcess(ProcessAccessFlags.QueryInformation, false, pid);
            if (handle == IntPtr.Zero)
                return -1;

            int status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), ref returnLength);
            CloseHandle(handle);

            return (status == 0) ? pbi.InheritedFromUniqueProcessId.ToInt32() : -1;
        }

        // ---------- WinAPI ----------
        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr Reserved1;
            public IntPtr PebBaseAddress;
            public IntPtr Reserved2_0;
            public IntPtr Reserved2_1;
            public IntPtr UniqueProcessId;
            public IntPtr InheritedFromUniqueProcessId;
        }

        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(
            IntPtr processHandle,
            int processInformationClass,
            ref PROCESS_BASIC_INFORMATION processInformation,
            int processInformationLength,
            ref int returnLength);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(
            ProcessAccessFlags dwDesiredAccess,
            bool bInheritHandle,
            int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        [Flags]
        private enum ProcessAccessFlags : uint
        {
            QueryInformation = 0x400
        }
    }

}
