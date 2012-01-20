using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Disney.Spice.POBO;
using Disney.Spice.ErrorProvider;

namespace Disney.Spice.POUI
{
    public partial class ComponentDetails : Form
    {
        private PurchaseOrder  _porderdisplay;
        private POItemDetails  _porderitemline;
        private Validation validationcls;
        private APPcomponent _componentdetails;

        private decimal     _totalcost;    
        private decimal     _totallandedcost;
        private decimal     _totalretail;
        private decimal     _totalmargin;
        private decimal     _totalmarginperc;
        private bool        _isitemcomponent;

        private int         _componentqty;
        private decimal     _componentcost;
        
        private Boolean     bComponentCostChanged;
        private Boolean     bQuantityChanged;

        private decimal     _currencyratemarket;
        private decimal     _currencyratepo;

        private decimal     _landedcost;

        private const string MAGICDCSTOREVATCODE = "A";
        private const string _currencyformat = "N"; 

        public ComponentDetails(PurchaseOrder porder, ref POItemDetails poitemdetails, bool bcomponent)
        {
            InitializeComponent();

            _porderdisplay = porder;
            _porderitemline = poitemdetails;
            _isitemcomponent = bcomponent;

            validationcls = new Validation(_porderdisplay.DbParamRef, _porderdisplay.UserName, _porderdisplay.Penvironment);

            InitialiseValues();
        }

        public ComponentDetails(PurchaseOrder porder, ref APPcomponent componentdetails, bool bcomponent)
        {
            InitializeComponent();

            _porderdisplay   = porder;
            _componentdetails = componentdetails;
            _isitemcomponent = bcomponent;
        }

        private void InitialiseValues()
        {
            StringBuilder sblongitemdesc = new StringBuilder();
            sblongitemdesc.Append(_porderitemline.ClassCode.ToString("0000"));
            sblongitemdesc.Append("-");
            sblongitemdesc.Append(_porderitemline.Vendorcode.ToString("00000"));
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

            lblSeasonValue.Text    = _porderitemline.SeasonDesc;
            lblCharacterValue.Text = _porderitemline.Characterdesc;
                        
            // Calculation should be same as in PO Summary calculation
            decimal totalretailexvat = 0;
            decimal landing          = 1;

            _currencyratemarket = validationcls.GetCurrency(_porderdisplay.MarketCurrency);
            _currencyratepo     = _porderdisplay.ExchangeRate;

            // Total Cost is item cost * item quantiy
            _totalcost = _porderitemline.Cost * _porderitemline.Itemquantity;

            if (_porderdisplay.Landing == 0)
            {
                landing = 1;
            }
            else
            {
                landing = _porderdisplay.Landing;
            }

            _totallandedcost = _totalcost * landing;
            _totalretail     = _porderitemline.Retailprice * _porderitemline.Itemquantity;

            totalretailexvat = _porderdisplay.CalculateTotalRetailExVatItem(_porderitemline, MAGICDCSTOREVATCODE);
            _totalmargin     = totalretailexvat - _totallandedcost;

            if (totalretailexvat > 0)
            {
                _totalmarginperc = Decimal.Round((_totalmargin / totalretailexvat) * 100, 2);
            }
            else
            {
                _totalmarginperc = 0;
            }

            lblTotalCostValue.Text = decimal.Round(_totallandedcost, 2).ToString(_currencyformat);
            lblRetailValue.Text = _porderitemline.Retailprice.ToString(_currencyformat);
            lblMarginValue.Text     = _totalmargin.ToString(_currencyformat); // CJ 03-02-2010
            lblMarginPercValue.Text = _totalmarginperc.ToString(_currencyformat);
            lblSimpleCost.Text = (_porderitemline.ConvertedCost * _porderitemline.Itemquantity).ToString(_currencyformat);

            lblTotalRetailValue.Text = _totalretail.ToString(_currencyformat);
            txtOrderQty.Text         = _porderitemline.Itemquantity.ToString();
            txtUnitCost.Text         = _porderitemline.ConvertedCost.ToString();

            // Currency codes
            lblTotalCostCurr.Text   = _porderdisplay.MarketCurrency;
            lblTotalRetailCurr.Text = _porderdisplay.MarketCurrency;
            lblMarginPercCurr.Text  = _porderdisplay.MarketCurrency;
            lblMarginValCurr.Text   = _porderdisplay.MarketCurrency;
            lblTotalRetailCurr.Text = _porderdisplay.MarketCurrency;
            lblRetailCurr.Text      = _porderdisplay.MarketCurrency;
            lblSimpleCostCcy.Text   = _porderdisplay.Currencycode;
            lblUnitCostCurr.Text    = _porderdisplay.Currencycode;

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

            this.DialogResult = DialogResult.OK;
            this.Close();
            this.Dispose();
        }

        private void CalculateLineSummary()
        {
            decimal totalretailexvat = 0;

            _totalcost       = _porderitemline.Cost        * _porderitemline.Itemquantity;
            _totallandedcost = _totalcost                  * _porderdisplay.Landing;
            _totalretail     = _porderitemline.Retailprice * _porderitemline.Itemquantity;
            
            totalretailexvat = _porderdisplay.CalculateTotalRetailExVatItem(_porderitemline, MAGICDCSTOREVATCODE);
            _totalmargin     = Decimal.Round(totalretailexvat - _totallandedcost, 2);

            lblTotalCostValue.Text   = Decimal.Round(_totallandedcost, 2).ToString(_currencyformat);
            lblMarginValue.Text = _totalmargin.ToString(_currencyformat);
            lblSimpleCost.Text = (_porderitemline.ConvertedCost * _porderitemline.Itemquantity).ToString(_currencyformat);
            lblTotalRetailValue.Text = Decimal.Round(_porderitemline.Itemquantity * _porderitemline.Retailprice, 2).ToString(_currencyformat);

            _componentqty  = _porderitemline.Itemquantity;
            _componentcost = _porderitemline.Cost;
        }

        private void txtOrderQty_Validating(object sender, CancelEventArgs e)
        {
            Int32 orderQty;

            errorProvider.SetError(txtOrderQty, string.Empty);
            if (String.IsNullOrEmpty(txtOrderQty.Text))
            {
                errorProvider.SetError(txtOrderQty,"Please enter a Quantity");
                e.Cancel = true;
                return;
            }
            
            if (!Int32.TryParse(txtOrderQty.Text, out orderQty))
            {
                errorProvider.SetError(txtOrderQty, "You have entered an invalid quantity value");
                e.Cancel = true;
                return;
            }

            if (orderQty > 999999 || orderQty <= 0)
            {
                errorProvider.SetError(txtOrderQty, "Quantity must not be greater than 999,999, Zero, or Negative");
                e.Cancel = true;
                return;
            }

            if (orderQty % _porderitemline.DistroQty != 0)
            {
                QuantityRounding roundingform = new QuantityRounding(orderQty, _porderitemline.DistroQty);
                if (roundingform.ShowDialog(this) == DialogResult.OK)
                {
                    txtOrderQty.Text = roundingform.RoundedQuantity.ToString();
                    _porderitemline.Itemquantity = orderQty;
                    CalculateLineSummary();
                }
                else
                {
                    errorProvider.SetError(txtOrderQty, "Quantity must be rounded to the nearest Distro quantity");
                    e.Cancel = true;
                    return;
                }
            }
            else
            {
                txtOrderQty.Text = orderQty.ToString();
                _porderitemline.Itemquantity = orderQty;
                CalculateLineSummary();
            }
        }

        private void txtUnitCost_Validating(object sender, CancelEventArgs e)
        {
            decimal unitcost;
            
            errorProvider.SetError(txtUnitCost, string.Empty);

            if (string.IsNullOrEmpty(txtUnitCost.Text))
            {
                errorProvider.SetError(txtUnitCost, "Please enter a cost value");
                e.Cancel = true;
                return;
            }

            if (!decimal.TryParse(txtUnitCost.Text,out unitcost))
            {
                errorProvider.SetError(txtUnitCost, "Invalid number");
                e.Cancel = true;
                return;
            }

            if (unitcost <= 0)
            {
                errorProvider.SetError(txtUnitCost, "Cost cannot be zero or negative");
                e.Cancel = true;
                return;
            }


            txtUnitCost.Text = decimal.Round(unitcost, 2).ToString();
            _porderitemline.ConvertedCost = unitcost;
            CalculateLineSummary();
        }
                
        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx"); 
        }

    }
}