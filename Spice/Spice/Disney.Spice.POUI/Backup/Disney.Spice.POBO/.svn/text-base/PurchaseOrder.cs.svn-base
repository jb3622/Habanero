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

        #region POProperties
        
        //PO Header
        public enum POType { DropShipSingle, DropShipMultiple, StandardDCPO };

        //Populate PO number

        // All private variables holdding public property values
        private string      _spiceponumber;
        private POType      _purchaseordertype;
        private bool        _bisValid;
        private bool        _isPONewLine;
        private short       _deptcode;
        private string      _deptdesc;
        private int         _vendorcode;
        private string      _vendorDesc;
        private string      _currencycode;
        private string      _currencydesc;
        private string      _termscode;
        private string      _termsdesc;
        private bool        _shipSingle;
        private string[]    _shippingStores;
        private decimal     _landing;
        private DateTime    _anticipateDate             = DateTime.MinValue;
        private string      _shipViaCode;
        private string      _shipViaDesc;
        private DateTime    _shippingDate               = DateTime.MinValue;
        private DateTime    _orderDate                  = DateTime.Now;
        private DateTime    _ssd                        = DateTime.MinValue; //Us only
        private DateTime    _cancelDate                 = DateTime.MinValue;
        private int         _numofPOLines;
        private int         _numofPOPacks;
        private int         _totalUnits;
        private decimal     _totalCost;
        private decimal     _totalLandedCost;
        private decimal     _totalRetail                = 0;
        private decimal     _marginValue                = 0;
        private decimal     _marginPercentage           = 0;
        private int         _portofdeparturecode;
        private string      _portofdeparturedesc;
        private int         _portofentrycode;
        private string      _portofentrydesc            = "";
        private string      _internalcomments1;
        private string      _internalcomments2;
        private short       _shipTo;
        private POType      _poType;
        private string      _defaultMarket;
        private string      _deltermsdesc               = "";
        private string      _vendorcomments1;
        private string      _vendorcomments2;
        private string      _deltermscode               = "";
        private string      _vendorcomments3;
        private Disney.Menu.Users _userName;
        private ASNA.VisualRPG.Runtime.Database _dbInternalParam;
        private short       _spicepoversion             = 0;

        private DataTable   _dtDropShipMatrix;
        private string      _sStoreColumnNamePrefix = "Store_";

        // HK : FC : 17-12-2009 : Add Freight
        private string _freight;

        // HK : 31-12-2009 : Capture the IP PO Number
        private string _ipponumber;

        public DataTable dtDropShipMatrix
        {
            get { return _dtDropShipMatrix; }
            set { _dtDropShipMatrix = value; }
        }

        public string Spiceponumber
        {
            get { return _spiceponumber; }
            set { _spiceponumber = value; }
        }

        public short SpicePoVersion
        {
            get { return _spicepoversion; }
            set { _spicepoversion = value; }
        }

        public POType Purchaseordertype
        {
            get { return _purchaseordertype; }
            set { _purchaseordertype = value; }
        }

        public bool bIsValid
        {
            get { return _bisValid; }
            set { _bisValid = value; }
        }
     
        public bool IsPONewLine
        {
            get { return _isPONewLine; }
            set { _isPONewLine = value; }
        }
        
        public short Deptcode
        {
            get { return _deptcode; }
            set { _deptcode = value; }
        }

        public string Deptdesc
        {
            get { return _deptdesc; }

        }

        public int Vendorcode
        {
            get { return _vendorcode; }
            set { _vendorcode = value; }
        }

        public string VendorDesc
        {
            get { return _vendorDesc; }

        }

        public string Currencycode
        {
            get { return _currencycode; }
            set { _currencycode = value; }
        }

        public string Currencydesc
        {
            get { return _currencydesc; }

        }

        public string Termscode
        {
            get { return _termscode; }
            set { _termscode = value; }

        }

        public string Termsdesc
        {
            get { return _termsdesc; }
            set { _termscode = value; }
        }

        public bool ShipSingle
        {
            get { return _shipSingle; }
            set { _shipSingle = value; }
        }

        public string[] ShippingStores
        {
            get { return _shippingStores; }
            set { _shippingStores = value; }
        }
        
        public string ShipViaCode
        {
            get { return _shipViaCode; }
            set { _shipViaCode = value; }
        }
        
        public string ShipViaDesc
        {
            get { return _shipViaDesc; }

        }

        public decimal Landing
        {
            get { return _landing; }
            set { _landing = value; }
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

        public DateTime OrderDate
        {
            get { return _orderDate; }

        }

        public DateTime Ssd
        {
            get { return _ssd; }
            set { _ssd = value; }
        }

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

        public decimal TotalCost
        {
            get { return _totalCost; }
            set { _totalCost = value; }
        }

        public decimal TotalLandedCost
        {
            get { return _totalLandedCost; }
            set { _totalLandedCost = value; }
        }

        public decimal TotalRetail
        {
            get { return _totalRetail; }
            set { _totalRetail = value; }

        }

        public decimal MarginValue
        {
            get { return _marginValue; }
            set { _marginValue = value; }
        }

        public decimal MarginPercentage
        {
            get { return _marginPercentage; }
            set { _marginPercentage = value; }
        }
        
        public int Portofdeparturecode
        {
            get { return _portofdeparturecode; }
            set { _portofdeparturecode = value; }
        }

        public string Portofdeparturedesc
        {
            get { return _portofdeparturedesc; }

        }
        
        public int Portofentrycode
        {
            get { return _portofentrycode; }
            set { _portofentrycode = value; }
        }

        public string Portofentrydesc
        {
            get { return _portofentrydesc; }

        }
        
        public string Deltermscode
        {
            get { return _deltermscode; }
            set { _deltermscode = value; }
        }
        
        public string Deltermsdesc
        {
            get { return _deltermsdesc; }

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
        
        public POType PoType
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

        // ////////////////////////////////////////////////
        // HK : 09-11-2009
        // Stub : Holds a list of PO Line Items
        // ////////////////////////////////////////////////
        // List of PO Line Details (class POItemDetails)
        List<POItemDetails> _lstpoLineItemDetails;

        public List<POItemDetails> lstpoLineItemDetails
        {
            get { return _lstpoLineItemDetails; }
            set { _lstpoLineItemDetails = value; }
        }

        // ////////////////////////////////////////////////

        // ////////////////////////////////////////////////
        // HK : 02-12-2009
        // Holds a list of PO Hits
        // ////////////////////////////////////////////////
        private PurchaseOrder.PoHitsCollection _pohitscollection;

        public PurchaseOrder.PoHitsCollection poHitsCollection
        {
            get { return _pohitscollection; }
            set { _pohitscollection = value; }
        }
        
        #endregion POProperties

        private IPDEPTS ipdeptscls;

        public PurchaseOrder(ASNA.VisualRPG.Runtime.Database dbparam, Disney.Menu.Users username, Disney.Menu.Environments paramenv)
        {
            _dbInternalParam = dbparam;
            _userName = username;
            _penvironment = paramenv;


            // ///////////////////////////////////////////////
            // HK : 9-11-2009
            // Initalize the PO Line Items properties

            // ///////////////////////////////////////////////
            DataTable _dtpoLineItemDetails = new DataTable();

            _dtpoLineItemDetails.Columns.Add ("POnumber",       typeof(string));
            //_dtpoLineItemDetails.Columns.Add("Version",       typeof(Int16));     //Not populated
            //_dtpoLineItemDetails.Columns.Add("Sequence",      typeof(Int16));     //Not populated
            _dtpoLineItemDetails.Columns.Add ("Class",          typeof(Int16));
            _dtpoLineItemDetails.Columns.Add ("Vendor",         typeof(Int32));
            _dtpoLineItemDetails.Columns.Add ("Style",          typeof(Int16));
            _dtpoLineItemDetails.Columns.Add ("Colour",         typeof(Int16));
            _dtpoLineItemDetails.Columns.Add ("Size",           typeof(Int16));
            // _dtpoLineItemDetails.Columns.Add("SKU",          typeof(Int32));       
            // dtPOLines.Columns.Add("CheckDigit",              typeof(Int16));
            _dtpoLineItemDetails.Columns.Add ("UPC",            typeof(string));
            _dtpoLineItemDetails.Columns.Add ("Quantity",       typeof(Int32));
            _dtpoLineItemDetails.Columns.Add ("LandedCost",     typeof(decimal));   //Cost * Landing Factor
            _dtpoLineItemDetails.Columns.Add ("Retail",         typeof(decimal));
            _dtpoLineItemDetails.Columns.Add ("LongDesc",       typeof(string));
            _dtpoLineItemDetails.Columns.Add ("ShortDesc",      typeof(string));
            _dtpoLineItemDetails.Columns.Add ("VendorStyle",    typeof(string));
            _dtpoLineItemDetails.Columns.Add ("Season",         typeof(string));
            _dtpoLineItemDetails.Columns.Add ("SubClass",       typeof(string));
            _dtpoLineItemDetails.Columns.Add ("Ticket",         typeof(string));
            _dtpoLineItemDetails.Columns.Add ("MinimumPack",    typeof(Int32));
            _dtpoLineItemDetails.Columns.Add ("DistroLot",      typeof(Int32));
            _dtpoLineItemDetails.Columns.Add ("VendorCost",     typeof(decimal));
            _dtpoLineItemDetails.Columns.Add ("LandFactor",     typeof(decimal));
            _dtpoLineItemDetails.Columns.Add ("Character",      typeof(string));    // Character code

            // Initalise the PO Line Items collection

            _lstpoLineItemDetails = new List<POItemDetails>();

        }

        public DataTable GetPOItems (string spiceponumber, short spicepoversion)
        {
            DataTable dtTemp;

            // Create the instance of the header class
            DSSPPOIcls poitemcls = new DSSPPOIcls(DbParamRef);

            dtTemp = poitemcls.GetPOlines(spiceponumber, _spicepoversion);

            return dtTemp;

        }


        // //////////////////////////////////////////////////////
        // HK : CJONES : 30-12-2009 : Get the previous version of 
        // PO header from the database

        // //////////////////////////////////////////////////////
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

            }
            else
            {
                bSuccess = false;
            }

            return bSuccess;

        }

        // //////////////////////////////////////////////////////
        // HK : CJ : 08-12-2009 : Get the PO header from the database

        // //////////////////////////////////////////////////////
        public Boolean GetPOHeader(string spiceponumber)
        {
            Boolean bSuccess = false;
            
            // Create the instance of the header class
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


        // HK : 20-01-2010 : Get the PO Status
        public String GetPOStatus(string spiceponumber, out short changesequence)
        {

            Boolean bSuccess = false;

            // Create the instance of the header class
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

        // //////////////////////////////////////////////////////
        // HK : CJ : 08-12-2009 : Get the PO Comments from the database

        // ////////////////////////////////////////////////////

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

        // /////////////////////////////////////////////////////
        // HK : 02-12-2009 : For standard PO if the user has 
        // activated hits then we must write those hits

        // /////////////////////////////////////////////////////
        public bool CreatePoHeader(int iHitNumber)
        {
            // Create the instance of the header class
            DSSPPOHcls poheadercls = new DSSPPOHcls(DbParamRef, UserName);

            // Assign common properties
            poheadercls.Market          = _defaultMarket;
            poheadercls.Department      = _deptcode;
            poheadercls.Vendor          = _vendorcode;
            poheadercls.CurrencyCode    = _currencycode;
            poheadercls.CurrencyRate    = _exchangeRate;
            poheadercls.Terms           = _termscode;

            poheadercls.ShipTo          = _shipTo;
            //Replace when DC is activated
            //poheadercls.ShipTo = 723;

            poheadercls.ShipVia         = _shipViaCode;
            poheadercls.LandingFactor   = _landing;

            // Get the anticipate date, ship date and cancel date for specified hit
            DateTime __anticipatedate = DateTime.Today;
            DateTime __shippingdate = DateTime.Today;
            string __canceldate = String.Empty;
            GetHitDates(iHitNumber, ref __anticipatedate, ref __shippingdate, ref __canceldate);

            _anticipateDate = __anticipatedate;
            _shippingDate   = __shippingdate;
            CancelDate      = Convert.ToDateTime (__canceldate);
            
            poheadercls.AnticipateDate  = _anticipateDate;
            poheadercls.ShipDate        = _shippingDate;
            poheadercls.OrderDate       = _orderDate;

            poheadercls.CancelDate = CancelDate;

            if (Penvironment.Domain == "SWNA")
            {
                poheadercls.StageSetDate = _ssd;
            }

            // Total Lines = number of lines on the hits that have a quantity of greater than 0
            poheadercls.TotalLines = (short)NumofPOLines;

            // Now get the Hit quantity
            int qty = GetTotalUnitsForHit(iHitNumber);
            _totalUnits = qty;
            poheadercls.TotalUnits = _totalUnits;

            // Number of packs  = number of packs (APP == 'Y') where quantiy greater 
            // than 0 (qty > 0)
            _numofPOPacks = GetTotalPacksForHit(iHitNumber);
            poheadercls.TotalPacks = (short)_numofPOPacks;

            // Total Lines on hit = count of lines where quantity > 0
            int totallines = GetTotalLinesForHit(iHitNumber);
            _numofPOLines = totallines;
            poheadercls.TotalLines = (short)totallines;

            // Total Cost = cost * quantiy (where quantiy > 0)
            _totalCost = CalculateTotalCost(iHitNumber);
            poheadercls.TotalCost = _totalCost;

            // Total Retail = retail * quantiy (where quantiy > 0)
            _totalRetail = CalculateTotalRetail(iHitNumber);
            poheadercls.TotalRetail = _totalRetail;

            // Cost including landing factor
            decimal _totalcostincludinglanding = Decimal.Round(poheadercls.TotalCost * _landing, 2);

            // Margin annd Margin %age
            poheadercls.Margin = (poheadercls.TotalRetail - (_totalcostincludinglanding));
            poheadercls.MarginPercent = (100 * _totalCost / _totalRetail);

            if (_shipViaCode == "OCN")
            {
                poheadercls.PortOfDeparture = _portofdeparturecode;
                poheadercls.PortOfEntry = _portofentrycode;
                poheadercls.DeliveryTerms = _deltermscode;
            }

            poheadercls.NewLine = _isPONewLine;
            poheadercls.IPuserInitials = _userName.IPinitials;

            // HK : 18-12-2009 : Frieght Charge Code
            poheadercls.FreightChargeCode = _freight;

            if (poheadercls.WritePOheader())
            {

                _spiceponumber = poheadercls.SpicePOnumber;
                _spicepoversion = poheadercls.SpicePOversion;

            }

            return true;

        }

        public int GetItemQuatityOnHit(int ihitnumber, int iitemindex, short iClass, int iVendor,
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

        private decimal CalculateTotalRetail(int iHitNumber)
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
                    iClass = poItemDetails.Classcode;
                    iVendor = poItemDetails.Vendorcode;
                    iStyle = poItemDetails.Stylecode;
                    iColor = poItemDetails.Colorcode;
                    iSize = poItemDetails.Itemsize;

                    iitemquatityonhit = GetItemQuatityOnHit(iHitNumber, iitemindex, iClass, iVendor,
                                        iStyle, iColor, iSize);

                    // Item Cost on main Po Entry form * Hit Quantity
                    totalretail = totalretail + (poItemDetails.Retailprice * iitemquatityonhit);
                }

            }

            return totalretail;

        }

        private decimal CalculateTotalCost(int iHitNumber)
        {
            int icollectionindexforhit;
            decimal totalcost = 0;
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

                // ///////////////////////////////////////////////////////////////
                // HK : Fix Bug 304 : TotalCost = Total Landed Cost in PO currency
                // ///////////////////////////////////////////////////////////////
                foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
                {
                    iitemindex  = poItemDetails.Itemindex;
                    iClass      = poItemDetails.Classcode;
                    iVendor     = poItemDetails.Vendorcode;
                    iStyle      = poItemDetails.Stylecode;
                    iColor      = poItemDetails.Colorcode;
                    iSize       = poItemDetails.Itemsize;

                    iitemquatityonhit = GetItemQuatityOnHit(iHitNumber, iitemindex, iClass, iVendor,
                                        iStyle, iColor, iSize);

                    // Item Cost on main Po Entry form * Hit Quantity
                    totalcost = totalcost + (poItemDetails.Cost * iitemquatityonhit);
                }

            }

            return totalcost;

        }

        // HK : 04-12-2009 : Get the total quantity on a particular hit
        // If total quantity is 0 then no need to create PO
        // 
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

        // ////////////////////////////////////////////////////////////////
        // HK : 02-12-2009 : Calculate the Total Units on the Hit
        //
        // ////////////////////////////////////////////////////////////////
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

            poheadercls.TotalPacks      = (short)_numofPOPacks; //Check 

            poheadercls.TotalLines      = (short)_numofPOLines;

            // HK : 22-01-2010 : Fix Bug 304
            // Send the Total Landed Cost in PO Currency
            //poheadercls.TotalCost = _totalCost;
            poheadercls.TotalCost       = CalculateTotalCostInPoCurrency() * _landing;
            poheadercls.TotalRetail     = _totalRetail;

            decimal _totalcostincludinglanding = Decimal.Round(_totalCost * _landing, 2);
            poheadercls.Margin                 = (_totalRetail - (_totalcostincludinglanding));
            poheadercls.MarginPercent          = (100 * _totalCost / _totalRetail);

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
            //Replace when DC is activated
            //poheadercls.ShipTo = 723;

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

            Debug.Print("Anticipate Date:" + poheadercls.AnticipateDate.ToLongDateString());

            // //////////////////////////////////////////////////
            // //////////////////////////////////////////////////
            // HK : FC : When PO type is drop ship then 
            // poheadercls.TotalUnits = quantity assigned to store
            // //////////////////////////////////////////////////
            if (Purchaseordertype != POType.DropShipMultiple)
            {
                poheadercls.TotalUnits  = _totalUnits;

                poheadercls.TotalPacks  = (short)_numofPOPacks; //Check 

                poheadercls.TotalLines  = (short)_numofPOLines;

                // HK : 22-01-2010 : Fix Bug 304
                // Send the Total Landed Cost in PO Currency
                //poheadercls.TotalCost   = _totalCost;
                poheadercls.TotalCost = CalculateTotalCostInPoCurrency() * _landing;
                poheadercls.TotalRetail = _totalRetail;

                decimal _totalcostincludinglanding = Decimal.Round(_totalCost * _landing, 2);
                poheadercls.Margin = (_totalRetail - (_totalcostincludinglanding));
                poheadercls.MarginPercent = (100 * _totalCost / _totalRetail);
            }
            // //////////////////////////////////////////////////

            // ////////////////////////////////////////////////////////////
            // HK : FC 28-11-2009. When PO type = DCPO then the calculation
            // on the PO Header changes with each store
            // Values aaffected are : poheadercls.TotalUnits, 
            // poheadercls.TotalPacks, poheadercls.TotalLines, 
            // poheadercls.TotalCost, poheadercls.TotalRetail.
            // 
            // However poheadercls.Margin and poheadercls.MarginPercent
            // ////////////////////////////////////////////////////////////
            if (Purchaseordertype == POType.DropShipMultiple)
            {
                // HK : 29-11-2009
                // Get the current store for which the PO header is being created
                string sstore = poheadercls.ShipTo.ToString();
                
                // Now get the store quantity
                int qty = GetStoreQuantityForAllItems(sstore);
                _totalUnits = qty;
                poheadercls.TotalUnits = _totalUnits;

                // HK : FC : ?? Unsure how to get totalpacks
                _numofPOPacks = GetTotalPacks(sstore);
                poheadercls.TotalPacks = (short)_numofPOPacks; //Check 

                // HK : FC : 29-11-2009
                int totallines = GetTotalLinesForStore(sstore);
                _numofPOLines = totallines;
                poheadercls.TotalLines = (short)totallines;

                // HK : 22-01-2010 : Fix Bug 304
                // Send the Total Landed Cost in PO Currency
                //_totalCost = CalculateTotalCost(sstore);
                _totalCost = CalculateTotalCostInPoCurrency(sstore) * _landing;
                poheadercls.TotalCost = _totalCost;
                
                _totalRetail = CalculateTotalRetail(sstore);
                poheadercls.TotalRetail = _totalRetail; //CalculateTotalRetail(sstore);

                decimal _totalcostincludinglanding = Decimal.Round(poheadercls.TotalCost * _landing, 2);
                poheadercls.Margin = (poheadercls.TotalRetail - (_totalcostincludinglanding));
                poheadercls.MarginPercent = (100 * _totalCost / _totalRetail);
            }

            if (_shipViaCode == "OCN")
            {
                poheadercls.PortOfDeparture = _portofdeparturecode;
                poheadercls.PortOfEntry = _portofentrycode;
                poheadercls.DeliveryTerms = _deltermscode;
            }

            poheadercls.NewLine = _isPONewLine;
            poheadercls.IPuserInitials = _userName.IPinitials;

            // HK : 18-12-2009 : Frieght Charge Code
            poheadercls.FreightChargeCode = _freight;

            if (poheadercls.WritePOheader())
            {
                _spiceponumber = poheadercls.SpicePOnumber;
                _spicepoversion = poheadercls.SpicePOversion;

            }

            return true;

        }

        // ///////////////////////////////////////////////////////
        // /HK : 30-11-2009 : Get the total packs for the store 
        // where quantity is greater then 0.
        // ///////////////////////////////////////////////////////
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

        private string GetStoreColumnNameFromStore(string sstore)
        {
            return _sStoreColumnNamePrefix + sstore;
        }

        // ///////////////////////////////////////////////////////
        // /HK : 28-11-2009 : Get the total lines for the store 
        // where quantity is greater then 0.
        // ///////////////////////////////////////////////////////
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

        // ////////////////////////////////////////////////////////
        // /HK : 28-11-2009 : Get the sum of item quantities entered
        // across a specified store
        // ////////////////////////////////////////////////////////
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
            string i1 = String.Empty; ;
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
            catch (Exception ex)
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
            catch (Exception ex)
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

            catch (Exception e)
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

                _dtpoLineItemDetails.Rows.Add(Spiceponumber,
                    //     0, //Version
                    //     0, //Sequence
                              polinedetails.Classcode,
                              polinedetails.Vendorcode,
                              polinedetails.Stylecode,
                              polinedetails.Colorcode,
                              polinedetails.Itemsize,
                    //        polinedetails.Sku,//SKU
                    //        0,//Check Digit
                              polinedetails.Upc,//UPC
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
                              polinedetails.MinPack,
                              polinedetails.Distrolot,
                    //        polinedetails.Cost * _polinedetails.Itemquantity, //Vendor Cost 
                              polinedetails.Cost, //Vendor Cost 
                              Landing,
                              polinedetails.Charactercode
                              );
                
            }

        }

        public Boolean ItemPurchaseVATRateLookup(string defaultmarket, string storevatcode, string itemvatcode, ref decimal dPurchaseVatrate)
        {

            Boolean bSuccess = false;

            DSSPIVR dsspivr = new DSSPIVR(_dbInternalParam);

            bSuccess = dsspivr.GetVATrate(defaultmarket, storevatcode, itemvatcode);

            if (bSuccess)
            {
                dPurchaseVatrate = dsspivr.PurchasesVATrate;

                return bSuccess;
            }

            return bSuccess;
        }

        public Boolean ItemSalesVATRateLookup(string defaultmarket, string storevatcode, string itemvatcode, ref decimal dSalesVatrate)
        {

            Boolean bSuccess = false;

            DSSPIVR dsspivr = new DSSPIVR(_dbInternalParam);

            bSuccess = dsspivr.GetVATrate(defaultmarket, storevatcode, itemvatcode);

            if (bSuccess)
            {
                dSalesVatrate = dsspivr.SalesVATrate;
                Debug.Print("Sales VAT Code: " + itemvatcode);
                Debug.Print("Sales VAT Rate: " + dSalesVatrate.ToString());

                return bSuccess;
            }

            return bSuccess;
        }

        // ///////////////////////////////////////////////////////////////////
        // HK : 11-11-2009 : Calculate the Total Retail (property TotalRetail)
        // for  all lines on the PO
        // ///////////////////////////////////////////////////////////////////
        public decimal CalculateTotalRetail()
        {
            decimal totalretail = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                totalretail = totalretail + (poItemDetails.Retailprice * poItemDetails.Itemquantity);
            }

            return totalretail;
        }

        public decimal CalculateTotalCostInPoCurrency()
        {
            decimal totalcost = 0;

            // ///////////////////////////////////////////////////////////////
            // HK : Fix Bug 304 : TotalCost = Total Landed Cost in PO currency
            // ///////////////////////////////////////////////////////////////
            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                totalcost = totalcost + (poItemDetails.ConvertedCost * poItemDetails.Itemquantity);
            }

            return totalcost;
        }

        public decimal CalculateTotalCostInPoCurrency(string sstore)
        {
            decimal totalcost = 0;

            // ///////////////////////////////////////////////////////////////
            // HK : Fix Bug 304 : TotalCost = Total Landed Cost in PO currency
            // ///////////////////////////////////////////////////////////////

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {

                //totalcost = totalcost + (poItemDetails.Cost * poItemDetails.Itemquantity);
                // Get the Item Quantity Assigned to a specific store
                int qty = GetItemQuantityForStore(sstore, poItemDetails.Itemindex,
                                                            poItemDetails.Classcode,
                                                            poItemDetails.Vendorcode,
                                                            poItemDetails.Stylecode,
                                                            poItemDetails.Colorcode,
                                                            poItemDetails.Itemsize);

                totalcost = totalcost + (poItemDetails.ConvertedCost * qty);
            }

            return totalcost;
        }

        public decimal CalculateTotalCost()
        {
            decimal totalcost = 0;

            // ///////////////////////////////////////////////////////////////
            // HK : Fix Bug 304 : TotalCost = Total Landed Cost in PO currency
            // ///////////////////////////////////////////////////////////////
            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                totalcost = totalcost + (poItemDetails.Cost * poItemDetails.Itemquantity);
            }

            return totalcost;
        }

        // ///////////////////////////////////////////////////////////////////
        // HK : 29-11-2009 : // HK : Get the TotalCost for a apecific store
        // for  all lines on the PO
        // ///////////////////////////////////////////////////////////////////
        public decimal CalculateTotalCost(string sstore)
        {
            decimal totalcost = 0;

            // ///////////////////////////////////////////////////////////////
            // HK : Fix Bug 304 : TotalCost = Total Landed Cost in PO currency
            // ///////////////////////////////////////////////////////////////

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                
                //totalcost = totalcost + (poItemDetails.Cost * poItemDetails.Itemquantity);
                // Get the Item Quantity Assigned to a specific store
                int qty = GetItemQuantityForStore(sstore, poItemDetails.Itemindex, 
                                                            poItemDetails.Classcode, 
                                                            poItemDetails.Vendorcode,
                                                            poItemDetails.Stylecode, 
                                                            poItemDetails.Colorcode, 
                                                            poItemDetails.Itemsize);

                totalcost = totalcost + (poItemDetails.Cost * qty);
            }

            return totalcost;
        }

        public decimal CalculateTotalRetail(string sstore)
        {
            decimal totalretail = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                // Get the Item Quantity for store
                int qty = GetItemQuantityForStore(sstore, poItemDetails.Itemindex, 
                                                          poItemDetails.Classcode, 
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

        // //////////////////////////////////////////////////////////////
        // HK : 28-12-2009 : Calculate the Total Retail Ex VAT
        // for  a single PO Item
        // //////////////////////////////////////////////////////////////
        public decimal CalculateTotalRetailExVatItem(POItemDetails poItemDetails, string storevatcode)
        {
            decimal itemretailexvat = 0;
            decimal itemsalesvatrate = 0;
            Boolean bSuccess;

            // Get the Item Sales VAT Rate
            bSuccess = ItemSalesVATRateLookup(DefaultMarket, storevatcode, poItemDetails.VatCode, ref itemsalesvatrate);

            if (bSuccess)
            {
                // 
                itemretailexvat = (poItemDetails.Retailprice / (1 + itemsalesvatrate)) * poItemDetails.Itemquantity;
            }

            return itemretailexvat;

        }


        // //////////////////////////////////////////////////////////////
        // HK : 17-11-2009 : Calculate the Total Retail Ex VAT
        // for  all lines on the PO
        // //////////////////////////////////////////////////////////////

        public decimal CalculateTotalRetailExVat(string storevatcode)
        {
            decimal totalretailexvat = 0;
            decimal itemretailexvat = 0;
            decimal itemsalesvatrate = 0;
            
            Boolean bSuccess;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                //totalcost = totalcost + (poItemDetails.Cost * poItemDetails.Itemquantity);
                
                // Get the Item Sales VAT Rate
                bSuccess = ItemSalesVATRateLookup (DefaultMarket, storevatcode, poItemDetails.VatCode, ref itemsalesvatrate);

                if (bSuccess)
                {
                    // 
                    itemretailexvat = (poItemDetails.Retailprice / (1 + itemsalesvatrate)) * poItemDetails.Itemquantity;
                    totalretailexvat = totalretailexvat + itemretailexvat;
                }

            }

            return totalretailexvat;
        }


        // //////////////////////////////////////////////////////////////
        // HK : 11-11-2009 : Calculate the Total Units (property TotalUnits)
        // for  all lines on the PO
        // //////////////////////////////////////////////////////////////
        public int CalculateTotalUnit()
        {
            int totalunits = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                totalunits = totalunits + poItemDetails.Itemquantity;
            }

            return totalunits;
        }

        // ///////////////////////////////////////////////////////////////////
        // HK : 02-12-2009 : Returns the correct POHit Collection index number
        // for the specified Hit
        // ie.  HIT         Index
        // ========================
        //      2nd         0
        //      3rd         1
        //      4th         2
        //      5th         3
        //      6th         4
        // ///////////////////////////////////////////////////////////////////
        private int GetHitsCollectionIndexForSpecificHit(int iHitNumber)
        {

            if (iHitNumber >= 2 && iHitNumber <= 6)
            {
                return (iHitNumber - 2);
            }

            // Invalid hit number
            return -1;
        }

        // ///////////////////////////////////////////////////////////////
        // HK : 30-11-2009 : PO Hits
        // ///////////////////////////////////////////////////////////////

        public class POHits
        {
            private Boolean _hitactivated;
            private Boolean _hitcreated;
            private Boolean _hitinitalised;
            private int     _hitnumber;


            private DateTime _anticipateDate    = DateTime.Today;
            private DateTime _shippingDate      = DateTime.Today;
            //private DateTime _cancelDate      = DateTime.Today; //DateTime.MinValue;
            private string _cancelDate;
            
            private string _vendorcomments1;
            private string _vendorcomments2;
            private string _vendorcomments3;
            
            private string _internalcomments1;
            private string _internalcomments2;

            private DataTable _dtPoHits;

            // HK : 08-01-2010 :
            // Fix Bug ?? Keep the original copy of the datatable
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

            // HK : 08-01-2010
            // Keep original copy of table
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

            /// <summary>
            /// Indexer for our collection
            /// </summary>
            /// <param name="idx">Index of the collection</param>
            /// <returns>POItemDetails at that index</returns>
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

            /*
            public bool Contains(POHits pohits)
            {
                // If value is not of type poitemdetails, this will return false.
                //return (this.List.Contains(poitemdetails));

                foreach (POHits item in this.InnerList)
                {
                    if (item.CompareTo(pohits) == 0)
                    {
                        // return true
                        return true;
                    }
                }

                return false;
            }
            */
            
            /*
            public override bool Equals(object obj)
            {
                if (obj is POItemDetails)
                {
                    POItemDetails temp = (POItemDetails)obj;

                    foreach (POItemDetails item in this.InnerList)
                    {
                        if (temp.Classcode == item.Classcode &&
                            temp.Vendorcode == item.Vendorcode &&
                            temp.Stylecode == item.Stylecode &&
                            temp.Colorcode == item.Colorcode &&
                            temp.Itemsize == item.Itemsize)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Object is not PO Item");
                }
                return false;
            }
            */
        }

    }

}