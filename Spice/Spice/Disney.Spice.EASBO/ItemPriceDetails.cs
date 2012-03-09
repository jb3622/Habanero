using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.EASBO
{
    public class ItemPriceDetails
    {
        private iDB2Connection DB2connect;
        private DataTable ItemPriceTable;
        private Int16 m_class;
        private Int32 m_vendor;
        private Int16 m_style;
        private Int16 m_colour;
        private Int16 m_size;
        private decimal m_itemRetailPrice;
        private string m_marketCurrency;
        private string m_market;

        public ItemPriceDetails(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public Int16 Class
        {
            get { return m_class; }
            set { m_class = value; }
        }

        public Int32 Vendor
        {
            get { return m_vendor; }
            set { m_vendor = value; }
        }

        public Int16 Style
        {
            get { return m_style; }
            set { m_style = value; }
        }

        public Int16 Colour
        {
            get { return m_colour; }
            set { m_colour = value; }
        }

        public Int16 Size
        {
            get { return m_size; }
            set { m_size = value; }
        }

        public decimal ItemRetailPrice
        {
            get { return m_itemRetailPrice; }
            set { m_itemRetailPrice = value; }
        }

        public string MarketCurrency
        {
            get { return m_marketCurrency; }
            set { m_marketCurrency = value; }
        }

        public string Market
        {
            get { return m_market; }
            set { m_market = value; }
        }

        public DataTable GetItemPriceDetails(Int16 Class, Int32 Vendor, Int16 Style, Int16 Colour, Int16 Size)
        {

            if (ItemPriceTable == null)
            {
                DAL.ItemPriceDetails itemPriceDetails = new DAL.ItemPriceDetails(DB2connect);
                ItemPriceTable = itemPriceDetails.GetItemPriceDetails(Class, Vendor, Style, Colour, Size);
            }

            return ItemPriceTable;
        }
    }
}

