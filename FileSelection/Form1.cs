using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace FileSelection
{
    public partial class Form1 : MetroForm
    {
        private Process _trackedProcess;
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _ramCounter;
        private Timer _monitorTimer;
        public Form1()
        {
            InitializeComponent();


            // Настройка таймера мониторинга
            _monitorTimer = new Timer { Interval = 1000 }; // Обновление раз в секунду
            _monitorTimer.Tick += MonitorProcessResources;
        }

        

        private void buttonChoiceFile_Click(object sender, EventArgs e)
        {
            if (openFileDialogTask.ShowDialog() == DialogResult.OK)
            {
                string openFilePath = openFileDialogTask.FileName;
                button2.Enabled = true;
                try
                {
                    _trackedProcess = Process.Start(openFilePath);
                    InitializePerformanceCounters();
                    _monitorTimer.Start();
                }
                catch (Exception ex)
                {

                    MessageBox.Show($"Не удалось открыть файл: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void InitializePerformanceCounters()
        {
            // CPU usage counter (делим на ядра для получения %)
            _cpuCounter = new PerformanceCounter(
                "Process",
                "% Processor Time",
                _trackedProcess.ProcessName,
                true
            );
            _cpuCounter.NextValue(); // Первое значение всегда 0

            // RAM usage counter (в байтах)
            _ramCounter = new PerformanceCounter(
                "Process",
                "Working Set",
                _trackedProcess.ProcessName,
                true
            );
        }
        private void MonitorProcessResources(object sender, EventArgs e)
        {
            if (_trackedProcess.HasExited)
            {
                _monitorTimer.Stop();
                UpdateStatus("Процесс завершен");
                return;
            }

            try
            {
                // Обновляем данные
                float cpuUsage = _cpuCounter.NextValue() / Environment.ProcessorCount;
                float ramUsageMb = _ramCounter.NextValue() / 1024 / 1024;

                // Выводим информацию
                UpdateStatus($"CPU: {cpuUsage:F1}% | RAM: {ramUsageMb:F1} MB");

                // Дополнительная информация о процессе
                lblProcessInfo.Text = $"Threads: {_trackedProcess.Threads.Count} | " +
                                    $"Handles: {_trackedProcess.HandleCount}";
            }
            catch
            {
                _monitorTimer.Stop();
                UpdateStatus("Ошибка мониторинга");
            }
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
            // Корректная очистка ресурсов
            _monitorTimer?.Stop();
            _cpuCounter?.Dispose();
            _ramCounter?.Dispose();
            _trackedProcess?.Close();
            base.OnFormClosing(e);
        }


        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {


            //  var process = new Process();
            //var startInfo = new ProcessStartInfo();
            //startInfo.FileName = MonResSM.Task1;
            //startInfo.ErrorDialog = true;
            //process.StartInfo = startInfo;
            //process.Start();
        }

       

        private void checkedListBoxfunction_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       
    }
}
