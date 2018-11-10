using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMSServices
{
    class Attendance
    {
        ReadAllData readAll = new ReadAllData();
        String MyConnection2;
        MySqlRead Msql;
        MySqlConnection MyConn2;
        
      
            
        public int insertintoServer(string pid, string vdateid, MySqlConnection MyConn2,Boolean am,Boolean pm, string AM, string PM)
        {
            try
            {
                string actualdateid = vdateid;
                string amtime = AM;
                string pmtime = PM;
                string pupilid = pid;
                string sql;
                MySqlRead Msql = new MySqlRead();
                MyConn2.Open();
                MySqlCommand cmd = new MySqlCommand("set net_write_timeout=99999; set net_read_timeout=99999", MyConn2); // Setting tiimeout on mysqlServer
                cmd.ExecuteNonQuery();
                if (pmtime.Contains("\\"))
                {
                    pmtime = pmtime + "\\";
                }
                if (am == true)
                {
                     sql = "insert into " + Msql.GetDBName() + "." + "virtual_pup_attendance (actual_date_id, am, pupil_id) values (" + actualdateid + " ,'" + amtime + "', " + pupilid + ");";
                }
                else
                {
                    sql = "insert into " + Msql.GetDBName() + "." + "virtual_pup_attendance (actual_date_id, pm, pupil_id) values (" + actualdateid + " , '" + pmtime + "', " + pupilid + ");";
                }

                    MySqlCommand MyCommand21 = new MySqlCommand(sql, MyConn2);
                MySqlDataAdapter MyAdapter1 = new MySqlDataAdapter();
                MyAdapter1.SelectCommand = MyCommand21;
                int rowCount = MyCommand21.ExecuteNonQuery();
                // totinsertcount = totinsertcount + rowCount;
                MyConn2.Close();
                return rowCount;
            }
            catch (Exception ex)
            {
                readAll.WriteToFile("insert "+ ex.Message + " " + DateTime.Now);
                return -1;
            }
        }
        public int updateintoServer(string pid, string vdateid, MySqlConnection MyConn2, Boolean am, Boolean pm, string AM, string PM)
        {
            try
            {
                string actualdateid = vdateid;
                string amtime = AM;
                string pmtime = PM;
                string pupilid = pid;
                MySqlRead Msql = new MySqlRead();
                string sql;
                MyConn2.Open();

                MySqlCommand cmd = new MySqlCommand("set net_write_timeout=99999; set net_read_timeout=99999", MyConn2); // Setting tiimeout on mysqlServer
                cmd.ExecuteNonQuery();

                if (pmtime.Contains("\\"))
                {
                    pmtime = pmtime + "\\";
                }
                if (am == true)
                {
                    sql = "update  " + Msql.GetDBName() + "." + "virtual_pup_attendance set am= '" + amtime + "'  where actual_date_id=" + actualdateid + " and pupil_id=" + pupilid + ";";
                }
                else
                {
                    sql = "update  " + Msql.GetDBName() + "." + "virtual_pup_attendance set  pm= '" + pmtime + "' where actual_date_id=" + actualdateid + " and pupil_id=" + pupilid + ";";
                }
                  
                MySqlCommand MyCommand21 = new MySqlCommand(sql, MyConn2);
                MySqlDataAdapter MyAdapter1 = new MySqlDataAdapter();
                MyAdapter1.SelectCommand = MyCommand21;
                int rowuCount = MyCommand21.ExecuteNonQuery();
               // totupdatecount = totupdatecount + rowuCount;
                MyConn2.Close();
                return rowuCount;
            }
            catch (Exception ex)
            {
                readAll.WriteToFile("update " + ex.Message + " " + DateTime.Now);
                return -1;
            }
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

        public void updateDFE(string upn, string dfe)
        {
            MySqlConnection MyConn2;
            MySqlRead Msql = new MySqlRead();
            MyConn2 = new MySqlConnection("datasource=" + Msql.GetServerName() + ";port=3306;username=" + Msql.UserName() + ";password=" + Msql.Password() + ";SslMode=none");
            MyConn2.Open();

            MySqlCommand cmd = new MySqlCommand("set net_write_timeout=99999; set net_read_timeout=99999", MyConn2); // Setting tiimeout on mysqlServer
            cmd.ExecuteNonQuery();

            string sql = "update  " + Msql.GetDBName() + "." + "virtual_upn set dfe_number ='"+ dfe +"' where upn='"+ upn +"'";
            MySqlCommand MyCommanddfe = new MySqlCommand(sql, MyConn2);
            MySqlDataAdapter MyAdapterdfe = new MySqlDataAdapter();
            MyAdapterdfe.SelectCommand = MyCommanddfe;
            MyCommanddfe.ExecuteNonQuery();
        }
        
        public  bool checkRecordonServer(string pid, string vdateid, MySqlConnection MyConn2)
        {
            string actualdateid = vdateid;
            string pupilid = pid;
            DataTable dtDuplicate;
            MySqlRead Msql = new MySqlRead();
            MyConn2.Open();
            MySqlCommand cmd = new MySqlCommand("set net_write_timeout=99999; set net_read_timeout=99999", MyConn2); // Setting tiimeout on mysqlServer
            cmd.ExecuteNonQuery();
            string sql = "select * from " + Msql.GetDBName() + "." + "virtual_pup_attendance where actual_date_id=" + actualdateid + " and pupil_id=" + pupilid + ";";
            MySqlCommand MyCommand21 = new MySqlCommand(sql, MyConn2);
            MySqlDataAdapter MyAdapter1 = new MySqlDataAdapter();
            MyAdapter1.SelectCommand = MyCommand21;
            dtDuplicate = new DataTable();
            MyAdapter1.Fill(dtDuplicate);
            MyConn2.Close();

            if (dtDuplicate.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        public void LastUpdateonUPN(string upn, string dfe,string adno, string pupil_id)
        {
            try
            {
            
                String currentdatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss",CultureInfo.InvariantCulture);
                string sql;
                 MySqlConnection MyConn2;
                MySqlRead Msql = new MySqlRead();
                MyConn2 = new MySqlConnection("datasource=" + Msql.GetServerName() + ";port=3306;username=" + Msql.UserName() + ";password=" + Msql.Password() + ";SslMode=none");
                 MyConn2.Open();
                MySqlCommand cmd = new MySqlCommand("set net_write_timeout=99999; set net_read_timeout=99999", MyConn2); // Setting tiimeout on mysqlServer
                cmd.ExecuteNonQuery();
                if (upn.Length > 0)
                {
                    sql = "update  " + Msql.GetDBName() + "." + "virtual_upn set lastupdated= '" + currentdatetime + "', dfe_number='" + dfe + "', pupil_id='"+ pupil_id + "'  where upn='" + upn + "';";
                }
                else
                {
                    sql = "update  " + Msql.GetDBName() + "." + "virtual_upn set lastupdated= '" + currentdatetime + "', dfe_number='" + dfe + "', pupil_id='" + pupil_id + "'   where adno='" + adno + "';";
                }

                MySqlCommand MyCommandlst = new MySqlCommand(sql, MyConn2);
                MySqlDataAdapter MyAdapterlst = new MySqlDataAdapter();
                MyAdapterlst.SelectCommand = MyCommandlst;
                MyCommandlst.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                WriteToFile("update Date  " + ex.Message + " " + DateTime.Now);

            }


        }

        public void LastUpdateonUPNwithflag(string upn, string dfe, string adno, string pupil_id)
        {
            try
            {

                String currentdatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                string sql;
                MySqlConnection MyConn2;
                MySqlRead Msql = new MySqlRead();
                MyConn2 = new MySqlConnection("datasource=" + Msql.GetServerName() + ";port=3306;username=" + Msql.UserName() + ";password=" + Msql.Password() + ";SslMode=none");
                MyConn2.Open();
                MySqlCommand cmd = new MySqlCommand("set net_write_timeout=99999; set net_read_timeout=99999", MyConn2); // Setting tiimeout on mysqlServer
                cmd.ExecuteNonQuery();
                if (upn.Length > 0)
                {
                    sql = "update  " + Msql.GetDBName() + "." + "virtual_upn set lastupdated= '" + currentdatetime + "', dfe_number='" + dfe + "', sims_flag='1', pupil_id='" + pupil_id + "'   where upn='" + upn + "';";
                }
                else
                {
                        sql = "update  " + Msql.GetDBName() + "." + "virtual_upn set lastupdated= '" + currentdatetime + "', dfe_number='" + dfe + "', sims_flag='1', pupil_id='" + pupil_id + "'   where adno='" + adno + "';";
                }

                MySqlCommand MyCommandlst = new MySqlCommand(sql, MyConn2);
                MySqlDataAdapter MyAdapterlst = new MySqlDataAdapter();
                MyAdapterlst.SelectCommand = MyCommandlst;
                MyCommandlst.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                WriteToFile("update Date  " + ex.Message + " " + DateTime.Now);

            }


        }


        public DataTable Getmatchupn()
        {
            Msql = new MySqlRead();
            MyConnection2 = Msql.connectionString();
            MyConn2 = new MySqlConnection("datasource=" + Msql.GetServerName() + ";port=3306;username=" + Msql.UserName() + ";password=" + Msql.Password() + ";SslMode=none");
            String Query = "select a.upn, a.school_id, a.adno from  " + Msql.GetDBName() + "." + "virtual_upn a , " + Msql.GetDBName() + "." + "pupils_unique b where (a.upn =b.upn) OR (a.adno=b.adno);";
            MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
            MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
            MyAdapter.SelectCommand = MyCommand2;
            DataTable dtUpn = new DataTable();
            MyAdapter.Fill(dtUpn);
            return dtUpn;
                        
        }

        public DataTable GetFirstmatchupn()
        {


            Msql = new MySqlRead();
            MyConnection2 = Msql.connectionString();
            MyConn2 = new MySqlConnection("datasource=" + Msql.GetServerName() + ";port=3306;username=" + Msql.UserName() + ";password=" + Msql.Password() + ";SslMode=none");
            String Query = "select a.upn, a.school_id, a.adno from  " + Msql.GetDBName() + "." + "virtual_upn a , " + Msql.GetDBName() + "." + "pupils_unique b where (a.upn =b.upn And  sims_flag IS NULL) OR  (a.adno=b.adno And  sims_flag IS NULL);";
            MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
            MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
            MyAdapter.SelectCommand = MyCommand2;
            DataTable dtUpnmatch = new DataTable();
            MyAdapter.Fill(dtUpnmatch);
            return dtUpnmatch;

        }

        public string CheckConnectionServer()
        {
            try
            {


                Msql = new MySqlRead();
                MyConnection2 = Msql.connectionString();
                MyConn2 = new MySqlConnection("datasource=" + Msql.GetServerName() + ";port=3306;username=" + Msql.UserName() + ";password=" + Msql.Password() + ";SslMode=none");
                MyConn2.Open();
             
                String Query = "select * from  " + Msql.GetDBName() + "." + "virtual_upn;";
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

    }
}
