namespace SMERH
{
    partial class MainWindow_SMA
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow_SMA));
            this.openFileDialogTask = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonChoiceFile = new System.Windows.Forms.Button();
            this.buttonOpenDataMon_SMA = new System.Windows.Forms.Button();
            this.numericUpDownInterval_SMA = new System.Windows.Forms.NumericUpDown();
            this.lblProcessInfo = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.checkedListBoxfunction = new System.Windows.Forms.CheckedListBox();
            this.OutPutTextBox_BVP = new MetroFramework.Controls.MetroTextBox();
            this.ButtonStartTimer_SMA = new MetroFramework.Controls.MetroButton();
            this.MonitoringDurationNumeric_SMA = new System.Windows.Forms.NumericUpDown();
            this.TimeRemainingLabel_SMA = new MetroFramework.Controls.MetroLabel();
            this.metroTextBoxMetric_SMA = new MetroFramework.Controls.MetroTextBox();
            this.EmergencyStopButton_SMA = new MetroFramework.Controls.MetroButton();
            this.metroLabel_DIA = new MetroFramework.Controls.MetroLabel();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInterval_SMA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonitoringDurationNumeric_SMA)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialogTask
            // 
            this.openFileDialogTask.FileName = "openFileDialogTask";
            // 
            // toolTip1
            // 
            this.toolTip1.ShowAlways = true;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "Подсказка";
            // 
            // buttonChoiceFile
            // 
            this.buttonChoiceFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonChoiceFile.Image = ((System.Drawing.Image)(resources.GetObject("buttonChoiceFile.Image")));
            this.buttonChoiceFile.Location = new System.Drawing.Point(10, 67);
            this.buttonChoiceFile.Margin = new System.Windows.Forms.Padding(2);
            this.buttonChoiceFile.Name = "buttonChoiceFile";
            this.buttonChoiceFile.Size = new System.Drawing.Size(68, 50);
            this.buttonChoiceFile.TabIndex = 0;
            this.toolTip1.SetToolTip(this.buttonChoiceFile, "выберете файл");
            this.buttonChoiceFile.UseVisualStyleBackColor = true;
            this.buttonChoiceFile.Click += new System.EventHandler(this.buttonChoiceFile_Click);
            // 
            // buttonOpenDataMon_SMA
            // 
            this.buttonOpenDataMon_SMA.Enabled = false;
            this.buttonOpenDataMon_SMA.Image = ((System.Drawing.Image)(resources.GetObject("buttonOpenDataMon_SMA.Image")));
            this.buttonOpenDataMon_SMA.Location = new System.Drawing.Point(82, 67);
            this.buttonOpenDataMon_SMA.Margin = new System.Windows.Forms.Padding(2);
            this.buttonOpenDataMon_SMA.Name = "buttonOpenDataMon_SMA";
            this.buttonOpenDataMon_SMA.Size = new System.Drawing.Size(68, 50);
            this.buttonOpenDataMon_SMA.TabIndex = 1;
            this.toolTip1.SetToolTip(this.buttonOpenDataMon_SMA, "Перейти к выбору мониторинга");
            this.buttonOpenDataMon_SMA.UseVisualStyleBackColor = true;
           
            // 
            // numericUpDownInterval_SMA
            // 
            this.numericUpDownInterval_SMA.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownInterval_SMA.Location = new System.Drawing.Point(170, 99);
            this.numericUpDownInterval_SMA.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDownInterval_SMA.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownInterval_SMA.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownInterval_SMA.Name = "numericUpDownInterval_SMA";
            this.numericUpDownInterval_SMA.Size = new System.Drawing.Size(90, 20);
            this.numericUpDownInterval_SMA.TabIndex = 10;
            this.toolTip1.SetToolTip(this.numericUpDownInterval_SMA, "Интервал таймера в миллисекундах");
            this.numericUpDownInterval_SMA.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // lblProcessInfo
            // 
            this.lblProcessInfo.AutoSize = true;
            this.lblProcessInfo.Location = new System.Drawing.Point(17, 180);
            this.lblProcessInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblProcessInfo.Name = "lblProcessInfo";
            this.lblProcessInfo.Size = new System.Drawing.Size(0, 13);
            this.lblProcessInfo.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(17, 225);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 3;
            // 
            // checkedListBoxfunction
            // 
            this.checkedListBoxfunction.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.checkedListBoxfunction.FormattingEnabled = true;
            this.checkedListBoxfunction.Items.AddRange(new object[] {
            "Сетевая активность",
            "Файловая активность",
            "Процессы",
            "Нагрузка"});
            this.checkedListBoxfunction.Location = new System.Drawing.Point(10, 203);
            this.checkedListBoxfunction.Margin = new System.Windows.Forms.Padding(2);
            this.checkedListBoxfunction.Name = "checkedListBoxfunction";
            this.checkedListBoxfunction.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.checkedListBoxfunction.Size = new System.Drawing.Size(155, 60);
            this.checkedListBoxfunction.TabIndex = 5;
            this.checkedListBoxfunction.MouseUp += new System.Windows.Forms.MouseEventHandler(this.checkedListBoxfunction_DIA_MouseUp);
            // 
            // OutPutTextBox_BVP
            // 
            this.OutPutTextBox_BVP.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.OutPutTextBox_BVP.CustomButton.Image = null;
            this.OutPutTextBox_BVP.CustomButton.Location = new System.Drawing.Point(244, 2);
            this.OutPutTextBox_BVP.CustomButton.Margin = new System.Windows.Forms.Padding(2);
            this.OutPutTextBox_BVP.CustomButton.Name = "";
            this.OutPutTextBox_BVP.CustomButton.Size = new System.Drawing.Size(181, 181);
            this.OutPutTextBox_BVP.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.OutPutTextBox_BVP.CustomButton.TabIndex = 1;
            this.OutPutTextBox_BVP.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.OutPutTextBox_BVP.CustomButton.UseSelectable = true;
            this.OutPutTextBox_BVP.CustomButton.Visible = false;
            this.OutPutTextBox_BVP.Lines = new string[] {
        "Выберите прикладное программмное обеспечение для тестирования"};
            this.OutPutTextBox_BVP.Location = new System.Drawing.Point(170, 180);
            this.OutPutTextBox_BVP.Margin = new System.Windows.Forms.Padding(2);
            this.OutPutTextBox_BVP.MaxLength = 32767;
            this.OutPutTextBox_BVP.Multiline = true;
            this.OutPutTextBox_BVP.Name = "OutPutTextBox_BVP";
            this.OutPutTextBox_BVP.PasswordChar = '\0';
            this.OutPutTextBox_BVP.ReadOnly = true;
            this.OutPutTextBox_BVP.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.OutPutTextBox_BVP.SelectedText = "";
            this.OutPutTextBox_BVP.SelectionLength = 0;
            this.OutPutTextBox_BVP.SelectionStart = 0;
            this.OutPutTextBox_BVP.ShortcutsEnabled = true;
            this.OutPutTextBox_BVP.Size = new System.Drawing.Size(428, 186);
            this.OutPutTextBox_BVP.TabIndex = 6;
            this.OutPutTextBox_BVP.Text = "Выберите прикладное программмное обеспечение для тестирования";
            this.OutPutTextBox_BVP.UseSelectable = true;
            this.OutPutTextBox_BVP.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.OutPutTextBox_BVP.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // ButtonStartTimer_SMA
            // 
            this.ButtonStartTimer_SMA.Location = new System.Drawing.Point(274, 150);
            this.ButtonStartTimer_SMA.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonStartTimer_SMA.Name = "ButtonStartTimer_SMA";
            this.ButtonStartTimer_SMA.Size = new System.Drawing.Size(88, 15);
            this.ButtonStartTimer_SMA.TabIndex = 7;
            this.ButtonStartTimer_SMA.Text = "Старт таймера\r\n";
            this.ButtonStartTimer_SMA.UseSelectable = true;
            this.ButtonStartTimer_SMA.Click += new System.EventHandler(this.ButtonStartTimer_SMA_Click);
            // 
            // MonitoringDurationNumeric_SMA
            // 
            this.MonitoringDurationNumeric_SMA.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.MonitoringDurationNumeric_SMA.Location = new System.Drawing.Point(170, 147);
            this.MonitoringDurationNumeric_SMA.Margin = new System.Windows.Forms.Padding(2);
            this.MonitoringDurationNumeric_SMA.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.MonitoringDurationNumeric_SMA.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MonitoringDurationNumeric_SMA.Name = "MonitoringDurationNumeric_SMA";
            this.MonitoringDurationNumeric_SMA.Size = new System.Drawing.Size(90, 20);
            this.MonitoringDurationNumeric_SMA.TabIndex = 8;
            this.MonitoringDurationNumeric_SMA.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // TimeRemainingLabel_SMA
            // 
            this.TimeRemainingLabel_SMA.AutoSize = true;
            this.TimeRemainingLabel_SMA.ForeColor = System.Drawing.Color.Blue;
            this.TimeRemainingLabel_SMA.Location = new System.Drawing.Point(374, 150);
            this.TimeRemainingLabel_SMA.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.TimeRemainingLabel_SMA.Name = "TimeRemainingLabel_SMA";
            this.TimeRemainingLabel_SMA.Size = new System.Drawing.Size(157, 19);
            this.TimeRemainingLabel_SMA.TabIndex = 9;
            this.TimeRemainingLabel_SMA.Text = "Таймер не активирован";
            this.TimeRemainingLabel_SMA.UseCustomForeColor = true;
            // 
            // metroTextBoxMetric_SMA
            // 
            // 
            // 
            // 
            this.metroTextBoxMetric_SMA.CustomButton.Image = null;
            this.metroTextBoxMetric_SMA.CustomButton.Location = new System.Drawing.Point(15, 2);
            this.metroTextBoxMetric_SMA.CustomButton.Margin = new System.Windows.Forms.Padding(2);
            this.metroTextBoxMetric_SMA.CustomButton.Name = "";
            this.metroTextBoxMetric_SMA.CustomButton.Size = new System.Drawing.Size(13, 13);
            this.metroTextBoxMetric_SMA.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroTextBoxMetric_SMA.CustomButton.TabIndex = 1;
            this.metroTextBoxMetric_SMA.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroTextBoxMetric_SMA.CustomButton.UseSelectable = true;
            this.metroTextBoxMetric_SMA.CustomButton.Visible = false;
            this.metroTextBoxMetric_SMA.Lines = new string[] {
        "мс"};
            this.metroTextBoxMetric_SMA.Location = new System.Drawing.Point(274, 99);
            this.metroTextBoxMetric_SMA.Margin = new System.Windows.Forms.Padding(2);
            this.metroTextBoxMetric_SMA.MaxLength = 32767;
            this.metroTextBoxMetric_SMA.Multiline = true;
            this.metroTextBoxMetric_SMA.Name = "metroTextBoxMetric_SMA";
            this.metroTextBoxMetric_SMA.PasswordChar = '\0';
            this.metroTextBoxMetric_SMA.ReadOnly = true;
            this.metroTextBoxMetric_SMA.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.metroTextBoxMetric_SMA.SelectedText = "";
            this.metroTextBoxMetric_SMA.SelectionLength = 0;
            this.metroTextBoxMetric_SMA.SelectionStart = 0;
            this.metroTextBoxMetric_SMA.ShortcutsEnabled = true;
            this.metroTextBoxMetric_SMA.Size = new System.Drawing.Size(31, 18);
            this.metroTextBoxMetric_SMA.TabIndex = 12;
            this.metroTextBoxMetric_SMA.Text = "мс";
            this.metroTextBoxMetric_SMA.UseSelectable = true;
            this.metroTextBoxMetric_SMA.UseStyleColors = true;
            this.metroTextBoxMetric_SMA.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.metroTextBoxMetric_SMA.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // EmergencyStopButton_SMA
            // 
            this.EmergencyStopButton_SMA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EmergencyStopButton_SMA.Cursor = System.Windows.Forms.Cursors.Default;
            this.EmergencyStopButton_SMA.ForeColor = System.Drawing.Color.Red;
            this.EmergencyStopButton_SMA.Location = new System.Drawing.Point(526, 28);
            this.EmergencyStopButton_SMA.Margin = new System.Windows.Forms.Padding(2);
            this.EmergencyStopButton_SMA.Name = "EmergencyStopButton_SMA";
            this.EmergencyStopButton_SMA.Size = new System.Drawing.Size(56, 50);
            this.EmergencyStopButton_SMA.TabIndex = 13;
            this.EmergencyStopButton_SMA.Text = "СТОП";
            this.EmergencyStopButton_SMA.UseCustomBackColor = true;
            this.EmergencyStopButton_SMA.UseCustomForeColor = true;
            this.EmergencyStopButton_SMA.UseSelectable = true;
            this.EmergencyStopButton_SMA.UseStyleColors = true;
            this.EmergencyStopButton_SMA.Click += new System.EventHandler(this.EmergencyStopButton_SMA_Click);
            // 
            // metroLabel_DIA
            // 
            this.metroLabel_DIA.AutoSize = true;
            this.metroLabel_DIA.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel_DIA.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel_DIA.ForeColor = System.Drawing.Color.Transparent;
            this.metroLabel_DIA.Location = new System.Drawing.Point(170, 76);
            this.metroLabel_DIA.Name = "metroLabel_DIA";
            this.metroLabel_DIA.Size = new System.Drawing.Size(137, 15);
            this.metroLabel_DIA.TabIndex = 14;
            this.metroLabel_DIA.Text = "Интервал мониторинга";
            // 
            // MainWindow_SMA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            this.ClientSize = new System.Drawing.Size(600, 366);
            this.Controls.Add(this.metroLabel_DIA);

            this.Controls.Add(this.EmergencyStopButton_SMA);
            this.Controls.Add(this.metroTextBoxMetric_SMA);
            this.Controls.Add(this.numericUpDownInterval_SMA);
            this.Controls.Add(this.TimeRemainingLabel_SMA);
            this.Controls.Add(this.MonitoringDurationNumeric_SMA);
            this.Controls.Add(this.ButtonStartTimer_SMA);
            this.Controls.Add(this.OutPutTextBox_BVP);
            this.Controls.Add(this.checkedListBoxfunction);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblProcessInfo);
            this.Controls.Add(this.buttonOpenDataMon_SMA);
            this.Controls.Add(this.buttonChoiceFile);
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(580, 360);
            this.Name = "MainWindow_SMA";
            this.Padding = new System.Windows.Forms.Padding(15, 60, 15, 16);
            this.Text = "СМЕРШ";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInterval_SMA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonitoringDurationNumeric_SMA)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialogTask;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button buttonChoiceFile;
        private System.Windows.Forms.Button buttonOpenDataMon_SMA;
        private System.Windows.Forms.Label lblProcessInfo;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckedListBox checkedListBoxfunction;
        private MetroFramework.Controls.MetroTextBox OutPutTextBox_BVP;
        private MetroFramework.Controls.MetroButton ButtonStartTimer_SMA;
        private System.Windows.Forms.NumericUpDown MonitoringDurationNumeric_SMA;
        private MetroFramework.Controls.MetroLabel TimeRemainingLabel_SMA;
        private System.Windows.Forms.NumericUpDown numericUpDownInterval_SMA;
        private MetroFramework.Controls.MetroTextBox metroTextBoxMetric_SMA;
        private MetroFramework.Controls.MetroButton EmergencyStopButton_SMA;
        private MetroFramework.Controls.MetroLabel metroLabel_DIA;
    }
}

