using System;
using System.Collections.Generic;
using System.Text;

namespace Disney.Spice.EASUI
{
    public class LineDetails
    {
        private short m_class;
        private int m_vendor;
        private short m_style;
        private short m_colour;
        private short m_size;
        private short m_lineSequence;
        private string m_longDescription;
        private string m_shortDescription;
        private string m_seasonDescription;
        private string m_characterName;
        private String m_vendorStyle;
        private decimal m_vendorUPC;
        private decimal m_SKUNumber;
        private string m_className;
        private string m_colourName;
        private string m_sizeName;
        private string m_casePackType;
        private decimal m_casePackQty;
        private decimal m_DistroInnerQty;
        private decimal m_height;
        private decimal m_width;
        private decimal m_length;
        private decimal m_weight;
        private string m_spicePO;        
        private Int16 m_spicePOVersion;
        private decimal m_POOrderQty;
        private decimal m_PORetailPrice;
        private decimal m_POSimpleVendorCost;
        private string m_POCurrency;
        private string m_marketCurrency;
        private string m_casePackTypeDes;
        private decimal m_landingFactor;
        private decimal m_currencyRate;
        private string m_market;
        private string m_itemVatCode;
        private string m_storeVatCode;
        private decimal m_vatRate;
        private decimal m_landedCost;

        public LineDetails()
        {
        }

        public short Class
        {
            get
            { return m_class; }

            set
            { m_class = value; }
        }

        public int Vendor
        {
            get { return m_vendor; }
            set { m_vendor = value; }
        }

        public short Style
        {
            get { return m_style; }
            set { m_style = value; }
        }

        public short Colour
        {
            get { return m_colour; }
            set { m_colour = value; }
        }

        public short Size
        {
            get { return m_size; }
            set { m_size = value; }
        }

        public short LineSequence
        {
            get { return m_lineSequence; }
            set { m_lineSequence = value; }
        }

        public string LongDescription
        {
            get { return m_longDescription; }
            set { m_longDescription = value; }
        }

        public string ShortDescription
        {
            get { return m_shortDescription; }
            set { m_shortDescription = value; }
        }

        public string SeasonDescription
        {
            get { return m_seasonDescription; }
            set { m_seasonDescription = value; }
        }
               
        public string CharacterName
        {
            get { return m_characterName; }
            set { m_characterName = value; }
        }

        public String VendorStyle
        {
            get { return m_vendorStyle; }
            set { m_vendorStyle = value; }
        }

        public decimal VendorUPC
        {
            get { return m_vendorUPC; }
            set { m_vendorUPC = value; }
        }

        public decimal SKUNumber
        {
            get { return m_SKUNumber; }
            set { m_SKUNumber = value; }
        }

        public string ClassName
        {
            get { return m_className; }
            set { m_className = value; }
        }

        public string ColourName
        {
            get { return m_colourName; }
            set { m_colourName = value; }
        }

        public string SizeName
        {
            get { return m_sizeName; }
            set { m_sizeName = value; }
        }

        public string CasePackType
        {
            get { return m_casePackType; }
            set { m_casePackType = value; }
        }

        public decimal CasePackQty
        {
            get { return m_casePackQty; }
            set { m_casePackQty = value; }
        }

        public decimal DistroInnerQty
        {
            get { return m_DistroInnerQty; }
            set { m_DistroInnerQty = value; }
        }

        public decimal Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public decimal Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        public decimal Length
        {
            get { return m_length; }
            set { m_length = value; }
        }

        public decimal Weight
        {
            get { return m_weight; }
            set { m_weight = value; }
        }

        public string SpicePO
        {
            get { return m_spicePO; }
            set { m_spicePO = value; }
        }

        public Int16 SpicePOVersion
        {
            get { return m_spicePOVersion; }
            set { m_spicePOVersion = value; }
        }

        public decimal POOrderQty
        {
            get { return m_POOrderQty; }
            set { m_POOrderQty = value; }
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

        public string CasePackTypeDes
        {
            get { return m_casePackTypeDes; }
            set { m_casePackTypeDes = value; }
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

        public string Market
        {
            get { return m_market; }
            set { m_market = value; }
        }

        public string ItemVatCode
        {
            get { return m_itemVatCode; }
            set { m_itemVatCode = value; }
        }

        public string StoreVatCode
        {
            get { return m_storeVatCode; }
            set { m_storeVatCode = value; }
        }

        public decimal VatRate
        {
            get { return m_vatRate; }
            set { m_vatRate = value; }
        }

        public decimal LandedCost
        {
            get { return m_landedCost; }
            set { m_landedCost = value; }
        }
    }
}