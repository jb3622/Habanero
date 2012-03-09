using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class Currency
    {
        private iDB2Connection DB2connect;

        public Currency(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetCurrencyTbl()
        {
            String cmd = "Select CCUR,CURN,CVAL from ipcurcy";
            iDB2DataAdapter Adapter = new iDB2DataAdapter(cmd, DB2connect);

            DataTable dt = new DataTable("Currency");
            Adapter.Fill(dt);

            return dt;
        }
    }
}