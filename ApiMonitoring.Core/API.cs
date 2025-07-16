using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    /// Конфигурация для файлового мониторинга
    /// </summary>
    public class FileMonitorConfig
    {
        /// <summary>
        /// Базовый путь для сохранения логов
        /// </summary>
        public string LogDirectory { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ApiLogs");

        /// <summary>
        /// Количество дней хранения логов
        /// </summary>
        public int LogRetentionDays { get; set; } = 7;

        /// <summary>
        /// Формат имени файла логов
        /// </summary>
        public string FileNameFormat { get; set; } = "{service}_{date:yyyyMMdd}.log";
    }

    /// <summary>
    /// Реализация мониторинга с записью в файл
    /// </summary>
    public class FileApiMonitor : IApiMonitor, IDisposable
    {
        private readonly string _serviceName;
        private readonly FileMonitorConfig _config;
        private readonly object _fileLock = new object();
        private readonly System.Timers.Timer _cleanupTimer;

        public FileApiMonitor(string serviceName, FileMonitorConfig config = null)
        {
            _serviceName = serviceName;
            _config = config ?? new FileMonitorConfig();

            // Создаем директорию, если ее нет
            Directory.CreateDirectory(_config.LogDirectory);

            // Настраиваем регулярную очистку старых логов
            _cleanupTimer = new System.Timers.Timer(TimeSpan.FromDays(1).TotalMilliseconds);
            _cleanupTimer.Elapsed += (s, e) => CleanupOldLogs();
            _cleanupTimer.Start();
        }

        public void LogCall(string methodName, object[] parameters, object? result = null, TimeSpan? duration = null)
        {
            string args = string.Join(", ", parameters.Select(p => p?.ToString() ?? "null"));
            string message = $"[{DateTime.Now:HH:mm:ss.fff}] ВЫЗОВ {methodName}({args})";

            if (duration.HasValue)
                message += $" | Время: {duration.Value.TotalMilliseconds}мс";

            if (result != null)
                message += $"\n  ↳ РЕЗУЛЬТАТ: {result}";

            WriteToFile(message);
        }

        public void LogException(string methodName, Exception exception, TimeSpan? duration = null)
        {
            string message = $"[{DateTime.Now:HH:mm:ss.fff}] ОШИБКА в {methodName}: {exception.Message}";

            if (duration.HasValue)
                message += $" | Время: {duration.Value.TotalMilliseconds}мс";

            WriteToFile(message);
        }

        private void WriteToFile(string message)
        {
            lock (_fileLock)
            {
                string fileName = _config.FileNameFormat
                    .Replace("{service}", _serviceName)
                    .Replace("{date:yyyyMMdd}", DateTime.Now.ToString("yyyyMMdd"));

                string filePath = Path.Combine(_config.LogDirectory, fileName);
                File.AppendAllText(filePath, message + Environment.NewLine);
            }
        }

        private void CleanupOldLogs()
        {
            try
            {
                lock (_fileLock)
                {
                    var cutoff = DateTime.Now.AddDays(-_config.LogRetentionDays);
                    var files = Directory.GetFiles(_config.LogDirectory, $"{_serviceName}_*.log");

                    foreach (var file in files)
                    {
                        if (File.GetCreationTime(file) < cutoff)
                            File.Delete(file);
                    }
                }
            }
            catch
            {
                // Игнорируем ошибки при очистке
            }
        }

        public void Dispose()
        {
            _cleanupTimer?.Stop();
            _cleanupTimer?.Dispose();
        }
    }

    /// <summary>
    /// Базовый класс для API с мониторингом
    /// </summary>
    public abstract class MonitoredApi
    {
        protected readonly IApiMonitor Monitor;

        protected MonitoredApi(IApiMonitor monitor)
        {
            Monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
        }

        /// <summary>
        /// Выполняет метод с возвращаемым значением и мониторингом
        /// </summary>
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

        /// <summary>
        /// Выполняет метод без возвращаемого значения с мониторингом
        /// </summary>
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

        /// <summary>
        /// Выполняет асинхронный метод с возвращаемым значением и мониторингом
        /// </summary>
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

        /// <summary>
        /// Выполняет асинхронный метод без возвращаемого значения с мониторингом
        /// </summary>
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