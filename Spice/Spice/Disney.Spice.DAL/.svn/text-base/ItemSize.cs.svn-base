using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class ItemSize
    {
        private iDB2Connection DB2connect;

        public ItemSize(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetItemSizeDetails(Int16 Class, Int32 Vendor, Int16 Style, Int16 Colour, Int16 Size)
        {
            StringBuilder cmd = new StringBuilder();
            cmd.Append("Select IIASKU, IIAVUP, IIADUP, IIACPT, IIAMIN, IIAMLT, IIACWG, IIACHT, IIACLN, IIACWI, CADES from dsspiial1, DSCASPK where IIACLS = ");
            cmd.Append(Class.ToString());
            cmd.Append(" and IIAVEN = ");
            cmd.Append(Vendor.ToString());
            cmd.Append(" and IIASTY = ");
            cmd.Append(Style.ToString());
            cmd.Append(" and IIACLR = ");
            cmd.Append(Colour.ToString());
            cmd.Append(" and IIASIZ = ");
            cmd.Append(Size.ToString());
            cmd.Append(" and IIACPT = CACOD");
                        
            iDB2DataAdapter Adapter = new iDB2DataAdapter(cmd.ToString(), DB2connect);

            DataTable dtSKU = new DataTable("ItemSize");
            Adapter.Fill(dtSKU);
            
            return dtSKU;
        }
    }
}