using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Disney.Spice.POBO;
using System.Diagnostics;

namespace Disney.Spice.POUI
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

        private Boolean     bComponentCostChanged;
        private Boolean     bQuantityChanged;

        private decimal     _currencyratemarket;
        private decimal     _currencyratepo;

        private Validation validationcls;
        

        // Landed Cost and Calculated Cost
        private decimal     _landedcost;
        private decimal     _calculatedcost;

        // Constants
        private const string MAGICDCSTOREVATCODE = "A";
        private const string _currencyformat = "N"; 

        public POLineForm()
        {
            InitializeComponent();
        }

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

            sblongitemdesc.Append(_porderitemline.Classcode.ToString().PadLeft(4, '0'));
            sblongitemdesc.Append("-");
            sblongitemdesc.Append(_porderitemline.Vendorcode.ToString().PadLeft(4, '0'));
            sblongitemdesc.Append("-");
            sblongitemdesc.Append(_porderitemline.Stylecode.ToString().PadLeft(4, '0'));
            sblongitemdesc.Append("-");
            sblongitemdesc.Append(_porderitemline.Colorcode.ToString().PadLeft(3, '0'));
            sblongitemdesc.Append("-");
            sblongitemdesc.Append(_porderitemline.Itemsize.ToString().PadLeft(4, '0'));
            
            lblLongItemDesc.Text    = sblongitemdesc.ToString();
            lblUPCSKUValue.Text     = _porderitemline.Upc;

            lblDescValue1.Text     = _porderitemline.Itemlongdescription;
            lblDescValue2.Text     = _porderitemline.Itemshortdescription;
            lblClassNameValue.Text = _porderitemline.Classname;
            lblColorNameDesc.Text  = _porderitemline.Colordesc;
            lblSizeValue.Text      = _porderitemline.Sizename;

            lblVendorStyleValue.Text = _porderitemline.Vendorstyle;

            lblSeasonValue.Text      = _porderitemline.SeasonDesc;
            lblCharacterValue.Text   = _porderitemline.Characterdesc;

            // Case Pack details
            lblCasePackTypeValue.Text = _porderitemline.Packdescription;
            lblCasePackQtyValue.Text  = _porderitemline.Casepackqty.ToString();
            lblInnerQtyValue.Text     = _porderitemline.Distrolot.ToString();
            lblHeightValue.Text       = _porderitemline.Height.ToString();
            lblLengthValue.Text       = _porderitemline.Length.ToString();
            lblWeightValue.Text       = _porderitemline.Weight.ToString();
            lblWidthValue.Text        = _porderitemline.Width.ToString();

            // Calculation should be same as in PO Summary calculation
            decimal totalretailexvat    = 0;
            decimal landing             = 1;

            _currencyratemarket = validationcls.GetCurrency(_porderdisplay.MarketCurrency);
            _currencyratepo     = _porderdisplay.ExchangeRate;

            // Total Cost is item cost * item quantiy
            _totalcost              = _porderitemline.Cost * _porderitemline.Itemquantity;

            if (_porderdisplay.Landing == 0)
            {
                landing = 1;
            }
            else
            {
                landing = _porderdisplay.Landing;
            }

            _totallandedcost    = _totalcost * landing; //_porderdisplay.Landing;
            _totalretail        = _porderitemline.Retailprice * _porderitemline.Itemquantity;

            // Total Retail Ex Vat
            // totalretailexvat = _porderdisplay.CalculateTotalRetailExVat(MAGICDCSTOREVATCODE);
            totalretailexvat    = _porderdisplay.CalculateTotalRetailExVatItem(_porderitemline, MAGICDCSTOREVATCODE);
            
            //_totalmargin        = _totalretail - _totalcost;
            _totalmargin          = totalretailexvat - _totallandedcost;

            if (totalretailexvat > 0)
            {
                //_totalmarginperc = Decimal.Round((Math.Abs(_totalmargin) / _totalretail * 100), 2);
                _totalmarginperc = Decimal.Round((_totalmargin / totalretailexvat) * 100, 2);
            }
            else
            {
                Debug.Print("Margin percentage not calculated as totalretailexvat = 0:");
                _totalmarginperc = 0;
            }

            lblTotalCostValue.Text      = _totallandedcost.ToString();
            lblRetailValue.Text         = _porderitemline.Retailprice.ToString();
            lblMarginValue.Text         = _totalmargin.ToString(_currencyformat); // CJ 03-02-2010
            lblMarginPercValue.Text     = _totalmarginperc.ToString();

            lblTotalRetailValue.Text    = _totalretail.ToString();
            txtOrderQty.Text            = _porderitemline.Itemquantity.ToString();
            txtUnitCost.Text            = _porderitemline.ConvertedCost.ToString ();

            //Currency codes
            lblTotalCostCurr.Text       = _porderdisplay.Currencycode;
            lblTotalRetailCurr.Text     = _porderdisplay.Currencycode;
            lblMarginPercCurr.Text      = _porderdisplay.Currencycode;
            lblMarginValCurr.Text       = _porderdisplay.Currencycode;
            lblTotalRetailCurr.Text     = _porderdisplay.Currencycode;
            lblRetailvCurr.Text         = _porderdisplay.Currencycode;

            // HK : 14-11-2009 : Calculate Summary
            CalculateLineSummary();

            // HK : 17-11-2009 : If Po Line Item is a APP then Quantity cannot be changed
            if (_isitemcomponent)
            {
                txtOrderQty.Enabled = false;
                txtUnitCost.Focus();
            }
            else
            {
                txtOrderQty.Focus();
            }

            // hourglass cursor
            Cursor.Current = Cursors.Default;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ValidateChildren();
            if (!string.IsNullOrEmpty(errorProvider.GetError(txtUnitCost))) return;

            if (bQuantityChanged)
            {
                _porderitemline.Itemquantity = int.Parse(txtOrderQty.Text);
            }

            if (bComponentCostChanged)
            {
                decimal convertedcost;
                if (Decimal.TryParse(txtUnitCost.Text, out convertedcost))
                    _porderitemline.ConvertedCost = convertedcost;
                else
                    _porderitemline.ConvertedCost = 0;
            }

            // HHK : 11-11-2009 : Dont know why 0 is always passed when event is raised.
            // Ideally it should be the Itemindex property of POItemDetails.
            // However the grid counts rows based on 0 based index. 
            // When _porderitemline.Itemindex = 1 corresponding Grid Item is 0
            // When _porderitemline.Itemindex = 2 corresponding Grid Item is 1
            // When _porderitemline.Itemindex = 3 corresponding Grid Item is 2
            
            //                        ^^ ^^
            //                        || ||
            // HK : 28-12-2009 : The above assumption is wrong. See note below.
            
            // HK : 28-12-2009 : While unit testing the Bug ?? it came to my attention that 
            // as the item(s) get deleted or inserted on the main PO Entrry form, the location
            // of that item in the grid (rowindex) will not match with the items class property 
            // Itemindex, hence we must find the correct record in the main Po Entry form or 
            // change the constructor to send the datagrid row index as we do when we call this 
            // form from the PO Line Details pack form.

            // HK : 01-01-2010 : Use datagridrowindex instead of _porderitemline.Itemindex
            // when the below event is raised and the correct record will be updated.
            // In the Po Entry form call the overloaded constructor with the datagridindex.
            //_porderitemline.RaiseItemorQtyChanged (_porderitemline.Itemindex -1);
            
            _porderitemline.RaiseItemorQtyChanged(_datagridrowindex);

            // Since any change in quantity or cost will cause the entire 
            // retail to be recalculated in the grid I will channel them on the same event 
            // handler

            if ((_isitemcomponent && (bComponentCostChanged || bQuantityChanged)))
            {
                AppDetailsEventArgs e1 = new AppDetailsEventArgs(_porderitemline, _porderdisplay, _datagridrowindex, _porderitemline.Itemquantity);
                RaiseComponentQuantityOrCostChangedEvent(e1);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
            this.Dispose();
        }

        public void CalculateLineSummary()
        {
            //do the line level summary here         

            decimal totalretailexvat    = 0;
            //decimal marginvalue         = 0;

            // HK : Fix Bug
            // Remove VAT from Retail value and cost should be in 
            // PO Currency and not in Markey currency

            _porderitemline.Itemquantity = Int32.Parse(txtOrderQty.Text);

            // HK : 14-01-2010 : "Cost" i.e simple vendor cost cannot change.
            // This form is now for changing Converted Cost and not simple vendor cost.
            // So comment the below
            //_porderitemline.Cost = Decimal.Parse(txtUnitCost.Text);

            _totalcost          = _porderitemline.Cost          * _porderitemline.Itemquantity;
            _totallandedcost    = _totalcost                    * _porderdisplay.Landing;
            
            _totalretail        = _porderitemline.Retailprice   * _porderitemline.Itemquantity;
            
            // HK 28-12-2009 : Total Retail Ex Vat
            //totalretailexvat  = _porderdisplay.CalculateTotalRetailExVat(MAGICDCSTOREVATCODE);
            totalretailexvat    = _porderdisplay.CalculateTotalRetailExVatItem(_porderitemline, MAGICDCSTOREVATCODE);
            
            //_totalmargin          = _totalretail - _totalcost;
            _totalmargin            = Decimal.Round(totalretailexvat - _totallandedcost, 2);

            lblTotalCostValue.Text  = _totallandedcost.ToString();
            lblRetailValue.Text     = _totalretail.ToString();
            lblMarginValue.Text     = _totalmargin.ToString();

            // Changing quantity or cost has same meaning
            _componentqty   = _porderitemline.Itemquantity;
            _componentcost  = _porderitemline.Cost;

        }

        private void txtOrderQty_Validating(object sender, CancelEventArgs e)
        {
            bQuantityChanged = true;
            CalculateLineSummary();
        }

        private void txtUnitCost_Validating(object sender, CancelEventArgs e)
        {
            decimal unitcost;
            
            errorProvider.SetError(txtUnitCost, string.Empty);
            if (decimal.TryParse(txtUnitCost.Text,out unitcost))
            {
                bComponentCostChanged = true;
                CalculateLineSummary();
            }
            else
            {
                errorProvider.SetError(txtUnitCost,"Invalid number");
                e.Cancel = true;
            }
        }

        private void RaiseComponentQuantityOrCostChangedEvent(AppDetailsEventArgs e)
        {
            if (OnAppQuantityOrCostChanged != null)
            {
                OnAppQuantityOrCostChanged(this, e);
            }
        }

        public class AppDetailsEventArgs : EventArgs
        {
            private POBO.POItemDetails _poline;
            private PurchaseOrder _porder;
            int _rowindex;
            int _quantity;

            public POBO.POItemDetails poline
            {
                get
                {
                    return _poline;
                }
            }

            public PurchaseOrder porder
            {
                get
                {
                    return _porder;
                }
            }

            public int rowindex
            {
                get
                {
                    return _rowindex;
                }
            }

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

        // Calculate Landed Cost
        private decimal CalculateLandedCost()
        {
            return Decimal.Round((_porderitemline.Cost * _currencyratemarket) * _porderdisplay.Landing, 2);
        }

        // Calculate Converted Cost
        private decimal CalculateConvertedCost()
        {
            if (_currencyratepo != 0)
            {
                return Decimal.Round((_porderitemline.Cost * _currencyratemarket) / _currencyratepo, 2);
            }

            return 0;
        }
    }
}