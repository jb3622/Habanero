using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class POcomments
    {
        private iDB2Connection DB2connection;

        public POcomments(iDB2Connection DB2connection)
        {
            this.DB2connection = DB2connection;
        }

        public DataTable GetPOcomments(String POnumber, int POversion)
        {
            StringBuilder SelectCmd = new StringBuilder();

            SelectCmd.Append("Select * from DSSPPOCL1 Where POCSPO = '");
            SelectCmd.Append(POnumber);
            SelectCmd.Append("' And POCVER = '");
            SelectCmd.Append(POversion.ToString());
            SelectCmd.Append("'");

            iDB2DataAdapter Adapter = new iDB2DataAdapter(SelectCmd.ToString(), DB2connection);

            DataTable dtComments = new DataTable("POcomments");
            Adapter.Fill(dtComments);

            return dtComments;
        }
    }
}