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
    public partial class POLineDetails : Form
    {
        private PurchaseOrder  _porderdisplay;
        private POItemDetails  _porderitemline;

        private decimal     _totalcost;    
        private decimal     _totallandedcost;
        private decimal     _totalretail;
        private decimal     _totalmargin;
        private decimal     _totalmarginperc;
        private bool        _isitemcomponent;

        private Decimal unitcost;
        private Int32   quantity;

        private const string MAGICDCSTOREVATCODE = "A";
        private const string _currencyformat = "N"; 

        public POLineDetails(PurchaseOrder porder, ref POItemDetails poitemdetails, bool bcomponent)
        {
            InitializeComponent();

            _porderdisplay = porder;
            _porderitemline = poitemdetails;
            _isitemcomponent = bcomponent;

            InitialiseFormControls();
        }

        private void InitialiseFormControls()
        {
            StringBuilder sblongitemdesc = new StringBuilder();
            sblongitemdesc.Append(_porderitemline.ClassCode.ToString("0000"));
            sblongitemdesc.Append("-");
            sblongitemdesc.Append(_porderitemline.Vendorcode.ToString("00000"));
            sblongitemdesc.Append("-");
            sblongitemdesc.Append(_porderitemline.Stylecode.ToString("0000"));
            sblongitemdesc.Append("-");
            sblongitemdesc.Append(_porderitemline.Colorcode.ToString("000"));
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

            // Case Pack details
            lblCasePackTypeValue.Text = _porderitemline.Packdescription;
            lblCasePackQtyValue.Text  = _porderitemline.CasePackQty.ToString();
            lblInnerQtyValue.Text     = _porderitemline.DistroQty.ToString();
            lblHeightValue.Text       = _porderitemline.Height.ToString();
            lblLengthValue.Text       = _porderitemline.Length.ToString();
            lblWeightValue.Text       = _porderitemline.Weight.ToString();
            lblWidthValue.Text        = _porderitemline.Width.ToString();

            decimal totalretailexvat = 0;
            decimal landing          = 1;

            unitcost = _porderitemline.ConvertedCost;
            quantity = _porderitemline.Itemquantity;
            
            landing = (_porderdisplay.Landing == 0) ? 1 : _porderdisplay.Landing;

            _totalcost = Decimal.Round((_porderitemline.Cost * landing),2) * _porderitemline.Itemquantity;

            _totallandedcost = _totalcost; // Decimal.Round(_totalcost, 2);

            _totalretail     = _porderitemline.Retailprice * _porderitemline.Itemquantity;

            // Total Retail Ex Vat
            totalretailexvat = Decimal.Round(_porderdisplay.CalculateTotalRetailExVatItem(_porderitemline, MAGICDCSTOREVATCODE), 2);
            _totalmargin     = totalretailexvat - _totallandedcost;
            _totalmarginperc = (totalretailexvat > 0) ? Decimal.Round(_totalmargin / totalretailexvat * 100, 2) : 0;

            lblTotalCostValue.Text = decimal.Round(_totallandedcost, 2).ToString(_currencyformat);
            lblRetailValue.Text = _porderitemline.Retailprice.ToString(_currencyformat);
            lblMarginValue.Text     = _totalmargin.ToString(_currencyformat); // CJ 03-02-2010
            lblMarginPercValue.Text = _totalmarginperc.ToString(_currencyformat);
            Decimal SimpleCost = Decimal.Round(_porderitemline.ConvertedCost, 2);
            lblSimpleCost.Text = (Decimal.Round(SimpleCost * _porderitemline.Itemquantity, 2)).ToString(_currencyformat);

            lblTotalRetailValue.Text = _totalretail.ToString(_currencyformat);
            txtOrderQty.Text         = _porderitemline.Itemquantity.ToString();
            txtUnitCost.Text         = Decimal.Round(_porderitemline.ConvertedCost, 2).ToString();

            // Currency codes
            lblTotalCostCurr.Text   = _porderdisplay.MarketCurrency;
            lblTotalRetailCurr.Text = _porderdisplay.MarketCurrency;
            lblMarginPercCurr.Text  = _porderdisplay.MarketCurrency;
            lblMarginValCurr.Text   = _porderdisplay.MarketCurrency;
            lblTotalRetailCurr.Text = _porderdisplay.MarketCurrency;
            lblRetailCurr.Text      = _porderdisplay.MarketCurrency;
            lblSimpleCostCcy.Text   = _porderdisplay.Currencycode;
            lblUnitCostCurr.Text    = _porderdisplay.Currencycode;

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
            ValidateChildren();
            if (!string.IsNullOrEmpty(errorProvider.GetError(txtUnitCost))) return;
            if (!String.IsNullOrEmpty(errorProvider.GetError(txtOrderQty))) return;

            _porderitemline.ConvertedCost = unitcost;
            _porderitemline.Itemquantity = quantity;

            this.DialogResult = DialogResult.OK;
            this.Close();
            this.Dispose();
        }

        private void CalculateLineSummary()
        {
            unitcost = decimal.Parse(txtUnitCost.Text);
            quantity = int.Parse(txtOrderQty.Text);
            
            decimal landing = (_porderdisplay.Landing == 0) ? 1 : _porderdisplay.Landing;

            _totalcost = Decimal.Round((unitcost * landing),2) * quantity;

            _totallandedcost = _totalcost; // Decimal.Round(_totalcost, 2);

            lblTotalCostValue.Text = _totallandedcost.ToString(_currencyformat);

            _totalretail = _porderitemline.Retailprice * quantity;
            lblTotalRetailValue.Text = _totalretail.ToString(_currencyformat);

            Decimal totalretailexvat = Decimal.Round(this.CalculateTotalRetailExVatItem(_porderitemline, MAGICDCSTOREVATCODE), 2);
            if (totalretailexvat != 0)
            {
                _totalmargin = Decimal.Round(totalretailexvat - _totallandedcost, 2);
                lblMarginValue.Text = _totalmargin.ToString(_currencyformat);
                lblMarginPercValue.Text = Decimal.Round(_totalmargin / totalretailexvat * 100, 2).ToString();
            }
            else if (_totalretail != 0)
            {
                _totalmargin = Decimal.Round(_totalretail - _totallandedcost, 2);
                lblMarginValue.Text = _totalmargin.ToString(_currencyformat);
                lblMarginPercValue.Text = Decimal.Round(_totalmargin / _totalretail * 100, 2).ToString();
            }
            else
            {
                lblMarginValue.Text = "0.00";
                lblMarginPercValue.Text = "0.00";
            }

            
        }

        private void txtOrderQty_Validating(object sender, CancelEventArgs e)
        {
            errorProvider.SetError(txtOrderQty, string.Empty);
            if (String.IsNullOrEmpty(txtOrderQty.Text))
            {
                errorProvider.SetError(txtOrderQty,"Please enter a Quantity");
                e.Cancel = true;
                return;
            }
            
            if (!Int32.TryParse(txtOrderQty.Text, out quantity))
            {
                errorProvider.SetError(txtOrderQty, "You have entered an invalid quantity value");
                e.Cancel = true;
                return;
            }

            if (quantity > 999999 || quantity <= 0)
            {
                errorProvider.SetError(txtOrderQty, "Quantity must not be greater than 999,999, Zero, or Negative");
                e.Cancel = true;
                return;
            }

                if (_porderdisplay.PurchaseOrderType == PurchaseOrder.POtype.StandardDCPO)
                {
                    if (quantity % _porderitemline.CasePackQty != 0)
                    {
                        QuantityRounding roundingform = new QuantityRounding(quantity, _porderitemline.CasePackQty);
                        if (roundingform.ShowDialog(this) == DialogResult.OK)
                        {
                            quantity = roundingform.RoundedQuantity;
                            txtOrderQty.Text = quantity.ToString();

                            CalculateLineSummary();
                        }
                        else
                        {
                            errorProvider.SetError(txtOrderQty, "Quantity must be rounded to the nearest CasePack quantity");
                            e.Cancel = true;
                            return;
                        }
                    }
                }
                
                if (_porderdisplay.PurchaseOrderType != PurchaseOrder.POtype.StandardDCPO)
                {
                    if (quantity % _porderitemline.DistroQty != 0)
                    {
                        QuantityRounding roundingform = new QuantityRounding(quantity, _porderitemline.DistroQty);
                        if (roundingform.ShowDialog(this) == DialogResult.OK)
                        {
                            quantity = roundingform.RoundedQuantity;
                            txtOrderQty.Text = quantity.ToString();

                            CalculateLineSummary();
                        }
                        else
                        {
                            errorProvider.SetError(txtOrderQty, "Quantity must be rounded to the nearest Distro quantity");
                            e.Cancel = true;
                            return;
                        }
                    }
                }
                   
             else
             {
                CalculateLineSummary();
             }
        }

        private void txtUnitCost_Validating(object sender, CancelEventArgs e)
        {
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

            unitcost = Decimal.Round(unitcost, 2);
            txtUnitCost.Text = unitcost.ToString();

            CalculateLineSummary();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx"); 
        }

        public Decimal CalculateTotalRetailExVatItem(POItemDetails poItemDetails, string storevatcode)
        {
            decimal itemretailexvat = 0, itemsalesvatrate = 0;

            Boolean bSuccess = _porderdisplay.ItemSalesVATRateLookup(_porderdisplay.DefaultMarket, storevatcode, poItemDetails.VatCode, ref itemsalesvatrate);
            if (bSuccess)
            {
                itemretailexvat = (poItemDetails.Retailprice / (1 + itemsalesvatrate)) * quantity;
            }

            return itemretailexvat;
        }
    }
}