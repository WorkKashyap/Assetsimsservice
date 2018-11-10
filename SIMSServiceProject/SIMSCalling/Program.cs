using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Globalization;
using System.Xml;
using System.Diagnostics;
using System.Configuration.Install;
using System.Reflection;
using System.Windows.Forms;

namespace SIMSServices
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///    XmlDocument student = new XmlDocument();
        ///    

        [STAThread]
        static void Main(String[] args)
        {
            INIFile inif = new INIFile(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini");
            // ExecuteCommand("SImsServices.exe -install");
           // inif.Write("Server", "Name", "");
            String Server = inif.Read("Server", "Name");
            if (Server.Length ==0)
            {
                //SIMSServices.SplashScreen frm1 = new SIMSServices.SplashScreen();
                //frm1.ShowDialog();
                Application.Run(new SplashScreen());

            }
            else
            { 

            SIMSInterface.SIMSDllResolution.AddSIMSDllResolution();

                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                 {
                new Service1()
                 };
                ServiceBase.Run(ServicesToRun);
            }

        }

    }
}