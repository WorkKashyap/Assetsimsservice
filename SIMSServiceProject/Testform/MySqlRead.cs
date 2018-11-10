using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace TestConnection
{   
    class MySqlRead
    {
                
        public String GetServerName() 
        {
            // testing // string strServer = "gp901c2240ovgnf.mysql.database.azure.com";
            string strServer = "173.194.108.301";
            return strServer;
        }

        public String GetDBName()
        {
            string strDatabase = "neweyfs"; //"b6kag6grihudl9q1";
            return strDatabase;
        }

        public String UserName()
        {
            string strUser = "cklgHDg3RZEjtI";
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