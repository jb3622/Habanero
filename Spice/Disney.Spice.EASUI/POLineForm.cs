using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Disney.Spice.EASUI
{
    public partial class POLineForm : Form
    {
        private LineDetails lineDetails;
        private const string _currencyformat = "N";

        public POLineForm(LineDetails lineDetails)
        {
            InitializeComponent();

            this.lineDetails = lineDetails;

            InitialiseValues();
        }

        private void InitialiseValues()
        {
            StringBuilder itemnumber = new StringBuilder("");

            itemnumber.Append(lineDetails.Class.ToString("0000"));
            itemnumber.Append("-");
            itemnumber.Append(lineDetails.Vendor.ToString("00000"));
            itemnumber.Append("-");
            itemnumber.Append(lineDetails.Style.ToString("0000"));
            itemnumber.Append("-");
            itemnumber.Append(lineDetails.Colour.ToString("000"));
            itemnumber.Append("-");
            itemnumber.Append(lineDetails.Size.ToString("0000"));

            lblLongItemNumber.Text = itemnumber.ToString();

            lblLongDescription.Text = lineDetails.LongDescription;
            lblShortDescription.Text = lineDetails.ShortDescription;

            lblSeasonDescription.Text = lineDetails.SeasonDescription;
            lblCharacterName.Text = lineDetails.CharacterName;

            
            lblUPCSKUValue.Text = Convert.ToString(lineDetails.VendorUPC) + ' ' + '/' + ' ' + Convert.ToString(lineDetails.SKUNumber);


            lblClassNameValue.Text = lineDetails.ClassName;
            lblColorNameDesc.Text  = lineDetails.ColourName;
            lblSizeValue.Text = lineDetails.SizeName;

            lblVendorStyleValue.Text = lineDetails.VendorStyle;

            lblCasePackTypeValue.Text = lineDetails.CasePackTypeDes;
            lblCasePackQtyValue.Text = Convert.ToString(lineDetails.CasePackQty);
            lblInnerQtyValue.Text = Convert.ToString(lineDetails.DistroInnerQty);
            lblHeightValue.Text = Convert.ToString(lineDetails.Height);
            lblWidthValue.Text = Convert.ToString(lineDetails.Width);
            lblLengthValue.Text = Convert.ToString(lineDetails.Length);
            lblWeightValue.Text = Convert.ToString(lineDetails.Weight);
            
            
            

            //decimal totalretailexvat = 0;
            //decimal landing          = 1;

            //_totalcost = _porderitemline.Cost * _porderitemline.Itemquantity;

            //landing = (_porderdisplay.Landing == 0) ? 1 : _porderdisplay.Landing;
            //_totallandedcost = Decimal.Round(_totalcost * landing, 2);

            //_totalretail = _porderitemline.Retailprice * _porderitemline.Itemquantity;

            //totalretailexvat = _porderdisplay.CalculateTotalRetailExVatItem(_porderitemline, MAGICDCSTOREVATCODE);
            //_totalmargin = totalretailexvat - _totallandedcost;
            //_totalmarginperc = (totalretailexvat > 0) ? Decimal.Round(_totalmargin / totalretailexvat * 100, 2) : 0;

            //lblTotalCostValue.Text = _totallandedcost.ToString();
            //lblRetailValue.Text = _porderitemline.Retailprice.ToString();
            //lblMarginValue.Text = _totalmargin.ToString();
            //lblMarginPercValue.Text = _totalmarginperc.ToString();
            //lblSimpleCost.Text = (_porderitemline.ConvertedCost * _porderitemline.Itemquantity).ToString("N");

            //lblTotalRetailValue.Text = _totalretail.ToString();

            //Decimal VatRate = Decimal.Round(


            txtOrderQty.Text = lineDetails.POOrderQty.ToString();
            txtUnitCost.Text = lineDetails.POSimpleVendorCost.ToString();
            lblUnitCostCurr.Text = "(" + lineDetails.POCurrency + ")";
            lblRetailValue.Text = lineDetails.PORetailPrice.ToString(_currencyformat);    
            lblRetailCurr.Text = "(" + lineDetails.MarketCurrency + ")";

            Decimal TotalCostValue = lineDetails.POOrderQty * lineDetails.POSimpleVendorCost;
            lblTotalCostValue.Text = (TotalCostValue.ToString(_currencyformat));
            lblTotalCostCurr.Text = "(" + lineDetails.POCurrency + ")";

            //decimal totalLandedCost = Decimal.Round((lineDetails.POSimpleVendorCost * lineDetails.LandingFactor) * (lineDetails.POOrderQty) * (lineDetails.CurrencyRate),2);
            decimal totalLandedCost = Decimal.Round((lineDetails.LandedCost * lineDetails.POOrderQty) , 2);
            lblTotalLandedCostValue.Text = totalLandedCost.ToString(_currencyformat);
            lblTotalLandedCostCurr.Text = "(" + lineDetails.MarketCurrency+ ")";
            Decimal TotalRetailValue = Decimal.Round(lineDetails.POOrderQty * lineDetails.PORetailPrice, 2);
            lblTotalRetailValue.Text = TotalRetailValue.ToString(_currencyformat);
            lblTotalRetailCurr.Text = "(" + lineDetails.MarketCurrency + ")";

            decimal itemRetailXvat = lineDetails.PORetailPrice / (1 + lineDetails.VatRate) * lineDetails.POOrderQty;
            decimal marginValue = Decimal.Round((itemRetailXvat - totalLandedCost), 2);

            lblMarginValue.Text = marginValue.ToString(_currencyformat);
            
            lblMarginValCurr.Text = "(" + lineDetails.MarketCurrency + ")";

            decimal marginperc = Decimal.Round((marginValue / itemRetailXvat) * 100, 2);
            lblMarginPercValue.Text = marginperc.ToString(_currencyformat);
            lblMarginPercCurr.Text = "(" + lineDetails.MarketCurrency + ")";
            
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

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx"); 
        }
    }
}