namespace FileSelection
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.openFileDialogTask = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonChoiceFile = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.lblProcessInfo = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.checkedListBoxfunction = new System.Windows.Forms.CheckedListBox();
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
            this.toolTip1.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip1_Popup);
            // 
            // buttonChoiceFile
            // 
            this.buttonChoiceFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonChoiceFile.Image = ((System.Drawing.Image)(resources.GetObject("buttonChoiceFile.Image")));
            this.buttonChoiceFile.Location = new System.Drawing.Point(13, 83);
            this.buttonChoiceFile.Name = "buttonChoiceFile";
            this.buttonChoiceFile.Size = new System.Drawing.Size(91, 61);
            this.buttonChoiceFile.TabIndex = 0;
            this.toolTip1.SetToolTip(this.buttonChoiceFile, "выберете файл");
            this.buttonChoiceFile.UseVisualStyleBackColor = true;
            this.buttonChoiceFile.Click += new System.EventHandler(this.buttonChoiceFile_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.Location = new System.Drawing.Point(110, 83);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(91, 61);
            this.button2.TabIndex = 1;
            this.toolTip1.SetToolTip(this.button2, "Перейти к выбору мониторинга");
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lblProcessInfo
            // 
            this.lblProcessInfo.AutoSize = true;
            this.lblProcessInfo.Location = new System.Drawing.Point(23, 222);
            this.lblProcessInfo.Name = "lblProcessInfo";
            this.lblProcessInfo.Size = new System.Drawing.Size(0, 16);
            this.lblProcessInfo.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(23, 277);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 16);
            this.lblStatus.TabIndex = 3;
            // 
            // checkedListBoxfunction
            // 
            this.checkedListBoxfunction.FormattingEnabled = true;
            this.checkedListBoxfunction.Items.AddRange(new object[] {
            "Сетевая активность",
            "Файловая активность",
            "Процессы",
            "Нагрузка"});
            this.checkedListBoxfunction.Location = new System.Drawing.Point(13, 250);
            this.checkedListBoxfunction.Name = "checkedListBoxfunction";
            this.checkedListBoxfunction.Size = new System.Drawing.Size(207, 89);
            this.checkedListBoxfunction.TabIndex = 5;
            this.checkedListBoxfunction.SelectedIndexChanged += new System.EventHandler(this.checkedListBoxfunction_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.checkedListBoxfunction);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblProcessInfo);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buttonChoiceFile);
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Name = "Form1";
            this.Text = "СМЕРШ";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialogTask;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button buttonChoiceFile;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label lblProcessInfo;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckedListBox checkedListBoxfunction;
    }
}

