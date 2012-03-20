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
    public partial class POLineDetailsPack : Form
    {
        public delegate void    AppQuantityChangedEventHandler(object sender, AppDetailsEventArgs e);
        public event            AppQuantityChangedEventHandler OnAppQuantityChanged;


        private POItemDetails   _poline;
        //private SPICECommon     _commonparam;
        private POLineForm      polineform;
        private PurchaseOrder   _porder;

        private int             _rowindex;
        private decimal         _costalllines = 0;

        // HHK : 11-11-2009
        // Capture the Quantity rounded UP or DOWN by the user
        // When 0 then the user pressed 'Cancel' button
        private int             _itemquantityrounded = 0;


        // HK : 22-12-2009 : Some attributes of the PO item are populated 
        // by the validation classes and not ItemLookup
        private Validation validationcls;

        private decimal _currencyratemarket;
        private decimal _currencyratepo;

        public POLineDetailsPack(POBO.POItemDetails poline, PurchaseOrder porder)//,SPICECommon commonparam)
        {
            _poline             = poline;
            //_commonparam        = commonparam;
            _porder             = porder;

            InitializeComponent();
        }


        // HK : 13-11-2009 : Overloaded constructor. I have kept ASH's 
        // original one as it may have been used from different places
        public POLineDetailsPack(POBO.POItemDetails poline, PurchaseOrder porder, int rowindex)
        {
            _poline         = poline;
            //_commonparam    = commonparam;
            _porder         = porder;
            _rowindex       = rowindex;

            validationcls = new Validation(_porder.DbParamRef, _porder.UserName, _porder.Penvironment);

            InitializeComponent();
        }

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
            //lblVendorValue.Text = _poline.Vendorcode + "-" + _poline.Vendordesc;

            lblVendorValue.Text = _poline.Vendorcode + " - " + _poline.Vendordesc;

            lblDescriptionValue.Text = _poline.Itemlongdescription;

            // Append the SHORT DESC (ShtDesc) and VENDOR REFERENCE (VenStyle)
            lblDescriptionValue1.Text = sShtDesc;
            lblVendorRefValue.Text = sVenStyle;

            txtQuantity.Text = _poline.Itemquantity.ToString();

            PopulateGrid(_poline.Itemquantity);
        }

        private void PopulateGrid(int qty)
        {
            AssortedPrePack appbo = new AssortedPrePack(_poline,_porder);//, _commonparam);
            DataTable dtInitialLoad = appbo.PopulateAPPStructure();

            dgvPackDetails.DataSource = dtInitialLoad;

            if (_poline.Components.Count == 0)
            {
                PopulateComponents(dtInitialLoad);
            }
            else
            {
                for (int count = 0; count < dgvPackDetails.Rows.Count; count++)
                {
                    dgvPackDetails.Rows[count].Cells["ComponentCost"].Value = _poline.Components[count].Cost
                    dgvPackDetails.Rows[count].Cells["ConvertedCost"].Value = _poline.Components[count]._poline.pocomponents[count].ConvertedCost;
                    //dtgrdVwPOLineDetailPack.Rows[count].Cells["LandedCost"].Value = _poline.pocomponents[count].LandedCost;
                }
            }

            // hourglass cursor
            Cursor.Current = Cursors.Default;
        }

        private void PopulateComponents(DataTable dtInitialLoad)
        {
            for (int i = 0; i < dtInitialLoad.Rows.Count; i++)
            {
                APPcomponent component = new APPcomponent();
                //pocomponent.ItemQtyChanged += new POItemDetails.delItemQtyChanged(pocomponent_ItemQtyChanged);

                component.ComponentClass  = (short)dtInitialLoad.Rows[i]["ComponentClass"];
                component.ComponentVendor = (int)  dtInitialLoad.Rows[i]["ComponentVendor"];
                component.ComponentStyle  = (short)dtInitialLoad.Rows[i]["ComponentStyle"];
                component.ComponentColour = (short)dtInitialLoad.Rows[i]["ComponentColour"];
                component.ComponentSize   = (short)dtInitialLoad.Rows[i]["ComponentSize"];

                // ClassName
                //List<string> retValues = validationcls.ValidateClass(component.ComponentClass.ToString());
                //component.Classname = retValues[1].ToString();

                //// Vendor
                //retValues = validationcls.ValidateVendor(component.Vendorcode.ToString(), true);
                //component.Vendordesc = retValues[1].ToString();

                //// Color
                //retValues = validationcls.ValidateColour(component.Colorcode.ToString());
                //component.Colordesc = retValues[1];

                //// Size
                //retValues = validationcls.ValidateSize(component.Itemsize.ToString ());
                //component.Sizename = retValues[1];

                //This function assumes the reqd fields have populated
                //component.ItemLookup(_porder.Dbparamref, _commonparam.Username, _commonparam.Penvironment, _commonparam.DefaultMarket);

                // Apply the ItemQuantity to the PO Component ( PO Item Details) business object 
                component.RatioQuantity    = (short)dtInitialLoad.Rows[i]  ["ComponentQuantity"];
                component.Cost            = (decimal)dtInitialLoad.Rows[i]["ComponentCost"];

                // Apply Converted Cost and Landed Cost
                component.LandedCost = Decimal.Round((component.Cost * _currencyratemarket) * _porder.Landing, 2);
                dtInitialLoad.Rows[i]["LandedCost"] = component.LandedCost;

                if (_currencyratepo != 0)
                {
                    component.Cost = Decimal.Round((component.Cost * _currencyratemarket) / _currencyratepo, 2);
                    dtInitialLoad.Rows[i]["ConvertedCost"] = component.Cost;
                }

                // Since we are loopin through the datatable in sequential order starting from 0
                // we just use the Add method instead of InsertAt
                _poline.Components.Add(component);

            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //AppDetailsEventArgs e1 = new AppDetailsEventArgs (_poline, _porder, _rowindex, _poline.Itemquantity);
            this.DialogResult = DialogResult.OK;
            //RaiseAppQuantityChangedEvent(e1);

            //Save the values to the datatable and close
            this.Close();
        }

        private void txtQuantity_Validating(object sender, CancelEventArgs e)
        {
            Int16 iqty;
            //Int32 icomponentqty;

            if (Int16.TryParse(txtQuantity.Text, out iqty))
            {
                if (ValidateQuantity(iqty.ToString(), _poline.CasePackQty))
                {
                    // If no rounding was done then continue validating 
                    if (_itemquantityrounded == 0)
                    {
                        //_poline.Itemquantity = int.Parse(e.FormattedValue.ToString());
                        _poline.Itemquantity = iqty;

                        // HH: 05-11-2009 : Display the suggested quantity in the datagrid

                        //dtgrdPOLinesView.Rows[e.RowIndex].Cells["Quantity"].Value = _poline.Itemquantity;
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

        private void dtgrdVwPOLineDetailPack_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (polineform == null)
            {
                // hourglass cursor
                //Cursor.Current = Cursors.WaitCursor;

                //APPcomponent potemp = new APPcomponent(e.RowIndex);
                //potemp = _poline.Components[e.RowIndex];
                //polineform = new POLineForm(_porder, ref potemp, true, e.RowIndex);

                //polineform.OnAppQuantityOrCostChanged += new POLineForm.ComponentQuantityOrCostChangedEventHandler(polineform_OnAppQuantityOrCostChanged);

                //polineform.Show(this);
                //polineform = null;
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
            dgvPackDetails["ConvertedCost", e.rowindex].Value = e.poline.ConvertedCost;

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

            // Recalculate the ComponentQuantity
            foreach (DataGridViewRow dr in dgvPackDetails.Rows)
            {
                if (dr.Index < dgvPackDetails.Rows.Count - 1)
                {
                    //Total the qtys
                    icomponentqty = Convert.ToInt32(dr.Cells["ComponentQuantity"].Value);
                    dgvPackDetails["OrderQuantity", dr.Index].Value = iqty * icomponentqty;
                }
            }

            //_poline.Itemquantity = Int32.Parse(txtQuantity.Text);
        }

        private void dtgrdVwPOLineDetailPack_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

            try
            {
                if (dgvPackDetails.Columns[e.ColumnIndex].Name.Equals("ComponentCost"))
                {
                    decimal _cost;
                    if (!String.IsNullOrEmpty(e.FormattedValue.ToString()) || Decimal.TryParse(e.FormattedValue.ToString(), out _cost))
                    {

                        _poline.Cost = Decimal.Parse(e.FormattedValue.ToString());
                        //CalculatePOSummary();
                    }
                    else
                    {
                        dgvPackDetails["Cost", e.RowIndex].Value = 0;
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

            if (dgvPackDetails.Columns[e.ColumnIndex].Name.Equals("ComponentCost"))
            {

                foreach (DataGridViewRow dr in dgvPackDetails.Rows)
                {
                    if (dr.Index < dgvPackDetails.Rows.Count - 1)
                    {
                        //Total the cost across all component lines
                        //nominalquantity = (int)dtgrdVwPOLineDetailPack ["ComponentQuantity", dr.Index].Value;
                        nominalquantity = Convert.ToInt32(dr.Cells["ComponentQuantity"].Value);
                        cost = (decimal)dgvPackDetails["ComponentCost", dr.Index].Value;
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

            if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput) && itemqtyinput >= packQty)
            {

                /*
                if (itemqtyinput % packQty != 0)
                {
                    ItemQuantityForm itemqtyform = new ItemQuantityForm(itemqtyinput, packQty, ref _poline);

                    itemqtyform.OnQuantityRounded += new ItemQuantityForm.QuantityRoundedEventHandler(itemqtyform_OnQuantityRounded);

                    if (itemqtyform.ShowDialog(this) == DialogResult.OK)
                    {
                        bisValid = true;
                    }
                }
                */
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
         
    }
}