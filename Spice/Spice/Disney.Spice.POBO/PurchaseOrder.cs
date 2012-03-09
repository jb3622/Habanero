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
        
        public enum POtype { DropShipSingle, DropShipMultiple, StandardDCPO }

        private int         _totalUnits;
        private Decimal     _totalCost;
        private Decimal     _totalLandedCost;
        private Decimal     _totalCostPOCurr;
        private Decimal     _totalRetail                = 0;
        private Decimal     _totalRetailXVat            = 0;
        private Decimal     _marginValue                = 0;
        private Decimal     _marginPercentage           = 0;
        private int         _portofdeparturecode;
        private int         _portofentrycode;
        private String      _portofentrydesc            = "";
        private String      _internalcomments1;
        private String      _internalcomments2;
        private short       _shipTo;
        private String      _domainMarket;
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

        private string _shipToMethod;
        public String ShipToMethod
        {
            get { return _shipToMethod; }
            set { _shipToMethod = value; }
        }

        private string _shipToRounding;
        public String ShipToRounding
        {
            get { return _shipToRounding; }
            set { _shipToRounding = value; }
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

        private DateTime _ssd = DateTime.MinValue;
        public DateTime StageSetDate
        {
            get { return _ssd; }
            set { _ssd = value; }
        }

        private Int32 _ssdid;
        public Int32 StageSetDateID
        {
            get
            {
                return _ssdid;
            }
            set
            {
                _ssdid = value;
            }
        }

        private DateTime _cancelDate = DateTime.MinValue;
        public DateTime CancelDate
        {
            get { return _cancelDate; }
            set { _cancelDate = value; }
        }
        
        private int _numofPOLines;
        public int NumofPOLines
        {
            get { return _numofPOLines; }
            set { _numofPOLines = value; }
        }
        
        private int _numofPOPacks;
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

        public Decimal TotalCostPOCurr
        {
            get { return _totalCostPOCurr; }
            set { _totalCostPOCurr = value; }
        }

        public Decimal TotalRetail
        {
            get { return _totalRetail; }
            set { _totalRetail = value; }
        }

        public Decimal TotalRetailXVat
        {
            get { return _totalRetailXVat; }
            set { _totalRetailXVat = value; }
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

        private String _portofdeparturedesc;
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

        public string DomainMarket
        {
            get { return _domainMarket; }
            set { _domainMarket = value; }
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

        int _poLineItemDetailsCurent;
        public int poLineItemsCurrent
        {
            get { return _poLineItemDetailsCurent; }
            set { _poLineItemDetailsCurent = value; }
        }
        
        DataTable _dtpoLineItemDetails;
        public DataTable dtpoLineItemDetails
        {
            get { return _dtpoLineItemDetails; }
            set { _dtpoLineItemDetails = value; }
        }

        List<POItemDetails> _lstpoLineItemDetails;
        public List<POItemDetails> lstpoLineItemDetails
        {
            get { return _lstpoLineItemDetails; }
            set { _lstpoLineItemDetails = value; }
        }

        private PurchaseOrder.PoHitsCollection _pohitscollection;
        public PurchaseOrder.PoHitsCollection poHitsCollection
        {
            get { return _pohitscollection; }
            set { _pohitscollection = value; }
        }
        #endregion POproperties

        public PurchaseOrder(ASNA.VisualRPG.Runtime.Database dbparam, Disney.Menu.Users username, Disney.Menu.Environments paramenv)
        {
            _dbInternalParam = dbparam;
            _userName = username;
            _penvironment = paramenv;

            _lstpoLineItemDetails = new List<POItemDetails>();
        }

        public DataTable GetPOItems(string spiceponumber, short spicepoversion)
        {
            DSSPPOIcls poitemcls = new DSSPPOIcls(DbParamRef);
            DataTable dtTemp = poitemcls.GetPOlines(spiceponumber, _spicepoversion);

            return dtTemp;
        }

        public Boolean GetPreviousPOHeader(string spiceponumber, short currentversion)
        {
            Boolean bSuccess;

            DSSPPOHcls poheadercls = new DSSPPOHcls(DbParamRef, UserName);
            bSuccess = poheadercls.GetPreviousPOheader(spiceponumber);

            if (bSuccess == true)
            {
                _defaultMarket  = poheadercls.Market;
                _deptcode       = poheadercls.Department;
                _vendorcode     = poheadercls.Vendor;
                _currencycode   = poheadercls.CurrencyCode;
                _exchangeRate   = poheadercls.CurrencyRate;
                _termscode      = poheadercls.Terms;
                _shipTo         = poheadercls.ShipTo;
                _shipViaCode    = poheadercls.ShipVia;
                _shipToMethod   = poheadercls.ShipToMethod;
                _shipToRounding = poheadercls.ShipToRounding;
                _landing        = poheadercls.LandingFactor;

                _anticipateDate = poheadercls.AnticipateDate;
                _shippingDate = poheadercls.ShipDate;
                _orderDate = poheadercls.OrderDate;
                _cancelDate = poheadercls.CancelDate;

                //if (Penvironment.Domain == "SWNA")
                if (DataCache.AreStageSetDatesChangeable == true)                
                {
                    _ssd = poheadercls.StageSetDate;
                    _ssdid = poheadercls.StageSetDateID;
                }

                _numofPOLines     = poheadercls.TotalLines;
                _totalUnits       = poheadercls.TotalUnits;
                _numofPOPacks     = poheadercls.TotalPacks;
                 _totalCost       = poheadercls.TotalCost;
                _totalRetail      = poheadercls.TotalRetail;
                _marginValue      = poheadercls.Margin;
                _marginPercentage = poheadercls.MarginPercent;

                _portofdeparturecode = poheadercls.PortOfDeparture;
                _portofentrycode     = poheadercls.PortOfEntry;
                _deltermscode        = poheadercls.DeliveryTerms;

                _isPONewLine = poheadercls.NewLine;

                _spiceponumber  = poheadercls.SpicePOnumber;
                _spicepoversion = poheadercls.SpicePOversion;

                _freight = poheadercls.FreightChargeCode;

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
                _defaultMarket      = poheadercls.Market;
                _deptcode           = poheadercls.Department;
                _vendorcode         = poheadercls.Vendor;
                _currencycode       = poheadercls.CurrencyCode;
                _exchangeRate       = poheadercls.CurrencyRate;
                _termscode          = poheadercls.Terms;
                _shipTo             = poheadercls.ShipTo;
                _shipToMethod       = poheadercls.ShipToMethod;
                _shipToRounding     = poheadercls.ShipToRounding;
                _shipViaCode        = poheadercls.ShipVia;
                _landing            = poheadercls.LandingFactor;
                _anticipateDate     = poheadercls.AnticipateDate;
                _shippingDate       = poheadercls.ShipDate;
                _orderDate          = poheadercls.OrderDate;
                _cancelDate         = poheadercls.CancelDate;

                //if (Penvironment.Domain == "SWNA")
                if (DataCache.AreStageSetDatesChangeable == true) 
                {
                    _ssd = poheadercls.StageSetDate;
                    _ssdid = poheadercls.StageSetDateID;
                }

                _numofPOLines = poheadercls.TotalLines;
                _totalUnits   = poheadercls.TotalUnits;
                _numofPOPacks = poheadercls.TotalPacks;
                _totalCost    = poheadercls.TotalCost;
                _totalRetail  = poheadercls.TotalRetail;
                _marginValue  = poheadercls.Margin;
                _marginPercentage = poheadercls.MarginPercent;

                _portofdeparturecode    = poheadercls.PortOfDeparture;
                _portofentrycode        = poheadercls.PortOfEntry;
                _deltermscode           = poheadercls.DeliveryTerms;

                _isPONewLine                = poheadercls.NewLine;
                _spiceponumber              = poheadercls.SpicePOnumber;
                _spicepoversion             = poheadercls.SpicePOversion;

                _freight = poheadercls.FreightChargeCode;

                _ipponumber = poheadercls.IPPOnumber;
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
            poheadercls.ShipToMethod    = _shipToMethod;
            poheadercls.ShipToRounding  = _shipToRounding;
            poheadercls.ShipVia         = _shipViaCode;
            poheadercls.LandingFactor   = _landing;

            DateTime __anticipatedate   = DateTime.Today;
            DateTime __shippingdate     = DateTime.Today;
            string __canceldate         = String.Empty;
            GetHitDates(iHitNumber, ref __anticipatedate, ref __shippingdate, ref __canceldate);

            _anticipateDate = __anticipatedate;
            _shippingDate   = __shippingdate;
            CancelDate      = Convert.ToDateTime(__canceldate);
            
            poheadercls.AnticipateDate  = _anticipateDate;
            poheadercls.ShipDate        = _shippingDate;
            poheadercls.OrderDate       = _orderDate;
            poheadercls.CancelDate = CancelDate;
                        
            poheadercls.StageSetDate = DateTime.MinValue;

            poheadercls.TotalLines = (short)_numofPOLines;

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


            if (retailExVat != 0)
            {
                poheadercls.Margin = retailExVat - landedcost;
                poheadercls.MarginPercent = (100 * poheadercls.Margin) / retailExVat;
            }
            else
            {
                poheadercls.Margin = poheadercls.TotalRetail - landedcost;
                poheadercls.MarginPercent = (100 * poheadercls.Margin) / poheadercls.TotalRetail;
            }
           
            //---------------------------------------
            poheadercls.PortOfDeparture = _portofdeparturecode;
            poheadercls.PortOfEntry = _portofentrycode;
            poheadercls.DeliveryTerms = _deltermscode;

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
            poheadercls.ShipToMethod    = _shipToMethod;
            poheadercls.ShipToRounding  = _shipToRounding;
            poheadercls.ShipVia         = _shipViaCode;
            poheadercls.LandingFactor   = _landing;
            poheadercls.AnticipateDate  = _anticipateDate;
            poheadercls.ShipDate        = _shippingDate;
            poheadercls.OrderDate       = _orderDate;
            poheadercls.CancelDate      = CancelDate;
            
            //if (Penvironment.Domain == "SWNA")
            if (DataCache.AreStageSetDatesChangeable == true) 
            {
                poheadercls.StageSetDate   = _ssd;
                poheadercls.StageSetDateID = _ssdid;
            }

            poheadercls.TotalLines = (short)_numofPOLines;

            if (PurchaseOrderType != POtype.DropShipMultiple)
            {   
                poheadercls.TotalUnits  = _totalUnits;
                poheadercls.TotalPacks  = (short)_numofPOPacks;
                poheadercls.TotalLines  = (short)_numofPOLines;

                poheadercls.TotalCost = _totalCostPOCurr * _landing;
                
                poheadercls.TotalRetail   = _totalRetail;
                poheadercls.Margin        = _marginValue;
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
                poheadercls.TotalLines = (short)_numofPOLines;

                _totalCost = CalculateTotalCostInPoCurrency(sstore) * _landing;
                poheadercls.TotalCost = _totalCost;

                // Remove - 3622
                //Decimal TotalLandedCost = CalculateTotalLandedCost(sstore) * _landing;
                Decimal TotalLandedCost = CalculateTotalLandedCost(sstore);
                                                                              
                _totalRetail = CalculateTotalRetail(sstore);
                poheadercls.TotalRetail = _totalRetail;

                // Would never work for multiple items as calculation has to be on units per item not total item qty
                //_totalRetailXVat = Decimal.Round(CalculateTotalRetailExVat("A", _totalUnits), 2);

                //Added 09/05/2011 Joseph Urbina
                _totalRetailXVat = Decimal.Round(CalculateStoreTotalRetailExVat("A", sstore), 2);

                if (_totalRetailXVat == 0)
                {
                    poheadercls.Margin = _totalRetail - TotalLandedCost;
                    poheadercls.MarginPercent = Decimal.Round(poheadercls.Margin / _totalRetail * 100, 2);
                }
                else
                {
                    poheadercls.Margin = _totalRetailXVat - TotalLandedCost;
                    poheadercls.MarginPercent = Decimal.Round(poheadercls.Margin / _totalRetailXVat * 100, 2);
                }
            }

            poheadercls.PortOfDeparture = _portofdeparturecode;
            poheadercls.PortOfEntry     = _portofentrycode;
            poheadercls.DeliveryTerms   = _deltermscode;

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
            int icollectionindexforhit = GetHitsCollectionIndexForSpecificHit(ihitnumber);

            for (int i = 0; i < poHitsCollection[icollectionindexforhit].dtPoHits.Rows.Count; i++)
            {
                if (Convert.ToInt32(poHitsCollection[icollectionindexforhit].dtPoHits.Rows[i]["ItemIndex"]) == iitemindex &&
                    Convert.ToInt16(poHitsCollection[icollectionindexforhit].dtPoHits.Rows[i]["Class"]) == iClass &&
                    Convert.ToInt32(poHitsCollection[icollectionindexforhit].dtPoHits.Rows[i]["Vendor"]) == iVendor &&
                    Convert.ToInt16(poHitsCollection[icollectionindexforhit].dtPoHits.Rows[i]["Style"]) == iStyle &&
                    Convert.ToInt16(poHitsCollection[icollectionindexforhit].dtPoHits.Rows[i]["Color"]) == iColor &&
                    Convert.ToInt16(poHitsCollection[icollectionindexforhit].dtPoHits.Rows[i]["Size"]) == iSize)
                {
                    return Convert.ToInt32(poHitsCollection[icollectionindexforhit].dtPoHits.Rows[i]["Quantity"]);;
                }
            }

            return 0;
        }

        private Decimal CalculateTotalRetail(int iHitNumber)
        {
            int icollectionindexforhit;
            decimal totalretail = 0;

            icollectionindexforhit = GetHitsCollectionIndexForSpecificHit(iHitNumber);

            if (icollectionindexforhit != -1)
            {
                foreach (POItemDetails item in lstpoLineItemDetails)
                {
                    int iitemquatityonhit = GetItemQuantityOnHit(iHitNumber, item.Sequence, item.ClassCode, item.Vendorcode,
                                        item.Stylecode, item.Colorcode, item.Itemsize);

                    totalretail = totalretail + (item.Retailprice * iitemquatityonhit);
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
                foreach (POItemDetails item in lstpoLineItemDetails)
                {
                   Int32 iitemquatityonhit = GetItemQuantityOnHit(iHitNumber, item.Sequence, item.ClassCode, item.Vendorcode, item.Stylecode, item.Colorcode, item.Itemsize);

                   totalcost += item.LandedCost * iitemquatityonhit;
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
                foreach (POItemDetails item in lstpoLineItemDetails)
                {
                    Int32 iitemquatityonhit = GetItemQuantityOnHit(iHitNumber, item.Sequence, item.ClassCode, item.Vendorcode, item.Stylecode, item.Colorcode, item.Itemsize);

                    totalcost += item.ConvertedCost * iitemquatityonhit;
                }
            }

            return totalcost;
        }

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
                        // If the quantity is 0 then this line will not be written to the database.
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
            poheadercls.ShipToMethod    = _shipToMethod;
            poheadercls.ShipToRounding  = _shipToRounding;
            poheadercls.ShipVia         = _shipViaCode;
            poheadercls.LandingFactor   = _landing;
            poheadercls.AnticipateDate  = _anticipateDate;
            poheadercls.ShipDate        = _shippingDate;
            poheadercls.OrderDate       = _orderDate;
            poheadercls.CancelDate      = CancelDate;

            //if (Penvironment.Domain == "SWNA")
            if (DataCache.AreStageSetDatesChangeable == true) 
            {
                poheadercls.StageSetDate   = _ssd;
                poheadercls.StageSetDateID = _ssdid;
            }

            poheadercls.TotalUnits      = _totalUnits;
            poheadercls.TotalPacks      = (short)_numofPOPacks;
            poheadercls.TotalLines      = (short)_numofPOLines;

            poheadercls.TotalCost       = CalculateTotalCostInPoCurrency() * _landing;
            poheadercls.TotalRetail     = _totalRetail;

            poheadercls.Margin = _marginValue;
            poheadercls.MarginPercent = _marginPercentage;

            poheadercls.PortOfDeparture = _portofdeparturecode;
            poheadercls.PortOfEntry     = _portofentrycode;
            poheadercls.DeliveryTerms   = _deltermscode;

            poheadercls.NewLine         = _isPONewLine;
            poheadercls.IPuserInitials  = _userName.IPinitials;
            poheadercls.FreightChargeCode = _freight;
            poheadercls.IPPOnumber = _ipponumber;

            if (poheadercls.UpdatePOheader (spiceponumber))
            {
                _spiceponumber = poheadercls.SpicePOnumber;
                _spicepoversion = poheadercls.SpicePOversion;

                spicepoversionprevious = poheadercls.POPreviousVer;
            }
            else
            {
                return false;
            }

            return true;
        }

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
            
            POItemDetails polinedetails;

            _dtpoLineItemDetails.Clear();
            
            poLineCount = lstpoLineItemDetails.Count;

            for (iCount = 0; iCount < poLineCount; iCount++)
            {
                polinedetails = lstpoLineItemDetails[iCount];

                _dtpoLineItemDetails.Rows.Add(SpicePOnumber,
                    polinedetails.ClassCode,
                    polinedetails.Vendorcode,
                    polinedetails.Stylecode,
                    polinedetails.Colorcode,
                    polinedetails.Itemsize,
                    polinedetails.UPC,
                    polinedetails.Itemquantity,
                    (polinedetails.Cost * Landing), 
                    polinedetails.Retailprice,
                    polinedetails.Itemlongdescription,
                    polinedetails.Itemshortdescription,
                    polinedetails.Vendorstyle,
                    polinedetails.SeasonDesc,
                    polinedetails.Subclass,
                    polinedetails.Tickettype,
                    polinedetails.CasePackQty,
                    polinedetails.DistroQty,
                    polinedetails.Cost, 
                    Landing,
                    polinedetails.Charactercode
                );
            }
        }

        public Boolean ItemPurchaseVATRateLookup(string defaultmarket, string storevatcode, string itemvatcode, ref decimal dPurchaseVatrate)
        {
            DSSPIVR dsspivr = new DSSPIVR(_dbInternalParam, _penvironment);

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
            DSSPIVR dsspivr = new DSSPIVR(_dbInternalParam, _penvironment);

            Boolean bSuccess = dsspivr.GetVATrate(defaultmarket, storevatcode, itemvatcode);

            if (bSuccess)
            {
                dSalesVatrate = dsspivr.SalesVATrate;
                return bSuccess;
            }

            return bSuccess;
        }

        public Decimal CalculateTotalRetail()
        {
            decimal totalretail = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                if (poItemDetails.IsDeleted == false)
                {
                    totalretail = totalretail + (poItemDetails.Retailprice * poItemDetails.Itemquantity);
                }
            }

            return totalretail;
        }

        public Decimal CalculateTotalCostInPoCurrency()
        {
            decimal totalcost = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                if (poItemDetails.IsDeleted == false)
                {
                    totalcost = totalcost + (Decimal.Round(poItemDetails.ConvertedCost,2) * poItemDetails.Itemquantity);
                }
            }

            return totalcost;
        }

        public Decimal CalculateTotalCostInPoCurrency(string sstore)
        {
            decimal totalcost = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                if (poItemDetails.IsDeleted == false)
                {
                    int qty = GetItemQuantityForStore(sstore,
                                              poItemDetails.Sequence,
                                              poItemDetails.ClassCode,
                                              poItemDetails.Vendorcode,
                                              poItemDetails.Stylecode,
                                              poItemDetails.Colorcode,
                                              poItemDetails.Itemsize);

                    totalcost = totalcost + (poItemDetails.ConvertedCost * qty);
                }
            }

            return totalcost;
        }

        public Decimal CalculateTotalCost()
        {
            decimal totalcost = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                if (poItemDetails.IsDeleted == false)
                {
                    totalcost = totalcost + (poItemDetails.Cost * poItemDetails.Itemquantity);
                }
            }

            return totalcost;
        }

        public Decimal CalculateTotalCostPOCurr()
        {
            decimal totalcostPOCurr = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                if (poItemDetails.IsDeleted == false)
                {
                    totalcostPOCurr = totalcostPOCurr + (poItemDetails.ConvertedCost * poItemDetails.Itemquantity);
                }
            }

            return totalcostPOCurr;
        }

        public Decimal CalculateTotalLandedCost()
        {
            decimal totalcost = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                if (poItemDetails.IsDeleted == false)
                {
                    totalcost += poItemDetails.LandedCost * poItemDetails.Itemquantity;
                }
            }

            return totalcost;
        }

        public Decimal CalculateTotalLandedCost(string sstore)
        {
            decimal totalcost = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                if (poItemDetails.IsDeleted == false)
                {
                    int qty = GetItemQuantityForStore(sstore,
                                              poItemDetails.Sequence,
                                              poItemDetails.ClassCode,
                                              poItemDetails.Vendorcode,
                                              poItemDetails.Stylecode,
                                              poItemDetails.Colorcode,
                                              poItemDetails.Itemsize);

                    totalcost += poItemDetails.LandedCost * qty;
                }
            }

            return totalcost;
        }

        public Decimal CalculateTotalCost(string sstore)
        {
            decimal totalcost = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                int qty = GetItemQuantityForStore(sstore,
                                          poItemDetails.Sequence,
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
                if (poItemDetails.IsDeleted == false)
                {
                    int qty = GetItemQuantityForStore(sstore,
                                              poItemDetails.Sequence,
                                              poItemDetails.ClassCode,
                                              poItemDetails.Vendorcode,
                                              poItemDetails.Stylecode,
                                              poItemDetails.Colorcode,
                                              poItemDetails.Itemsize);

                    totalretail = totalretail + (poItemDetails.Retailprice * qty);
                }
                    
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
        }

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

        public Decimal CalculateTotalRetailExVat(string storevatcode)
        {
            decimal totalretailexvat = 0;
            decimal itemretailexvat = 0;
            decimal itemsalesvatrate = 0;
            
            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                if (poItemDetails.IsDeleted == false)
                {
                    Boolean bSuccess = ItemSalesVATRateLookup(DefaultMarket, storevatcode, poItemDetails.VatCode, ref itemsalesvatrate);

                    if (bSuccess)
                    {
                        itemretailexvat = (poItemDetails.Retailprice / (1 + itemsalesvatrate)) * poItemDetails.Itemquantity;
                        totalretailexvat = totalretailexvat + itemretailexvat;
                    }
                }
            }

            return totalretailexvat;
        }

        public Decimal CalculateTotalRetailExVat(string storevatcode, decimal ItemQuantity)
        {
            decimal totalretailexvat = 0;
            decimal itemretailexvat = 0;
            decimal itemsalesvatrate = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                if (poItemDetails.IsDeleted == false)
                {
                    Boolean bSuccess = ItemSalesVATRateLookup(DefaultMarket, storevatcode, poItemDetails.VatCode, ref itemsalesvatrate);

                    if (bSuccess)
                    {
                        itemretailexvat = (poItemDetails.Retailprice / (1 + itemsalesvatrate)) * ItemQuantity;
                        totalretailexvat = totalretailexvat + itemretailexvat;
                    }
                }
            }

            return totalretailexvat;
        }

        //Added 09/05/2011 Joseph Urbina
        public Decimal CalculateStoreTotalRetailExVat(string storevatcode, string sstore)
        {
            decimal totalretailexvat = 0;
            decimal itemretailexvat  = 0;
            decimal itemsalesvatrate = 0;

            foreach (POItemDetails poItemDetails in lstpoLineItemDetails)
            {
                if (poItemDetails.IsDeleted == false)
                {
                    int qty = GetItemQuantityForStore(sstore,
                                              poItemDetails.Sequence,
                                              poItemDetails.ClassCode,
                                              poItemDetails.Vendorcode,
                                              poItemDetails.Stylecode,
                                              poItemDetails.Colorcode,
                                              poItemDetails.Itemsize);

                   Boolean bSuccess = ItemSalesVATRateLookup(DefaultMarket, storevatcode, poItemDetails.VatCode, ref itemsalesvatrate);

                    if (bSuccess)
                    {
                        itemretailexvat = (poItemDetails.Retailprice / (1 + itemsalesvatrate)) * qty;
                        totalretailexvat = totalretailexvat + itemretailexvat;
                    }
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
                foreach (POItemDetails item in lstpoLineItemDetails)
                {
                    Int32 qtyonhit = GetItemQuantityOnHit(iHitNumber, item.Sequence, item.ClassCode, item.Vendorcode, item.Stylecode, item.Colorcode, item.Itemsize);

                    Boolean bSuccess = ItemSalesVATRateLookup(DefaultMarket, "A", item.VatCode, ref itemsalesvatrate);
                    if (bSuccess)
                    {
                        Decimal itemretailexvat = (item.Retailprice / (1 + itemsalesvatrate)) * qtyonhit;
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
                if (poItemDetails.IsDeleted == false)
                {
                    totalunits = totalunits + poItemDetails.Itemquantity;
                }
            }

            return totalunits;
        }

        private int GetHitsCollectionIndexForSpecificHit(int iHitNumber)
        {
            if (iHitNumber >= 2 && iHitNumber <= 6)
            {
                return (iHitNumber - 2);
            }

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
                _dtPoHitsOriginal.Clear();
                _dtPoHitsOriginal = _dtPoHits.Copy();
            }

            public void RevertCopyOfPoHit()
            {
                _dtPoHits.Clear ();
                _dtPoHits = _dtPoHitsOriginal.Copy();
            }
            
            public POHits()
            {
                _dtPoHits = new DataTable();

                _dtPoHits.Columns.Add("colSelect",      typeof(Boolean));
                _dtPoHits.Columns.Add("ItemIndex",      typeof(String));
                _dtPoHits.Columns.Add("Pack",           typeof(String));
                _dtPoHits.Columns.Add("Class",          typeof(short));
                _dtPoHits.Columns.Add("Vendor",         typeof(String));
                _dtPoHits.Columns.Add("Style",          typeof(String));
                _dtPoHits.Columns.Add("Color",          typeof(String));
                _dtPoHits.Columns.Add("Size",           typeof(String));
                _dtPoHits.Columns.Add("Description",    typeof(String));
                _dtPoHits.Columns.Add("Quantity",       typeof(String));

                _cancelDate = ShippingDate.ToLongDateString();
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
