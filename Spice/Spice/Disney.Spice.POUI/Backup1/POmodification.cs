using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Disney.Spice.POBO;
using Disney.Spice.LookUp;
using Disney.Spice.ItemsBO;
using System.Globalization;
using System.Collections;
using System.Diagnostics;
using Disney.Spice.StoreSearch;
using System.Threading;
using Disney.Spice.ItemsUI;
using System.Runtime;
using System.Xml.Serialization;
using System.IO;

namespace Disney.Spice.POUI
{
    public partial class POmodification : Form
    {
        private PurchaseOrder              _porder;
        private LookupBO                   lookupbo;
        private Validation                 validationcls;
        private Disney.Spice.ItemsBO.Items _iteminstance;
        private POItemDetails              _polinedetails;

        //private const string MAGICDCSTORE   = "723";
        //private const string MAGICDCSTOREUS = "899";
        private string defaultdc;

        private const int      MINLANDINGVALUEFOROCN = 1;
        private const string   OCEANSHIPVIACODE = "OCN";

        private POLineDetails  polineform;
        private DataTable dtSelectedStores = null;
        private ProgressWindow pwindow;

        private int _itemquantityrounded = 0;

        private const string MAGICDCSTOREVATCODE = "A";

        private Boolean _bFormCancelClicked;
        private Boolean _bOkToValidate = true;
        private Boolean _bUserWantsToDeleteLine;
        private Boolean _bDuplicateItem;

        private DataTable _dtCurrency = null;
        private decimal _currencyratemarket;
        //private decimal _currencyratepo;
        private decimal _ccyrateprev;
        private Color       _cellbackgroundcolor;
        private DataGridViewCellStyle dgvcsPoLinesnormal;
        private DataGridViewCellStyle dgvcsPoLinesalternate;
        private DataTable   dtDropShipMatrix        = null;
        private string      _sStoreColumnNamePrefix = "Store_";
        private PurchaseOrder.PoHitsCollection _pohitscollection;
        private string _defaultshipvia = String.Empty;
        private Form _mdiparent;
        private ASNA.VisualRPG.Runtime.Database    _dbparamref;
        private Disney.Menu.Users                  _username;
        private Disney.Menu.Environments           _paramenv;
        private string                             _spiceponumber;
        private Boolean   _bDataBindingsInitalised;
        private DataTable _dtPoLines;
        private DataTable dtFreight = new DataTable("Freight");
        private Boolean   IsItemClassChanged;
        private Boolean   IsItemVendorChanged;
        private Boolean   IsItemStyleChanged;
        private Boolean   IsItemColorChanged;
        private Boolean   IsItemSizeChanged;
        private Boolean   IsPoModified;

        string _currencyformat = "N";              // Standard .NET culture aware currency format mask
        //string _currencyformat1 = "#,#,##,###,##"; // User forced non culture aware currency format mask 

        #region constructors and form events
        public POmodification(ASNA.VisualRPG.Runtime.Database dbparamref, Disney.Menu.Users username, Disney.Menu.Environments paramenv, string spiceponumber)
        {
            InitializeComponent();

            this.MaximizeBox = false;

            _mdiparent = null;

            _dbparamref = dbparamref;
            _username   = username;
            _paramenv   = paramenv;
            _spiceponumber = spiceponumber;

            SetupClassObjects();
            SetupInitialValues();
        }
        
        private void SetupInitialValues()
        {
            lblSSD.Visible = false;
            cmbSSD.Visible = false;

            cmbShipTo.Text = _porder.ShipTo.ToString();

            lookupbo = new LookupBO(_porder.DbParamRef, _porder.UserName,_porder.Penvironment);

            validationcls = new Validation(_porder.DbParamRef,_porder.UserName,_porder.Penvironment);

            if (_porder.Penvironment.DateFormat == "DMY")
            {
                dtpkrAnticipateDate.CustomFormat = "d MMMM yyyy";
                dtpkrShipDate.CustomFormat = "d MMMM yyyy";
            }
            else
            {
                dtpkrAnticipateDate.CustomFormat = "MMMMd,  yyyy";
                dtpkrShipDate.CustomFormat = "MMMMd,  yyyy";
            }

            lblMarketValue.Text = _porder.DefaultMarket + "-" + _porder.MarketDescription;

            btnStores.Enabled = false;
            btnStores.Visible = false;

            rdBtnDropShipSingle.Visible = false;
            rdBtnDropShipMatrix.Visible = false;

            if (_porder.Penvironment.Domain == "SWNA")
            {
                lblSSD.Visible = true;
                cmbSSD.Visible = true;
                cmbSSD.Enabled = true;
                PopulateSSD();        

                // The field "Total Display Ex Vat" should only be displayed if 
                // domain is US ("TDSE")
                txtTotalRetailExVat.Visible = false;
                lblTotalRetailExVat.Visible = false;

                // The checkbod "This PO is for a new Line" should only be displayed if 
                // domain is US ("TDSE")
                chkNewLineSelection.Visible = true;

                DataColumn dc = new DataColumn("FreightId", typeof(string));
                dtFreight.Columns.Add(dc);
                dc = new DataColumn("FreightDesc", typeof(string));
                dtFreight.Columns.Add(dc);

                //dtFreight.Rows.Add("", "      ");
                dtFreight.Rows.Add("C", "Collect");
                dtFreight.Rows.Add("P", "Pre Pay");

                cbxFreight.DisplayMember = "FreightDesc";
                cbxFreight.ValueMember = "FreightId";

                // Has to be explicitly set after setting the datamember and displaymember
                cbxFreight.DataSource = dtFreight;

                lblFreightCharges.Visible = true;
                cbxFreight.Visible = true;
            }

            dtSelectedStores = GetEmptyStores();
            defaultdc = LookUpDefaultDC.GetDefaultDC(_porder.Penvironment.Domain, _porder.DefaultMarket);
            dtSelectedStores.Rows.Add(true, defaultdc, "Distribution Centre");
            _porder.PoType = PurchaseOrder.POtype.StandardDCPO;

            txtLanding.Enabled = false;
            txtPortofEntry.Enabled = false;
            txtPortofDeparture.Enabled = false;
            txtDelTerms.Enabled = false;

            txtDept.Focus();

            dgvcsPoLinesnormal    = new DataGridViewCellStyle(dgvPOlines.DefaultCellStyle);
            dgvcsPoLinesalternate = new DataGridViewCellStyle(dgvPOlines.AlternatingRowsDefaultCellStyle);

            dgvPOlines.Enabled = true;

            SetupSimpleDataBinding();
        }
        #endregion

        #region POHeader
        private void txtVendor_TextChanged(object sender, EventArgs e)
        {
            lblVendorDesc.Text = "";
            errPOEntry.SetError(txtVendor, "");
        }

        private void pctBoxCurrency_Click(object sender, EventArgs e)
        {
            DataTable dt = lookupbo.CurrencyLookup();

            Enquiry currlookup = new Enquiry(dt, "CurrencyLookup");

            currlookup.ShowGrid();

            if (currlookup.DialogResult == DialogResult.OK)
            {
                txtCurrency.Text = currlookup.SelectedValue[0];
                lblCurrencyDesc.Text = currlookup.SelectedValue[1];
                errPOEntry.SetError(txtCurrency, "");
                validationcls.HighlightErrControls(lblCurrency, txtCurrency, true);
                txtCurrency.Focus();
            }
            else
            {
                txtCurrency.Text = "";
                lblCurrencyDesc.Text = "";
                errPOEntry.SetError(txtCurrency, "Please enter a valid Currency code");
                validationcls.HighlightErrControls(lblCurrency, txtCurrency, false);
            }
        }

        private void txtCurrency_Validating(object sender, CancelEventArgs e)
        {
            if (!_bFormCancelClicked)
            {
                if (_bDataBindingsInitalised == true)
                {
                    try
                    {
                        if (_porder.Currencycode == txtCurrency.Text)
                        {
                            return;
                        }

                        List<string> lstReturn = new List<string>();

                        lstReturn = validationcls.CurrencyValidate(txtCurrency.Text, _porder.MarketCurrency);
                        if (String.IsNullOrEmpty(lstReturn[0].ToString()))
                        {
                            txtCurrency.Clear();
                            lblCurrencyDesc.Text = "";
                            lblCurrValue.Text = "";
                            lblCurrVal1.Text = "";
                            validationcls.HighlightErrControls(lblCurrency, txtCurrency, false);
                            errPOEntry.SetError(txtCurrency, "Please enter a valid currency code");
                            e.Cancel = true;
                        }
                        else
                        {
                            errPOEntry.SetError(txtCurrency, "");

                            lblCurrVal1.Text = "(" + _porder.MarketCurrency + ")";
                            lblCurrValue.Text = "(" + _porder.MarketCurrency + ")";

                            _porder.Currencycode = txtCurrency.Text;
                            Decimal exchangerate = Convert.ToDecimal(lstReturn[2]);

                            if (exchangerate == _currencyratemarket)
                            {
                                exchangerate = 1.000000m;
                            }

                            lblCurrencyDesc.Text = lstReturn[1] + " (" + exchangerate + ")";

                            validationcls.HighlightErrControls(lblCurrency, txtCurrency, true);
                            ValidateHeaderFields(sender);

                            // PO is now modified so enable the "PO Modify" button
                            btnCreatePO.Enabled = true;
                            IsPoModified = true;

                            // Once currency is validated we must recalculate 
                            // the Cost field(s) with the new currency rate. 
                            for (int RowNbr = 0; RowNbr < dgvPOlines.Rows.Count - 1; RowNbr++)
                            {
                                //Int16 sequence = Convert.ToInt16(dgvPOlines["Sequence", RowNbr].Value);
                                _polinedetails = _porder.lstpoLineItemDetails[RowNbr];

                                decimal ConvertedCost = Convert.ToDecimal(dgvPOlines["ConvertedCost", RowNbr].Value);
                                ConvertedCost = Decimal.Round((ConvertedCost * _porder.ExchangeRate) / exchangerate, 2);

                                dgvPOlines["ConvertedCost", RowNbr].Value = ConvertedCost;
                                _polinedetails.ConvertedCost = ConvertedCost;
                            }

                            // Save new exchange rate in Order
                            _porder.ExchangeRate = exchangerate;
                        }
                    }
                    catch (Exception)
                    {
                        lblCurrencyDesc.Text = "";
                        validationcls.HighlightErrControls(lblCurrency, txtCurrency, false);
                        errPOEntry.SetError(txtCurrency, "Enter a valid currency code");
                        e.Cancel = true;
                    }
                }
            }
        }

        private void pctBxShipVia_Click(object sender, EventArgs e)
        {
            //Ship Via Lookup
            errPOEntry.SetError(txtShipVia, "");

            DataTable dtShipVia = lookupbo.ShipviaLookup();

            Enquiry shipvialookup = new Enquiry(dtShipVia, "ShipViaLookup");

            shipvialookup.ShowGrid();

            if (shipvialookup.DialogResult == DialogResult.OK)
            {
                txtShipVia.Text = shipvialookup.SelectedValue[0];
                lblShipViaDesc.Text = shipvialookup.SelectedValue[1];
                errPOEntry.SetError(txtShipVia, "");
                validationcls.HighlightErrControls(lblShipVia, txtShipVia, true);
                txtShipVia.Focus();
            }
            else
            {
                lblShipViaDesc.Text = "";
                errPOEntry.SetError(txtShipVia, "Please enter a valid ShipVia code");
                validationcls.HighlightErrControls(lblShipVia, txtShipVia, false);
            }
            
            ImportControlChanges();
        }
        
        private void btnStores_Click(object sender, EventArgs e)
        {
            //Store Selector Lookup Good luck Joe
            frmStoreSelection frmStoreSelection =null;
         
            if (frmStoreSelection == null)
            {
                frmStoreSelection = new frmStoreSelection(_porder.DbParamRef, _porder.UserName, _porder.Penvironment);

                if (dtSelectedStores == null)
                {
                    DataTable dtSelection = GetEmptyStores();
                    dtSelectedStores = frmStoreSelection.frmStoreSelection_Load(_porder.DefaultMarket, dtSelection);
                }
                else
                {
                    DataTable dtOriginalSet = dtSelectedStores.Copy();
                    dtSelectedStores = frmStoreSelection.frmStoreSelection_Load(_porder.DefaultMarket, dtOriginalSet);
                }
                                          
                frmStoreSelection = null;

                frmDropShipMatrix frmdropshipmatrix = null;

                // HK : BM : 1-12-2009 : If no stores selected then do not hos the Drop Ship Matrix form 
                if (rdBtnDropShipMatrix.Checked && dtSelectedStores.Rows.Count > 0)
                {
                    // Pass in the Purchase Order class object, any previously drop ship matrix datatable and the selected stores
                    frmdropshipmatrix = new frmDropShipMatrix(_porder, dtDropShipMatrix, dtSelectedStores);
                    frmdropshipmatrix.OnOkButtonClicked += new frmDropShipMatrix.OkButtonClickedEventHandler(frmdropshipmatrix_OnOkButtonClicked);
                    frmdropshipmatrix.OnCancelButtonClicked += new frmDropShipMatrix.CancelButtonClickedEventHandler(frmdropshipmatrix_OnCancelButtonClicked);

                    frmdropshipmatrix.ShowDialog();
                }

                frmdropshipmatrix = null;
            }
        }

        void frmdropshipmatrix_OnCancelButtonClicked(object sender, frmDropShipMatrix.DropShipMatrixEventArgs e)
        {
            dtDropShipMatrix = e.dtdropdhipmatrix;
        }

        void frmdropshipmatrix_OnOkButtonClicked(object sender, frmDropShipMatrix.DropShipMatrixEventArgs e)
        {
            dtDropShipMatrix = e.dtdropdhipmatrix;
        }

        private DataTable GetEmptyStores()
        {
            DataTable dtStores = new DataTable();
            dtStores.Columns.Add("clmSelected", typeof(bool));
            dtStores.Columns.Add("clmStore", typeof(string));
            dtStores.Columns.Add("clmStoreName", typeof(string));

            return dtStores;
        }

        private void rdBtnDCPO_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdBtnDCPO.Checked)
            {
                dtSelectedStores = GetEmptyStores();
                dtSelectedStores.Rows.Add(true, defaultdc, "Distribution Centre");

                _porder.PurchaseOrderType = PurchaseOrder.POtype.StandardDCPO;
                btnStores.Enabled = false;

                CalculatePOSummary();
                cmbShipTo.Enabled = false;
            }
        }
        
        private void rdBtnDropShipSingle_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnDropShipSingle.Checked)
            {
                _porder.PurchaseOrderType = PurchaseOrder.POtype.DropShipSingle;
                btnStores.Enabled = true;
                //btnHits.Enabled = false;
                dtSelectedStores.Rows.Clear();

                // HK : 16-09-2009Blank out the PO Summary
                DisplayPOSummaryNA();
                cmbShipTo.Enabled = false;
            }
        }

        private void rdBtnDropShipMatrix_CheckedChanged(object sender, EventArgs e)
        {
            // Blank out the PO Summary
            DisplayPOSummaryNA();
            cmbShipTo.Enabled = false;

            btnStores.Enabled = true;

            //btnHits.Enabled = false;

            _porder.PurchaseOrderType = PurchaseOrder.POtype.DropShipMultiple;
        }

        //private void cmbShipTo_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //}

        private void txtLanding_Validating(object sender, CancelEventArgs e)
        {
            decimal dLanding;

            // Dont validate when user is trying to close the form
            if (!_bFormCancelClicked)
            {
                if (txtShipVia.Text == "ROD")
                {
                    errPOEntry.SetError(txtLanding, "");
                    txtLanding.Text = "1";
                    _porder.Landing = 1;
                }
                else
                {
                    if ((Decimal.TryParse(txtLanding.Text, out dLanding)) && (dLanding > 0))
                    {
                        errPOEntry.SetError(txtLanding, "");
                        _porder.Landing = dLanding + 1;

                        validationcls.HighlightErrControls(lblLanding, txtLanding, true);
                    }
                    else
                    {
                        validationcls.HighlightErrControls(lblLanding, txtLanding, false);
                        errPOEntry.SetError(txtLanding, "Enter a value greater than zero");
                        e.Cancel = true;
                    }
                }

                btnCreatePO.Enabled = true;

                // Update Landed Cost values in Grid
                for (int RowNbr = 0; RowNbr < dgvPOlines.Rows.Count - 1; RowNbr++)
                {
                    _polinedetails = _porder.lstpoLineItemDetails[RowNbr];

                    decimal cost = Convert.ToDecimal(dgvPOlines["Cost", RowNbr].Value);
                    decimal landedcost = Decimal.Round(cost * _porder.Landing, 2);
                    
                    dgvPOlines["LandedCost", RowNbr].Value = landedcost;
                    _polinedetails.LandedCost = landedcost;
                }

                //Calculate PO Summary if not empty
                if (!String.IsNullOrEmpty(txtMarginPercent.Text)) CalculatePOSummary();
            }
        }
        #endregion

        #region Shipping
        private void pctBxPortofDeparture_Click(object sender, EventArgs e)
        {
            DataTable dtPortofDepartureLookup = lookupbo.PortofDepartureLookup();

            Enquiry portofdeparturelookup = new Enquiry(dtPortofDepartureLookup, "PortofDepartureLookup");

            portofdeparturelookup.ShowGrid();

            if (portofdeparturelookup.DialogResult == DialogResult.OK)
            {
                txtPortofDeparture.Text = portofdeparturelookup.SelectedValue[0];
                lblDeparturePortDesc.Text = portofdeparturelookup.SelectedValue[1];
                errPOEntry.SetError(txtPortofDeparture, "");
                validationcls.HighlightErrControls(lblPortofDeparture, txtPortofDeparture, true);
                txtPortofDeparture.Focus();
            }
            else
            {
                txtPortofDeparture.Text = "";
                lblDeparturePortDesc.Text= "";
                errPOEntry.SetError(txtPortofDeparture, "Please enter a valid port code");
                validationcls.HighlightErrControls(lblPortofDeparture, txtPortofDeparture, false);
            }
        }

        private void pctBxPortofEntry_Click(object sender, EventArgs e)
        {
            //Using the same object since both ports have the same lookup
            DataTable dtPortofEntryLookup = lookupbo.PortofDepartureLookup();

            Enquiry portofEntryLookup = new Enquiry(dtPortofEntryLookup, "PortofEntryLookup");

            portofEntryLookup.ShowGrid();

            if (portofEntryLookup.DialogResult == DialogResult.OK)
            {
                txtPortofEntry.Text = portofEntryLookup.SelectedValue[0];
                lblEntryPortDesc.Text = portofEntryLookup.SelectedValue[1];
                errPOEntry.SetError(txtPortofEntry, "");
                txtPortofEntry.Focus();
                validationcls.HighlightErrControls(lblPortofEntry, txtPortofEntry, true);
            }
            else
            {
                txtPortofEntry.Text = "";
                lblEntryPortDesc.Text = "";
                errPOEntry.SetError(txtPortofEntry, "Please enter a valid port");
                validationcls.HighlightErrControls(lblPortofEntry, txtPortofEntry, false);
            }
        }

        private void pctBxDelTerms_Click(object sender, EventArgs e)
        {
            DataTable dtDeliveryTerms = lookupbo.DeliveryTermsLookup();

            Enquiry deltermslookup = new Enquiry(dtDeliveryTerms, "DeliveryTermsLookup");

            deltermslookup.ShowGrid();

            if (deltermslookup.DialogResult == DialogResult.OK)
            {
                txtDelTerms.Text = deltermslookup.SelectedValue[0];
                lblDeliveryTermsDesc.Text = deltermslookup.SelectedValue[1];
                errPOEntry.SetError(txtDelTerms, "");
                txtDelTerms.Focus();
                validationcls.HighlightErrControls(lblDelTerms, txtDelTerms, true);
            }
            else
            {
                txtDelTerms.Text = "";
                lblDeliveryTermsDesc.Text = "";
                errPOEntry.SetError(txtDelTerms,"Please enter a valid delivery code");
                validationcls.HighlightErrControls(lblDelTerms, txtDelTerms, false);
            }
        }
        #endregion

        #region Item DataGrid
        private void dgvPOlines_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1) return;

            Int16 seq = Convert.ToInt16(dgvPOlines["sequence", e.RowIndex].Value);
            if (seq == 0) return;

            _polinedetails = _porder.lstpoLineItemDetails[seq - 1];

            if (_polinedetails.Sequence != seq) MessageBox.Show("Sequence numbers are wrong");

            //Display APP or Item Details Window
            if (_polinedetails.IsValid && _polinedetails.APP1=="N")
            {
                Cursor.Current = Cursors.WaitCursor;

                polineform = new POLineDetails(_porder, ref _polinedetails, false);
                polineform.ShowDialog(this);

                // Update DataGrid Quantity, Cost, and Summary figures
                dgvPOlines["Quantity", e.RowIndex].Value = _polinedetails.Itemquantity;
                dgvPOlines["ConvertedCost", e.RowIndex].Value = _polinedetails.ConvertedCost;
                
                CalculatePOSummary();

                polineform = null;
            }
            else if (_polinedetails.IsValid && _polinedetails.APP1 == "Y")
            {
                Cursor.Current = Cursors.WaitCursor;

                POLineDetailsPack polinedetailspack = new POLineDetailsPack(_polinedetails, _porder, e.RowIndex);

                polinedetailspack.OnAppQuantityChanged += new POLineDetailsPack.AppQuantityChangedEventHandler(polinedetailspack_OnAppQuantityChanged);
                polinedetailspack.ShowDialog(this);

                dgvPOlines["Quantity", e.RowIndex].Value = _polinedetails.Itemquantity;

                //_polinedetails.ConvertedCost = Decimal.Round(_polinedetails.Cost * _currencyratemarket / _currencyratepo, 2);
                _polinedetails.ConvertedCost = Decimal.Round(_polinedetails.Cost * _currencyratemarket / _porder.ExchangeRate, 2);
                dgvPOlines["ConvertedCost", e.RowIndex].Value = _polinedetails.ConvertedCost;

                if (_porder.Landing == 0) _porder.Landing = 1;

                _polinedetails.LandedCost = _polinedetails.Cost * _porder.Landing;
                dgvPOlines["LandedCost", e.RowIndex].Value = _polinedetails.LandedCost;

                CalculatePOSummary();
                
                polinedetailspack = null;
            }
        }

        void polinedetailspack_OnAppQuantityChanged(object sender, POLineDetailsPack.AppDetailsEventArgs e)
        {
            // Reassign the instance valriable of the business objects and others that may have 
            // been modified in the called window
            _porder = e.porder;
            _polinedetails = e.poline;

            // Refresh the value in the corresponding record in the grid
            dgvPOlines.Rows[e.rowindex].Cells["Quantity"].Value = e.quantity;

            //dtgrdPOLinesView.Rows[e.rowindex].Cells["Cost"].Value = e.poline.Cost;
            dgvPOlines.Rows[e.rowindex].Cells["ConvertedCost"].Value = e.poline.ConvertedCost;

            CalculatePOSummary();

            // Re subscribe the event handler on the PO Line Items business boject
            // for ASH's existing coding and functionaly to function as normal
            //_polinedetails.ItemQtyChanged += new POItemDetails.delItemQtyChanged(_polinedetails_ItemQtyChanged);
        }

        private void dgvPOlines_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (_bFormCancelClicked) return;

            // If the user wants to delete the row then disable any pending row or cell level 
            // validation on the datagrid
            if (_bUserWantsToDeleteLine) return;

            if (dgvPOlines.Rows[e.RowIndex].IsNewRow) return;

            try
            {
                if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Class"))
                {
                    // HK : Fix Bug 162 : Validation and delete annoyances faced by users
                    // HK : FC : Do not validate class if nothing or spaces entered by user
                    if (String.IsNullOrEmpty(e.FormattedValue.ToString().Trim()))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = String.Empty;
                        return;
                    }

                    List<string> retValues = validationcls.ValidateClass(e.FormattedValue.ToString());
                    if (("False" == retValues[0]))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "Please enter valid  " + dgvPOlines.Columns[e.ColumnIndex].Name;
                        e.Cancel = true;
                    }
                    else
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "";
                        _polinedetails.Classname = retValues[1].ToString();
                        _polinedetails.ClassCode = Int16.Parse(e.FormattedValue.ToString());
                        e.Cancel = false;

                        // Check to see if the item class in the datagrid row the same as one entered 
                        // by the user.
                        short itemclass = Convert.ToInt16(dgvPOlines["Class", e.RowIndex].Value);
                        if (itemclass != Int16.Parse(e.FormattedValue.ToString()))
                        {
                            IsItemClassChanged = true;
                        }
                    }
                }

                else if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Vendor"))
                {

                    // HK : Fix Bug 162 : Validation and delete annoyances faced by users
                    // HK : FC : Do not validate class if nothing or spaces entered by user
                    if (String.IsNullOrEmpty(e.FormattedValue.ToString().Trim()))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "";
                        return;
                    }

                    List<string> retValues = validationcls.ValidateVendor(e.FormattedValue.ToString(), true);

                    if (("False" == retValues[0]))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "Please enter valid " + dgvPOlines.Columns[e.ColumnIndex].Name;
                        e.Cancel = true;
                    }
                    else
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "";
                        //Pack values to the poline class
                        _polinedetails.Vendorcode = Int32.Parse(e.FormattedValue.ToString());
                        _polinedetails.Vendordesc = retValues[1].ToString();
                        e.Cancel = false;

                        // HK : 18-01-2010 : Fix Bug 253
                        // Check to see if the item class in the datagrid row the same as one entered 
                        // by the user.
                        // HK : CJ : 27-01-2010 : Change vendor from short to int
                        int vendor = Convert.ToInt32(dgvPOlines["Vendor", e.RowIndex].Value);
                        if (vendor != Int32.Parse(e.FormattedValue.ToString()))
                        {
                            IsItemVendorChanged = true;
                        }
                    }

                }

                else if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Style"))
                {
                    // HK : Fix Bug 162 : Validation and delete annoyances faced by users
                    // HK : FC : Do not validate class if nothing or spaces entered by user
                    if (String.IsNullOrEmpty(e.FormattedValue.ToString().Trim()))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "";
                        return;
                    }

                    List<string> retValues = validationcls.ValidateStyle(e.FormattedValue.ToString());
                    if (("False" == retValues[0]))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "Please enter valid " + dgvPOlines.Columns[e.ColumnIndex].Name;
                        e.Cancel = true;
                    }
                    else
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "";
                        _polinedetails.Stylecode = Int16.Parse(e.FormattedValue.ToString());

                        // HK : 18-01-2010 : Fix Bug 253
                        // Check to see if the item class in the datagrid row the same as one entered 
                        // by the user.
                        short style = Convert.ToInt16(dgvPOlines["Style", e.RowIndex].Value);
                        if (style != Int16.Parse(e.FormattedValue.ToString()))
                        {
                            IsItemStyleChanged = true;
                        }
                    }
                }
                else if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Color"))
                {
                    // HK : Fix Bug 162 : Validation and delete annoyances faced by users
                    // HK : FC : Do not validate class if nothing or spaces entered by user
                    if (String.IsNullOrEmpty(e.FormattedValue.ToString().Trim()))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "";
                        return;
                    }

                    List<string> retValues = validationcls.ValidateColour(e.FormattedValue.ToString());

                    if (("False" == retValues[0]))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "Please enter valid " + dgvPOlines.Columns[e.ColumnIndex].Name;
                        e.Cancel = true;
                    }
                    else
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "";
                        _polinedetails.Colorcode = Int16.Parse(e.FormattedValue.ToString());
                        _polinedetails.Colordesc = retValues[1];

                        // HK : 18-01-2010 : Fix Bug 253
                        // Check to see if the item class in the datagrid row the same as one entered 
                        // by the user.
                        short color = Convert.ToInt16(dgvPOlines["Color", e.RowIndex].Value);
                        if (color != Int16.Parse(e.FormattedValue.ToString()))
                        {
                            IsItemColorChanged = true;
                        }
                    }
                }

                else if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Size"))
                {
                    short iItem;
                    int iVendor;
                    short iStyle;
                    short iColour;
                    int iitemindex;

                    // Try and convert Class, Vendor, Style, Color, Size
                    Int16.TryParse(Convert.ToString(dgvPOlines.Rows[e.RowIndex].Cells["Class"].Value), out iItem);
                    Int32.TryParse(Convert.ToString(dgvPOlines.Rows[e.RowIndex].Cells["Vendor"].Value), out iVendor);
                    Int16.TryParse(Convert.ToString(dgvPOlines.Rows[e.RowIndex].Cells["Style"].Value), out iStyle);
                    Int16.TryParse(Convert.ToString(dgvPOlines.Rows[e.RowIndex].Cells["Color"].Value), out iColour);
                    iitemindex = _polinedetails.Itemindex;

                    // Check to see if this item is a duplicate
                    _bDuplicateItem = CheckForDuplicateLine(iitemindex, iItem, iVendor, iStyle, iColour, e.FormattedValue.ToString());

                    // HK : Fix Bug 162 : Validation and delete annoyances faced by users
                    // HK : FC : Do not validate class if nothing or spaces entered by user
                    if (String.IsNullOrEmpty(e.FormattedValue.ToString().Trim()))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "";
                        return;
                    }

                    List<string> retValues = validationcls.ValidateSize(e.FormattedValue.ToString());

                    if (("False" == retValues[0]))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "Please enter valid " + dgvPOlines.Columns[e.ColumnIndex].Name;
                        e.Cancel = true;
                    }
                    else
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "";

                        _polinedetails.Itemsize = Int16.Parse(e.FormattedValue.ToString());
                        _polinedetails.Sizename = retValues[1];

                        // Check to see if the item class in the datagrid row the same as one entered 
                        // by the user.
                        short size = Convert.ToInt16(dgvPOlines["Size", e.RowIndex].Value);
                        if (size != Int16.Parse(e.FormattedValue.ToString()))
                        {
                            IsItemSizeChanged = true;
                        }

                        if (!IsOkToDoItemLookup(iitemindex, iItem, iVendor, iStyle, iColour, iStyle))
                        {
                            dgvPOlines.Rows[e.RowIndex].ErrorText = "";
                            return;
                        }

                        if (_polinedetails.ItemLookup(_porder.DbParamRef, _porder.UserName, _porder.Penvironment, _porder.DefaultMarket))
                        {
                            dgvPOlines.Rows[e.RowIndex].Cells["Description"].Value  = _polinedetails.Itemlongdescription;
                            dgvPOlines.Rows[e.RowIndex].Cells["Retail"].Value       = _polinedetails.Retailprice.ToString();
                            dgvPOlines.Rows[e.RowIndex].Cells["Cost"].Value         = _polinedetails.Cost.ToString();
                            dgvPOlines.Rows[e.RowIndex].Cells["Character"].Value    = _polinedetails.Characterdesc;
                            dgvPOlines.Rows[e.RowIndex].Cells["Season"].Value       = _polinedetails.SeasonCode;
                            dgvPOlines.Rows[e.RowIndex].Cells["CasePackType"].Value = _polinedetails.Packdescription;
                            dgvPOlines.Rows[e.RowIndex].Cells["TicketType"].Value   = _polinedetails.Tickettype;
                            dgvPOlines.Rows[e.RowIndex].Cells["Pack"].Value         = _polinedetails.APP1;

                            if (_polinedetails.APP1 == "Y")
                            {
                                _porder.NumofPOPacks += 1;

                                dgvPOlines.Rows[e.RowIndex].Cells["ConvertedCost"].ReadOnly = true;

                                AssortedPrePack AssPrePack = new AssortedPrePack(_polinedetails, _porder);
                                DataTable prePackTbl = AssPrePack.PopulateAPPStructure();

                                for (int i = 0; i < prePackTbl.Rows.Count; i++)
                                {
                                    APPcomponent component = new APPcomponent();
                                    component.ComponentClass = (Int16)prePackTbl.Rows[i]["ComponentClass"];
                                    component.ComponentVendor = (Int32)prePackTbl.Rows[i]["ComponentVendor"];
                                    component.ComponentStyle = (Int16)prePackTbl.Rows[i]["ComponentStyle"];
                                    component.ComponentColour = (Int16)prePackTbl.Rows[i]["ComponentColour"];
                                    component.ComponentSize = (Int16)prePackTbl.Rows[i]["ComponentSize"];
                                    component.RatioQuantity = (Int16)prePackTbl.Rows[i]["ComponentQuantity"];
                                    component.Cost = (Decimal)prePackTbl.Rows[i]["ComponentCost"];
                                    component.Retail = (Decimal)prePackTbl.Rows[i]["Retail"];
                                    component.ItemDescription = (String)prePackTbl.Rows[i]["ComponentLongDesc"];

                                    _polinedetails.Components.Add(component);
                                }
                            }
                            else
                            {
                                dgvPOlines.Rows[e.RowIndex].Cells["ConvertedCost"].ReadOnly = false;
                            } 
                            
                            if (_porder.Landing == 0)
                            {
                                _porder.Landing = 1;
                            }

                            _polinedetails.ConvertedCost = Decimal.Round((_polinedetails.Cost * _currencyratemarket) / _porder.ExchangeRate, 2);
                            dgvPOlines.Rows[e.RowIndex].Cells["ConvertedCost"].Value = _polinedetails.ConvertedCost;

                            _polinedetails.LandedCost = _polinedetails.Cost * _porder.Landing;
                            dgvPOlines.Rows[e.RowIndex].Cells["LandedCost"].Value = _polinedetails.LandedCost;

                            _polinedetails.IsValid = true;
                            e.Cancel = false;

                            // Reset the Lookup flags
                            IsItemClassChanged  = false;
                            IsItemVendorChanged = false;
                            IsItemStyleChanged  = false;
                            IsItemColorChanged  = false;
                            IsItemSizeChanged   = false;

                            btnCreatePO.Enabled = true;
                        }
                        else
                        {
                            dgvPOlines.Rows[e.RowIndex].Cells["Description"].Value = _polinedetails.Itemlongdescription;
                            DataGridClearItemnotfound(e.RowIndex);

                            _polinedetails.IsValid = false;
                        }
                    }
                }

                // Cost can be changed. However the cost displayed in the grid is the 
                // ConvertedCost and not the Cost got from the database.
                else if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("ConvertedCost"))
                {
                    if (_polinedetails.IsValid == false) return;

                    if (String.IsNullOrEmpty(e.FormattedValue.ToString()))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "Please enter a cost value";
                        e.Cancel = true;
                        return;
                    }

                    decimal cost;
                    if (!Decimal.TryParse(e.FormattedValue.ToString(), out cost))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "You have entered an invalid cost value";
                        e.Cancel = true;
                        return;
                    }

                    if (cost <= 0)
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "Cost value cannot be zero or negative";
                        e.Cancel = true;
                        return;
                    }

                    // We need to convert the cost to 2 decimal positions
                    cost = Decimal.Round(cost, 2);

                    _polinedetails.ConvertedCost = cost;
                    dgvPOlines["ConvertedCost", e.RowIndex].Value = cost;

                    // Convert "ConvertedCost" value back to Market currency Cost
                    //_polinedetails.Cost = _polinedetails.ConvertedCost / _currencyratemarket * _currencyratepo;
                    _polinedetails.Cost = _polinedetails.ConvertedCost / _currencyratemarket * _porder.ExchangeRate;
                    dgvPOlines["Cost", e.RowIndex].Value = _polinedetails.Cost;

                    _polinedetails.LandedCost = Decimal.Round(_polinedetails.Cost * _porder.Landing, 2);
                    dgvPOlines["LandedCost", e.RowIndex].Value = _polinedetails.LandedCost;

                    CalculatePOSummary();

                    btnCreatePO.Enabled = true;
                    IsPoModified = true;
                }

                else if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Quantity"))
                {
                    if (_polinedetails.IsValid == false) return;

                    if (String.IsNullOrEmpty(e.FormattedValue.ToString()))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "Please enter a quantity";
                        e.Cancel = true;
                        return;
                    }

                    Int32 quantity;
                    if (!Int32.TryParse(e.FormattedValue.ToString(), out quantity))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "You have entered an invalid quantity value";
                        e.Cancel = true;
                        return;
                    }

                    if (quantity > 999999 || quantity <= 0)
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "Quantity must not be greater than 999,999, Zero, or Negative";
                        e.Cancel = true;
                        return;
                    }

                    _polinedetails.Itemquantity = quantity;
                    if (quantity % _polinedetails.DistroQty != 0)
                    {
                        QuantityRounding roundingform = new QuantityRounding(quantity, _polinedetails.DistroQty);
                        if (roundingform.ShowDialog(this) == DialogResult.OK)
                        {
                            _polinedetails.Itemquantity = roundingform.RoundedQuantity;
                        }
                        else
                        {
                            dgvPOlines.Rows[e.RowIndex].ErrorText = "Quantity must be rounded to the nearest Distro quantity";
                            e.Cancel = true;
                            return;
                        }
                    }

                    //dgvPOlines.Rows[e.RowIndex].Cells["Quantity"].Value = _polinedetails.Itemquantity;
                    CalculatePOSummary();
                    //UpdatePoLinesPacks();

                    btnCreatePO.Enabled = true;
                    IsPoModified = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source);
            }
        }

        private void dgvPOlines_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {
                dgvPOlines[e.ColumnIndex, e.RowIndex].Value = _polinedetails.Itemquantity;
            }

            if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Size"))
            {
                if (_bDuplicateItem)
                {
                    // Always in Red
                    SetItemColor (e.ColumnIndex, e.RowIndex, System.Drawing.Color.Red);
                    _bDuplicateItem = false;
                }
                else
                {
                    // Un highlight the even row
                    if (e.RowIndex % 2 == 0)
                    {
                        SetItemColor(e.ColumnIndex, e.RowIndex, dgvcsPoLinesnormal.BackColor);
                    }

                    // Un highlight the odd row
                    if (e.RowIndex % 2 != 0)
                    {
                        SetItemColor(e.ColumnIndex, e.RowIndex, dgvcsPoLinesalternate.BackColor);
                    }
                }
            }
        }

        //private void dgvPOlines_RowValidated(object sender, DataGridViewCellEventArgs e)
        //{
        //    // CJ: Recalculate Line level LandedCost
        //    if (dgvPOlines.Rows[e.RowIndex].IsNewRow == false)
        //    {
        //        //_polinedetails.LandedCost = Decimal.Round((_polinedetails.Cost * _currencyratemarket) * _porder.Landing, 2);
        //        //_polinedetails.LandedCost = (_polinedetails.Cost * _currencyratemarket) * _porder.Landing;
        //        _polinedetails.LandedCost = _polinedetails.Cost * _porder.Landing;
        //        dgvPOlines["LandedCost", e.RowIndex].Value = _polinedetails.LandedCost;
        //    }
        //}

        void _polinedetails_ItemQtyChanged(int qty, decimal cost, int RowIndex)
        {
            // HK : 10-11-2009 : Called from PO Line form and ItemQuantityForm. 
            // In the ItemQuantityForm when the user rounded up or rounded down the 
            // quantity, it has to be displayed in the grid on the correct row.
            // s
            // In the PO Line form, any changes to quantiry and  / or cost price
            // must also be displayed on the correct row in the grid

            dgvPOlines["Quantity", RowIndex].Value = qty;

            // HK : 15-01-2010 : Fix Bug 233 : Converted Cost can be changed by user
            //dtgrdPOLinesView["Cost",RowIndex].Value = cost;

            // HK : 15-11-2009 : Fix Bug 135
            // When the Cost changes in the PoLineForm window it mut reflect the 
            // changes to ConvertedCost and LandedCost

            // Column Name                  Label           Visible     Expression
            // =====================================================================================
            // ConvertedCost                Cost            True        Decimal.Round((_polinedetails.Cost * _currencyratemarket) / _currencyratepo, 2);
            // Cost                         Uplift Cot      False       Value retrieved from database by ItemLookup. Also cased simple vendor cost
            // LandedCost                   Landed Cost     False       Decimal.Round((_polinedetails.Cost * _currencyratemarket) * _porder.Landing, 2);

            // HK : 15-01-2010 : Fix Bug 233 : No need to change the landed cost
            dgvPOlines.Rows[RowIndex].Cells["LandedCost"].Value = Decimal.Round((_polinedetails.Cost * _currencyratemarket) * _porder.Landing, 2);

            dgvPOlines.Rows[RowIndex].Cells["ConvertedCost"].Value = cost;

            CalculatePOSummary();
        }

        void DataGridClearItemnotfound(int rowindex)
        {
            dgvPOlines.Rows[rowindex].Cells["Retail"].Value = String.Empty;
            dgvPOlines.Rows[rowindex].Cells["Cost"].Value = String.Empty;
            dgvPOlines.Rows[rowindex].Cells["Character"].Value = String.Empty;
            dgvPOlines.Rows[rowindex].Cells["Season"].Value = String.Empty;
            dgvPOlines.Rows[rowindex].Cells["CasePackType"].Value = String.Empty;
            dgvPOlines.Rows[rowindex].Cells["TicketType"].Value = String.Empty;
            dgvPOlines.Rows[rowindex].Cells["Pack"].Value = String.Empty;
            dgvPOlines.Rows[rowindex].Cells["Quantity"].Value = String.Empty;
            dgvPOlines.Rows[rowindex].Cells["ConvertedCost"].Value = String.Empty;
            dgvPOlines.Rows[rowindex].Cells["LandedCost"].Value = String.Empty;
            //dgvPOlines.Rows[rowindex].Cells["Sequence"].Value = String.Empty;  // CJ woz 'ere

            _polinedetails.Itemlongdescription = String.Empty;
            _polinedetails.Retailprice = 0.00m;
            _polinedetails.Cost = 0.00m;
            _polinedetails.Characterdesc = String.Empty;
            _polinedetails.SeasonDesc = String.Empty;
            _polinedetails.Packdescription = String.Empty;
            _polinedetails.Tickettype = String.Empty;
            _polinedetails.APP1 = String.Empty;
            _polinedetails.Itemquantity = 0;

            _polinedetails.ConvertedCost = 0m;
            _polinedetails.LandedCost = 0m;
        }
        
        private void dgvPOlines_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dgvPOlines.Rows[e.RowIndex].ErrorText = String.Empty;                 
        }

        //private void dgvPOlines_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        //{
        //    // HHK : 10-11-2009 : Dont know the purpose of below code i.e why increment 
        //    // _porder.NumofPOLines by 1 if rowcount in grid != 1. Since toal number of lines
        //    // on a PO Header is always reported 1, I have decided to comment the below 'if' 
        //    // code block
        //    if (e.RowCount != 1)
        //    { 
        //        _porder.NumofPOLines += 1;
        //    }
        //}

        private void dgvPOlines_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            // When user clicks buttton 'Create PO', the 'cell validating' and 
            // 'row validating' is triggered for the new row in the datagrid/
            // Solution is not to validate a row in the grid that has no 
            // valid entry in object porder.lstpoLineItemDetails

            // HK : Prevent Datagrid Validation if the user clicked Cancel button
            if (_bFormCancelClicked)
            {
                return;
            }

            // Prevent Row level validation if user is deleting the row.
            // Dont bother with validation if user selects to delete a row.
            if (_bUserWantsToDeleteLine)
            {
                return;
            }

            if (e.RowIndex < dgvPOlines.RowCount - 1)
            {
                // Probably a good place to pack and finalize PO Summary Collection
                if ((!_polinedetails.IsValid) && IsOrderValid())
                {
                    dgvPOlines.Rows[e.RowIndex].ErrorText = "Please enter valid item";
                    e.Cancel = true;
                }
                else
                {
                    errPOEntry.SetError(dgvPOlines, "");
                    e.Cancel = false;

                    // Once all validation has been done on the row then reassign the Po Line details
                    // object to the collections to get all the lates updated and validated values

                    // If row index < count of item collection, then this row is new and 
                    // has incomplete or no 
                    if (e.RowIndex + 1 == _porder.lstpoLineItemDetails.Count)
                    {
                        if (_porder.lstpoLineItemDetails.IndexOf(_polinedetails) != -1)
                        {
                            _porder.lstpoLineItemDetails[e.RowIndex] = _polinedetails;
                        }
                    }
                }
            }
        }

        private void dgvPOlines_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!e.Row.IsNewRow)
            {
                DialogResult response = MessageBox.Show("Are you sure?", "Delete row?",
                         MessageBoxButtons.YesNo,
                         MessageBoxIcon.Question,
                         MessageBoxDefaultButton.Button2);

                if (response == DialogResult.Yes)
                {
                }
            }
        }

        private void dgvPOlines_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            Int16 nextsequencenumber;

            if (e.Row.IsNewRow)
            {
                _polinedetails = new POItemDetails(e.Row.Index);

                nextsequencenumber = GetNextSequenceNumber();
                _polinedetails.Sequence = nextsequencenumber;
                _polinedetails.IsNewLine = true;

                if (_porder.lstpoLineItemDetails.IndexOf(_polinedetails) == -1)
                {
                    _porder.lstpoLineItemDetails.Insert( (e.Row.Index - 1), _polinedetails);
                    dgvPOlines["sequence", e.Row.Index - 1].Value = nextsequencenumber;  // CJ woz 'ere
                }

                _porder.NumofPOLines += 1;
            }
        }

        private void dgvPOlines_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (_bFormCancelClicked)
            {
                return;
            }

            if (_porder.lstpoLineItemDetails.Count > 0 &&  e.RowIndex < _porder.lstpoLineItemDetails.Count)
            {
                _polinedetails = _porder.lstpoLineItemDetails[e.RowIndex];
            }
        }

        private void dgvPOlines_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            // Prevent Datagrid Validation if the user clicked Cancel button
            if (_bFormCancelClicked == false)
            {
                if (!IsOrderValid())
                {
                    errPOEntry.SetError(btnHelp, "Please enter valid order data");
                    dgvPOlines.ReadOnly = true;
                }
                else
                {
                    dgvPOlines.ReadOnly = false;
                    errPOEntry.SetError(btnHelp, "");
                }
            }
        }

        private void dgvPOlines_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (_porder.NumofPOLines >= 1) 
            {
                _porder.NumofPOLines = _porder.NumofPOLines - 1;
            }

            // HKJ : 20-11-2009 : Commented below call to CalculatePOSummary()
            // as this is now handled in the clicked event of 'Delete Line' button
            // Here it would have fired for every row deleted. So if user deleted 900
            // rows it will fire 900 times and make the systemslow
            
            // CalculatePOSummary();
        }
        
        private void dgvPOlines_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // HK : 18-12-2009 : Do not allow this event if ColumnIndex  = -1
            if (e.ColumnIndex == -1)
            {
                return;
            }

            if (!dgvPOlines.Rows[e.RowIndex].IsNewRow && dgvPOlines.Columns[e.ColumnIndex].Name == "Vendor")
            {
                // Highlight the row in red
                if (e.Value != null && txtVendor.Text != e.Value.ToString())
                {
                    _cellbackgroundcolor = e.CellStyle.BackColor;
                    e.CellStyle.BackColor = System.Drawing.Color.Red;

                    //Font newFont = new Font(dtgrdPOLinesView.Font, FontStyle.Strikeout);
                    //dtgrdPOLinesView[e.ColumnIndex, e.RowIndex].Style.Font = newFont;
                }

                // Un highlight the even row
                if (e.RowIndex % 2 == 0 && e.Value != null && txtVendor.Text == e.Value.ToString())
                {
                    //e.CellStyle.BackColor = _cellbackgroundcolor;
                    e.CellStyle.BackColor = dgvcsPoLinesnormal.BackColor;
                }

                // Un highlight the odd row
                if (e.RowIndex % 2 != 0 && e.Value != null && txtVendor.Text == e.Value.ToString())
                {
                    e.CellStyle.BackColor = dgvcsPoLinesalternate.BackColor;
                }
            }
        }
        
        private void dgvPOlines_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {
                if (_polinedetails.IsValid == false)
                {
                    e.Cancel = true;
                }
            }
        }

        //private void dgvPOlines_Sorted(object sender, EventArgs e)
        //{
        //}
        
        //private void dgvPOlines_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        //{
        //}
        #endregion Item DataGrid

        #region Hits
        //private void btnHits_Click(object sender, EventArgs e)
        //{
        //    POHits pohitsform = new POHits(_porder, _pohitscollection);

        //    // Subscribe to event handlers
        //    pohitsform.OnOkButtonClicked += new POHits.OkButtonClickedEventHandler(pohitsform_OnOkButtonClicked);
        //    pohitsform.OnCancelButtonClicked += new POHits.CancelButtonClickedEventHandler(pohitsform_OnCancelButtonClicked);

        //    //this.Hide();
        //    //pohitsform.Show();
        //    pohitsform.ShowDialog();
        //}

        //void pohitsform_OnCancelButtonClicked(object sender, POHits.PoHitsEventArgs e)
        //{
        //    _pohitscollection = e.poHitsCollection;
        //    //throw new Exception("The method or operation is not implemented.");
        //}

        //void pohitsform_OnOkButtonClicked(object sender, POHits.PoHitsEventArgs e)
        //{
        //    _pohitscollection = e.poHitsCollection;
        //    //throw new Exception("The method or operation is not implemented.");
        //}
        #endregion

        private Boolean CheckRequiredFields()
        {
            // Check to see if the PO Line Items are valid
            Boolean bSuccess = true;

            // Check the Ship via field
            if (String.IsNullOrEmpty(txtShipVia.Text))
            {
                errPOEntry.SetError(txtShipVia, "Enter a valid ship via code");
                validationcls.HighlightErrControls(lblShipVia, txtShipVia, false);
                bSuccess = false;
            }

            // Check the Landing field
            if (String.IsNullOrEmpty(txtLanding.Text))
            {
                errPOEntry.SetError(txtLanding, "Enter a valid landing code");
                validationcls.HighlightErrControls(lblLanding, txtLanding, false);
                bSuccess = false;
            }

            // Check the Port of Departure field
            if (String.IsNullOrEmpty(txtPortofDeparture.Text) && (txtShipVia.Text == "OCN"))
            {
                errPOEntry.SetError(txtPortofDeparture, "Enter a port of departure code");
                validationcls.HighlightErrControls(lblPortofDeparture, txtPortofDeparture, false);
                bSuccess = false;
            }

            // Check the Port of Port of Entry field
            if (String.IsNullOrEmpty(txtPortofEntry.Text) && (txtShipVia.Text == "OCN"))
            {
                errPOEntry.SetError(txtPortofEntry, "Enter a port of entry code");
                validationcls.HighlightErrControls(lblPortofEntry, txtPortofEntry, false);
                bSuccess = false;
            }


            // Check the Delivery Terms field
            if (String.IsNullOrEmpty(txtDelTerms.Text) && (txtShipVia.Text == "OCN"))
            {
                errPOEntry.SetError(txtDelTerms, "Enter a valid delivery terms code");
                validationcls.HighlightErrControls(lblDelTerms, txtDelTerms, false);
                bSuccess = false;
            }

            // Check that the Ship Date is less than Anticipate date
            if ((dtpkrAnticipateDate.Value.Date < dtpkrShipDate.Value.Date))
            {
                errPOEntry.SetError(dtpkrShipDate, "Shipping date cannot be after the Anticipate date");
                bSuccess = false;
            }
            
            // Check whether PO Items are entered
            if (_porder.lstpoLineItemDetails.Count == 0)
            {
                bSuccess = false;
            }

            return bSuccess;
        }

        private Boolean CheckItemQuantity()
        {
            Boolean bSuccess = true;
            foreach (POItemDetails item in _porder.lstpoLineItemDetails)
            {
                // Validate each item number
                if (validationcls == null)
                {
                    validationcls = new Validation(_porder.DbParamRef, _porder.UserName, _porder.Penvironment);
                }

                if (!validationcls.ValidateItemNumber(item.ClassCode, item.Vendorcode, item.Stylecode, item.Colorcode, item.Itemsize, _porder.DefaultMarket))
                {
                    return false;
                }

                if (item.IsValid == false)
                {
                    bSuccess = false;
                }

                if (item.Itemquantity == 0)
                {
                    bSuccess = false;
                }
            }

            return bSuccess;
        }

        private Boolean CheckDropShipMatrixQuantity()
        {
            int iquantity;
            int iquantityassigned;

            if (dtDropShipMatrix.Rows.Count == 0)
            {
                return false;
            }
            
            for (int i = 0; i < dtDropShipMatrix.Rows.Count; i++)
            {
                iquantity           = Convert.ToInt32(dtDropShipMatrix.Rows[i]["Quantity"]);
                iquantityassigned   = Convert.ToInt32(dtDropShipMatrix.Rows[i]["QuantityAssigned"]);

                if (iquantity != iquantityassigned)
                {
                    return false;
                }
            }

            return true;
        }

        private bool isSpicePOlocked(string SpicePOnumber,string IPPOnumber,string Status)
        {
            if (!string.IsNullOrEmpty(SpicePOnumber))
            {
                switch (Status)
                {
                    case "CE":
                        return true;

                    case "CS":
                        return true;

                    case "OC":
                        return true;

                    case "OS":
                        return true;

                    case "RC":
                        return true;

                    case "RS":
                        return true;

                    default:
                        return false;
                }
            }
            else
            {
                return validationcls.CheckIPPOstatus(IPPOnumber);
            }
        }

        private bool ModifyPurchaseOrder(string spiceponumber)
        {
            short _spicepoversionprevious;

            if (_porder.ModifyPOHeader(spiceponumber, out _spicepoversionprevious) && _porder.ModifyPOComments(_spicepoversionprevious))
            {
                DataTable dtLines = PopulatePOLines();

                if (_polinedetails.ModifyOrderLines(_porder, dtLines, _spicepoversionprevious) == true)
                {
                    return true;
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        #region Dates
        private void PopulateSSD()
        {

            DataTable dtSSD = lookupbo.PopulateSSD();

            foreach (DataRow drow in dtSSD.Rows)
            {
               cmbSSD.Items.Add(drow["clmSSDDATMDY"].ToString());
            }
            cmbSSD.SelectedIndex = 0;

        }

        private void dtpkrAnticipateDate_Validating(object sender, CancelEventArgs e)
        {
            if (!_bFormCancelClicked)
            {
                errPOEntry.SetError(dtpkrAnticipateDate, "");
                if (dtpkrAnticipateDate.Value < DateTime.Today)
                {
                    errPOEntry.SetError(dtpkrAnticipateDate, "Please enter a date greater than Today");
                    e.Cancel = true;
                    return;
                }

                _porder.AnticipateDate = dtpkrAnticipateDate.Value;
                btnCreatePO.Enabled = true;
                IsPoModified = true;
            }
        }

        //private void dtpkrAnticipateDate_ValueChanged(object sender, EventArgs e)
        //{
        //    if (_bFormCancelClicked == false)
        //    {
        //        if (_bDataBindingsInitalised == true)
        //        {
        //            _porder.AnticipateDate = dtpkrAnticipateDate.Value;
        //            if ((dtpkrAnticipateDate.Value.Date < dtpkrShipDate.Value.Date))
        //            {
        //                errPOEntry.SetError(dtpkrAnticipateDate, "Anticipate date cannot be before the ship date");
        //            }
        //            else
        //            {
        //                errPOEntry.SetError(dtpkrAnticipateDate, "");
        //                btnCreatePO.Enabled = true;
        //                IsPoModified = true;
        //            }
        //        }
        //    }
        //}
        
        private void dtpkrShipDate_Validating(object sender, CancelEventArgs e)
        {
            if (!_bFormCancelClicked)
            {
                errPOEntry.SetError(dtpkrShipDate, string.Empty);
                if (dtpkrShipDate.Value < DateTime.Today)
                {
                    errPOEntry.SetError(dtpkrShipDate, "Please enter a date greater than  Today");
                    e.Cancel = true;
                    return;
                }

                _porder.ShippingDate = dtpkrShipDate.Value;
                btnCreatePO.Enabled = true;
                IsPoModified = true;
            }
        }

        //private void dtpkrShipDate_ValueChanged(object sender, EventArgs e)
        //{
        //    // If user decided to Cancel the form do not force validation
        //    if (_bFormCancelClicked == false)
        //    {
        //        if (_bDataBindingsInitalised == true)
        //        {
        //            _porder.ShippingDate = dtpkrShipDate.Value;
        //            //if (_porder.Penvironment.Domain == "TDSNA")
        //            if (_porder.Penvironment.Domain == "SWNA")
        //            //For TDSNA
        //            {
        //                txtCancelDate.Text = dtpkrShipDate.Value.AddDays(7).ToString("D");
        //                _porder.CancelDate = dtpkrShipDate.Value.AddDays(7);
        //            }
        //            //For TDSE
        //            // HK : 19-11-2009 : Format date as [LondDate]
        //            txtCancelDate.Text = dtpkrShipDate.Value.ToLongDateString();
        //            _porder.CancelDate = dtpkrShipDate.Value;
        //            if ((dtpkrAnticipateDate.Value.Date < dtpkrShipDate.Value.Date))
        //            {
        //                errPOEntry.SetError(dtpkrShipDate, "Shipping date cannot be after the anticipate date");
        //            }
        //            else
        //            {
        //                btnCreatePO.Enabled = true;
        //                IsPoModified = true;
        //                errPOEntry.SetError(dtpkrShipDate, "");
        //            }
        //        }
        //    }
        //}

        private void grpBoxDates_Leave(object sender, EventArgs e)
        {
            // Cross validation of Anticipate date and Ship date
            if (string.IsNullOrEmpty(errPOEntry.GetError(dtpkrAnticipateDate)) & string.IsNullOrEmpty(errPOEntry.GetError(dtpkrShipDate)))
            {
                if ((dtpkrAnticipateDate.Value.Date < dtpkrShipDate.Value.Date))
                {
                errPOEntry.SetError(dtpkrAnticipateDate, "Anticipate date cannot be before the ship date");
                return;
                }

                if (_porder.AnticipateDate != dtpkrAnticipateDate.Value || _porder.ShippingDate != dtpkrShipDate.Value)
                {
                    _porder.AnticipateDate = dtpkrAnticipateDate.Value;
                    _porder.ShippingDate = dtpkrShipDate.Value;
                    
                    btnCreatePO.Enabled = true;
                    IsPoModified = true;

                    if (_porder.Penvironment.Domain == "SWNA")
                    {
                        txtCancelDate.Text = dtpkrShipDate.Value.AddDays(6).ToString("D");
                        _porder.CancelDate = dtpkrShipDate.Value.AddDays(6);
                    }
                    else
                    {
                        // Format date as [LongDate]
                        txtCancelDate.Text = dtpkrShipDate.Value.ToLongDateString();
                        _porder.CancelDate = dtpkrShipDate.Value;
                    }
                }
            }
        }
        #endregion Dates

        // If the PO type is other than Standart PO then VAT calulation is irrelevant
        private void DisplayPOSummaryNA()
        {
            txtTotalUnits.Text      = "N/A";
            txtTotalCost.Text       = "N/A";
            txtTotalRetail.Text     = "N/A";
            txtMarginValue.Text     = "N/A";
            txtMarginPercent.Text   = "N/A";
        }
        
        private void CalculatePOSummary()
        {
            decimal totalretailexvat = 0;
            decimal marginvalue = 0;

            try
            {
                // Total Retail
                _porder.TotalRetail     = _porder.CalculateTotalRetail();
                
                // Total Cost
                _porder.TotalCost       = _porder.CalculateTotalCost();

                // Total Units
                _porder.TotalUnits      = _porder.CalculateTotalUnit();

                if (_porder.Landing == 0)
                {
                    _porder.Landing = 1;
                }
                
                // Total Landed Cost
                _porder.TotalLandedCost = _porder.CalculateTotalLandedCost();
                //Magic No Alert # of stores for DC PO
                //txtNumberofDrops.Text = "1";

                // Total retail should now display Total retail Ex Vat
                // Infact the textbox txtTotalRetail is actually total retail ex vat
                totalretailexvat = _porder.CalculateTotalRetailExVat(MAGICDCSTOREVATCODE);

                // Display Total Retail as totalretail
                txtTotalRetail.Text = Decimal.Round(_porder.TotalRetail, 2).ToString(_currencyformat);

                marginvalue = (totalretailexvat - _porder.TotalLandedCost);
                _porder.MarginValue = marginvalue;

                if (totalretailexvat != 0)
                {
                    _porder.MarginPercentage = (marginvalue / totalretailexvat) * 100;
                }
                else
                {
                    _porder.MarginPercentage = 0;
                }

                // Use new methods to Calculate the Po Lines / Packs
                UpdatePoLinesPacks();

                //txtTotalCost.Text    = Decimal.Round (_porder.TotalCost).ToString();
                txtTotalCost.Text = Decimal.Round(_porder.TotalLandedCost, 2).ToString(_currencyformat);
                txtTotalUnits.Text = _porder.TotalUnits.ToString("G");

                // Display Total retail Ex VAT
                txtTotalRetailExVat.Text = Decimal.Round(totalretailexvat, 2).ToString(_currencyformat);
                
                if (_porder.MarginValue < 0)
                {
                    validationcls.HighlightErrControls(lblMarginValue, txtMarginValue, false);                     
                }
                else
                {
                    validationcls.HighlightErrControls(lblMarginValue, txtMarginValue, true);  
                }

                txtMarginValue.Text   = Decimal.Round(_porder.MarginValue, 2).ToString(_currencyformat);
                txtMarginPercent.Text = Decimal.Round(_porder.MarginPercentage, 2).ToString(_currencyformat);
                //txtNumberofDrops.Text =   "1";
            }
            
            catch (Exception)
            { 
                MessageBox.Show ("It appears that the pack size is 0", "PO Entry");
            }
        }

        private DataTable PopulatePOLines(int iHitNUmber)
        {
            // 1. Send all lines (not just one as it is currently doing) to the PO 
            //    line creation objects
            // 2. If the line item is a pack then send all the items that belong to 
            //    the pack

            DataTable dtAllPOLines = new DataTable();
            int iCount, poLineCount;

            dtAllPOLines.Columns.Add("POnumber", typeof(string));
            dtAllPOLines.Columns.Add("Version", typeof(Int16));  //Not populated
            dtAllPOLines.Columns.Add("Sequence", typeof(Int16)); //HK : CJ : 10-12-2009 : Now populated
            dtAllPOLines.Columns.Add("Class", typeof(Int16));
            dtAllPOLines.Columns.Add("Vendor", typeof(Int32));
            dtAllPOLines.Columns.Add("Style", typeof(Int16));
            dtAllPOLines.Columns.Add("Colour", typeof(Int16));
            dtAllPOLines.Columns.Add("Size", typeof(Int16));
            dtAllPOLines.Columns.Add("SKU", typeof(Int32));
            dtAllPOLines.Columns.Add("SKUCHK", typeof(Int16));
            dtAllPOLines.Columns.Add("UPC", typeof(string));
            dtAllPOLines.Columns.Add("Quantity", typeof(Int32));
            dtAllPOLines.Columns.Add("LandedCost", typeof(decimal)); //Cost * Landing Factor
            dtAllPOLines.Columns.Add("Retail", typeof(decimal));
            dtAllPOLines.Columns.Add("LongDesc", typeof(string));
            dtAllPOLines.Columns.Add("ShortDesc", typeof(string));
            dtAllPOLines.Columns.Add("VendorStyle", typeof(string));
            dtAllPOLines.Columns.Add("Season", typeof(string));
            dtAllPOLines.Columns.Add("SubClass", typeof(string));
            dtAllPOLines.Columns.Add("Ticket", typeof(string));
            dtAllPOLines.Columns.Add("CaseQuantity", typeof(Int32));
            dtAllPOLines.Columns.Add("DistroQty", typeof(Int32));
            dtAllPOLines.Columns.Add("VendorCost", typeof(decimal));
            dtAllPOLines.Columns.Add("LandFactor", typeof(decimal));
            dtAllPOLines.Columns.Add("Character", typeof(string)); // Character code

            // Total rowcount in the grid (The rows to process is one less than total rows)
            poLineCount = _porder.lstpoLineItemDetails.Count;

            for (iCount = 0; iCount < poLineCount; iCount++)
            {
                _polinedetails = _porder.lstpoLineItemDetails[iCount];

                // Check if this is a Drop Ship Matrix PO.
                // If it is then replace the item quantity in the PO Line Item with the 
                // one from the Drop Ship Matrix datatable.

                int qty = _porder.GetItemQuantityOnHit(iHitNUmber,
                                                       _polinedetails.Itemindex,
                                                       _polinedetails.ClassCode,
                                                       _polinedetails.Vendorcode,
                                                       _polinedetails.Stylecode,
                                                       _polinedetails.Colorcode,
                                                       _polinedetails.Itemsize);

                // Explicitly change the _polinedetails.Itemquantity for  
                // this store on this PO Line Item
                _polinedetails.Itemquantity = qty;

                dtAllPOLines.Rows.Add(_porder.SpicePOnumber,
                                      _porder.SpicePOversion,   //Version
                                      _polinedetails.Sequence,  //Sequence
                                      _polinedetails.ClassCode,
                                      _polinedetails.Vendorcode,
                                      _polinedetails.Stylecode,
                                      _polinedetails.Colorcode,
                                      _polinedetails.Itemsize,
                                      _polinedetails.Sku,//SKU
                                      _polinedetails.SkuChk,
                                      _polinedetails.UPC,//UPC
                                      _polinedetails.Itemquantity,
                                      Decimal.Round((_polinedetails.Cost * _currencyratemarket) * _porder.Landing, 2),
                                      _polinedetails.Retailprice,
                                      _polinedetails.Itemlongdescription,
                                      _polinedetails.Itemshortdescription,
                                      _polinedetails.Vendorstyle,
                                      _polinedetails.SeasonCode,
                                      _polinedetails.Subclass,
                                      _polinedetails.Tickettype,
                                      _polinedetails.CasePackQty,
                                      _polinedetails.DistroQty,
                                      _polinedetails.Cost, //Simple Vendor Cost 
                                      _porder.Landing,
                                      _polinedetails.Charactercode
                                      );
            }

            return dtAllPOLines;
        }
        
        private DataTable PopulatePOLines()
        {
            // 1. Send all lines (not just one as it is currently doing) to the PO 
            //    line creation objects
            // 2. If the line item is a pack then send all the items that belong to 
            //    the pack
            DataTable dtAllPOLines = new DataTable();
            int iCount, poLineCount;

            dtAllPOLines.Columns.Add("POnumber",    typeof(string));
            dtAllPOLines.Columns.Add("Version",     typeof(Int16));  // HK : Now populated
            dtAllPOLines.Columns.Add("Sequence",    typeof(Int16)); // HK : CJ : 10-12-2009 : Now populated
            dtAllPOLines.Columns.Add("Class",       typeof(Int16));
            dtAllPOLines.Columns.Add("Vendor",      typeof(Int32));
            dtAllPOLines.Columns.Add("Style",       typeof(Int16));
            dtAllPOLines.Columns.Add("Colour",      typeof(Int16));
            dtAllPOLines.Columns.Add("Size",        typeof(Int16));
            dtAllPOLines.Columns.Add("SKU",         typeof(Int32));
            dtAllPOLines.Columns.Add("SKUCHK",      typeof(Int16));       
            dtAllPOLines.Columns.Add("UPC",         typeof(string));
            dtAllPOLines.Columns.Add("Quantity",    typeof(Int32));
            dtAllPOLines.Columns.Add("LandedCost",  typeof(decimal)); //Cost * Landing Factor
            dtAllPOLines.Columns.Add("Retail",      typeof(decimal));
            dtAllPOLines.Columns.Add("LongDesc",    typeof(string));
            dtAllPOLines.Columns.Add("ShortDesc",   typeof(string));
            dtAllPOLines.Columns.Add("VendorStyle", typeof(string));
            dtAllPOLines.Columns.Add("Season",      typeof(string));
            dtAllPOLines.Columns.Add("SubClass",    typeof(string));
            dtAllPOLines.Columns.Add("Ticket",      typeof(string));
            dtAllPOLines.Columns.Add("CaseQuantity",typeof(Int32));
            dtAllPOLines.Columns.Add("DistroQty",   typeof(Int32));
            dtAllPOLines.Columns.Add("VendorCost",  typeof(decimal));
            dtAllPOLines.Columns.Add("LandFactor",  typeof(decimal));
            dtAllPOLines.Columns.Add("Character",   typeof(string)); // Character code

            // Total rowcount in the grid (The rows to process is one less than total rows)
            //poLineCount = dtgrdPOLinesView.Rows.Count;
            poLineCount = _porder.lstpoLineItemDetails.Count;

            for (iCount = 0; iCount < poLineCount ; iCount++)
            {
                _polinedetails = _porder.lstpoLineItemDetails[iCount];

                // SeasonCode to be used instead of SeasonDesc
                dtAllPOLines.Rows.Add(_porder.SpicePOnumber,
                                      _porder.SpicePOversion, //Version
                                      _polinedetails.Sequence,
                                      _polinedetails.ClassCode,
                                      _polinedetails.Vendorcode,
                                      _polinedetails.Stylecode,
                                      _polinedetails.Colorcode,
                                      _polinedetails.Itemsize,
                                      _polinedetails.Sku,//SKU
                                      _polinedetails.SkuChk,
                                      _polinedetails.UPC,//UPC
                                      _polinedetails.Itemquantity,
                                      Math.Round(_polinedetails.LandedCost,2),
                                      _polinedetails.Retailprice,
                                      _polinedetails.Itemlongdescription,
                                      _polinedetails.Itemshortdescription,
                                      _polinedetails.Vendorstyle,
                                      _polinedetails.SeasonCode,
                                      _polinedetails.Subclass,
                                      _polinedetails.Tickettype,
                                      _polinedetails.CasePackQty,
                                      _polinedetails.DistroQty,
                                      _polinedetails.ConvertedCost,
                                      _porder.Landing,
                                      _polinedetails.Charactercode
                                );
            }

            return dtAllPOLines;
        }
                
        private bool IsOrderValid()
        {
            return true;
        } 

        private void chkNewLineSelection_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNewLineSelection.CheckState == CheckState.Checked)
            {
                _porder.IsPONewLine = true;
            }
            else
            {
                _porder.IsPONewLine = false;
            }

            // PO is now modified so enable the "PO Modify" button
            btnCreatePO.Enabled = true;
            IsPoModified = true;
        }

        private void txtShipVia_Validating(object sender, CancelEventArgs e)
        {
            // Dont validate when user is trying to close the form
            if (!_bFormCancelClicked)
            {
                // Unhilight any validation errors that may have appeared
                // due to the click of the Create PO button
                validationcls.HighlightErrControls(lblLanding, txtLanding, true);
                errPOEntry.SetError(txtLanding, "");

                // No need to validate if the user is just tabbing out of the field
                // and has not changed anything.
                if (_porder.ShipViaCode == txtShipVia.Text)
                {
                    e.Cancel = false;
                    return;
                }

                List<string> lstretvalues = new List<string>();

                lstretvalues = validationcls.ValidateShipVia(txtShipVia.Text);

                if (lstretvalues[0] == "True")
                {
                    //Populate the right data etc and 
                    lblShipViaDesc.Text = lstretvalues[1];
                    errPOEntry.SetError(txtShipVia, "");
                    validationcls.HighlightErrControls(lblShipVia, txtShipVia, true);
                    e.Cancel = false;
                    _porder.ShipViaCode = txtShipVia.Text;

                    // HK : 02-12-2009 : Validate Header
                    ValidateHeaderFields(sender);

                    // HK : 19-01-2010 : Fix Bug 254, 289
                    // PO is now modified so enable the "PO Modify" button
                    btnCreatePO.Enabled = true;
                    IsPoModified = true;
                }
                else
                {
                    lblShipViaDesc.Text = "";
                    validationcls.HighlightErrControls(lblShipVia, txtShipVia, false);
                    errPOEntry.SetError(txtShipVia, "Please enter a valid port");
                    _porder.ShipViaCode = "";
                    e.Cancel = true;
                }
            }
        }

        private void txtShipVia_Validated(object sender, EventArgs e)
        {
            ImportControlChanges();
        }

        private void ImportControlChanges()
        {
            if (txtShipVia.Text == OCEANSHIPVIACODE)
            //Enable the fields
            {
                txtPortofDeparture.Enabled = true;
                pctBxPortofDeparture.Enabled = true;
                txtPortofEntry.Enabled = true;
                pctBxPortofEntry.Enabled = true;
                txtDelTerms.Enabled = true;
                pctBxDelTerms.Enabled = true;
                txtLanding.Enabled = true;
                txtLanding.Focus();
            }
            else if (txtShipVia.Text == "ROD")
            {
                txtLanding.Text = "0";
                txtLanding.Enabled = false;
                txtPortofDeparture.Text = "";
                txtPortofEntry.Text = "";
                txtDelTerms.Text = "";
                txtPortofDeparture.Enabled = false;
                pctBxPortofDeparture.Enabled = false;
                txtPortofEntry.Enabled = false;
                pctBxPortofEntry.Enabled = false;
                txtDelTerms.Enabled = false;
                pctBxDelTerms.Enabled = false;

                // HK : 13-01-2010 : If user selected 'OCN' and then 'ROD' we must 
                // blank out the description of Port of Departure, Port of Entry and 
                // Delivery Terms
                lblDeparturePortDesc.Text = "";
                lblEntryPortDesc.Text = "";
                lblDeliveryTermsDesc.Text = "";

                // HK : 14-01-2010 : Fix Bug 247
                // We could reset Validation highlights here. If we disable the controls
                // then we must un hilight the previous validation error hilighted.
                validationcls.HighlightErrControls(lblPortofDeparture, txtPortofDeparture, true);
                errPOEntry.SetError(txtPortofDeparture, "");

                validationcls.HighlightErrControls(lblPortofEntry, txtPortofEntry, true);
                errPOEntry.SetError(txtPortofEntry, "");

                validationcls.HighlightErrControls(lblDelTerms, txtDelTerms, true);
                errPOEntry.SetError(txtDelTerms, "");

                // HK : 26-01-2010 : Fix Bug 362
                validationcls.HighlightErrControls(lblLanding, txtLanding, true);
                errPOEntry.SetError(txtLanding, "");
            }
            else
            {
                txtPortofDeparture.Text = "";
                txtPortofEntry.Text = "";
                txtDelTerms.Text = "";
                txtPortofDeparture.Enabled = false;
                pctBxPortofDeparture.Enabled = false;
                txtPortofEntry.Enabled = false;
                pctBxPortofEntry.Enabled = false;
                txtDelTerms.Enabled = false;
                pctBxDelTerms.Enabled = false;
                txtLanding.Enabled = true;
                txtLanding.Focus();
            }
        }
       
        private void ClearPoItemsDataGrid()
        {
            int iRowsDeleted = 0;
            int iRunningCountTotalRows = 0;
            int iLoopCounter;

            _bUserWantsToDeleteLine = true;

            iLoopCounter = 0;
            iRunningCountTotalRows = dgvPOlines.Rows.Count;

            do
            {
                if (!dgvPOlines.Rows[iLoopCounter].IsNewRow)
                {

                    // HK : FC : BM : 09-12-2009. Fix Bug 132
                    // Read values in datagrid in variables using Convert methods
                    // and then output them to Debug.Print
                    // 
                    //iitemindex  = Convert.ToInt32(dtgrdPOLinesView.Rows[iLoopCounter].Index);
                    // HK : 11-12-2009 : Item Index is not on the datagrid. So we must use the 
                    // item index on the PO Items Collection
                    //iitemindex = _porder.lstpoLineItemDetails[iLoopCounter].Itemindex;
                    //iClass = Convert.ToInt16(dtgrdPOLinesView.Rows[iLoopCounter].Cells["Class"].Value);
                    //iVendor = Convert.ToInt32(dtgrdPOLinesView.Rows[iLoopCounter].Cells["Vendor"].Value);
                    //iStyle = Convert.ToInt16(dtgrdPOLinesView.Rows[iLoopCounter].Cells["Style"].Value);
                    //iColor = Convert.ToInt16(dtgrdPOLinesView.Rows[iLoopCounter].Cells["Color"].Value);
                    //iSize = Convert.ToInt16(dtgrdPOLinesView.Rows[iLoopCounter].Cells["Size"].Value);

                    //if (dtgrdPOLinesView.Rows[iLoopCounter].Cells[0].Value != null &&
                    //            Convert.ToBoolean(dtgrdPOLinesView.Rows[iLoopCounter].Cells[0].Value) == true)
                    if (1==1)
                    {

                        //Debug.Print("Data Grid row removed at index: " + iLoopCounter.ToString());

                        //Debug.Print("Class Code:" + iClass.ToString()
                        //              + "Vendor Code:" + iVendor.ToString()
                        //              + "Style Code:" + iStyle.ToString()
                        //              + "Color Code:" + iColor.ToString()
                        //              + "Size Code:" + iSize.ToString()
                        //              + "");

                        // HK : 30-11-2009 : The below RemoveAt will cause a row enter event to fire.
                        // As we delete the row at iLoopCounter, the nex valid row (if any) will become 
                        // the current row.
                        dgvPOlines.Rows.RemoveAt(iLoopCounter);

                        // HK : Bug : 70 : Remove the reference to this PO Line Item from 
                        // the POItems collection
                        //Debug.Print("About to remove item from collectioin. Collection index: " + iLoopCounter.ToString()
                        //                                  + "Class Code:" + _porder.lstpoLineItemDetails[iLoopCounter].Classcode.ToString()
                        //                                  + "Vendor Code:" + _porder.lstpoLineItemDetails[iLoopCounter].Vendorcode.ToString()
                        //                                  + "Style Code:" + _porder.lstpoLineItemDetails[iLoopCounter].Stylecode.ToString()
                        //                                  + "Color Code:" + _porder.lstpoLineItemDetails[iLoopCounter].Colorcode.ToString()
                        //                                  + "Size Code:" + _porder.lstpoLineItemDetails[iLoopCounter].Itemsize.ToString()
                        //                                  + "");
                        // ////////////////////////////////////////////////////////
                        // HK : 30-11-2009 : If Item is a APP then decrement the 
                        // pack count
                        // ///////////////////////////////////////////////////////
                        //if (_porder.NumofPOPacks >= 1)
                        //{
                        //    _porder.NumofPOPacks = _porder.NumofPOPacks - 1;
                        //}
                        // ////////////////////////////////////////////////////////

                        //_porder.lstpoLineItemDetails.RemoveAt(iLoopCounter);
                        //Debug.Print("Item Collection count: " + _porder.lstpoLineItemDetails.Count.ToString());

                        iRowsDeleted++;

                        iRunningCountTotalRows = dgvPOlines.Rows.Count;

                    }
                    //else
                    //{
                    //    //Debug.Print("Data Grid row skipped index: " + iitemindex.ToString());
                    //    //DisplayDataGridItems(iLoopCounter);
                    //    /*
                    //    Debug.Print("Class Code:" + iClass.ToString()
                    //                  + "Vendor Code:" + iVendor.ToString()
                    //                  + "Style Code:" + iStyle.ToString()
                    //                  + "Color Code:" + iColor.ToString()
                    //                  + "Size Code:" + iSize.ToString()
                    //                  + "");
                    //     */

                    //    iLoopCounter++;
                    //}
                }
                else
                {
                    iLoopCounter++;
                }

            } while (iLoopCounter < iRunningCountTotalRows);

            _bUserWantsToDeleteLine = false;

            // HK : 20-11-2009 : After all selected rows have been deleted and the 
            // items collection has been updated then calculate PO summary
            // Calculate Po Summary if any records were deleted.
            // Note : ASH originally did this from the RowsDeleteting event handler
            if (iRowsDeleted > 0)
            {
                //CalculatePOSummary();
            }

            // Display the contents of the grid annd the items collection to verify that they are consistent
            //Debug.Print("DataGrid Items after delete");
            //Debug.Print("=========================================");
            //DisplayDataGridItems();
            //Debug.Print("=========================================");

            //Debug.Print("Items collection after delete");
            //Debug.Print("=========================================");
            //DisplayItemCollection();
            //Debug.Print("=========================================");

        }

        private void txtPortofDeparture_Validating(object sender, CancelEventArgs e)
        {
            // HK : 01-12-2009 : Dont validate when user is trying to close the form
            if (!_bFormCancelClicked)
            {
                // No need to validate if the user is just tabbing out of the field
                // and has not changed anything.
                if (string.IsNullOrEmpty(txtPortofDeparture.Text))
                {
                    return;
                }

                if (_porder.Portofdeparturecode == Int32.Parse(txtPortofDeparture.Text))
                {
                    e.Cancel = false;
                    return;
                }

                List<string> lstretvalues = new List<string>();

                lstretvalues = validationcls.ValidatePort(txtPortofDeparture.Text);

                if (lstretvalues[0] == "True")
                {
                    //Populate the right data etc and 
                    lblDeparturePortDesc.Text = lstretvalues[1];
                    validationcls.HighlightErrControls(lblPortofDeparture, txtPortofDeparture, true);
                    errPOEntry.SetError(txtPortofDeparture, "");
                    e.Cancel = false;
                    _porder.Portofdeparturecode = Int32.Parse(txtPortofDeparture.Text);

                    // PO is now modified so enable the "PO Modify" button
                    btnCreatePO.Enabled = true;
                    IsPoModified = true;
                }
                else
                {
                    lblDeparturePortDesc.Text = "";
                    validationcls.HighlightErrControls(lblPortofDeparture, txtPortofDeparture, false);
                    errPOEntry.SetError(txtPortofDeparture, "Please enter a valid port");
                    e.Cancel = true;
                    _porder.Portofdeparturecode = 0;
                }
            }
        }

        private void txtPortofEntry_Validating(object sender, CancelEventArgs e)
        {
            // HK : 01-12-2009 : Dont validate when user is trying to close the form
            if (!_bFormCancelClicked)
            {
                // HK : 19-01-2010 : Fix Bug 254, 289
                // No need to validate if the user is just tabbing out of the field
                // and has not changed anything.
                if (_porder.Portofentrycode == Int32.Parse(txtPortofEntry.Text))
                {
                    Debug.Print("Txt Port of Entry field. Just tabbing out .....");
                    e.Cancel = false;
                    return;
                }

                List<string> lstretvalues = new List<string>();

                lstretvalues = validationcls.ValidatePort(txtPortofEntry.Text);

                if (lstretvalues[0] == "True")
                {
                    //Populate the right data etc and 
                    lblEntryPortDesc.Text = lstretvalues[1];
                    errPOEntry.SetError(txtPortofEntry, "");
                    validationcls.HighlightErrControls(lblPortofEntry, txtPortofEntry, true);
                    e.Cancel = false;
                    _porder.Portofentrycode = Int32.Parse(txtPortofEntry.Text);

                    // HK : 19-01-2010 : Fix Bug 254, 289
                    // PO is now modified so enable the "PO Modify" button
                    btnCreatePO.Enabled = true;
                    IsPoModified = true;
                }
                else
                {
                    lblEntryPortDesc.Text = "";
                    validationcls.HighlightErrControls(lblPortofEntry, txtPortofEntry, false);
                    errPOEntry.SetError(txtPortofEntry, "Please enter a valid port");
                    e.Cancel = true;
                }
            }
        }

        private void txtDelTerms_Validating(object sender, CancelEventArgs e)
        {
            // HK : 01-12-2009 : Dont validate when user is trying to close the form
            if (!_bFormCancelClicked)
            {
                // HK : 19-01-2010 : Fix Bug 254, 289
                // No need to validate if the user is just tabbing out of the field
                // and has not changed anything.
                if (_porder.Deltermscode == txtDelTerms.Text)
                {
                    Debug.Print("Txt Delivery Terms field. Just tabbing out .....");
                    e.Cancel = false;
                    return;
                }

                List<string> lstretvalues = new List<string>();

                lstretvalues = validationcls.ValidateDeliveryTerms(txtDelTerms.Text);

                if (lstretvalues[0] == "True")
                {

                    //Populate the right data etc and 
                    lblDeliveryTermsDesc.Text = lstretvalues[1];
                    errPOEntry.SetError(txtDelTerms, "");
                    validationcls.HighlightErrControls(lblDelTerms, txtDelTerms, true);
                    _porder.Deltermscode = txtDelTerms.Text;
                    e.Cancel = false;

                    // HK : 01-12-2009 : Resolve Bug 90
                    // HK : 02-12-2009 : Validate Header
                    ValidateHeaderFields(sender);

                    // HK : 19-01-2010 : Fix Bug 254, 289
                    // 
                    // PO is now modified so enable the "PO Modify" button
                    btnCreatePO.Enabled = true;
                    IsPoModified = true;
                }
                else
                {
                    //Populate the error
                    lblDeliveryTermsDesc.Text = "";
                    validationcls.HighlightErrControls(lblDelTerms, txtDelTerms, false);
                    errPOEntry.SetError(txtDelTerms, "Please enter a valid delivery term");
                    _porder.Deltermscode = "";
                    e.Cancel = true;
                    _porder.Deltermscode = "";
                }
            }
        }

        //private bool ValidateQuantity(string svalue, int packQty, int rowindex)
        //{
        //    bool bisValid = false;
        //    int itemqtyinput;
        //    // Fix Bug 94
        //    // HK : 11-01-2010 : Open the rounding form irrespective of 
        //    // whether the quantity entered is less than case pack quantity or not 
        //    //if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput) && itemqtyinput >= packQty )
        //    if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput))
        //    {
        //        if (itemqtyinput % packQty != 0)
        //        {
        //            // HK : 04-01-2010 : Must send the datagridview row index, otherwise it will not know 
        //            // which record to update
        //            ItemQuantityForm itemqtyform = new ItemQuantityForm(itemqtyinput, packQty, ref _polinedetails, rowindex);
        //            itemqtyform.OnQuantityRounded += new ItemQuantityForm.QuantityRoundedEventHandler(itemqtyform_OnQuantityRounded);
        //            if (itemqtyform.ShowDialog(this) == DialogResult.OK)
        //            {
        //                bisValid = true;
        //            }
        //            else
        //            {
        //                bisValid = false;
        //            }
        //        }
        //        else
        //        {
        //            bisValid = true;
        //        }
        //    }
        //    else
        //    {
        //        bisValid = false;
        //    }
        //    return bisValid;
        //}

        void itemqtyform_OnQuantityRounded(object sender, int iroundedquantity)
        {
            _itemquantityrounded = iroundedquantity;
        }

        // Sums the quantity assigned to each item for 
        // the store in question
        private int SumStoreQuantity(string sstore)
        {
            int     isummedquantity = 0;
            string  sstorecolumnname;

            // Convert passed store (sstore) to a valid store column name
            sstorecolumnname = GetStoreColumnNameFromStore(sstore);

            for (int i = 0; i < dtDropShipMatrix.Rows.Count; i++)
            {
                isummedquantity = isummedquantity + Convert.ToInt32(dtDropShipMatrix.Rows[i][sstorecolumnname]);
            }

            //isummedquantity = Convert.ToInt32 (dtDropShipMatrix.Compute("Sum (QuantityAssigned)", ""));

            return isummedquantity;
        }

        private string GetStoreColumnNameFromStore(string sstore)
        {
            return _sStoreColumnNamePrefix + sstore;
        }

        private int GetItemQuantityForStore(string sstore, short iClass, int iVendor, short iStyle, short iColor, short iSize)
        {
            int iTemp;
            int istorequantity = 0;
            string sfilterexpr;
            string sstorecolumnname = GetStoreColumnNameFromStore(sstore);

            sfilterexpr = "Class = '" + iClass.ToString() + "' and " +
                          "Vendor = '" + iVendor.ToString() + "' and " +
                          "Style = '" + iStyle.ToString() + "' and " +
                          "Color = '" + iColor.ToString() + "' and " +
                          "Size = '" + iSize.ToString() + "'";


            for (int i = 0; i < dtDropShipMatrix.Rows.Count; i++)
            {
                iTemp = Convert.ToInt32(dtDropShipMatrix.Rows[i][sstorecolumnname]);
                istorequantity = istorequantity + iTemp;
            }

            return istorequantity;
        }

        private void txtPortofDeparture_EnabledChanged(object sender, EventArgs e)
        {
            errPOEntry.Clear();
        }

        // Checks for duplicate items and return the 
        // first item found that is duplicate
        // The collection lstpoLineItemDetails will hold all the 
        // Po Line Items entered so far
        private Boolean CheckForDuplicateLine(short itemclass, int vendor, short style, short colour, string size, ref int rowid)
        {
            // HK ?? Could be implemented using Find and Findall for custom collection
            // on the class POItemDetails and class PurchaseOrder

            // Find will return the index of the found item
            // FinaAll will return a List<POItemDetails> of all items
            // that match

            return false;
        }

        // The collection lstpoLineItemDetails will hold all the 
        // Po Line Items entered so far
        private Boolean CheckForDuplicateLine(int itemindex, short itemclass, int vendor, short style, short colour, string size)
        {
            Boolean bSuccess = false;
            Int16 iClass;
            Int32 iVendor;
            Int16 iStyle;
            Int16 iColour;
            string ssize;
            //short iSize;

            foreach (POItemDetails item in _porder.lstpoLineItemDetails)
            {
                iClass = item.ClassCode;
                iVendor = item.Vendorcode;
                iStyle = item.Stylecode;
                iColour = item.Colorcode;
                ssize = item.Itemsize.ToString ();
                //iSize = item.;

                // Check against valid items only.
                // For a PO Item row that is being validated then the Size will be null 
                // as this method iscalled from the "Size" validation 
                if (item.IsValid && (item.Itemindex != itemindex))
                {
                    if (iClass == itemclass && iVendor == vendor && iStyle == style
                                            && iColour == colour && ssize.Equals(size))
                    {
                        bSuccess = true;
                    }
                }
            }

            return bSuccess;
        }

        //private void LoadCurrencyFromXMLDocument()
        //{
        //    _dtCurrency = null;
        //    _dtCurrency = new DataTable("Currency");
        //    _dtCurrency.Columns.Add("CurrencyCode", typeof(string));
        //    _dtCurrency.Columns.Add("CurrencyName", typeof(string));
        //    _dtCurrency.Columns.Add("CurrencyRate", typeof(decimal));
        //    _dtCurrency.Columns.Add("CurrencyCodeName", typeof(decimal));
        //}

        private void SetItemColor(int columnindex, int rowindex, Color color)
        {
            dgvPOlines["Class", rowindex].Style.BackColor = color;
            dgvPOlines["Vendor", rowindex].Style.BackColor = color;
            dgvPOlines["Style", rowindex].Style.BackColor = color;
            dgvPOlines["Color", rowindex].Style.BackColor = color;
            dgvPOlines["Size", rowindex].Style.BackColor = color;
        }

        private Boolean ValidateHeaderFields (object sender)
        {
            TextBox txtBox = (TextBox)sender;

            Debug.Print("Type: " + txtBox.GetType().ToString ());

            if (txtBox.Name == "txtCurrency")
            {
                // Department
                if (String.IsNullOrEmpty(txtDept.Text))
                {
                    return false;
                }

                // Vendor
                if (String.IsNullOrEmpty(txtVendor.Text))
                {
                    return false;
                }

                // Currency
                if (String.IsNullOrEmpty(txtCurrency.Text))
                {
                    return false;
                }

                // Because certain vendors put a default ship via code
                // Ship Via
                if (String.IsNullOrEmpty(txtShipVia.Text))
                {
                    return false;
                }

                // If the ShipVia was populated annd it was not OCN the 
                // header is valid
                if (txtShipVia.Text != "OCN" && !String.IsNullOrEmpty(txtShipVia.Text))
                {
                    dgvPOlines.Enabled = true;
                    return true;
                }
            }

            if (txtBox.Name == "txtShipVia")
            {
                if (String.IsNullOrEmpty(txtShipVia.Text))
                {
                    return false;
                }

                if (txtShipVia.Text != "OCN" && !String.IsNullOrEmpty(txtShipVia.Text))
                {
                    dgvPOlines.Enabled = true;
                    return true;
                }
            }

            if (txtBox.Name == "txtDelTerms")
            {
                if (String.IsNullOrEmpty(txtLanding.Text))
                {
                    return false;
                }

                // Port of departure
                if (String.IsNullOrEmpty(txtPortofDeparture.Text))
                {
                    return false;
                }


                // Port of entry
                if (String.IsNullOrEmpty(txtPortofEntry.Text))
                {
                    return false;
                }


                // Delivery Terms
                if (String.IsNullOrEmpty(txtDelTerms.Text))
                {
                    return false;
                }

                //if (_porder.Penvironment.Domain == "TDSNA")
                if (_porder.Penvironment.Domain == "SWNA")
                {
                    if (String.IsNullOrEmpty(cmbSSD.Text))
                    {
                        return false;
                    }
                }

                dgvPOlines.Enabled = true;

                return true;
            }

            return false;
        }

        private void SetupClassObjects()
        {
            _porder = new PurchaseOrder(_dbparamref, _username, _paramenv);
            _porder.GetPOHeader(_spiceponumber);

            // Get the comments (both vendor and internal)
            _porder.GetPOComments(_spiceponumber, _porder.SpicePOversion);

            // Get the PO Line Details
            _dtPoLines = _porder.GetPOItems(_spiceponumber, _porder.SpicePOversion);
        }

        private void SetupSimpleDataBinding()
        {
            Boolean bSuccess;

            // Setup simple DataBinding
            //txtDept.DataBindings.Add("Text", _porder, "Department", false, DataSourceUpdateMode.OnPropertyChanged);

            ItemsBO.Items itembo = new Items(_dbparamref, _username, _paramenv);

            itembo.GetMarket(_porder.DefaultMarket);

            // Assign the proper market description
            lblMarketValue.Text = _porder.DefaultMarket + "-" + itembo.MarketName; //  _porder.MarketDescription;

            // Display the Currency of the PO in the PO Summary Area
            _porder.MarketCurrency = itembo.MarketCurrency;
            lblCurrVal1.Text = "(" + _porder.MarketCurrency + ")";
            lblCurrValue.Text = "(" + _porder.MarketCurrency + ")";

            // HK : CJ : 10-12-2009 : The above method GetMarket will initalise the property itembo.MarketCurrency
            // which is the currency code of the market.
            itembo.GetCurrency(itembo.MarketCurrency);

            // Get the Market Rate of the currency
            _currencyratemarket = itembo.CurrencyRate;

            txtDept.Text = _porder.Deptcode.ToString ();
            itembo.GetDepartment(_porder.Deptcode);
            lblDeptDesc.Text = itembo.DepartmentName;
            
            txtVendor.Text = _porder.Vendorcode.ToString();
            itembo.GetVendor(_porder.Vendorcode);
            lblVendorDesc.Text = itembo.VendorName;

            txtCurrency.Text = _porder.Currencycode;
            itembo.GetCurrency(_porder.Currencycode);
            
            // Currency rate of PO
            //_currencyratepo = _porder.ExchangeRate;
            
            lblCurrencyDesc.Text = itembo.CurrencyName + " (" + _porder.ExchangeRate + ")";

            // HK : 10-12-2009 : Terms code is null. Ask CJ
            txtTerms.Text = _porder.Termscode;
            bSuccess = itembo.GetTerms(_porder.Termscode);
            if (bSuccess == true)
            {
                lblTermsDesc.Text = itembo.VendorTermsDescription;
            }

            cmbShipTo.Items.Add(_porder.ShipTo.ToString());
            cmbShipTo.Text = _porder.ShipTo.ToString();

            txtShipVia.Text = _porder.ShipViaCode;
            itembo.GetShipVia(_porder.ShipViaCode);
            lblShipViaDesc.Text = itembo.ShipViaDescription;
            
            // If Landing = 1 it means that the default landing (of 1) 
            // used. This need not be shown. If landing = 1.1 or 1.2 etc then we must 
            // show the remainder part of the landing. So in case the landing is 1.2, 
            // the landing textbox should show 0.2. If it is 1.3 the kanding textbox 
            // should show 1.3
            decimal idisplaylandingfactor = _porder.Landing - 1;

            //txtLanding.Text = Math.Round (_porder.Landing, 2).ToString();
            txtLanding.Text = Math.Round(idisplaylandingfactor, 2).ToString();
            
            txtPortofDeparture.Text = _porder.Portofdeparturecode.ToString ();
            itembo.GetPort(_porder.Portofdeparturecode);
            lblDeparturePortDesc.Text = itembo.PortDescription;
            
            txtPortofEntry.Text = _porder.Portofentrycode.ToString(); ;
            itembo.GetPort(_porder.Portofentrycode);
            lblEntryPortDesc.Text = itembo.PortDescription;
            
            txtDelTerms.Text = _porder.Deltermscode;
            itembo.GetDelTerms (_porder.Deltermscode);
            lblDeliveryTermsDesc.Text = itembo.DelTermsDescription;

            // HK : 16-12-2009 : Enable disable the controls relating to 
            // port of departure, port of entry and delivery terms
            ImportControlChanges();

            dtpkrShipDate.Value = _porder.ShippingDate;
            
            // HK : FC : CJONES : 23-12-2009 : For some unknown reason the anticipate date
            // is being set to 01/01/01 during the Create Po process. The calendar picker 
            // will not like this date. So we must default to MinDate (01/01/1753)
            if (_porder.AnticipateDate.Year < 1753)
            {
                dtpkrAnticipateDate.Value = dtpkrAnticipateDate.MinDate;
                MessageBox.Show("Anticipate Date got from the database is less than 01/01/1753." +
                                 "\r\n\r\n The Anticipate Date wil now be defaulted to 01/01/1753", "PO Modification",
                                 MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                dtpkrAnticipateDate.Value = _porder.AnticipateDate;
            }
            
            txtOrderDate.Text = _porder.OrderDate.ToLongDateString ();
            txtCancelDate.Text = _porder.CancelDate.ToLongDateString();

            if (_porder.Penvironment.Domain == "SWNA")
            {
                int i = cmbSSD.FindStringExact(_porder.Ssd.ToLongDateString());
                cmbSSD.SelectedIndex = i;
            }

            txtVendorComment1.Text = _porder.Vendorcomments1;
            txtVendorComment2.Text = _porder.Vendorcomments2;
            txtVendorComment3.Text = _porder.Vendorcomments3;

            txtInternalComments1.Text = _porder.Internalcomments1;
            txtInternalComments2.Text = _porder.Internalcomments2;

            lblPONumber.Text = _porder.SpicePOnumber;

            // HK : 18-12-2009 : Freight
            if (_porder.Penvironment.Domain == "SWNA")
            {
                cbxFreight.SelectedIndex = cbxFreight.FindStringExact(GetFreightDesc(_porder.Freight));
            }

            // HK : 13-01-2010 : Fix Bug 239, 240
            if (String.IsNullOrEmpty(_porder.IPPOnumber.Trim ()) == false)
            {
                lblIP.Visible = true;
                lblIPPoNumber.Text = _porder.IPPOnumber;
            }
            
            // Now add the handler for CheckedChanged
            chkNewLineSelection.Checked = _porder.IsPONewLine;
            this.chkNewLineSelection.CheckedChanged += new System.EventHandler(this.chkNewLineSelection_CheckedChanged);
            
            _bDataBindingsInitalised = true;

            DisplayPoItems();
        }

        private void DisplayOriginalPoHeader()
        {
            Boolean bSuccess;

            // Setup simple DataBinding
            //txtDept.DataBindings.Add("Text", _porder, "Department", false, DataSourceUpdateMode.OnPropertyChanged);

            ItemsBO.Items itembo = new Items(_dbparamref, _username, _paramenv);

            // HK : CJ : 10-12-2009 : We assume that _porder.DefaultMarket 
            // holds the market selected by the user on the market selection form 
            // when creating a new PO 
            itembo.GetMarket(_porder.DefaultMarket);

            // Assign the proper market description
            lblMarketValue.Text = _porder.DefaultMarket + "-" + itembo.MarketName; //  _porder.MarketDescription;

            // HK : 11-12-2009 : 
            // Display the Currency of the PO in the PO Summary Area
            _porder.MarketCurrency = itembo.MarketCurrency;
            lblCurrVal1.Text = "(" + _porder.MarketCurrency + ")";
            lblCurrValue.Text = "(" + _porder.MarketCurrency + ")";

            // HK : CJ : 10-12-2009 : The above method GetMarket will initalise the property itembo.MarketCurrency
            // which is the currency code of the market.
            itembo.GetCurrency(itembo.MarketCurrency);

            // Get the Market Rate of the currency
            _currencyratemarket = itembo.CurrencyRate;

            txtDept.Text = _porder.Deptcode.ToString();
            itembo.GetDepartment(_porder.Deptcode);
            lblDeptDesc.Text = itembo.DepartmentName;

            txtVendor.Text = _porder.Vendorcode.ToString();
            itembo.GetVendor(_porder.Vendorcode);
            lblVendorDesc.Text = itembo.VendorName;

            txtCurrency.Text = _porder.Currencycode;
            itembo.GetCurrency(_porder.Currencycode);

            // Currency rate of PO
            //_currencyratepo = _porder.ExchangeRate;

            lblCurrencyDesc.Text = itembo.CurrencyName + " (" + _porder.ExchangeRate + ")";

            // HK : 10-12-2009 : Terms code is null. Ask CJ
            txtTerms.Text = _porder.Termscode;
            bSuccess = itembo.GetTerms(_porder.Termscode);
            if (bSuccess == true)
            {
                lblTermsDesc.Text = itembo.VendorTermsDescription;
            }

            txtShipVia.Text = _porder.ShipViaCode;
            itembo.GetShipVia(_porder.ShipViaCode);
            lblShipViaDesc.Text = itembo.ShipViaDescription;

            // HK : 16-12-2009 : If Landing = 1 it means that the default landing (of 1) 
            // used. This need not be shown. If landing = 1.1 or 1.2 etc then we must 
            // show the remainder part of the landing. So in case the landing is 1.2, 
            // the landing textbox should show 0.2. If it is 1.3 the kanding textbox 
            // should show 1.3
            decimal idisplaylandingfactor = _porder.Landing - 1;

            //txtLanding.Text = Math.Round (_porder.Landing, 2).ToString();
            txtLanding.Text = Math.Round(idisplaylandingfactor, 2).ToString();

            txtPortofDeparture.Text = _porder.Portofdeparturecode.ToString();
            itembo.GetPort(_porder.Portofdeparturecode);
            lblDeparturePortDesc.Text = itembo.PortDescription;

            txtPortofEntry.Text = _porder.Portofentrycode.ToString(); ;
            itembo.GetPort(_porder.Portofentrycode);
            lblEntryPortDesc.Text = itembo.PortDescription;

            txtDelTerms.Text = _porder.Deltermscode;
            itembo.GetDelTerms(_porder.Deltermscode);
            lblDeliveryTermsDesc.Text = itembo.DelTermsDescription;

            // HK : 16-12-2009 : Enable disable the controls relating to 
            // port of departure, port of entry and delivery terms
            ImportControlChanges();

            dtpkrShipDate.Value = _porder.ShippingDate;

            // HK : FC : CJONES : 23-12-2009 : For some unknown reason the anticipate date
            // is being set to 01/01/01 during the Create Po process. The calendar picker 
            // will not like this date. So we must default to MinDate (01/01/1753)
            if (_porder.AnticipateDate.Year < 1753)
            {
                dtpkrAnticipateDate.Value = dtpkrAnticipateDate.MinDate;
                MessageBox.Show("Anticipate Date got from the database is less than 01/01/1753." +
                                 "\r\n\r\n The Anticipate Date wil now be defaulted to 01/01/1753", "PO Modification",
                                 MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                dtpkrAnticipateDate.Value = _porder.AnticipateDate;
            }

            txtOrderDate.Text = _porder.OrderDate.ToLongDateString();
            txtCancelDate.Text = _porder.CancelDate.ToLongDateString();

            if (_porder.Penvironment.Domain == "SWNA")
            {
                int i = cmbSSD.FindStringExact(_porder.Ssd.ToLongDateString());
                cmbSSD.SelectedIndex = i;
                //cmbSSD.Text = _porder.Ssd.ToLongDateString ();
            }

            txtVendorComment1.Text = _porder.Vendorcomments1;
            txtVendorComment2.Text = _porder.Vendorcomments2;
            txtVendorComment3.Text = _porder.Vendorcomments3;

            txtInternalComments1.Text = _porder.Internalcomments1;
            txtInternalComments2.Text = _porder.Internalcomments2;

            lblPONumber.Text = _porder.SpicePOnumber;

            // HK : 18-12-2009 : Freight
            if (_porder.Penvironment.Domain == "SWNA")
            {
                cbxFreight.SelectedIndex = cbxFreight.FindStringExact(GetFreightDesc(_porder.Freight));
            }

            lblIPPoNumber.Text = _porder.IPPOnumber;

            _bDataBindingsInitalised = true;
        }

        private void DisplayOriginalPoItems()
        {
            int iStartRow;
            int iTotalRowsInGrid;

            // Add the items to the datagrid and setup the collections list
            if (_porder.lstpoLineItemDetails.Count > 0)
            {
                iStartRow = iTotalRowsInGrid = dgvPOlines.Rows.Count;

                foreach (POItemDetails poitem in _porder.lstpoLineItemDetails)
                {
                    // Insert the row and populate the Pk values. 
                    // Also make sure the Check Box in the first column is un checked

                    iStartRow = dgvPOlines.Rows.Add(false, poitem.ClassCode,
                                                                        poitem.Vendorcode,
                                                                        poitem.Stylecode,
                                                                        poitem.Colorcode,
                                                                        poitem.Itemsize);

                    // Lookup the item to populate the class with the rest of the details
                    if (poitem.IsValid == true)
                    {
                        dgvPOlines.Rows[iStartRow].Cells["Description"].Value = poitem.Itemlongdescription;
                        dgvPOlines.Rows[iStartRow].Cells["Retail"].Value = poitem.Retailprice.ToString();
                        dgvPOlines.Rows[iStartRow].Cells["Cost"].Value = poitem.Cost.ToString();
                        dgvPOlines.Rows[iStartRow].Cells["Character"].Value = poitem.Characterdesc;
                        dgvPOlines.Rows[iStartRow].Cells["Season"].Value = poitem.SeasonDesc;
                        dgvPOlines.Rows[iStartRow].Cells["CasePackType"].Value = poitem.Packdescription;
                        dgvPOlines.Rows[iStartRow].Cells["TicketType"].Value = poitem.Tickettype;
                        //This will determine if the qty and cost can be changed 
                        dgvPOlines.Rows[iStartRow].Cells["Pack"].Value = poitem.APP1;

                        // Display the quantity and assign this quantity to the PO Line Item object
                        dgvPOlines.Rows[iStartRow].Cells["Quantity"].Value = poitem.Itemquantity;
                        //dgvPOlines.Rows[iStartRow].Cells["Sequence"].Value = iStartRow;  // CJ woz 'ere
                        dgvPOlines.Rows[iStartRow].Cells["Sequence"].Value = poitem.Sequence;
                        // Display Converted cost and hide actual cost from database

                        dgvPOlines.Rows[iStartRow].Cells["ConvertedCost"].Value = Decimal.Round((poitem.Cost * _currencyratemarket) / _porder.ExchangeRate, 2);

                        if (poitem.APP1 == "Y")
                        {
                            //Make the UnitCost Readonly
                            dgvPOlines.Rows[iStartRow].Cells["Cost"].ReadOnly = true;
                        }
                        else
                        {
                            //Not an APP
                            //Enable cost
                            dgvPOlines.Rows[iStartRow].Cells["Cost"].ReadOnly = false;
                        }

                        // HK : 22-12-2009 : Make the entire row readonly
                        dgvPOlines.Rows[iStartRow].ReadOnly = true;
                    }
                }
            }

            // Now calculate the PO Summary
            CalculatePOSummary();
        }

        private void DisplayPoItems()
        {
            Int16 iClass;
            Int32 iVendor;
            Int16 iStyle;
            Int16 iColour;
            Int16 iSize;

            int iStartRow;
            int iTotalRowsInGrid;

            char padchar = Convert.ToChar(" ");
            Boolean bItemLookupError = false;

            DataTable dtComponents;

            // Add the items to the datagrid and setup the collections list
            if (_dtPoLines.Rows.Count > 0)
            {
                iStartRow = iTotalRowsInGrid = dgvPOlines.Rows.Count;

                foreach (DataRow dr in _dtPoLines.Rows)
                {
                    // Insert the row and populate the Pk values. 
                    // Also make sure the Check Box in the first column is un checked

                    iClass = (Int16)dr["Class"];

                    iStartRow = dgvPOlines.Rows.Add(false, dr["class"],
                                                           dr["vendor"],
                                                           dr["style"],
                                                           dr["colour"],
                                                           dr["size"]);

                    // Create a new item for the POItemDetails collection
                    POItemDetails poitem = new POItemDetails((short)dr["class"], (int)dr["vendor"],
                                                             (short)dr["style"], (short)dr["colour"],
                                                             (short)dr["size"],  iStartRow + 1);

                    // ClassName
                    List<string> retValues = validationcls.ValidateClass(poitem.ClassCode.ToString());
                    poitem.Classname = retValues[1].ToString();

                    // Vendor
                    retValues = validationcls.ValidateVendor(poitem.Vendorcode.ToString(), true);
                    poitem.Vendordesc = retValues[1].ToString();

                    // Color
                    retValues = validationcls.ValidateColour(poitem.Colorcode.ToString());
                    poitem.Colordesc = retValues[1];

                    // Size
                    retValues = validationcls.ValidateSize(poitem.Itemsize.ToString());
                    poitem.Sizename = retValues[1];

                    // Lookup the item to populate the class with the rest of the details
                    if (poitem.ItemLookup(_porder.DbParamRef, _porder.UserName, _porder.Penvironment, _porder.DefaultMarket))
                    {
                        dgvPOlines.Rows[iStartRow].Cells["Description"].Value     = poitem.Itemlongdescription;
                        //dgvPOlines.Rows[iStartRow].Cells["Retail"].Value          = poitem.Retailprice.ToString();
                        //dgvPOlines.Rows[iStartRow].Cells["Cost"].Value            = poitem.Cost.ToString();
                        dgvPOlines.Rows[iStartRow].Cells["Character"].Value       = poitem.Characterdesc;
                        dgvPOlines.Rows[iStartRow].Cells["Season"].Value          = poitem.SeasonDesc;
                        dgvPOlines.Rows[iStartRow].Cells["CasePackType"].Value    = poitem.Packdescription;
                        dgvPOlines.Rows[iStartRow].Cells["TicketType"].Value      = poitem.Tickettype;
                        //This will determine if the qty and cost can be changed 
                        dgvPOlines.Rows[iStartRow].Cells["Pack"].Value            = poitem.APP1;

                        Decimal retail = Convert.ToDecimal(_dtPoLines.Rows[iStartRow]["Retail"]);
                        dgvPOlines.Rows[iStartRow].Cells["Retail"].Value = retail;
                        poitem.Retailprice = retail;

                        Int32 qty = Convert.ToInt32(_dtPoLines.Rows[iStartRow]["Quantity"]);
                        dgvPOlines.Rows[iStartRow].Cells["Quantity"].Value = qty;
                        poitem.Itemquantity = qty;

                        Int16 sequence = Convert.ToInt16(_dtPoLines.Rows[iStartRow]["Sequence"]);
                        dgvPOlines.Rows[iStartRow].Cells["sequence"].Value = sequence;  // CJ woz 'ere
                        poitem.Sequence = sequence;


                        decimal SVcost = Convert.ToDecimal(_dtPoLines.Rows[iStartRow]["VendorCost"]);
                        poitem.ConvertedCost = SVcost;
                        dgvPOlines.Rows[iStartRow].Cells["ConvertedCost"].Value = SVcost;

                        //decimal cost = SVcost / _currencyratemarket * _currencyratepo;
                        decimal cost = SVcost * _porder.ExchangeRate;
                        poitem.Cost = cost;
                        dgvPOlines.Rows[iStartRow].Cells["Cost"].Value = cost;

                        decimal landedcost = decimal.Round(cost * _porder.Landing, 2);
                        poitem.LandedCost = landedcost;
                        dgvPOlines.Rows[iStartRow].Cells["LandedCost"].Value = landedcost;


                        if (poitem.APP1 == "Y")
                        {
                            dgvPOlines.Rows[iStartRow].Cells["ConvertedCost"].ReadOnly = true;
                        }
                        else
                        {
                            dgvPOlines.Rows[iStartRow].Cells["ConvertedCost"].ReadOnly = false;
                        }

                        if (poitem.APP1 == "Y")
                        {
                            AssortedPrePack appbo = new AssortedPrePack(poitem, _porder);

                            dtComponents = appbo.PopulateAPPComponents(_spiceponumber, _porder.SpicePOversion, poitem.Sequence);
                            PopulateComponents(poitem, dtComponents);
                        }

                        _porder.lstpoLineItemDetails.Add(poitem);
                    }
                    else
                    {
                        //_polinedetails.Isvalid = false;

                        // Check before implementing
                        poitem.IsValid = false;

                        // HK : 12-01-2010 : FC has managed to get an item into the PO Lines
                        // event though it does not exist in the database.
                        bItemLookupError = true;
                        
                        iClass      = poitem.ClassCode;
                        iVendor     = poitem.Vendorcode;
                        iStyle      = poitem.Stylecode;
                        iColour     = poitem.Colorcode;
                        iSize       = poitem.Itemsize;

                        MessageBox.Show("Item Lookup failed on : "
                              + "\r\n"
                              + "Item Index: "     + poitem.Itemindex.ToString().PadRight(6, padchar)
                              + "\r\n"
                              + "\r\n"
                              + "Class Code: "     + iClass.ToString().PadRight(6, padchar)
                              + "\r\n"
                              + "\r\n"
                              + "Vendor Code: "    + iVendor.ToString().PadRight(6, padchar)
                              + "\r\n"
                              + "\r\n"
                              + "Style Code: "     + iStyle.ToString().PadRight(6, padchar)
                              + "\r\n"
                              + "\r\n"
                              + "Color Code: "     + iColour.ToString().PadRight(6, padchar)
                              + "\r\n"
                              + "\r\n"
                              + "Size Code: "      + iSize.ToString().PadRight(6, padchar)
                              + "", "Spice - PO Modification");
                    }
                }
            }

            CalculatePOSummary();
        }

        // Populate the POComponents collection of POItemDetails
        // for each component in the PO Item
        private void PopulateComponents(POItemDetails poitemdetails, DataTable dtInitialLoad)
        {
            Validation validationcls = new Validation(_porder.DbParamRef, _porder.UserName, _porder.Penvironment);

            for (int i = 0; i < dtInitialLoad.Rows.Count; i++)
            {
                APPcomponent component = new APPcomponent();
                //pocomponent.ItemQtyChanged += new POItemDetails.delItemQtyChanged(pocomponent_ItemQtyChanged);

                component.ComponentClass  = (Int16)dtInitialLoad.Rows[i]["ComponentClass"];
                component.ComponentVendor = (Int32)dtInitialLoad.Rows[i]["ComponentVendor"];
                component.ComponentStyle  = (Int16)dtInitialLoad.Rows[i]["ComponentStyle"];
                component.ComponentColour = (Int16)dtInitialLoad.Rows[i]["ComponentColour"];
                component.ComponentSize   = (Int16)dtInitialLoad.Rows[i]["ComponentSize"];
                component.RatioQuantity   = (Int16)dtInitialLoad.Rows[i]["ComponentQuantity"];
                component.Cost            = (Decimal)dtInitialLoad.Rows[i]["ComponentCost"];
                component.Retail          = (Decimal)dtInitialLoad.Rows[i]["Retail"];

                //component.ItemDescription = validationcls.GetItemDescription(component.ComponentClass, component.ComponentVendor, component.ComponentStyle, component.ComponentColour, component.ComponentSize, _porder.DefaultMarket);
                component.ItemDescription = (String)dtInitialLoad.Rows[i]["ComponentLongDesc"];

                poitemdetails.Components.Add(component);

                // Item description
     //           pocomponent.ItemDescription = Description;


                //// ClassName
                //List<string> retValues = validationcls.ValidateClass(pocomponent.ComponentClass.ToString());
                //pocomponent.ComponentClass = retValues[1].ToString();

                //// Vendor
                //retValues = validationcls.ValidateVendor(pocomponent.ComponentVendor.ToString(), true);
                //pocomponent.Vendordesc = retValues[1].ToString();

                //// Color
                //retValues = validationcls.ValidateColour(pocomponent.ComponentColour.ToString());
                //pocomponent.Colordesc = retValues[1];

                //// Size
                //retValues = validationcls.ValidateSize(pocomponent.ComponentSize.ToString());
                //pocomponent.Sizename = retValues[1];

                //This function assumes the reqd fields have populated
                //pocomponent.ItemLookup(_porder.DbParamRef, _porder.UserName, _porder.Penvironment, _porder.DefaultMarket);

                // Apply the ItemQuantity to the PO Component ( PO Item Details) business object 
                //pocomponent.Itemquantity = (short)dtInitialLoad.Rows[i]["ComponentQuantity"];

                // HK : 18-01-2010 : 
                //pocomponent.Cost = (decimal)dtInitialLoad.Rows[i]["ComponentCost"];
                //pocomponent.ConvertedCost = (decimal)dtInitialLoad.Rows[i]["ComponentCost"];
                //pocomponent.LandedCost = Decimal.Round((pocomponent.Cost * _currencyratemarket) * _porder.Landing, 2);
                //pocomponent.LandedCost = (pocomponent.Cost * _currencyratemarket) * _porder.Landing;
                // Since we are loopin through the datatable in sequential order starting from 0
                // we just use the Add method instead of InsertAt
            }
        }

        //void pocomponent_ItemQtyChanged(int qty, decimal cost, int rowindex)
        //{
        //    {
        //        // HK : 14-11-2009 : Dummy event so that ASH's stuff does not 
        //        // fall over
        //    }
        //}
        //private void DisplayDataGridItems(int rowindex)
        //{
        //    char padchar = Convert.ToChar(" ");
        //    int iitemindex;
        //    short iClass;
        //    int iVendor;
        //    short iStyle;
        //    short iColor;
        //    short iSize;
        //    String description;
        //    // Try and convert Class, Vendor, Style, Color, Size
        //    iitemindex = rowindex;
        //    Int16.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[rowindex].Cells["Class"].Value), out iClass);
        //    Int32.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[rowindex].Cells["Vendor"].Value), out iVendor);
        //    Int16.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[rowindex].Cells["Style"].Value), out iStyle);
        //    Int16.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[rowindex].Cells["Color"].Value), out iColor);
        //    Int16.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[rowindex].Cells["Size"].Value), out iSize);
        //    description = Convert.ToString(dtgrdPOLinesView.Rows[rowindex].Cells["Description"].Value);
        //    if (dtgrdPOLinesView.Rows[rowindex].IsNewRow == false)
        //    {
        //    }
        //}
        //private void DisplayDataGridItems()
        //{
        //    char padchar = Convert.ToChar(" ");
        //    int iitemindex;
        //    short iClass;
        //    int iVendor;
        //    short iStyle;
        //    short iColor;
        //    short iSize;
        //    String description;
        //    // HK : 12-01-2010 : Fix Bug 213. Need to do a TryParse to get values from the datagrid cells
        //    // into the variables.
        //    for (int i = 0; i < dgvPOlines.Rows.Count; i++)
        //    {
        //        if (dgvPOlines.Rows[i].IsNewRow == false)
        //        {
        //            iitemindex = dgvPOlines.Rows[i].Index;
        //            // Try and convert Class, Vendor, Style, Color, Size
        //            Int16.TryParse(Convert.ToString(dgvPOlines.Rows[i].Cells["Class"].Value), out iClass);
        //            Int32.TryParse(Convert.ToString(dgvPOlines.Rows[i].Cells["Vendor"].Value), out iVendor);
        //            Int16.TryParse(Convert.ToString(dgvPOlines.Rows[i].Cells["Style"].Value), out iStyle);
        //            Int16.TryParse(Convert.ToString(dgvPOlines.Rows[i].Cells["Color"].Value), out iColor);
        //            Int16.TryParse(Convert.ToString(dgvPOlines.Rows[i].Cells["Size"].Value), out iSize);
        //            description = Convert.ToString(dgvPOlines.Rows[i].Cells["Description"].Value);
        //        }
        //    }
        //}
        //private void DisplayLineItemDetails(POItemDetails item)
        //    {
        //        char padchar = Convert.ToChar(" ");
        //    }
        //private void DisplayItemCollection(int collectionindex)
        //{
        //    char padchar = Convert.ToChar(" ");
        //    if ( collectionindex < _porder.lstpoLineItemDetails.Count)
        //    {
        //    }
        //}
        //private void DisplayItemCollection()
        //{
        //    char padchar = Convert.ToChar(" ");
        //    for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
        //    {
        //    }
        //}

        // HK : CJ : 10-12-2009 : Get the sequence number 
        // for the new record
        private short GetNextSequenceNumber()
        {
            int totalrowsinitemslist;
            short sequencenumberonlastitem;
            // HK : Sequence number always start at 1
            short nextsequencenumber = 1;

            // Get the last item in list
            totalrowsinitemslist = _porder.lstpoLineItemDetails.Count;

            if (_porder.lstpoLineItemDetails.Count > 0)
            {
                sequencenumberonlastitem = _porder.lstpoLineItemDetails[totalrowsinitemslist - 1].Sequence;

                if (sequencenumberonlastitem >= 1)
                {
                    sequencenumberonlastitem++;
                    nextsequencenumber = sequencenumberonlastitem;
                }
            }

            return nextsequencenumber;
        }

        private short GetMaxSequenceNumber()
        {
            // HK : 26-01-2010 : Get the next sequence number.
            // The next sequence number is the maximum sequence number + 1

            short nextsequencenumber = 1;
            short tempsequencenumber;

            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                tempsequencenumber = _porder.lstpoLineItemDetails[i].Sequence;

                if (tempsequencenumber > nextsequencenumber)
                {
                    nextsequencenumber = tempsequencenumber;
                }
            }

            return nextsequencenumber;
        }

        private string GetFreightId(int index)
        {
            switch (index)
            {
                case 0:
                    return "";

                case 1:
                    return "P";

                case 2:
                    return "C";

                default:
                    return "";
            }

        }

        private string GetFreightDesc(string freightid)
        {
            switch (freightid)
            {
                case "P":
                    return "Pre Pay";

                case "C":
                    return "Collect";

                case "":
                    return "      ";

                default:
                    return "      ";
            }
        }

        private void cbxFreight_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxFreight.SelectedItem != null)
            {
                _porder.Freight = GetFreightId(cbxFreight.SelectedIndex);
            }
        }

        private Boolean IsOkToDoItemLookup(int itemindex, short itemclass, int vendor, short style, short color, short size)
        {
            // Ignore ItemIndex for the time being
            if (IsItemClassChanged ||
                 IsItemVendorChanged ||
                 IsItemStyleChanged ||
                 IsItemColorChanged ||
                 IsItemSizeChanged)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void UpdatePoLinesPacks()
        {
            int numofpolines = 0;
            int numofpacks = 0;

            foreach (POItemDetails poitem in _porder.lstpoLineItemDetails)
            {
                if (poitem.APP1 == "Y" && poitem.Itemquantity > 0)
                {
                    numofpacks++;
                }

                if (poitem.Itemquantity > 0)
                {
                    numofpolines++;
                }
            }

            txtPOLines.Text = numofpolines.ToString();
            txtPOPacks.Text = numofpacks.ToString();
        }

        private int GetCollectionIndexForItem(int sequence)
        {
            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                if (_porder.lstpoLineItemDetails[i].Sequence == sequence)
                {
                    return i;
                }
            }

            return -1;
        }

        #region Comments
        private void txtVendorComment1_Validating(object sender, CancelEventArgs e)
        {
            if (_porder.Vendorcomments1 == txtVendorComment1.Text)
            {
                e.Cancel = false;
                return;
            }

            _porder.Vendorcomments1 = txtVendorComment1.Text;

            btnCreatePO.Enabled = true;
            IsPoModified = true;
        }

        private void txtVendorComment2_Validating(object sender, CancelEventArgs e)
        {
            // No need to validate if the user is just tabbing out of the field
            // and has not changed anything.
            if (_porder.Vendorcomments2 == txtVendorComment2.Text)
            {
                e.Cancel = false;
                return;
            }

            _porder.Vendorcomments2 = txtVendorComment2.Text;

            // PO is now modified so enable the "PO Modify" button
            btnCreatePO.Enabled = true;
            IsPoModified = true;
        }

        private void txtVendorComment3_Validating(object sender, CancelEventArgs e)
        {
            // No need to validate if the user is just tabbing out of the field
            // and has not changed anything.
            if (_porder.Vendorcomments3 == txtVendorComment3.Text)
            {
                e.Cancel = false;
                return;
            }

            _porder.Vendorcomments3 = txtVendorComment3.Text;

            // PO is now modified so enable the "PO Modify" button
            btnCreatePO.Enabled = true;
            IsPoModified = true;
        }

        private void txtInternalComments1_Validating(object sender, CancelEventArgs e)
        {
            // No need to validate if the user is just tabbing out of the field
            // and has not changed anything.
            if (_porder.Internalcomments1 == txtInternalComments1.Text)
            {
                e.Cancel = false;
                return;
            }

            _porder.Internalcomments1 = txtInternalComments1.Text;

            // PO is now modified so enable the "PO Modify" button
            btnCreatePO.Enabled = true;
            IsPoModified = true;
        }

        private void txtInternalComments2_Validating(object sender, CancelEventArgs e)
        {
            // No need to validate if the user is just tabbing out of the field
            // and has not changed anything.
            if (_porder.Internalcomments2 == txtInternalComments2.Text)
            {
                e.Cancel = false;
                return;
            }
            
            _porder.Internalcomments2 = txtInternalComments2.Text;

            // PO is now modified so enable the "PO Modify" button
            btnCreatePO.Enabled = true;
            IsPoModified = true;
        }
        #endregion

        #region Buttons
        private void btnRollback_Click(object sender, EventArgs e)
        {

            MessageBox.Show("This feature is not available in the current release!",
                                            "PO Modification",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);
            /*
            DialogResult dlgResult;

            dlgResult = MessageBox.Show("Are you sure you want to Rollback the changes made to this PO?!",
                                            "PO Modification",
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                            MessageBoxDefaultButton.Button2);

            // Call a method in the DA to Rollback the PO
            // Dont know if we have to refresh the data or do something with th UI
            if (dlgResult == DialogResult.Yes)
            {
                ClearPoItemsDataGrid();
                
                _porder = _porderoriginal;

                DisplayOriginalPoHeader();
                DisplayOriginalPoItems();

            }
            */
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Prevent control validation when user clicks the Cancel button
            _bFormCancelClicked = true;

            DialogResult dlgResult = MessageBox.Show("Are you sure you want to Cancel this PO Modification?", "SPICE PO Modification", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dlgResult == DialogResult.No)
            {
                _bFormCancelClicked = false;
            }
            else
            {
                this.Close();
            }
        }

        private void btnPOHistory_Click(object sender, EventArgs e)
        {
            // HK : 05-01-2010
            // Fix Bug ??
            string sippo = _porder.IPPOnumber;

            History historylookup;

            historylookup = new History(_dbparamref, "PO", _spiceponumber, sippo);

            historylookup.ShowDialog();

            historylookup = null;

        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx");
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            short       itemclass;
            short       dept;
            int         vendor;
            string      itemclasslck        = "N";
            string      deptlck             = "Y";
            string      vendorlck           = "N";
            DataTable   dtSelectedItems;
            int         iTotalRowsInGrid;
            int         iStartRow;
            short nextsequencenumber;
            
            // DataRow variables
            Int16 iClass;
            Int32 iVendor;
            Int16 iStyle;
            Int16 iColour;
            Int16 iSize;

            itemclass = 0;
            dept      = Convert.ToInt16(txtDept.Text);
            vendor    = Convert.ToInt32(txtVendor.Text);

            Disney.Spice.ItemsUI.SelectItem frmSelectItem = new SelectItem(_porder.DbParamRef, _porder.UserName, 
                                                                            _porder.Penvironment, 
                                                                            _mdiparent,  itemclass, itemclasslck, 
                                                                            dept, deptlck, vendor, vendorlck);

            dtSelectedItems = frmSelectItem.GetSelectedItems();

            char padchar = Convert.ToChar(" ");

            if (dtSelectedItems.Rows.Count > 0)
            {
                iStartRow = iTotalRowsInGrid = dgvPOlines.Rows.Count;

                foreach (DataRow dr in dtSelectedItems.Rows)
                {
                    iClass = (Int16)dr["Class"];

                    POItemDetails poitem = new POItemDetails((short)dr["class"], (int)dr["vendor"], 
                                                    (short)dr["style"], (short)dr["colour"],
                                                    (short)dr["size"], iStartRow + 1);
                    
                    nextsequencenumber = GetNextSequenceNumber();
                    poitem.Sequence = nextsequencenumber;

                    List<string> retValues = validationcls.ValidateClass(poitem.ClassCode.ToString());
                    poitem.Classname = retValues[1].ToString();

                    retValues = validationcls.ValidateVendor(poitem.Vendorcode.ToString(), true);
                    poitem.Vendordesc = retValues[1].ToString();

                    retValues = validationcls.ValidateColour(poitem.Colorcode.ToString());
                    poitem.Colordesc = retValues[1];

                    retValues = validationcls.ValidateSize(poitem.Itemsize.ToString());
                    poitem.Sizename = retValues[1];

                    if (poitem.ItemLookup(_porder.DbParamRef, _porder.UserName, _porder.Penvironment, _porder.DefaultMarket))
                    {
                        iStartRow = dgvPOlines.Rows.Add(false, dr["class"],
                                                               dr["vendor"],
                                                               dr["style"],
                                                               dr["colour"],
                                                               dr["size"]);

                        dgvPOlines["Sequence", iStartRow].Value = nextsequencenumber;  // CJ woz 'ere

                        dgvPOlines.Rows[iStartRow].Cells["Description"].Value     = poitem.Itemlongdescription;
                        dgvPOlines.Rows[iStartRow].Cells["Retail"].Value          = poitem.Retailprice.ToString();
                        dgvPOlines.Rows[iStartRow].Cells["Cost"].Value            = poitem.Cost.ToString();
                        dgvPOlines.Rows[iStartRow].Cells["Character"].Value       = poitem.Characterdesc;
                        dgvPOlines.Rows[iStartRow].Cells["Season"].Value          = poitem.SeasonDesc;
                        dgvPOlines.Rows[iStartRow].Cells["CasePackType"].Value    = poitem.Packdescription;
                        dgvPOlines.Rows[iStartRow].Cells["TicketType"].Value      = poitem.Tickettype;
                        dgvPOlines.Rows[iStartRow].Cells["Pack"].Value            = poitem.APP1;

                        _porder.NumofPOLines += 1;

                        if (poitem.APP1 == "Y")
                        {
                            _porder.NumofPOPacks += 1;
                            dgvPOlines.Rows[iStartRow].Cells["ConvertedCost"].ReadOnly = true;

                            AssortedPrePack AssPrePack = new AssortedPrePack(poitem, _porder);
                            DataTable prePackTbl = AssPrePack.PopulateAPPStructure();

                            for (int i = 0; i < prePackTbl.Rows.Count; i++)
                            {
                                APPcomponent component = new APPcomponent();
                                component.ComponentClass  = (Int16)prePackTbl.Rows[i]["ComponentClass"];
                                component.ComponentVendor = (Int32)prePackTbl.Rows[i]["ComponentVendor"];
                                component.ComponentStyle  = (Int16)prePackTbl.Rows[i]["ComponentStyle"];
                                component.ComponentColour = (Int16)prePackTbl.Rows[i]["ComponentColour"];
                                component.ComponentSize   = (Int16)prePackTbl.Rows[i]["ComponentSize"];
                                component.RatioQuantity   = (Int16)prePackTbl.Rows[i]["ComponentQuantity"];
                                component.Cost   = (Decimal)prePackTbl.Rows[i]["ComponentCost"];
                                component.Retail = (Decimal)prePackTbl.Rows[i]["Retail"];
                                component.ItemDescription = (String)prePackTbl.Rows[i]["ComponentLongDesc"];

                                poitem.Components.Add(component);
                            }
                        }
                        else
                        {
                            dgvPOlines.Rows[iStartRow].Cells["ConvertedCost"].ReadOnly = false;
                        }

                        if (_porder.Landing == 0)
                        {
                            _porder.Landing = 1;
                        }

                        poitem.ConvertedCost = Decimal.Round((poitem.Cost * _currencyratemarket) / _porder.ExchangeRate, 2);
                        dgvPOlines.Rows[iStartRow].Cells["ConvertedCost"].Value = poitem.ConvertedCost;

                        poitem.LandedCost = poitem.Cost * _porder.Landing;
                        dgvPOlines.Rows[iStartRow].Cells["LandedCost"].Value = poitem.LandedCost;

                        _porder.lstpoLineItemDetails.Add(poitem);
                    }
                    else
                    {
                        poitem.IsValid = false;

                        Boolean bItemLookupError = true;

                        iClass = poitem.ClassCode;
                        iVendor = poitem.Vendorcode;
                        iStyle = poitem.Stylecode;
                        iColour = poitem.Colorcode;
                        iSize = poitem.Itemsize;

                        MessageBox.Show("Skipping the following item as it does not belong to the current market."
                              + "\r\n"
                              + "\r\n"
                              + " Item Index: " + poitem.Itemindex.ToString().PadRight(6, padchar)
                              + " Class Code: " + iClass.ToString().PadRight(6, padchar)
                              + " Vendor Code: " + iVendor.ToString().PadRight(6, padchar)
                              + " Style Code: " + iStyle.ToString().PadRight(6, padchar)
                              + " Color Code: " + iColour.ToString().PadRight(6, padchar)
                              + " Size Code: " + iSize.ToString().PadRight(6, padchar)
                              + "", "Spice - PO Create");
                    }
                }

                btnCreatePO.Enabled = true;
            }
        }
        
        private void btnDeleteLine_Click(object sender, EventArgs e)
        {
            // HK : 27-11-2009 : To Do : Check that the row being tested in not a new row.
            // i.e skip any rows that have a status of IsNewRow = true

            // HK : Bug 70 : A number of annoyances were observed in the navigation 
            // and validation of datagridview.
            int iRowsDeleted = 0;
            int iRunningCountTotalRows = 0;
            int iLoopCounter;

            _bUserWantsToDeleteLine = true;

            // Init the looping counter
            iLoopCounter = 0;
            iRunningCountTotalRows = dgvPOlines.Rows.Count;

            // HK : FC : BM : 09-12-2009. Fix Bug 132
            //if (_polinedetails != null && _polinedetails.Isvalid == false)
            /*
            if (_polinedetails != null)
            {
                _bUserWantsToDeleteLine = false;
                return;
            }
            */

            do
            {
                if (!dgvPOlines.Rows[iLoopCounter].IsNewRow)
                {
                    // HK : FC : BM : 09-12-2009. Fix Bug 132
                    // Read values in datagrid in variables using Convert methods
                    // and then output them to Debug.Print
                    // 
                    //iitemindex  = Convert.ToInt32(dtgrdPOLinesView.Rows[iLoopCounter].Index);
                    // HK : 11-12-2009 : Item Index is not on the datagrid. So we must use the 
                    // item index on the PO Items Collection
                    //iitemindex  = _porder.lstpoLineItemDetails[iLoopCounter].Itemindex;
                    //iClass      = Convert.ToInt16(dtgrdPOLinesView.Rows[iLoopCounter].Cells["Class"].Value);
                    //iVendor     = Convert.ToInt32(dtgrdPOLinesView.Rows[iLoopCounter].Cells["Vendor"].Value);
                    //iStyle      = Convert.ToInt16(dtgrdPOLinesView.Rows[iLoopCounter].Cells["Style"].Value);
                    //iColor      = Convert.ToInt16(dtgrdPOLinesView.Rows[iLoopCounter].Cells["Color"].Value);
                    //iSize       = Convert.ToInt16(dtgrdPOLinesView.Rows[iLoopCounter].Cells["Size"].Value);

                    if (dgvPOlines.Rows[iLoopCounter].Cells[0].Value != null &&
                                Convert.ToBoolean(dgvPOlines.Rows[iLoopCounter].Cells[0].Value) == true)
                    {
                        Debug.Print("Data Grid row removed at index: " + iLoopCounter.ToString());
                        //DisplayDataGridItems(iLoopCounter);

                        // HK : 30-11-2009 : The below RemoveAt will cause a row enter event to fire.
                        // As we delete the row at iLoopCounter, the nex valid row (if any) will become 
                        // the current row.
                        dgvPOlines.Rows.RemoveAt(iLoopCounter);

                        // HK : Bug : 70 : Remove the reference to this PO Line Item from 
                        // the POItems collection
                        // HK : 30-11-2009 : If Item is a APP then decrement the 
                        // pack count
                        if (_porder.lstpoLineItemDetails[iLoopCounter].APP1 == "Y")
                        {
                            _porder.NumofPOPacks = _porder.NumofPOPacks - 1;
                        }

                        // Display the items on the class object in the collection about to be deleted
                        //DisplayLineItemDetails(_porder.lstpoLineItemDetails[iLoopCounter]);

                        _porder.lstpoLineItemDetails.RemoveAt(iLoopCounter);
                        iRowsDeleted++;
                        iRunningCountTotalRows = dgvPOlines.Rows.Count;
                    }
                    else
                    {
                        //DisplayDataGridItems(iLoopCounter);
                        iLoopCounter++;
                    }
                }
                else
                {
                    iLoopCounter++;
                }

            } while (iLoopCounter < iRunningCountTotalRows);

            _bUserWantsToDeleteLine = false;

            // HK : 20-11-2009 : After all selected rows have been deleted and the 
            // items collection has been updated then calculate PO summary
            // Calculate Po Summary if any records were deleted.
            // Note : ASH originally did this from the RowsDeleteting event handler
            if (iRowsDeleted > 0)
            {
                // HK : 22-01-2010 : Fix Bug 312 :
                btnCreatePO.Enabled = true;

                CalculatePOSummary();
            }

            // Display the contents of the grid annd the items collection to verify that they are consistent
            //DisplayDataGridItems();

            //DisplayItemCollection();

            // ?? To Do ?? To Do
            // HK : 28-11-2009 : After rows have been deleted,  
            // certain row(s) that were duplicate will not be 
            // duplicate as the user must have deleted their 
            // duplicate(s). So we must un highlight them.

            /*
            for (int count = dtgrdPOLinesView.Rows.Count -1; count >=0; count--)
            {

                if (dtgrdPOLinesView.Rows[count].Cells[0].Value != null &&
                            Convert.ToBoolean(dtgrdPOLinesView.Rows[count].Cells[0].Value) == true)
                {
                    dtgrdPOLinesView.Rows.RemoveAt(count);

                    // HK : Bug : 70 : Remove the reference to this PO Line Item from 
                    // the POItems collection
                    _porder.lstpoLineItemDetails.RemoveAt(count);

                    iRowsDeleted++;

                    // Skip back as row has been deleted
                    //count--;
                }
            }
            */

            /*
            foreach (DataGridViewRow row in  dtgrdPOLinesView.Rows)

            {
                if(row.Cells[0].Value!=null &&             
                    Convert.ToBoolean(row.Cells[0].Value) == true)            
                {

                    iRowIndexToRemove = row.Index;

                    //dtgrdPOLinesView.Rows.RemoveAt(row.Index);

                    dtgrdPOLinesView.Rows.Remove(row);

                    //Remove the item from the polinesarray and reindex

                    // HK : Bug : 70 : Remove the reference to this PO Line Item from 
                    // the POItems collection
                    _porder.lstpoLineItemDetails.RemoveAt(iRowIndexToRemove);

                    iRowsDeleted++;

                }

            }
            */

        }

        private void btnCreatePO_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckRequiredFields() == false)
                {
                    MessageBox.Show("Not enough information is available to modify the PO", "SPICE PO MANAGEMENT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (CheckItemQuantity() == false)
                {
                    MessageBox.Show("Item quantity cannot be 0, \r\n or Item number is invalid", "SPICE PO MANAGEMENT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int _numberofPurchaseOrders = 0;

                switch (_porder.PurchaseOrderType)
                {
                    case PurchaseOrder.POtype.StandardDCPO:
                        _numberofPurchaseOrders = 1;
                        break;
                    case PurchaseOrder.POtype.DropShipSingle:
                        _numberofPurchaseOrders = dtSelectedStores.Rows.Count;
                        break;
                    case PurchaseOrder.POtype.DropShipMultiple:
                        //Not implemented
                        break;
                }

                if (MessageBox.Show("You are about to modify PO No. " + _spiceponumber
                                + "\r\n"
                                + "\r\n"
                                + "Do you want to continue? ", "SPICE - PO Modification - Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {

                    if (isSpicePOlocked(_porder.SpicePOnumber,_porder.IPPOnumber,_porder.OrderStatus))
                    {
                        MessageBox.Show("This Order is locked against modification",
                                        "Update not possible",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        pwindow = new ProgressWindow(_numberofPurchaseOrders);
                        pwindow.Show();
                        bkgrndWorker.RunWorkerAsync();
                    }
                }
            }

            catch (Exception)
            {
                MessageBox.Show("An Error has occurred with the Purchase Order Please contact Support", "SPICE PO MANAGEMENT");
            }
        }
        #endregion

        #region Background worker
        private void bkgrndWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            string sStore = String.Empty;
            //int istorequantity = 0;
            // HK : 02-12-2009 : Drop Ship Single, Drop Ship Matrix, Standard PO
            //if (!CreatePurchaseOrder())
            if (!ModifyPurchaseOrder(_spiceponumber))
            {
                e.Cancel = true;
                //break;
            }

            bkgrndWorker.ReportProgress(index);
        }
        
        private void bkgrndWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pwindow.UpdateProgressBar(e.ProgressPercentage); 
        }

        private void bkgrndWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                MessageBox.Show("There was an error with your order please contact Support", "SPICE PO MANAGMENT");
            }

            pwindow.Close();
            this.Close();
        }
        #endregion
    }
}