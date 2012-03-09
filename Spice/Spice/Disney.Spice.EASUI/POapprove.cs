using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using IBM.Data.DB2.iSeries;
using Disney.Spice.EASBO;
using Disney.Spice.LookUp;
using Disney.DA.IP400;
using Disney.Menu;

namespace Disney.Spice.EASUI
{
    public partial class POapprove : Form
    {
        private ASNA.VisualRPG.Runtime.Database PgmDB;
        private iDB2Connection                  DB2connection;
        private Disney.Menu.Users               User;
        private Disney.Menu.Environments        Environment;
        private String                          SpicePOnumber;
        private short                           SpicePOversion;
        private long                            _requestid;
        private Int16                           GridClass;
        private Int32                           GridVendor;
        private Int16                           GridStyle;
        private Int16                           GridColour;
        private Int16                           GridSize;

        
        private PurchaseOrder purchaseorder;

        public POapprove(ASNA.VisualRPG.Runtime.Database PgmDB,
                         iDB2Connection DB2connection,
                         Disney.Menu.Users User,
                         Disney.Menu.Environments Environment,
                         string SpicePOnumber,
                         int functionid,
                         Int64 requestid)
        {
            InitializeComponent();

            this.PgmDB = PgmDB;
            this.DB2connection = DB2connection;
            this.User          = User;
            this.Environment   = Environment;
            this.SpicePOnumber = SpicePOnumber;
            this._requestid    = requestid;

            PurchaseOrder purchaseorder = new PurchaseOrder(DB2connection, Environment);
            bool OrderFound = purchaseorder.GetPurchaseOrder(SpicePOnumber);

            if (OrderFound == false)
            {
                MessageBox.Show("Original order is missing", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                throw new Exception("Original order is missing");
            }

            DataColumn[] key = new DataColumn[] { User.AuthorisedDepartments.Columns["Department"] };
            User.AuthorisedDepartments.PrimaryKey = key;
            DataRow dr = User.AuthorisedDepartments.Rows.Find(purchaseorder.Department);
            if (dr == null)
            {
                MessageBox.Show("You are not authorised to view this PO." +
                        " \n\r\n\r This PO was raised on a department that is not authorised to you.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                throw new Exception("Not authorised to PO");
            }

            this.purchaseorder = purchaseorder;

            SpicePOversion = purchaseorder.SpicePOversion;
            lblMarketValue.Text = purchaseorder.Market + " - " + purchaseorder.MarketName;

            txtDept.Text     = purchaseorder.Department.ToString();
            lblDeptDesc.Text = purchaseorder.DepartmentName;

            txtVendor.Text     = purchaseorder.VendorCode.ToString();
            lblVendorDesc.Text = purchaseorder.VendorName;

            txtCurrency.Text     = purchaseorder.POcurrencyCode;
            lblCurrencyDesc.Text = purchaseorder.POcurrencyDescription;

            
            txtTerms.Text    = purchaseorder.VendorTerms;
            lblTermsDesc.Text = purchaseorder.VendorTermsDescription;

            txtShipTo.Text      = purchaseorder.ShipTo.ToString();
            txtShipVia.Text     = purchaseorder.ShipVia;
            lblShipViaDesc.Text = purchaseorder.ShipViaDescription;

            txtLanding.Text = purchaseorder.LandingFactor.ToString();
            txtPortofDeparture.Text = purchaseorder.PortOfDeparture.ToString();
            lblDeparturePortDesc.Text = purchaseorder.PortOfDepartureName;
            txtPortofEntry.Text = purchaseorder.PortOfEntry.ToString();
            lblEntryPortDesc.Text = purchaseorder.PortOfEntryName;
                       
            //if (Environment.Domain == "SWNA")
            if (DataCache.DisplayFreightCharges == true)
            {
                txtFreight.Visible        = true;
                lblFreightCharges.Visible = true;

                if (purchaseorder.FreightChargeCode == "P")
                {
                    txtFreight.Text = "Pre pay";
                }
                else
                {
                    txtFreight.Text = "Collect";
                }
            }
            else
            {
                txtFreight.Visible = false;
                lblFreightCharges.Visible = false;
            }

            String DateFmt = (Environment.DateFormat == "DMY") ? "d MMMM yyyy" : "MMMM d,  yyyy";

            txtAnticipateDate.Text = purchaseorder.AnticipateDate.ToString(DateFmt);
            txtOrderDate.Text      = purchaseorder.OrderDate.ToString(DateFmt);
            txtCancelDate.Text     = purchaseorder.CancelDate.ToString(DateFmt);
            txtShipDate.Text       = purchaseorder.ShipDate.ToString(DateFmt);

            if (Environment.DateFormat == "DMY")
            {
                txtStageSetDate.Visible = false;
                lblSSD.Visible = false;
            }
            else
            {
                if (purchaseorder.StageSetDate.ToString(DateFmt) != "January 1,  0001")
                {
                    txtStageSetDate.Text = purchaseorder.StageSetDate.ToString(DateFmt);
                }
                    txtStageSetDate.Visible = true;
                    lblSSD.Visible = true;
            }

            
            txtDelTerms.Text          = purchaseorder.DeliveryTerms;
            lblDeliveryTermsDesc.Text = purchaseorder.DeliveryTermsDescription;

            txtVendorComment1.Text    = purchaseorder.Comment1;
            txtVendorComment2.Text    = purchaseorder.Comment2;
            txtVendorComment3.Text    = purchaseorder.Comment3;
            txtInternalComments1.Text = purchaseorder.Comment4;
            txtInternalComments2.Text = purchaseorder.Comment5;

            //PO Summary Group Box
            txtPOLines.Text = Convert.ToString(purchaseorder.POTotalLines);
            txtPOPacks.Text = Convert.ToString(purchaseorder.POTotalPacks);
            lblPONumber.Text = purchaseorder.SpicePOnumber;
            txtTotalUnits.Text = Convert.ToString(purchaseorder.POTotalUnits);
            lblIPPoNumber.Text = purchaseorder.IPPOnumber;

            //Don't display the IP label if IPPOnumber = blank
            if (purchaseorder.IPPOnumber == "     ")
            {
                lblIP.Visible = false;
            }

            //decimal TotalCostcalc = purchaseorder.POTotalCost * purchaseorder.CurrencyRate;
            decimal TotalCostcalc = purchaseorder.POTotalCost;
            txtTotalCost.Text = Convert.ToString(Decimal.Round(TotalCostcalc,2));
            txtTotalRetail.Text = Convert.ToString(Decimal.Round(purchaseorder.POTotalRetail,2));
            txtMarginValue.Text = Convert.ToString(Decimal.Round(purchaseorder.POMarginValue,2));
            txtMarginPercent.Text = Convert.ToString(Decimal.Round(purchaseorder.POMarginPercent,2));
            lblCurrVal1.Text = "(" + purchaseorder.MarketCurrency + ")";
            lblCurrValue.Text = "(" + purchaseorder.MarketCurrency + ")";

            //Set the new line checkbox
            if (purchaseorder.hasNewLine)
            {
                chkNewLineSelection.Checked = true;
            }


            //Set the PK Flag
            foreach (DataRow drPoLines in purchaseorder.POlines.Rows)
            {
                if ((string)drPoLines["IIAAPP"] == "APP")
                {
                    drPoLines["IIAAPP"] = "Y";
                }
                else
                {
                    drPoLines["IIAAPP"] = "N";
                }
            }

                        
            
            dgvPOLines.AutoGenerateColumns = false;
            dgvPOLines.DataSource = purchaseorder.POlines;
                        
            EnableDisableButtons(functionid);
                                   
        }

        #region Item DataGrid
        private void dgvPOlines_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex == -1) || (e.ColumnIndex == -1)) return;

            Cursor.Current = Cursors.WaitCursor;

            LineDetails      lineDetails      = new LineDetails();
            ItemDetails      itemDetails      = new ItemDetails(DB2connection);
            ItemPODetails    itemPODetails    = new ItemPODetails(DB2connection);
            ItemVatRate      itemVatRate      = new ItemVatRate(DB2connection);
            
            lineDetails.Class        = Convert.ToInt16(dgvPOLines.Rows[e.RowIndex].Cells["Class"].Value);
            lineDetails.Vendor       = Convert.ToInt32(dgvPOLines.Rows[e.RowIndex].Cells["Vendor"].Value);
            lineDetails.Style        = Convert.ToInt16(dgvPOLines.Rows[e.RowIndex].Cells["Style"].Value);
            lineDetails.Colour       = Convert.ToInt16(dgvPOLines.Rows[e.RowIndex].Cells["Colour"].Value);
            lineDetails.Size         = Convert.ToInt16(dgvPOLines.Rows[e.RowIndex].Cells["Size"].Value);
            lineDetails.LineSequence = Convert.ToInt16(dgvPOLines.Rows[e.RowIndex].Cells["POLineSequence"].Value);

            lineDetails.LongDescription = Convert.ToString(dgvPOLines.Rows[e.RowIndex].Cells["Description"].Value);
            lineDetails.ShortDescription = Convert.ToString(dgvPOLines.Rows[e.RowIndex].Cells["ShortDesc"].Value);
            lineDetails.SeasonDescription = Convert.ToString(dgvPOLines.Rows[e.RowIndex].Cells["Season"].Value);
            lineDetails.CharacterName = Convert.ToString(dgvPOLines.Rows[e.RowIndex].Cells["Character"].Value);
            lineDetails.Market = purchaseorder.Market;
            lineDetails.StoreVatCode = "A";

            itemDetails.GetItemDetails(lineDetails.Class, lineDetails.Vendor, lineDetails.Style, lineDetails.Colour, lineDetails.Size, lineDetails.Market);
            

            lineDetails.VendorStyle = itemDetails.VendorStyle;
            lineDetails.VendorUPC = itemDetails.VendorUPC;
            lineDetails.ItemVatCode = itemDetails.ItemVatCode;
            lineDetails.SKUNumber = itemDetails.SKUNumber;
            lineDetails.ClassName = itemDetails.ClassName;
            lineDetails.ColourName = itemDetails.ColourName;
            lineDetails.SizeName = itemDetails.SizeName;
            lineDetails.CasePackType = itemDetails.CasePackType;
            lineDetails.CasePackTypeDes = itemDetails.CasePackTypeDes;
            lineDetails.CasePackQty = itemDetails.CasePackQty;
            lineDetails.DistroInnerQty = itemDetails.DistroInnerQty;
            lineDetails.Height = itemDetails.Height;
            lineDetails.Width = itemDetails.Width;
            lineDetails.Length = itemDetails.Length;
            lineDetails.Weight = itemDetails.Weight;
            lineDetails.SpicePO = purchaseorder.SpicePOnumber;
            lineDetails.SpicePOVersion = purchaseorder.SpicePOversion;
               


            itemVatRate.GetVatRate(lineDetails.Market, lineDetails.StoreVatCode, lineDetails.ItemVatCode);
            lineDetails.VatRate = itemVatRate.VatRate;

            itemPODetails.GetItemPODetails(lineDetails.Class, lineDetails.Vendor, lineDetails.Style, lineDetails.Colour, lineDetails.Size, lineDetails.SpicePO, lineDetails.SpicePOVersion, lineDetails.LineSequence);

            lineDetails.POOrderQty = itemPODetails.POOrderQty;
            lineDetails.PORetailPrice = itemPODetails.PORetailPrice;
            lineDetails.POSimpleVendorCost = itemPODetails.POSimpleVendorCost;
            lineDetails.POCurrency = itemPODetails.POCurrency;
            lineDetails.MarketCurrency = itemPODetails.MarketCurrency;
            lineDetails.LandingFactor = itemPODetails.LandingFactor;
            lineDetails.CurrencyRate = itemPODetails.CurrencyRate;
            lineDetails.LandedCost = itemPODetails.LandedCost;

            if ((string)dgvPOLines.Rows[e.RowIndex].Cells["Pack"].Value == "Y")
            {
                POLineDetailsPack polinedetailspack = new POLineDetailsPack(purchaseorder, lineDetails);

                polinedetailspack.Show(this);
                polinedetailspack = null;
            }
            else
            {
                POLineForm polineform = new POLineForm(lineDetails);

                polineform.Show(this);
                polineform = null;
            }
        }

        private void dgvPOLines_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {              
            //Show Market Retail Prices
            if (dgvPOLines.Columns[e.ColumnIndex].Name == "Retail" && e.RowIndex != -1)
            {   
                GridClass  = Convert.ToInt16(dgvPOLines.Rows[e.RowIndex].Cells["Class"].Value);
                GridVendor = Convert.ToInt32(dgvPOLines.Rows[e.RowIndex].Cells["Vendor"].Value);
                GridStyle  = Convert.ToInt16(dgvPOLines.Rows[e.RowIndex].Cells["Style"].Value);
                GridColour = Convert.ToInt16(dgvPOLines.Rows[e.RowIndex].Cells["Colour"].Value);
                GridSize   = Convert.ToInt16(dgvPOLines.Rows[e.RowIndex].Cells["Size"].Value);
                
                ItemPriceDetails itemPriceDetails = new ItemPriceDetails(DB2connection);

                DataTable dtItemPriceDetails = itemPriceDetails.GetItemPriceDetails(GridClass, GridVendor, GridStyle, GridColour, GridSize);
                                               
                StringBuilder Prices     = new  StringBuilder();

                Prices.Append("Retail : ");
                foreach (DataRow drow in dtItemPriceDetails.Rows)
                {
                    Prices.Append("(");
                    Prices.Append(drow["IMIMKT"]);
                    Prices.Append(")  ");
                    Prices.Append(drow["IMIRP"]);
                    Prices.Append("   ");
                }

                label1.Text = String.Empty;
                label1.Text = Prices.ToString();
            }
        }

        private void dgvPOLines_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            label1.Text = String.Empty;
        }
        
        #endregion Item DataGrid

        #region ActionButtons


        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx");
        }

        private void btnPOHistory_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            History historylookup;

            historylookup = new History(PgmDB, "PO", SpicePOnumber, purchaseorder.IPPOnumber);
            historylookup.ShowDialog();
            historylookup = null;
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            Boolean bSuccess;
            Boolean bRequestApprove;

            string value = "";
            if (InputBox("SPICE - EAS - Confirm Request Approval", "Please confirm you are approving this request?", false, ref value) == DialogResult.OK)
            {
                bRequestApprove = true;
            }
            else
            {
                bRequestApprove = false;
            }

            if (bRequestApprove == true)
            {
                WriteToiSeriesDTAQ writetoiseriesdtaq = new WriteToiSeriesDTAQ(PgmDB, User);
                bSuccess = writetoiseriesdtaq.WritePOtoDtaQ("PRCUPDREQ", (Int32)_requestid, "A", "");

                this.Close();
            }
            
        }


        private void btnViewChanges_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            frmPoModificationSummary frmpomodificationsummary = new frmPoModificationSummary(PgmDB, User, Environment, this, SpicePOnumber, SpicePOversion);
            frmpomodificationsummary.ShowDialog();
            frmpomodificationsummary = null;
        }

        private void EnableDisableButtons(int functionid)
        {
            if (functionid == 10003)
            {
                btnApprove.Visible = true;
                btnReject.Visible = true;
                this.Text = "SPICE - EAS - Approve/Reject - PO Creation";
            }

            if (functionid == 10004)
            {
                btnApprove.Visible = true;
                btnReject.Visible = true;
                
                btnViewChanges.Visible = true;

                this.Text = "SPICE - EAS - Approve/Reject - PO Modification - Current";
            }

            if (functionid == 10005)
            {
                btnApprove.Visible = true;
                btnReject.Visible = true;
                this.Text = "SPICE - EAS - Approve/Reject - PO Cancellation (SPICE)";
            }

            if (functionid == 10013)
            {
                btnApprove.Visible = true;
                btnReject.Visible = true;
                this.Text = "SPICE - EAS - Approve/Reject - PO Cancellation (IP)";
            }
        }

        private void btnEAS_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            frmEASRequest frmeasrequest = new frmEASRequest(PgmDB, DB2connection, User, Environment, this, _requestid);
            frmeasrequest.ShowDialog();
            frmeasrequest = null;
        }

        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(146, 72, 75, 23);
            buttonCancel.SetBounds(227, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            // Allow 30 characters for reason code
            textBox.MaxLength = 30;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        public static DialogResult InputBox(string title, string promptText, Boolean bShowInput, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(166, 72, 75, 23);
            buttonCancel.SetBounds(247, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            textBox.MaxLength = 30;

            if (bShowInput == false)
            {
                textBox.Visible = false;
            }

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            Boolean bSuccess;
            string rejectreason = String.Empty;

            string value = "";
            if (InputBox("SPICE - EAS - Confirm Request Rejection", "Please provide a reason and confirm you are rejecting this request?", ref value) == DialogResult.OK)
            {
                rejectreason = value;

                if (String.IsNullOrEmpty(rejectreason) == true)
                {
                    MessageBox.Show("Please provide a reason for rejection", this.Text, MessageBoxButtons.OK);
                }
                else
                {
                    WriteToiSeriesDTAQ writetoiseriesdtaq = new WriteToiSeriesDTAQ(PgmDB, User);
                    bSuccess = writetoiseriesdtaq.WritePOtoDtaQ("PRCUPDREQ", (Int32)_requestid, "R", rejectreason);
                    this.Close();
                }
            }          
        }

        #endregion

        private void lblShipTo_Click(object sender, EventArgs e)
        {

        }

        private void lblLanding_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult = MessageBox.Show("Are you sure you want to Cancel Approving/Rejecting this Request ?", "SPICE Approve/Reject PO Creation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (dlgResult == DialogResult.Yes)
            {
                this.Close();
            }
        }


    }
}