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
        private PurchaseOrder _porder;
                
        private int _rowindex;
        
        public POLineDetailsPack(POBO.POItemDetails poline, PurchaseOrder porder, int rowindex)
        {
            _poline   = poline;
            _porder   = porder;
            _rowindex = rowindex;

            InitializeComponent();
        }

        private void POLineDetailsPack_Load(object sender, EventArgs e)
        {
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
                    dgvPackDetails["ComponentClass", i].Value    = component.ComponentClass;
                    dgvPackDetails["ComponentVendor", i].Value   = component.ComponentVendor;
                    dgvPackDetails["ComponentStyle", i].Value    = component.ComponentStyle;
                    dgvPackDetails["ComponentColour", i].Value   = component.ComponentColour;
                    dgvPackDetails["ComponentSize", i].Value     = component.ComponentSize;
                    dgvPackDetails["ComponentLongDesc", i].Value = component.ItemDescription;
                    dgvPackDetails["RatioQuantity", i].Value     = component.RatioQuantity;
                    dgvPackDetails["ComponentCost", i].Value     = component.Cost;
                    dgvPackDetails["SavedCost", i].Value         = component.Cost;
                    dgvPackDetails["Retail", i].Value            = component.Retail;
                    dgvPackDetails["ItemQuantity", i].Value      = component.RatioQuantity * OrderQuantity;
                }
            }
        }

        
        private void ResetCostValue()
        {
            foreach (DataGridViewRow dgvRow in dgvPackDetails.Rows)
            {
                foreach (APPcomponent component in _poline.Components)
                {
                    if (component.ComponentClass  == Convert.ToInt16(dgvRow.Cells["ComponentClass"].Value)  &&
                        component.ComponentVendor == Convert.ToInt32(dgvRow.Cells["ComponentVendor"].Value) &&
                        component.ComponentStyle  == Convert.ToInt16(dgvRow.Cells["ComponentStyle"].Value)  &&
                        component.ComponentColour == Convert.ToInt16(dgvRow.Cells["ComponentColour"].Value) &&
                        component.ComponentSize   == Convert.ToInt16(dgvRow.Cells["ComponentSize"].Value))
                    {
                        component.Cost = Convert.ToDecimal(dgvRow.Cells["SavedCost"].Value);
                    }
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Decimal totalcost = 0;
            foreach (APPcomponent component in _poline.Components)
            {
                totalcost += component.RatioQuantity * component.Cost;
            }

            _poline.ConvertedCost = Decimal.Round(totalcost, 2);
            
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
            ResetCostValue();
           this.DialogResult = DialogResult.Cancel;
           this.Close();
        }

        

        private void dgvPackDetails_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
        //    if (e.ColumnIndex == -1 || e.RowIndex == -1) return;

            
        //    _polinedetails = _porder.lstpoLineItemDetails[_rowindex];

        //    Cursor.Current = Cursors.WaitCursor;

        //    polineform = new POLineDetails(_porder, ref _polinedetails, true);
        //    polineform.ShowDialog(this);

        //    //dgvPackDetails["Quantity", e.RowIndex].Value = _polinedetails.Itemquantity;
        //    //dgvPackDetails["ConvertedCost", e.RowIndex].Value = _polinedetails.ConvertedCost;

        //    //_polinedetails.Cost = _polinedetails.ConvertedCost * _porder.ExchangeRate;
        //    //dgvPackDetails["Cost", e.RowIndex].Value = _polinedetails.Cost;

        //    //_polinedetails.LandedCost = _polinedetails.Cost * _porder.Landing;
        //    //dgvPackDetails["LandedCost", e.RowIndex].Value = _polinedetails.LandedCost;

        //    //CalculatePOSummary();

        //    polineform = null;
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

                if (componentcost != Decimal.Round(componentcost, 2))
                {
                    dgvPackDetails.Rows[e.RowIndex].ErrorText = "The cost value cannot have more than 2 decimal places";
                    e.Cancel = true;
                    return;
                }

                if (componentcost.Equals(0) || componentcost < 0)
                {
                    dgvPackDetails.Rows[e.RowIndex].ErrorText = "The cost value cannot be negative or zero";
                    e.Cancel = true;
                    return;
                }

                _poline.Components[Convert.ToInt32(dgvPackDetails["RowNumber",e.RowIndex].Value)].Cost             = componentcost;
                _poline.Components[Convert.ToInt32(dgvPackDetails["RowNumber", e.RowIndex].Value)].UnConvertedCost = componentcost * _porder.ExchangeRate;
            }
        }
                
        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx"); 
        }

    }
}