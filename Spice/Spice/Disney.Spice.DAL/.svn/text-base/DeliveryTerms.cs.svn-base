using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class DeliveryTerms
    {
        private iDB2Connection DB2connection;

        public DeliveryTerms(iDB2Connection DB2connection)
        {
            this.DB2connection = DB2connection;
        }

        public DataTable GetDeliveryTermsTable()
        {
            String SelectCmd = "Select * from dsdltrm";

            iDB2DataAdapter Adapter = new iDB2DataAdapter(SelectCmd, DB2connection);

            DataTable dt = new DataTable("DeliveryTerms");
            Adapter.Fill(dt);

            return dt;
        }
    }
}