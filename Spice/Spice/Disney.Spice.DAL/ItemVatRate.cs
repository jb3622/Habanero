using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class ItemVatRate
    {
        private iDB2Connection DB2connect;

        public ItemVatRate(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetItemVatRate(string Market, string StoreVatCode, string ItemVatCode)
        {
            StringBuilder cmd = new StringBuilder();
            cmd.Append("Select IVRSVR from dsspivr where IVRMKT = '");
            cmd.Append(Market.ToString());
            cmd.Append("'");
            cmd.Append(" and IVRSVC = '");
            cmd.Append(StoreVatCode.ToString());
            cmd.Append("'");
            cmd.Append(" and IVRIVC = '");
            cmd.Append(ItemVatCode.ToString());
            cmd.Append("'");
                                    
            iDB2DataAdapter Adapter = new iDB2DataAdapter(cmd.ToString(), DB2connect);

            DataTable dtItemVatRate = new DataTable("ItemVatRate");
            Adapter.Fill(dtItemVatRate);
            
            return dtItemVatRate;
        }
    }
}