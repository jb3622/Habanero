using System;
using System.Collections.Generic;
using System.Text;
using Disney.DA.IP400;
using Disney.Menu;
using System.Data;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Disney.Spice.POBO
{
    public class PurchaseOrder
    {
        #region PO Properties
        
        //PO Header
        public enum POtype { DropShipSingle, DropShipMultiple, StandardDCPO }


        private int         _numofPOLines;
        private int         _numofPOPacks;
        private int         _totalUnits;
        private Decimal     _totalCost;
        private Decimal     _totalLandedCost;
        private Decimal     _totalRetail                = 0;
        private Decimal     _marginValue                = 0;
        private Decimal     _marginPercentage           = 0;
        private int         _portofdeparturecode;
        private String      _portofdeparturedesc;
        private int         _portofentrycode;
        private String      _portofentrydesc            = "";
        private String      _internalcomments1;
        private String      _internalcomments2;
        private short       _shipTo;
        private String      _defaultMarket;
        private String      _deltermsdesc               = "";
        private String      _vendorcomments1;
        private String      _vendorcomments2;
        private String      _deltermscode               = "";
        private String      _vendorcomments3;
        private Disney.Menu.Users _userName;
        private ASNA.VisualRPG.Runtime.Database _dbInternalParam;

        private DataTable   _dtDropShipMatrix;
        private String      _sStoreColumnNamePrefix = "Store_";

        private String _freight;

        private String _ipponumber;

        public DataTable dtDropShipMatrix
        {
            get { return _dtDropShipMatrix; }
            set { _dtDropShipMatrix = value; }
        }

        private string _spiceponumber;
        public string SpicePOnumber
        {
            get { return _spiceponumber; }
            set { _spiceponumber = value; }
        }

        private short _spicepoversion;
        public short SpicePOversion
        {
            get { return _spicepoversion; }
            set { _spicepoversion = value; }
        }

        private POtype _purchaseordertype;
        public POtype PurchaseOrderType
        {
            get { return _purchaseordertype; }
            set { _purchaseordertype = value; }
        }

        private bool _bisValid;
        public bool IsValid
        {
            get { return _bisValid; }
            set { _bisValid = value; }
        }

        private bool _isPONewLine;
        public bool IsPONewLine
        {
            get { return _isPONewLine; }
            set { _isPONewLine = value; }
        }

        private short _deptcode;
        public short Deptcode
        {
            get { return _deptcode; }
            set { _deptcode = value; }
        }

        private string _deptdesc;
        public string Deptdesc
        {
            get { return _deptdesc; }
        }

        private int _vendorcode;
        public int Vendorcode
        {
            get { return _vendorcode; }
            set { _vendorcode = value; }
        }

        private string _vendorDesc;
        public string VendorDesc
        {
            get { return _vendorDesc; }
        }

        private string _currencycode;
        public string Currencycode
        {
            get { return _currencycode; }
            set { _currencycode = value; }
        }

        private string _currencydesc;
        public string Currencydesc
        {
            get { return _currencydesc; }
        }

        private string _termscode;
        public string Termscode
        {
            get { return _termscode; }
            set { _termscode = value; }
        }

        private string _termsdesc;
        public string Termsdesc
        {
            get { return _termsdesc; }
            set { _termscode = value; }
        }

        private bool _shipSingle;
        public bool ShipSingle
        {
            get { return _shipSingle; }
            set { _shipSingle = value; }
        }

        private string[] _shippingStores;
        public String[] ShippingStores
        {
            get { return _shippingStores; }
            set { _shippingStores = value; }
        }

        private string _shipViaCode;
        public String ShipViaCode
        {
            get { return _shipViaCode; }
            set { _shipViaCode = value; }
        }

        private string _shipViaDesc;
        public String ShipViaDesc
        {
            get { return _shipViaDesc; }
        }

        private decimal _landing;
        public Decimal Landing
        {
            get { return _landing; }
            set { _landing = value; }
        }

        private DateTime _anticipateDate = DateTime.MinValue;
        public DateTime AnticipateDate
        {
            get { return _anticipateDate; }
            set { _anticipateDate = value; }
        }

        private DateTime _shippingDate = DateTime.MinValue;
        public DateTime ShippingDate
        {
            get { return _shippingDate; }
            set { _shippingDate = value; }
        }

        private DateTime _orderDate = DateTime.Now;
        public DateTime OrderDate
        {
            get { return _orderDate; }
        }

        private DateTime _ssd = DateTime.MinValue; //Us only
        public DateTime Ssd
        {
            get { return _ssd; }
            set { _ssd = value; }
        }

        private DateTime _cancelDate = DateTime.MinValue;
        public DateTime CancelDate
        {
            get { return _cancelDate; }
            set { _cancelDate = value; }
        }

        public int NumofPOLines
        {
            get { return _numofPOLines; }
            set { _numofPOLines = value; }
        }

        public int NumofPOPacks
        {
            get { return _numofPOPacks; }
            set { _numofPOPacks = value; }
        }
        
        public int TotalUnits
        {
            get { return _totalUnits; }
            set { _totalUnits = value; }
        }

        public Decimal TotalCost
        {
            get { return _totalCost; }
            set { _totalCost = value; }
        }

        public Decimal TotalLandedCost
        {
            get { return _totalLandedCost; }
            set { _totalLandedCost = value; }
        }

        public Decimal TotalRetail
        {
            get { return _totalRetail; }
            set { _totalRetail = value; }
        }

        public Decimal MarginValue
        {
            get { return _marginValue; }
            set { _marginValue = value; }
        }

        public Decimal MarginPercentage
        {
            get { return _marginPercentage; }
            set { _marginPercentage = value; }
        }
        
        public int Portofdeparturecode
        {
            get { return _portofdeparturecode; }
            set { _portofdeparturecode = value; }
        }

        public String Portofdeparturedesc
        {
            get { return _portofdeparturedesc; }
        }
        
        public int Portofentrycode
        {
            get { return _portofentrycode; }
            set { _portofentrycode = value; }
        }

        public String Portofentrydesc
        {
            get { return _portofentrydesc; }
        }
        
        public String Deltermscode
        {
            get { return _deltermscode; }
            set { _deltermscode = value; }
        }
        
        public String Deltermsdesc
        {
            get { return _deltermsdesc; }
        }

        public String Vendorcomments1
        {
            get { return _vendorcomments1; }
            set { _vendorcomments1 = value; }
        }

        public String Vendorcomments2
        {
            get { return _vendorcomments2; }
            set { _vendorcomments2 = value; }
        }
        
        public string Vendorcomments3
        {
            get { return _vendorcomments3; }
            set { _vendorcomments3 = value; }
        }

        public string Internalcomments1
        {
            get { return _internalcomments1; }
            set { _internalcomments1 = value; }
        }
        
        public string Internalcomments2
        {
            get { return _internalcomments2; }
            set { _internalcomments2 = value; }
        }

        public ASNA.VisualRPG.Runtime.Database DbParamRef
        {
            get { return _dbInternalParam; }
            set { _dbInternalParam = value; }
        }

        public short ShipTo
        {
            get { return _shipTo; }
            set { _shipTo = value; }
        }

        private POtype _poType;
        public POtype PoType
        {
            get { return _poType; }
            set { _poType = value; }
        }

        public Disney.Menu.Users UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string DefaultMarket
        {
            get { return _defaultMarket; }
            set { _defaultMarket = value; }
        }

        private string _smarketdescription;
        public string MarketDescription
        {
            get { return _smarketdescription; }
            set { _smarketdescription = value; }
        }

        private string _defaultCurrency;
        public string BaseCurrency
        {
            get { return _defaultCurrency; }
            set { _defaultCurrency = value; }
        }

        private decimal _exchangeRate;
        public decimal ExchangeRate
        {
            get { return _exchangeRate; }
            set { _exchangeRate = value; }
        }

        private string _marketCurrency;
        public string MarketCurrency
        {
            get { return _marketCurrency; }
            set { _marketCurrency = value; }
        }

        public string Freight
        {
            get { return _freight; }
            set { _freight = value; }
        }

        public string IPPOnumber
        {
            get { return _ipponumber; }
            set { _ipponumber = value; }
        }

        private string _orderStatus;
        public string OrderStatus
        {
            get { return _orderStatus; }
            set { _orderStatus = value;}
        }

        private Disney.Menu.Environments _penvironment;
        public Disney.Menu.Environments Penvironment
        {
            get { return _penvironment; }
            set { _penvironment = value; }
        }

        // Current PO Line Item
        int _poLineItemDetailsCurent;
        public int poLineItemsCurrent
        {
            get { return _poLineItemDetailsCurent; }
            set { _poLineItemDetailsCurent = value; }

        }
        
        // DataTable holding the PO Line Items as displayed in the Grid View.
        DataTable _dtpoLineItemDetails;
        public DataTable dtpoLineItemDetails
        {
            get { return _dtpoLineItemDetails; }
            set { _dtpoLineItemDetails = value; }
        }

        // List of PO Line Details (class POItemDetails)
        List<POItemDetails> _lstpoLineItemDetails;
        public List<POItemDetails> lstpoLineItemDetails
        {
            get { return _lstpoLineItemDetails; }
            set { _lstpoLineItemDetails = value; }
        }

        // Holds a list of PO Hits
        private PurchaseOrder.PoHitsCollection _pohitscollection;
        public PurchaseOrder.PoHitsCollection poHitsCollection
        {
            get { return _pohitscollection; }
            set { _pohitscollection = value; }
        }
        #endregion POproperties

        private IPDEPTS ipdeptscls;

        public PurchaseOrder(ASNA.VisualRPG.Runtime.Database dbparam, Disney.Menu.Users username, Disney.Menu.Environments paramenv)
        {
            _dbInternalParam = dbparam;
            _userName = username;
            _penvironment = paramenv;


            // HK : 9-11-2009
            // Initalize the PO Line Items properties

            DataTable _dtpoLineItemDetails = new DataTable();

            _dtpoLineItemDetails.Columns.Add("POnumber",       typeof(string));
            //_dtpoLineItemDetails.Columns.Add("Version",       typeof(Int16));     //Not populated
            //_dtpoLineItemDetails.Columns.Add("Sequence",      typeof(Int16));     //Not populated
            _dtpoLineItemDetails.Columns.Add("Class",          typeof(Int16));
            _dtpoLineItemDetails.Columns.Add("Vendor",         typeof(Int32));
            _dtpoLineItemDetails.Columns.Add("Style",          typeof(Int16));
            _dtpoLineItemDetails.Columns.Add("Colour",         typeof(Int16));
            _dtpoLineItemDetails.Columns.Add("Size",           typeof(Int16));
            // _dtpoLineItemDetails.Columns.Add("SKU",          typeof(Int32));       
            // dtPOLines.Columns.Add("CheckDigit",              typeof(Int16));
            _dtpoLineItemDetails.Columns.Add("UPC",            typeof(string));
            _dtpoLineItemDetails.Columns.Add("Quantity",       typeof(Int32));
            _dtpoLineItemDetails.Columns.Add("LandedCost",     typeof(decimal));   //Cost * Landing Factor
            _dtpoLineItemDetails.Columns.Add("Retail",         typeof(decimal));
            _dtpoLineItemDetails.Columns.Add("LongDesc",       typeof(string));
            _dtpoLineItemDetails.Columns.Add("ShortDesc",      typeof(string));
            _dtpoLineItemDetails.Columns.Add("VendorStyle",    typeof(string));
            _dtpoLineItemDetails.Columns.Add("Season",         typeof(string));
            _dtpoLineItemDetails.Columns.Add("SubClass",       typeof(string));
            _dtpoLineItemDetails.Columns.Add("Ticket",         typeof(string));
            _dtpoLineItemDetails.Columns.Add("CaseQuantity",   typeof(Int32));
            _dtpoLineItemDetails.Columns.Add("DistroQty",      typeof(Int32));
            _dtpoLineItemDetails.Columns.Add("VendorCost",     typeof(decimal));
            _dtpoLineItemDetails.Columns.Add("LandFactor",     typeof(decimal));
            _dtpoLineItemDetails.Columns.Add("Character",      typeof(string));    // Character code

            // Initalise the PO Line Items collection

            _lstpoLineItemDetails = new List<POItemDetails>();
        }

        public DataTable GetPOItems(string spiceponumber, short spicepoversion)
        {
            DataTable dtTemp;

            // Create the instance of the header class
            DSSPPOIcls poitemcls = new DSSPPOIcls(DbParamRef);

            dtTemp = poitemcls.GetPOlines(spiceponumber, _spicepoversion);

            return dtTemp;
        }

        public Boolean GetPreviousPOHeader(string spiceponumber, short currentversion)
        {
            Boolean bSuccess;

            // Create the instance of the header class
            DSSPPOHcls poheadercls = new DSSPPOHcls(DbParamRef, UserName);

            //bSuccess = poheadercls.GetPreviousPOheader (spiceponumber, currentversion);
            bSuccess = poheadercls.GetPreviousPOheader(spiceponumber);

            if (bSuccess == true)
            {

                // Assign common properties
                _defaultMarket  = poheadercls.Market;
                _deptcode       = poheadercls.Department;
                _vendorcode     = poheadercls.Vendor;
                _currencycode   = poheadercls.CurrencyCode;
                _exchangeRate   = poheadercls.CurrencyRate;
                _termscode      = poheadercls.Terms;

                _shipTo         = poheadercls.ShipTo;
                // HK : 11-12-2009 : poheadercls.ShipVia is returning a shipvia padded with spaces.
                // This will cause validation to fail on the txtShipVia. Ask CJ for advice???
                // HK : 12-12-2009 : This has now been fixed in the DA for shipvia. But 
                // others like vendor and internal still have to be resolved.
                _shipViaCode    = poheadercls.ShipVia;
                _landing        = poheadercls.LandingFactor;

                _anticipateDate = poheadercls.AnticipateDate;
                _shippingDate = poheadercls.ShipDate;
                _orderDate = poheadercls.OrderDate;
                _cancelDate = poheadercls.CancelDate;

                if (Penvironment.Domain == "SWNA")
                {
                    _ssd = poheadercls.StageSetDate;
                }

                _numofPOLines   = poheadercls.TotalLines;

                _totalUnits     = poheadercls.TotalUnits;

                _numofPOPacks   = poheadercls.TotalPacks; //(short)

                _numofPOLines   = poheadercls.TotalLines; //(short);

                _totalCost      = poheadercls.TotalCost;
                _totalRetail    = poheadercls.TotalRetail;

                _marginValue = poheadercls.Margin;
                _marginPercentage = poheadercls.MarginPercent;

                if (_shipViaCode == "OCN")
                {
                    _portofdeparturecode    = poheadercls.PortOfDeparture;
                    _portofentrycode        = poheadercls.PortOfEntry;
                    _deltermscode           = poheadercls.DeliveryTerms;
                }

                _isPONewLine = poheadercls.NewLine;

                _spiceponumber  = poheadercls.SpicePOnumber;
                _spicepoversion = poheadercls.SpicePOversion;

                // HK : 18-12-2009 : Freight Charge Code
                _freight = poheadercls.FreightChargeCode;

                // Capture the IP PO NUmber
                _ipponumber = poheadercls.IPPOnumber;
                _orderStatus = poheadercls.Status;
            }
            else
            {
                bSuccess = false;
            }

            return bSuccess;
        }

        public Boolean GetPOHeader(string spiceponumber)
        {
            Boolean bSuccess = false;
            
            DSSPPOHcls poheadercls = new DSSPPOHcls(DbParamRef, UserName);

            try 
            {
                bSuccess = poheadercls.GetPOheader(spiceponumber);
            }
            catch (Exception ex)
            {
                Debug.Print (ex.Message, "Error on Po Header class - GetPoHeader method");
            }

            if (bSuccess == true)
            {
                // Assign common properties
                _defaultMarket      = poheadercls.Market;
                _deptcode           = poheadercls.Department;
                _vendorcode         = poheadercls.Vendor;
                _currencycode       = poheadercls.CurrencyCode;
                _exchangeRate       = poheadercls.CurrencyRate;
                _termscode          = poheadercls.Terms;

                _shipTo             = poheadercls.ShipTo;
                // HK : 11-12-2009 : poheadercls.ShipVia is returning a shipvia padded with spaces.
                // This will cause validation to fail on the txtShipVia. Ask CJ for advice???
                // HK : 12-12-2009 : This has now been fixed in the DA for shipvia. But 
                // others like vendor and internal still have to be resolved.
                _shipViaCode        = poheadercls.ShipVia;
                _landing            = poheadercls.LandingFactor;

                _anticipateDate     = poheadercls.AnticipateDate;
                _shippingDate       = poheadercls.ShipDate;
                _orderDate          = poheadercls.OrderDate;
                _cancelDate         = poheadercls.CancelDate;

                if (Penvironment.Domain == "SWNA")
                {
                    _ssd = poheadercls.StageSetDate;
                }

                _numofPOLines       = poheadercls.TotalLines;

                _totalUnits = poheadercls.TotalUnits;

                _numofPOPacks = poheadercls.TotalPacks; //(short)

                _numofPOLines = poheadercls.TotalLines; //(short);

                _totalCost = poheadercls.TotalCost;
                _totalRetail = poheadercls.TotalRetail;

                _marginValue = poheadercls.Margin;
                _marginPercentage = poheadercls.MarginPercent;

                if (_shipViaCode == "OCN")
                {
                    _portofdeparturecode    = poheadercls.PortOfDeparture;
                    _portofentrycode        = poheadercls.PortOfEntry;
                    _deltermscode           = poheadercls.DeliveryTerms;
                }

                _isPONewLine                = poheadercls.NewLine;
                _spiceponumber              = poheadercls.SpicePOnumber;
                _spicepoversion             = poheadercls.SpicePOversion;

                // HK : 18-12-2009 : Freight Charge Code
                _freight = poheadercls.FreightChargeCode;

                // Capture the IP PO NUmber
                _ipponumber                 = poheadercls.IPPOnumber;
            }
            else
            {
                bSuccess = false;
            }

            return bSuccess;
        }

        public String GetPOStatus(string spiceponumber, out short changesequence)
        {
            Boolean bSuccess = false;

            DSSPPOHcls poheadercls = new DSSPPOHcls(DbParamRef, UserName);

            try
            {
                bSuccess = poheadercls.GetPOheader(spiceponumber);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message, "Error on Po Header class - GetPoHeader method");
            }

            if (bSuccess == true)
            {
                changesequence = poheadercls.ChangeSeq;
                return poheadercls.Status;
            }

            else
            {
                changesequence = -1;
                return String.Empty;
            }
        }

        public Boolean GetPOComments(string spiceponumber, short _spicepoversion)
        {
            Boolean bSuccess;

            // Create the instance of the header class
            DSSPPOC dsspcomments = new DSSPPOC(_dbInternalParam);

            bSuccess = dsspcomments.GetPOcomments (spiceponumber, _spicepoversion);

            if (bSuccess == true)
            {
                _vendorcomments1 = dsspcomments.Comment1;
                _vendorcomments2 = dsspcomments.Comment2;
                _vendorcomments3 = dsspcomments.Comment3;

                _internalcomments1 = dsspcomments.Comment4;
                _internalcomments2 = dsspcomments.Comment5;
            }
            else
            {
                _vendorcomments1 = String.Empty;
                _vendorcomments3 = String.Empty;
                _vendorcomments3 = String.Empty;

                _internalcomments1 = String.Empty;
                _internalcomments1 = String.Empty;
            }

            return true;
        }

        #region Create PO header
        public bool CreatePoHeader(int iHitNumber)
        {
            DSSPPOHcls poheadercls = new DSSPPOHcls(DbParamRef, UserName);

            poheadercls.Market          = _defaultMarket;
            poheadercls.Department      = _deptcode;
            poheadercls.Vendor          = _vendorcode;
            poheadercls.CurrencyCode    = _currencycode;
            poheadercls.CurrencyRate    = _exchangeRate;
            poheadercls.Terms           = _termscode;
            poheadercls.ShipTo          = _shipTo;
            poheadercls.ShipVia         = _shipViaCode;
            poheadercls.LandingFactor   = _landing;

            DateTime __anticipatedate = DateTime.Today;
            DateTime __shippingdate = DateTime.Today;
            string __canceldate = String.Empty;
            GetHitDates(iHitNumber, ref __anticipatedate, ref __shippingdate, ref __canceldate);

            _anticipateDate = __anticipatedate;
            _shippingDate   = __shippingdate;
            CancelDate      = Convert.ToDateTime(__canceldate);
            
            poheadercls.AnticipateDate  = _anticipateDate;
            poheadercls.ShipDate        = _shippingDate;
            poheadercls.OrderDate       = _orderDate;
            poheadercls.CancelDate = CancelDate;

            if (Penvironment.Domain == "SWNA")
            {
                poheadercls.StageSetDate = _ssd;
            }

            poheadercls.TotalLines = (short)NumofPOLines;

            int qty = GetTotalUnitsForHit(iHitNumber);
            _totalUnits = qty;
            poheadercls.TotalUnits = _totalUnits;

            _numofPOPacks = GetTotalPacksForHit(iHitNumber);
            poheadercls.TotalPacks = (short)_numofPOPacks;

            int totallines = GetTotalLinesForHit(iHitNumber);
            _numofPOLines = totallines;
            poheadercls.TotalLines = (short)totallines;

            //---------------------------------------
            Decimal landedcost = CalculateTotalCostInPoCurrency(iHitNumber);
            poheadercls.TotalRetail = CalculateTotalRetail(iHitNumber);
            Decimal retailExVat = CalculateTotalRetailExVat(iHitNumber);
            poheadercls.TotalCost = CalculateTotalCost(iHitNumber) * _landing;

            poheadercls.Margin = retailExVat - landedcost;          
            poheadercls.MarginPercent = (100 * poheadercls.Margin) / retailExVat;
           
            //---------------------------------------
            if (_shipViaCode == "OCN")
            {
                poheadercls.PortOfDeparture = _portofdeparturecode;
                poheadercls.PortOfEntry = _portofentrycode;
                poheadercls.DeliveryTerms = _deltermscode;
            }

            poheadercls.NewLine = _isPONewLine;
            poheadercls.IPuserInitials = _userName.IPinitials;

            poheadercls.FreightChargeCode = _freight;

            if (poheadercls.WritePOheader())
            {
                _spiceponumber = poheadercls.SpicePOnumber;
                _spicepoversion = poheadercls.SpicePOversion;
            }

            return true;
        }

        public bool CreatePOHeader()
        {
            DSSPPOHcls poheadercls = new DSSPPOHcls(DbParamRef, UserName);
            
            poheadercls.Market          = _defaultMarket;
            poheadercls.Department      = _deptcode;
            poheadercls.Vendor          = _vendorcode;
            poheadercls.CurrencyCode    = _currencycode;
            poheadercls.CurrencyRate    = _exchangeRate;
            poheadercls.Terms           = _termscode;
            poheadercls.ShipTo          = _shipTo;
            poheadercls.ShipVia         = _shipViaCode;
            poheadercls.LandingFactor   = _landing;
            poheadercls.AnticipateDate  = _anticipateDate;
            poheadercls.ShipDate        = _shippingDate;
            poheadercls.OrderDate       = _orderDate;
            poheadercls.CancelDate      = CancelDate;

            if (Penvironment.Domain == "SWNA")
            {
                poheadercls.StageSetDate = _ssd;
            }

            poheadercls.TotalLines = (short)NumofPOLines;

            if (PurchaseOrderType != POtype.DropShipMultiple)
            {
                poheadercls.TotalUnits  = _totalUnits;
                poheadercls.TotalPacks  = (short)_numofPOPacks;
                poheadercls.TotalLines  = (short)_numofPOLines;

                poheadercls.TotalCost = CalculateTotalCostInPoCurrency() * _landing;
                poheadercls.TotalRetail = _totalRetail;
                poheadercls.Margin = _marginValue;
                poheadercls.MarginPercent = _marginPercentage;
            }


            if (PurchaseOrderType == POtype.DropShipMultiple)
            {
                string sstore = poheadercls.ShipTo.ToString();
                
                int qty = GetStoreQuantityForAllItems(sstore);
                _totalUnits = qty;
                poheadercls.TotalUnits = _totalUnits;

                _numofPOPacks = GetTotalPacks(sstore);
                poheadercls.TotalPacks = (short)_numofPOPacks;

                int totallines = GetTotalLinesForStore(sstore);
                _numofPOLines = totallines;
                poheadercls.TotalLines = (short)totallines;

                _totalCost = CalculateTotalCostInPoCurrency(sstore) * _landing;
                poheadercls.TotalCost = _totalCost;
                
                _totalRetail = CalculateTotalRetail(sstore);
                poheadercls.TotalRetail = _totalRetail;

                poheadercls.Margin = _marginValue;
                poheadercls.MarginPercent = _marginPercentage;
            }

            if (_shipViaCode == "OCN")
            {
                poheadercls.PortOfDeparture = _portofdeparturecode;
                poheadercls.PortOfEntry = _portofentrycode;
                poheadercls.DeliveryTerms = _deltermscode;
            }

            poheadercls.NewLine = _isPONewLine;
            poheadercls.IPuserInitials = _userName.IPinitials;
            poheadercls.FreightChargeCode = _freight;

            if (poheadercls.WritePOheader())
            {
                _spiceponumber = poheadercls.SpicePOnumber;
                _spicepoversion = poheadercls.SpicePOversion;
            }

            return true;
        }
        #endregion

        public int GetItemQuantityOnHit(int ihitnumber, int iitemindex, short iClass, int iVendor,
                                        short iStyle, short iColor, short iSize)
        {
            int icollectionindexforhit;
            int itemp = 0;

            icollectionindexforhit = GetHitsCollectionIndexForSpecificHit(ihitnumber);

            for (int i = 0; i < poHitsCollection[icollectionindexforhit].dtPoHits.Rows.Count; i++)
            {
                if (Convert.ToInt32(poHitsCollection[icollectionindexforhit].dtPoHits.Rows[i]["ItemIndex"]) == iitemindex &&
                    Convert.ToInt32(poHitsCollection[icollectionindexforhit].dtPoHits.Rows[i]["Class"]) == iClass &&
                    Convert.ToInt32(poHitsCollection[icollectionindexforhit].dtPoHits.Rows[i]["Vendor"]) == iVendor &&
                    Convert.ToInt32(poHitsCollection[icollectionindexforhit].dtPoHits.Rows[i]["Style"]) == iStyle &&
                    Convert.ToInt32(poHitsCollection[icollectionindexforhit].dtPoHits.Rows[i]["Color"]) == iColor &&
                    Convert.ToInt32(poHitsCollection[icollectionindexforhit].dtPoHits.Rows[i]["Size"]) == iSize)
                {
                    itemp = Convert.ToInt32(poHitsCollection[icollectionindexforhit].dtPoHits.Rows[i]["Quantity"]);

                    return itemp;
                }
            }

            return 0;
        }

        private Decimal CalculateTotalRetail(int iHitNumber)
        {
            int icollectionindexforhit;
            decimal totalretail = 0;
            int iitemquatityonhit = 0;
            int iitemindex;
            short iClass;
            int iVendor;
            short iStyle;
            short iColor;
            short iSize;

            icollectionindexforhit = GetHitsCollectionIndexForSpecificHit(iHitNumber);

            if (icollectionindexforhit != -1)
            {
                foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
                {
                    iitemindex = poItemDetails.Itemindex;
                    iClass = poItemDetails.ClassCode;
                    iVendor = poItemDetails.Vendorcode;
                    iStyle = poItemDetails.Stylecode;
                    iColor = poItemDetails.Colorcode;
                    iSize = poItemDetails.Itemsize;

                    iitemquatityonhit = GetItemQuantityOnHit(iHitNumber, iitemindex, iClass, iVendor,
                                        iStyle, iColor, iSize);

                    totalretail = totalretail + (poItemDetails.Retailprice * iitemquatityonhit);
                }
            }

            return totalretail;
        }

        private Decimal CalculateTotalCostInPoCurrency(int iHitNumber)
        {
            decimal totalcost = 0;

            Int32 hitindex = GetHitsCollectionIndexForSpecificHit(iHitNumber);
            if (hitindex != -1)
            {
                foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
                {
                    Int32 iitemindex = poItemDetails.Itemindex;
                    Int16 iClass  = poItemDetails.ClassCode;
                    Int32 iVendor = poItemDetails.Vendorcode;
                    Int16 iStyle  = poItemDetails.Stylecode;
                    Int16 iColor  = poItemDetails.Colorcode;
                    Int16 iSize   = poItemDetails.Itemsize;

                    Int32 iitemquatityonhit = GetItemQuantityOnHit(iHitNumber, iitemindex, iClass, iVendor,
                                        iStyle, iColor, iSize);

                    totalcost += poItemDetails.LandedCost * iitemquatityonhit;
                }
            }

            return totalcost;
        }

        private Decimal CalculateTotalCost(int iHitNumber)
        {
            decimal totalcost = 0;

            Int32 hitindex = GetHitsCollectionIndexForSpecificHit(iHitNumber);
            if (hitindex != -1)
            {
                foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
                {
                    Int32 iitemindex = poItemDetails.Itemindex;
                    Int16 iClass = poItemDetails.ClassCode;
                    Int32 iVendor = poItemDetails.Vendorcode;
                    Int16 iStyle = poItemDetails.Stylecode;
                    Int16 iColor = poItemDetails.Colorcode;
                    Int16 iSize = poItemDetails.Itemsize;

                    Int32 iitemquatityonhit = GetItemQuantityOnHit(iHitNumber, iitemindex, iClass, iVendor,
                                        iStyle, iColor, iSize);

                    totalcost += poItemDetails.ConvertedCost * iitemquatityonhit;
                }
            }

            return totalcost;
        }

        // HK : 04-12-2009 : Get the total quantity on a particular hit
        // If total quantity is 0 then no need to create PO
        private int GetTotalQuantityOnHit(int iHitNumber)
        {
            int icollectionindexforhit;
            int itotalquantityonhit = 0;

            icollectionindexforhit = GetHitsCollectionIndexForSpecificHit(iHitNumber);

            if (icollectionindexforhit != -1)
            {
                for (int i = 0; i < _pohitscollection[icollectionindexforhit].dtPoHits.Rows.Count; i++)
                {
                    itotalquantityonhit = itotalquantityonhit + 
                            Convert.ToInt32(_pohitscollection[icollectionindexforhit].dtPoHits.Rows[i]["Quantity"]);
                }
            }

            return itotalquantityonhit;
        }

        private int GetTotalLinesForHit(int iHitNumber)
        {
            int icollectionindexforhit;
            int itotallines = 0;

            icollectionindexforhit = GetHitsCollectionIndexForSpecificHit(iHitNumber);

            if (icollectionindexforhit != -1)
            {
                for (int i = 0; i < _pohitscollection[icollectionindexforhit].dtPoHits.Rows.Count; i++)
                {
                    if (Convert.ToInt32(_pohitscollection[icollectionindexforhit].dtPoHits.Rows[i]["Quantity"]) != 0)
                    {
                        itotallines = itotallines + 1;
                    }
                }
            }

            return itotallines;
        }

        private int GetTotalPacksForHit(int iHitNumber)
        {
            int icollectionindexforhit;
            int inumofpacks = 0;
            int iquantity;

            icollectionindexforhit = GetHitsCollectionIndexForSpecificHit(iHitNumber);

            if (icollectionindexforhit != -1)
            {
                for (int i = 0; i < _pohitscollection[icollectionindexforhit].dtPoHits.Rows.Count; i++)
                {
                    if (Convert.ToString (_pohitscollection[icollectionindexforhit].dtPoHits.Rows[i]["Pack"]) == "Y")
                    {
                        // Get the quantity assigned by the user for this item.
                        // If the quantity is 0 thenb this line will not be wrrite to the database.
                        // If quantity > 0 then increment the pack count
                        iquantity = 0;
                        iquantity = Convert.ToInt32(_pohitscollection[icollectionindexforhit].dtPoHits.Rows[i]["Quantity"]);
                        if (iquantity > 0)
                        {
                            inumofpacks = inumofpacks + 1;
                        }
                    }
                }
           }

           return inumofpacks;

        }

        public int GetTotalUnitsForHit(int ihitnumber)
        {
            int icollectionindexforhit;
            int itemp = 0;
            int itotalunits = 0;

            icollectionindexforhit = GetHitsCollectionIndexForSpecificHit(ihitnumber);

            if (icollectionindexforhit != -1)
            {
                for (int i = 0; i < _pohitscollection[icollectionindexforhit].dtPoHits.Rows.Count; i++)
                {
                    itemp = Convert.ToInt32(_pohitscollection[icollectionindexforhit].dtPoHits.Rows[i]["Quantity"]);
                    itotalunits = itotalunits + itemp;
                }
            }

            return itotalunits;
        }

        public void GetHitDates(int ihitnumber, ref DateTime anticipatedate, ref DateTime shippingdate, ref string canceldate)
        {
            int icollectionindexforhit;

            icollectionindexforhit = GetHitsCollectionIndexForSpecificHit(ihitnumber);

            if (icollectionindexforhit != -1)
            {
                anticipatedate = poHitsCollection[icollectionindexforhit].AnticipateDate;
                shippingdate = poHitsCollection[icollectionindexforhit].ShippingDate;
                canceldate = poHitsCollection[icollectionindexforhit].CancelDate;
            }
        }

        public void GetHitComments(int ihitnumber, ref string vendor1, ref string vendor2, ref string vendor3,
                                        ref string internal1, ref string internal2)
        {
            int icollectionindexforhit;

            icollectionindexforhit = GetHitsCollectionIndexForSpecificHit(ihitnumber);

            if (icollectionindexforhit != -1)
            {
                vendor1 = poHitsCollection[icollectionindexforhit].Vendorcomments1;
                vendor2 = poHitsCollection[icollectionindexforhit].Vendorcomments2;
                vendor3 = poHitsCollection[icollectionindexforhit].Vendorcomments3;
                internal1 = poHitsCollection[icollectionindexforhit].Internalcomments1;
                internal2 = poHitsCollection[icollectionindexforhit].Internalcomments2;
            }
        }

        public bool ModifyPOHeader(string spiceponumber, out short spicepoversionprevious)
        {
            spicepoversionprevious = 0;

            DSSPPOHcls poheadercls = new DSSPPOHcls(DbParamRef, UserName);

            poheadercls.Market          = _defaultMarket;
            poheadercls.Department      = _deptcode;
            poheadercls.Vendor          = _vendorcode;
            poheadercls.CurrencyCode    = _currencycode;
            poheadercls.CurrencyRate    = _exchangeRate;
            poheadercls.Terms           = _termscode;

            poheadercls.ShipTo          = _shipTo;

            poheadercls.ShipVia         = _shipViaCode;
            poheadercls.LandingFactor   = _landing;

            poheadercls.AnticipateDate  = _anticipateDate;
            poheadercls.ShipDate        = _shippingDate;
            poheadercls.OrderDate       = _orderDate;

            poheadercls.CancelDate      = CancelDate;

            if (Penvironment.Domain == "SWNA")
            {
                poheadercls.StageSetDate = _ssd;
            }

            poheadercls.TotalLines      = (short)NumofPOLines;
            poheadercls.TotalUnits      = _totalUnits;
            poheadercls.TotalPacks      = (short)_numofPOPacks;
            poheadercls.TotalLines      = (short)_numofPOLines;

            // HK : 22-01-2010 : Fix Bug 304
            // Send the Total Landed Cost in PO Currency
            //poheadercls.TotalCost = _totalCost;
            poheadercls.TotalCost       = CalculateTotalCostInPoCurrency() * _landing;
            poheadercls.TotalRetail     = _totalRetail;

            //decimal _totalcostincludinglanding = Decimal.Round(_totalCost * _landing, 2);
            //poheadercls.Margin                 = (_totalRetail - (_totalcostincludinglanding));
            //poheadercls.MarginPercent          = (100 * _totalCost / _totalRetail);

            poheadercls.Margin = _marginValue;
            poheadercls.MarginPercent = _marginPercentage;

            if (_shipViaCode == "OCN")
            {
                poheadercls.PortOfDeparture = _portofdeparturecode;
                poheadercls.PortOfEntry     = _portofentrycode;
                poheadercls.DeliveryTerms   = _deltermscode;
            }

            poheadercls.NewLine         = _isPONewLine;
            poheadercls.IPuserInitials  = _userName.IPinitials;

            // HK : 18-12-2009 : Frieght Charge Code
            poheadercls.FreightChargeCode = _freight;

            // HK : FC : CJONES : Pass IPPonumber
            poheadercls.IPPOnumber = _ipponumber;

            if (poheadercls.UpdatePOheader (spiceponumber))
            {
                _spiceponumber = poheadercls.SpicePOnumber;
                _spicepoversion = poheadercls.SpicePOversion;

                // HK : CJ : 15-12-2009 : Get the previous version number
                spicepoversionprevious = poheadercls.POPreviousVer;
            }
            else
            {
                return false;
            }

            return true;
        }

        // /HK : 30-11-2009 : Get the total packs for the store 
        // where quantity is greater then 0.
        private int GetTotalPacks(string sstore)
        {
            int inumofpopacks = 0;
            int iquantityassigned;
            string sstorecolumnname = GetStoreColumnNameFromStore(sstore);

            for (int i = 0; i < dtDropShipMatrix.Rows.Count; i++)
            {
                if ( Convert.ToString (dtDropShipMatrix.Rows[i]["Pack"]) == "Y")
                {
                    iquantityassigned = Convert.ToInt32(dtDropShipMatrix.Rows[i][sstorecolumnname]);
                    if (iquantityassigned > 0)
                    {
                        inumofpopacks = inumofpopacks + 1;
                    }
                }
            }

            return inumofpopacks;
        }

        private String GetStoreColumnNameFromStore(string sstore)
        {
            return _sStoreColumnNamePrefix + sstore;
        }

        // /HK : 28-11-2009 : Get the total lines for the store 
        // where quantity is greater then 0.
        private int GetTotalLinesForStore(string sstore)
        {
            int iTemp;
            int ilines = 0;
            string sstorecolumnname = GetStoreColumnNameFromStore(sstore);

            for (int i = 0; i < dtDropShipMatrix.Rows.Count; i++)
            {
                iTemp = Convert.ToInt32(dtDropShipMatrix.Rows[i][sstorecolumnname]);
                if (iTemp > 0)
                {
                    ilines++;
                }
            }

            return ilines;
        }

        // /HK : 28-11-2009 : Get the sum of item quantities entered
        // across a specified store
        private int GetStoreQuantityForAllItems(string sstore)
        {
            int iTemp;
            int istorequantity = 0;
            string sstorecolumnname = GetStoreColumnNameFromStore(sstore);

            for (int i = 0; i < dtDropShipMatrix.Rows.Count; i++)
            {
                iTemp = Convert.ToInt32(dtDropShipMatrix.Rows[i][sstorecolumnname]);
                istorequantity = istorequantity + iTemp;
            }

            return istorequantity;
        }

        public bool CreatePOComments(int iHitNumber)
        {
            string v1 = String.Empty;
            string v2 = String.Empty;
            string v3 = String.Empty;
            string i1 = String.Empty;
            string i2 = String.Empty;

            try
            {
                DSSPPOC dsspcomments = new DSSPPOC(_dbInternalParam);

                GetHitComments(iHitNumber, ref v1, ref v2, ref v3, ref i1, ref i2);

                dsspcomments.Comment1 = String.IsNullOrEmpty(v1) ? "" : v1;
                dsspcomments.Comment2 = String.IsNullOrEmpty(v2) ? "" : v2;
                dsspcomments.Comment3 = String.IsNullOrEmpty(v3) ? "" : v3;
                dsspcomments.Comment4 = String.IsNullOrEmpty(i1) ? "" : i1;
                dsspcomments.Comment5 = String.IsNullOrEmpty(i2) ? "" : i2;

                // HK : CJ : 19-11-2009 : Add the spicy Po number and spicy po version to the 
                // comments record
                dsspcomments.SpicePOnumber = _spiceponumber;
                dsspcomments.SpicePOversion = _spicepoversion;

                return dsspcomments.WritePOcomments();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ModifyPOComments(short spicepoversionprevious)
        {
            try
            {
                DSSPPOC dsspcomments = new DSSPPOC(_dbInternalParam);

                dsspcomments.Comment1 = String.IsNullOrEmpty(_vendorcomments1) ? "" : _vendorcomments1;
                dsspcomments.Comment2 = String.IsNullOrEmpty(_vendorcomments2) ? "" : _vendorcomments2;
                dsspcomments.Comment3 = String.IsNullOrEmpty(_vendorcomments3) ? "" : _vendorcomments3;
                dsspcomments.Comment4 = String.IsNullOrEmpty(_internalcomments1) ? "" : _internalcomments1;
                dsspcomments.Comment5 = String.IsNullOrEmpty(_internalcomments2) ? "" : _internalcomments2;

                // HK : CJ : 19-11-2009 : Add the spicy Po number and spicy po version to the 
                // comments record
                dsspcomments.SpicePOnumber  = _spiceponumber;
                dsspcomments.SpicePOversion = _spicepoversion;

                return dsspcomments.UpdatePOcomments(_spiceponumber, spicepoversionprevious, _spicepoversion);

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CreatePOComments()
        {
            try
            {
                DSSPPOC dsspcomments = new DSSPPOC(_dbInternalParam);

                dsspcomments.Comment1 = String.IsNullOrEmpty(_vendorcomments1) ? "" : _vendorcomments1;
                dsspcomments.Comment2 = String.IsNullOrEmpty(_vendorcomments2) ? "" : _vendorcomments2;
                dsspcomments.Comment3 = String.IsNullOrEmpty(_vendorcomments3) ? "" : _vendorcomments3;
                dsspcomments.Comment4 = String.IsNullOrEmpty(_internalcomments1) ? "" : _internalcomments1;
                dsspcomments.Comment5 = String.IsNullOrEmpty(_internalcomments2) ? "" : _internalcomments2;

                // HK : CJ : 19-11-2009 : Add the spicy Po number and spicy po version to the 
                // comments record
                dsspcomments.SpicePOnumber  = _spiceponumber;
                dsspcomments.SpicePOversion = _spicepoversion;

                return dsspcomments.WritePOcomments();
            }

            catch (Exception)
            {
                return false;
            }
        }

        public void PopulatePOLines()
        {
            int poLineCount, iCount;
            
            // Local variable to assign a real instance of POItemDetails
            POItemDetails polinedetails;

            // Clear existing data in the datatable
            _dtpoLineItemDetails.Clear();
            
            // Total lines Po Lines
            poLineCount = lstpoLineItemDetails.Count;

            for (iCount = 0; iCount < poLineCount; iCount++)
            {
                polinedetails = lstpoLineItemDetails[iCount];

                _dtpoLineItemDetails.Rows.Add(SpicePOnumber,
                    // 0, Version
                    // 0, polinedetails.Sequence,
                    polinedetails.ClassCode,
                    polinedetails.Vendorcode,
                    polinedetails.Stylecode,
                    polinedetails.Colorcode,
                    polinedetails.Itemsize,
                    //        polinedetails.Sku,//SKU
                    //        0,//Check Digit
                    polinedetails.UPC,//UPC
                    polinedetails.Itemquantity,
                    //        (polinedetails.Cost * porder.Landing * polinedetails.Itemquantity), //Landed cost = Cost*LF
                    (polinedetails.Cost * Landing), //Landed cost = Cost*LF
                    //        polinedetails.Retailprice * _polinedetails.Itemquantity,
                    polinedetails.Retailprice,
                    polinedetails.Itemlongdescription,
                    polinedetails.Itemshortdescription,
                    polinedetails.Vendorstyle,
                    polinedetails.SeasonDesc,
                    polinedetails.Subclass,
                    polinedetails.Tickettype,
                    polinedetails.CasePackQty,
                    polinedetails.DistroQty,
                    //        polinedetails.Cost * _polinedetails.Itemquantity, //Vendor Cost 
                    polinedetails.Cost, //Vendor Cost
                              Landing,
                    polinedetails.Charactercode
                );
            }
        }

        public Boolean ItemPurchaseVATRateLookup(string defaultmarket, string storevatcode, string itemvatcode, ref decimal dPurchaseVatrate)
        {
            DSSPIVR dsspivr = new DSSPIVR(_dbInternalParam);

            Boolean bSuccess = dsspivr.GetVATrate(defaultmarket, storevatcode, itemvatcode);

            if (bSuccess)
            {
                dPurchaseVatrate = dsspivr.PurchasesVATrate;
                return bSuccess;
            }

            return bSuccess;
        }

        public Boolean ItemSalesVATRateLookup(string defaultmarket, string storevatcode, string itemvatcode, ref decimal dSalesVatrate)
        {
            DSSPIVR dsspivr = new DSSPIVR(_dbInternalParam);

            Boolean bSuccess = dsspivr.GetVATrate(defaultmarket, storevatcode, itemvatcode);

            if (bSuccess)
            {
                dSalesVatrate = dsspivr.SalesVATrate;
                return bSuccess;
            }

            return bSuccess;
        }

        // HK : 11-11-2009 : Calculate the Total Retail (property TotalRetail)
        // for  all lines on the PO
        public Decimal CalculateTotalRetail()
        {
            decimal totalretail = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                totalretail = totalretail + (poItemDetails.Retailprice * poItemDetails.Itemquantity);
            }

            return totalretail;
        }

        public Decimal CalculateTotalCostInPoCurrency()
        {
            decimal totalcost = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                totalcost = totalcost + (poItemDetails.ConvertedCost * poItemDetails.Itemquantity);
            }

            return totalcost;
        }

        public Decimal CalculateTotalCostInPoCurrency(string sstore)
        {
            decimal totalcost = 0;

            // HK : Fix Bug 304 : TotalCost = Total Landed Cost in PO currency

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {

                //totalcost = totalcost + (poItemDetails.Cost * poItemDetails.Itemquantity);
                // Get the Item Quantity Assigned to a specific store
                int qty = GetItemQuantityForStore(sstore, poItemDetails.Itemindex,
                                                          poItemDetails.ClassCode,
                                                          poItemDetails.Vendorcode,
                                                          poItemDetails.Stylecode,
                                                          poItemDetails.Colorcode,
                                                          poItemDetails.Itemsize);

                totalcost = totalcost + (poItemDetails.ConvertedCost * qty);
            }

            return totalcost;
        }

        public Decimal CalculateTotalCost()
        {
            decimal totalcost = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                totalcost = totalcost + (poItemDetails.Cost * poItemDetails.Itemquantity);
            }

            return totalcost;
        }

        public Decimal CalculateTotalLandedCost()
        {
            decimal totalcost = 0;

            // HK : Fix Bug 304 : TotalCost = Total Landed Cost in PO currency
            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                totalcost += poItemDetails.LandedCost * poItemDetails.Itemquantity;
            }

            return totalcost;
        }

        // HK : 29-11-2009 : // HK : Get the TotalCost for a apecific store
        // for  all lines on the PO
        public Decimal CalculateTotalCost(string sstore)
        {
            decimal totalcost = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                int qty = GetItemQuantityForStore(sstore, poItemDetails.Itemindex, 
                                                          poItemDetails.ClassCode, 
                                                          poItemDetails.Vendorcode,
                                                          poItemDetails.Stylecode, 
                                                          poItemDetails.Colorcode, 
                                                          poItemDetails.Itemsize);

                totalcost += poItemDetails.LandedCost * qty;
            }

            return totalcost;
        }

        public Decimal CalculateTotalRetail(string sstore)
        {
            decimal totalretail = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                // Get the Item Quantity for store
                int qty = GetItemQuantityForStore(sstore, poItemDetails.Itemindex, 
                                                          poItemDetails.ClassCode, 
                                                          poItemDetails.Vendorcode,
                                                          poItemDetails.Stylecode, 
                                                          poItemDetails.Colorcode, 
                                                          poItemDetails.Itemsize);

                //totalretail = totalretail + (poItemDetails.Retailprice * poItemDetails.Itemquantity);
                totalretail = totalretail + (poItemDetails.Retailprice * qty);
            }

            return totalretail;
        }

        public int GetItemQuantityForStore(string sstore, int iItemIndex, short iClass, int iVendor, short iStyle, short iColor, short iSize)
        {
            int     iTemp       = 0;
            int     iitemindex  = 0;
            short   iclass      = 0;
            int     ivendor     = 0;
            short   istyle      = 0;
            short   icolor      = 0;
            short   isize       = 0;
            string  sstorecolumnname = GetStoreColumnNameFromStore(sstore);

            for (int i = 0; i < dtDropShipMatrix.Rows.Count; i++)
            {
                iitemindex  = Convert.ToInt32(dtDropShipMatrix.Rows[i]["ItemIndex"]);
                iclass      = Convert.ToInt16(dtDropShipMatrix.Rows[i]["Class"]);
                ivendor     = Convert.ToInt32(dtDropShipMatrix.Rows[i]["Vendor"]);
                istyle      = Convert.ToInt16(dtDropShipMatrix.Rows[i]["Style"]);
                icolor      = Convert.ToInt16(dtDropShipMatrix.Rows[i]["Color"]);
                isize       = Convert.ToInt16(dtDropShipMatrix.Rows[i]["Size"]);

                if (iItemIndex  == iitemindex   && iClass   == iclass && 
                    iVendor     == ivendor      && iStyle   == istyle &&
                    iColor      == icolor       && iSize    == isize)
                {
                    iTemp = Convert.ToInt32(dtDropShipMatrix.Rows[i][sstorecolumnname]);
                    
                    return iTemp;
                }
            }

            return iTemp;

            /*
            int istorequantity = 0;
            string sfilterexpr;
            sfilterexpr = "Class = '" + iClass.ToString() + "' and " +
              "Vendor = '" + iVendor.ToString() + "' and " +
              "Style = '" + iStyle.ToString() + "' and " +
              "Color = '" + iColor.ToString() + "' and " +
              "Size = '" + iSize.ToString() + "'";
             */

            // string scomputeexpression = "Sum (Convert.ToInt32 (sstore))
            //iTemp = Convert.ToInt32(dtDropShipMatrix.Compute(scomputeexpression, sfilterexpr));

        }

        // Calculate the Total Retail Ex VAT for  a single PO Item
        public Decimal CalculateTotalRetailExVatItem(POItemDetails poItemDetails, string storevatcode)
        {
            decimal itemretailexvat = 0, itemsalesvatrate = 0;

            Boolean bSuccess = ItemSalesVATRateLookup(DefaultMarket, storevatcode, poItemDetails.VatCode, ref itemsalesvatrate);

            if (bSuccess)
            {
                itemretailexvat = (poItemDetails.Retailprice / (1 + itemsalesvatrate)) * poItemDetails.Itemquantity;
            }

            return itemretailexvat;
        }

        // Calculate the Total Retail Ex VAT for all lines on the PO
        public Decimal CalculateTotalRetailExVat(string storevatcode)
        {
            decimal totalretailexvat = 0;
            decimal itemretailexvat = 0;
            decimal itemsalesvatrate = 0;
            
            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                Boolean bSuccess = ItemSalesVATRateLookup(DefaultMarket, storevatcode, poItemDetails.VatCode, ref itemsalesvatrate);

                if (bSuccess)
                {
                    itemretailexvat = (poItemDetails.Retailprice / (1 + itemsalesvatrate)) * poItemDetails.Itemquantity;
                    totalretailexvat = totalretailexvat + itemretailexvat;
                }
            }

            return totalretailexvat;
        }

        public Decimal CalculateTotalRetailExVat(int iHitNumber)
        {
            Decimal totalretailexvat = 0, itemsalesvatrate = 0;

            Int32 hitindex = GetHitsCollectionIndexForSpecificHit(iHitNumber);
            if (hitindex != -1)
            {
                foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
                {
                    Int32 iitemindex = poItemDetails.Itemindex;
                    Int16 iClass     = poItemDetails.ClassCode;
                    Int32 iVendor    = poItemDetails.Vendorcode;
                    Int16 iStyle     = poItemDetails.Stylecode;
                    Int16 iColor     = poItemDetails.Colorcode;
                    Int16 iSize      = poItemDetails.Itemsize;

                    Int32 qtyonhit = GetItemQuantityOnHit(iHitNumber, iitemindex, iClass, iVendor,
                                        iStyle, iColor, iSize);

                    Boolean bSuccess = ItemSalesVATRateLookup(DefaultMarket, "A", poItemDetails.VatCode, ref itemsalesvatrate);

                    if (bSuccess)
                    {
                        Decimal itemretailexvat = (poItemDetails.Retailprice / (1 + itemsalesvatrate)) * qtyonhit;
                        totalretailexvat += itemretailexvat;
                    }
                }
            }

            return totalretailexvat;
        }

        public int CalculateTotalUnit()
        {
            int totalunits = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                totalunits = totalunits + poItemDetails.Itemquantity;
            }

            return totalunits;
        }

        private int GetHitsCollectionIndexForSpecificHit(int iHitNumber)
        {

            if (iHitNumber >= 2 && iHitNumber <= 6)
            {
                return (iHitNumber - 2);
            }

            // Invalid hit number
            return -1;
        }

        public class POHits
        {
            private Boolean _hitactivated;
            private Boolean _hitcreated;
            private Boolean _hitinitalised;
            private int     _hitnumber;


            private DateTime _anticipateDate    = DateTime.Today;
            private DateTime _shippingDate      = DateTime.Today;
            private string _cancelDate;
            
            private string _vendorcomments1;
            private string _vendorcomments2;
            private string _vendorcomments3;
            
            private string _internalcomments1;
            private string _internalcomments2;

            private DataTable _dtPoHits;

            private DataTable _dtPoHitsOriginal;

            public int HitNUmber
            {
                get { return _hitnumber; }
                set { _hitnumber = value; }

            }

            public DateTime AnticipateDate
            {
                get { return _anticipateDate; }
                set { _anticipateDate = value; }
            }

            public DateTime ShippingDate
            {
                get { return _shippingDate; }
                set { _shippingDate = value; }
            }

            public string CancelDate
            {
                get { return _cancelDate; }
                set { _cancelDate = value; }

            }

            public string Vendorcomments1
            {
                get { return _vendorcomments1; }
                set { _vendorcomments1 = value; }
            }

            public string Vendorcomments2
            {
                get { return _vendorcomments2; }
                set { _vendorcomments2 = value; }
            }

            public string Vendorcomments3
            {
                get { return _vendorcomments3; }
                set { _vendorcomments3 = value; }
            }

            public string Internalcomments1
            {
                get { return _internalcomments1; }
                set { _internalcomments1 = value; }
            }

            public string Internalcomments2
            {
                get { return _internalcomments2; }
                set { _internalcomments2 = value; }
            }

            public DataTable dtPoHits
            {
                get { return _dtPoHits; }
                set { _dtPoHits = value; }
            }

            public Boolean HitActivated
            {
                get { return _hitactivated; }
                set { _hitactivated = value; }
            }

            public Boolean HitCreated
            {
                get { return _hitcreated; }
                set { _hitcreated = value; }
            }

            public Boolean HitInitalised
            {
                get { return _hitinitalised; }
                set { _hitinitalised = value; }
            }

            public DataTable dtPoHitsOriginal
            {
                get { return _dtPoHitsOriginal; }
                set { _dtPoHitsOriginal = value; }
            }

            public void CreateCopyOfPoHit()
            {
                // Clear and copy
                _dtPoHitsOriginal.Clear();
                _dtPoHitsOriginal = _dtPoHits.Copy();
            }

            public void RevertCopyOfPoHit()
            {
                // Clear and copy
                _dtPoHits.Clear ();
                _dtPoHits = _dtPoHitsOriginal.Copy();
            }
            
            public POHits()
            {
                _dtPoHits = new DataTable();
                _dtPoHits.Columns.Add("colSelect",      typeof(Boolean));
                _dtPoHits.Columns.Add("ItemIndex",      typeof(String));
                _dtPoHits.Columns.Add("Pack",           typeof(String));
                _dtPoHits.Columns.Add("Class",          typeof(String));
                _dtPoHits.Columns.Add("Vendor",         typeof(String));
                _dtPoHits.Columns.Add("Style",          typeof(String));
                _dtPoHits.Columns.Add("Color",          typeof(String));
                _dtPoHits.Columns.Add("Size",           typeof(String));
                _dtPoHits.Columns.Add("Description",    typeof(String));
                _dtPoHits.Columns.Add("Quantity",       typeof(String));

                //HK : 01-12-2009 : Initalize the Cancel date
                _cancelDate = ShippingDate.ToLongDateString();

                // HK : 16-01-2010
                _dtPoHitsOriginal = new DataTable();
            }
        }

        public class PoHitsCollection : CollectionBase
        {
            public POHits this[int index]
            {
                get { return (POHits)List[index]; }

                set { List[index] = value; }

            }

            public void Add(POHits pohits)
            {
                this.List.Add(pohits);
            }

            public void Remove(POHits pohits)
            {
                this.List.Remove(pohits);
            }

            public int IndexOf(POHits pohits)
            {
                return (this.List.IndexOf(pohits));
            }

            public void Insert(int index, POHits pohits)
            {
                this.List.Insert(index, pohits);

            }
        }
    }
}