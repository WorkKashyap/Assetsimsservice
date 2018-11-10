using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestConnection;

namespace Testform
{
    public partial class Connection : Form
    {
        ReadAllData readAll = new ReadAllData();
        String MyConnection2;
        MySqlRead Msql;
        MySqlConnection MyConn2;
        String emaildir = AppDomain.CurrentDomain.BaseDirectory + "logs\\EmailLog.txt";
        public Connection()
        {
            InitializeComponent();
        }

        private void Connection_Load(object sender, EventArgs e)
        {

            IPAddress IP;
            if (IPAddress.TryParse("173.194.108.30", out IP))
            {
                Socket s = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

                try
                {
                    s.Connect(IP, 3306);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection failed with 173.194.108.30 " + ex.Message);

                }
            }
        }

        public string GetFirstmatchupn()
        {
            try
            {

            
             Msql = new MySqlRead();
            MyConnection2 = Msql.connectionString();
            MyConn2 = new MySqlConnection("datasource=" + Msql.GetServerName() + ";port=3306;username=" + Msql.UserName() + ";password=" + Msql.Password() + ";SslMode=none");
            MyConn2.Open();
           
                String Query = "select a.upn, a.school_id, a.adno from  " + Msql.GetDBName() + "." + "virtual_upn a , " + Msql.GetDBName() + "." + "pupils_unique b where (a.upn =b.upn And  sims_flag IS NULL) OR  (a.adno=b.adno And  sims_flag IS NULL);";
            MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
            MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
            MyAdapter.SelectCommand = MyCommand2;
            DataTable dtUpnmatch = new DataTable();
            MyAdapter.Fill(dtUpnmatch);
                MyConn2.Close();

                return "true";

            }
            catch (Exception ex)
            {
                return "false " + ex.Message;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                SIMSInterface.SIMSDllResolution.AddSIMSDllResolution();
                if (SIMSInterface.LoginHelper.SIMSlogin(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text))
                {
                    MessageBox.Show("SIMS Connection successfully");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void DeleteToEmailLog(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\EmailLog.txt";
            File.Create(filepath).Close();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                CheckConnection("test");

          //  DataTable DT = GetFirstmatchupn();
          //MessageBox.Show("Number of Records " + DT.Rows.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void WriteToEmailLog(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\EmailLog.txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   

                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
        protected void SendEmail(string status, string dfe, string message)
        {
            ReadAllData readAll = new ReadAllData();
            var fileName = emaildir;
            if(!File.Exists(fileName))
            {
                DeleteToEmailLog("");
            }
            FileInfo fi = new FileInfo(fileName);
           

            var size = fi.Length;
            if (size == 0)
            {
                string schoolname = "test";
                Testform.Email e = new Email();

                string body = e.createEmailBody(status, dfe, schoolname, "Other Information", message);

                e.SendHtmlFormattedEmail("ASSET SIMS Service - " + schoolname, body);
                WriteToEmailLog(message);
            }
        }
        private void CheckConnection(string dfe)
        {
        
            string connectionstatus = GetFirstmatchupn();
            if (connectionstatus.Contains("false"))
            {
                SendEmail("server", dfe, connectionstatus);
            }
        }
    }
}
