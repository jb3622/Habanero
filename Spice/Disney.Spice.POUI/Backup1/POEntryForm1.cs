using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Disney.Spice.POBO;
using Disney.Spice.LookUp;
using Disney.Spice.ItemsBO;
using Disney.Spice.StoreSearch;
using Disney.Spice.ItemsUI;

namespace Disney.Spice.POUI
{
    public partial class POEntryForm1 : Form
    {
        private PurchaseOrder   _porder;
        private Enquiry         _vendorlookup;
        private Enquiry         _deptlookup;
        private LookupBO        lookupbo;
        private Validation      validationcls;
        private POItemDetails   _polinedetails;
        private POLineDetails   polineform;
        private ProgressWindow  pwindow;
        //private AssortedPrePack AssPrePack;

        private int _itemquantityrounded = 0;

        private Boolean _bFormCancelClicked;
        private Boolean _bUserWantsToDeleteLine;
        private Boolean _bDuplicateItem;

        private DataTable _dtCurrency = null;
        private DataTable dtSelectedStores = null;

        private decimal _ccyratemarket;
        //private decimal _ccyratepo;
        private decimal _ccyrateprev;
        
        private DataGridViewCellStyle   dgvcsPoLinesnormal;
        private DataGridViewCellStyle   dgvcsPoLinesalternate;

        private DataTable   dtDropShipMatrix        = null;
        private string      _sStoreColumnNamePrefix = "Store_";

        private PurchaseOrder.PoHitsCollection _pohitscollection;

        private string _defaultshipvia      = String.Empty;

        private DataTable dtFreight = new DataTable("Freight");

        private Boolean IsItemClassChanged;
        private Boolean IsItemVendorChanged;
        private Boolean IsItemStyleChanged;
        private Boolean IsItemColorChanged;
        private Boolean IsItemSizeChanged;

        private string _currencyformat = "N"; // Standard .NET culture aware currency format mask

        private Form _mdiparent;
        
        // Constants
        private string defaultdc;

        private const int MINLANDINGVALUEFOROCN = 1;
        private const string OCEANSHIPVIACODE = "OCN";
        private const string MAGICDCSTOREVATCODE = "A";

        #region Constructors and form events
        public POEntryForm1(PurchaseOrder purchaseorder, Form owner)
        {
            InitializeComponent();

            //Initialise the local variable so that its available to ROW
            _porder = purchaseorder;
            
            //this.MaximizeBox = false;
            _mdiparent = owner;
            this.MdiParent = owner;
  
            SetupInitialValues();
        }

        private void POEntryForm1_Load(object sender, EventArgs e)
        {
        }

        private void SetupInitialValues()
        {
            //Depending on the default market enable/disable controls
            lblSSD.Visible = false;
            cmbSSD.Visible = false;
            
            //dtpkrShipDate.Value = DateTime.Now;
            //dtpkrAnticipateDate.Value = DateTime.Now;

            lookupbo = new LookupBO(_porder.DbParamRef, _porder.UserName,_porder.Penvironment);

            validationcls = new Validation(_porder.DbParamRef,_porder.UserName,_porder.Penvironment);

            if (_porder.Penvironment.DateFormat == "DMY")
            {
                dtpkrAnticipateDate.CustomFormat = "d MMMM yyyy";
                dtpkrShipDate.CustomFormat = "d MMMM yyyy";

                dtpkrAnticipateDate.Value = DateTime.Now;
                dtpkrShipDate.Value       = DateTime.Now;

                txtOrderDate.Text = DateTime.Now.ToString("d MMMM yyyy");
                txtCancelDate.Text = dtpkrShipDate.Value.ToString("d MMMM yyyy");
                _porder.CancelDate = dtpkrShipDate.Value;
            }
            else
            {
                dtpkrAnticipateDate.CustomFormat = "MMMMd,  yyyy";
                dtpkrShipDate.CustomFormat = "MMMMd,  yyyy";

                dtpkrAnticipateDate.Value = DateTime.Now;
                dtpkrShipDate.Value       = DateTime.Now;

                txtOrderDate.Text = DateTime.Now.ToString("MMMM d,  yyyy");
                txtCancelDate.Text = dtpkrShipDate.Value.AddDays(6).ToString("MMMM d,  yyyy");
                _porder.CancelDate = dtpkrShipDate.Value.AddDays(6);
            }


            lblMarketValue.Text = _porder.DefaultMarket + "-" + _porder.MarketDescription;

            //Disabling of buttons
            btnStores.Enabled = false;

            defaultdc = LookUpDefaultDC.GetDefaultDC(_porder.Penvironment.Domain, _porder.DefaultMarket);
            cmbShipTo.Items.Add(defaultdc);
            cmbShipTo.SelectedItem = cmbShipTo.Items[0];

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

                // Freight
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

                //cbxFreight.SelectedValue = "";
                cbxFreight.SelectedIndex = 0;

                lblFreightCharges.Visible = true;
                cbxFreight.Visible = true;
            }


            // Since the default value selected is for DC.
            dtSelectedStores = GetEmptyStores();
            dtSelectedStores.Rows.Add(true, defaultdc, "Distribution Centre");
            _porder.PoType = PurchaseOrder.POtype.StandardDCPO; ;

            txtLanding.Enabled = false;
            txtPortofEntry.Enabled = false;
            txtPortofDeparture.Enabled = false;
            txtDelTerms.Enabled = false;

            txtDept.Focus();

            _porder.NumofPOLines = 0;

            // Capture the Market currency Rate. (Market is selected on the Market Selection form)
            _ccyratemarket = validationcls.GetCurrency(_porder.MarketCurrency);

            // Capture the Default and Alternating cell styles in instance variables
            dgvcsPoLinesnormal = new DataGridViewCellStyle(dgvPOlines.DefaultCellStyle);
            dgvcsPoLinesalternate = new DataGridViewCellStyle(dgvPOlines.AlternatingRowsDefaultCellStyle);

            // Initalise the structure of the 'Drop Ship Matrix PO' datatable.
            // Drop Ship Matrix
            dtDropShipMatrix = new DataTable();

            // items in a PO
            dtDropShipMatrix.Columns.Add("ItemIndex",        typeof(String));
            dtDropShipMatrix.Columns.Add("Pack",             typeof(String));
            dtDropShipMatrix.Columns.Add("Class",            typeof(String));
            dtDropShipMatrix.Columns.Add("Vendor",           typeof(String));
            dtDropShipMatrix.Columns.Add("Style",            typeof(String));
            dtDropShipMatrix.Columns.Add("Color",            typeof(String));
            dtDropShipMatrix.Columns.Add("Size",             typeof(String));
            dtDropShipMatrix.Columns.Add("Description",      typeof(String));
            dtDropShipMatrix.Columns.Add("Quantity",         typeof(String));
            dtDropShipMatrix.Columns.Add("QuantityAssigned", typeof(String));

            // Initalise the structure of the 'PO Hits' datatable.
            // PO Hits
            // Fix Bug 148. Hits button has to be disabled until
            // at least one valid PO Item has been added.
            // Hold on to resolving this bug.
            btnHits.Enabled = true;

            // HK : 03-12-2009 Because of the issue of the checkbox 
            // to create/activate and deactive Hits I have decided to 
            // create the hits collection at startup

            _pohitscollection = new PurchaseOrder.PoHitsCollection();

            for (int i = 0; i <= 4; i++)
            {
                PurchaseOrder.POHits pohits;
                pohits = new PurchaseOrder.POHits();

                // Set the property HitCreated to true
                //pohits.HitCreated = true;
                pohits.HitNUmber = i + 2;

                // Add to our collection
                _pohitscollection.Insert(i, pohits);
            }

            dgvPOlines.Enabled = false;

            InitalisePOsummary();
       }

        private void POEntryForm1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
        #endregion

        #region POHeader
        private void txtVendor_TextChanged(object sender, EventArgs e)
        {

            //Check if its an integer
            lblVendorDesc.Text = "";
            errPOEntry.SetError(txtVendor, "");



        }

        private void pctBoxVendor_Click(object sender, EventArgs e)
        {
            //Vendor Lookup

            errPOEntry.SetError(txtVendor, "");
                      
            DataSet dsVendors =  lookupbo.VendorLookup();

            if (_vendorlookup == null) _vendorlookup = new Enquiry(dsVendors.Tables["FilteredVendors"], "Vendors");

            _vendorlookup.ShowGrid();

            // HK : 27-01-2010 : Fix Bug : 367

            if (_vendorlookup.SelectedValue != null)
            {
                txtVendor.Text = _vendorlookup.SelectedValue[0];
                lblVendorDesc.Text = _vendorlookup.SelectedValue[1];

                txtVendor_Validating(txtVendor, new CancelEventArgs());
            }
            else
            {
                txtVendor.Focus();
            }

            /*
            if (_vendorlookup.DialogResult == DialogResult.OK)
            {       
                txtVendor.Text = _vendorlookup.SelectedValue[0];
                lblVendorDesc.Text = _vendorlookup.SelectedValue[1];

                txtVendor_Validating(txtVendor,new CancelEventArgs());
                
                //List<string> lstReturn = new List<string>();
                //lstReturn = validationcls.ValidateVendor(txtVendor.Text, false);

                //txtTerms.Text = lstReturn[3];
                //lblTermsDesc.Text = lstReturn[5];
                //errPOEntry.SetError(txtVendor, "");
                //validationcls.HighlightErrControls(lblVendor, txtVendor, true);

            }
            else
            {

                lblVendorDesc.Text = "";
                errPOEntry.SetError(txtVendor, "Please enter a valid Vendor code");
                validationcls.HighlightErrControls(lblVendor, txtVendor, false);

            }
            */

        }

        private void txtVendor_Validating(object sender, CancelEventArgs e)
        {
            if (!_bFormCancelClicked)
            {
                try
                {
                    List<string> lstReturn = new List<string>();

                    lstReturn = validationcls.ValidateVendor(txtVendor.Text, false);

                    if (lstReturn[0] == "False")
                    {
                        txtVendor.Clear();
                        lblVendorDesc.Text = "";
                        lblTermsDesc.Text = "";
                        txtTerms.Text = "";
                        validationcls.HighlightErrControls(lblVendor, txtVendor, false);
                        errPOEntry.SetError(txtVendor, "Enter a valid Vendor code");
                        e.Cancel = true;
                        _porder.Vendorcode = 0;
                    }
                    else
                    {
                        lblVendorDesc.Text = lstReturn[1];

                        // HK : FC : BM : CJ : 03-12-2009 : Fix Bug 101
                        // Returns OCNFRT which is not a valid ShipVia Type Data incorrect.
                        // Data in file is wrong. So validate the data against the 
                        // validation class and if it is wrong then do not display 
                        // the code
                        List<string> lstretvalues_1 = new List<string>();

                        lstretvalues_1 = validationcls.ValidateShipVia(lstReturn[2]);

                        if (lstretvalues_1[0] == "True")
                        {
                            //Populate the right data etc and 
                            // HK : Fix Bug 103 : Once the delault Ship Via has been populated 
                            // then dont re populate it on subsequent validation of the same vendor
                            // because the user might have changed the default shipping method to 
                            // something else other that the default shipping method.
                            if (_defaultshipvia != lstretvalues_1[1])
                            {
                                lblShipViaDesc.Text = lstretvalues_1[1];
                                _defaultshipvia     = lstretvalues_1[1];

                                txtShipVia.Text = lstReturn[2]; //Returns OCNFRT which is not a valid ShipVia Type Data incorrect Check with Fabrice

                                //txtShipVia.Text = lstReturn[2]; //Returns OCNFRT which is not a valid ShipVia Type Data incorrect Check with Fabrice
                            }

                            errPOEntry.SetError(txtShipVia, "");
                            validationcls.HighlightErrControls(lblShipVia, txtShipVia, true);
                            e.Cancel = false;
                            _porder.ShipViaCode = txtShipVia.Text;
                        }

                        
                        txtTerms.Text = lstReturn[3];
                        // Assign the terms code to its appropriate 
                        // property in the PO Header
                        _porder.Termscode = lstReturn[3];

                        if (!String.IsNullOrEmpty(lstReturn[4]))
                        { txtCurrency.Text = lstReturn[4]; }
                        lblTermsDesc.Text = lstReturn[5];
                        validationcls.HighlightErrControls(lblVendor, txtVendor, true);
                        errPOEntry.SetError(txtVendor, "");
                        e.Cancel = false;
                        _porder.Vendorcode = Int32.Parse(txtVendor.Text);
                    }
                }

                catch (System.Exception)
                {
                    validationcls.HighlightErrControls(lblVendor, txtVendor, false);
                    errPOEntry.SetError(txtVendor, "Enter a valid Vendor Code");
                    lblVendorDesc.Text = "";
                    lblTermsDesc.Text = "";
                    txtTerms.Text = "";
                    txtVendor.Focus();
                }
            }
        }

        private void pctBoxDept_Click(object sender, EventArgs e)
        {
            string[] strretvalues;

            errPOEntry.SetError(txtDept, "");

            DataTable dtAuthorized = lookupbo.DepartmentLookup();

            _deptlookup = new Enquiry(dtAuthorized, "DepartmentLookup");

            _deptlookup.ShowGrid();

            // HK : HK : 28-01-2010 : Fix Bug 366 : 
            strretvalues = _deptlookup.SelectedValue;

            if (strretvalues != null)
            {
                strretvalues = _deptlookup.SelectedValue;

                txtDept.Text = strretvalues[0];
                lblDeptDesc.Text = strretvalues[1];
                errPOEntry.SetError(txtDept, "");
                txtDept.Focus();
                validationcls.HighlightErrControls(lblDept, txtDept, true);
            }
            else
            {
                // HK : 27-01-2010 : Fix Bug 366 : 
                txtDept.Focus();
            }

            /*
            if (_deptlookup.DialogResult == DialogResult.OK)
            {
                strretvalues = _deptlookup.SelectedValue;

                txtDept.Text = strretvalues[0];
                lblDeptDesc.Text = strretvalues[1];
                errPOEntry.SetError(txtDept, "");
                txtDept.Focus();
                validationcls.HighlightErrControls(lblDept, txtDept, true);
                
            }
            else
            {

                lblDeptDesc.Text = "";
                _porder.Deptcode = 0;
                errPOEntry.SetError(txtDept, "Please enter a valid Department code");
                validationcls.HighlightErrControls(lblDept, txtDept, false);

                // HK : 27-01-2010 : Fix Bug 366 : 
                txtDept.Focus();
            }
            */
        }

        private void txtDept_TextChanged(object sender, EventArgs e)
        {
            //Check if its an integer
            lblDeptDesc.Text = "";
            errPOEntry.SetError(txtDept, "");
        }

        private void txtDept_Validating(object sender, CancelEventArgs e)
        {
            if (!_bFormCancelClicked)
            {
                try
                {
                    List<string> lstReturn = new List<string>();

                    lstReturn = validationcls.ValidateDeptCode(txtDept.Text);

                    if (String.IsNullOrEmpty(lstReturn[0].ToString()))
                    {
                        txtDept.Clear();
                        lblDeptDesc.Text = "";
                        errPOEntry.SetError(txtDept, "Enter a valid dept code");
                        validationcls.HighlightErrControls(lblDept, txtVendor, false);
                        e.Cancel = true;
                        _porder.Deptcode = 0;
                    }
                    else
                    {
                        lblDeptDesc.Text = lstReturn[1].ToString();
                        errPOEntry.SetError(txtDept, "");
                        validationcls.HighlightErrControls(lblDept, txtVendor, true);
                        e.Cancel = false;
                        _porder.Deptcode = Int16.Parse(txtDept.Text);
                    }
                }
                catch (Exception)
                {
                    lblDeptDesc.Text = "";
                    errPOEntry.SetError(txtDept, "Enter a valid dept code");
                    validationcls.HighlightErrControls(lblDept, txtVendor, false);
                    e.Cancel = true;
                }
            }
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
                // Error here
                errPOEntry.SetError(txtCurrency, "Please enter a valid Currency code");
                validationcls.HighlightErrControls(lblCurrency, txtCurrency, false);
            }
        }

        private void txtCurrency_Validating(object sender, CancelEventArgs e)
        {
            if (!_bFormCancelClicked)
            {
                try
                {
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

                        lblCurrVal1.Text  = "(" + _porder.MarketCurrency + ")";
                        lblCurrValue.Text = "(" + _porder.MarketCurrency + ")";
                        
                        _porder.Currencycode = txtCurrency.Text;
                        _porder.ExchangeRate = Decimal.Parse(lstReturn[2]);

                        if (_porder.ExchangeRate.Equals(_ccyratemarket))
                        {
                            _porder.ExchangeRate = 1.000000m;
                        }
                        //_ccyratepo = _porder.ExchangeRate;

                        lblCurrencyDesc.Text = lstReturn[1] + " (" + _porder.ExchangeRate + ")";

                        validationcls.HighlightErrControls(lblCurrency, txtCurrency, true);
                        ValidateHeaderFields(sender);

                        // Once currency is validated we must recalculate 
                        // the Cost field(s) again with the new currency rate
                        for (int RowNbr = 0; RowNbr < dgvPOlines.Rows.Count - 1; RowNbr++)
                        {
                            _polinedetails = _porder.lstpoLineItemDetails[RowNbr];

                            decimal ConvertedCost = Convert.ToDecimal(dgvPOlines["ConvertedCost", RowNbr].Value);
                            ConvertedCost = Decimal.Round((ConvertedCost * _ccyrateprev) / _porder.ExchangeRate, 2);
                            
                            dgvPOlines["ConvertedCost", RowNbr].Value = ConvertedCost;
                            _polinedetails.ConvertedCost = ConvertedCost;
                        }

                        _ccyrateprev = _porder.ExchangeRate;
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

        private void txtVendor_Validated(object sender, EventArgs e)
        {
            // HK : 18-12-2009 : Vendor validation get the default ShipVia for 
            // certain vendors if one is set against that vendor in the database.
            // So we must enable disable control specific to ShipVia code = OCN.
            ImportControlChanges();

            // Force focus on Currency field as method ImportControlChanges 
            // may have set focus to a different field.
            txtCurrency.Focus();
        }
        #endregion POHeader

        #region Shipping
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

                // Drop Ship (Matrix) PO 
                frmDropShipMatrix frmdropshipmatrix = null;

                // If no stores selected then do not hos the Drop Ship Matrix form 
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

            if (rdBtnDropShipMatrix.Checked)
            {
                txtNumberofDrops.Text = GetDrops().ToString();
            }
            if (rdBtnDropShipSingle.Checked)
            {
                txtNumberofDrops.Text = dtSelectedStores.Rows.Count.ToString(); 
            }
        }

        private void UpdateDrops()
        {
            //HK : 14-01-2010 : Fix Bug 214
            if (rdBtnDropShipMatrix.Checked)
            {
                txtNumberofDrops.Text = GetDrops().ToString();
            }
            
            if (rdBtnDropShipSingle.Checked)
            {
                txtNumberofDrops.Text = dtSelectedStores.Rows.Count.ToString();
            }

            if (rdBtnDCPO.Checked)
            {
                txtNumberofDrops.Text = "1";
            }
        }
        
        private int GetDrops()
        {
            string sStore = String.Empty;
            int istorequantity = 0;
            int drops = 0;

            foreach (DataRow dtrow in dtSelectedStores.Rows)
            {
                // HK : CJ : 26-11-2009 : Check if the quantity assigned every PO Line Item 
                // for that store in question is > 0. If not then skip this store (ie. do not 
                // create a PO for this store)
                if (rdBtnDropShipMatrix.Checked)
                {
                    sStore = dtrow["clmStore"].ToString();
                    istorequantity = SumStoreQuantity(sStore);

                    if (istorequantity > 0)
                    {
                        drops++;
                    }
                }
            }

            return drops;
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
                // _porder.ShipTo = Int16.Parse(cmbShipTo.Text);
                _porder.PurchaseOrderType = PurchaseOrder.POtype.StandardDCPO;
                btnStores.Enabled = false;
                btnHits.Enabled = true;

                // Re calculate the PO Summary
                CalculatePOsummary();
                cmbShipTo.Enabled = true;
            }
        }
        
        private void rdBtnDropShipSingle_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnDropShipSingle.Checked)
            {
                _porder.PurchaseOrderType = PurchaseOrder.POtype.DropShipSingle;
                btnStores.Enabled = true;
                btnHits.Enabled = false;
                dtSelectedStores.Rows.Clear();

                // Blank out the PO Summary
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

            btnHits.Enabled = false;

            dtSelectedStores.Rows.Clear();

            _porder.PurchaseOrderType = PurchaseOrder.POtype.DropShipMultiple;
        }

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
                if (!String.IsNullOrEmpty(txtMarginPercent.Text)) CalculatePOsummary();
            }
        }
        
        private void txtShipVia_Validating(object sender, CancelEventArgs e)
        {
            // Dont validate when user is trying to close the form
            if (!_bFormCancelClicked)
            {
                // HK : FC : Resolve Bug 96
                // Unhilight any validation errors that may have appeared
                // due to the click of the Create PO button
                validationcls.HighlightErrControls(lblLanding, txtLanding, true);
                errPOEntry.SetError(txtLanding, "");

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

                    ValidateHeaderFields(sender);
                }
                else
                {
                    //Populate the error
                    lblShipViaDesc.Text = "";
                    validationcls.HighlightErrControls(lblShipVia, txtShipVia, false);
                    errPOEntry.SetError(txtShipVia, "Please enter a valid Shipping Method");
                    _porder.ShipViaCode = "";
                    e.Cancel = true;
                }
            }
        }

        private void txtShipVia_Validated(object sender, EventArgs e)
        {
            ImportControlChanges();
        }
        #endregion

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
            // If user decided to Cancel the form do not force validation
            if (_bFormCancelClicked == false)
            {
                errPOEntry.SetError(dtpkrAnticipateDate, string.Empty);
                if (dtpkrAnticipateDate.Value < DateTime.Today)
                {
                    errPOEntry.SetError(dtpkrAnticipateDate, "Please enter a date greater than Today");
                    e.Cancel = true;
                    return;
                }

                _porder.AnticipateDate = dtpkrAnticipateDate.Value;
            }
        }

        private void dtpkrShipDate_Validating(object sender, CancelEventArgs e)
        {
            // If user decided to Cancel the form do not force validation
            if (_bFormCancelClicked == false)
            {
                errPOEntry.SetError(dtpkrShipDate, "");
                if (dtpkrShipDate.Value < DateTime.Today)
                {
                    errPOEntry.SetError(dtpkrShipDate, "Please enter a date greater than  Today");
                    e.Cancel = true;
                    return;
                }

                _porder.ShippingDate = dtpkrShipDate.Value;
            }
        }

        private void grpBoxDates_Leave(object sender, EventArgs e)
        {
            // Cross validation of Anticipate date and Ship date
            if (string.IsNullOrEmpty(errPOEntry.GetError(dtpkrAnticipateDate)) & string.IsNullOrEmpty(errPOEntry.GetError(dtpkrShipDate)))
            {
                _porder.AnticipateDate = dtpkrAnticipateDate.Value;
                _porder.ShippingDate = dtpkrShipDate.Value;
                if ((dtpkrAnticipateDate.Value.Date < dtpkrShipDate.Value.Date))
                {
                    errPOEntry.SetError(dtpkrAnticipateDate, "Anticipate date cannot be before the ship date");
                    return;
                }
                
                if (_porder.Penvironment.Domain == "SWNA")
                {
                    txtCancelDate.Text = dtpkrShipDate.Value.AddDays(6).ToString("D");
                    _porder.CancelDate = dtpkrShipDate.Value.AddDays(6);
                }
                else
                {
                    txtCancelDate.Text = dtpkrShipDate.Value.ToLongDateString();
                    _porder.CancelDate = dtpkrShipDate.Value;
                }
            }
        }
        #endregion Dates

        #region Import
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
                errPOEntry.SetError(txtDelTerms, "Please enter a valid delivery code");
                validationcls.HighlightErrControls(lblDelTerms, txtDelTerms, false);
            }
        }

        private void txtPortofDeparture_Validating(object sender, CancelEventArgs e)
        {
            // HK : 01-12-2009 : Dont validate when user is trying to close the form
            if (!_bFormCancelClicked)
            {
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

        private void txtPortofDeparture_EnabledChanged(object sender, EventArgs e)
        {
            errPOEntry.Clear();
        }
        #endregion

        #region Summary
        private void InitalisePOsummary()
        {
            txtNumberofHits.Text  = "0".ToString();
            txtNumberofDrops.Text = "0".ToString();
            txtPOLines.Text       = "0".ToString();
            txtPOPacks.Text       = "0".ToString();

            txtTotalUnits.Text       = "0".ToString();
            txtTotalCost.Text        = "0.00".ToString();
            txtTotalRetail.Text      = "0.00".ToString();
            txtMarginValue.Text      = "0.00".ToString();
            txtTotalRetailExVat.Text = "0.00".ToString();
            txtMarginPercent.Text    = "0.00".ToString();
        }

        private void DisplayPOSummaryNA()
        {
            // Blank the Po Summary Fields
            txtTotalUnits.Text    = "N/A";
            txtTotalCost.Text     = "N/A";
            txtTotalRetail.Text   = "N/A";
            txtMarginValue.Text   = "N/A";
            txtMarginPercent.Text = "N/A";
        }

        private void CalculatePOsummary()
        {
            decimal totalretailexvat = 0;
            decimal marginvalue = 0;

            // Total Retail
            _porder.TotalRetail = _porder.CalculateTotalRetail();

            // Total Cost
            _porder.TotalCost   = _porder.CalculateTotalCost();
            
            // Total Units
            _porder.TotalUnits  = _porder.CalculateTotalUnit();

            if (_porder.Landing == 0)
            {
                _porder.Landing = 1;
            }
            
            // Total Landed Cost
            _porder.TotalLandedCost = _porder.CalculateTotalLandedCost();
            
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

            txtTotalCost.Text  = _porder.TotalLandedCost.ToString(_currencyformat);
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

            txtMarginValue.Text = Decimal.Round(_porder.MarginValue, 2).ToString(_currencyformat);
            txtMarginPercent.Text = Decimal.Round(_porder.MarginPercentage, 2).ToString(_currencyformat);
                
            UpdateDrops();
        }
        #endregion

        #region Item DataGrid
        private void dgvPOLines_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1) return;

            Int16 seq = Convert.ToInt16(dgvPOlines["sequence", e.RowIndex].Value);
            if (seq == 0) return;

            _polinedetails = _porder.lstpoLineItemDetails[seq - 1];

            //Display APP or Item Details Window
            if (_polinedetails.IsValid && _polinedetails.APP1=="N")
            {
                Cursor.Current = Cursors.WaitCursor;

                polineform = new POLineDetails(_porder, ref _polinedetails, false);

                polineform.ShowDialog(this);
                polineform = null;

                // Update DataGrid Quantity, Cost, and summary figures
                dgvPOlines["Quantity", e.RowIndex].Value     = _polinedetails.Itemquantity;
                dgvPOlines["ConvertedCost",e.RowIndex].Value = _polinedetails.ConvertedCost;
                
                CalculatePOsummary();
            }
            else if (_polinedetails.IsValid && _polinedetails.APP1 == "Y")
            {
                Cursor.Current = Cursors.WaitCursor;

                POLineDetailsPack polinedetailspack = new POLineDetailsPack(_polinedetails, _porder, e.RowIndex);

                polinedetailspack.OnAppQuantityChanged += new POLineDetailsPack.AppQuantityChangedEventHandler(polinedetailspack_OnAppQuantityChanged);
                polinedetailspack.ShowDialog(this);

                dgvPOlines["Quantity", e.RowIndex].Value = _polinedetails.Itemquantity;

                _polinedetails.ConvertedCost = Decimal.Round(_polinedetails.Cost * _ccyratemarket / _porder.ExchangeRate, 2);
                dgvPOlines["ConvertedCost", e.RowIndex].Value = _polinedetails.ConvertedCost;

                if (_porder.Landing == 0) _porder.Landing = 1;

                _polinedetails.LandedCost = _polinedetails.Cost * _porder.Landing;
                dgvPOlines["LandedCost",e.RowIndex].Value = _polinedetails.LandedCost;

                CalculatePOsummary();

                polinedetailspack = null;
            }
        }

        void polinedetailspack_OnAppQuantityChanged(object sender, POLineDetailsPack.AppDetailsEventArgs e)
        {
            // Reassign the instance valriable of the business objects and others that may have 
            // been modified in the called window
            _porder        = e.porder;
            _polinedetails = e.poline;

            // Refresh the value in the corresponding record in the grid
            dgvPOlines.Rows[e.rowindex].Cells["Quantity"].Value = e.quantity;

            dgvPOlines.Rows[e.rowindex].Cells["ConvertedCost"].Value = e.poline.ConvertedCost;

            CalculatePOsummary();

            // Re subscribe the event handler on the PO Line Items business boject
            // for ASH's existing coding and functionaly to function as normal
            //_polinedetails.ItemQtyChanged += new POItemDetails.delItemQtyChanged(_polinedetails_ItemQtyChanged);
        }

        //void _polinedetails_ItemQtyChanged(int qty, decimal cost, int RowIndex)
        //{
        //    // Called from PO Line form and ItemQuantityForm. 
        //    // In the ItemQuantityForm when the user rounded up or rounded down the 
        //    // quantity, it has to be displayed in the grid on the correct row.

        //    // In the PO Line form, any changes to quantiry and  / or cost price
        //    // must also be displayed on the correct row in the grid

        //    dgvPOlines["Quantity",RowIndex].Value = qty;

        //    // Converted Cost can be changed by user
        //    // dgvPOlines["Cost",RowIndex].Value = cost;
            
        //    // When the Cost changes in the PoLineForm window it mut reflect the 
        //    // changes to ConvertedCost and LandedCost

        //    // Column Name       Label           Visible     Expression
        //    // =====================================================================================
        //    // ConvertedCost     Cost            True        Decimal.Round((_polinedetails.Cost * _currencyratemarket) / _currencyratepo, 2);
        //    // Cost              Uplift Cot      False       Value retrieved from database by ItemLookup. Also cased simple vendor cost
        //    // LandedCost        Landed Cost     False       Decimal.Round((_polinedetails.Cost * _currencyratemarket) * _porder.Landing, 2);

        //    //  No need to change the landed cost
        //    dgvPOlines.Rows[RowIndex].Cells["LandedCost"].Value = Decimal.Round((_polinedetails.Cost * _ccyratemarket) * _porder.Landing, 2);

        //    // Cannot divide by zero
        //    if (_ccyratepo != 0)
        //    {
        //        // Converted Cost can be changed by user
        //        // dgvPOlines.Rows[RowIndex].Cells["ConvertedCost"].Value = Decimal.Round((_polinedetails.Cost * _currencyratemarket) / _currencyratepo, 2);
        //        dgvPOlines.Rows[RowIndex].Cells["ConvertedCost"].Value = cost;
        //    }
        //    CalculatePOsummary();
        //}

        void DataGridClearItemnotfound(int rowindex)
        {
            dgvPOlines.Rows[rowindex].Cells["Retail"].Value        = String.Empty;
            dgvPOlines.Rows[rowindex].Cells["Cost"].Value          = String.Empty;
            dgvPOlines.Rows[rowindex].Cells["Character"].Value     = String.Empty;
            dgvPOlines.Rows[rowindex].Cells["Season"].Value        = String.Empty;
            dgvPOlines.Rows[rowindex].Cells["CasePackType"].Value  = String.Empty;
            dgvPOlines.Rows[rowindex].Cells["TicketType"].Value    = String.Empty;
            dgvPOlines.Rows[rowindex].Cells["Pack"].Value          = String.Empty;
            dgvPOlines.Rows[rowindex].Cells["Quantity"].Value      = String.Empty;

            dgvPOlines.Rows[rowindex].Cells["ConvertedCost"].Value = String.Empty;
            dgvPOlines.Rows[rowindex].Cells["LandedCost"].Value    = String.Empty;

            _polinedetails.Itemlongdescription      = String.Empty;
            _polinedetails.Retailprice              = 0.00m;
            _polinedetails.Cost                     = 0.00m;
            _polinedetails.Characterdesc            = String.Empty;
            _polinedetails.SeasonDesc               = String.Empty;
            _polinedetails.Packdescription          = String.Empty;
            _polinedetails.Tickettype               = String.Empty;
            _polinedetails.APP1                     = String.Empty;
            _polinedetails.Itemquantity             = 0;

            _polinedetails.ConvertedCost            = 0m;
            _polinedetails.LandedCost               = 0m;
        }
        
        private void dgvPOlines_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            // Validate all reqd fields notnull/empty
            //Debug.Print("Row Enter fired, Row Index: " + e.RowIndex.ToString() + "======" + "Row Count: " + dgvPOlines.Rows.Count.ToString());
            //Debug.Print("Collection Count: " + _porder.lstpoLineItemDetails.Count.ToString());

            // Prevent Datagrid Validation if the user clicked Cancel button
            if (_bFormCancelClicked)
            {
                return;
            }

            // When user navigates to existing row we must re-assign  
            // to _polinedetails the value held in the collection (if one exists).
            // Since this is the very first event fired (agmonst row events), the collection 
            // may not have been initalised for the 1st row.

            // if (_porder.lstpoLineItemDetails.Count > 0  && _porder.lstpoLineItemDetails[e.RowIndex] != null)
            if (_porder.lstpoLineItemDetails.Count > 0 && e.RowIndex < _porder.lstpoLineItemDetails.Count)
            {
                // This means that an item in the collection is found
                _polinedetails = _porder.lstpoLineItemDetails[e.RowIndex];

                // Re Subscribe the Quantity or Cont changed event handler.
                // The _polinedetails is passed by reference to the PoLineForm form.
                // the. So when the user changes quantity or cost in that form and 
                // raises  ItemQtyChanged event on _polinedetails, the event handler 
                // in the parent form  ie. this form will be triggered. Not an ideal 
                // way to send data back to calling form.

                // This re subscribing of event handler is necessary as the event original 
                // subsctiption is lost when we re-assign _polinedetails with a new instance.
                //_polinedetails.ItemQtyChanged += new POItemDetails.delItemQtyChanged(_polinedetails_ItemQtyChanged);

                // Output the main attributes of the line items object
                /*Debug.Print("From Event (Row Enter). Line Item Main Details :  "
                                             + Convert.ToString(_polinedetails.Classcode) + "   "
                                             + Convert.ToString(_polinedetails.Vendorcode) + "   "
                                             + Convert.ToString(_polinedetails.Stylecode) + "   "
                                             + Convert.ToString(_polinedetails.Colorcode) + "   Quantity: "
                                             + Convert.ToString(_polinedetails.Itemquantity) + "   Cost: "
                                             + Convert.ToString(_polinedetails.Cost)
                                             );*/
            }
        }
        
        private void dgvPOlines_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            // HK : Prevent Datagrid Validation if the user clicked Cancel button
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
        
        //private void dgvPOlines_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        //{
        //    if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Quantity"))
        //    {
        //        if (_polinedetails != null)
        //        {
        //            if (_polinedetails.IsValid == false)
        //            {
        //                e.Cancel = true;
        //            }
        //        }
        //    }
        //}

        private void dgvPOlines_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Prevent Datagrid Validation if the user clicked Cancel button
            if (_bFormCancelClicked)
            {
                return;
            }

            // If the user wants to delete the row then disable any pending row or cell level 
            // validation on the datagrid
            if (_bUserWantsToDeleteLine)
            {
                return;
            }

            if (dgvPOlines.Rows[e.RowIndex].IsNewRow)
            {
                return;
            }

            try
            {
                if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Class"))
                {
                    if (String.IsNullOrEmpty(e.FormattedValue.ToString().Trim()))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;

                        return;
                    }

                    List<string> retValues = validationcls.ValidateClass(e.FormattedValue.ToString(), txtDept.Text);
                    if ("False" == retValues[0])
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
                    // Validation and delete annoyances faced by users
                    // Do not validate class if nothing or spaces entered by user
                    if (String.IsNullOrEmpty(e.FormattedValue.ToString().Trim()))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;
                        return;
                    }

                    List<string> retValues = validationcls.ValidateVendor(e.FormattedValue.ToString(),true);
                    if (retValues[0] == "False")
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "Please enter valid Vendor code";
                        e.Cancel = true;
                        return;
                    }

                    dgvPOlines.Rows[e.RowIndex].ErrorText = "";
                    //Pack values to the poline class
                    _polinedetails.Vendorcode = Int32.Parse(e.FormattedValue.ToString());
                    _polinedetails.Vendordesc = retValues[1].ToString();
                    e.Cancel = false;

                    // Check to see if the item class in the datagrid row the same as one entered 
                    // by the user.
                    int vendor = Convert.ToInt32(dgvPOlines["Vendor", e.RowIndex].Value);
                    if (vendor != Int32.Parse(e.FormattedValue.ToString()))
                    {
                        IsItemVendorChanged = true;
                    }
                }

                else if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Style"))
                {
                    // Validation and delete annoyances faced by users
                    // Do not validate class if nothing or spaces entered by user
                    if (String.IsNullOrEmpty(e.FormattedValue.ToString().Trim()))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;
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
                        //Style code
                        _polinedetails.Stylecode = Int16.Parse(e.FormattedValue.ToString());
                        e.Cancel = false;

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
                    if (String.IsNullOrEmpty(e.FormattedValue.ToString().Trim()))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;

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
                        e.Cancel = false;

                        // Check to see if the item class in the datagrid row the same as one entered 
                        // by the user.
                        Int16 color = Convert.ToInt16(dgvPOlines["Color", e.RowIndex].Value);
                        if (color != Int16.Parse(e.FormattedValue.ToString()))
                        {
                            IsItemColorChanged = true;
                        }
                    }
                }

                else if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Size"))
                {
                    Int16   iItem;
                    Int32   iVendor;
                    Int16   iStyle;
                    Int16   iColour;
                    Int32   iitemindex;

                    // Try and convert Class, Vendor, Style, Color, Size
                    Int16.TryParse(Convert.ToString(dgvPOlines.Rows[e.RowIndex].Cells["Class"].Value),  out iItem);
                    Int32.TryParse(Convert.ToString(dgvPOlines.Rows[e.RowIndex].Cells["Vendor"].Value), out iVendor);
                    Int16.TryParse(Convert.ToString(dgvPOlines.Rows[e.RowIndex].Cells["Style"].Value),  out iStyle);
                    Int16.TryParse(Convert.ToString(dgvPOlines.Rows[e.RowIndex].Cells["Color"].Value),  out iColour);
                    
                    // Column header sorting issue. Use the sequence number instead of 
                    // itemindex
                    iitemindex = _polinedetails.Itemindex;
                    
                    _bDuplicateItem = CheckForDuplicateLine(iitemindex, iItem, iVendor, iStyle, iColour, e.FormattedValue.ToString());

                    if (String.IsNullOrEmpty(e.FormattedValue.ToString().Trim()))
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;

                        return;
                    }

                    List<string> retValues = validationcls.ValidateSize(e.FormattedValue.ToString());
                    if ("False" == retValues[0])
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
                            e.Cancel = false;
                            return;
                        }
                        
                        if (_polinedetails.ItemLookup(_porder.DbParamRef, _porder.UserName, _porder.Penvironment, _porder.DefaultMarket))
                        {
                            dgvPOlines.Rows[e.RowIndex].Cells["Description"].Value  = _polinedetails.Itemlongdescription;
                            dgvPOlines.Rows[e.RowIndex].Cells["Retail"].Value       = _polinedetails.Retailprice.ToString();
                            dgvPOlines.Rows[e.RowIndex].Cells["Cost"].Value         = _polinedetails.Cost.ToString();
                            dgvPOlines.Rows[e.RowIndex].Cells["Character"].Value    = _polinedetails.Characterdesc;
                            dgvPOlines.Rows[e.RowIndex].Cells["Season"].Value       = _polinedetails.SeasonDesc;
                            dgvPOlines.Rows[e.RowIndex].Cells["CasePackType"].Value = _polinedetails.Packdescription;
                            dgvPOlines.Rows[e.RowIndex].Cells["TicketType"].Value   = _polinedetails.Tickettype;
                            dgvPOlines.Rows[e.RowIndex].Cells["Pack"].Value         = _polinedetails.APP1;

                            // Add sequence number to datagrid to tie it with Po items collection
                            dgvPOlines.Rows[e.RowIndex].Cells["Sequence"].Value = _polinedetails.Sequence.ToString();

                            if (_polinedetails.APP1 == "Y")
                            {
                                _porder.NumofPOPacks += 1;
                                dgvPOlines.Rows[e.RowIndex].Cells["ConvertedCost"].ReadOnly = true;

                                AssortedPrePack AssPrePack = new AssortedPrePack(_polinedetails, _porder);
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

                            //_polinedetails.ConvertedCost = Decimal.Round((_polinedetails.Cost * _ccyratemarket) / _porder.ExchangeRate, 2);
                            _polinedetails.ConvertedCost = Decimal.Round(_polinedetails.Cost / _porder.ExchangeRate, 2);
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

                    // CJ: 25/02/2010 Note - Both Grid and the lines collection is changed
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

                    // We need to convert the Cost to 2 decimal positions incase of input errors
                    cost = Decimal.Round(cost, 2);

                    _polinedetails.ConvertedCost = cost;
                    dgvPOlines["ConvertedCost", e.RowIndex].Value = cost;

                    //_polinedetails.Cost = _polinedetails.ConvertedCost / _ccyratemarket * _porder.ExchangeRate;
                    _polinedetails.Cost = _polinedetails.ConvertedCost  * _porder.ExchangeRate;
                    dgvPOlines["Cost", e.RowIndex].Value = _polinedetails.Cost;

                    _polinedetails.LandedCost = Decimal.Round(_polinedetails.Cost * _porder.Landing,2);
                    dgvPOlines["LandedCost", e.RowIndex].Value = _polinedetails.LandedCost;

                    CalculatePOsummary();
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
                    
                    CalculatePOsummary();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source);
            }
        }

        private void dgvPOlines_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            // CJ: Reflect any changed Quantity values back into grid

            if (dgvPOlines.Rows[e.RowIndex].IsNewRow == true) return;

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

        private void dgvPOlines_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dgvPOlines.Rows[e.RowIndex].ErrorText = String.Empty;
        }

        //private void dgvPOlines_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        //{
        //    if (e.ColumnIndex == -1)
        //    {
        //        return;
        //    }
        //    if (!dgvPOlines.Rows[e.RowIndex].IsNewRow && dgvPOlines.Columns[e.ColumnIndex].Name == "Vendor") 
        //    {
        //        Int32 vendor;
        //        if (string.IsNullOrEmpty(txtVendor.Text))
        //            vendor = 0;
        //        else if (!Int32.TryParse(txtVendor.Text,out vendor))
        //            vendor = 0;
        //        if (e.Value != null && vendor != Convert.ToInt32(e.Value.ToString()))
        //            {
        //                _cellbackgroundcolor  = e.CellStyle.BackColor;
        //                e.CellStyle.BackColor = System.Drawing.Color.Red;
        //            }
        //        // Un highlight the even row
        //        if (e.RowIndex % 2 == 0 && e.Value != null && txtVendor.Text == e.Value.ToString())
        //        {
        //            //e.CellStyle.BackColor = _cellbackgroundcolor;
        //            e.CellStyle.BackColor = dgvcsPoLinesnormal.BackColor;
        //        }
        //        // Un highlight the odd row
        //        if (e.RowIndex % 2 != 0 && e.Value != null && txtVendor.Text == e.Value.ToString())
        //        {
        //            e.CellStyle.BackColor = dgvcsPoLinesalternate.BackColor;
        //        }
        //    }
        //}

        private void dgvPOlines_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            // When user clicks buttton 'Create PO', the 'cell validating' and 
            // 'row validating' is triggered for the new row in the datagrid/
            // Solution is not to validate a row in the grid that has no 
            // valid entry in object porder.lstpoLineItemDetails

            // Prevent Datagrid Validation if the user clicked Cancel button
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
                //Probably a good place to pack and finalize PO Summary Collection
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

        //private void dgvPOlines_RowValidated(object sender, DataGridViewCellEventArgs e)
        //{
        //    // CJ: Recalculate Line level LandedCost
        //    if (dgvPOlines.Rows[e.RowIndex].IsNewRow == false)
        //    {
        //        //_polinedetails.LandedCost = (_polinedetails.Cost * _ccyratemarket) / _ccyratepo * _porder.Landing;
        //        _polinedetails.LandedCost = Decimal.Round(_polinedetails.Cost * _porder.Landing,2);
        //        dgvPOlines["LandedCost", e.RowIndex].Value = _polinedetails.LandedCost;
        //    }
        //}
        //private void dgvPOlines_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        //{
        //    // Dont know the purpose of below code i.e why increment 
        //    // _porder.NumofPOLines by 1 if rowcount in grid != 1. Since toal number of lines
        //    // on a PO Header is always reported 1, I have decided to comment the below 'if' 
        //    // code block
        //    if (e.RowCount != 1)
        //    { 
        //        _porder.NumofPOLines += 1;
        //    }
        //}

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

                if (_porder.lstpoLineItemDetails.IndexOf(_polinedetails) == -1)
                {
                    _porder.lstpoLineItemDetails.Insert(e.Row.Index - 1, _polinedetails);
                    dgvPOlines["sequence", e.Row.Index].Value = nextsequencenumber;
                }

                _porder.NumofPOLines += 1;
            }
        }

        private void dgvPOlines_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (_porder.NumofPOLines >= 1) 
            {
                _porder.NumofPOLines = _porder.NumofPOLines - 1;
            }
        }
        #endregion Item DataGrid

        #region Hits
        private void btnHits_Click(object sender, EventArgs e)
        {
            Boolean bDisallowHits = false;
            int poitemcount = _porder.lstpoLineItemDetails.Count;

            if (poitemcount == 0)
            {
                bDisallowHits = true;
                MessageBox.Show("There are no lines on the PO. This prevents Hits being created.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (poitemcount > 0)
            {
                for (int i = 0; i < poitemcount; i++)
                {
                    if (_porder.lstpoLineItemDetails[i].IsValid == false)
                    {
                        bDisallowHits = true;
                        MessageBox.Show("You cannot enable Hits until the is one one valid PO Item!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }

            // Is there a invalid (half or incompletely entered) item
            if (bDisallowHits == true)
            {
                return;
            }

            POHits pohitsform = new POHits(_porder, _pohitscollection);

            // Subscribe to event handlers
            pohitsform.OnOkButtonClicked += new POHits.OkButtonClickedEventHandler(pohitsform_OnOkButtonClicked);
            pohitsform.OnCancelButtonClicked += new POHits.CancelButtonClickedEventHandler(pohitsform_OnCancelButtonClicked);

            //this.Hide();
            //pohitsform.Show();
            pohitsform.ShowDialog();

            pohitsform = null;
            
            UpdateNumberOfHits();
        }

        private int GetTotalNumberofHits()
        {
            int totalnumberofhits = 0;

            foreach (PurchaseOrder.POHits item in _pohitscollection)
            {
                if (item.HitActivated == true)
                {
                    totalnumberofhits++;
                }
            }

            return totalnumberofhits;
        }

        private void UpdateNumberOfHits()
        {
            int totalnumberofhits = 0;

            foreach (PurchaseOrder.POHits item in _pohitscollection)
            {
                if (item.HitActivated == true)
                {
                    totalnumberofhits++;
                }
            }

            txtNumberofHits.Text = totalnumberofhits.ToString();
        }

        void pohitsform_OnCancelButtonClicked(object sender, POHits.PoHitsEventArgs e)
        {
            _pohitscollection = e.poHitsCollection;
        }

        void pohitsform_OnOkButtonClicked(object sender, POHits.PoHitsEventArgs e)
        {
            _pohitscollection = e.poHitsCollection;
        }
        #endregion

        private Boolean CheckRequiredFields()
        {
            // Check to see if the PO Line Items are valid
            Boolean bSuccess = true;

            // Add Vendor and currency to validation
            // Check the Department code
            if (String.IsNullOrEmpty(txtDept.Text))
            {
                errPOEntry.SetError(txtShipVia, "Enter a valid department code");
                validationcls.HighlightErrControls(lblDept, txtDept, false);
                bSuccess = false;
            }

            if (String.IsNullOrEmpty(txtVendor.Text))
            {
                errPOEntry.SetError(txtVendor, "Enter a valid Vendor code");
                validationcls.HighlightErrControls(lblVendor, txtVendor, false);
                bSuccess = false;
            }

            if (String.IsNullOrEmpty(txtCurrency.Text))
            {
                errPOEntry.SetError(txtCurrency, "Enter a valid Currency code");
                validationcls.HighlightErrControls(lblCurrency, txtCurrency, false);
                bSuccess = false;
            }

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
            
            return bSuccess;
        }

        private Boolean CheckItemCount()
        {
            Boolean bSuccess = true;

            // Check whether PO Items are entered
            if (_porder.lstpoLineItemDetails.Count == 0)
            {
                bSuccess = false;
            }

            return bSuccess;
        }

        private Boolean ValidatePOlines()
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
                    bSuccess = false;
                }

                if (!item.IsValid)
                {
                    bSuccess = false;
                }

                if (item.Itemquantity.Equals(0))
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

        // Get number of invoices to create for Drop Ship Matrix
        private int GetInvoicesCountForDroShipMatrix()
        {
            int iquantity;
            //int iquantityassigned;
            int invoicecount = 0;
            string columnname;
            //string storename;

            if (dtDropShipMatrix.Rows.Count == 0)
            {
                return 0;
            }

            ///for (int i = 0; i < dtSelectedStores.Rows.Count; i++)
            foreach (DataRow dr in dtSelectedStores.Rows)
            {
                columnname = dr["clmStore"].ToString();
                iquantity = SumStoreQuantity(columnname);

                if (iquantity > 0)
                {
                    invoicecount++;
                }
            }

            return invoicecount;

        }

        private bool CreatePurchaseOrder(int iHitNumber)
        {
            if (_porder.CreatePoHeader(iHitNumber) && _porder.CreatePOComments(iHitNumber))
            {
                DataTable dtLines = PopulatePOLines(iHitNumber);

                if (_polinedetails.CreateOrderLines(_porder, dtLines) == true)
                {
                    //MessageBox.Show("Purchase Order Number" + _porder.Spiceponumber + "  created ");
                    return true;
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        private bool CreatePurchaseOrder()
        {            
            if (_porder.CreatePOHeader() && _porder.CreatePOComments())
            {
                DataTable dtLines = PopulatePOLines();

                if (_polinedetails.CreateOrderLines(_porder, dtLines) == true)
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

        private DataTable PopulatePOLines(int iHitNUmber)
        {
            DataTable dtAllPOLines = new DataTable();
            int iCount, poLineCount;

            dtAllPOLines.Columns.Add("POnumber",    typeof(string));
            dtAllPOLines.Columns.Add("Version",     typeof(Int16));  //Not populated
            dtAllPOLines.Columns.Add("Sequence",    typeof(Int16)); //HK : CJ : 10-12-2009 : Now populated
            dtAllPOLines.Columns.Add("Class",       typeof(Int16));
            dtAllPOLines.Columns.Add("Vendor",      typeof(Int32));
            dtAllPOLines.Columns.Add("Style",       typeof(Int16));
            dtAllPOLines.Columns.Add("Colour",      typeof(Int16));
            dtAllPOLines.Columns.Add("Size",        typeof(Int16));
            dtAllPOLines.Columns.Add("SKU",         typeof(Int32));
            dtAllPOLines.Columns.Add("SKUCHK",      typeof(Int16));
            // dtAllPOLines.Columns.Add("CheckDigit", typeof(Int16));
            dtAllPOLines.Columns.Add("UPC",         typeof(string));
            dtAllPOLines.Columns.Add("Quantity",    typeof(Int32));
            dtAllPOLines.Columns.Add("LandedCost",  typeof(decimal));
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
            //poLineCount = dgvPOlines.Rows.Count;
            poLineCount = _porder.lstpoLineItemDetails.Count;

            for (iCount = 0; iCount < poLineCount; iCount++)
            {
                _polinedetails = _porder.lstpoLineItemDetails[iCount];

                // HK : CJ : 26-11-2009 : Check if thsi is a Drop Ship Matrix PO.
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

                // HK : CJ : 26-01-2010 : Fix Bug 311. SeasonCode to be used instead of SeasonDesc
                dtAllPOLines.Rows.Add(_porder.SpicePOnumber,
                                      _porder.SpicePOversion, //Version
                                      _polinedetails.Sequence,
                                      _polinedetails.ClassCode,
                                      _polinedetails.Vendorcode,
                                      _polinedetails.Stylecode,
                                      _polinedetails.Colorcode,
                                      _polinedetails.Itemsize,
                                      _polinedetails.Sku,
                                      _polinedetails.SkuChk,
                                      _polinedetails.UPC,
                                      _polinedetails.Itemquantity,
                                      _polinedetails.LandedCost,
                               //Decimal.Round((_polinedetails.Cost * _currencyratemarket) * _porder.Landing, 2), // LandedCost. HK : 11-12-2009 : This has to be the "LandedCost"
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
                               //_polinedetails.Cost,             //Simple Vendor Cost. HK : 11-12-2009 : Looks like this is to be assigned the "ConvertedCost" 
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

            DataTable dtAllPOLines = new DataTable ();
            int iCount, poLineCount;

            dtAllPOLines.Columns.Add("POnumber",   typeof(string));
            dtAllPOLines.Columns.Add("Version",    typeof(Int16));  // HK : Now populated
            dtAllPOLines.Columns.Add("Sequence",   typeof(Int16)); // HK : CJ : 10-12-2009 : Now populated
            dtAllPOLines.Columns.Add("Class",      typeof(Int16));
            dtAllPOLines.Columns.Add("Vendor",     typeof(Int32));
            dtAllPOLines.Columns.Add("Style",      typeof(Int16));
            dtAllPOLines.Columns.Add("Colour",     typeof(Int16));
            dtAllPOLines.Columns.Add("Size",       typeof(Int16));
            dtAllPOLines.Columns.Add("SKU",        typeof(Int32));
            dtAllPOLines.Columns.Add("SKUCHK",     typeof(Int16));       
            // dtAllPOLines.Columns.Add("CheckDigit", typeof(Int16));
            dtAllPOLines.Columns.Add("UPC",        typeof(string));
            dtAllPOLines.Columns.Add("Quantity",   typeof(Int32));
            dtAllPOLines.Columns.Add("LandedCost", typeof(decimal));
            dtAllPOLines.Columns.Add("Retail",     typeof(decimal));
            dtAllPOLines.Columns.Add("LongDesc",   typeof(string));
            dtAllPOLines.Columns.Add("ShortDesc",  typeof(string));
            dtAllPOLines.Columns.Add("VendorStyle", typeof(string));
            dtAllPOLines.Columns.Add("Season",     typeof(string));
            dtAllPOLines.Columns.Add("SubClass",   typeof(string));
            dtAllPOLines.Columns.Add("Ticket",     typeof(string));
            dtAllPOLines.Columns.Add("CaseQuantity",typeof(Int32));
            dtAllPOLines.Columns.Add("DistroQty",   typeof(Int32));
            dtAllPOLines.Columns.Add("VendorCost", typeof(decimal));
            dtAllPOLines.Columns.Add("LandFactor", typeof(decimal));
            dtAllPOLines.Columns.Add("Character",  typeof(string)); // Character code

            // Total rowcount in the grid (The rows to process is one less than total rows)
            //poLineCount = dgvPOlines.Rows.Count;
            poLineCount = _porder.lstpoLineItemDetails.Count;

            for (iCount = 0; iCount < poLineCount ; iCount++)
            {
                _polinedetails = _porder.lstpoLineItemDetails[iCount];

                // HK : CJ : 26-11-2009 : Check if this is a Drop Ship Matrix PO.
                // If it is then replace the item quantity in the PO Line Item with the 
                // one from the Drop Ship Matrix datatable.

                if (rdBtnDropShipMatrix.Checked)
                {
                    int qty = _porder.GetItemQuantityForStore(_porder.ShipTo.ToString(),
                                                                      _polinedetails.Itemindex,
                                                                      _polinedetails.ClassCode,
                                                                      _polinedetails.Vendorcode,
                                                                      _polinedetails.Stylecode,
                                                                      _polinedetails.Colorcode,
                                                                      _polinedetails.Itemsize);

                    _polinedetails.Itemquantity = qty;
                }

                // Fix Bug 311. SeasonCode to be used instead of SeasonDesc
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

            //bool bInvalid;

            //////Check all fields are non empty to ensure complete order header is valid
            //////Individual fields check their values themselves

            ////if (String.IsNullOrEmpty(txtDept.Text) ||
            //// String.IsNullOrEmpty(txtVendor.Text) ||
            //// String.IsNullOrEmpty(txtCurrency.Text) ||
            //// String.IsNullOrEmpty(txtTerms.Text) ||
            //// String.IsNullOrEmpty(txtLanding.Text) ||
            //// String.IsNullOrEmpty(txtShipVia.Text))
            ////{

            ////    bInvalid = false;

            ////}
            ////else if (txtShipVia.Text == OCEANSHIPVIACODE)
            ////{
            ////    if (String.IsNullOrEmpty(txtPortofEntry.Text) ||
            ////       String.IsNullOrEmpty(txtPortofDeparture.Text) ||
            ////       String.IsNullOrEmpty(txtDelTerms.Text))
            ////    { bInvalid = false; }
            ////    else
            ////    { bInvalid = true; }
            ////}
            ////else
            ////{
            ////    bInvalid = true;

            ////}

            ////return bInvalid;

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
        }

        private void ImportControlChanges()
        {
            if (txtShipVia.Text == OCEANSHIPVIACODE)
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

                // If user selected 'OCN' and then 'ROD' we must 
                // blank out the description of Port of Departure, Port of Entry and 
                // Delivery Terms
                lblDeparturePortDesc.Text   = "";
                lblEntryPortDesc.Text = "";
                lblDeliveryTermsDesc.Text = "";

                // We could reset Validation highlights here. If we disable the controls
                // then we must un hilight the previous validation error hilighted.
                validationcls.HighlightErrControls(lblPortofDeparture, txtPortofDeparture, true);
                errPOEntry.SetError(txtPortofDeparture, "");

                validationcls.HighlightErrControls(lblPortofEntry, txtPortofEntry, true);
                errPOEntry.SetError(txtPortofEntry, "");

                validationcls.HighlightErrControls(lblDelTerms, txtDelTerms, true);
                errPOEntry.SetError(txtDelTerms, "");

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

        //    for (int i = 0; i < dgvPOlines.Rows.Count; i++)
        //    {
        //        if (dgvPOlines.Rows[i].IsNewRow == false)
        //        {
        //            iitemindex = dgvPOlines.Rows[i].Index;
        //            // Try and convert Class, Vendor, Style, Color, Size
        //            Int16.TryParse (Convert.ToString(dgvPOlines.Rows[i].Cells["Class"].Value), out iClass);
        //            Int32.TryParse (Convert.ToString(dgvPOlines.Rows[i].Cells["Vendor"].Value), out iVendor);
        //            Int16.TryParse (Convert.ToString(dgvPOlines.Rows[i].Cells["Style"].Value), out iStyle);
        //            Int16.TryParse (Convert.ToString(dgvPOlines.Rows[i].Cells["Color"].Value), out iColor);
        //            Int16.TryParse (Convert.ToString(dgvPOlines.Rows[i].Cells["Size"].Value), out iSize);
        //            description =  Convert.ToString(dgvPOlines.Rows[i].Cells["Description"].Value);
        //        }
        //    }
        //}

        private short GetNextSequenceNumber()
        {
            int     totalrowsinitemslist;
            short   sequencenumberonlastitem;
            // HK : Sequence number always start at 1
            short   nextsequencenumber = 1;

            // Get the last item in list
            totalrowsinitemslist = _porder.lstpoLineItemDetails.Count;

            if (_porder.lstpoLineItemDetails.Count > 0)
            {
                sequencenumberonlastitem = _porder.lstpoLineItemDetails[totalrowsinitemslist - 1].Sequence;
                Debug.Print ("Sequence Number on last item:" + sequencenumberonlastitem.ToString ());

                if (sequencenumberonlastitem >= 1)
                {
                    sequencenumberonlastitem++;
                    nextsequencenumber = sequencenumberonlastitem;
                    Debug.Print("NEXT Sequence Number generated:" + nextsequencenumber.ToString());
                }
            }

            return nextsequencenumber;
        }

        private short GetMaxSequenceNumber()
        {
            // HK : 26-01-2010 : Get the next sequence number.
            // The next sequence number is the maximum sequence number + 1

            short nextsequencenumber = 0;
            short tempsequencenumber;

            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                tempsequencenumber = _porder.lstpoLineItemDetails[i].Sequence;

                if (tempsequencenumber > nextsequencenumber)
                {
                    nextsequencenumber = tempsequencenumber;
                }
            }

            nextsequencenumber++;
            
            return nextsequencenumber;
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
                    if (dgvPOlines.Rows[iLoopCounter].Cells[0].Value != null &&
                                Convert.ToBoolean(dgvPOlines.Rows[iLoopCounter].Cells[0].Value) == true)
                    {
                        Debug.Print("Data Grid row removed at index: " + iLoopCounter.ToString());
                        DisplayDataGridItems(iLoopCounter);
                        
                        // The below RemoveAt will cause a row enter event to fire.
                        // As we delete the row at iLoopCounter, the nex valid row (if any) will become 
                        // the current row.
                        dgvPOlines.Rows.RemoveAt(iLoopCounter);

                        // HK : 30-11-2009 : If Item is a APP then decrement the 
                        // pack count

                        if (_porder.lstpoLineItemDetails[iLoopCounter].APP1 == "Y")
                        {
                            _porder.NumofPOPacks = _porder.NumofPOPacks - 1;
                        }

                        // Display the items on the class object in the collection about to be deleted
                        //Debug.Print("Item Collection to be deleted: " + iLoopCounter.ToString());
                        //DisplayLineItemDetails(_porder.lstpoLineItemDetails[iLoopCounter]);
                        
                        _porder.lstpoLineItemDetails.RemoveAt(iLoopCounter);
                        //Debug.Print("Item Collection count: " + _porder.lstpoLineItemDetails.Count.ToString());

                        iRowsDeleted++;
                        iRunningCountTotalRows = dgvPOlines.Rows.Count;
                    }
                    else
                    {
                        DisplayDataGridItems(iLoopCounter);
                        iLoopCounter++;
                    }
                }
                else
                {
                    iLoopCounter++;
                }

            } while (iLoopCounter < iRunningCountTotalRows);
            
            _bUserWantsToDeleteLine = false;

            // After all selected rows have been deleted and the 
            // items collection has been updated then calculate PO summary
            // Calculate Po Summary if any records were deleted.
            // Note : ASH originally did this from the RowsDeleteting event handler
            if (iRowsDeleted > 0)
            {
                CalculatePOsummary();
            }

            // Display the contents of the grid and the items collection to verify that they are consistent
            //DisplayDataGridItems();
        }

        private void txtDelTerms_Validating(object sender, CancelEventArgs e)
        {
            // HK : 01-12-2009 : Dont validate when user is trying to close the form
            if (!_bFormCancelClicked)
            {
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

        //private bool ValidateQuantity(Int32 quantity, int packQty, int rowindex)
        //{
        //    if (quantity % packQty != 0)
        //    {
        //        // Must send the datagridview row index, otherwise it will not know 
        //        // which record to update
        //        ItemQuantityForm itemqtyform = new ItemQuantityForm(quantity, packQty, ref _polinedetails, rowindex);

        //        itemqtyform.OnQuantityRounded += new ItemQuantityForm.QuantityRoundedEventHandler(itemqtyform_OnQuantityRounded);
        //        if (itemqtyform.ShowDialog(this) == DialogResult.OK)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else return true;
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

            //iTemp = Convert.ToInt32(dtDropShipMatrix.Compute(sstore, sfilterexpr));

            for (int i = 0; i < dtDropShipMatrix.Rows.Count; i++)
            {
                iTemp = Convert.ToInt32(dtDropShipMatrix.Rows[i][sstorecolumnname]);
                istorequantity = istorequantity + iTemp;
            }

            return istorequantity;
        }

        // Checks for duplicate items and return the 
        // first item found that is duplicate
        // The collection lstpoLineItemDetails will hold all the 
        // Po Line Items entered so far
        //private Boolean CheckForDuplicateLine(short itemclass, int vendor, short style, short colour, string size, ref int rowid)
        //{
        //    // HK ?? Could be implemented using Find and Findall for custom collection
        //    // on the class POItemDetails and class PurchaseOrder

        //    // Find will return the index of the found item
        //    // FinaAll will return a List<POItemDetails> of all items
        //    // that match

        //    return false;
        //}

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

        private void AddItemsControler(DataTable dtselectedstems)
        {
            int iStartRow;
            int iTotalRowsInGrid;
            short nextsequencenumber;

            if (dtselectedstems.Rows.Count > 0)
            {
                iStartRow = iTotalRowsInGrid = dgvPOlines.Rows.Count;

                foreach (DataRow dr in dtselectedstems.Rows)
                {

                    POItemDetails poitem = new POItemDetails((short)dr["class"], (int)dr["vendor"],
                                                    (short)dr["style"], (short)dr["colour"],
                                                    (short)dr["size"], iStartRow + 1);

                    // HK : CJ : 10-12-2009 : Property ItemIndex on class  POItemDetails should be 
                    // the same as the datagrid row index for that item
                    nextsequencenumber = GetNextSequenceNumber();
                    poitem.Sequence = nextsequencenumber;

                    // HK : 14-12-2009 : Multi thread the Item Lookup and free up the UI
                    // Comment this if you want to revert to original method
                    ParameterizedThreadStart job = new ParameterizedThreadStart(ItemLookUp);
                    Thread myThread = new Thread(job);

                    myThreadParms mythreadparms = new myThreadParms(iStartRow, poitem);
                    myThread.Start(mythreadparms);

                    object [] parms = {iStartRow, poitem };

                    //myThread.Join();
                }
            }
        }

        // Marshall the update of the datagrid using this method call
        //private void UpdateGrid (POItemDetails poitem)
        //{
        //}

        //HK : 11-12-2009 : MultiThread the ItemLookup and free the GUI
        
        private void ItemLookUp(object myThreadParms)
        {
            Boolean bSuccess;
            myThreadParms mythreadparms = myThreadParms as myThreadParms;
            int rowindex = mythreadparms.RowIndex;
            POItemDetails item = mythreadparms.PoItem;

            object [] o = (object [])myThreadParms;

            // 1.
            // Loop through the datatable and pass each new PO Items object
            // to a method which is invoded Asynchronously or in a different thread

            // 2.
            // In another process, display the item details on the datagrid
            // as they are retrieved.
            // 2.1 Keep a common place where a list is built of items as they are looked up.
            // 2.2 Also keep a a variable of the current item place in the list
            // 2.3 Keep a counter of the last datagrid record for which the item details were displayed

            bSuccess = item.ItemLookup(_porder.DbParamRef, _porder.UserName, 
                                _porder.Penvironment, _porder.DefaultMarket);

            if (bSuccess == true)
            {
                dgvPOlines.Rows[rowindex].Cells["Description"].Value = item.Itemlongdescription;
                dgvPOlines.Rows[rowindex].Cells["Retail"].Value = item.Retailprice.ToString();
                dgvPOlines.Rows[rowindex].Cells["Cost"].Value = item.Cost.ToString();
                dgvPOlines.Rows[rowindex].Cells["Character"].Value = item.Characterdesc;
                dgvPOlines.Rows[rowindex].Cells["Season"].Value = item.SeasonDesc;
                dgvPOlines.Rows[rowindex].Cells["CasePackType"].Value = item.Packdescription;
                dgvPOlines.Rows[rowindex].Cells["TicketType"].Value = item.Tickettype;
                //This will determine if the qty and cost can be changed 
                dgvPOlines.Rows[rowindex].Cells["Pack"].Value = item.APP1;

                // For some strange reason 
                //dgvPOlines.Rows[iStartRow].Cells["colSelect"].Value = false;

                // HK : 16-11-2009 : Display Converted cost and hide actual cost from database

                dgvPOlines.Rows[rowindex].Cells["ConvertedCost"].Value = Decimal.Round((item.Cost * _ccyratemarket) / _porder.ExchangeRate, 2);

                if (item.APP1 == "Y")
                {
                    //Make the UnitCost Readonly
                    dgvPOlines.Rows[rowindex].Cells["Cost"].ReadOnly = true;
                }
                else
                {
                    //Not an APP
                    //Enable cost
                    dgvPOlines.Rows[rowindex].Cells["Cost"].ReadOnly = false;
                }

                _porder.lstpoLineItemDetails.Add(item);
            }
        }

        public class myThreadParms
        {
            POItemDetails _poitem;
            int _rowindex;

            public int RowIndex
            {
                get { return _rowindex; }
                set { _rowindex = value; }
            }

            public POItemDetails PoItem
            {
                get { return _poitem; }
                set { _poitem = value; }
            }

            public myThreadParms(int rowindex, POItemDetails poitem)
            {
                _rowindex = rowindex;
                _poitem = poitem;
            }
        }

        private void LoadCurrencyFromXMLDocument()
        {
            _dtCurrency = null;

            _dtCurrency = new DataTable("Currency");

            _dtCurrency.Columns.Add("CurrencyCode", typeof(string));
            _dtCurrency.Columns.Add("CurrencyName", typeof(string));
            _dtCurrency.Columns.Add("CurrencyRate", typeof(decimal));
            _dtCurrency.Columns.Add("CurrencyCodeName", typeof(decimal));
        }

        private void HilightDuplicateLines()
        {
            for (int i = 0; i < dgvPOlines.Rows.Count; i++)
            {

            }
        }

        private void SetItemColor (int columnindex, int rowindex, Color color)
        {
            dgvPOlines["Class", rowindex].Style.BackColor = color;
            dgvPOlines["Vendor", rowindex].Style.BackColor = color;
            dgvPOlines["Style", rowindex].Style.BackColor = color;
            dgvPOlines["Color", rowindex].Style.BackColor = color;
            dgvPOlines["Size", rowindex].Style.BackColor = color;
        }

        private Boolean ValidateHeaderFields(object sender)
        {
            TextBox txtBox = (TextBox)sender;

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
                    
                    // Can Add item only after Header is valid
                    btnAddItem.Enabled = true;

                    dtpkrAnticipateDate.Focus();

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

                // Can Add item only after Header is valid
                btnAddItem.Enabled = true;

                return true;
            }

            return false;
        }

        private void DisplayDataGridItems(int rowindex)
        {
            char padchar = Convert.ToChar(" ");

            int iitemindex;
            short iClass;
            int iVendor;
            short iStyle;
            short iColor;
            short iSize;
            String description;

            // HK : 12-01-2010 : Fix Bug 213. Need to do a TryParse to get values from the datagrid cells
            // into the variables.
            /*
            iitemindex  = _porder.lstpoLineItemDetails[rowindex].Itemindex;
            iClass      = Convert.ToInt16(dgvPOlines.Rows[rowindex].Cells["Class"].Value);
            iVendor     = Convert.ToInt32(dgvPOlines.Rows[rowindex].Cells["Vendor"].Value);
            iStyle      = Convert.ToInt16(dgvPOlines.Rows[rowindex].Cells["Style"].Value);
            iColor      = Convert.ToInt16(dgvPOlines.Rows[rowindex].Cells["Color"].Value);
            iSize       = Convert.ToInt16(dgvPOlines.Rows[rowindex].Cells["Size"].Value);
            */
            // Try and convert Class, Vendor, Style, Color, Size
            iitemindex = rowindex;
            Int16.TryParse(Convert.ToString(dgvPOlines.Rows[rowindex].Cells["Class"].Value),  out iClass);
            Int32.TryParse(Convert.ToString(dgvPOlines.Rows[rowindex].Cells["Vendor"].Value), out iVendor);
            Int16.TryParse(Convert.ToString(dgvPOlines.Rows[rowindex].Cells["Style"].Value),  out iStyle);
            Int16.TryParse(Convert.ToString(dgvPOlines.Rows[rowindex].Cells["Color"].Value),  out iColor);
            Int16.TryParse(Convert.ToString(dgvPOlines.Rows[rowindex].Cells["Size"].Value),   out iSize);
            description = Convert.ToString (dgvPOlines.Rows[rowindex].Cells["Description"].Value);

            if (dgvPOlines.Rows[rowindex].IsNewRow == false)
            {
                //Debug.Print("Item Index: "          + iitemindex.ToString().PadRight(6, padchar)
                //              + " Class Code: "     + iClass.ToString().PadRight(6, padchar)
                //              + " Vendor Code: "    + iVendor.ToString().PadRight(6, padchar)
                //              + " Style Code: "     + iStyle.ToString().PadRight(6, padchar)
                //              + " Color Code: "     + iColor.ToString().PadRight(6, padchar)
                //              + " Size Code: "      + iSize.ToString().PadRight(6, padchar)
                //              + " Description: "    + description.ToString()
                //              + "");
            }
        }

        private void cbxFreight_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxFreight.SelectedItem != null)
            {
                _porder.Freight = GetFreightId(cbxFreight.SelectedIndex);
            }
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

                default :
                    return "";
            }
        }

        private void cmbSSD_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSSD.SelectedItem != null)
            {
                _porder.Ssd = Convert.ToDateTime(cmbSSD.SelectedItem);
            }
        }

        private Boolean IsOkToDoItemLookup(int itemindex, short itemclass, int vendor, short style, short color, short size)
        {
            // Ignore ItemIndex for the time being
            if ( IsItemClassChanged ||
                 IsItemVendorChanged ||
                 IsItemStyleChanged ||
                 IsItemColorChanged ||
                 IsItemSizeChanged )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int GetItemCollectionIndexForDataGridRow(int sequence)
        {
            // HK : 25-01-2010 : Sorting on column header issue
            // Use sequence number instead of itemindex. Itemindex is based
            // on the datagrid rowindex and chan change thereby corrupting 
            // the position of the item in the correspondinng po collectin.
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
            _porder.Vendorcomments1 = txtVendorComment1.Text;
        }

        private void txtVendorComment2_Validating(object sender, CancelEventArgs e)
        {
            _porder.Vendorcomments2 = txtVendorComment2.Text;
        }

        private void txtVendorComment3_Validating(object sender, CancelEventArgs e)
        {
            _porder.Vendorcomments3 = txtVendorComment3.Text;
        }

        private void txtInternalComments1_Validating(object sender, CancelEventArgs e)
        {
            _porder.Internalcomments1 = txtInternalComments1.Text;
        }

        private void txtInternalComments2_Validating(object sender, CancelEventArgs e)
        {
            _porder.Internalcomments2 = txtInternalComments2.Text;
        }
        #endregion Comments

        #region Buttons
        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dgvPOlines.Rows[0].Height = 50;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dgvPOlines.Rows[1].ErrorText = "Please enter valid " + dgvPOlines.Columns[4].Name;
        }
        
        private void btnAddItem_Click(object sender, EventArgs e)
        {
            Int16 itemclass;
            Int16 dept;
            Int32 vendor;
            string itemclasslck = "N";
            string deptlck = "Y";
            string vendorlck = "N";
            Int32 iTotalRowsInGrid;
            Int32 iStartRow;
            Int16 nextsequencenumber;

            DataTable dtSelectedItems;

            Int16 iClass;
            Int32 iVendor;
            Int16 iStyle;
            Int16 iColour;
            Int16 iSize;

            itemclass = 0;
            dept = Convert.ToInt16(txtDept.Text);

            if (string.IsNullOrEmpty(txtVendor.Text))
            {
                vendor = 0;
            }
            else if (!Int32.TryParse(txtVendor.Text, out vendor))
            {
                vendor = 0;
            }

            Disney.Spice.ItemsUI.SelectItem frmSelectItem = new SelectItem(_porder.DbParamRef, _porder.UserName,
                                                                            _porder.Penvironment,
                                                                            _mdiparent, itemclass, itemclasslck,
                                                                            dept, deptlck, vendor, vendorlck);

            dtSelectedItems = frmSelectItem.GetSelectedItems();

            char padchar = Convert.ToChar(" ");
            Boolean bItemLookupError = false;

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
                        iStartRow = dgvPOlines.Rows.Add(false, (Int16)dr["class"],
                                                                      dr["vendor"],
                                                                      dr["style"],
                                                                      dr["colour"],
                                                                      dr["size"]);

                        dgvPOlines.Rows[iStartRow].Cells["Description"].Value = poitem.Itemlongdescription;
                        dgvPOlines.Rows[iStartRow].Cells["Retail"].Value = poitem.Retailprice.ToString();
                        dgvPOlines.Rows[iStartRow].Cells["Cost"].Value = poitem.Cost.ToString();
                        dgvPOlines.Rows[iStartRow].Cells["Character"].Value = poitem.Characterdesc;
                        dgvPOlines.Rows[iStartRow].Cells["Season"].Value = poitem.SeasonDesc;
                        dgvPOlines.Rows[iStartRow].Cells["CasePackType"].Value = poitem.Packdescription;
                        dgvPOlines.Rows[iStartRow].Cells["TicketType"].Value = poitem.Tickettype;
                        dgvPOlines.Rows[iStartRow].Cells["Pack"].Value = poitem.APP1;
                        dgvPOlines.Rows[iStartRow].Cells["Sequence"].Value = poitem.Sequence.ToString(); ;

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

                        //poitem.ConvertedCost = Decimal.Round((poitem.Cost * _ccyratemarket) / _porder.ExchangeRate, 2);
                        poitem.ConvertedCost = Decimal.Round(poitem.Cost / _porder.ExchangeRate, 2);
                        dgvPOlines.Rows[iStartRow].Cells["ConvertedCost"].Value = poitem.ConvertedCost;

                        poitem.LandedCost = poitem.Cost * _porder.Landing;
                        dgvPOlines.Rows[iStartRow].Cells["LandedCost"].Value = poitem.LandedCost;

                        _porder.lstpoLineItemDetails.Add(poitem);
                    }
                    else
                    {
                        poitem.IsValid = false;
                        bItemLookupError = true;
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
            }
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            _bFormCancelClicked = true;

            // HK : 25-01-2010 : Fix Bug 319
            DialogResult dlgResult = MessageBox.Show("Are you sure you want to Cancel this PO Entry?", "SPICE PO Entry", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (dlgResult == DialogResult.Yes)
            {
                this.Close();
            }
            else
            {
                _bFormCancelClicked = false;
            }
        }
        
        private void btnCreatePO_Click(object sender, EventArgs e)
        {
            try
            {
                this.ValidateChildren();
                if (CheckRequiredFields() == false)
                {
                    MessageBox.Show("Not enough information is available to create the PO", "SPICE PO MANAGEMENT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (CheckItemCount() == false)
                {
                    MessageBox.Show("This PO cannot be created as there are no Po Lines.", "SPICE PO MANAGEMENT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (ValidatePOlines() == false)
                {
                    MessageBox.Show("Item quantity cannot be 0, \r\n or Item number is invalid", "SPICE PO MANAGEMENT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check that the Quantiy = Quantity Assigned
                if (rdBtnDropShipMatrix.Checked)
                {
                    if (!CheckDropShipMatrixQuantity())
                    {
                        MessageBox.Show("Store quantity does not match item quantity!", "SPICE PO MANAGEMENT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                if (rdBtnDropShipSingle.Checked)
                {
                    if (dtSelectedStores.Rows.Count == 0)
                    {
                        MessageBox.Show("No store(s) have been selected \n\r\n\r This PO cannot be created.", "SPICE PO MANAGEMENT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
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
                        _numberofPurchaseOrders = GetInvoicesCountForDroShipMatrix();
                        break;
                }


                // Total number of PO created = 1 PO from the Main PO Entry form and total number of hits
                int totalnumberofhits = GetTotalNumberofHits();
                int totalpostocreate = totalnumberofhits + _numberofPurchaseOrders;
                string pomessage = " PO";

                if (totalpostocreate > 1)
                {
                    pomessage = pomessage + "s";
                }

                if (MessageBox.Show("You are about to create " + totalpostocreate.ToString() + pomessage + " in the SPICE Database."
                                + "\r\n"
                                + "\r\n"
                                + "Do you want to continue? ", "SPICE - PO Entry - Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    pwindow = new ProgressWindow(_numberofPurchaseOrders);
                    pwindow.Show();
                    bkgrndWorker.RunWorkerAsync();
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
            int istorequantity = 0;
            
            foreach (DataRow dtrow in dtSelectedStores.Rows)
            {
                // Check if the quantity assigned every PO Line Item 
                // for that store in question is > 0. If not then skip this store (ie. do not 
                // create a PO for this store)
                if (rdBtnDropShipMatrix.Checked)
                {
                    sStore = dtrow["clmStore"].ToString ();
                    istorequantity = SumStoreQuantity(sStore);

                    if (istorequantity == 0)
                    {
                        continue;
                    }
                }
                
                _porder.ShipTo = Convert.ToInt16(dtrow["clmStore"].ToString());
                index++;

                // Assigne our Drop Ship Matrix datatable
                _porder.dtDropShipMatrix = dtDropShipMatrix;

                // HK : 02-12-2009 : Assign the PO Hits collection
                if (rdBtnDCPO.Checked)
                {
                    if (_pohitscollection.Count > 0 )
                    {
                        _porder.poHitsCollection = _pohitscollection;
                    }
                }

                // Drop Ship Single, Drop Ship Matrix, Standard PO
                if (!CreatePurchaseOrder())
                {
                    e.Cancel = true;
                    break;
                }

                // HK : CJ : 02-12-2009 : If Hits have been activated 
                // then loop through the Hits to create the PO 
                // for the Hits

                // HK : 09-01-2010 :  The _porder.poHitsCollection property will
                // not get set if rdBtnDCPO.Checked = false
                if (rdBtnDCPO.Checked)
                {
                    foreach (PurchaseOrder.POHits item in _porder.poHitsCollection)
                    {
                        int ihitnumber = item.HitNUmber;
                        int itotalquantityonhit;

                        // Only create the Hit if it is activated
                        // Unit Test case 1 : If sum (quantiy) on 
                        // a particular hit is 0 then no need to create the hit
                        itotalquantityonhit = _porder.GetTotalUnitsForHit(ihitnumber);

                        if (item.HitActivated && itotalquantityonhit > 0)
                        {
                            if (!CreatePurchaseOrder(ihitnumber))
                            {
                                e.Cancel = true;
                                break;
                            }
                        }
                    }
                }

                bkgrndWorker.ReportProgress(index);
            }
        }
        
        private void bkgrndWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pwindow.UpdateProgressBar(e.ProgressPercentage);    
        }

        private void bkgrndWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                MessageBox.Show("There was an error with your order please contact Support", "SPICE PO MANAGEMENT");
            }

            pwindow.Close();
            this.Close();
        }
        #endregion
    }
}