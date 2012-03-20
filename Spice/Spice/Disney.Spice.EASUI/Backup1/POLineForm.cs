using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Disney.Spice.POBO;
using System.Diagnostics;

namespace Disney.Spice.EASUI
{
    public partial class POLineForm : Form
    {
        public delegate void    ComponentQuantityOrCostChangedEventHandler(object sender, AppDetailsEventArgs e);
        public event            ComponentQuantityOrCostChangedEventHandler OnAppQuantityOrCostChanged;

        PurchaseOrder       _porderdisplay;
        //only one line at a time.
        POItemDetails       _porderitemline; 

        //Line level variables
        private decimal     _totalcost;
        private decimal     _totallandedcost;
        private decimal     _totalretail;
        private decimal     _totalmargin;
        private decimal     _totalmarginperc;
        private bool        _isitemcomponent;

        private int         _componentqty;
        private decimal     _componentcost;
        private int         _datagridrowindex;

        //private Boolean     bComponentCostChanged;
        //private Boolean     bQuantityChanged;

        private decimal _currencyratemarket;
        private decimal _currencyratepo;

        private Validation validationcls;

        // HK : 16-11-2009 : Add a stub for Store VAt Code
        private const string MAGICDCSTOREVATCODE = "A";

        public POLineForm(PurchaseOrder porder, ref POItemDetails poitemdetails, bool bcomponent)
        {
            InitializeComponent();

            _porderdisplay = porder;
            _porderitemline = poitemdetails;
            _isitemcomponent = bcomponent;

            validationcls = new Validation(_porderdisplay.DbParamRef, _porderdisplay.UserName, _porderdisplay.Penvironment);

            InitialiseValues();
        }

        public POLineForm(PurchaseOrder porder, ref POItemDetails poitemdetails, bool bcomponent, int datagridrowindex)
        {
            InitializeComponent();
            
            _porderdisplay  = porder;
            _porderitemline = poitemdetails;
            _isitemcomponent = bcomponent;
            _datagridrowindex = datagridrowindex;

            validationcls = new Validation(_porderdisplay.DbParamRef, _porderdisplay.UserName, _porderdisplay.Penvironment);

            InitialiseValues();
        }

        private void InitialiseValues()
        {
            StringBuilder sblongitemdesc = new StringBuilder("");

            sblongitemdesc.Append(_porderitemline.ClassCode.ToString().PadLeft(4, '0'));
            sblongitemdesc.Append("-");
            sblongitemdesc.Append(_porderitemline.Vendorcode.ToString().PadLeft(4, '0'));
            sblongitemdesc.Append("-");
            sblongitemdesc.Append(_porderitemline.Stylecode.ToString().PadLeft(4, '0'));
            sblongitemdesc.Append("-");
            sblongitemdesc.Append(_porderitemline.Colorcode.ToString().PadLeft(3, '0'));
            sblongitemdesc.Append("-");
            sblongitemdesc.Append(_porderitemline.Itemsize.ToString().PadLeft(4, '0'));

            lblLongItemDesc.Text = sblongitemdesc.ToString();
            lblUPCSKUValue.Text  = _porderitemline.UPC;

            lblDescValue1.Text     = _porderitemline.Itemlongdescription;
            lblDescValue2.Text     = _porderitemline.Itemshortdescription;
            lblClassNameValue.Text = _porderitemline.Classname;
            lblColorNameDesc.Text  = _porderitemline.Colordesc;
            lblSizeValue.Text      = _porderitemline.Sizename;

            lblVendorStyleValue.Text = _porderitemline.Vendorstyle;

            lblSeasonValue.Text       = _porderitemline.SeasonDesc;
            lblCharacterValue.Text    = _porderitemline.Characterdesc;
            lblCasePackTypeValue.Text = _porderitemline.Packdescription;
            lblCasePackQtyValue.Text  = _porderitemline.CasePackQty.ToString();
            lblInnerQtyValue.Text     = _porderitemline.DistroQty.ToString();

            lblHeightValue.Text = _porderitemline.Height.ToString();
            lblLengthValue.Text = _porderitemline.Length.ToString();
            lblWeightValue.Text = _porderitemline.Weight.ToString();
            lblWidthValue.Text  = _porderitemline.Width.ToString();

            // Calculation should be same as in Po Summary calculation

            decimal totalretailexvat = 0;
            //decimal marginvalue = 0;
            decimal landing = 1;

            _currencyratemarket = validationcls.GetCurrency(_porderdisplay.MarketCurrency);
            _currencyratepo = _porderdisplay.ExchangeRate;

            // HK : 28-12-2009 : Total Cost is item cost * item quantiy
            _totalcost = _porderitemline.Cost * _porderitemline.Itemquantity;

            if (_porderdisplay.Landing == 0)
            {
                landing = 1;
            }
            else
            {
                landing = _porderdisplay.Landing;
            }

            _totallandedcost = _totalcost * landing; //_porderdisplay.Landing;
            _totalretail = _porderitemline.Retailprice * _porderitemline.Itemquantity;

            // Total Retail Ex Vat
            // totalretailexvat = _porderdisplay.CalculateTotalRetailExVat(MAGICDCSTOREVATCODE);
            totalretailexvat = _porderdisplay.CalculateTotalRetailExVatItem(_porderitemline, MAGICDCSTOREVATCODE);

            //_totalmargin        = _totalretail - _totalcost;
            _totalmargin = totalretailexvat - _totallandedcost;

            if (totalretailexvat > 0)
            {
                //_totalmarginperc = Decimal.Round((Math.Abs(_totalmargin) / _totalretail * 100), 2);
                _totalmarginperc = Decimal.Round((_totalmargin / totalretailexvat) * 100, 2);
            }
            else
            {
                _totalmarginperc = 0;
            }


            lblTotalCostValue.Text = _totallandedcost.ToString();
            lblRetailValue.Text = _porderitemline.Retailprice.ToString();
            lblMarginValue.Text = _totalmargin.ToString();
            lblMarginPercValue.Text = _totalmarginperc.ToString();

            lblTotalRetailValue.Text = _totalretail.ToString();

            txtOrderQty.Text = _porderitemline.Itemquantity.ToString();

            txtUnitCost.Text = _porderitemline.ConvertedCost.ToString();

            //Currency codes
            lblTotalCostCurr.Text   = _porderdisplay.MarketCurrency;
            lblTotalRetailCurr.Text = _porderdisplay.MarketCurrency;
            lblMarginPercCurr.Text  = _porderdisplay.MarketCurrency;
            lblMarginValCurr.Text   = _porderdisplay.MarketCurrency;
            lblTotalRetailCurr.Text = _porderdisplay.MarketCurrency;
            lblRetailCurr.Text      = _porderdisplay.MarketCurrency;
            lblSimpleCostCcy.Text   = _porderdisplay.Currencycode;

            CalculateLineSummary();

            // If Po Line Item is a APP then Quantity cannot be changed
            if (_isitemcomponent)
            {
                txtOrderQty.Enabled = false;
                txtUnitCost.Focus();
            }
            else
            {
                txtOrderQty.Focus();
            }

            Cursor.Current = Cursors.Default;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
            this.Dispose();
        }

        public void CalculateLineSummary()
        {
            decimal totalretailexvat = 0;

            _porderitemline.Itemquantity = Int32.Parse(txtOrderQty.Text);

            //_totalcost = _porderitemline.Cost * _porderitemline.Itemquantity;
            //_totallandedcost = _totalcost * _porderdisplay.Landing;

            lblSimpleCostValue.Text = _porderitemline.ConvertedCost.ToString("N");

            _totalretail = _porderitemline.Retailprice * _porderitemline.Itemquantity;

            totalretailexvat = _porderdisplay.CalculateTotalRetailExVatItem(_porderitemline, MAGICDCSTOREVATCODE);

            _totalmargin = Math.Round(totalretailexvat - _totallandedcost, 2);

            //lblTotalCostValue.Text = _totallandedcost.ToString();
            lblTotalCostValue.Text = _porderdisplay.TotalCost.ToString("N");
            
            lblMarginValue.Text = _totalmargin.ToString();

            // Changing quantity or cost has same meaning
            _componentqty  = _porderitemline.Itemquantity;
            _componentcost = _porderitemline.Cost;
        }

        public class AppDetailsEventArgs : EventArgs
        {
            private POBO.POItemDetails _poline;
            public POBO.POItemDetails poline
            {
                get
                {
                    return _poline;
                }
            }

            private PurchaseOrder _porder;
            public PurchaseOrder porder
            {
                get
                {
                    return _porder;
                }
            }

            int _rowindex;
            public int rowindex
            {
                get
                {
                    return _rowindex;
                }
            }

            int _quantity;
            public int quantity
            {
                get
                {
                    return _quantity;
                }
            }

            public AppDetailsEventArgs(POBO.POItemDetails poline, PurchaseOrder porder, int rowindex, int quantity)
            {
                this._poline = poline;
                this._porder = porder;
                this._rowindex = rowindex;
                this._quantity = quantity;
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx"); 
        }
    }
}