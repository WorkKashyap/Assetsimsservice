using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml;
using MySql.Data.MySqlClient;
using System.IO;

namespace Testform
{
    class ReadAllData
    {
        INIFile inif = new INIFile(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Config.ini");
        
   
        public DataTable GetAllStudent()
        {
                DataTable dt;
                XmlDocument sg = new XmlDocument();
                sg = SIMSInterface.Students.GetCurrentStudents();
                XmlReader xr = new XmlNodeReader(sg);
                DataSet ds;
                
                ds = new DataSet();
                ds.ReadXml(xr);
                dt = ds.Tables[0];
                return dt;
            
        }
        public bool timematch()
        {
            String AMTime;
            String PMTime;

            AMTime = inif.Read("AMStartTime", "AM");
            PMTime = inif.Read("PMEndTime", "PM");

            String[] AMtime = AMTime.Split(':');
            String[] PMtime = PMTime.Split(':');

            TimeSpan start = new TimeSpan(Convert.ToInt16(AMtime[0]), Convert.ToInt16(AMtime[1]), 0); //10 o'clock
            TimeSpan end = new TimeSpan(Convert.ToInt16(PMtime[0]), Convert.ToInt16(PMtime[1]), 0);
            TimeSpan now = DateTime.Now.TimeOfDay;

            if ((now > start) && (now < end))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool timeEmatch()
        {
            String AMTime;
            String PMTime;

            AMTime = inif.Read("AMStartTime", "AM");
            PMTime = inif.Read("PMEndTime", "PM");

            String[] AMtime = AMTime.Split(':');
            String[] PMtime = PMTime.Split(':');

            TimeSpan start = new TimeSpan(Convert.ToInt16(AMtime[0]), Convert.ToInt16(AMtime[1]), 0); //10 o'clock
            TimeSpan end = new TimeSpan(Convert.ToInt16(PMtime[0]), Convert.ToInt16(PMtime[1]), 0);
            TimeSpan now = DateTime.Now.TimeOfDay;

            TimeSpan time1 = TimeSpan.FromMinutes(15);
            TimeSpan startserver = start.Add(time1);

            TimeSpan endserver = end.Add(time1);


            if ((start > now) && (now < startserver))
            {
                return true;
            }
            else if ((end > now) && (now < endserver))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public string GetServer()
        {
          String  Server = inif.Read("Server", "Name");
          return Server;
        }
         public string GetDBName()
        {
          String  DBName = inif.Read("DBName", "DName");
          return DBName;
        }
        public string GetUser()
        {
          String  User = inif.Read("UserName", "UName");
          return User;
        }
        public string GetPass()
        {
          String  Pass = inif.Read("Password", "pass");
          return Pass;
        }
        public string GetXmlpath()
        {
          String  xml = inif.Read("Xmlpath", "Xmlpath");
          return xml;
        }
        public string GetAM()
        {
          String  am = inif.Read("AMStartTime", "AM");
          return am;
        }
         public string GetPM()
        {
          String  pm = inif.Read("PMStartTime", "PM");
          return pm;
        }
       

         public void WriteToFile(string Message)
         {
             string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
             if (!Directory.Exists(path))
             {
                 Directory.CreateDirectory(path);
             }
             string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + System.DateTime.Now.Date.ToString("dd/MM/yyyy").Replace('/', '_') + ".txt";
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

        public DataTable FinalDT()
        {
            DataTable FDB = new DataTable { TableName = "FDBTable" };
            FDB.Columns.Add(new DataColumn("PersonID", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("BaseGroupID", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("AttendanceDate", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("SessionName", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("AttendanceMark", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("Minute", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("Comments", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("UPN", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("AdmissionNumber", typeof(System.String)));
            return FDB;  
        }

                
    }
}
