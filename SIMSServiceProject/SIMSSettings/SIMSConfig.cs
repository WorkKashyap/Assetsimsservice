using SIMSSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;


namespace SIMSServiceProject
{
    public partial class SIMSConfig : Form
    {

        SIMSSettings.INIFile inif = new SIMSSettings.INIFile(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini");
        string path = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini";
       
        public SIMSConfig()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {

             
                backgroundWorker1.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            try
            {


             
                String Destination = txtSimspath.Text;
            File.WriteAllText(path, String.Empty);
            String ffile = txtSimspath.Text + "\\logs";
            System.IO.DirectoryInfo di = new DirectoryInfo(ffile);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            if (Destination.Length > 0)
            {
                String exefilename = "\"" + Destination + "\\SIMSServices.exe" + "\"";
                Process.Start(
                @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe ",
                 @" -u " + exefilename + ""
            );

            }
            }
            catch (Exception ex)
            {
                MessageBox.Show("From Uninstall " +ex.Message);
            }


        }
     

        private void SIMSSettings_Load(object sender, EventArgs e)
        {
            try
            {
                // txtPassword.Text = Decrypt(inif.Read("Password", "pass"));


                StopService("SIMSCalling", 500000);

                txtSimspath.Text = SIMSInterface.SIMSLocation.SIMSDotNetDirectory;
                String Server = inif.Read("Server", "Name");
              
                txtServer.Text = inif.Read("Server", "Name");
                txtDBName.Text = inif.Read("DBName", "DName");
                txtUser.Text = inif.Read("UserName", "UName");

                String Pass = inif.Read("Password", "pass");
                byte xorConstant1 = 0x53;
                byte[] data1 = Convert.FromBase64String(Pass);
                for (int i = 0; i < data1.Length; i++)
                {
                    data1[i] = (byte)(data1[i] ^ xorConstant1);
                }
                string plainText = Encoding.UTF8.GetString(data1);

                txtPassword.Text = plainText;
                //txtxmlpath.Text = inif.Read("Xmlpath", "Xmlpath");
                cmbamtime.Text = inif.Read("AMStartTime", "AM");
                cmbpmtime.Text = inif.Read("PMEndTime", "PM");
                String Destination = inif.Read("FirstTimeLoad", "Start");


              
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

       

        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                RestartService("SIMSCalling", 500000);

            }
            catch(Exception ex)
            {
                MessageBox.Show("From Stop service " + ex.Message);
            }
             


        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {  try
            {
                if(txtServer.Text.Length >0 && txtPassword.Text.Length > 0)

                inif.Write("Server", "Name", txtServer.Text);
                inif.Write("DBName", "DName", txtDBName.Text);
                inif.Write("UserName", "UName", txtUser.Text);
                //inif.Write("Xmlpath", "Xmlpath", txtxmlpath.Text);
                inif.Write("AMStartTime", "AM", cmbamtime.Text);
                inif.Write("PMEndTime", "PM", cmbpmtime.Text);
                inif.Write("SIMSPATH", "SIMSVALUE", txtSimspath.Text);



                string input = txtPassword.Text;
                byte xorConstant = 0x53;
                byte[] data = Encoding.UTF8.GetBytes(input);
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = (byte)(data[i] ^ xorConstant);
                }
                string output = Convert.ToBase64String(data);

                inif.Writepass("Password", "pass", output);

                button3.Enabled = true;
               // MessageBox.Show("ASSET settings saved successfully");


                String Destination = inif.Read("SIMSPATH", "SIMSVALUE");
            if (Destination.Length > 0)
            {
                    if (!Directory.Exists(Destination + "/config"))
                    {
                        Directory.CreateDirectory(Destination + "/config");
                    }
                    String exefilename = "\"" + Destination + "\\SIMSServices.exe" + "\"";
                String CurrentPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                foreach (string newPath in Directory.GetFiles(CurrentPath, "*.*",
                SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(CurrentPath, Destination), true);

                    DirectoryInfo dInfo = new DirectoryInfo(Destination);
                    DirectorySecurity dSecurity = dInfo.GetAccessControl();
                    dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    dInfo.SetAccessControl(dSecurity);

            }
            }
            catch (Exception ex)
            {
                MessageBox.Show("From Start Service " + ex.Message); 
            }


        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1_Click(sender, e);

            MessageBox.Show("ASSET settings saved successfully");
        }

        private void SIMSSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
           
        }

        public static void RestartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

             
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch
            {
                // ...
            }
        }

        public static void StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
              
            }
            catch
            {
                // ...
            }
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {


                button1_Click(sender, e);
                button2_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

      
    }
}
