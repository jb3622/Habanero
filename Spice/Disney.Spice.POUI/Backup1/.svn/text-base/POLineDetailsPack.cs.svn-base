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
    public partial class POLineDetailsPack : Form
    {
        public delegate void AppQuantityChangedEventHandler(object sender, AppDetailsEventArgs e);
        public event         AppQuantityChangedEventHandler OnAppQuantityChanged;

        private POItemDetails _poline;
        private POLineDetails polineform;
        private PurchaseOrder _porder;

        private int _rowindex;
        //private decimal _costalllines = 0;

        // Capture the Quantity rounded UP or DOWN by the user
        // When 0 then the user pressed 'Cancel' button
        //private int _itemquantityrounded = 0;

        // Some attributes of the PO item are populated 
        // by the validation classes and not ItemLookup
        //private Validation validationcls;

        //private Decimal _currencyratemarket;
        //private Decimal _currencyratepo;

        //public POLineDetailsPack(POBO.POItemDetails poline, PurchaseOrder porder)
        //{
        //    _poline = poline;
        //    _porder = porder;
        //    InitializeComponent();
        //}

        public POLineDetailsPack(POBO.POItemDetails poline, PurchaseOrder porder, int rowindex)
        {
            _poline   = poline;
            _porder   = porder;
            _rowindex = rowindex;

            //validationcls = new Validation(_porder.DbParamRef, _porder.UserName, _porder.Penvironment);
            InitializeComponent();
        }

        private void POLineDetailsPack_Load(object sender, EventArgs e)
        {
            //_currencyratemarket = validationcls.GetCurrency(_porder.MarketCurrency);
            //_currencyratepo     = _porder.ExchangeRate;
           
            string sShtDesc  = _poline.Itemshortdescription;
            string sVenStyle = _poline.Vendorstyle;

            StringBuilder sblongitemdesc = new StringBuilder("");
            
            sblongitemdesc.Append(_poline.ClassCode.ToString().PadLeft(4, '0'));
            sblongitemdesc.Append("-");
            sblongitemdesc.Append(_poline.Vendorcode.ToString().PadLeft(4, '0'));
            sblongitemdesc.Append("-");
            sblongitemdesc.Append(_poline.Stylecode.ToString().PadLeft(4, '0'));
            sblongitemdesc.Append("-");
            sblongitemdesc.Append(_poline.Colorcode.ToString().PadLeft(3, '0'));
            sblongitemdesc.Append("-");
            sblongitemdesc.Append(_poline.Itemsize.ToString().PadLeft(4, '0'));
            lblLongItemValue.Text= sblongitemdesc.ToString();

            lblClassNameValue.Text = _poline.Classname;

            lblVendorValue.Text = _poline.Vendorcode + " - " + _poline.Vendordesc;
            lblDescriptionValue.Text = _poline.Itemlongdescription;
           
            lblDescriptionValue1.Text = sShtDesc;
            lblVendorRefValue.Text = sVenStyle;

            txtQuantity.Text = _poline.Itemquantity.ToString();
            PopulateGrid(_poline.Itemquantity);
        }

        private void PopulateGrid(Int32 OrderQuantity)
        {
            Int32 i;
            if (_poline.Components.Count != 0)
            {
                foreach (APPcomponent component in _poline.Components)
                {
                    i = dgvPackDetails.Rows.Add();
                    dgvPackDetails["RowNumber", i].Value = i;
                    dgvPackDetails["ComponentClass", i].Value  = component.ComponentClass;
                    dgvPackDetails["ComponentVendor", i].Value = component.ComponentVendor;
                    dgvPackDetails["ComponentStyle", i].Value  = component.ComponentStyle;
                    dgvPackDetails["ComponentColour", i].Value = component.ComponentColour;
                    dgvPackDetails["ComponentSize", i].Value   = component.ComponentSize;
                    dgvPackDetails["ComponentLongDesc", i].Value = component.ItemDescription;
                    dgvPackDetails["RatioQuantity", i].Value = component.RatioQuantity;
                    dgvPackDetails["ComponentCost", i].Value = component.Cost;
                    dgvPackDetails["Retail", i].Value = component.Retail;
                    dgvPackDetails["ItemQuantity", i].Value  = component.RatioQuantity * OrderQuantity;
                }
            }
        }

        //private void PopulateComponents(DataTable dtInitialLoad)
        //{
        //    for (int i = 0; i < dtInitialLoad.Rows.Count; i++)
        //    {
        //        POItemDetails pocomponent = new POItemDetails(i);

        //        pocomponent.ClassCode  = (short)dtInitialLoad.Rows[i]["ComponentClass"];
        //        pocomponent.Colorcode  = (short)dtInitialLoad.Rows[i]["ComponentColour"];
        //        pocomponent.Stylecode  = (short)dtInitialLoad.Rows[i]["ComponentStyle"];
        //        pocomponent.Vendorcode = (int)dtInitialLoad.Rows[i]  ["ComponentVendor"];
        //        pocomponent.Itemsize   = (short)dtInitialLoad.Rows[i]["ComponentSize"];

        //        // Some attributes of the PO item are populated 
        //        // by the validation classes and not ItemLookup

        //        // ClassName
        //        List<string> retValues = validationcls.ValidateClass(pocomponent.ClassCode.ToString());
        //        pocomponent.Classname = retValues[1].ToString();

        //        // Vendor
        //        retValues = validationcls.ValidateVendor(pocomponent.Vendorcode.ToString(), true);
        //        pocomponent.Vendordesc = retValues[1].ToString();

        //        // Color
        //        retValues = validationcls.ValidateColour(pocomponent.Colorcode.ToString());
        //        pocomponent.Colordesc = retValues[1];

        //        // Size
        //        retValues = validationcls.ValidateSize(pocomponent.Itemsize.ToString ());
        //        pocomponent.Sizename = retValues[1];

        //        //This function assumes the reqd fields have populated
        //        pocomponent.ItemLookup(_porder.DbParamRef, _porder.UserName, _porder.Penvironment, _porder.DefaultMarket);

        //        // Apply the ItemQuantity to the PO Component ( PO Item Details) business object 
        //        pocomponent.Itemquantity = (short)dtInitialLoad.Rows[i]  ["ComponentQuantity"];
        //        pocomponent.Cost         = (decimal)dtInitialLoad.Rows[i]["ComponentCost"];

        //        // Apply Converted Cost and Landed Cost
        //        pocomponent.LandedCost = Decimal.Round((pocomponent.Cost * _currencyratemarket) * _porder.Landing, 2);
        //        dtInitialLoad.Rows[i]["LandedCost"] = pocomponent.LandedCost;

        //        if (_currencyratepo != 0)
        //        {
        //            pocomponent.ConvertedCost = Decimal.Round((pocomponent.Cost * _currencyratemarket) / _currencyratepo, 2);
        //            dtInitialLoad.Rows[i]["ConvertedCost"] = pocomponent.ConvertedCost;
        //        }

        //        _poline.pocomponents.Add(pocomponent);
        //    }
        //}

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Loop thru all component and calculate cost value
            //Decimal totalcost = 0;
            //foreach (POBO.POItemDetails components in _poline.pocomponents)
            //{
            //    totalcost += components.Itemquantity * components.ConvertedCost;
            //}
            //_poline.ConvertedCost = totalcost;

            //AppDetailsEventArgs e1 = new AppDetailsEventArgs (_poline, _porder, _rowindex, _poline.Itemquantity);
            //this.DialogResult = DialogResult.OK;
            //RaiseAppQuantityChangedEvent(e1);

            Decimal totalcost = 0;
            foreach (APPcomponent component in _poline.Components)
            {
                totalcost += component.RatioQuantity * component.Cost;
            }

            _poline.Cost = Decimal.Round(totalcost, 2);

            this.Close();
        }

        private void txtQuantity_Validating(object sender, CancelEventArgs e)
        {
            errorProvider.SetError(txtQuantity, string.Empty);
            if (string.IsNullOrEmpty(txtQuantity.Text))
            {
                errorProvider.SetError(txtQuantity, "Please enter an order quantity");
                e.Cancel = true;
                return;
            }

            Int32 quantity;
            if (!Int32.TryParse(txtQuantity.Text, out quantity))
            {
                errorProvider.SetError(txtQuantity, "The Quantity you have entered is invalid");
                e.Cancel = true;
                return;
            }

            if (quantity > 999999 || quantity <= 0)
            {
                errorProvider.SetError(txtQuantity, "Quantity must not be greater than 999,999, Zero, or Negative");
                e.Cancel = true;
                return;
            }

            _poline.Itemquantity = quantity;
        }

        private void txtQuantity_Validated(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in dgvPackDetails.Rows)
            {
                dr.Cells["ItemQuantity"].Value = (Int16)dr.Cells["RatioQuantity"].Value * _poline.Itemquantity;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void dgvPackDetails_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1) return;

            //Cursor.Current = Cursors.WaitCursor;
            //polineform = new POLineDetails(_porder, ref _poline.pocomponents[e.RowIndex], true);
            //polineform.Show(this);
            //polineform = null;
        }

        //private void AddComponentToList(int rowindex, POItemDetails pocomponent)
        //{
        //    POItemDetails poc;

        //    poc = new POItemDetails(pocomponent.Itemindex);
        //    poc = pocomponent;

        //    //_poline.pocomponents.Add(poc);
        //    //_poline.pocomponents[rowindex] = poc;
        //    //if (_poline.pocomponents.IndexOf(poc) >= 0)
        //    //if (_poline.pocomponents.Count > 0 && _poline.pocomponents[rowindex] != null && rowindex < _poline.pocomponents.Count)
        //    //if (_poline.pocomponents.IndexOf(poc) == -1)

        //    if (_poline.lstComponentDetails.IndexOf (poc) == -1)
        //    {
        //        _poline.lstComponentDetails.Insert(rowindex, poc);
        //    }
        //    else
        //    {
        //        _poline.lstComponentDetails[rowindex] = poc;
        //       // _poline.pocomponents[rowindex] = poc;
        //    }
        //}

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

            public AppDetailsEventArgs (POBO.POItemDetails poline, PurchaseOrder porder, int rowindex, int quantity)
            {
                this._poline    = poline;
                this._porder    = porder;
                this._rowindex  = rowindex;
                this._quantity  = quantity;
            }
        }

        private void RaiseAppQuantityChangedEvent(AppDetailsEventArgs e)
        {
            if (OnAppQuantityChanged != null)
            {
                OnAppQuantityChanged(this, e);
            }
        }

        private void dgvPackDetails_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dgvPackDetails.Columns[e.ColumnIndex].Name.Equals("ComponentCost"))
            {
                dgvPackDetails.Rows[e.RowIndex].ErrorText = String.Empty;

                if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
                {
                    dgvPackDetails.Rows[e.RowIndex].ErrorText = "Please enter a cost value";
                    e.Cancel = true;
                    return;
                }

                decimal componentcost;
                if (!Decimal.TryParse(e.FormattedValue.ToString(), out componentcost))
                {
                    dgvPackDetails.Rows[e.RowIndex].ErrorText = "The cost value entered is invalid";
                    e.Cancel = true;
                    return;
                }

                if (componentcost.Equals(0) || componentcost < 0)
                {
                    dgvPackDetails.Rows[e.RowIndex].ErrorText = "The cost value cannot be negative or zero";
                    e.Cancel = true;
                    return;
                }

                _poline.Components[Convert.ToInt32(dgvPackDetails["RowNumber",e.RowIndex].Value)].Cost = componentcost;
            }
        }

        //private void dgvPackDetails_CellValidated(object sender, DataGridViewCellEventArgs e)
        //{
        //    int     nominalquantity;
        //    decimal cost;

        //    _costalllines = 0;

        //    if (dgvPackDetails.Columns[e.ColumnIndex].Name.Equals("ComponentCost"))
        //    {
        //        foreach (DataGridViewRow dr in dgvPackDetails.Rows)
        //        {
        //            if (dr.Index < dgvPackDetails.Rows.Count - 1)
        //            {
        //                //Total the cost across all component lines
        //                //nominalquantity = (int)dtgrdVwPOLineDetailPack ["ComponentQuantity", dr.Index].Value;
        //                nominalquantity = Convert.ToInt32(dr.Cells["ComponentQuantity"].Value);
        //                cost = (decimal)dgvPackDetails["ComponentCost", dr.Index].Value;
        //                _costalllines = _costalllines + (nominalquantity * cost);
        //            }
        //        }

        //        _poline.Cost = _costalllines;
        //    }
        //}

        //private bool ValidateQuantity(string svalue, int packQty)
        //{
        //    bool bisValid;
        //    int itemqtyinput;

        //    //if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput) && itemqtyinput >= packQty)
        //    if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput))
        //    {
        //        if (itemqtyinput % packQty != 0)
        //        {
        //            ItemQuantityForm itemqtyform = new ItemQuantityForm(itemqtyinput, packQty, ref _poline);

        //            itemqtyform.OnQuantityRounded += new ItemQuantityForm.QuantityRoundedEventHandler(itemqtyform_OnQuantityRounded);

        //            if (itemqtyform.ShowDialog(this) == DialogResult.OK)
        //            {
        //                bisValid = true;
        //            }
        //        }

        //        bisValid = true;
        //    }
        //    else
        //    {
        //        bisValid = false;
        //    }

        //    return bisValid;
        //}

        //void itemqtyform_OnQuantityRounded(object sender, int iroundedquantity)
        //{
        //    _itemquantityrounded = iroundedquantity;
        //}

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx"); 
        }
    }
}