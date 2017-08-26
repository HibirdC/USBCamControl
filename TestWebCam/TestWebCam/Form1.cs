using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Webcam;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private webCamHelper CamHelper;
        public Form1()
        {
            InitializeComponent();
            CamHelper = new webCamHelper(ref this.pictureBox1, 640, 480);
            loadDeviceList();
        }
        private void openBtn_Click(object sender, EventArgs e)
        {
            string device = comboBox1.Text;
            if(device.Equals(""))
            {
                MessageBox.Show("请选择一个视频输入设备");
                return;
            }
            CamHelper.Open(device);
        }
        void loadDeviceList()
        {
            Dictionary<string, string> deviceList = new Dictionary<string,string>();
            CamHelper.GetCamList(ref deviceList);
            foreach(var item in deviceList)
            {
                comboBox1.Items.Add(item.Key);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void 关闭_Click(object sender, EventArgs e)
        {
            CamHelper.Close();
        }

        private void captureBtn_Click(object sender, EventArgs e)
        {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Capture\"+123+".jpg");
            if (true == CamHelper.Capture(file))
            {
                MessageBox.Show("拍照成功");
            }
        }

        private void Close(object sender, FormClosingEventArgs e)
        {
            if(CamHelper.isRunning())
            {
                CamHelper.Close();
            }
        }
    }
}
