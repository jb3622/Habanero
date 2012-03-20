using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class ItemMarket
    {
        private iDB2Connection DB2connect;

        public ItemMarket(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetItemMarketDetails(Int16 Class, Int32 Vendor, Int16 Style, Int16 Colour, Int16 Size, string Market)
        {
            StringBuilder cmd = new StringBuilder();
            cmd.Append("Select IMIVUP, IMIVAT from dsspimil1 where IMICLS = ");
            cmd.Append(Class.ToString());
            cmd.Append(" and IMIVEN = ");
            cmd.Append(Vendor.ToString());
            cmd.Append(" and IMISTY = ");
            cmd.Append(Style.ToString());
            cmd.Append(" and IMICLR = ");
            cmd.Append(Colour.ToString());
            cmd.Append(" and IMISIZ = ");
            cmd.Append(Size.ToString());
            cmd.Append(" and IMIMKT = '");
            cmd.Append(Market.ToString());
            cmd.Append("'");
                        
            iDB2DataAdapter Adapter = new iDB2DataAdapter(cmd.ToString(), DB2connect);

            DataTable dtItemMarket = new DataTable("ItemMarket");
            Adapter.Fill(dtItemMarket);
            
            return dtItemMarket;
        }
    }
}