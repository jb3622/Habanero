using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.EASBO
{
    public class ItemPODetails
    {
        private iDB2Connection DB2connect;
        private DataTable ItemPOtbl;
        private Int16 m_class;
        private Int32 m_vendor;
        private Int16 m_style;
        private Int16 m_colour;
        private Int16 m_size;
        private string m_spicePO;
        private decimal m_spicePOVersion;
        private decimal m_lineSequence;
        private decimal m_POOrderQty;
        private decimal m_PORetailPrice;
        private decimal m_LandedCost;
        private decimal m_POSimpleVendorCost;
        private string m_POCurrency;
        private string m_marketCurrency;
        private decimal m_landingFactor;
        private decimal m_currencyRate;


        public ItemPODetails(iDB2Connection DB2connect)
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
                
        public string SpicePO
        {
            get { return m_spicePO; }
            set { m_spicePO = value; }
        }

        public decimal SpicePOVersion
        {
            get { return m_spicePOVersion; }
            set { m_spicePOVersion = value; }
        }

        public decimal LineSequence
        {
            get { return m_lineSequence; }
            set { m_lineSequence = value; }
        }

        public decimal POOrderQty
        {
            get { return m_POOrderQty; }
            set { m_POOrderQty = value; }
        }

        public decimal LandedCost
        {
            get { return m_LandedCost; }
            set { m_LandedCost = value; }
        }

        public decimal PORetailPrice
        {
            get { return m_PORetailPrice; }
            set { m_PORetailPrice = value; }
        }

        public decimal POSimpleVendorCost
        {
            get { return m_POSimpleVendorCost; }
            set { m_POSimpleVendorCost = value; }
        }

        public string POCurrency
        {
            get { return m_POCurrency; }
            set { m_POCurrency = value; }
        }

        public string MarketCurrency
        {
            get { return m_marketCurrency; }
            set { m_marketCurrency = value; }
        }

        public decimal LandingFactor
        {
            get { return m_landingFactor; }
            set { m_landingFactor = value; }
        }

        public decimal CurrencyRate
        {
            get { return m_currencyRate; }
            set { m_currencyRate = value; }
        }
       

        public Boolean GetItemPODetails(Int16 Class, Int32 Vendor, Int16 Style, Int16 Colour, Int16 Size, String SpicePO, Int16 SpicePOVersion, Int16 LineSequence)
        {
          
            if (ItemPOtbl == null)
            {
                DAL.ItemPODetails itemPODetails = new DAL.ItemPODetails(DB2connect);
                ItemPOtbl = itemPODetails.GetItemPODetails(Class, Vendor, Style, Colour, Size, SpicePO, SpicePOVersion, LineSequence);
            }

            if (ItemPOtbl != null) 
            {
                m_class = Class;
                m_vendor = Vendor;
                m_style = Style;
                m_colour = Colour;
                m_size = Size;

                m_POOrderQty = (Decimal)ItemPOtbl.Rows[0]["POIQTY"];
                m_PORetailPrice = (Decimal)ItemPOtbl.Rows[0]["POIRET"];
                m_POSimpleVendorCost = (Decimal)ItemPOtbl.Rows[0]["POIVNC"];
                m_POCurrency = (String)ItemPOtbl.Rows[0]["POHCYC"];
                m_marketCurrency = (String)ItemPOtbl.Rows[0]["CSCUR#"];
                m_landingFactor = (Decimal)ItemPOtbl.Rows[0]["POILNF"];
                m_currencyRate = (Decimal)ItemPOtbl.Rows[0]["POHCYR"];
                m_LandedCost = (Decimal)ItemPOtbl.Rows[0]["POILNC"];
                
                return true;
            }
            else return false;
          }
       }
}
