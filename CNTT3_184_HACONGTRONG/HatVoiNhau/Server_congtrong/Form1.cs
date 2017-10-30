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


namespace Server_congtrong
{

    public partial class Form1 : Form
    {
        TcpListener server;
        TcpClient client;
        Thread thread;
        BinaryReader br;
        BinaryWriter bw;
        NetworkStream ns;
        List<TcpClient> ListClient;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txtMessage.Text != "")
            {
                string str = "Server: " + txtMessage.Text;
                addMessage(str);
                foreach (TcpClient item in ListClient)
                {
                    Send(item, str);
                }
                txtMessage.Clear();
            }




        }
        private void btnKetNoi_Click(object sender, EventArgs e)
        {
            ListClient = new List<TcpClient>();
            server = new TcpListener(IPAddress.Any, 8000);
            server.Start();
            MessageBox.Show("Mở Máy Chủ Thành Công");
            thread = new Thread(Listen);
            thread.Start();
            btnKetNoi.Enabled = false;
        }

        private void Listen()
        {
            try
            {
                while (true)
                {
                    client = server.AcceptTcpClient();
                    ListClient.Add(client);
                    MessageBox.Show("Có Thiết Bị Kết Nối");

                    ns = client.GetStream();
                    bw = new BinaryWriter(ns);
                    bw.Write("1" + Scan());

                    new ClientThread(client, this);
                }
            }
            catch
            {
                MessageBox.Show("Error", "Lỗi");
            }
        }

        private string Scan()
        {
            string result = "";
            string filePath = Application.StartupPath + "\\data\\";
            string[] filePaths = Directory.GetFiles(filePath);
            foreach (var dir in filePaths)
            {
                string[] p = dir.Split('\\');
                string duoiF = p[p.Count() - 1];

                result += duoiF + ";";
            }
            if (result.Length > 2)
            {
                result = result.Remove(result.Length - 1);
            }

            return result;
        }

        class ClientThread
        {
            private TcpClient client;
            private Form1 form1;

            public ClientThread(TcpClient client, Form1 form1)
            {
                // TODO: Complete member initialization
                this.client = client;
                this.form1 = form1;
                Thread thread = new Thread(XuLy);
                thread.Start();
            }

            private void XuLy()
            {
                try
                {
                    while (true)
                    {

                        //break;
                        NetworkStream ns = client.GetStream();
                        BinaryReader br = new BinaryReader(ns);
                        BinaryWriter bw = new BinaryWriter(ns);
                        string chuoi = br.ReadString();
                        form1.addMessage(chuoi);
                        form1.SendClient(chuoi);

                        string filePath = Application.StartupPath + "\\data\\" + chuoi;
                        //MessageBox.Show(filePath);
                        bw.Write("2" + Serialize(filePath) + "_" + chuoi);
                        //bw.Write("2"+filePath);


                    }
                }
                catch
                {

                }
            }

            private string Serialize(string fileName)
            {
                using (FileStream reader = new FileStream(fileName, FileMode.Open))
                {
                    byte[] buffer = new byte[reader.Length];
                    reader.Read(buffer, 0, (int)reader.Length);
                    return Convert.ToBase64String(buffer);
                }
            }

        }

        

        private void Send(TcpClient client, string mess)
        {
            ns = client.GetStream();
            bw = new BinaryWriter(ns);
            bw.Write(mess);
        }
        private void SendClient(string mess)
        {
            foreach (TcpClient item in ListClient)
            {
                ns = item.GetStream();
                bw = new BinaryWriter(ns);
                bw.Write(mess);
            }
        }

        private void addMessage(string s)
        {
            rtbMessage.BeginInvoke((MethodInvoker)delegate ()
            {
                rtbMessage.Text += s + "\n";
            });
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDung_Click(object sender, EventArgs e)
        {
            client.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ListClient = new List<TcpClient>();
            server = new TcpListener(IPAddress.Any, 8000);
            server.Start();
            MessageBox.Show("Mở Máy Chủ Thành Công");
            thread = new Thread(Listen);
            thread.Start();
            btnKetNoi.Enabled = false;
        }
    }
}
