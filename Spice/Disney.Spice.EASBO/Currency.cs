using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;
using Disney.Spice.DAL;

namespace Disney.Spice.EASBO
{
    public class Currency
    {
        private iDB2Connection DB2connect;
        private DataTable currencytbl;
        private String m_currencyCode;
        private String m_currencyName;
        private Decimal m_currencyRate;
        
        public String CurrencyCode
        {
            get { return m_currencyCode; }
            set { m_currencyCode = value; }
        }

        public String CurrencyName
        {
            get { return m_currencyName; }
            set { m_currencyName = value; }
        }

        public Decimal CurrencyRate
        {
            get { return m_currencyRate; }
            set { m_currencyRate = value; }
        }

        public Currency(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public Boolean GetCurrency(String CurrencyCode)
        {
            if (currencytbl == null)
            {
                DAL.Currency currency = new DAL.Currency(DB2connect);
                currencytbl = currency.GetCurrencyTbl();

                DataColumn[] Key = new DataColumn[] { currencytbl.Columns["CCUR"] };
                currencytbl.PrimaryKey = Key;
            }

            if (currencytbl != null)
            {
                DataRow dr = currencytbl.Rows.Find(CurrencyCode);
                if (dr != null)
                {
                    m_currencyCode = CurrencyCode;
                    m_currencyName = (String)dr["CURN"];
                    m_currencyRate = (Decimal)dr["CVAL"];

                    return true;
                }
                else return false;
            }
            else return false;
        }
    }
}