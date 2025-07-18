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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow_SMA));
            openFileDialogTask = new OpenFileDialog();
            toolTip1 = new ToolTip(components);
            buttonChoiceFile = new Button();
            buttonOpenDataMon_SMA = new Button();
            numericUpDownInterval_SMA = new NumericUpDown();
            lblProcessInfo = new Label();
            lblStatus = new Label();
            ButtonStartTimer_SMA = new MetroFramework.Controls.MetroButton();
            MonitoringDurationNumeric_SMA = new NumericUpDown();
            TimeRemainingLabel_SMA = new MetroFramework.Controls.MetroLabel();
            EmergencyStopButton_SMA = new MetroFramework.Controls.MetroButton();
            metroLabel_DIA = new MetroFramework.Controls.MetroLabel();
            metroButtonOptions_DIA = new MetroFramework.Controls.MetroButton();
            OutPutTextBox_BVP = new TextBox();
            ((System.ComponentModel.ISupportInitialize)numericUpDownInterval_SMA).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MonitoringDurationNumeric_SMA).BeginInit();
            SuspendLayout();
            // 
            // openFileDialogTask
            // 
            openFileDialogTask.FileName = "openFileDialogTask";
            // 
            // toolTip1
            // 
            toolTip1.ShowAlways = true;
            toolTip1.ToolTipIcon = ToolTipIcon.Info;
            toolTip1.ToolTipTitle = "Подсказка";
            // 
            // buttonChoiceFile
            // 
            buttonChoiceFile.Cursor = Cursors.Hand;
            buttonChoiceFile.Image = (Image)resources.GetObject("buttonChoiceFile.Image");
            buttonChoiceFile.Location = new Point(12, 77);
            buttonChoiceFile.Margin = new Padding(2);
            buttonChoiceFile.Name = "buttonChoiceFile";
            buttonChoiceFile.Size = new Size(79, 58);
            buttonChoiceFile.TabIndex = 0;
            toolTip1.SetToolTip(buttonChoiceFile, "выберете файл");
            buttonChoiceFile.UseVisualStyleBackColor = true;
            buttonChoiceFile.Click += buttonChoiceFile_Click;
            // 
            // buttonOpenDataMon_SMA
            // 
            buttonOpenDataMon_SMA.Enabled = false;
            buttonOpenDataMon_SMA.Image = (Image)resources.GetObject("buttonOpenDataMon_SMA.Image");
            buttonOpenDataMon_SMA.Location = new Point(96, 77);
            buttonOpenDataMon_SMA.Margin = new Padding(2);
            buttonOpenDataMon_SMA.Name = "buttonOpenDataMon_SMA";
            buttonOpenDataMon_SMA.Size = new Size(79, 58);
            buttonOpenDataMon_SMA.TabIndex = 1;
            toolTip1.SetToolTip(buttonOpenDataMon_SMA, "Перейти к выбору мониторинга");
            buttonOpenDataMon_SMA.UseVisualStyleBackColor = true;
            // 
            // numericUpDownInterval_SMA
            // 
            numericUpDownInterval_SMA.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            numericUpDownInterval_SMA.Location = new Point(198, 114);
            numericUpDownInterval_SMA.Margin = new Padding(2);
            numericUpDownInterval_SMA.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDownInterval_SMA.Minimum = new decimal(new int[] { 1000, 0, 0, 0 });
            numericUpDownInterval_SMA.Name = "numericUpDownInterval_SMA";
            numericUpDownInterval_SMA.Size = new Size(105, 23);
            numericUpDownInterval_SMA.TabIndex = 10;
            toolTip1.SetToolTip(numericUpDownInterval_SMA, "Интервал таймера в миллисекундах");
            numericUpDownInterval_SMA.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            // 
            // lblProcessInfo
            // 
            lblProcessInfo.AutoSize = true;
            lblProcessInfo.Location = new Point(20, 208);
            lblProcessInfo.Margin = new Padding(2, 0, 2, 0);
            lblProcessInfo.Name = "lblProcessInfo";
            lblProcessInfo.Size = new Size(0, 15);
            lblProcessInfo.TabIndex = 2;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(20, 260);
            lblStatus.Margin = new Padding(2, 0, 2, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 15);
            lblStatus.TabIndex = 3;
            // 
            // ButtonStartTimer_SMA
            // 
            ButtonStartTimer_SMA.Location = new Point(320, 173);
            ButtonStartTimer_SMA.Margin = new Padding(2);
            ButtonStartTimer_SMA.Name = "ButtonStartTimer_SMA";
            ButtonStartTimer_SMA.Size = new Size(103, 17);
            ButtonStartTimer_SMA.TabIndex = 7;
            ButtonStartTimer_SMA.Text = "Старт таймера\r\n";
            ButtonStartTimer_SMA.UseSelectable = true;
            ButtonStartTimer_SMA.Click += ButtonStartTimer_SMA_Click;
            // 
            // MonitoringDurationNumeric_SMA
            // 
            MonitoringDurationNumeric_SMA.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            MonitoringDurationNumeric_SMA.Location = new Point(198, 170);
            MonitoringDurationNumeric_SMA.Margin = new Padding(2);
            MonitoringDurationNumeric_SMA.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            MonitoringDurationNumeric_SMA.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            MonitoringDurationNumeric_SMA.Name = "MonitoringDurationNumeric_SMA";
            MonitoringDurationNumeric_SMA.Size = new Size(105, 23);
            MonitoringDurationNumeric_SMA.TabIndex = 8;
            MonitoringDurationNumeric_SMA.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // TimeRemainingLabel_SMA
            // 
            TimeRemainingLabel_SMA.AutoSize = true;
            TimeRemainingLabel_SMA.ForeColor = Color.Blue;
            TimeRemainingLabel_SMA.Location = new Point(436, 173);
            TimeRemainingLabel_SMA.Margin = new Padding(2, 0, 2, 0);
            TimeRemainingLabel_SMA.Name = "TimeRemainingLabel_SMA";
            TimeRemainingLabel_SMA.Size = new Size(157, 19);
            TimeRemainingLabel_SMA.TabIndex = 9;
            TimeRemainingLabel_SMA.Text = "Таймер не активирован";
            TimeRemainingLabel_SMA.UseCustomForeColor = true;
            // 
            // EmergencyStopButton_SMA
            // 
            EmergencyStopButton_SMA.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            EmergencyStopButton_SMA.ForeColor = Color.Red;
            EmergencyStopButton_SMA.Location = new Point(614, 32);
            EmergencyStopButton_SMA.Margin = new Padding(2);
            EmergencyStopButton_SMA.Name = "EmergencyStopButton_SMA";
            EmergencyStopButton_SMA.Size = new Size(65, 58);
            EmergencyStopButton_SMA.TabIndex = 13;
            EmergencyStopButton_SMA.Text = "СТОП";
            EmergencyStopButton_SMA.UseCustomBackColor = true;
            EmergencyStopButton_SMA.UseCustomForeColor = true;
            EmergencyStopButton_SMA.UseSelectable = true;
            EmergencyStopButton_SMA.UseStyleColors = true;
            EmergencyStopButton_SMA.Click += EmergencyStopButton_SMA_Click;
            // 
            // metroLabel_DIA
            // 
            metroLabel_DIA.AutoSize = true;
            metroLabel_DIA.FontSize = MetroFramework.MetroLabelSize.Small;
            metroLabel_DIA.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            metroLabel_DIA.ForeColor = Color.Transparent;
            metroLabel_DIA.Location = new Point(198, 88);
            metroLabel_DIA.Margin = new Padding(4, 0, 4, 0);
            metroLabel_DIA.Name = "metroLabel_DIA";
            metroLabel_DIA.Size = new Size(137, 15);
            metroLabel_DIA.TabIndex = 14;
            metroLabel_DIA.Text = "Интервал мониторинга";
            // 
            // metroButtonOptions_DIA
            // 
            metroButtonOptions_DIA.Location = new Point(12, 166);
            metroButtonOptions_DIA.Margin = new Padding(4, 3, 4, 3);
            metroButtonOptions_DIA.Name = "metroButtonOptions_DIA";
            metroButtonOptions_DIA.Size = new Size(91, 57);
            metroButtonOptions_DIA.TabIndex = 15;
            metroButtonOptions_DIA.Text = "Параметры";
            metroButtonOptions_DIA.UseSelectable = true;
            metroButtonOptions_DIA.Click += metroButtonOptions_DIA_Click;
            // 
            // OutPutTextBox_BVP
            // 
            OutPutTextBox_BVP.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            OutPutTextBox_BVP.BackColor = SystemColors.Window;
            OutPutTextBox_BVP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            OutPutTextBox_BVP.Location = new Point(198, 208);
            OutPutTextBox_BVP.Multiline = true;
            OutPutTextBox_BVP.Name = "OutPutTextBox_BVP";
            OutPutTextBox_BVP.ReadOnly = true;
            OutPutTextBox_BVP.Size = new Size(481, 193);
            OutPutTextBox_BVP.TabIndex = 0;
            // 
            // MainWindow_SMA
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 422);
            Controls.Add(OutPutTextBox_BVP);
            Controls.Add(metroButtonOptions_DIA);
            Controls.Add(metroLabel_DIA);
            Controls.Add(EmergencyStopButton_SMA);
            Controls.Add(numericUpDownInterval_SMA);
            Controls.Add(TimeRemainingLabel_SMA);
            Controls.Add(MonitoringDurationNumeric_SMA);
            Controls.Add(ButtonStartTimer_SMA);
            Controls.Add(lblStatus);
            Controls.Add(lblProcessInfo);
            Controls.Add(buttonOpenDataMon_SMA);
            Controls.Add(buttonChoiceFile);
            ForeColor = Color.Transparent;
            Margin = new Padding(2);
            MinimumSize = new Size(677, 415);
            Name = "MainWindow_SMA";
            Padding = new Padding(18, 69, 18, 18);
            Text = "СМЕРШ";
            ((System.ComponentModel.ISupportInitialize)numericUpDownInterval_SMA).EndInit();
            ((System.ComponentModel.ISupportInitialize)MonitoringDurationNumeric_SMA).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialogTask;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button buttonChoiceFile;
        private System.Windows.Forms.Button buttonOpenDataMon_SMA;
        private System.Windows.Forms.Label lblProcessInfo;
        private System.Windows.Forms.Label lblStatus;
        private MetroFramework.Controls.MetroButton ButtonStartTimer_SMA;
        private System.Windows.Forms.NumericUpDown MonitoringDurationNumeric_SMA;
        private MetroFramework.Controls.MetroLabel TimeRemainingLabel_SMA;
        private System.Windows.Forms.NumericUpDown numericUpDownInterval_SMA;
        private MetroFramework.Controls.MetroTextBox metroTextBoxMetric_SMA;
        private MetroFramework.Controls.MetroButton EmergencyStopButton_SMA;
        private MetroFramework.Controls.MetroLabel metroLabel_DIA;
        private MetroFramework.Controls.MetroButton metroButtonOptions_DIA;
        private TextBox OutPutTextBox_BVP;
    }
}

