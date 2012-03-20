using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class ItemPriceDetails
    {
        private iDB2Connection DB2connect;

        public ItemPriceDetails(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetItemPriceDetails(Int16 Class, Int32 Vendor, Int16 Style, Int16 Colour, Int16 Size)
        {
            StringBuilder cmd = new StringBuilder();
            cmd.Append("Select IMICYC, IMIRP, IMIMKT from dsspimil1 where IMICLS = ");
            cmd.Append(Class.ToString());
            cmd.Append(" and IMIVEN = ");
            cmd.Append(Vendor.ToString());
            cmd.Append(" and IMISTY = ");
            cmd.Append(Style.ToString());
            cmd.Append(" and IMICLR = ");
            cmd.Append(Colour.ToString());
            cmd.Append(" and IMISIZ = ");
            cmd.Append(Size.ToString());
            
            iDB2DataAdapter Adapter = new iDB2DataAdapter(cmd.ToString(), DB2connect);

            DataTable dtItemPrices = new DataTable("ItemPrices");
            Adapter.Fill(dtItemPrices);

            return dtItemPrices;
        }
    }
}
