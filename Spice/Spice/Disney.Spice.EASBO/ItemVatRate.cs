using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.EASBO
{
    public class ItemVatRate
    {
        private iDB2Connection DB2connect;
        private DataTable itemVatRatetbl;
        private decimal m_vatRate;
    
        public ItemVatRate(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

       public decimal VatRate
       {
           get { return m_vatRate; }
           set { m_vatRate = value; }
       }

       public Boolean GetVatRate(string Market, string StoreVatCode, string ItemVatCode)
        {
            if (itemVatRatetbl == null)
            {
                DAL.ItemVatRate itemvatrate = new DAL.ItemVatRate(DB2connect);
                itemVatRatetbl = itemvatrate.GetItemVatRate(Market, StoreVatCode, ItemVatCode);
            }
                                   
            if (itemVatRatetbl != null & itemVatRatetbl.Rows.Count > 0)
            {

                m_vatRate = (Decimal)itemVatRatetbl.Rows[0]["IVRSVR"];
                
                return true;
            }
            else return false;
          }
       }
}
