using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ProcessGuardAPI.Models
{
    public class Helpers
    {
        public static string GetRDSConnectionString()
        {
            string username = "ruben69695";
            string password = "qtU^Q1tN0rgw";
            string hostname = "rds-mysql.czepthgipwqe.us-west-2.rds.amazonaws.com";
            string port = "3306";
            string dbname = "processguarddb";

            return "Data Source=" + hostname + ";Port=" + port + ";Initial Catalog=" + dbname + ";User ID=" + username + ";Password=" + password + ";";

        }

        public static string GetLocalConnectionString()
        {
            string username = "root";
            string password = "jub";
            string hostname = "localhost";
            string port = "3306";
            string dbname = "processguarddb";

            return "Data Source=" + hostname + ";Port=" + port + ";Initial Catalog=" + dbname + ";User ID=" + username + ";Password=" + password + ";";
        }

        public static string GetClearDBConnectionString()
        {
            string username = "bc985701a8203c";
            string password = "b041d238";
            string hostname = "eu-cdbr-azure-west-b.cloudapp.net";
            string port = "3306";
            string dbname = "processguarddb";

            return "Data Source=" + hostname + ";Port=" + port + ";Initial Catalog=" + dbname + ";User ID=" + username + ";Password=" + password + ";";
        }

        public static string GetAzureSQLServerConnectionString()
        {
            string username = "ruben69695";
            string password = "qtU^Q1tN0rgw";
            string hostname = "educem2x3.database.windows.net";
            string port = "1433";
            string dbname = "processguarddb";

            return "Server=tcp:"+hostname+","+port+";Initial Catalog="+dbname+";Persist Security Info=False;User ID="+username+";Password="+password+";MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }
    }
}
