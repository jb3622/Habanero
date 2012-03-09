using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Disney.Spice.DAL;
using IBM.Data.DB2.iSeries;
using Disney.Menu;

namespace Disney.Spice.EASBO
{
    public class PurchaseOrder
    {
        private iDB2Connection DB2connection;
        private Environments   environment;
        private POheader       poheader;
        private DataTable      dtHeader;
        private String m_SpicePOnumber;
        private short m_SpicePOversion;
        private string m_IPPOnumber;
        private string m_Market;
        private string m_MarketName;
        private short m_Department;
        private string m_DepartmentName;
        private int m_VendorCode = 0;
        private string m_VendorName;
        private string m_POcurrencyCode;
        private string m_POcurrencyDescription;
        private string m_VendorTerms;
        private string m_VendorTermsDescription;
        private short m_ShipTo;
        private string m_ShipVia;
        private string m_ShipViaDescription;
        private decimal m_LandingFactor;
        private string m_FreightChargeCode;
        private DateTime m_AnticipateDate;
        private DateTime m_OrderDate;
        private DateTime m_CancelDate;
        private DateTime m_ShipDate;
        private DateTime m_StageSetDate;
        private int m_PortOfDeparture;
        private string m_PortOfDepartureName;
        private int m_PortOfEntry;
        private string m_PortOfEntryName;
        private string m_DeliveryTerms;
        private string m_DeliveryTermsDescription;
        private bool m_hasNewLine;
        private String m_comment1;
        private String m_comment2;
        private String m_comment3;
        private String m_comment4;
        private String m_comment5;
        private String m_marketCurrency;
        private decimal m_POTotalUnits;
        private decimal m_POTotalLines;
        private decimal m_POTotalPacks;
        private decimal m_POTotalCost;
        private decimal m_POTotalRetail;
        private decimal m_POMarginValue;
        private decimal m_POMarginPercent;
        private decimal m_CurrencyRate;

        private const string DescriptionNotFound = "*** Description not found ***";
        private const string NameNotFound        = "*** Name not found ***";

        public PurchaseOrder(iDB2Connection DB2connection, Environments environment)
        {
            this.DB2connection = DB2connection;
            this.environment   = environment;
        }

        #region Properties
        public String SpicePOnumber
        {
            get { return m_SpicePOnumber; }
            set { m_SpicePOnumber = value; }  
        }

        public short SpicePOversion
        {
            get { return m_SpicePOversion; }
            set { m_SpicePOversion = value; }
        }

        public string IPPOnumber
        {
            get { return m_IPPOnumber; }
            set { m_IPPOnumber = value; }
        }

        public string Market
        {
            get { return m_Market; }
            set { m_Market = value; }
        }

        public string MarketName
        {
            get { return m_MarketName; }
            set { m_MarketName = value; }
        }

        public short Department
        {
            get { return m_Department; }
            set { m_Department = value; }
        }

        public string DepartmentName
        {
            get { return m_DepartmentName; }
            set { m_DepartmentName = value; }
        }

        public int VendorCode
        {
            get { return m_VendorCode; }
            set { m_VendorCode = value; }
        }

        public string VendorName
        {
            get { return m_VendorName; }
            set { m_VendorName = value; }
        }

        public string POcurrencyCode
        {
            get { return m_POcurrencyCode; }
            set { m_POcurrencyCode = value; }
        }

        public string POcurrencyDescription
        {
            get { return m_POcurrencyDescription; }
            set { m_POcurrencyDescription = value; }
        }

        public string VendorTerms
        {
            get { return m_VendorTerms; }
            set { m_VendorTerms = value; }
        }

        public string VendorTermsDescription
        {
            get { return m_VendorTermsDescription; }
            set { m_VendorTermsDescription = value; }
        }

        public short ShipTo
        {
            get { return m_ShipTo; }
            set { m_ShipTo = value; }
        }

        public string ShipVia
        {
            get { return m_ShipVia; }
            set { m_ShipVia = value; }
        }

        public string ShipViaDescription
        {
            get { return m_ShipViaDescription; }
            set { m_ShipViaDescription = value; }
        }

        public decimal LandingFactor
        {
            get { return m_LandingFactor; }
            set { m_LandingFactor = value; }
        }

        public string FreightChargeCode
        {
            get { return m_FreightChargeCode; }
            set { m_FreightChargeCode = value; }
        }

        public DateTime AnticipateDate
        {
            get { return m_AnticipateDate; }
            set { m_AnticipateDate = value; }
        }

        public DateTime OrderDate
        {
            get { return m_OrderDate; }
            set { m_OrderDate = value; }
        }

        public DateTime CancelDate
        {
            get { return m_CancelDate; }
            set { m_CancelDate = value; }
        }

        public DateTime ShipDate
        {
            get { return m_ShipDate; }
            set { m_ShipDate = value; }
        }

        public DateTime StageSetDate
        {
            get { return m_StageSetDate; }
            set { m_StageSetDate = value; }
        }

        public int PortOfDeparture
        {
            get { return m_PortOfDeparture; }
            set { m_PortOfDeparture = value; }
        }

        public string PortOfDepartureName
        {
            get { return m_PortOfDepartureName; }
            set { m_PortOfDepartureName = value; }
        }

        public int PortOfEntry
        {
            get { return m_PortOfEntry; }
            set { m_PortOfEntry = value; }
        }

        public string PortOfEntryName
        {
            get { return m_PortOfEntryName; }
            set { m_PortOfEntryName = value; }
        }

        public string DeliveryTerms
        {
            get { return m_DeliveryTerms; }
            set { m_DeliveryTerms = value; }
        }

        public string DeliveryTermsDescription
        {
            get { return m_DeliveryTermsDescription; }
            set { m_DeliveryTermsDescription = value; }
        }

        public bool hasNewLine
        {
            get { return m_hasNewLine; }
            set { m_hasNewLine = value; }
        }

        private DataTable dtPOlines;
        public DataTable POlines
        {
            get
            {
                return dtPOlines;
            }
        }

        public int NumberOfLines
        {
            get
            {
                return dtPOlines.Rows.Count;
            }
        }

        public String Comment1
        {
            get { return m_comment1; }
            set { m_comment1 = value; }
        }

        public String Comment2
        {
            get { return m_comment2; }
            set { m_comment2 = value; }
        }

        public String Comment3
        {
            get { return m_comment3; }
            set { m_comment3 = value; }
        }

        public String Comment4
        {
            get { return m_comment4; }
            set { m_comment4 = value; }
        }

        public String Comment5
        {
            get { return m_comment5; }
            set { m_comment5 = value; }
        }

        public String MarketCurrency
        {
            get { return m_marketCurrency; }
            set { m_marketCurrency = value; }
        }

        public decimal POTotalUnits
        {
            get { return m_POTotalUnits; }
            set { m_POTotalUnits = value; }
        }

        public decimal POTotalLines
        {
            get { return m_POTotalLines; }
            set { m_POTotalLines = value; }
        }

        public decimal POTotalPacks
        {
            get { return m_POTotalPacks; }
            set { m_POTotalPacks = value; }
        }

        public decimal POTotalCost
        {
            get { return m_POTotalCost; }
            set { m_POTotalCost = value; }
        }

        public decimal POTotalRetail
        {
            get { return m_POTotalRetail; }
            set { m_POTotalRetail = value; }
        }

        public decimal POMarginValue
        {
            get { return m_POMarginValue; }
            set { m_POMarginValue = value; }
        }

        public decimal POMarginPercent
        {
            get { return m_POMarginPercent; }
            set { m_POMarginPercent = value; }
        }

        public decimal CurrencyRate
        {
            get { return m_CurrencyRate; }
            set { m_CurrencyRate = value; }
        }

        #endregion

        public bool GetPurchaseOrder(String POnumber)
        {
            if (poheader == null) poheader = new POheader(DB2connection);
            dtHeader = poheader.GetPOheader(POnumber);

            if (dtHeader != null && dtHeader.Rows.Count != 0)
            {
                DataRow drHeader = dtHeader.Rows[0];

                m_SpicePOnumber = (String)drHeader["POHSPO"];
                m_SpicePOversion = Convert.ToInt16(drHeader["POHVER"]);

                m_IPPOnumber = (String)drHeader["POHIPO"];

                m_POTotalUnits = (Decimal)drHeader["POHUNI"];
                m_POTotalLines = (Decimal)drHeader["POHLIN"];
                m_POTotalPacks = (Decimal)drHeader["POHPCK"];
                m_POTotalCost = (Decimal)drHeader["POHCST"];
                m_POTotalRetail = (Decimal)drHeader["POHRET"];
                m_POMarginValue = (Decimal)drHeader["POHMAV"];
                m_POMarginPercent = (Decimal)drHeader["POHMAP"];
                m_marketCurrency = (String)drHeader["CSCUR#"];
                m_CurrencyRate = (Decimal)drHeader["POHCYR"];

                m_Market = (String)drHeader["POHMKT"];
                DAL.Markets market = new Markets(environment);
                m_MarketName = market.GetMarketDescription(m_Market);

                m_Department = Convert.ToInt16(drHeader["POHDPT"]);
                Departments departments = new Departments(DB2connection);
                m_DepartmentName = (departments.GetDepartmentDetails(m_Department)) ? departments.DepartmentName.Trim() : NameNotFound;

                m_VendorCode = int.Parse(drHeader["POHVEN"].ToString());

                Vendors vendors = new Vendors(DB2connection);
                m_VendorName = (vendors.GetVendorName(m_VendorCode)) ? vendors.Name.Trim() : NameNotFound;

                m_POcurrencyCode = (String)drHeader["POHCYC"];
                Currency currency = new Currency(DB2connection);
                m_POcurrencyDescription = (currency.GetCurrency(m_POcurrencyCode))
                            ? currency.CurrencyName.Trim() + " (" + CurrencyRate.ToString().Trim() + ")"
                            : "Description not found";

                //CurrencyRate = currency.CurrencyRate;
                m_VendorTerms = Convert.ToString(drHeader["POHTCD"]).Trim();
                PaymentTerms payterms = new PaymentTerms(DB2connection);
                try
                {
                    VendorTermsDescription = (payterms.GetPaymentTerms(m_VendorTerms)) ? payterms.Description : DescriptionNotFound;
                }
                catch (Exception)
                {
                    VendorTermsDescription = string.Empty;
                }                

                m_ShipTo = Convert.ToInt16(drHeader["POHSTR"]);

                m_ShipVia = Convert.ToString(drHeader["POHVIA"]).Trim();
                ShipVia shipvia = new EASBO.ShipVia(DB2connection);
                ShipViaDescription = (shipvia.GetShipViaDescription(m_ShipVia)) ? shipvia.Description.Trim() : DescriptionNotFound;

                m_LandingFactor = Decimal.Round((decimal)drHeader["POHLNF"], 2) - 1;
                m_FreightChargeCode = (string)drHeader["POHFRC"];

                m_AnticipateDate = ConvertToDateTime((decimal)drHeader["POHADT"]);
                m_OrderDate = ConvertToDateTime((decimal)drHeader["POHODT"]);
                m_CancelDate = ConvertToDateTime((decimal)drHeader["POHCDT"]);
                m_ShipDate = ConvertToDateTime((decimal)drHeader["POHSDT"]);
                m_StageSetDate = ConvertToDateTime((decimal)drHeader["POHSSD"]);

                Ports ports = new Ports(DB2connection);
                m_PortOfDeparture = Convert.ToInt32(drHeader["POHPDP"]);
                m_PortOfDepartureName = (ports.GetPortDetails(m_PortOfDeparture)) ? ports.Name.Trim() : "";
                m_PortOfEntry = Convert.ToInt32(drHeader["POHPEN"]);
                m_PortOfEntryName = (ports.GetPortDetails(m_PortOfEntry)) ? ports.Name.Trim() : "";

                m_DeliveryTerms = Convert.ToString(drHeader["POHDLV"]).Trim();
                EASBO.DeliveryTerms deliveryterms = new EASBO.DeliveryTerms(DB2connection);
                m_DeliveryTermsDescription = (deliveryterms.GetDeliveryTerms(m_DeliveryTerms)) ? deliveryterms.Description.Trim() : "";

                hasNewLine = ((string)drHeader["POHNPO"] == "Y") ? true : false;

                GetPOComments();

                GetPOlines();

                return true;
            }
            else
            {
                return false;
            }
        }

        private void GetPOlines()
        {
            decimal landedCost = 0;
            decimal qty = 0;
            decimal totalLandedCost = 0;

            POlines polines = new POlines(DB2connection);
            dtPOlines = polines.GetPOlines(m_SpicePOnumber, m_SpicePOversion);

            // JB: The following is a bit of a hack to get the total (landed) cost dynamically 
            // generated from the PO lines(DSSPPOI), instead of taking the calculated value 
            // stored in the header table (DSSPPOH).
            // This is to keep the total (landed) cost in line with the TotalLandedCost figure
            // in the PurchaseOrder class in the POBO assembly.
            // [*NOT IDEAL] The purchase order figures should ideally all be generated in the 
            // same place./JB
            foreach (DataRow POItem in dtPOlines.Rows)
            {
                decimal.TryParse(POItem["POILNC"].ToString(), out landedCost);
                decimal.TryParse(POItem["POIQTY"].ToString(), out qty);
                totalLandedCost += landedCost * qty;
            }
            m_POTotalCost = totalLandedCost;
        }


        private void GetPOComments()
        {
            POcomments pocomments = new POcomments(DB2connection);
            DataTable dtComments = pocomments.GetPOcomments(m_SpicePOnumber, m_SpicePOversion);

            foreach (DataRow drComments in dtComments.Rows)
            {
                if ((decimal)drComments["POCSEQ"] == 1 && (String)drComments["POCCTP"] == "V")
                {
                    Comment1 = Convert.ToString(drComments["POCCOM"]).Trim();
                    continue;
                }

                if ((decimal)drComments["POCSEQ"] == 2 && (String)drComments["POCCTP"] == "V")
                {
                    Comment2 = Convert.ToString(drComments["POCCOM"]).Trim();
                    continue;
                }

                if ((decimal)drComments["POCSEQ"] == 3 && (String)drComments["POCCTP"] == "V")
                {
                    Comment3 = Convert.ToString(drComments["POCCOM"]).Trim();
                    continue;
                }

                if ((decimal)drComments["POCSEQ"] == 1 && (String)drComments["POCCTP"] == "I")
                {
                    Comment4 = Convert.ToString(drComments["POCCOM"]).Trim();
                    continue;
                }

                if ((decimal)drComments["POCSEQ"] == 2 && (String)drComments["POCCTP"] == "I")
                {
                    Comment5 = Convert.ToString(drComments["POCCOM"]).Trim();
                    continue;
                }
            }
        }

        public DataTable GetPOcomponents(String POnumber, short POversion, short POsequence)
        {
            DAL.POcomponents components = new POcomponents(DB2connection);
            DataTable dt = components.GetPOcomponents(POnumber, POversion, POsequence);

            return dt;
        }

        private DateTime ConvertToDateTime(decimal Date)
        {
            if (Date != 0)
            {
                string StrDate = Date.ToString("00000000");
                int Year  = Convert.ToInt32(StrDate.Substring(0, 4));
                int Month = Convert.ToInt32(StrDate.Substring(4, 2));
                int Day   = Convert.ToInt32(StrDate.Substring(6, 2));
                return new DateTime(Year, Month, Day, 0, 0, 0);
            }
            else
            {
                return DateTime.MinValue;
            }
        }
    }
}