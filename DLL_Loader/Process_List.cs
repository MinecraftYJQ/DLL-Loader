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

namespace DLL_Loader
{
    public partial class Process_List : Form
    {
        public Process_List()
        {
            InitializeComponent();
            Process[] process = Process.GetProcesses();

            foreach(Process proc in process)
            {
                listBox1.Items.Add(proc.ProcessName + ".exe");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1.gets_name = listBox1.SelectedItem.ToString();
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            Process[] process = Process.GetProcesses();

            foreach (Process proc in process)
            {
                listBox1.Items.Add(proc.ProcessName+".exe");
            }
        }
    }
}
