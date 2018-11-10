using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace SIMSServices
{   
    class MySqlRead
    {
                
        public String GetServerName() 
        {
            // testing // string strServer = "gp901c2240ovgnf.mysql.database.azure.com";
            //  string strServer = "173.194.108.30";
            string strServer = "p9ew8h4ogj.mysql.database.azure.com";
            return strServer;
        }

        public String GetDBName()
        {
            string strDatabase = "rqc3d804lxwo3oaqisdob"; //"neweyfs"; //"b6kag6grihudl9q1";
            return strDatabase;
        }

        public String UserName()
        {
            string strUser = "cklgHDg3RZEjtI@p9ew8h4ogj";
            return strUser;
        }

        public String Password()
        {
            string strPassword = "c$f1Y@WhX3iu!4L1!Z4I@Kh";
            return strPassword;   
        }

        public String connectionString()
        {
            return "datasource=" + GetServerName() + ";persistsecurityinfo=True;port=3306;username=" + UserName() + ";password=" + Password() + ";SslMode=none";
        }

       
    }
       
}