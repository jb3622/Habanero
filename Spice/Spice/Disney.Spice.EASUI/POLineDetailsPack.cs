using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Disney.Spice.EASBO;

namespace Disney.Spice.EASUI
{
    public partial class POLineDetailsPack : Form
    {
        private LineDetails lineDetails;
        private PurchaseOrder purchaseorder;
        private DataTable   componentsTable;

        public POLineDetailsPack(PurchaseOrder purchaseorder, LineDetails lineDetails)
        {
            InitializeComponent();

            this.componentsTable = purchaseorder.GetPOcomponents(purchaseorder.SpicePOnumber, purchaseorder.SpicePOversion, lineDetails.LineSequence);
            this.lineDetails = lineDetails;
            this.purchaseorder = purchaseorder;
        }
        
        private void POLineDetailsPack_Load(object sender, EventArgs e)
        {
            StringBuilder itemnumber = new StringBuilder();

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
            
            lblClassNameValue.Text = lineDetails.ClassName;
            lblVendorValue.Text = lineDetails.Vendor.ToString() + " - " + purchaseorder.VendorName.ToString();
            
            lblDescriptionValue.Text = lineDetails.LongDescription;
            lblDescriptionValue1.Text = lineDetails.ShortDescription;
            lblVendorRefValue.Text = lineDetails.VendorStyle;
            txtQuantity.Text = lineDetails.POOrderQty.ToString();

            if (componentsTable != null && componentsTable.Rows.Count > 0)
            {
                dgvPackDetails.DataSource = componentsTable;
            }
        }
                
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx"); 
        }
    }
}