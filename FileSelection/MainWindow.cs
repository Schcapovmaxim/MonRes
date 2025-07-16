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
using SharpPcap;
using SharpPcap.LibPcap;
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
using FileSelection; // для использования других форм
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using PacketDotNet;
using System.Text.RegularExpressions;

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
        private OptionsCheckedListBoxForm optionsCheckedListBoxForm; // Хранит информацию о форме Checkbox
        private string[] selectedCheckBoxes; // Хранит информацию о выбранных чекбоксах у формы Checkbox
        private List<string> allowedParameters = new List<string> { // список параметров (ключей), которые требуется забрать из пакета (вместе со значениями)
            "SourceAddress",
            "DestinationAddress",
            "Protocol",
            "SourcePort",
            "DestinationPort"
        };
        private List<string> listOfIgnoredSourceAddress = new List<string> { }; // список игнорируемых ip отправителя

        private List<string> listOfIgnoredDestinationAddress = new List<string> { }; // список игнорируемых ip получателя

        private List<string> listOfIgnoredSourcePorts = new List<string> { }; // список игнорируемых портов отправителя

        private List<string> listOfIgnoredDestinationPorts = new List<string> { }; // список игнорируемых портов получателя

        private Dictionary<(string, string, string, string, string), bool> listOfAwaitedConnections = new Dictionary<(string, string, string, string, string), bool>(); // словарь ожидаемых подключений
        public MainWindow_SMA() // Конструктор класса для инциализации начального состояния
        {
            InitializeComponent(); // Инициализирует все компоненты формы
            getOptions(); // Загрузить настройки для мониторинга сети
            _monitorTimer = new Timer {Interval = (int)numericUpDownInterval_SMA.Value }; // Создаёт переменную таймер с заданным интервалом 
            _monitorTimer.Tick += (s, e) => UpdateMonitoring(); // Подписывает на событие Tick таймера лямбда-выражение, которое вызывает метод UpdateMonitoring()

            _stopMonitoringTimer = new Timer { Interval = 1000 }; // Второй таймер с интервалом 1 секунда для обратного отсчёта
            _stopMonitoringTimer.Tick += StopMonitoringTimer_Tick; // Подписывает на событие Tick метод StopMonitoringTimer_Tick который уменьшает таймера и останавливает при достижении нуля
        }

       

        

        private void getOptions() // Функция по загрузке настроек для мониторинга сети из файлов
        {
            string optionsNumber = ""; // хранит номер текущей конфигурации
            int optionsMaxNumber = 1; // хранит максимальный номер доступных настроек

            foreach (string line in File.ReadLines("cfg\\main.cfg"))
            {
                string[] options = line.Split(';'); // получение номера текущих настроек и какой максимальный доступный номер настроек соответственно
                optionsNumber = options[0];
                optionsMaxNumber = Int32.Parse(options[1]);

            }
            foreach (string line in File.ReadLines($"cfg\\{optionsNumber}\\ignoredSourceAddresses.cfg"))
            {
                listOfIgnoredSourceAddress.Add(line);
            }

            foreach (string line in File.ReadLines($"cfg\\{optionsNumber}\\ignoredDestinationAddress.cfg"))
            {
                listOfIgnoredDestinationAddress.Add(line);
            }

            foreach (string line in File.ReadLines($"cfg\\{optionsNumber}\\ignoredSourcePorts.cfg"))
            {
                listOfIgnoredSourcePorts.Add(line);
            }

            foreach (string line in File.ReadLines($"cfg\\{optionsNumber}\\ignoredDestinationPorts.cfg"))
            {
                listOfIgnoredDestinationPorts.Add(line);
            }

            foreach (string line in File.ReadLines($"cfg\\awaitedConnetions.cfg")) // получение ожидаемых подключений (адреса, порты, протокол)
            {
                string[] line_Splitted = line.Split(','); // разбиение строки на отдельные слова используя запятую как разделитель
                listOfAwaitedConnections.Add((line_Splitted[0], line_Splitted[1], line_Splitted[2], line_Splitted[3], line_Splitted[4]), true); // запись в словарь подключения
            }

            int optionsNumber_int = Int32.Parse(optionsNumber) + 1; // увеличение номера настроек для следующего запуска
            if (optionsNumber_int > optionsMaxNumber) // проверка если полученный номер оказался больше чем всего настроек доступно
            {
                optionsNumber_int = 1; // в таком случае будет начальный номер настроек
            }
            File.WriteAllText("cfg\\main.cfg", $"{optionsNumber_int};{optionsMaxNumber}"); // запись в файл номера настроек для следующего запуска и сколько всего доступно настроек соответственно
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
                metroButtonOptions_DIA.Enabled = true; // Активация возможности выбрать мониторинг 
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
            if (selectedCheckBoxes == null || selectedCheckBoxes.Length == 0) // Проверка выбран ли тип мониторинга
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

            if (ChoiceMonCheck() == false) // Вызываем метод, проверяющий сделан ли выбор мониторинга 
            {
                return; // Останавливаем метод UpdateMonitoring(), если чекбоксы не стоят  
            }

            foreach (var item in selectedCheckBoxes) // Запускаем цикл по всем чекбоксам
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


                Task.Run(() =>
                {
                    CaptureDeviceList devices = CaptureDeviceList.Instance; // получение сетевых устройств
                    foreach (ICaptureDevice device in devices) //проход по всем сетевым устройствам
                    {
                        device.OnPacketArrival += (sender, e) => // событие при появлении нового пакета
                        {
                            var raw = e.GetPacket(); // получение ссылки на информацию о пакете
                            var dataCopy = raw.Data.ToArray(); // сохранение информации о пакете
                            var packet = Packet.ParsePacket(raw.LinkLayerType, dataCopy); // преобразует информацию о пакете в читательный вид; первый параметр указывает как эту информацию "читать"
                            Task.Run(() => // Обработка пакета в отдельном потоке
                            {
                                long microSeconds = (long)(raw.Timeval.Seconds * 1_000_000L + raw.Timeval.MicroSeconds); // получение микросекунды в которую отслеживается пакет
                                TimeSpan time = TimeSpan.FromMilliseconds(microSeconds / 1000.0); // получение более понятной информацию о времени изучения пакета - день, месяц там, а не сколько миллисекунд с нулевой даты прошло
                                string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", time.Hours, time.Minutes, time.Seconds, time.Milliseconds); // запись времени как строки

                                string s = $"{formattedTime},{device.Description},"; // в этой переменной хранится строка, которая будет выводиться в поле вывода. Первый параметр - время, в которое пакет был обнаружен, второй параметр - с каким устройством связан этот пакет
                                int s_Length = s.Length; // запись длины s
                                Connection connection = new Connection(); // содержит информацию о соединении в более удобном виде

                                string packet_ToString = packet.ToString(); // превращение информации о пакете в строку
                                packet_ToString = packet_ToString.Replace("[", ""); // убирает [ из данных о пакете
                                packet_ToString = packet_ToString.Replace("]", ""); // убирает ] из данных о пакете
                                packet_ToString = packet_ToString.Replace(",", ""); // убирает , из данных о пакете

                                foreach (string pair in packet_ToString.Split(' ')) // получение пар ключ->значение
                                {
                                    string[] splittedPair = pair.Split('='); // разделение пары на ключ->значение
                                    if (splittedPair.Length == 2 && allowedParameters.Contains(splittedPair[0])) // проверяет, что можно получить ключ->значение и что ключ входит в список тех ключей которые требуются
                                    {
                                        switch (splittedPair[0])
                                        { // проверяет, что пара содержит полезные данные и соотетственная подстановка в connection если это так
                                            case "SourceAddress":
                                                connection.SourceAddress = splittedPair[1];
                                                break;
                                            case "DestinationAddress":
                                                connection.DestinationAddress = splittedPair[1];
                                                break;
                                            case "Protocol":
                                                connection.Protocol = splittedPair[1];
                                                break;
                                            case "SourcePort":
                                                connection.SourcePort = splittedPair[1];
                                                break;
                                            case "DestinationPort":
                                                connection.DestinationPort = splittedPair[1];
                                                break;
                                        }
                                        s += $"{splittedPair[0]}={splittedPair[1]},"; // отправляет в переменную новую пару ключ->значение
                                    }
                                }
                            

                                if (s.Length == s_Length) // проверка если у пакета нет обычной информации по типу адреса получателя
                                {
                                    s += "L2,"; // в таком случае указывается, что это L2 соединение
                                }

                                if (connection.SourceAddress != null) // проверка 
                                {
                                    if (listOfIgnoredSourceAddress.Contains(connection.SourceAddress) || // проверка если это соединение, ожидаемое не от проверяемого приложения по списку портов и адресов
                                    listOfIgnoredDestinationAddress.Contains(connection.DestinationAddress) ||
                                    connection.SourcePort != null && listOfIgnoredSourcePorts.Contains(connection.SourcePort) ||
                                    connection.DestinationPort != null && listOfIgnoredDestinationPorts.Contains(connection.DestinationPort)
                                    )
                                    {
                                        goto EX; // пропуск пакета
                                    }

                                    if (!listOfAwaitedConnections.ContainsKey((connection.SourceAddress, // проверка если это неожиданное соединение от проверяемого приложения
                                    connection.SourcePort,
                                    connection.DestinationAddress,
                                    connection.DestinationPort,
                                    connection.Protocol)))
                                    {
                                        s += $"Bytes={dataCopy.Length},!!!UKNOWN CONNECTION!!!"; // запись в переменную того, сколько байт было передано и что это неизвестное подключение
                                        AppendOutputSafe(s);
                                        if (!_trackedProcess.HasExited)
                                        {
                                            //_trackedProcess.Kill();
                                            //_trackedProcess.WaitForExit();
                                        }
                                        goto EX;
                                    }
                                }

                                s += $"Bytes={dataCopy.Length}"; // запись в переменную того, сколько байт было передано
                                AppendOutputSafe(s); // отправка строки в поле вывода
                            EX: // метка для пропуска пакета 
                                { }
                            });
                        };

                        device.Open(DeviceModes.Promiscuous, (int)numericUpDownInterval_SMA.Value); // Открывает устройство
                        device.StartCapture(); // Запускает захват
                    }
                });
            }
            catch (Exception ex)
            {
                OutPutTextBox_BVP.AppendText($"Ошибка: {ex.Message}\r\n"); // Обработка ошибок
            }
        }

        private void AppendOutputSafe(string text)
        {
            if (OutPutTextBox_BVP.InvokeRequired)
            {
                // Мы не в UI-потоке → вызываем через Invoke
                OutPutTextBox_BVP.Invoke(new Action(() =>
                {
                    OutPutTextBox_BVP.AppendText(text + "\r\n");
                }));
            }
            else
            {
                // Уже в UI-потоке → можно безопасно обращаться к контролу
                OutPutTextBox_BVP.AppendText(text + "\r\n");
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
            metroButtonOptions_DIA.Enabled = false; // Диактивация возможности выбрать мониторинг
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
                    metroButtonOptions_DIA.Enabled = true; // Активация возможности выбрать мониторинг 
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

                    // 5. Сделать кнопку "Старт таймера" снова активной
                    ButtonStartTimer_SMA.Enabled = true;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при остановке: {ex.Message}", "Ошибка",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error); // Обработка ошибок
                }
            }
        }

        private void switchEnabledOption(Control parent, bool enabled) // функция по переключению свойства Enabled у всего на форме или у объекта
        {
            foreach (Control ctrl in parent.Controls)
            {
                ctrl.Enabled = enabled; // переключение свойства Enabled

                if (ctrl.HasChildren) // если у объекта есть дочерние объекты
                {
                    switchEnabledOption(ctrl, enabled); // новый вызов, но для такого объекта
                }
            }
        }

        private void metroButtonOptions_DIA_Click(object sender, EventArgs e) // функция по открытию формы с чекбоксом
        {
            optionsCheckedListBoxForm = new OptionsCheckedListBoxForm(); // инициализация формы
            optionsCheckedListBoxForm.StartPosition = FormStartPosition.Manual; // указание, что место появления формы будет чётко указано
            optionsCheckedListBoxForm.Location = new Point(this.Location.X + metroButtonOptions_DIA.Location.X, this.Location.Y + metroButtonOptions_DIA.Location.Y + metroButtonOptions_DIA.Height); // указание местоположения формы
            optionsCheckedListBoxForm.Deactivate += (s, args) => { // обработка если форма станет не активной
                optionsCheckedListBoxForm.Close(); // закрытие формы Checkbox
                this.BeginInvoke(new Action(() => {
                    this.Activate(); // активация основной ормы
                    switchEnabledOption(this, true); // активация объектов основной формы
                }));
            };
            optionsCheckedListBoxForm.CheckedItemsChanged += (checkedItems) => // обработка если на какой-то чекбокс нажали
            {
                selectedCheckBoxes = checkedItems.ToArray(); // получение нового списка выбранных чекбоксов
            };
            if (selectedCheckBoxes != null && selectedCheckBoxes.Length != 0) // если выбран хоть один чекбокс
            {
                optionsCheckedListBoxForm.SetCheckedItems(selectedCheckBoxes.ToList()); // отправка на форму OptionsCheckedListBoxForm выбранных чекбоксов
            }
            switchEnabledOption(this, false); // деактивация объектов основной формы
            optionsCheckedListBoxForm.Show(this); // открытие формы Checkbox
        }
    }
    public class Connection // Содержит информацию о соединении
    {
        public string SourceAddress; // Адрес отправителя
        public string SourcePort; // Порт отправителя
        public string DestinationAddress; // Адрес получателя
        public string DestinationPort; // Порт получателя
        public string Protocol; // Протокол
    }
}