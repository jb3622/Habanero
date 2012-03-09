using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;
using Disney.Spice.DAL;

namespace Disney.Spice.EASBO
{
    public class POlines
    {
        private iDB2Connection DB2connect;

        public POlines(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetPOlines(String POnumber, Int16 POversion)
        {
            DAL.POlines polines = new DAL.POlines(DB2connect);

            DataTable dt = polines.GetPOlines(POnumber, POversion);

            return dt;
        }
    }
}
