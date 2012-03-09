using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class PaymentTerms
    {
        private iDB2Connection DB2connect;

        public PaymentTerms(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetPaymentTerms(String Terms)
        {
            StringBuilder SelectCmd = new StringBuilder();

            SelectCmd.Append("Select TCOD, TDES from IPTERMS ");
            SelectCmd.Append("where TCOD = '");
            SelectCmd.Append(Terms);
            SelectCmd.Append("'");

            iDB2DataAdapter Adapter = new iDB2DataAdapter(SelectCmd.ToString(), DB2connect);

            DataTable dt = new DataTable("PaymentTerms");
            Adapter.Fill(dt);

            return dt;
        }
    }
}