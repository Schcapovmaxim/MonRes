using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ApiMonitoring.Core
{
    /// <summary>
    /// Интерфейс для мониторинга API-вызовов
    /// </summary>
    public interface IApiMonitor
    {
        void LogCall(string methodName, object[] parameters, object? result = null, TimeSpan? duration = null);
        void LogException(string methodName, Exception exception, TimeSpan? duration = null);
    }

    /// <summary>
    /// Реализация монитора, выводящая информацию в консоль
    /// </summary>
    public class ConsoleApiMonitor : IApiMonitor
    {
        public void LogCall(string methodName, object[] parameters, object? result = null, TimeSpan? duration = null)
        {
            string args = string.Join(", ", parameters.Select(p => p?.ToString() ?? "null"));
            string output = $"[{DateTime.UtcNow:u}] Called: {methodName}({args})";

            if (duration.HasValue)
                output += $" [Duration: {duration.Value.TotalMilliseconds} ms]";

            if (result != null)
                output += $"\n\t↳ Result: {result}";

            Console.WriteLine(output);
        }

        public void LogException(string methodName, Exception exception, TimeSpan? duration = null)
        {
            string output = $"[{DateTime.UtcNow:u}] Error in {methodName}: {exception.Message}";

            if (duration.HasValue)
                output += $" [Duration: {duration.Value.TotalMilliseconds} ms]";

            Console.WriteLine(output);
        }
    }

    /// <summary>
    /// Базовый класс для отслеживаемых API сервисов
    /// </summary>
    public abstract class MonitoredApi
    {
        protected readonly IApiMonitor Monitor;

        protected MonitoredApi(IApiMonitor monitor)
        {
            Monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
        }

        protected T Execute<T>(string methodName, Func<T> method, params object[] parameters)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                T result = method();
                sw.Stop();
                Monitor.LogCall(methodName, parameters, result, sw.Elapsed);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                Monitor.LogException(methodName, ex, sw.Elapsed);
                throw;
            }
        }

        protected void Execute(string methodName, Action method, params object[] parameters)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                method();
                sw.Stop();
                Monitor.LogCall(methodName, parameters, duration: sw.Elapsed);
            }
            catch (Exception ex)
            {
                sw.Stop();
                Monitor.LogException(methodName, ex, sw.Elapsed);
                throw;
            }
        }

        protected async Task<T> ExecuteAsync<T>(string methodName, Func<Task<T>> method, params object[] parameters)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                T result = await method();
                sw.Stop();
                Monitor.LogCall(methodName, parameters, result, sw.Elapsed);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                Monitor.LogException(methodName, ex, sw.Elapsed);
                throw;
            }
        }

        protected async Task ExecuteAsync(string methodName, Func<Task> method, params object[] parameters)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                await method();
                sw.Stop();
                Monitor.LogCall(methodName, parameters, duration: sw.Elapsed);
            }
            catch (Exception ex)
            {
                sw.Stop();
                Monitor.LogException(methodName, ex, sw.Elapsed);
                throw;
            }
        }
    }
}