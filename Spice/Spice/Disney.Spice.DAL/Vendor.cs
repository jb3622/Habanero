using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class Vendor
    {
        private iDB2Connection DB2connect;

        public Vendor(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetVendorDetails(Int32 VendorNumber)
        {
            String cmd = "Select VVEN,VNAM from ipmrven where VVEN = " + VendorNumber.ToString();
            iDB2DataAdapter Adapter = new iDB2DataAdapter(cmd, DB2connect);

            DataTable dt = new DataTable("Vendor");
            Adapter.Fill(dt);

            return dt;
        }
    }
}