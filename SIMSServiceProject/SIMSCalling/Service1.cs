using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using MySql.Data.MySqlClient;




namespace SIMSServices
{
    public partial class Service1 : ServiceBase
    {
        int totins = 0;
        int totups = 0;
        String AM = "";
        String PM = "";
        Boolean AMflag =false;
        Boolean PMflag = false;
        String PID = "";
        String VDID = "";
        String basedir = AppDomain.CurrentDomain.BaseDirectory + "logs\\FirstServiceLog.txt";
        String emaildir = AppDomain.CurrentDomain.BaseDirectory + "logs\\EmailLog.txt";
        String FileName="";
        ReadAllData readAll;
        DataTable StudentDT;
        DataTable matchUPN;
        DataTable dtstudent = new DataTable();
        DataTable dTpupil;
        DataTable dTattdate;
        DataTable virtual_pup_attendance;
        MySqlRead Msql;
        MySqlConnection MyConn2;
        XmlDocument student = new XmlDocument();
        DataSet dsstudent;
        String MyConnection2;
        INIFile inif = new INIFile(AppDomain.CurrentDomain.BaseDirectory + "config\\Config.ini");
        Attendance Att;
        string connectionstatus;
        DataTable FDB;

        public Service1()
        {
            InitializeComponent();
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

        protected override void OnStart(string[] args)
        {
            Timer timer = new Timer(); // name space(using System.Timers;)  
                                       // SIMS DLLS ARE STRONG NAMED AND THE APPLICATION WILL FAIL WITHOUT THIS CALL
            WriteToFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 300000; //number in milisecinds  
            timer.Enabled = true;
              
        }

        protected void SendEmail(string status,string dfe,string message)
        {

            ReadAllData readAll = new ReadAllData();
            var fileName = emaildir;
            if (!File.Exists(fileName))
            {
                readAll.DeleteToEmailLog("");
            }
            FileInfo fi = new FileInfo(fileName);

            var size = fi.Length;
            if(size == 0)
            {
                string schoolname = readAll.GetSchoolName();
                SIMSServices.Email e = new Email();

                string body = e.createEmailBody(status, dfe,schoolname, "Other Information", message);

                e.SendHtmlFormattedEmail("ASSET SIMS Service - " + schoolname, body);
                readAll.WriteToEmailLog(message);
            }
        }
            

            //calling for creating the email body with html template   
            

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
                backgroundWorker1.RunWorkerAsync();
               // WriteToFile("Service is recall at " + DateTime.Now);
        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped " + DateTime.Now);
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

          

            GetBeforeFirstTimeAttendanceData();
            System.Threading.Thread.Sleep(1000);
            GetFirstTimeAttendanceData();
            System.Threading.Thread.Sleep(1000);
            GetAttendanceData();
         
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // WriteToFile("Records are updated on the server" + DateTime.Now);
         
        }

        private void CheckConnection(string dfe)
        {
            Attendance ad = new Attendance();
            connectionstatus = ad.CheckConnectionServer();
            if (connectionstatus.Contains("false"))
            {
                SendEmail("server", dfe, connectionstatus);
            }
        }
        private void GetBeforeFirstTimeAttendanceData()
        {
            try
            {

                ReadAllData readAll = new ReadAllData();
                Attendance Att = new Attendance();
                DataTable Dtblupn = new DataTable();
                if (!File.Exists(basedir))
                {

                        SIMSInterface.SIMSDllResolution.AddSIMSDllResolution();

                        if (SIMSInterface.LoginHelper.SIMSlogin(readAll.GetServer(), readAll.GetDBName(), readAll.GetUser(), readAll.GetPass()))
                        {
                            string dfe = SIMSInterface.School.DCSFNumber();
                            CheckConnection(dfe);
                            Dtblupn = Att.GetFirstmatchupn();
                            if (Dtblupn.Rows.Count > 0)
                            {


                            WriteToFile("Login Successfully " + DateTime.Now);
                            System.Threading.Thread.Sleep(1000);
                            FDB = new DataTable { TableName = "FDBTable" };
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
                            StudentDT = readAll.GetAllStudent();
                            
                            // Match UPN LOGIC ////
                            DataRow[] rowval;
                            matchUPN = Dtblupn;
                            DataTable STUD = new DataTable { TableName = "STUD" };
                            STUD.Columns.Add(new DataColumn("StudentId", typeof(System.String)));
                            STUD.Columns.Add(new DataColumn("upn", typeof(System.String)));
                            STUD.Columns.Add(new DataColumn("AdmissionNumber", typeof(System.String)));
                            STUD.Columns.Add(new DataColumn("school_id", typeof(System.String)));

                            for (int u = 0; u < matchUPN.Rows.Count; u++)
                            {
                                if (matchUPN.Rows[u][0].ToString().Trim().Length > 0)
                                {
                                    rowval = StudentDT.Select("UPN = '" + matchUPN.Rows[u][0].ToString().Trim() + "'");

                                }
                                else
                                {
                                    rowval = StudentDT.Select("AdmissionNumber = '" + matchUPN.Rows[u][2].ToString().Trim() + "'");
                                }
                                foreach (DataRow row in rowval)
                                {
                                    DataRow dr = STUD.NewRow();
                                    dr[0] = row[0];
                                    dr[1] = row[13];
                                    dr[2] = row[12];
                                    dr[3] = matchUPN.Rows[u][1].ToString().Trim();
                                    STUD.Rows.Add(dr);

                                }
                            }
                            STUD.AcceptChanges();
                            for (int i = 0; i < STUD.Rows.Count; i++)
                            {
                                string Stime = "00:00:00";
                                String Sdate = "2018-09-01";
                                String StartDate = Sdate + "T" + Stime;

                                string Etime = DateTime.Now.ToString("HH:mm:ss");
                                String Edate = DateTime.Now.ToString("yyyy-MM-dd");
                                String EndDate = Edate + "T" + Etime;

                                student = SIMSInterface.Attendance.GetAttendanceChanges(StartDate, EndDate);
                                XmlReader xr1 = new XmlNodeReader(student);
                                dsstudent = new DataSet();
                                dsstudent.ReadXml(xr1);
                                if (dsstudent.Tables.Count > 0)
                                {

                                    dtstudent = dsstudent.Tables[0];

                                    for (int j = 0; j < dtstudent.Rows.Count; j++)
                                    {
                                        DataRow dr = FDB.NewRow();
                                        String XMLPersonID = dtstudent.Rows[j][0].ToString();
                                        String SIMSPersonID = STUD.Rows[i][0].ToString();
                                        if (XMLPersonID.Equals(SIMSPersonID))
                                        {
                                            dr[0] = dtstudent.Rows[j][0].ToString();
                                            dr[1] = dtstudent.Rows[j][1].ToString();
                                            dr[2] = dtstudent.Rows[j][2].ToString();
                                            dr[3] = dtstudent.Rows[j][3].ToString();
                                            dr[4] = dtstudent.Rows[j][4].ToString(); //comments

                                            dr[5] = dtstudent.Rows[j][6].ToString();
                                            dr[6] = dtstudent.Rows[j][7].ToString();
                                            dr[7] = dtstudent.Rows[j][8].ToString(); //last updated

                                            dr[8] = STUD.Rows[i][1].ToString();
                                            dr[9] = STUD.Rows[i][2].ToString();
                                            dr[10] = STUD.Rows[i][3].ToString();

                                            FDB.Rows.Add(dr);
                                        }
                                    }

                                }


                            }
                            FDB.AcceptChanges();
                            //string result;
                            string time = DateTime.Now.ToString("HH:mm");
                            time = time.Replace(":", "_");
                            if (FDB.Rows.Count > 0)
                            {
                                readAll.WriteToFileFirst("First log created..");
                                //using (StringWriter sw = new StringWriter())
                                //{
                                //    FDB.WriteXml(sw);
                                //    result = sw.ToString();
                                //    FileName = readAll.GetXmlpath() + "stud_attendance_" + time + "_01_09_2018" + ".xml";
                                //    File.WriteAllText(FileName, result);
                                //    WriteToFile("File Created.." + DateTime.Now);
                                //    System.Threading.Thread.Sleep(1000);
                                //}
                                FileName = FDB.Rows.Count.ToString();
                            }
                            // }
                        }
                        if (FileName!= "")
                        {
                           updateFirstAttendanceOnServer();
                        }
                    }
                    else
                    {
                        SendEmail("login","","SIMS login failed.. Authentication problem");
                    }
                }


            }
            catch (Exception ex)
            {
                SendEmail("server", SIMSInterface.School.DCSFNumber(),"Error due to " + ex.Message);
                WriteToFile(ex.Message + "--" + DateTime.Now);

            }

        }

        private void GetAttendanceData()
        {
            try
            {
                ReadAllData readAll = new ReadAllData();
                Attendance Att = new Attendance();
                if (readAll.timeEmatch() == true)
                {

                   
                  
                    if (SIMSInterface.LoginHelper.SIMSlogin(readAll.GetServer(), readAll.GetDBName(), readAll.GetUser(), readAll.GetPass()))
                    {

                        WriteToFile("Login Successfully " + DateTime.Now);
                        string dfe = SIMSInterface.School.DCSFNumber();
                        CheckConnection(dfe);
                        System.Threading.Thread.Sleep(1000);
                         FDB = new DataTable { TableName = "FDBTable" };
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
                        StudentDT = readAll.GetAllStudent();
                        // Match UPN LOGIC ////
                        DataRow[] rowval;
                        matchUPN = Att.Getmatchupn();
                        DataTable STUD = new DataTable { TableName = "STUD" };
                        STUD.Columns.Add(new DataColumn("StudentId", typeof(System.String)));
                        STUD.Columns.Add(new DataColumn("upn", typeof(System.String)));
                        STUD.Columns.Add(new DataColumn("AdmissionNumber", typeof(System.String)));
                        STUD.Columns.Add(new DataColumn("school_id", typeof(System.String)));


                        for (int u = 0; u < matchUPN.Rows.Count; u++)
                        {
                            if (matchUPN.Rows[u][0].ToString().Trim().Length >0)
                            {
                                rowval = StudentDT.Select("UPN = '" + matchUPN.Rows[u][0].ToString().Trim() + "'");

                            }
                            else
                            {
                                rowval = StudentDT.Select("AdmissionNumber = '" + matchUPN.Rows[u][2].ToString().Trim() + "'");
                            }
                            foreach (DataRow row in rowval)
                            {
                                DataRow dr = STUD.NewRow();
                                dr[0] = row[0];
                                dr[1] = row[13];
                                dr[2] = row[12];
                                dr[3] = matchUPN.Rows[u][1].ToString().Trim();
                                STUD.Rows.Add(dr);

                            }
                        }
                        STUD.AcceptChanges();
                        for (int i = 0; i < STUD.Rows.Count; i++)
                        {
                          
                            string Stime = DateTime.Now.AddMinutes(-5).ToString("HH:mm:ss");
                            String Sdate = DateTime.Now.ToString("yyyy-MM-dd");
                            String StartDate = Sdate + "T" + Stime;

                            string Etime = DateTime.Now.ToString("HH:mm:ss");
                            String Edate = DateTime.Now.ToString("yyyy-MM-dd");
                            String EndDate = Edate + "T" + Etime;

                            student = SIMSInterface.Attendance.GetAttendanceChanges(StartDate, EndDate);
                            XmlReader xr1 = new XmlNodeReader(student);
                            dsstudent = new DataSet();
                            dsstudent.ReadXml(xr1);
                            if (dsstudent.Tables.Count > 0)
                            {
                               
                                dtstudent = dsstudent.Tables[0];

                                for (int j = 0; j < dtstudent.Rows.Count; j++)
                                {
                                    DataRow dr = FDB.NewRow();
                                    String XMLPersonID = dtstudent.Rows[j][0].ToString();
                                    String SIMSPersonID = STUD.Rows[i][0].ToString();
                                    if (XMLPersonID.Equals(SIMSPersonID))
                                    {

                                    
                                    dr[0] = dtstudent.Rows[j][0].ToString();
                                    dr[1] = dtstudent.Rows[j][1].ToString();
                                    dr[2] = dtstudent.Rows[j][2].ToString();
                                    dr[3] = dtstudent.Rows[j][3].ToString();
                                    dr[4] = dtstudent.Rows[j][4].ToString(); //comments

                                    dr[5] = dtstudent.Rows[j][6].ToString();
                                    dr[6] = dtstudent.Rows[j][7].ToString();
                                    dr[7] = dtstudent.Rows[j][8].ToString(); //last updated

                                    dr[8] = STUD.Rows[i][1].ToString();
                                    dr[9] = STUD.Rows[i][2].ToString();
                                    dr[10] = STUD.Rows[i][3].ToString();

                                    FDB.Rows.Add(dr);
                                    }
                                }

                            }
                            
                        }
                        if (dsstudent.Tables.Count > 0)
                        {
                            if (FDB.Rows.Count > 0)
                            {
                                FDB.AcceptChanges();
                                //using (StringWriter sw = new StringWriter())
                                //{
                                //    FDB.WriteXml(sw);
                                //    result = sw.ToString();
                                //    totXmlRecCount = FDB.Rows.Count;
                                //    FileName = readAll.GetXmlpath() + "stud_attendance_" + "R" + totXmlRecCount + "_" + System.DateTime.Now.Date.ToString("dd/MM/yyyy").Replace('/', '_') + "_" + time + "_" + ".xml";
                                //    File.WriteAllText(FileName, result);
                                //    WriteToFile("File Created.." + DateTime.Now);
                                //    System.Threading.Thread.Sleep(1000);
                                //}
                                FileName = FDB.Rows.Count.ToString();
                            }
                            else
                            {
                                FileName = "";
                            }
                        }
                        else
                        {
                            FileName = "";
                        }
                        }
                    else
                    {
                        SendEmail("login","","SIMS login failed.. Authentication problem");
                    }
                    if (FileName.Length >0)
                    { 
                        updateAttendanceOnServer();
                    }

                }
                else
                {
                   // WriteToFile("Service time is over.. " + DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                SendEmail("server", SIMSInterface.School.DCSFNumber(),"Error due to " + ex.Message);
                WriteToFile(ex.Message + "--" + DateTime.Now);

            }

        }

     

        private void GetFirstTimeAttendanceData()
        {
            try
            {

                ReadAllData readAll = new ReadAllData();
                Attendance Att = new Attendance();
                DataTable Dtblupn = new DataTable();
                if (readAll.timeStartFirstmatch() == true)
                {

                    Dtblupn = Att.GetFirstmatchupn();

                    if (Dtblupn.Rows.Count > 0)
                    {
                       
                        
                        SIMSInterface.SIMSDllResolution.AddSIMSDllResolution();

                        if (SIMSInterface.LoginHelper.SIMSlogin(readAll.GetServer(), readAll.GetDBName(), readAll.GetUser(), readAll.GetPass()))
                        {

                            WriteToFile("Login Successfully " + DateTime.Now);
                            string dfe = SIMSInterface.School.DCSFNumber();
                            CheckConnection(dfe);
                            System.Threading.Thread.Sleep(1000);
                            FDB = new DataTable { TableName = "FDBTable" };
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
                            StudentDT = readAll.GetAllStudent();
                            // Match UPN LOGIC ////
                            DataRow[] rowval;
                            matchUPN = Dtblupn;
                            DataTable STUD = new DataTable { TableName = "STUD" };
                            STUD.Columns.Add(new DataColumn("StudentId", typeof(System.String)));
                            STUD.Columns.Add(new DataColumn("upn", typeof(System.String)));
                            STUD.Columns.Add(new DataColumn("AdmissionNumber", typeof(System.String)));
                            STUD.Columns.Add(new DataColumn("school_id", typeof(System.String)));


                            for (int u = 0; u < matchUPN.Rows.Count; u++)
                            {
                                if (matchUPN.Rows[u][0].ToString().Trim().Length > 0)
                                {
                                    rowval = StudentDT.Select("UPN = '" + matchUPN.Rows[u][0].ToString().Trim() + "'");

                                }
                                else
                                {
                                    rowval = StudentDT.Select("AdmissionNumber = '" + matchUPN.Rows[u][2].ToString().Trim() + "'");
                                }
                                foreach (DataRow row in rowval)
                                {
                                    DataRow dr = STUD.NewRow();
                                    dr[0] = row[0];
                                    dr[1] = row[13];
                                    dr[2] = row[12];
                                    dr[3] = matchUPN.Rows[u][1].ToString().Trim();
                                    STUD.Rows.Add(dr);

                                }
                            }
                            STUD.AcceptChanges();
                            for (int i = 0; i < STUD.Rows.Count; i++)
                            {
                                string Stime = "00:00:00";
                                String Sdate = "2018-09-01";
                                String StartDate = Sdate + "T" + Stime;

                                string Etime = DateTime.Now.ToString("HH:mm:ss");
                                String Edate = DateTime.Now.ToString("yyyy-MM-dd");
                                String EndDate = Edate + "T" + Etime;

                                student = SIMSInterface.Attendance.GetAttendanceChanges(StartDate, EndDate);
                                XmlReader xr1 = new XmlNodeReader(student);
                                dsstudent = new DataSet();
                                dsstudent.ReadXml(xr1);
                                if (dsstudent.Tables.Count > 0)
                                {

                                    dtstudent = dsstudent.Tables[0];

                                    for (int j = 0; j < dtstudent.Rows.Count; j++)
                                    {
                                        DataRow dr = FDB.NewRow();
                                        String XMLPersonID = dtstudent.Rows[j][0].ToString();
                                        String SIMSPersonID = STUD.Rows[i][0].ToString();
                                        if (XMLPersonID.Equals(SIMSPersonID))
                                        {
                                            dr[0] = dtstudent.Rows[j][0].ToString();
                                            dr[1] = dtstudent.Rows[j][1].ToString();
                                            dr[2] = dtstudent.Rows[j][2].ToString();
                                            dr[3] = dtstudent.Rows[j][3].ToString();
                                            dr[4] = dtstudent.Rows[j][4].ToString(); //comments

                                            dr[5] = dtstudent.Rows[j][6].ToString();
                                            dr[6] = dtstudent.Rows[j][7].ToString();
                                            dr[7] = dtstudent.Rows[j][8].ToString(); //last updated

                                            dr[8] = STUD.Rows[i][1].ToString();
                                            dr[9] = STUD.Rows[i][2].ToString();
                                            dr[10] = STUD.Rows[i][3].ToString();

                                            FDB.Rows.Add(dr);
                                        }
                                    }

                                }


                            }
                            FDB.AcceptChanges();
                            if (FDB.Rows.Count >0)
                            {
                                //using (StringWriter sw = new StringWriter())
                                //{
                                //    FDB.WriteXml(sw);
                                //    result = sw.ToString();
                                //    FileName = readAll.GetXmlpath() + "stud_attendance_" + time + "_01_09_2018" + ".xml";
                                //    File.WriteAllText(FileName, result);
                                //    WriteToFile("File Created.." + DateTime.Now);
                                //    System.Threading.Thread.Sleep(1000);
                                //}
                                FileName = FDB.Rows.Count.ToString();
                            }

                            // }

                        }
                        else
                        {
                            SendEmail("login","","SIMS login failed.. Authentication problem");
                            //  WriteToFile("Sevice time is over.. " + DateTime.Now);
                        }
                        if(FileName.Length >0)
                        { 
                                updateFirstAttendanceOnServer();
                        }

                    }
                }
                

            }
            catch (Exception ex)
            {
                SendEmail("server", SIMSInterface.School.DCSFNumber() ,"Error due to " + ex.Message);
                WriteToFile(ex.Message + "--" + DateTime.Now);

            }

        }


        public void updateAttendanceOnServer()
        {
            try
            {
                Msql = new MySqlRead();
                readAll = new ReadAllData();
                Att = new Attendance();
                SIMSServiceProject.SIMSSettings frm = new SIMSServiceProject.SIMSSettings();
                if (SIMSInterface.LoginHelper.SIMSlogin(readAll.GetServer(), readAll.GetDBName(), readAll.GetUser(), readAll.GetPass()))
                {

                    
                    MyConnection2 = Msql.connectionString();
                    MyConn2 = new MySqlConnection("datasource=" + Msql.GetServerName() + ";port=3306;username=" + Msql.UserName() + ";password=" + Msql.Password() + ";SslMode=none");
                    if (readAll.timeEmatch() == true)
                    {
                        // currentfile = FileName; //readAll.GetXmlpath() + "stud_attendance" + System.DateTime.Now.Date.ToString("dd/MM/yyyy").Replace('/', '_') + ".xml";
                        if (FileName.Length > 0 && FDB.Rows.Count > 0)
                        {
                            virtual_pup_attendance = new DataTable { TableName = "virtual_pup_attendance" };
                        virtual_pup_attendance.Columns.Add(new DataColumn("actual_date_id", typeof(Int32)));
                        virtual_pup_attendance.Columns.Add(new DataColumn("am", typeof(System.String)));
                        virtual_pup_attendance.Columns.Add(new DataColumn("pm", typeof(System.String)));
                        virtual_pup_attendance.Columns.Add(new DataColumn("pupil_id", typeof(Int32)));
                        WriteToFile("Time matched and updation start.... " + DateTime.Now);

                        ////// Diffirent files logic//////////////////////////


                        ///////////////////////////////////////////////////
                         
                        
                        DataTable studdtbl = FDB;

                            for (int k = 0; k < studdtbl.Rows.Count; k++)
                            {
                                String UPN = studdtbl.Rows[k][8].ToString();
                                String PeopleID = studdtbl.Rows[k][0].ToString();
                                String SCHOOLID = studdtbl.Rows[k][10].ToString();
                                String ADNO = studdtbl.Rows[k][9].ToString();
                                String Query;
                                if (UPN.Length > 0)
                                {
                                    Query = "select id from " + Msql.GetDBName() + "." + "pupils_unique where upn='" + UPN + "' and school_id='" + SCHOOLID + "' ;";
                                }
                                else
                                {
                                    Query = "select id from " + Msql.GetDBName() + "." + "pupils_unique where adno='" + ADNO + "' and school_id='" + SCHOOLID + "' ;";

                                }
                                MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
                                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                                MyAdapter.SelectCommand = MyCommand2;
                                dTpupil = new DataTable();
                                MyAdapter.Fill(dTpupil);
                                if (dTpupil.Rows.Count > 0)
                                {
                                    PID = dTpupil.Rows[0][0].ToString();
                                }

                               
                                String ADate = studdtbl.Rows[k][5].ToString(); // Event start date
                                string convertedADate = ADate.Substring(0, 10);
                                String Query1 = "select id from " + Msql.GetDBName() + "." + "virtual_attendance_date where actual_date='" + convertedADate + "' ;";
                                MySqlCommand MyCommand21 = new MySqlCommand(Query1, MyConn2);
                                MyConn2.Open();
                                MySqlDataAdapter MyAdapter1 = new MySqlDataAdapter();
                                MyAdapter1.SelectCommand = MyCommand21;
                                dTattdate = new DataTable();
                                MyAdapter1.Fill(dTattdate);
                                MyConn2.Close();

                                if (dTattdate.Rows.Count > 0)
                                {
                                    VDID = dTattdate.Rows[0][0].ToString();
                                }

                                if (PID.Length > 0 && VDID.Length > 0)
                                {
                                    if (studdtbl.Rows[k][1].ToString()=="AM")
                                    {
                                        AM = studdtbl.Rows[k][2].ToString();
                                        AMflag = true;
                                    }
                                    if (studdtbl.Rows[k][1].ToString() == "PM")
                                    {
                                        PM = studdtbl.Rows[k][2].ToString();
                                        PMflag = true;
                                    }
                                    bool InsertServer = Att.checkRecordonServer(PID, VDID, MyConn2);
                                    if (InsertServer == false)
                                    {
                                         int ins = Att.insertintoServer(PID, VDID, MyConn2, AMflag, PMflag,AM,PM);
                                        totins = totins + ins;

                                    }
                                    else
                                    {
                                         int upd = Att.updateintoServer(PID, VDID, MyConn2, AMflag, PMflag, AM, PM);
                                        totups = totups + upd;

                                    }
                                    string dfe = SIMSInterface.School.DCSFNumber();
                                    Att.LastUpdateonUPN(studdtbl.Rows[k][8].ToString(), dfe, studdtbl.Rows[k][9].ToString(), PID);
                                    virtual_pup_attendance.Clear();
                                    PID = "";
                                    VDID = "";
                                    dTpupil.Clear();
                                    dTattdate.Clear();
                                    AM = "";
                                    AMflag = false;
                                    PMflag = false;
                                    PM = "";



                                }
                                //String[] FName = FileName.Split('.');
                                //String filename = FName[0] + "updated_" + "" + RecordCount + "_" + ".xml";
                                //System.IO.File.Move(FileName, filename);
                            }
                        }
                       
                        WriteToFile("Total " + totins + " Records Inserted and " + totups + " updated on server   .." + DateTime.Now);
                        FileName = "";
                        totins = 0;
                        totups = 0;
                        readAll.DeleteToEmailLog("");
                        FDB.Clear();


                    }
                    else
                    {
                     //   WriteToFile("Waiting for the time match.." + DateTime.Now);

                    }
                }
            }
            catch (Exception ex)
            {
                SendEmail("server", SIMSInterface.School.DCSFNumber(),"Error due to " + ex.Message);
                WriteToFile(ex.Message + " " + DateTime.Now);
            }
        }

        public void updateFirstAttendanceOnServer()
        {
            try
            {
                Msql = new MySqlRead();
                readAll = new ReadAllData();
                Att = new Attendance();
                SIMSServiceProject.SIMSSettings frm = new SIMSServiceProject.SIMSSettings();
                if (SIMSInterface.LoginHelper.SIMSlogin(readAll.GetServer(), readAll.GetDBName(), readAll.GetUser(), readAll.GetPass()))
                {


                    MyConnection2 = Msql.connectionString();
                    MyConn2 = new MySqlConnection("datasource=" + Msql.GetServerName() + ";port=3306;username=" + Msql.UserName() + ";password=" + Msql.Password() + ";SslMode=none");
                        // currentfile = FileName; //readAll.GetXmlpath() + "stud_attendance" + System.DateTime.Now.Date.ToString("dd/MM/yyyy").Replace('/', '_') + ".xml";
                        if (FileName.Length > 0 && FDB.Rows.Count >0)
                        {
                            virtual_pup_attendance = new DataTable { TableName = "virtual_pup_attendance" };
                            virtual_pup_attendance.Columns.Add(new DataColumn("actual_date_id", typeof(Int32)));
                            virtual_pup_attendance.Columns.Add(new DataColumn("am", typeof(System.String)));
                            virtual_pup_attendance.Columns.Add(new DataColumn("pm", typeof(System.String)));
                            virtual_pup_attendance.Columns.Add(new DataColumn("pupil_id", typeof(Int32)));
                            WriteToFile("Time matched and updation start.... " + DateTime.Now);

                            DataSet ds = new DataSet();
                        ////// Diffirent files logic//////////////////////////


                        ///////////////////////////////////////////////////

                        //  ds.ReadXml(currentfile);
                        DataTable studdtbl = FDB;

                            for (int k = 0; k < studdtbl.Rows.Count; k++)
                            {
                                String UPN = studdtbl.Rows[k][8].ToString();
                                String PeopleID = studdtbl.Rows[k][0].ToString();
                                String SCHOOLID = studdtbl.Rows[k][10].ToString();
                                String ADNO = studdtbl.Rows[k][9].ToString();
                                String Query;
                                if (UPN.Length > 0)
                                {
                                    Query = "select id from " + Msql.GetDBName() + "." + "pupils_unique where upn='" + UPN + "' and school_id='" + SCHOOLID + "' ;";
                                }
                                else
                                {
                                    Query = "select id from " + Msql.GetDBName() + "." + "pupils_unique where adno='" + ADNO + "' and school_id='" + SCHOOLID + "' ;";

                                }
                                MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
                                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                                MyAdapter.SelectCommand = MyCommand2;
                                dTpupil = new DataTable();
                                MyAdapter.Fill(dTpupil);
                                if (dTpupil.Rows.Count > 0)
                                {
                                    PID = dTpupil.Rows[0][0].ToString();
                                }

                                //if (readAll.timeDaymatch() == true)
                                //{
                                //    uploadphotos(SCHOOLID, readAll.GetXmlpath(), Convert.ToInt16(PeopleID), Convert.ToInt16(PID));
                                //}

                                String ADate = studdtbl.Rows[k][5].ToString(); // Event start date
                                string convertedADate = ADate.Substring(0, 10);
                                String Query1 = "select id from " + Msql.GetDBName() + "." + "virtual_attendance_date where actual_date='" + convertedADate + "' ;";
                                MySqlCommand MyCommand21 = new MySqlCommand(Query1, MyConn2);
                                MyConn2.Open();
                                MySqlDataAdapter MyAdapter1 = new MySqlDataAdapter();
                                MyAdapter1.SelectCommand = MyCommand21;
                                dTattdate = new DataTable();
                                MyAdapter1.Fill(dTattdate);
                                MyConn2.Close();

                                if (dTattdate.Rows.Count > 0)
                                {
                                    VDID = dTattdate.Rows[0][0].ToString();
                                }

                                if (PID.Length > 0 && VDID.Length > 0)
                                {
                                    if (studdtbl.Rows[k][1].ToString() == "AM")
                                    {
                                        AM = studdtbl.Rows[k][2].ToString();
                                        AMflag = true;
                                    }
                                    if (studdtbl.Rows[k][1].ToString() == "PM")
                                    {
                                        PM = studdtbl.Rows[k][2].ToString();
                                        PMflag = true;
                                    }
                                    bool InsertServer = Att.checkRecordonServer(PID, VDID, MyConn2);
                                    if (InsertServer == false)
                                    {
                                        int ins = Att.insertintoServer(PID, VDID, MyConn2, AMflag, PMflag, AM, PM);
                                        totins = totins + ins;

                                    }
                                    else
                                    {
                                        int upd = Att.updateintoServer(PID, VDID, MyConn2, AMflag, PMflag, AM, PM);
                                        totups = totups + upd;

                                    }
                                    string dfe = SIMSInterface.School.DCSFNumber();
                                    Att.LastUpdateonUPNwithflag(studdtbl.Rows[k][8].ToString(), dfe, studdtbl.Rows[k][9].ToString(), PID);
                                    virtual_pup_attendance.Clear();
                                    PID = "";
                                    VDID = "";
                                    dTpupil.Clear();
                                    dTattdate.Clear();
                                    AM = "";
                                    AMflag = false;
                               
                                    PMflag = false;
                                    PM = "";
                                    



                                }
                                //String[] FName = FileName.Split('.');
                                //String filename = FName[0] + "updated_" + "" + RecordCount + "_" + ".xml";
                                //System.IO.File.Move(FileName, filename);
                            }
                        }
                        //if (FileName.Length > 0)
                        //{
                        //    int totcnt = totins + totups;
                        //    String[] FName1 = FileName.Split('.');
                        //    String filename1 = FName1[0] + "updated_" + "" + totcnt + "_" + ".xml";
                        //    System.IO.File.Move(FileName, filename1);

                        //}

                        WriteToFile("Total " + totins + " Records Inserted and " + totups + " updated on server   .." + DateTime.Now);
                        FileName = "";
                        FDB.Clear();
                    readAll.DeleteToEmailLog("");
                    totins = 0;
                        totups = 0;


                    }
                    else
                    {
                        //   WriteToFile("Waiting for the time match.." + DateTime.Now);

                    }
                
            }
            catch (Exception ex)
            {
                SendEmail("server", SIMSInterface.School.DCSFNumber(),"Error due to " + ex.Message);
                WriteToFile(ex.Message + " " + DateTime.Now);
            }
        }





       



        

    }
}
