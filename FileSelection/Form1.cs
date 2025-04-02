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
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialogTask.ShowDialog();
            string openFilePath = openFileDialogTask.FileName;
            button2.Enabled = true;
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
    }
}
