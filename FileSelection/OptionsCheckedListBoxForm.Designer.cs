namespace FileSelection
{
    partial class OptionsCheckedListBoxForm
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
            this.checkedListBoxfunction = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // checkedListBoxfunction
            // 
            this.checkedListBoxfunction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxfunction.FormattingEnabled = true;
            this.checkedListBoxfunction.IntegralHeight = false;
            this.checkedListBoxfunction.Items.AddRange(new object[] {
            "Сетевая активность",
            "Файловая активность",
            "Процессы",
            "Нагрузка"});
            this.checkedListBoxfunction.Location = new System.Drawing.Point(0, 0);
            this.checkedListBoxfunction.Margin = new System.Windows.Forms.Padding(0);
            this.checkedListBoxfunction.Name = "checkedListBoxfunction";
            this.checkedListBoxfunction.Size = new System.Drawing.Size(140, 66);
            this.checkedListBoxfunction.TabIndex = 0;
            this.checkedListBoxfunction.MouseUp += new System.Windows.Forms.MouseEventHandler(this.checkedListBoxfunction_MouseUp);
            // 
            // Checkbox
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(140, 66);
            this.Controls.Add(this.checkedListBoxfunction);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Checkbox";
            this.Text = "Параметры";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBoxfunction;
    }
}