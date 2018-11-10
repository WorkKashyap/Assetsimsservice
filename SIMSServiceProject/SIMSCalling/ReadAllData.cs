using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml;
using MySql.Data.MySqlClient;
using System.IO;
using System.Security.Cryptography;

namespace SIMSServices
{
    [System.Runtime.InteropServices.Guid("25177377-D7A4-4659-BCB0-7E0C66AA4010")]
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

        public bool timeDaymatch()
        {
            DateTime date = DateTime.Now;
            string dateToday = date.ToString("d");
            DayOfWeek day = DateTime.Now.DayOfWeek;
            string dayToday = day.ToString();

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


            if ((dayToday == DayOfWeek.Friday.ToString()))
            {
                if ((now > start) && (now < startserver))
                {
                     return true;
                }
                else if ((now > end) && (now < endserver))
                {
                     return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }

        }
        public bool timeStartFirstmatch()
        {

            String AMTime;
            String PMTime;

            AMTime = inif.Read("AMStartTime", "AM");
            PMTime = inif.Read("PMEndTime", "PM");

            String[] AMtime = AMTime.Split(':');
            String[] PMtime = PMTime.Split(':');

            TimeSpan start = new TimeSpan(Convert.ToInt16("7"), Convert.ToInt16("00"), 0); 
            TimeSpan now = DateTime.Now.TimeOfDay;

            TimeSpan time1 = TimeSpan.FromMinutes(15);
            TimeSpan startserver = start.Add(time1);
            
            if ((now > start) && (now < startserver))
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

            TimeSpan time1 = TimeSpan.FromHours(1);
            TimeSpan startserver = start.Add(time1);

            TimeSpan endserver = end.Add(time1);


            if ((now > start) && (now < startserver))
            {
                return true;
            }
            else if ((now > end) && (now < endserver))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public string GetSchoolName()
        {
            String Server = inif.Read("SCHOOLNAME", "SCHOOLVALUE");
            return Server;
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
            byte xorConstant1 = 0x53;
            byte[] data1 = Convert.FromBase64String(Pass);
            for (int i = 0; i < data1.Length; i++)
            {
                data1[i] = (byte)(data1[i] ^ xorConstant1);
            }
            string plainText = Encoding.UTF8.GetString(data1);

            return plainText;
        }
        public string GetXmlpath()
        {
          String  xml = inif.Read("Xmlpath", "Xmlpath");
            string value = xml.Substring(xml.Length - 1);
            if (value =="\\")
            {
                return xml;
            }
            else
            {
                return xml + "\\";
            }
         
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

        public string Firstimeload()
        {
            String fs = inif.Read("FirstTimeLoad", "Start");
            return fs;
        }

        public void Firstimewrite()
        {
             inif.Write("FirstTimeLoad", "Start", "OFF");
            
        }


        public DataTable GetAllFiles(string path)
        {
            DataTable FilesTable = new DataTable();
            FilesTable.Columns.Add("FileName");
            FilesTable.Columns.Add("FilePath");
            DataRow dRow;
            string yourPath = path;
            string searchPattern = "*.xml*";
            var resultData = Directory.GetFiles(yourPath, searchPattern, SearchOption.AllDirectories)
                            .Select(x => new { FileName = Path.GetFileName(x), FilePath = x });

            foreach (var item in resultData)
            {
                dRow = FilesTable.NewRow();
                dRow["FileName"] = item.FileName;
                dRow["FilePath"] = item.FilePath;
                FilesTable.Rows.Add(dRow);
            }
            return FilesTable;

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

        public void WriteToFileFirst(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\FirstServiceLog.txt";
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

        public DataTable FinalXmlDtbl()
        {
            DataTable FDB = new DataTable { TableName = "FDBTable" };
            FDB.Columns.Add(new DataColumn("PersonID", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("SessionName", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("AttendanceMark", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("Minute", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("Comments", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("EventStart", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("EventEnd", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("LastUpdated", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("UPN", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("AdmissionNumber", typeof(System.String)));
            FDB.Columns.Add(new DataColumn("school_id", typeof(System.String)));
            return FDB;  
        }

                
    }
}
