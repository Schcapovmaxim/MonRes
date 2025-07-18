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
            checkedListBoxfunction = new CheckedListBox();
            SuspendLayout();
            // 
            // checkedListBoxfunction
            // 
            checkedListBoxfunction.Dock = DockStyle.Fill;
            checkedListBoxfunction.FormattingEnabled = true;
            checkedListBoxfunction.IntegralHeight = false;
            checkedListBoxfunction.Items.AddRange(new object[] { "Сетевая активность", "Файловая активность", "Процессы", "Нагрузка", "Поиск дочерних процессов" });
            checkedListBoxfunction.Location = new Point(0, 0);
            checkedListBoxfunction.Margin = new Padding(0);
            checkedListBoxfunction.Name = "checkedListBoxfunction";
            checkedListBoxfunction.SelectionMode = SelectionMode.None;
            checkedListBoxfunction.Size = new Size(254, 208);
            checkedListBoxfunction.TabIndex = 0;
            checkedListBoxfunction.MouseUp += checkedListBoxfunction_MouseUp;
            // 
            // OptionsCheckedListBoxForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(254, 208);
            Controls.Add(checkedListBoxfunction);
            FormBorderStyle = FormBorderStyle.None;
            Name = "OptionsCheckedListBoxForm";
            Text = "Параметры";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBoxfunction;
    }
}