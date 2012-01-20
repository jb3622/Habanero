using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class POheader
    {
        private iDB2Connection DB2connection;

        public POheader(iDB2Connection DB2connection)
        {
            this.DB2connection = DB2connection;
        }

        public DataTable GetPOheader(String POnumber)
        {
            StringBuilder SelectCmd = new StringBuilder();

            SelectCmd.Append("Select * from DSSPPOHL1 "); 
            SelectCmd.Append(" Left Outer Join DSCNTAP   On POHMKT = CSCODE ");
            SelectCmd.Append("Where POHSPO = '");
            SelectCmd.Append(POnumber);
            SelectCmd.Append("'");

            iDB2DataAdapter Adapter = new iDB2DataAdapter(SelectCmd.ToString(), DB2connection);

            DataTable dt = new DataTable("POheader");
            Adapter.Fill(dt);

            return dt;
        }
    }
}