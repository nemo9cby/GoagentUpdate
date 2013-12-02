using sherlock99.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GoagentUpdate
{
    public partial class Form1 : Form
    {
        bool isNewInstall = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartInfo();
            CheckEnvironment();
        }

        #region UI组件

        // 写入log记录
        private void ShowMessage(string str)
        {
            string msg = str + "\r\n";
            textBox1.AppendText(msg);
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }

        // 浏览按钮
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result.Equals(DialogResult.OK))
            {
                comboBox1.Text = folderBrowserDialog1.SelectedPath;
            }
            if (!comboBox1.Text.ToLower().Contains("goagent"))
            {
                comboBox1.Text = "这可能不是Goagent文件夹";
                button1.Enabled = false;
            }
        }

        // 设置路径之后 执行按钮可选
        private void comboBox1_TextChanged(object sender, System.EventArgs e)
        {
            if (comboBox1.Text.ToLower().Contains("goagent") && !comboBox1.Text.Contains("存在"))
            {
                if (File.Exists(comboBox1.Text + @"\is99created.txt"))
                {
                    ShowMessage("");
                    ShowMessage("当前选择的目录Goagent信息:");
                    string filecontent = FileHelper.Read(comboBox1.Text + @"\is99created.txt");
                    ShowMessage(filecontent);
                    ShowMessage("");
                }
                else
                {
                    if (!isNewInstall)
                    {
                        ShowMessage("这可能不是99创建的文件夹，请确认");
                    }
                }
                button1.Enabled = true;
            }
        }

        // 开始执行按钮
        private void button1_Click(object sender, EventArgs e)
        {
            UpdateGoagent();
        }

        // 拷贝日志按钮
        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
            textBox1.Copy();
            label2.Visible = true;
        }

        // 日志改变时自动取消 已拷贝字样
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label2.Visible = false;
        }

        #endregion

        #region 启动信息

        // 启动信息
        private void StartInfo()
        {
            ShowMessage("*********************************");
            ShowMessage("99的Goagent自动部署程序");
            ShowMessage("仅限我的小伙伴们使用，请勿外传");
            ShowMessage("使用中遇到问题，请联系我");
            ShowMessage("*********************************");
            ShowMessage("Sherlock99  All Rights Reserved");
            ShowMessage("");
            ShowMessage("安装包信息:");
            string filecontent = FileHelper.Read(@"Goagent\is99created.txt");
            ShowMessage(filecontent);
            ShowMessage("");
        }

        #endregion

        #region 检查部署环境

        // 检查环境
        private void CheckEnvironment()
        {
            if (Exist360())
            {
                ShowMessage("360系列软件会阻止自动更新程序的正常运行...");
                ShowMessage("退出所有360软件后重试");
                ShowMessage("操作终止");
                return;
            }
            if (!CheckSource())
            {
                ShowMessage("未检测到原始Goagent文件夹");
                ShowMessage("操作终止");
                //return;
            }

            List<string> paths = GetAllGoagentDirectories();
            // 全新安装
            if (paths.Count == 0)
            {
                isNewInstall = true;
                ShowMessage("未在各磁盘根目录下找到Goagent文件夹");
                ShowMessage("将使用默认路径");
                if (Directory.Exists(@"D:\"))
                {
                    comboBox1.Text = @"D:\Goagent";
                }
                else if (Directory.Exists(@"C:\Program Files (x86)"))
                {
                    comboBox1.Text = @"C:\Program Files (x86)\Goagent";
                }
                else
                {
                    comboBox1.Text = @"C:\Program Files\Goagent";
                }
            }
            // 已经存在Goagent
            else
            {
                comboBox1.Items.AddRange(paths.ToArray());
                if (paths.Count == 1)
                {
                    comboBox1.SelectedIndex = 0;
                }
                else
                {
                    comboBox1.Text = "存在多个Goagent路径，请选择正确的路径";
                    ShowMessage("存在多个Goagent路径，请选择正确的路径");
                    button1.Enabled = false;
                }
            }
        }

        // 检测360是否存在
        private bool Exist360()
        {
            bool flag = false;
            string Name360 = "";
            string[] msg = CmdHelper.ExecCommand("tasklist /NH /FO csv");
            string[] tmp = msg[0].Split(',');
            foreach (string str in tmp)
            {
                if (str.ToLower().Contains("360se"))
                {
                    flag = true;
                    Name360 += "360安全浏览器" + "\r\n";
                }
                if (str.ToLower().Contains("360tray"))
                {
                    flag = true;
                    Name360 += "360安全卫士" + "\r\n";
                }
                if (str.ToLower().Contains("360sd"))
                {
                    flag = true;
                    Name360 += "360杀毒" + "\r\n";
                }
                if (str.ToLower().Contains("360rp"))
                {
                    flag = true;
                    Name360 += "360杀毒软件" + "\r\n";
                }
            }
            if (flag == true)
            {
                ShowMessage("检测到以下360软件");
                ShowMessage(Name360);
            }
            return flag;
        }

        // 检测解压出源目录
        private bool CheckSource()
        {
            if (Directory.Exists("Goagent"))
            {
                return true;
            }
            else
                return false;
        }

        // 查找所有Goagent路径
        private List<string> GetAllGoagentDirectories()
        {
            List<string> paths = new List<string>();
            string[] LogicalDrives = Directory.GetLogicalDrives();
            List<string> checkPaths = new List<string>();
            checkPaths.AddRange(LogicalDrives);
            checkPaths.Add(@"C:\Program Files (x86)");
            checkPaths.Add(@"C:\Program Files");
            try
            {
                foreach (string path in checkPaths)
                {
                    if (Directory.Exists(path))
                    {
                        string[] fileList = Directory.GetDirectories(path);

                        foreach (string file in fileList)
                        {
                            if (file.ToLower().Contains("goagent"))
                            {
                                paths.Add(file);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.ToString());
            }
            return paths;
        }

        #endregion

        #region 执行部署操作
        // 更新流程
        private void UpdateGoagent()
        {
            string GoagentPath = comboBox1.Text;
            if (isNewInstall)
            {
                Directory.CreateDirectory(GoagentPath);
            }
            if (!Directory.Exists(GoagentPath))
            {
                ShowMessage("路径 " + GoagentPath + " 不存在");
                ShowMessage("可能需要手工创建这个目录");
                ShowMessage("操作终止");
                return;
            }

            ShowMessage("");
            ShowMessage("开始结束进程");
            if (KillTask())
            {
                ShowMessage("结束进程完成");
            }
            else
            {
                ShowMessage("结束进程失败 请联系99");
                ShowMessage("操作终止");
                return;
            }

            ShowMessage("");
            ShowMessage("开始拷贝文件");

            // 删除文件
            try
            {
                FileHelper.Delete(GoagentPath);
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }

            int CopyFlag = 0;
            // 拷贝文件
            try
            {
                CopyFlag = FileHelper.Copy("Goagent", GoagentPath);
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }

            if (CopyFlag == 1)
            {
                ShowMessage("文件拷贝完成");
                ShowMessage("");
            }
            else
            {
                ShowMessage("文件拷贝失败: " + CopyFlag + "请联系99");
                ShowMessage("操作终止");
                return;
            }

            System.Threading.Thread.Sleep(250);

            // 设置开机自动启动
            try
            {
                if (checkBox1.Checked)
                {
                    CmdHelper.ExecCommand(string.Format("wscript {0}\\addto-startup.js {0}", GoagentPath));
                    ShowMessage("设置开机自动启动成功");
                }
                else
                {
                    CmdHelper.ExecCommand(string.Format("wscript {0}\\delete-startup.js", GoagentPath));
                    ShowMessage("删除开机自动启动成功");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("设置开机自动启动失败");
                ShowMessage(ex.Message);
            }

            // 启动Goagent
            try
            {
                Start(GoagentPath);
                ShowMessage("启动Goagent成功");
            }
            catch (Exception ex)
            {
                ShowMessage("启动Goagent失败");
                ShowMessage(ex.Message);
            }

            if (isNewInstall)
            {
                ShowMessage("");
                ShowMessage("Goagent客户端部署完成");
                ShowMessage("请联系99 协助安装浏览器插件");
            }

        }

        // 结束Goagent进程
        private bool KillTask()
        {
            if (Kill("goagent.exe") && Kill("python27.exe"))
                return true;
            else
            {
                return false;
            }

        }

        // 根据进程名结束进程
        private bool Kill(string processName)
        {
            bool flag = false;
            try
            {
                string[] msg = CmdHelper.ExecCommand(string.Format(@"tasklist /FI ""IMAGENAME eq {0}"" /NH /FO CSV", processName));
                // 查询出错
                if (msg[1].Length > 0)
                {
                    ShowMessage("查询" + processName + "PID出错，请联系99");
                    return flag;
                }
                // 进程未运行
                if (msg[0].Contains("没有运行"))
                {
                    ShowMessage(processName + "没有运行，自动跳过");
                    flag = true;
                    return flag;
                }

                string[] temp = msg[0].Split(',');
                string[] msg2 = CmdHelper.ExecCommand(string.Format(@"taskkill -pid {0} -f", temp[1]));
                // 结束进程成功
                if (msg2[1].Length == 0 && msg2[0].Contains("成功"))
                {
                    ShowMessage("结束" + processName + "成功");
                    flag = true;
                    return flag;
                }
                // 结束进程失败
                else
                {
                    ShowMessage("结束" + processName + "失败，请联系99");
                    return flag;
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.ToString());
                return flag;
            }
        }

        // 启动Goagent.exe
        private void Start(string GoagentPath)
        {
            if (!Directory.Exists(GoagentPath))
            {
                ShowMessage("Goagent文件不存在 启动失败");
            }
            else
            {
                Process proc = new Process();
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                proc.StandardInput.WriteLine(string.Format("{0}\\goagent.exe", GoagentPath));
                System.Threading.Thread.Sleep(250);
                proc.Close();
                ShowMessage("Goagent启动完成");
            }
        }

        #endregion
    }
}
