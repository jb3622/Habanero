using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class ShipVia
    {
        private iDB2Connection DB2connect;

        public ShipVia(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetShipViaTable()
        {
            String SelectCmd = "Select * from dsship";

            iDB2DataAdapter Adapter = new iDB2DataAdapter(SelectCmd, DB2connect);

            DataTable dt = new DataTable("ShipVia");
            Adapter.Fill(dt);

            return dt;
        }
    }
}