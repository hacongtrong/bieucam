using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace client_Congtrong
{
    public partial class Form1 : Form
    {
        TcpClient client;
        BinaryReader br;
        BinaryWriter bw;
        NetworkStream ns;
        string filename, strImg = " ";
        public Form1()
        {
            InitializeComponent();
        }
        private void btnChonAnh_Click(object sender, EventArgs e)
        {
            //Chuyển ảnh sang nhị phân
            openFileDialog1.ShowDialog();
            string filename = openFileDialog1.FileName;
            pictureBox1.Image = Image.FromFile(filename);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            byte[] imageArray = File.ReadAllBytes(filename);
            strImg = Convert.ToBase64String(imageArray);
        }
        private void btnGuiAnh_Click(object sender, EventArgs e)
        {
            if (client.Connected)
            {
                bw = new BinaryWriter(ns);
                bw.Write(strImg);
                bw.Flush();
                pictureBox1.Image = null;

                br = new BinaryReader(ns);
                MessageBox.Show(br.ReadString());
            }
        }
        private void btnKetNoi_Click(object sender, EventArgs e)
        {
            try
            {
                client = new TcpClient("127.0.0.1", 8000);
                Thread thread = new Thread(Listen);
                thread.Start();
                btnKetNoi.Enabled = false;
            }
            catch
            {
                MessageBox.Show("Không Kết Nối Thành Công");
            }

        }
        private void Listen()
        {
            try
            {
                while (true)
                {
                    ns = client.GetStream();
                    br = new BinaryReader(ns);
                    string chuoi = br.ReadString();

                    if (chuoi[0] == '1')
                        SetTreeView(chuoi.Remove(0, 1));
                    else if (chuoi[0] == '2')
                    {
                        chuoi = chuoi.Remove(0, 1);
                        string[] p = chuoi.Split('_');
                        DateTime localDate = DateTime.Now;
                        string filePath = Application.StartupPath + "\\data\\" + localDate.Millisecond.ToString() + p[1];
                        DeSerialize(filePath, p[0]);
                        axWindowsMediaPlayer1.URL = filePath;
                    }
                    else
                    {
                        addMessage(chuoi);
                    }

                }
            }
            catch
            {
                MessageBox.Show("Lỗi", "ERROR");
            }
        }
        private void addMessage(string mess)
        {
            rtbMessage.BeginInvoke((MethodInvoker)delegate ()
            {
                rtbMessage.Text += mess + "\n";
            });
        }

        private void SetTreeView(string chuoi)
        {
            treeView1.Nodes.Clear();
            string[] listName = chuoi.Split(';');

            foreach (var l in listName)
            {
                treeView1.BeginInvoke((MethodInvoker)delegate ()
                {
                    TreeNode treeNode = new TreeNode("Bài hát: " + l);
                    treeView1.Nodes.Add(treeNode);
                });

            }
        }

        private void DeSerialize(string fileName, string serializedFile)
        {
            using (System.IO.FileStream reader = System.IO.File.Create(fileName))
            {
                byte[] buffer = Convert.FromBase64String(serializedFile);
                reader.Write(buffer, 0, buffer.Length);
            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int i = ("Bài hát: ").Length;
            string name = e.Node.FullPath;
            name = name.Remove(0, i);
            string filePath = Application.StartupPath + "\\data\\" + name;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            BinaryWriter bw = new BinaryWriter(ns);
            bw.Write(name);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string filePath = Application.StartupPath + "\\data\\";
            string[] filePaths = Directory.GetFiles(filePath);
            foreach (var dir in filePaths)
            {
                //filePath += dir;
                if (File.Exists(dir))
                {
                    File.Delete(dir);
                }
            }
        }

       

       

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string str = (txtMessage.Text).Trim();

            if (str != "")
            {
                str = "Client: " + str;
                bw = new BinaryWriter(ns);
                bw.Write(str);
            }
            txtMessage.Clear();
        }

        private void rtbMessage_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSend_Click_1(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
