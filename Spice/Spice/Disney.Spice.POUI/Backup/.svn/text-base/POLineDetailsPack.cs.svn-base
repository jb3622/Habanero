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
        public delegate void    AppQuantityChangedEventHandler(object sender, AppDetailsEventArgs e);
        public event            AppQuantityChangedEventHandler OnAppQuantityChanged;


        private POItemDetails   _poline;
        private SPICECommon     _commonparam;
        private POLineForm      polineform;
        private PurchaseOrder   _porder;

        private int             _rowindex;
        private decimal         _costalllines = 0;

        // //////////////////////////////////////////////////////////
        // HHK : 11-11-2009
        // Capture the Quantity rounded UP or DOWN by the user
        // When 0 then the user pressed 'Cancel' button
        private int             _itemquantityrounded = 0;

        // //////////////////////////////////////////////////////////

        // ////////////////////////////////////////////////////////////
        // HK : 22-12-2009 : Some attributes of the PO item are populated 
        // by the validation classes and not ItemLookup
        private Validation validationcls;

        // ////////////////////////////////////////////////////////////

        private decimal _currencyratemarket;
        private decimal _currencyratepo;

        public POLineDetailsPack(POBO.POItemDetails poline, PurchaseOrder porder,SPICECommon commonparam)
        {
            _poline             = poline;
            _commonparam        = commonparam;
            _porder             = porder;

            InitializeComponent();
        }


        // /////////////////////////////////////////////////////////////
        // HK : 13-11-2009 : Overloaded constructor. I have kept ASH's 
        // original one as it may have been used from different places
        // /////////////////////////////////////////////////////////////
        public POLineDetailsPack(POBO.POItemDetails poline, PurchaseOrder porder, SPICECommon commonparam, int rowindex)
        {
            _poline         = poline;
            _commonparam    = commonparam;
            _porder         = porder;
            _rowindex       = rowindex;

            validationcls = new Validation(_porder.DbParamRef, _porder.UserName, _porder.Penvironment);

            InitializeComponent();
        }
        // /////////////////////////////////////////////////////////////

        private void POLineDetailsPack_Load(object sender, EventArgs e)
        {

            // HK : 18-01-2010 : Fix Bug 233
            _currencyratemarket = validationcls.GetCurrency(_porder.MarketCurrency);
            _currencyratepo     = _porder.ExchangeRate;
           
            //Populate the grid based on the Item

            // HK : 04-11-2009
            // Populate the SHORT DESC (ShtDesc) and VENDOR REFERENCE (VenStyle)
            string sShtDesc  = _poline.Itemshortdescription;
            string sVenStyle = _poline.Vendorstyle;

            StringBuilder sblongitemdesc = new StringBuilder("");

            sblongitemdesc.Append(_poline.Classcode.ToString().PadLeft(4, '0'));
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
           //lblVendorValue.Text = _poline.Vendorcode + "-" + _poline.Vendordesc;

           // HHK : 04-11-2009 : Bit of tidying up
           lblVendorValue.Text = _poline.Vendorcode + " - " + _poline.Vendordesc;

           lblDescriptionValue.Text = _poline.Itemlongdescription;
           
           // // HK : 04-10-2009
           // Append the SHORT DESC (ShtDesc) and VENDOR REFERENCE (VenStyle)
           lblDescriptionValue1.Text = sShtDesc;
           lblVendorRefValue.Text = sVenStyle;
           
           txtQuantity.Text = _poline.Itemquantity.ToString();

           PopulateGrid(_poline.Itemquantity);

        }

        private void PopulateGrid(int qty)
        {

            AssortedPrePack appbo = new AssortedPrePack(_poline, _commonparam);
            DataTable dtInitialLoad = appbo.PopulateAPPStructure(qty);

            dtgrdVwPOLineDetailPack.DataSource = dtInitialLoad;

            dtgrdVwPOLineDetailPack.Columns["ComponentClass"]       .HeaderText       = "Class";
            dtgrdVwPOLineDetailPack.Columns["ComponentVendor"]      .HeaderText       = "Vendor";
            dtgrdVwPOLineDetailPack.Columns["ComponentStyle"]       .HeaderText       = "Style";
            dtgrdVwPOLineDetailPack.Columns["ComponentColour"]      .HeaderText       = "Colour";
            dtgrdVwPOLineDetailPack.Columns["ComponentSize"]        .HeaderText       = "Size";

            // HK : 04-10-2009
            // Add missing columns from Functional Specification
            dtgrdVwPOLineDetailPack.Columns["ComponentLongDesc"]    .HeaderText     = "Description";
            dtgrdVwPOLineDetailPack.Columns["ComponentQuantity"]    .HeaderText     = "Nominal Quantity";
            dtgrdVwPOLineDetailPack.Columns["OrderQuantity"]        .HeaderText     = "Order Quantity";
            dtgrdVwPOLineDetailPack.Columns["ComponentCost"]        .HeaderText     = "Vendor Cost";
            dtgrdVwPOLineDetailPack.Columns["Retail"]               .HeaderText     = "Retail";

            // HK : 19-01-2010
            // ConvertedCost, LandedCost and Simple Vendor Cost stuff
            dtgrdVwPOLineDetailPack.Columns["LandedCost"]           .HeaderText     = "Landed Cost";
            dtgrdVwPOLineDetailPack.Columns["ConvertedCost"]        .HeaderText     = "Cost";

            // Size the columns
            dtgrdVwPOLineDetailPack.Columns["ComponentClass"]       .Width = 50;
            dtgrdVwPOLineDetailPack.Columns["ComponentVendor"]      .Width = 50;
            dtgrdVwPOLineDetailPack.Columns["ComponentStyle"]       .Width = 50;
            dtgrdVwPOLineDetailPack.Columns["ComponentColour"]      .Width = 40;
            dtgrdVwPOLineDetailPack.Columns["ComponentSize"]        .Width = 50;
            dtgrdVwPOLineDetailPack.Columns["ComponentLongDesc"]    .Width = 160;
            dtgrdVwPOLineDetailPack.Columns["OrderQuantity"]        .Width = 50;
            dtgrdVwPOLineDetailPack.Columns["ComponentCost"]        .Width = 50;
            dtgrdVwPOLineDetailPack.Columns["Retail"]               .Width = 50;

            // HK : 19-01-2010
            // ConvertedCost, LandedCost and Simple Vendor Cost stuff
            dtgrdVwPOLineDetailPack.Columns["LandedCost"]           .Width = 50;
            dtgrdVwPOLineDetailPack.Columns["ConvertedCost"]        .Width = 50;

            // HHK : 13-11-2009
            // Enable Disable columns
            dtgrdVwPOLineDetailPack.Columns["ComponentLongDesc"]    .ReadOnly   = true;
            dtgrdVwPOLineDetailPack.Columns["ComponentQuantity"]    .ReadOnly   = true;
            dtgrdVwPOLineDetailPack.Columns["OrderQuantity"]        .ReadOnly   = true;
            dtgrdVwPOLineDetailPack.Columns["Retail"]               .ReadOnly   = true;

            // HHK : 13-11-2009 : Only cost is editable
            // HK : 19-01-2010 : No they are not editable. So make them readonly
            dtgrdVwPOLineDetailPack.Columns["ComponentCost"].       ReadOnly    = true;

            // HK : 19-01-2010
            // ConvertedCost, LandedCost and Simple Vendor Cost stuff
            dtgrdVwPOLineDetailPack.Columns["LandedCost"].          ReadOnly    = true;
            dtgrdVwPOLineDetailPack.Columns["ConvertedCost"].       ReadOnly    = true;

            // HK : 19-01-2010
            // ConvertedCost, LandedCost and Simple Vendor Cost stuff
            dtgrdVwPOLineDetailPack.Columns["LandedCost"].          Visible = false;
            dtgrdVwPOLineDetailPack.Columns["ConvertedCost"].       Visible = true;
            dtgrdVwPOLineDetailPack.Columns["ComponentCost"].       Visible = false;

            // HK : 19-01-2010
            // ConvertedCost, LandedCost and Simple Vendor Cost stuff
            // Ordinal
            dtgrdVwPOLineDetailPack.Columns["ComponentClass"].      DisplayIndex = 1;
            dtgrdVwPOLineDetailPack.Columns["ComponentVendor"].     DisplayIndex = 2;
            dtgrdVwPOLineDetailPack.Columns["ComponentStyle"].      DisplayIndex = 3;
            dtgrdVwPOLineDetailPack.Columns["ComponentColour"].     DisplayIndex = 4;
            dtgrdVwPOLineDetailPack.Columns["ComponentSize"].       DisplayIndex = 5;
            dtgrdVwPOLineDetailPack.Columns["ComponentLongDesc"].   DisplayIndex = 6;
            dtgrdVwPOLineDetailPack.Columns["ComponentQuantity"].   DisplayIndex = 7;
            dtgrdVwPOLineDetailPack.Columns["ConvertedCost"].       DisplayIndex = 8;
            dtgrdVwPOLineDetailPack.Columns["OrderQuantity"].       DisplayIndex = 9;
            dtgrdVwPOLineDetailPack.Columns["Retail"].              DisplayIndex = 10;

            // //////////////////////////////////////////////////////////////////
            // HK : 13-11-2009 : Populate the POComponents object for all components
            // in the APP

            // //////////////////////////////////////////////////////////////////
            if (_poline.pocomponents.Count == 0)
            {
                PopulateComponents(dtInitialLoad);
            }
            else
            {
                // //////////////////////////////////////////////////////////////////
                // HK : 18-11-2009 : We need to display the "ComponentCost" from the 
                // comnponent level PoComponent object

                // //////////////////////////////////////////////////////////////////

                for (int count = 0; count < dtgrdVwPOLineDetailPack.Rows.Count; count++)
                {
                    dtgrdVwPOLineDetailPack.Rows[count].Cells ["ComponentCost"].Value = _poline.pocomponents[count].Cost;

                    // ///////////////////////////////////////////////////////////////
                    // HK : 18-01-2010 : Fix Bug 233
                    dtgrdVwPOLineDetailPack.Rows[count].Cells["ConvertedCost"].Value = _poline.pocomponents[count].ConvertedCost;
                    dtgrdVwPOLineDetailPack.Rows[count].Cells["LandedCost"].Value = _poline.pocomponents[count].LandedCost;

                    // ///////////////////////////////////////////////////////////////
                }

            }

            // hourglass cursor
            Cursor.Current = Cursors.Default;

        }

        // /////////////////////////////////////////////////////////////////////////////
        // HK : 17-11-2009 : Populate the POComponents collection of POItemDetails
        // for each component in the PO Item
        // /////////////////////////////////////////////////////////////////////////////
        private void PopulateComponents(DataTable dtInitialLoad)
        {

            for (int i = 0; i < dtInitialLoad.Rows.Count; i++)
            {

                POItemDetails pocomponent = new POItemDetails(i);
                pocomponent.ItemQtyChanged += new POItemDetails.delItemQtyChanged(pocomponent_ItemQtyChanged);

                pocomponent.Classcode       = (short)dtInitialLoad.Rows[i]["ComponentClass"];
                pocomponent.Colorcode       = (short)dtInitialLoad.Rows[i]["ComponentColour"];
                pocomponent.Stylecode       = (short)dtInitialLoad.Rows[i]["ComponentStyle"];
                pocomponent.Vendorcode      = (int)dtInitialLoad.Rows[i]  ["ComponentVendor"];
                pocomponent.Itemsize        = (short)dtInitialLoad.Rows[i]["ComponentSize"];

                // ////////////////////////////////////////////////////////////
                // HK : 22-12-2009 : Some attributes of the PO item are populated 
                // by the validation classes and not ItemLookup

                // ///////////////////////////////////////////////////////////////
                // ClassName
                List<string> retValues = validationcls.ValidateClass(pocomponent.Classcode.ToString());
                pocomponent.Classname = retValues[1].ToString();

                // Vendor
                retValues = validationcls.ValidateVendor(pocomponent.Vendorcode.ToString(), true);
                pocomponent.Vendordesc = retValues[1].ToString();

                // Color
                retValues = validationcls.ValidateColour(pocomponent.Colorcode.ToString());
                pocomponent.Colordesc = retValues[1];

                // Size
                retValues = validationcls.ValidateSize(pocomponent.Itemsize.ToString ());
                pocomponent.Sizename = retValues[1];

                // ///////////////////////////////////////////////////////////////

                //This function assumes the reqd fields have populated
                pocomponent.ItemLookup(_commonparam.Dbparamref, _commonparam.Username, _commonparam.Penvironment, _commonparam.DefaultMarket);

                // Apply the ItemQuantity to the PO Component ( PO Item Details) business object 
                pocomponent.Itemquantity    = (short)dtInitialLoad.Rows[i]    ["ComponentQuantity"];
                pocomponent.Cost            = (decimal)dtInitialLoad.Rows[i]  ["ComponentCost"];

                // ///////////////////////////////////////////////////////////////
                // HK : 18-01-2010 : Fix Bug 233
                // Apply Converted Cost and Landed Cost
                // ///////////////////////////////////////////////////////////////
                // HK : 14-01-2010 : Fix Bug 233
                pocomponent.LandedCost = Decimal.Round((pocomponent.Cost * _currencyratemarket) * _porder.Landing, 2);
                dtInitialLoad.Rows[i]["LandedCost"] = pocomponent.LandedCost;

                if (_currencyratepo != 0)
                {
                    // HK : 14-01-2010 : Fix Bug 233
                    pocomponent.ConvertedCost = Decimal.Round((pocomponent.Cost * _currencyratemarket) / _currencyratepo, 2);
                    dtInitialLoad.Rows[i]["ConvertedCost"] = pocomponent.ConvertedCost;

                }
                else
                {
                    Debug.Print("Add Item : While calculating converted cost, it was found that the PO Currency Rate is 0");
                }

                // ///////////////////////////////////////////////////////////////

                // Since we are loopin through the datatable in sequential order starting from 0
                // we just use the Add method instead of InsertAt
                _poline.pocomponents.Add(pocomponent);

            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            AppDetailsEventArgs e1 = new AppDetailsEventArgs (_poline, _porder, _rowindex, _poline.Itemquantity);
            this.DialogResult = DialogResult.OK;
            RaiseAppQuantityChangedEvent(e1);

            //Save the values to the datatable and close
            this.Close();
        }

        private void txtQuantity_Validating(object sender, CancelEventArgs e)
        {
            Int16 iqty;
            Int32 icomponentqty;

            Debug.Print("EVENT Validating fired");

            if (Int16.TryParse(txtQuantity.Text, out iqty))
            {

                if (ValidateQuantity(iqty.ToString(), _poline.Casepackqty))
                {

                    // ///////////////////////////////////////////////////////////////////
                    // HK : 11-11-2009 : If no rounding was done then continue validating 
                    // as usual
                    if (_itemquantityrounded == 0)
                    {
                        //_poline.Itemquantity = int.Parse(e.FormattedValue.ToString());
                        _poline.Itemquantity = iqty;

                        // /////////////////////////////////////////////////////////////////
                        // HH: 05-11-2009 : Display the suggested quantity in the datagrid
                        // /////////////////////////////////////////////////////////////////

                        //dtgrdPOLinesView.Rows[e.RowIndex].Cells["Quantity"].Value = _poline.Itemquantity;

                        // /////////////////////////////////////////////////////////////////
                    }

                    // If rounding was done then capture the value so that the "CellValidated" EVENT 
                    // can display the rounded value
                    if (_itemquantityrounded > 0)
                    {
                        _poline.Itemquantity = _itemquantityrounded;

                        // Set the value back to zero for reuse
                        _itemquantityrounded = 0;

                    }

                }
                
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            this.Close();
        }

        private void dtgrdVwPOLineDetailPack_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dtgrdVwPOLineDetailPack_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (polineform == null)
            {

                // hourglass cursor
                Cursor.Current = Cursors.WaitCursor;

                //polineform = new POLineForm(_porder, ref pocomponent, true, e.RowIndex);
                POItemDetails potemp = new POItemDetails(e.RowIndex);
                potemp = _poline.pocomponents[e.RowIndex];
                polineform = new POLineForm(_porder, ref potemp, true, e.RowIndex);

                // Quantity or Cost changed Event Handler (if cost or quantity changed in the POLine Form)
                polineform.OnAppQuantityOrCostChanged += new POLineForm.ComponentQuantityOrCostChangedEventHandler(polineform_OnAppQuantityOrCostChanged);

                polineform.Show(this);
                polineform = null;
            }
        }

        void pocomponent_ItemQtyChanged(int qty, decimal cost, int rowindex)
        {
            {
                // //////////////////////////////////////////////////////////////
                // HK : 14-11-2009 : Dummy event so that ASH's stuff does not 
                // fall over

            }
            
        }

        
        void polineform_OnAppQuantityOrCostChanged(object sender, POLineForm.AppDetailsEventArgs e)
        {

            // Capture the business objects and ressign them to the instance variables in this form
            // ?? To Do on monsay after asking FC and Clayton
            // below _poline should be pocomponent object and not the actual Po Line Item object
            //_poline.Itemquantity = e.poline.Itemquantity;
            //_poline.Cost = e.poline.Cost;
            //_poline.pocomponents[e.rowindex].Itemquantity = e.poline.Itemquantity;

            // HK : 18-01-2010 : Fix Bug 233. "ConvertedCost" is ammendable and not "Cost" i.e. Simple Vendor Cost
            //_poline.pocomponents[e.rowindex].Cost = e.poline.Cost;
            _poline.pocomponents[e.rowindex].ConvertedCost = e.poline.ConvertedCost;

            // /////////////////////////////////////////////////////////////
            // HK : 13-11-2009
            // "ComponentQuantity" is readonly
            // Display the changes in the grid
            //dtgrdVwPOLineDetailPack["ComponentQuantity", e.rowindex].Value = e.poline.Itemquantity;

            // HK : 18-01-2010 : Fix Bug 233. "ConvertedCost" is ammendable and not "Cost" i.e. Simple Vendor Cost
            //dtgrdVwPOLineDetailPack["ComponentCost", e.rowindex].Value = e.poline.Cost;
            dtgrdVwPOLineDetailPack["ConvertedCost", e.rowindex].Value = e.poline.ConvertedCost;

        }

        private void AddComponentToList(int rowindex, POItemDetails pocomponent)
        {
            POItemDetails poc;

            poc = new POItemDetails(pocomponent.Itemindex);
            poc = pocomponent;

            //_poline.pocomponents.Add(poc);
            //_poline.pocomponents[rowindex] = poc;
            //if (_poline.pocomponents.IndexOf(poc) >= 0)
            //if (_poline.pocomponents.Count > 0 && _poline.pocomponents[rowindex] != null && rowindex < _poline.pocomponents.Count)
            //if (_poline.pocomponents.IndexOf(poc) == -1)

            if (_poline.lstComponentDetails.IndexOf (poc) == -1)
            {
                _poline.lstComponentDetails.Insert(rowindex, poc);
                
            }
            else
            {
                _poline.lstComponentDetails[rowindex] = poc;
               // _poline.pocomponents[rowindex] = poc;
            }

        }

        private void grpBxPackInfo_Enter(object sender, EventArgs e)
        {

        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            // HHK : 13-11-2009
            //do the line level summary here         
            /*

            _porderitemline.Itemquantity = Int32.Parse(txtOrderQty.Text);
            _porderitemline.Cost = Decimal.Parse(txtUnitCost.Text);

            _totalcost = _porderitemline.Cost * _porderitemline.Itemquantity;
            _totallandedcost = _totalcost * _porderdisplay.Landing;
            _totalretail = _porderitemline.Retailprice * _porderitemline.Itemquantity;
            _totalmargin = _totalretail - _totalcost;


            lblTotalCostValue.Text = _totallandedcost.ToString();
            lblRetailValue.Text = _totalretail.ToString();
            lblMarginValue.Text = _totalmargin.ToString();
             */

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

        private void txtQuantity_Validated(object sender, EventArgs e)
        {
            Debug.Print("EVENT Validated fired");

            Int32 iqty = 0;
            Int32 icomponentqty;

            iqty = _poline.Itemquantity;

            txtQuantity.Text = iqty.ToString();

            // HK : Observation : 25-01-2010 : Looks like the below code is not executing.
            // Recalculate the ComponentQuantity
            foreach (DataGridViewRow dr in dtgrdVwPOLineDetailPack.Rows)
            {
                if (dr.Index < dtgrdVwPOLineDetailPack.Rows.Count - 1)
                {
                    //Total the qtys
                    icomponentqty = Convert.ToInt32(dr.Cells["ComponentQuantity"].Value);
                    dtgrdVwPOLineDetailPack["OrderQuantity", dr.Index].Value = iqty * icomponentqty;
                }
            }

            //_poline.Itemquantity = Int32.Parse(txtQuantity.Text);
        }

        private void dtgrdVwPOLineDetailPack_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

            try
            {
                if (dtgrdVwPOLineDetailPack.Columns[e.ColumnIndex].Name.Equals("ComponentCost"))
                {
                    decimal _cost;
                    if (!String.IsNullOrEmpty(e.FormattedValue.ToString()) || Decimal.TryParse(e.FormattedValue.ToString(), out _cost))
                    {

                        _poline.Cost = Decimal.Parse(e.FormattedValue.ToString());
                        //CalculatePOSummary();
                    }
                    else
                    {
                        dtgrdVwPOLineDetailPack["Cost", e.RowIndex].Value = 0;
                    }
                }

            }

            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, ex.Source);

            }

            
        }

        private void dtgrdVwPOLineDetailPack_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            int     nominalquantity;
            decimal cost;

            _costalllines = 0;

            if (dtgrdVwPOLineDetailPack.Columns[e.ColumnIndex].Name.Equals("ComponentCost"))
            {

                foreach (DataGridViewRow dr in dtgrdVwPOLineDetailPack.Rows)
                {
                    if (dr.Index < dtgrdVwPOLineDetailPack.Rows.Count - 1)
                    {
                        //Total the cost across all component lines
                        //nominalquantity = (int)dtgrdVwPOLineDetailPack ["ComponentQuantity", dr.Index].Value;
                        nominalquantity = Convert.ToInt32(dr.Cells["ComponentQuantity"].Value);
                        cost = (decimal)dtgrdVwPOLineDetailPack["ComponentCost", dr.Index].Value;
                        _costalllines = _costalllines + (nominalquantity * cost);

                    }
                }

                _poline.Cost = _costalllines;

            }
        }

        private bool ValidateQuantity(string svalue, int packQty)
        {
            bool bisValid;
            int itemqtyinput;

            //if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput) && itemqtyinput >= packQty)
            if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput))
            {

                if (itemqtyinput % packQty != 0)
                {
                    ItemQuantityForm itemqtyform = new ItemQuantityForm(itemqtyinput, packQty, ref _poline);

                    itemqtyform.OnQuantityRounded += new ItemQuantityForm.QuantityRoundedEventHandler(itemqtyform_OnQuantityRounded);

                    if (itemqtyform.ShowDialog(this) == DialogResult.OK)
                    {
                        bisValid = true;
                    }
                }

                bisValid = true;
            }
            else
            {
                bisValid = false;

            }

            return bisValid;

        }

        void itemqtyform_OnQuantityRounded(object sender, int iroundedquantity)
        {
            _itemquantityrounded = iroundedquantity;
           
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx"); 
        }

        private void UpdateComponentQuantity(int quantity)
        {
            for (int i = 0; i < _poline.pocomponents.Count; i++ )
            {
                _poline.pocomponents[i].Itemquantity = quantity;
                dtgrdVwPOLineDetailPack.Rows[i].Cells["ComponentQuantity"].Value = quantity;
            }
        }

    }
}