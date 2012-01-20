using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class Ports
    {
        private iDB2Connection DB2connect;

        public Ports(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetPortsTable()
        {
            String SelectCmd = "Select * from dsports";
            iDB2DataAdapter Adapter = new iDB2DataAdapter(SelectCmd, DB2connect);

            DataTable dt = new DataTable("Ports");
            Adapter.Fill(dt);

            return dt;
        }
    }
}