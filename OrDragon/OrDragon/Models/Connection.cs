using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrDragon.Models
{
    public static class Connection
    {
        static private string DataSource = "(DESCRIPTION="
            + "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)"
            + "(HOST=" + System.Configuration.ConfigurationManager.AppSettings["DB_Hostname"]
            + ")(PORT=" + System.Configuration.ConfigurationManager.AppSettings["DB_Port"]
            + ")))(CONNECT_DATA=(SERVICE_NAME=" + System.Configuration.ConfigurationManager.AppSettings["DB_ServiceName"]
            + ")))";

        private static String GetConnectionString()
        {
            return "Data Source =" + DataSource
                + " ; User Id=" + System.Configuration.ConfigurationManager.AppSettings["DB_Username"]
                + " ; Password=" + System.Configuration.ConfigurationManager.AppSettings["DB_Password"] + ";";
        }

        public static OracleConnection GetConnection()
        {
            return new OracleConnection(GetConnectionString());
        }
    }
}
