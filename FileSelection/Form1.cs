/*using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using Microsoft.VisualBasic.Devices;
using System.Management;

namespace FileSelection
{
    public partial class Form1 : MetroForm
    {
        private Process _trackedProcess;
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _ramCounter;
        private PerformanceCounter _ioReadCounter;
        private PerformanceCounter _ioWriteCounter;
        private Timer _monitorTimer;
        private TextBox _monitorTextBox;

        public Form1()
        {

            InitializeComponent();
            InitializeMonitoringComponents();
            InitializeMonitorTextBox();
            _monitorTimer = new Timer { Interval = 1000 }; // 1 секунда
            _monitorTimer.Tick += (s, e) => UpdateMonitoring();

        }

        private void InitializeMonitorTextBox()
        {
            _monitorTextBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Location = new Point(200, 200),
                Size = new Size(400, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(_monitorTextBox);
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

        private void InitializeMonitoringComponents()
        {
            checkedListBoxfunction.CheckOnClick = true;
            checkedListBoxfunction.SelectedIndexChanged += (s, e) => UpdateMonitoring();

        }

        private void UpdateMonitoring()
        {
            _monitorTextBox.Clear();

            if (_trackedProcess == null || _trackedProcess.HasExited)
            {
                _monitorTextBox.Text = "Процесс не запущен или завершен";
                return;
            }

            if (checkedListBoxfunction.CheckedItems.Count == 0)
            {
                _monitorTextBox.Text = "Выберите параметры для мониторинга";
                return;
            }

            foreach (var item in checkedListBoxfunction.CheckedItems)
            {
                switch (item.ToString())
                {
                    case "Сетевая активность":
                        ShowNetworkInfo();
                        break;
                    case "Файловая активность":
                        ShowFileActivity();
                        break;
                    case "Процессы":
                        ShowProcessesInfo();
                        break;
                    case "Нагрузка":
                        ShowSystemLoad();
                        break;
                }
                _monitorTextBox.AppendText(Environment.NewLine);
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

        private void ShowFileActivity()
        {
            _monitorTextBox.AppendText("=== Файловая активность процесса ===\r\n");

            try
            {
                float readSpeed = _ioReadCounter.NextValue() / 1024;
                float writeSpeed = _ioWriteCounter.NextValue() / 1024;

                _monitorTextBox.AppendText($"Скорость чтения: {readSpeed:F1} KB/s\n");
                _monitorTextBox.AppendText($"Скорость записи: {writeSpeed:F1} KB/s\n\n");

                // Список загруженных модулей (DLL)
                _monitorTextBox.AppendText("Загруженные модули:\n");
                foreach (ProcessModule module in _trackedProcess.Modules.Cast<ProcessModule>().Take(5))
                {
                    _monitorTextBox.AppendText($"{module.FileName}\n");
                }
            }
            catch (Exception ex)
            {
                _monitorTextBox.AppendText($"Ошибка: {ex.Message}\r\n");
            }
        }

        private void ShowProcessesInfo()
        {
            _monitorTextBox.AppendText("=== Информация о процессе ===\r\n");

            try
            {
                _monitorTextBox.AppendText($"Имя: {_trackedProcess.ProcessName}\n");
                _monitorTextBox.AppendText($"ID: {_trackedProcess.Id}\n");
                _monitorTextBox.AppendText($"Время запуска: {_trackedProcess.StartTime}\n");
                _monitorTextBox.AppendText($"Потоки: {_trackedProcess.Threads.Count}\n");
                _monitorTextBox.AppendText($"Дескрипторы: {_trackedProcess.HandleCount}\n");
            }
            catch (Exception ex)
            {
                _monitorTextBox.AppendText($"Ошибка: {ex.Message}\r\n");
            }
        }

        private void ShowSystemLoad()
        {
            _monitorTextBox.AppendText("=== Нагрузка процесса ===\r\n");

            try
            {
                float cpuUsage = _cpuCounter.NextValue() / Environment.ProcessorCount;
                float ramUsageMB = _ramCounter.NextValue() / (1024 * 1024);

                _monitorTextBox.AppendText($"CPU: {cpuUsage:F1}%\n");
                _monitorTextBox.AppendText($"RAM: {ramUsageMB:F1} MB\n");
            }
            catch (Exception ex)
            {
                _monitorTextBox.AppendText($"Ошибка: {ex.Message}\r\n");
            }
        }

        private void buttonChoiceFile_Click(object sender, EventArgs e)
        {
            if (openFileDialogTask.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Закрываем предыдущий процесс, если был
                    _trackedProcess?.Close();

                    // Запускаем новый процесс
                    _trackedProcess = Process.Start(openFileDialogTask.FileName);
                    InitializePerformanceCounters();
                    _monitorTimer.Start();
                    button2.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось открыть файл: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InitializePerformanceCounters()
        {
            if (_trackedProcess == null || _trackedProcess.HasExited) return;

            // CPU usage
            _cpuCounter = new PerformanceCounter("Process", "% Processor Time",
                _trackedProcess.ProcessName, true);
            _cpuCounter.NextValue();

            // RAM usage
            _ramCounter = new PerformanceCounter("Process", "Working Set",
                _trackedProcess.ProcessName, true);

            // IO Counters
            _ioReadCounter = new PerformanceCounter("Process", "IO Read Bytes/sec",
                _trackedProcess.ProcessName, true);
            _ioWriteCounter = new PerformanceCounter("Process", "IO Write Bytes/sec",
                _trackedProcess.ProcessName, true);
        }



        private void UpdateStatus(string message)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(() => lblStatus.Text = message));
            }
            else
            {
                lblStatus.Text = message;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _monitorTimer?.Stop();
            _cpuCounter?.Dispose();
            _ramCounter?.Dispose();
            _ioReadCounter?.Dispose();
            _ioWriteCounter?.Dispose();
            _trackedProcess?.Close();
            base.OnFormClosing(e);
        }

        // Остальные методы оставляем без изменений
        private void toolTip1_Popup(object sender, PopupEventArgs e) { }
        private void button2_Click(object sender, EventArgs e) { }
        private void checkedListBoxfunction_SelectedIndexChanged(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using Microsoft.VisualBasic.Devices;
using System.Management;

namespace FileSelection
{
    public partial class Form1 : MetroForm
    {
        private Process _trackedProcess;
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _ramCounter;
        private PerformanceCounter _ioReadCounter;
        private PerformanceCounter _ioWriteCounter;
        private Timer _monitorTimer;
        private Timer _stopMonitoringTimer;
        private TextBox _monitorTextBox;
        private NumericUpDown _monitoringDurationInput;
        private Button _setDurationButton;
        private Label _timeRemainingLabel;
        private int _remainingSeconds;

        public Form1()
        {
            InitializeComponent();
            InitializeMonitoringComponents();
            InitializeMonitorTextBox();
            InitializeMonitoringDurationControls();

            _monitorTimer = new Timer { Interval = 1000 }; // 1 секунда
            _monitorTimer.Tick += (s, e) => UpdateMonitoring();

            _stopMonitoringTimer = new Timer { Interval = 1000 }; // 1 секунда
            _stopMonitoringTimer.Tick += StopMonitoringTimer_Tick;
        }

        private void InitializeMonitoringDurationControls()
        {
            // Label
            var durationLabel = new Label
            {
                Text = "Длительность мониторинга (сек):",
                Location = new Point(0, 150),
                AutoSize = true
            };
            this.Controls.Add(durationLabel);

            // Numeric input
            _monitoringDurationInput = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 3600, // 1 час максимум
                Value = 60, // 1 минута по умолчанию
                Location = new Point(180, 150),
                Width = 60
            };
            this.Controls.Add(_monitoringDurationInput);

            // Set duration button
            _setDurationButton = new Button()
            {
                Text = "Установить",
                Location = new Point(250, 150),
                Width = 80
            };
            _setDurationButton.Click += SetDurationButton_Click;
            this.Controls.Add(_setDurationButton);

            // Time remaining label
            _timeRemainingLabel = new Label
            {
                Text = "Таймер не активирован",
                Location = new Point(350, 150),
                AutoSize = true,
                ForeColor = Color.Blue
            };
            this.Controls.Add(_timeRemainingLabel);
        }

        private void SetDurationButton_Click(object sender, EventArgs e)
        {
            if (_trackedProcess == null || _trackedProcess.HasExited)
            {
                MessageBox.Show("Сначала запустите процесс для мониторинга", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _remainingSeconds = (int)_monitoringDurationInput.Value;
            _stopMonitoringTimer.Start();
            _timeRemainingLabel.Text = $"Осталось: {_remainingSeconds} сек";
            _timeRemainingLabel.ForeColor = Color.Green;

            MessageBox.Show($"Таймер мониторинга установлен на {_remainingSeconds} секунд", "Таймер",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void StopMonitoringTimer_Tick(object sender, EventArgs e)
        {
            _remainingSeconds--;
            _timeRemainingLabel.Text = $"Осталось: {_remainingSeconds} сек";

            if (_remainingSeconds <= 0)
            {
                _stopMonitoringTimer.Stop();
                _monitorTimer.Stop();
                _timeRemainingLabel.Text = "Мониторинг остановлен по таймеру";
                _timeRemainingLabel.ForeColor = Color.Red;

                MessageBox.Show("Мониторинг остановлен по истечении заданного времени", "Таймер",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void InitializeMonitorTextBox()
        {
            _monitorTextBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Location = new Point(200, 200),
                Size = new Size(400, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(_monitorTextBox);
        }

        private void InitializeMonitoringComponents()
        {
            checkedListBoxfunction.CheckOnClick = true;
            checkedListBoxfunction.SelectedIndexChanged += (s, e) => UpdateMonitoring();
        }

        private void UpdateMonitoring()
        {
            _monitorTextBox.Clear();

            if (_trackedProcess == null || _trackedProcess.HasExited)
            {
                _monitorTextBox.Text = "Процесс не запущен или завершен";
                return;
            }

            if (checkedListBoxfunction.CheckedItems.Count == 0)
            {
                _monitorTextBox.Text = "Выберите параметры для мониторинга";
                return;
            }

            foreach (var item in checkedListBoxfunction.CheckedItems)
            {
                switch (item.ToString())
                {
                    case "Сетевая активность":
                        ShowNetworkInfo();
                        break;
                    case "Файловая активность":
                        ShowFileActivity();
                        break;
                    case "Процессы":
                        ShowProcessesInfo();
                        break;
                    case "Нагрузка":
                        ShowSystemLoad();
                        break;
                }
                _monitorTextBox.AppendText(Environment.NewLine);
            }
        }

        private void ShowNetworkInfo()
        {
            _monitorTextBox.AppendText("=== Сетевая активность процесса ===\r\n");

            try
            {
                _monitorTextBox.AppendText($"Используемые порты:\n");

                var connections = IPGlobalProperties.GetIPGlobalProperties()
                    .GetActiveTcpConnections()
                    .Where(c => c.LocalEndPoint.Port == _trackedProcess.Id);

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

        private void ShowFileActivity()
        {
            _monitorTextBox.AppendText("=== Файловая активность процесса ===\r\n");

            try
            {
                float readSpeed = _ioReadCounter.NextValue() / 1024;
                float writeSpeed = _ioWriteCounter.NextValue() / 1024;

                _monitorTextBox.AppendText($"Скорость чтения: {readSpeed:F1} KB/s\n");
                _monitorTextBox.AppendText($"Скорость записи: {writeSpeed:F1} KB/s\n\n");

                _monitorTextBox.AppendText("Загруженные модули:\n");
                foreach (ProcessModule module in _trackedProcess.Modules.Cast<ProcessModule>().Take(5))
                {
                    _monitorTextBox.AppendText($"{module.FileName}\n");
                }
            }
            catch (Exception ex)
            {
                _monitorTextBox.AppendText($"Ошибка: {ex.Message}\r\n");
            }
        }

        private void ShowProcessesInfo()
        {
            _monitorTextBox.AppendText("=== Информация о процессе ===\r\n");

            try
            {
                _monitorTextBox.AppendText($"Имя: {_trackedProcess.ProcessName}\n");
                _monitorTextBox.AppendText($"ID: {_trackedProcess.Id}\n");
                _monitorTextBox.AppendText($"Время запуска: {_trackedProcess.StartTime}\n");
                _monitorTextBox.AppendText($"Потоки: {_trackedProcess.Threads.Count}\n");
                _monitorTextBox.AppendText($"Дескрипторы: {_trackedProcess.HandleCount}\n");
            }
            catch (Exception ex)
            {
                _monitorTextBox.AppendText($"Ошибка: {ex.Message}\r\n");
            }
        }

        private void ShowSystemLoad()
        {
            _monitorTextBox.AppendText("=== Нагрузка процесса ===\r\n");

            try
            {
                float cpuUsage = _cpuCounter.NextValue() / Environment.ProcessorCount;
                float ramUsageMB = _ramCounter.NextValue() / (1024 * 1024);

                _monitorTextBox.AppendText($"CPU: {cpuUsage:F1}%\n");
                _monitorTextBox.AppendText($"RAM: {ramUsageMB:F1} MB\n");
            }
            catch (Exception ex)
            {
                _monitorTextBox.AppendText($"Ошибка: {ex.Message}\r\n");
            }
        }

        private void buttonChoiceFile_Click(object sender, EventArgs e)
        {
            if (openFileDialogTask.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _trackedProcess?.Close();
                    _stopMonitoringTimer.Stop();
                    _timeRemainingLabel.Text = "Таймер не активирован";
                    _timeRemainingLabel.ForeColor = Color.Blue;

                    _trackedProcess = Process.Start(openFileDialogTask.FileName);
                    InitializePerformanceCounters();
                    _monitorTimer.Start();
                    button2.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось открыть файл: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InitializePerformanceCounters()
        {
            if (_trackedProcess == null || _trackedProcess.HasExited) return;

            _cpuCounter = new PerformanceCounter("Process", "% Processor Time",
                _trackedProcess.ProcessName, true);
            _cpuCounter.NextValue();

            _ramCounter = new PerformanceCounter("Process", "Working Set",
                _trackedProcess.ProcessName, true);

            _ioReadCounter = new PerformanceCounter("Process", "IO Read Bytes/sec",
                _trackedProcess.ProcessName, true);
            _ioWriteCounter = new PerformanceCounter("Process", "IO Write Bytes/sec",
                _trackedProcess.ProcessName, true);
        }

        private void UpdateStatus(string message)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(() => lblStatus.Text = message));
            }
            else
            {
                lblStatus.Text = message;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _monitorTimer?.Stop();
            _stopMonitoringTimer?.Stop();
            _cpuCounter?.Dispose();
            _ramCounter?.Dispose();
            _ioReadCounter?.Dispose();
            _ioWriteCounter?.Dispose();
            _trackedProcess?.Close();
            base.OnFormClosing(e);
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e) { }
        private void button2_Click(object sender, EventArgs e) { }
        private void checkedListBoxfunction_SelectedIndexChanged(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Controls.Add(_setDurationButton);
        }
    }
}