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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static string gets_name="";
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process_List process = new Process_List();
            process.ShowDialog();

            textBox2.Text = gets_name;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            gets_name=textBox2.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 4)
            {
                if (textBox1.Text.Substring(textBox1.Text.Length - 4, 4) == ".dll")
                {
                    if (gets_name.Length > 4)
                    {
                        if (gets_name.Substring(gets_name.Length - 4, 4) == ".exe")
                        {
                            ProcessStartInfo startInfo = new ProcessStartInfo();

                            startInfo.FileName = "bin\\InjectDll.exe";
                            startInfo.Arguments = $"\"{textBox2.Text}\" \"{textBox1.Text}\"";

                            startInfo.UseShellExecute = false;
                            startInfo.RedirectStandardOutput = true;
                            using (Process process = Process.Start(startInfo))
                            {
                                process.WaitForExit();
                                string output = process.StandardOutput.ReadToEnd();
                                if(output== "DLL injected successfully.")
                                {
                                    MessageBox.Show("注入成功","提示",MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }else if(output== "Failed to inject DLL.")
                                {
                                    MessageBox.Show("无法注入DLL", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                    MessageBox.Show("未找到进程", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("目标进程不符合标准！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("目标进程不符合标准！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("目标DLL不符合标准！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("目标DLL不符合标准！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "选择dll";
            fileDialog.FileName = "*.dll";
            fileDialog.Filter = "DLL文件|*.dll";

            if(fileDialog.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine("OK");

                textBox1.Text = fileDialog.FileName;
            }
        }
    }
}
