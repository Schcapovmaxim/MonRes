/*
 }
        /*public static class ProcessExtensions
        {
            public static IList<Process> GetChildProcesses(this Process process)
            {
                var children = new List<Process>();
                var mos = new ManagementObjectSearcher(String.Format($"Select * From Win32_Process Where ParentProcessID={process.Id}"));

                foreach (ManagementObject mo in mos.Get())
                    children.Add(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));

                return children;
            }
        }

      

        

        private void ShowNetworkInfo()
        {
            _monitorTextBox.AppendText("=== Сетевая активность процесса ===\r\n");

            try
            {
                // Это упрощенный пример - в реальности используйте netstat или WMI
                _monitorTextBox.AppendText($"Используемые порты:\n");

                // Пример для TCP (требует прав администратора)
                var connections = IPGlobalProperties.GetIPGlobalProperties()
                    .GetActiveTcpConnections()
                    .Where(c => c.LocalEndPoint.Port == _trackedProcess.Id); // Это не совсем корректно, нужен другой способ

                foreach (var conn in connections.Take(5))
                {
                    _monitorTextBox.AppendText($"{conn.LocalEndPoint} -> {conn.RemoteEndPoint} ({conn.State})\n");
                }
            }
            catch (Exception ex)
            {
                _monitorTextBox.AppendText($"Ошибка: {ex.Message}\r\n");
            }
        }

        

    

    
    
    }
}*/
using System; // Стандартное пространство имён и функции .NET(Console,String,Array,Math),обработка исключений
using System.Collections.Generic; // Работа с коллекциями(список,очередь,стек,cловарь)
using System.ComponentModel; // Улучшение дизайна форм
using System.Data; // Работа с базами данных
using System.Diagnostics; // Запуск и управление процессами и сбор метрик(ram,cpu)
using System.Drawing; // Работа с пользовательским интерфейсом(color,font,point,size)
using System.IO; // Чтение/запись данных на диск и работа с файловами потоками
using System.Linq; // Удобные запросы к данным как на sql и фильтрация tcp соединений
using System.Net.NetworkInformation; // Сетевая информация (проверка открытых портов и данные о подключениях)
using System.Text; // Работа с текстом и кодировками
using System.Threading.Tasks; // Ассинхронное программирование
using System.Windows.Forms; // Основное контролы(form,button,Textbox,Timer) и управление "жизнью" интерфейса
using MetroFramework.Forms; // Стилизация интерфейса с красивыми плоскими формами
using Microsoft.VisualBasic.Devices; // Вспомогатлеьные классы данные о системе (оперативная память)
using System.Management; // Запросы к системной информации (процессы, диски)
using SMERH.Core;
using SMERH.Data;

namespace SMERH // Пространство имен служащее для логической группировки связанных классов всего приложения в данном случае
{
    public partial class MainWindow_SMA : MetroForm // Объявление класса, окно приложения
    {
        private Process _trackedProcess; // Хранит ссылку на процесс за которым ведётся мониторинг
        private PerformanceCounter _cpuCounter; // Собирает данные о загрузке CPU в %
        private PerformanceCounter _ramCounter; // Собирает использование оперативной памяти в байтах
        private PerformanceCounter _ioReadCounter; // Отслеживает скрость чтения с диска (байт/сек)
        private PerformanceCounter _ioWriteCounter; // Отслеживает скрость записи на диск (байт/сек)
        private Timer _monitorTimer; // Таймер для переодического обновления метрик
        private Timer _stopMonitoringTimer; // Таймер для автоматической остановки мониторинга через заданное время
        private int _remainingSeconds; // Счетчик секунд 

        public MainWindow_SMA() // Конструктор класса для инциализации начального состояния
        {
            InitializeComponent(); // Инициализирует все компоненты формы

            _monitorTimer = new Timer {Interval = (int)numericUpDownInterval_SMA.Value }; // Создаёт переменную таймер с заданным интервалом 
            _monitorTimer.Tick += (s, e) => UpdateMonitoring(); // Подписывает на событие Tick таймера лямбда-выражение, которое вызывает метод UpdateMonitoring()

            _stopMonitoringTimer = new Timer { Interval = 1000 }; // Второй таймер с интервалом 1 секунда для обратного отсчёта
            _stopMonitoringTimer.Tick += StopMonitoringTimer_Tick; // Подписывает на событие Tick метод StopMonitoringTimer_Tick который уменьшает таймера и останавливает при достижении нуля
        }

       

        

        private void StopMonitoringTimer_Tick(object sender, EventArgs e) // Метод отвечает за отсчёт времени и остановку мониторинга по истечении таймера
        {
            _remainingSeconds--; // Уменьшаем значение таймера на 1 каждую секунду
            TimeRemainingLabel_SMA.Text = $"Осталось: {_remainingSeconds} сек"; // Уведомляем пользователя о состоянии таймера

            if (_remainingSeconds <= 0) // Если время истекло
            {
                _stopMonitoringTimer.Stop(); // Останавливаем таймер
                _monitorTimer.Stop(); // Останавливаем таймер переодичности сбора данных
                TimeRemainingLabel_SMA.Text = "Мониторинг остановлен по таймеру"; // Уведомляем пользовтеля об остановке мониторинга
                ButtonStartTimer_SMA.Enabled = true; // Активируем кнопку для запуска таймера
                TimeRemainingLabel_SMA.ForeColor = Color.Red; // Меняем цвет на красный в конце мониторинга
                checkedListBoxfunction.Enabled = true; // Активация возможности выбрать мониторинг 
                MonitoringDurationNumeric_SMA.Enabled = true; // Активация возможности менять таймер
                numericUpDownInterval_SMA.Enabled = true; // Активация возможности менять интервал
                buttonChoiceFile.Enabled = true; // Активация выбора файла
                MessageBox.Show("Мониторинг остановлен по истечении заданного времени", "Таймер",
                    MessageBoxButtons.OK, MessageBoxIcon.Information); // Уведомляем о завершении мониторинга
            }
        }

        private bool StartProcessCheck() // Метод проверяющий запущен ли процесс
        {
            if (_trackedProcess == null || _trackedProcess.HasExited) // Проверка на запущенный процесс
            {

                OutPutTextBox_BVP.Text = "Процесс не запущен или завершен";
                return false; // Вывод соответвующего сообщения
            }
            return true;
        }
        private bool ChoiceMonCheck() // Метод проверяющий сделан ли выбор типа мониторинга
        {
            if (checkedListBoxfunction.CheckedItems.Count == 0) // Проверка выбран ли тип мониторинга
            {
                OutPutTextBox_BVP.Text = "Выберите параметры для мониторинга";
                return false; // Вывод сообщения об оишбке
            }
            return true;
        }

        private void UpdateMonitoring() // Метод проверяющий выбраны ли чекбоксы и обновляющий данные
        {
            OutPutTextBox_BVP.Clear(); // Очищаем текстовое поле для вывода новой информаци

            if (!StartProcessCheck()) // Вызываем метод, проверяющий запущен ли процеес 
            {
                return; // Останавливаем метод UpdateMonitoring(), если не запущен процесс
            }

            if (!ChoiceMonCheck()) // Вызываем метод, проверяющий сделан ли выбор мониторинга 
            {
                return; // Останавливаем метод UpdateMonitoring(), если чекбоксы не стоят  
            }

            foreach (var item in checkedListBoxfunction.CheckedItems) // Запускаем цикл по всем чекбоксам
            {
                switch (item.ToString()) // Принимаем значения чекбоксов
                {
                    case "Сетевая активность":
                        ShowNetworkInfo(); // Мониторинг сетевой активности
                        break;
                    case "Файловая активность": 
                        ShowFileActivity(); // Мониторинг файловой активности
                        break;
                    case "Процессы":
                        ShowProcessesInfo(); // Мониторинг id,время запуска, потоки, дискрипторы
                        break;
                    case "Нагрузка":
                        ShowSystemLoad(); // Мониторинг RAM и CPU
                        break;
                }
                OutPutTextBox_BVP.AppendText(Environment.NewLine); // Вывод результата
            }
        }

        private void ShowNetworkInfo() // Метод для вывода сетевой активности
        {
            OutPutTextBox_BVP.AppendText("=== Сетевая активность процесса ===\r\n"); // Заголовок для разделения логов

            try
            {
                OutPutTextBox_BVP.AppendText($"Используемые порты:\n"); // Заголовок для используемых портов

                var connections = IPGlobalProperties.GetIPGlobalProperties()
                   .GetActiveTcpConnections()
                   .Where(c => c.LocalEndPoint.Port == _trackedProcess.Id); // Получение портов по процессу

                foreach (var conn in connections.Take(5))
                {
                    OutPutTextBox_BVP.AppendText($"{conn.LocalEndPoint} -> {conn.RemoteEndPoint} ({conn.State})\n"); // Вывод портов
                }
            }
            catch (Exception ex)
            {
                OutPutTextBox_BVP.AppendText($"Ошибка: {ex.Message}\r\n"); // Обработка ошибок
            }
        }

        private void ShowFileActivity() // Метод для вывода файловой активности
        {
            OutPutTextBox_BVP.AppendText("=== Файловая активность процесса ===\r\n"); // Заголовок для разделения логов

            try
            {
                float readSpeed = _ioReadCounter.NextValue() / 1024; // Скорость чтения
                float writeSpeed = _ioWriteCounter.NextValue() / 1024;// Скорость записи

                OutPutTextBox_BVP.AppendText($"Скорость чтения: {readSpeed:F1} KB/s\n");  // Вывод скорости чтения
                OutPutTextBox_BVP.AppendText($"Скорость записи: {writeSpeed:F1} KB/s\n\n"); // Вывод скорости записи

                OutPutTextBox_BVP.AppendText("Загруженные модули:\n"); // Заголовок для загруженных модулей
                foreach (ProcessModule module in _trackedProcess.Modules.Cast<ProcessModule>().Take(5)) // Получение модулей
                {
                    OutPutTextBox_BVP.AppendText($"{module.FileName}\n"); // Вывод модулей
                }
            }
            catch (Exception ex)
            {
                OutPutTextBox_BVP.AppendText($"Ошибка: {ex.Message}\r\n"); // Обработка ошибок
            }
        }

        private void ShowProcessesInfo() // Метод для вывода базовой информации о процессе
        {
            OutPutTextBox_BVP.AppendText("=== Информация о процессе ===\r\n"); // Заголовок для разделения логов

            try
            {
                OutPutTextBox_BVP.AppendText($"Имя: {_trackedProcess.ProcessName}\n"); // Имя
                OutPutTextBox_BVP.AppendText($"ID: {_trackedProcess.Id}\n"); // Id процесса
                OutPutTextBox_BVP.AppendText($"Время запуска: {_trackedProcess.StartTime}\n");  // Время запуска
                OutPutTextBox_BVP.AppendText($"Потоки: {_trackedProcess.Threads.Count}\n"); // Потоки
                OutPutTextBox_BVP.AppendText($"Дескрипторы: {_trackedProcess.HandleCount}\n"); // Дескрипторы
            }
            catch (Exception ex)
            {
                OutPutTextBox_BVP.AppendText($"Ошибка: {ex.Message}\r\n"); // Обработка ошибок
            }
        }

        private void ShowSystemLoad() // Метод для сбора логов связанных с нагрузкой процесса
        {
            OutPutTextBox_BVP.AppendText("=== Нагрузка процесса ===\r\n"); // Заголовок для разделения логов

            try
            {
                float cpuUsage = _cpuCounter.NextValue() / Environment.ProcessorCount; // Средняя нагрузка на ядро
                float ramUsageMB = _ramCounter.NextValue() / (1024 * 1024); // Использование оперативной памяти в байтах

                OutPutTextBox_BVP.AppendText($"CPU: {cpuUsage:F1}%\n"); // Вывод данных о CPU
                OutPutTextBox_BVP.AppendText($"RAM: {ramUsageMB:F1} MB\n"); // Вывод данных о RAM 
            }
            catch (Exception ex)
            {
                OutPutTextBox_BVP.AppendText($"Ошибка: {ex.Message}\r\n"); // Обработка ошибок
            }
        }

        private void buttonChoiceFile_Click(object sender, EventArgs e) // Метод для выбора файла для тестирования
        {
            if (openFileDialogTask.ShowDialog() == DialogResult.OK) // Открываем проводник
            {
                try
                {
                    _trackedProcess?.Close(); // При отсутствии ошибок закрываем процесс, если он был инициализирован
                    _stopMonitoringTimer.Stop(); // Останавливаем таймер
                    TimeRemainingLabel_SMA.Text = "Таймер не активирован"; // Выводим текст о состоянии таймера
                    TimeRemainingLabel_SMA.ForeColor = Color.Blue; // Установка цвета на голубой

                    _trackedProcess = Process.Start(openFileDialogTask.FileName); // Запускаем файл
                    InitializePerformanceCounters(); // Инициализируем системные счётчики
                    _monitorTimer.Start(); // Запускаем таймер мониторинга
                    buttonOpenDataMon_SMA.Enabled = true; // Активируем кнопку с следующей формой
                }
                catch (Exception ex) // Обработка ошибки в случае не открытия файла
                {
                    MessageBox.Show($"Не удалось открыть файл: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error); // Вывод сообщения об ошибке
                }
            }
        }

        private void InitializePerformanceCounters() // Метод инициализирующий системные счетчики производительности для мониторинга процесса
        {
            if (_trackedProcess == null || _trackedProcess.HasExited) return; // Проверяет существует ли отслеживаемый процесс и не завершился ли он

            _cpuCounter = new PerformanceCounter("Process", "% Processor Time",
                _trackedProcess.ProcessName, true);
            _cpuCounter.NextValue(); // Показывает какой процент времени CPU занят процессом и в первый раз возвращает 0

            _ramCounter = new PerformanceCounter("Process", "Working Set",
                _trackedProcess.ProcessName, true); // Объём оперативной памяти выделенной процессу

            _ioReadCounter = new PerformanceCounter("Process", "IO Read Bytes/sec",
                _trackedProcess.ProcessName, true); // Дисковая скорость чтения в сек
            _ioWriteCounter = new PerformanceCounter("Process", "IO Write Bytes/sec",
                _trackedProcess.ProcessName, true); // Дисковая скорость записи в сек
        }

        private void UpdateStatus(string message) // Метод, который обеспечивает потоковобезопасное обновление текста
        {
            if (lblStatus.InvokeRequired) // Определяет был ли вызван метод из потока отличного от того, где создан эелемент
            {
                lblStatus.Invoke(new Action(() => lblStatus.Text = message)); // Ставим выражение в очередь для выполнения в правильном потоке
            }
            else
            {
                lblStatus.Text = message; // После выполнения напрямую выводим текст без затрат 
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e) // Метод, который срабатывает при закрытии формы
        {
            _monitorTimer?.Stop(); // Остановка таймера обновления данных
            _stopMonitoringTimer?.Stop(); // Остановка таймера обратного отсчёта
            _cpuCounter?.Dispose(); // Освобождение ресурсов взятых счётчкиом CPU
            _ramCounter?.Dispose(); // Освобождение ресурсов взятых счётчкиом RAM
            _ioReadCounter?.Dispose(); // Освобождает счётчик чтения с диска
            _ioWriteCounter?.Dispose(); // Освобождает счётчик записи на диск
            _trackedProcess?.Close(); // Завершает отслеживаемый процесс, если он был запущен
            base.OnFormClosing(e); // Базовое закрытие Метроформ
        }

      

       
        private void ButtonStartTimer_SMA_Click(object sender, EventArgs e) // Метод нажатия на кнопку запуска таймера
        {
            

            ButtonStartTimer_SMA.Enabled = false; // ликвидация визуального бага и лагов
            if (!StartProcessCheck()) // Вызываем метод, проверяющий запущен ли процеес 
            {
                return; // Останавливаем метод ButtonStartTimer_SMA_Click(), если не запущен процесс
            }

            if (!ChoiceMonCheck()) // Вызываем метод, проверяющий сделан ли выбор мониторинга 
            {
                return; // Останавливаем метод ButtonStartTimer_SMA_Click(), если чекбоксы не стоят  
            }
            checkedListBoxfunction.Enabled = false; // Диактивация возможности выбрать мониторинг
            MonitoringDurationNumeric_SMA.Enabled = false; // Диактивация возможности менять таймер
            numericUpDownInterval_SMA.Enabled = false; // Диактивация возможности менять интервал
            buttonChoiceFile.Enabled = false; // Диактивация выбора файла
            UpdateMonitoring(); // Обновляем мониторинг


            _remainingSeconds = (int)MonitoringDurationNumeric_SMA.Value; // Автоматическая установка таймера мониторинга
            _stopMonitoringTimer.Start(); // Запуска таймера
            TimeRemainingLabel_SMA.Text = $"Осталось: {_remainingSeconds} сек"; // Динамический вывод оставшихся данных
            TimeRemainingLabel_SMA.ForeColor = Color.Green; // Изменение цвета надписи на зелеый

            MessageBox.Show($"Таймер мониторинга установлен на {_remainingSeconds} секунд", "Таймер",
            MessageBoxButtons.OK, MessageBoxIcon.Information); // Уведомление об успешной установке таймера
        
        }

        private void EmergencyStopButton_SMA_Click(object sender, EventArgs e) // Экстренная остановка
        {
            EmergencyStopButton_SMA.Enabled = false;
            EmergencyStopButton_SMA.Enabled = true; // ликвидация визуального бага
            var result = MessageBox.Show("Вы уверены, что хотите экстренно остановить мониторинг?",
                               "Подтверждение",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question); // Профилактика случайного нажатия

            if (result == DialogResult.Yes) // Если да
            {
                

                try
                {
                    checkedListBoxfunction.Enabled = true; // Активация возможности выбрать мониторинг 
                    MonitoringDurationNumeric_SMA.Enabled = true; // Активация возможности менять таймер
                    numericUpDownInterval_SMA.Enabled = true; // Активация возможности менять интервал
                    buttonChoiceFile.Enabled = true; // Активация выбора файла
                    // 1. Остановка всех таймеров
                    _monitorTimer?.Stop();
                    _stopMonitoringTimer?.Stop();

                    // 2. Сброс состояния интерфейса
                    TimeRemainingLabel_SMA.Text = "МОНИТОРИНГ ПРИОСТАНОВЛЕН";
                    TimeRemainingLabel_SMA.ForeColor = Color.Red;

                    // 3. Очистка вывода
                    OutPutTextBox_BVP.Clear();

                    // 4. Уведомление пользователя
                    MessageBox.Show("Все операции аварийно остановлены!", "Экстренная остановка",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);


                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при остановке: {ex.Message}", "Ошибка",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error); // Обработка ошибок
                }
            }
        }
    }
}