using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class ItemStyle
    {
        private iDB2Connection DB2connect;

        public ItemStyle(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetItemStyleDetails(Int16 Class, Int32 Vendor, Int16 Style, Int16 Colour)
        {
            StringBuilder cmd = new StringBuilder();
            cmd.Append("Select ISADES,ISACHR,ISAVST,ISAVUP,ISACPT,ISAMIN,ISAMLT,ISACHT,");
            cmd.Append("ISACWI,ISACLN,ISACWG,CHDESC,CADES from dsspisal1,dschars,dscaspk where ISACLS = ");
            cmd.Append(Class.ToString());
            cmd.Append(" and ISAVEN = ");
            cmd.Append(Vendor.ToString());
            cmd.Append(" and ISASTY = ");
            cmd.Append(Style.ToString());
            cmd.Append(" and ISACLR = ");
            cmd.Append(Colour.ToString());
            cmd.Append(" and ISACHR = CHCODE");
            cmd.Append(" and ISACPT = CACOD");
            
            iDB2DataAdapter Adapter = new iDB2DataAdapter(cmd.ToString(), DB2connect);

            DataTable dt = new DataTable("ItemStyle");
            Adapter.Fill(dt);
            
            return dt;
        }
    }
}