using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Disney.Spice.POBO;

namespace Disney.Spice.POUI
{
    public partial class POLineDetailsPack : Form
    {
        private POItemDetails _poline;
        private SPICECommon _commonparam;
        private POLineForm polineform;
        private PurchaseOrder _porder;

        public POLineDetailsPack(POBO.POItemDetails poline, PurchaseOrder porder,SPICECommon commonparam)
        {
            _poline = poline;
            _commonparam = commonparam;
            _porder = porder;

            InitializeComponent();
        }

        private void POLineDetailsPack_Load(object sender, EventArgs e)
        {
           //Populate the grid based on the Item

            // HK : 04-10-2009
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

           // HHK : 04-10-2009 : Bit of tidying up
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

            dtgrdVwPOLineDetailPack.Columns["ComponentClass"].HeaderText = "Class";
            dtgrdVwPOLineDetailPack.Columns["ComponentVendor"].HeaderText = "Vendor";
            dtgrdVwPOLineDetailPack.Columns["ComponentStyle"].HeaderText = "Style";
            dtgrdVwPOLineDetailPack.Columns["ComponentColour"].HeaderText = "Colour";
            dtgrdVwPOLineDetailPack.Columns["ComponentSize"].HeaderText = "Size";

            // HK : 04-10-2009
            // Add missing columns from Functional Specification
            //dtgrdVwPOLineDetailPack.Columns["ComponentDescription"].HeaderText = "Description";
            dtgrdVwPOLineDetailPack.Columns["ComponentQuantity"].HeaderText = "Nominal Quantity";
            dtgrdVwPOLineDetailPack.Columns["OrderQuantity"].HeaderText = "Order Quantity";
            dtgrdVwPOLineDetailPack.Columns["ComponentCost"].HeaderText = "Cost";
            dtgrdVwPOLineDetailPack.Columns["Retail"].HeaderText = "Retail";

            // HK : 04-10-2009
            // Get the description of the component
            /*
            if (_polinedetails.ItemLookup(_porder.DbParamRef, _porder.UserName, _porder.Penvironment, _porder.DefaultMarket))
            {
                //dtgrdPOLinesView.Rows[e.RowIndex].Cells["Description"].Value = _polinedetails.Itemlongdescription;
            }
             * */
            

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //Save the values to the datatable and close
            this.Close();
        }

        private void txtQuantity_Validating(object sender, CancelEventArgs e)
        {
            Int16 iqty;
            if (Int16.TryParse(txtQuantity.Text, out iqty))
            {

                foreach (DataGridViewRow dr in dtgrdVwPOLineDetailPack.Rows)
                {
                    //Total the qtys
                     
                                       
                                        
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dtgrdVwPOLineDetailPack_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dtgrdVwPOLineDetailPack_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (polineform == null)
            {
                POItemDetails pocomponent = new POItemDetails(e.RowIndex);

                pocomponent.Classcode = (Int16)dtgrdVwPOLineDetailPack["ComponentClass", e.RowIndex].Value;
                pocomponent.Colorcode = (Int16)dtgrdVwPOLineDetailPack["ComponentColour", e.RowIndex].Value;
                pocomponent.Stylecode = (Int16)dtgrdVwPOLineDetailPack["ComponentStyle", e.RowIndex].Value;
                pocomponent.Vendorcode = (Int32)dtgrdVwPOLineDetailPack["ComponentVendor", e.RowIndex].Value;
                pocomponent.Itemsize = (Int16)dtgrdVwPOLineDetailPack["ComponentSize", e.RowIndex].Value;

                //This function assumes the reqd fields have populated
                pocomponent.ItemLookup(_commonparam.Dbparamref, _commonparam.Username, _commonparam.Penvironment, _commonparam.DefaultMarket);


                polineform = new POLineForm(_porder, ref pocomponent, true);
                polineform.Show(this);
                polineform = null;
            }
        }

        private void grpBxPackInfo_Enter(object sender, EventArgs e)
        {

        }

         
    }
}