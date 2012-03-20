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
    public partial class frmPOModification : Form
    {
        private PurchaseOrder                   _porder;
        private Enquiry                         _vendorlookup;
        private Enquiry                         _deptlookup;
        private LookupBO                        lookupbo;
        private Validation                      validationcls;
        private Disney.Spice.ItemsBO.Items      _iteminstance;
        private POItemDetails                   _polinedetails;
        private const string                    MAGICDCSTORE = "723";
        private const int                       MINLANDINGVALUEFOROCN = 1;
        private const string                    OCEANSHIPVIACODE = "OCN";
        POLineForm                              polineform;
        private DataTable dtSelectedStores =null;
        private ProgressWindow                  pwindow;

        // //////////////////////////////////////////////////////////
       // HHK : 11-11-2009
        // Capture the Quantity rounded UP or DOWN by the user
        // When 0 then the user pressed 'Cancel' button
        private int _itemquantityrounded = 0;

        // //////////////////////////////////////////////////////////

        // HK : 16-11-2009 : Add a stub for Store VAt Code
        private const string MAGICDCSTOREVATCODE = "A";

        private Boolean _bFormCancelClicked;
        private Boolean _bOkToValidate = true;
        private Boolean _bUserWantsToDeleteLine;
        private Boolean _bDuplicateItem;

        private DataTable   _dtCurrency = null;
        private decimal     _currencyratemarket;
        private decimal     _currencyratepo;
        private Color       _cellbackgroundcolor;
        
        private DataGridViewCellStyle   dgvcsPoLinesnormal;
        private DataGridViewCellStyle   dgvcsPoLinesalternate;

        private DataTable   dtDropShipMatrix        = null;
        private string      _sStoreColumnNamePrefix = "Store_";

        // PO Hits stuff
        private DataTable dtPoHits = null;
        private PurchaseOrder.POHits _pohits;
        private PurchaseOrder.PoHitsCollection _pohitscollection;

        private string _defaultshipvia      = String.Empty;

        private Form _mdiparent;

        // //////////////////////////////////////////////////////////////
        // HK : CJ : 08-12-2009 : Capture the constructor parms
        ASNA.VisualRPG.Runtime.Database         _dbparamref;
        Disney.Menu.Users                       _username;
        Disney.Menu.Environments                _paramenv;
        private string                          _spiceponumber;
        // //////////////////////////////////////////////////////////////

        // //////////////////////////////////////////////////////////////
        // HK : 05-01-2010 : Rollback PO stuff
        private PurchaseOrder _porderoriginal;

        // //////////////////////////////////////////////////////////////

        Boolean         _bDataBindingsInitalised;
        DataTable       _dtPoLines;

        // HK : 17-12-2009 : Freight functionality
        DataTable dtFreight = new DataTable("Freight");

        // HK : 18-01-2010 : Fix Bug 253 :
        Boolean IsItemClassChanged;
        Boolean IsItemVendorChanged;
        Boolean IsItemStyleChanged;
        Boolean IsItemColorChanged;
        Boolean IsItemSizeChanged;

        // HK : Fix Bug 254
        Boolean IsPoModified;

        // HK : 21-01-2010 : Fix Bug 256 : 
        string _currencyformat = "N";              // Standard .NET culture aware currency format mask
        string _currencyformat1 = "#,#,##,###,##"; // User forced non culture aware currency format mask 

        // HK : JU : 08-2009 : Create custom constructor for Po Modification as it is directly called 
        // from the double click of the main launch form
        public frmPOModification(ASNA.VisualRPG.Runtime.Database dbparamref, Disney.Menu.Users username, Disney.Menu.Environments paramenv, string spiceponumber)
        {
            InitializeComponent();

            this.MaximizeBox = false;

            // HK : CJ : 19-11-2009 : 
            //_mdiparent = owner;
            _mdiparent = null;

            // ////////////////////////////////////////////////////////////////
            // HK : CJ : 08-12-2009 : Create the Purchase Order and Po Itemd Details class
            _dbparamref = dbparamref;
            _username   = username;
            _paramenv   = paramenv;
            _spiceponumber = spiceponumber;

            SetupClassObjects();
            // ////////////////////////////////////////////////////////////////

            SetupInitialValues();

        }

        
        public frmPOModification(PurchaseOrder purchaseorder, Form owner)
        {

            InitializeComponent();

            //Initialise the local variable so that its available to ROW
            _porder = purchaseorder;

            this.MaximizeBox = false;

            // HK : CJ : 19-11-2009 : 
            //_mdiparent = owner;
            _mdiparent = null;
                   
            SetupInitialValues();

        }
        
        public frmPOModification()
        {

            InitializeComponent();

        }

        #region POHeader

        #region Vendor

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

                        // ////////////////////////////////////////////////
                        // HK : FC : BM : CJ : 03-12-2009
                        // Returns OCNFRT which is not a valid ShipVia Type Data incorrect.
                        // Data in file is wrong. So validate the data against the 
                        // validation class and if it is wrong then do not display 
                        // the code
                        // ///////////////////////////////////////////////
                        List<string> lstretvalues_1 = new List<string>();

                        lstretvalues_1 = validationcls.ValidateShipVia(lstReturn[2]);

                        if (lstretvalues_1[0] == "True")
                        {
                            // ////////////////////////////////////////////////////
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

                            // ////////////////////////////////////////////////////

                            errPOEntry.SetError(txtShipVia, "");
                            validationcls.HighlightErrControls(lblShipVia, txtShipVia, true);
                            e.Cancel = false;
                            _porder.ShipViaCode = txtShipVia.Text;

                        }
                        // ////////////////////////////////////////////////

                        
                        txtTerms.Text = lstReturn[3];
                        if (!String.IsNullOrEmpty(lstReturn[4]))
                        { txtCurrency.Text = lstReturn[4]; }
                        lblTermsDesc.Text = lstReturn[5];
                        validationcls.HighlightErrControls(lblVendor, txtVendor, true);
                        errPOEntry.SetError(txtVendor, "");
                        e.Cancel = false;
                        _porder.Vendorcode = Int32.Parse(txtVendor.Text);
                    }


                }

                catch (Exception ex)
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
              
           
        #endregion Vendor

        #region Department
        
        private void pctBoxDept_Click(object sender, EventArgs e)
        {
            

            string[] strretvalues;

            errPOEntry.SetError(txtDept, "");

            DataTable dtAuthorized = lookupbo.DepartmentLookup();

            _deptlookup = new Enquiry(dtAuthorized, "DepartmentLookup");

            _deptlookup.ShowGrid();


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


            }



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
                catch (Exception ex)
                {


                    lblDeptDesc.Text = "";
                    errPOEntry.SetError(txtDept, "Enter a valid dept code");
                    validationcls.HighlightErrControls(lblDept, txtVendor, false);
                    e.Cancel = true;


                }
            }
        }
        
        #endregion Department

        #region Currency
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
        private void txtCurrency_TextChanged(object sender, EventArgs e)
        {
          Debug.Print ("Txt Currency : EVENT TextChanged fired");   
        }

        private void txtCurrency_Validating(object sender, CancelEventArgs e)
        {
            Debug.Print ("Txt Currency : EVENT Validating fired");

            if (!_bFormCancelClicked)
            {
                // ////////////////////////////////////////////////////////
                // HK : 19-01-2010 : Prevent any validation from being fired 
                // until all data has been initslised properly
                // /////////////////////////////////////////////////////////
                if (_bDataBindingsInitalised == true)
                {

                    try
                    {
                        // ///////////////////////////////////////////////////////////////
                        // HK : 19-01-2010 : Fix Bug 254, 289
                        // No need to validate if the user is just tabbing out of the field
                        // and has not changed anything.
                        if (_porder.Currencycode == txtCurrency.Text)
                        {
                            Debug.Print("Txt Currenry field. Just tabbing out .....");
                            e.Cancel = false;
                            return;
                        }
                        // ///////////////////////////////////////////////////////////////

                        List<string> lstReturn = new List<string>();

                        // ///////////////////////////////////////////////////////////////////////
                        // lstReturn = validationcls.CurrencyValidate(txtCurrency.Text,_porder.BaseCurrency);
                        // HK : 16-11-2009 : Send the MarketCurrency to CurrencyValidate
                        lstReturn = validationcls.CurrencyValidate(txtCurrency.Text, _porder.MarketCurrency);

                        // ///////////////////////////////////////////////////////////////////////

                        //lstReturn = _porder.ValidateDeptCode(txtDept.Text);

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

                            lblCurrencyDesc.Text = lstReturn[1] + " (" + lstReturn[2] + ")";

                            lblCurrVal1.Text = "(" + _porder.MarketCurrency + ")";
                            lblCurrValue.Text = "(" + _porder.MarketCurrency + ")";

                            errPOEntry.SetError(txtCurrency, "");
                            _porder.Currencycode = txtCurrency.Text;
                            _porder.ExchangeRate = Decimal.Parse(lstReturn[2]);
                            validationcls.HighlightErrControls(lblCurrency, txtCurrency, true);

                            // //////////////////////////////////////////////////////
                            // HK : 16-11-2009 : Capture conversion rate
                            _currencyratepo = Convert.ToDecimal(lstReturn[2]);
                            // //////////////////////////////////////////////////////

                            e.Cancel = false;

                            // HK : 01-12-2009 : Resolve Bug 90
                            ValidateHeaderFields(sender);

                            // ///////////////////////////////////////////////////////////////
                            // HK : 19-01-2010 : Fix Bug 254, 289
                            // 
                            // PO is now modified so enable the "PO Modify" button
                            btnCreatePO.Enabled = true;
                            IsPoModified = true;

                            // HK : 01-01-2010 : Once currency is validated we must recalculate 
                            // the Cost field(s) again with the new currency rate. This needs to 
                            // only if user has entered PO Items and not while he is filling the 
                            // header.
                            if (_porder.lstpoLineItemDetails.Count > 0)
                            {
                                // HK : 19-01-2010 : Do not recalculate as this overwrites any 
                                // manually entered "ConvertedCost" entered by the user.
                                //ReCalculateItemCost();
                            }

                        }

                    }
                    catch (Exception ex)
                    {


                        lblCurrencyDesc.Text = "";
                        validationcls.HighlightErrControls(lblCurrency, txtCurrency, false);
                        errPOEntry.SetError(txtCurrency, "Enter a valid currency code");
                        e.Cancel = true;


                    }

                }
               
            }
              
        }

        #endregion Currency

        #region Terms
        #endregion Terms

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

                // ///////////////////////////////////////////////////////////////////////////
                // HK : Drop Ship (Matrix) PO 
                // ///////////////////////////////////////////////////////////////////////////

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

                // ///////////////////////////////////////////////////////////////////////////
                
            }

             txtNumberofDrops.Text = dtSelectedStores.Rows.Count.ToString(); 


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
                dtSelectedStores.Rows.Add(true, MAGICDCSTORE, "Distribution Centre");
                // _porder.ShipTo = Int16.Parse(cmbShipTo.Text);
                _porder.Purchaseordertype = PurchaseOrder.POType.StandardDCPO;
                btnStores.Enabled = false;
                btnHits.Enabled = true;

                // HK : 16-09-2009
                // Re calculate the PO Summary
                CalculatePOSummary();
                cmbShipTo.Enabled = true;

            }
        }
        
        private void rdBtnDropShipSingle_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnDropShipSingle.Checked)
            {
                _porder.Purchaseordertype = PurchaseOrder.POType.DropShipSingle;
                btnStores.Enabled = true;
                btnHits.Enabled = false;
                dtSelectedStores.Rows.Clear();

                // HK : 16-09-2009Blank out the PO Summary
                DisplayPOSummaryNA();
                cmbShipTo.Enabled = false;


            }
        }

        private void rdBtnDropShipMatrix_CheckedChanged(object sender, EventArgs e)
        {

            // HK : 16-09-2009Blank out the PO Summary
            DisplayPOSummaryNA();
            cmbShipTo.Enabled = false;

            btnStores.Enabled = true;

            btnHits.Enabled = false;

            _porder.Purchaseordertype = PurchaseOrder.POType.DropShipMultiple;

            //MessageBox.Show("This functionality is not available in this version", "SPICE PO MANAGEMENT");
            

        }

        private void cmbShipTo_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        #region Landing
        private void txtLanding_TextChanged(object sender, EventArgs e)
        {
            // HK : 23-11-2009 :
            // Calculate the Landed Cost (field LandedCost) on the grid)


        }

        private void txtLanding_Validating(object sender, CancelEventArgs e)
        {
            decimal dLanding;

            // HK : 01-12-2009 : Dont validate when user is trying to close the form
            if (!_bFormCancelClicked)
            {

                if (txtShipVia.Text == "ROD")
                {

                    if ((Decimal.TryParse(txtLanding.Text, out dLanding)) && (dLanding == 0))
                    {
                        errPOEntry.SetError(txtLanding, "");
                        e.Cancel = false;

                        // HK : BM : 18-11-2009 : Always add 1 to the landing entered by the user
                        _porder.Landing = dLanding + 1;
                        Debug.Print("Landing: " + (_porder.Landing).ToString());


                    }
                    else
                    {
                        txtLanding.Text = "";
                        errPOEntry.SetError(txtLanding, "Value must be greater than zero for ROD");
                        _porder.Landing = 0.00m;
                        e.Cancel = true;

                    }

                }
                else
                {

                    if ((Decimal.TryParse(txtLanding.Text, out dLanding)) && (dLanding > 0))
                    {
                        errPOEntry.SetError(txtLanding, "");
                        e.Cancel = false;

                        // HK : BM : 18-11-2009 : Always add 1 to the landing entered by the user
                        _porder.Landing = dLanding + 1;
                        Debug.Print("Landing: " + (_porder.Landing).ToString());

                        // //////////////////////////////////////////////////////////
                        // HK : 18-01-2010 : Fix Bug 236
                        // //////////////////////////////////////////////////////////
                        validationcls.HighlightErrControls(lblLanding, txtLanding, true);

                    }
                    else
                    {
                        txtLanding.Text = "";

                        // //////////////////////////////////////////////////////////
                        // HK : 18-01-2010 : Fix Bug 236
                        validationcls.HighlightErrControls(lblLanding, txtLanding, false);
                        // //////////////////////////////////////////////////////////

                        errPOEntry.SetError(txtLanding, "Enter a value greater than zero");
                        e.Cancel = true;
                        _porder.Landing = 0.00m;

                    }

                }

                //Calculate PO Summary if not empty
                if (!String.IsNullOrEmpty(txtMarginPercent.Text))
                { CalculatePOSummary(); }

                // HK : 01-12-2009 : Resolve Bug 90
                //ValidateHeaderFields();

            }

        }

        #endregion Landing

        #endregion Shipping
        
        #region ImportSection

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
                   
        #endregion ImportSection

        #region Item DataGrid
        
        private void dtgrdPOLinesView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            /// HK : 26-01-2010 : Fix Bug 360
            if (e.RowIndex == -1)
            {
                return;
            }

            //Display APP or Item Details Window
            if (_polinedetails.Isvalid && _polinedetails.APP1=="N")
            {
                //Item has been retrieved hence now its safe to populate the item details form
                if (polineform == null)
                {
                    // hourglass cursor
                    Cursor.Current = Cursors.WaitCursor;
                    
                    polineform = new POLineForm(_porder, ref _polinedetails, false, e.RowIndex);

                    // ////////////////////////////////////////////////////////////////////
                    // HK : 13-11-2009 : There is no event handler subscribed to quantity 
                    // or cost change in the PO Line form as it is handled by ASH's existing
                    // event handler on the _polinedetails object that is passed by REF
                    // to the constructor of the PO Line form. This handler handles the 
                    // changes to either cost or quantity and displays the changes in this
                    // form's PO Line Item grid and updates the cost and / or quantity
                    // in the _polinedetails object itself

                    // ////////////////////////////////////////////////////////////////////

                    polineform.Show(this);
                    polineform = null;
                }
                                          
            }
            else if (_polinedetails.Isvalid && _polinedetails.APP1 == "Y")
            {
                // hourglass cursor
                Cursor.Current = Cursors.WaitCursor;

                //Extract common params from ROW
                SPICECommon spcommon = new SPICECommon(_porder.DbParamRef, _porder.UserName, _porder.Penvironment, _porder.DefaultMarket);

                //POLineDetailsPack polinedetailspack = new POLineDetailsPack(_polinedetails, _porder,spcommon);
                POLineDetailsPack polinedetailspack = new POLineDetailsPack(_polinedetails, _porder, spcommon, e.RowIndex);

                polinedetailspack.OnAppQuantityChanged += new POLineDetailsPack.AppQuantityChangedEventHandler(polinedetailspack_OnAppQuantityChanged);
                polinedetailspack.ShowDialog(this);
                polinedetailspack = null;

            }

        }

        void polinedetailspack_OnAppQuantityChanged(object sender, POLineDetailsPack.AppDetailsEventArgs e)
        {

            // ///////////////////////////////////////////////////////////////////////////
            // HK : 11-11-2009
            // Reassign the instance valriable of the business objects and others that may have 
            // been modified in the called window
            _porder = e.porder;
            _polinedetails = e.poline;

            // Refresh the value in the corresponding record in the grid
            dtgrdPOLinesView.Rows[e.rowindex].Cells["Quantity"].Value = e.quantity;

            // HK : 15-01-2010 : Fix Bug 233
            //dtgrdPOLinesView.Rows[e.rowindex].Cells["Cost"].Value = e.poline.Cost;
            dtgrdPOLinesView.Rows[e.rowindex].Cells["ConvertedCost"].Value = e.poline.ConvertedCost;

            CalculatePOSummary();

            // Re subscribe the event handler on the PO Line Items business boject
            // for ASH's existing coding and functionaly to function as normal
            _polinedetails.ItemQtyChanged += new POItemDetails.delItemQtyChanged(_polinedetails_ItemQtyChanged);


        }

        private void dtgrdPOLinesView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //Create the empty items object and populate relevant bits as we go along

            Debug.Print("Cell Validating Fired");
            Debug.Print("Cell Name : " + dtgrdPOLinesView.Columns[e.ColumnIndex].Name.ToString ());


            // HK : Prevent Datagrid Validation if the user clicked Cancel button
            if (_bFormCancelClicked)
            {
                Debug.Print("Cell Validating Cancelled as user hit Cancel button");
                return;
            }

            // If the user wants to delete the row then disable any pending row or cell level 
            // validation on the datagrid
            if (_bUserWantsToDeleteLine)
            {
                Debug.Print("Cell Validating Cancelled as user hit Delete Item button");
                return;
            }

            try
            {
                // HK : 1-12-2009 : Work around the default navigation of the datagridview
                if (dtgrdPOLinesView.Rows[e.RowIndex].IsNewRow)
                {
                    Debug.Print("Cell Validating Cancelled as user navigated to a NEW uninitalised row");
                    return;
                }

                if (dtgrdPOLinesView.Columns[e.ColumnIndex].Name.Equals("Class"))
                {
                    // ////////////////////////////////////////////////////////////////////
                    // HK : Fix Bug 162 : Validation and delete annoyances faced by users
                    // ////////////////////////////////////////////////////////////////////
                    // HK : FC : Do not validate class if nothing or spaces entered by user
                    if (String.IsNullOrEmpty(e.FormattedValue.ToString().Trim()))
                    {
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;

                        return;
                    }

                    List<string> retValues = validationcls.ValidateClass(e.FormattedValue.ToString());

                    if (("False" == retValues[0]))
                    {
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "Please enter valid  " + dtgrdPOLinesView.Columns[e.ColumnIndex].Name;
                        e.Cancel = true;
                    }
                    else
                    {
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "";
                        //Class Code and class name
                        _polinedetails.Classname = retValues[1].ToString();
                        _polinedetails.Classcode = Int16.Parse(e.FormattedValue.ToString());
                        e.Cancel = false;

                        // HK : 18-01-2010 : Fix Bug 253
                        // Check to see if the item class in the datagrid row the same as one entered 
                        // by the user.
                        short itemclass = Convert.ToInt16(dtgrdPOLinesView["Class", e.RowIndex].Value);
                        if (itemclass != Int16.Parse(e.FormattedValue.ToString()))
                        {
                            IsItemClassChanged = true;
                        }
                    }

                }

                else if (dtgrdPOLinesView.Columns[e.ColumnIndex].Name.Equals("Vendor"))
                {

                    // ////////////////////////////////////////////////////////////////////
                    // HK : Fix Bug 162 : Validation and delete annoyances faced by users
                    // ////////////////////////////////////////////////////////////////////
                    // HK : FC : Do not validate class if nothing or spaces entered by user
                    if (String.IsNullOrEmpty(e.FormattedValue.ToString().Trim()))
                    {
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;

                        return;
                    }

                    // /////////////////////////////////////////////////////////

                    List<string> retValues = validationcls.ValidateVendor(e.FormattedValue.ToString(), true);

                    if (("False" == retValues[0]))
                    {
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "Please enter valid " + dtgrdPOLinesView.Columns[e.ColumnIndex].Name;
                        e.Cancel = true;
                    }
                    else
                    {
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "";
                        //Pack values to the poline class
                        _polinedetails.Vendorcode = Int32.Parse(e.FormattedValue.ToString());
                        _polinedetails.Vendordesc = retValues[1].ToString();
                        e.Cancel = false;

                        // HK : 18-01-2010 : Fix Bug 253
                        // Check to see if the item class in the datagrid row the same as one entered 
                        // by the user.
                        int vendor = Convert.ToInt16(dtgrdPOLinesView["Vendor", e.RowIndex].Value);
                        if (vendor != Int32.Parse(e.FormattedValue.ToString()))
                        {
                            IsItemVendorChanged = true;
                        }
                    }

                }

                else if (dtgrdPOLinesView.Columns[e.ColumnIndex].Name.Equals("Style"))
                {

                    // ////////////////////////////////////////////////////////////////////
                    // HK : Fix Bug 162 : Validation and delete annoyances faced by users
                    // ////////////////////////////////////////////////////////////////////
                    // HK : FC : Do not validate class if nothing or spaces entered by user
                    if (String.IsNullOrEmpty(e.FormattedValue.ToString().Trim()))
                    {
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;

                        return;
                    }

                    List<string> retValues = validationcls.ValidateStyle(e.FormattedValue.ToString());

                    if (("False" == retValues[0]))
                    {
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "Please enter valid " + dtgrdPOLinesView.Columns[e.ColumnIndex].Name;
                        e.Cancel = true;
                    }
                    else
                    {
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "";
                        //Style code
                        _polinedetails.Stylecode = Int16.Parse(e.FormattedValue.ToString());
                        e.Cancel = false;

                        // HK : 18-01-2010 : Fix Bug 253
                        // Check to see if the item class in the datagrid row the same as one entered 
                        // by the user.
                        short style = Convert.ToInt16(dtgrdPOLinesView["Style", e.RowIndex].Value);
                        if (style != Int16.Parse(e.FormattedValue.ToString()))
                        {
                            IsItemStyleChanged = true;
                        }
                    }


                }
                else if (dtgrdPOLinesView.Columns[e.ColumnIndex].Name.Equals("Color"))
                {

                    // ////////////////////////////////////////////////////////////////////
                    // HK : Fix Bug 162 : Validation and delete annoyances faced by users
                    // ////////////////////////////////////////////////////////////////////
                    // HK : FC : Do not validate class if nothing or spaces entered by user
                    if (String.IsNullOrEmpty(e.FormattedValue.ToString().Trim()))
                    {
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;

                        return;
                    }

                    List<string> retValues = validationcls.ValidateColour(e.FormattedValue.ToString());

                    if (("False" == retValues[0]))
                    {
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "Please enter valid " + dtgrdPOLinesView.Columns[e.ColumnIndex].Name;
                        e.Cancel = true;
                    }
                    else
                    {
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "";
                        _polinedetails.Colorcode = Int16.Parse(e.FormattedValue.ToString());
                        _polinedetails.Colordesc = retValues[1];
                        e.Cancel = false;

                        // HK : 18-01-2010 : Fix Bug 253
                        // Check to see if the item class in the datagrid row the same as one entered 
                        // by the user.
                        short color = Convert.ToInt16(dtgrdPOLinesView["Color", e.RowIndex].Value);
                        if (color != Int16.Parse(e.FormattedValue.ToString()))
                        {
                            IsItemColorChanged = true;
                        }
                    }

                }

                else if (dtgrdPOLinesView.Columns[e.ColumnIndex].Name.Equals("Size"))
                {

                    short iItem;
                    int iVendor;
                    short iStyle;
                    short iColour;
                    int iitemindex;

                    // Try and convert Class, Vendor, Style, Color, Size
                    Int16.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[e.RowIndex].Cells["Class"].Value), out iItem);
                    Int32.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[e.RowIndex].Cells["Vendor"].Value), out iVendor);
                    Int16.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[e.RowIndex].Cells["Style"].Value), out iStyle);
                    Int16.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[e.RowIndex].Cells["Color"].Value), out iColour);
                    iitemindex = _polinedetails.Itemindex;

                    // Check to see if this item is a duplicate
                    _bDuplicateItem = CheckForDuplicateLine(iitemindex, iItem, iVendor, iStyle, iColour, e.FormattedValue.ToString());

                    // ////////////////////////////////////////////////////////////////////
                    // HK : Fix Bug 162 : Validation and delete annoyances faced by users
                    // ////////////////////////////////////////////////////////////////////
                    // HK : FC : Do not validate class if nothing or spaces entered by user
                    if (String.IsNullOrEmpty(e.FormattedValue.ToString().Trim()))
                    {
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;

                        return;
                    }

                    List<string> retValues = validationcls.ValidateSize(e.FormattedValue.ToString());

                    if (("False" == retValues[0]))
                    {
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "Please enter valid " + dtgrdPOLinesView.Columns[e.ColumnIndex].Name;
                        e.Cancel = true;
                    }
                    else
                    {
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "";

                        _polinedetails.Itemsize = Int16.Parse(e.FormattedValue.ToString());
                        _polinedetails.Sizename = retValues[1];

                        // HK : 18-01-2010 : Fix Bug 253
                        // Check to see if the item class in the datagrid row the same as one entered 
                        // by the user.
                        short size = Convert.ToInt16(dtgrdPOLinesView["Size", e.RowIndex].Value);
                        if (size != Int16.Parse(e.FormattedValue.ToString()))
                        {
                            IsItemSizeChanged = true;
                        }

                        // ////////////////////////////////////////////////////////////////
                        // HK : 18-01-2010 : Fix Bug 253

                        if (!IsOkToDoItemLookup(iitemindex, iItem, iVendor, iStyle, iColour, iStyle))
                        {
                            dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "";
                            e.Cancel = false;

                            return;
                        }

                        // ////////////////////////////////////////////////////////////////

                        if (_polinedetails.ItemLookup(_porder.DbParamRef, _porder.UserName, _porder.Penvironment, _porder.DefaultMarket))
                        {

                            dtgrdPOLinesView.Rows[e.RowIndex].Cells["Description"].Value = _polinedetails.Itemlongdescription;
                            dtgrdPOLinesView.Rows[e.RowIndex].Cells["Retail"].Value = _polinedetails.Retailprice.ToString();
                            // HK : 30-11-2009 : "Cost" column holds the simple vendor cost as retrieved from the database
                            dtgrdPOLinesView.Rows[e.RowIndex].Cells["Cost"].Value = _polinedetails.Cost.ToString();
                            dtgrdPOLinesView.Rows[e.RowIndex].Cells["Character"].Value = _polinedetails.Characterdesc;
                            dtgrdPOLinesView.Rows[e.RowIndex].Cells["Season"].Value = _polinedetails.SeasonDesc;
                            dtgrdPOLinesView.Rows[e.RowIndex].Cells["CasePackType"].Value = _polinedetails.Packdescription;
                            dtgrdPOLinesView.Rows[e.RowIndex].Cells["TicketType"].Value = _polinedetails.Tickettype;
                            //This will determine if the qty and cost can be changed 
                            dtgrdPOLinesView.Rows[e.RowIndex].Cells["Pack"].Value = _polinedetails.APP1;

                            // ////////////////////////////////////////////////////////////////////
                            // HK : 28-11-2009 : Increment Total Number of Packs
                            // ////////////////////////////////////////////////////////////////////
                            if (_polinedetails.APP1 == "Y")
                            {
                                _porder.NumofPOPacks += 1;
                            }

                            // ///////////////////////////////////////////////////////////////////////
                            // HK : 16-11-2009 : Display Converted cost and hide actual cost from database
                            // 
                            // ///////////////////////////////////////////////////////////////////////

                            // HK :CJ : 23-11-2009
                            // Apply Landing Factor
                            if (_porder.Landing == 0)
                            {
                                _porder.Landing = 1;
                            }

                            // HK : 14-01-2010 : Fix Bug 233
                            _polinedetails.LandedCost = Decimal.Round((_polinedetails.Cost * _currencyratemarket) * _porder.Landing, 2);
                            // Display Landed Cost in datagrid
                            dtgrdPOLinesView.Rows[e.RowIndex].Cells["LandedCost"].Value = Decimal.Round((_polinedetails.Cost * _currencyratemarket) * _porder.Landing, 2);

                            // Cannot divide by zero
                            if (_currencyratepo != 0)
                            {
                                // HK : 14-01-2010 : Fix Bug 233
                                _polinedetails.ConvertedCost = Decimal.Round((_polinedetails.Cost * _currencyratemarket) / _currencyratepo, 2);

                                dtgrdPOLinesView.Rows[e.RowIndex].Cells["ConvertedCost"].Value = Decimal.Round((_polinedetails.Cost * _currencyratemarket) / _currencyratepo, 2);

                            }
                            else
                            {
                                Debug.Print("Add Item : While calculating converted cost, it was found that the PO Currency Rate is 0");
                            }

                            if (_polinedetails.APP1 == "Y")
                            {
                                // HK : 07-01-2010 : Fix Bug 144
                                //Make the UnitCost Readonly
                                //dtgrdPOLinesView.Rows[e.RowIndex].Cells["Cost"].ReadOnly = true;
                                dtgrdPOLinesView.Rows[e.RowIndex].Cells["ConvertedCost"].ReadOnly = true;

                            }
                            else
                            {
                                //Not an APP
                                //Enable cost
                                //dtgrdPOLinesView.Rows[e.RowIndex].Cells["Cost"].ReadOnly = false;
                                dtgrdPOLinesView.Rows[e.RowIndex].Cells["ConvertedCost"].ReadOnly = false;
                            }

                            // ///////////////////////////////////////////////////////////////////////

                            _polinedetails.Isvalid = true;
                            e.Cancel = false;

                            // HK : 18-01-2010 : Reset the Lookup flags
                            IsItemClassChanged = false;
                            IsItemVendorChanged = false;
                            IsItemStyleChanged = false;
                            IsItemColorChanged = false;
                            IsItemSizeChanged = false;

                            // HK : 22-01-2010 : Fix Bug 312 :
                            btnCreatePO.Enabled = true;

                            // ////////////////////////////////////////////////////////////////////////
                            // HK : 05-10-2009 : Stub to default Retail and Cost values
                            // In the intial days the item BO's and DA's were not bringing the "Cost" and 
                            // "Retail" price. So FC asked my to hardcode them

                            //dtgrdPOLinesView.Rows[e.RowIndex].Cells["Cost"].Value = 10.ToString();
                            //dtgrdPOLinesView.Rows[e.RowIndex].Cells["Retail"].Value = 50.ToString();

                            // ////////////////////////////////////////////////////////////////////////


                        }
                        else
                        {
                            dtgrdPOLinesView.Rows[e.RowIndex].Cells["Description"].Value = _polinedetails.Itemlongdescription;
                            DataGridClearItemnotfound(e.RowIndex);

                            _polinedetails.Isvalid = false;
                            // e.Cancel =true ; 

                        }

                    }

                }

                // HK : 14-01-2010 : Fix Bug 233:
                // HK : Cost can be changed. However the cost displayed in the grid is the 
                // ConvertedCost and not the Cost got from the database.
                else if (dtgrdPOLinesView.Columns[e.ColumnIndex].Name.Equals("ConvertedCost"))
                {
                    decimal _cost = 0m;
                    decimal _costalreadyincell = Convert.ToDecimal (dtgrdPOLinesView["ConvertedCost", e.RowIndex].Value);
                    if (!String.IsNullOrEmpty(e.FormattedValue.ToString()) && Decimal.TryParse(e.FormattedValue.ToString(), out _cost))
                    {
                        // HK : 20-01-2010 : Fix Bug 254, 289
                        // If user is just tabbing through, do nothing
                        if (_cost == _costalreadyincell)
                        {

                            Debug.Print("Cost field. Just tabbing out .....");
                            dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "";
                            e.Cancel = false;

                            return;
                        }

                        _polinedetails.ConvertedCost = Decimal.Parse(e.FormattedValue.ToString());
                        CalculatePOSummary();
                        
                        btnCreatePO.Enabled = true;
                        IsPoModified = true;
                    }
                    else
                    {
                        dtgrdPOLinesView["ConvertedCost", e.RowIndex].Value = 0;
                    }

                }

                else if (dtgrdPOLinesView.Columns[e.ColumnIndex].Name.Equals("Quantity"))
                {
                    int itemqtyinput;

                    // ////////////////////////////////////////////////////////////////////
                    // HK : FC : Do not validate quantity if 0 or null is entered by user
                    if (String.IsNullOrEmpty(e.FormattedValue.ToString()))
                    {
                        // HK : 03-12-2009 : Not exactly right but we cannot
                        // set an intger to null.
                        _polinedetails.Itemquantity = 0;
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;

                        // HK : 25-01-2010 : Fix Bug 350
                        CalculatePOSummary();

                        // HK : 14-01-2010 : Fix Bug 255
                        // HK : 25-01-2010 : Fix Bug 252
                        UpdatePoLinesPacks();

                        return;
                    }

                    // HK : FC : 13-01-2010 : To Do To Do 
                    // FC has asked if Quntity should not be allowed on a Po Line Itme that as
                    // has an Invalid Item
                    // No need to validate quantity if this Po Line Item is InValid
                    if (_polinedetails.Isvalid == false)
                    {
                        return;
                    }

                    // ////////////////////////////////////////////////////////////////
                    Int32.TryParse(e.FormattedValue.ToString(), out itemqtyinput);
                    if (itemqtyinput == 0)
                    {
                        _polinedetails.Itemquantity = 0;
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;

                        // HK : 25-01-2010 : Fix Bug 350
                        CalculatePOSummary();

                        // HK : 14-01-2010 : Fix Bug 255
                        // HK : 25-01-2010 : Fix Bug 252
                        UpdatePoLinesPacks();

                        // HK : 22-01-2010 : Fix Bug 312 :
                        btnCreatePO.Enabled = true;

                        return;

                    }

                    // HK : 20-01-2010 : Fix Bug 254, 289
                    // If user is just tabbing through, do nothing
                    itemqtyinput = Convert.ToInt32(dtgrdPOLinesView["Quantity", e.RowIndex].Value);
                    if (itemqtyinput == Int32.Parse(e.FormattedValue.ToString()))
                    {
                        Debug.Print("Quantity field. Just tabbing out .....");
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;

                        return;
                    }
                    
                    // ////////////////////////////////////////////////////////////////////
                    //Calculate the Totals in terms of total cost etc
                    if (ValidateQuantity(e.FormattedValue.ToString(), _polinedetails.Casepackqty, e.RowIndex))
                    {
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "";

                        // ///////////////////////////////////////////////////////////////////
                        // HK : 11-11-2009 : If no rounding was done then continue validating 
                        // as usual
                        if (_itemquantityrounded == 0)
                        {
                            _polinedetails.Itemquantity = int.Parse(e.FormattedValue.ToString());

                            // /////////////////////////////////////////////////////////////////
                            // HH: 05-11-2009 : Display the suggested quantity in the datagrid
                            // /////////////////////////////////////////////////////////////////

                            dtgrdPOLinesView.Rows[e.RowIndex].Cells["Quantity"].Value = _polinedetails.Itemquantity;

                            // /////////////////////////////////////////////////////////////////
                        }

                        // If rounding was done then capture the value so that the "CellValidated" EVENT 
                        // can display the rounded value
                        if (_itemquantityrounded > 0)
                        {
                            _polinedetails.Itemquantity = _itemquantityrounded;

                        }

                        CalculatePOSummary();

                        // HK : 14-01-2010 : Fix Bug 255
                        UpdatePoLinesPacks();

                        // HK : 22-01-2010 : Fix Bug 312 :
                        btnCreatePO.Enabled = true;

                        e.Cancel = false;

                    }
                    else
                    {
                        dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "Please enter valid " + dtgrdPOLinesView.Columns[e.ColumnIndex].Name;
                        e.Cancel = true;

                    }

                }

                else if (dtgrdPOLinesView.Columns[e.ColumnIndex].Name.Equals("Cost"))
                {
                    decimal _cost;
                    if (!String.IsNullOrEmpty(e.FormattedValue.ToString()) || Decimal.TryParse(e.FormattedValue.ToString(), out _cost))
                    {

                        _polinedetails.Cost = Decimal.Parse(e.FormattedValue.ToString());
                        CalculatePOSummary();
                    }
                    else
                    {
                        dtgrdPOLinesView["Cost", e.RowIndex].Value = 0;
                    }

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, ex.Source);

            }

        }

        void _polinedetails_ItemQtyChanged(int qty, decimal cost, int RowIndex)
        {

            // /////////////////////////////////////////////////////////////////////
            // HK : 10-11-2009 : Called from PO Line form and ItemQuantityForm. 
            // In the ItemQuantityForm when the user rounded up or rounded down the 
            // quantity, it has to be displayed in the grid on the correct row.
            // s
            // In the PO Line form, any changes to quantiry and  / or cost price
            // must also be displayed on the correct row in the grid

            // ////////////////////////////////////////////////////////////////

            dtgrdPOLinesView["Quantity", RowIndex].Value = qty;

            // HK : 15-01-2010 : Fix Bug 233 : Converted Cost can be changed by user
            //dtgrdPOLinesView["Cost",RowIndex].Value = cost;

            // ///////////////////////////////
            // HK : 15-11-2009 : Fix Bug 135
            // When the Cost changes in the PoLineForm window it mut reflect the 
            // changes to ConvertedCost and LandedCost

            // Column Name                  Label           Visible     Expression
            // =====================================================================================
            // ConvertedCost                Cost            True        Decimal.Round((_polinedetails.Cost * _currencyratemarket) / _currencyratepo, 2);
            // Cost                         Uplift Cot      False       Value retrieved from database by ItemLookup. Also cased simple vendor cost
            // LandedCost                   Landed Cost     False       Decimal.Round((_polinedetails.Cost * _currencyratemarket) * _porder.Landing, 2);

            // HK : 15-01-2010 : Fix Bug 233 : No need to change the landed cost
            dtgrdPOLinesView.Rows[RowIndex].Cells["LandedCost"].Value = Decimal.Round((_polinedetails.Cost * _currencyratemarket) * _porder.Landing, 2);

            // Cannot divide by zero
            if (_currencyratepo != 0)
            {
                // HK : 15-01-2010 : Fix Bug 233 : Converted Cost can be changed by user
                //dtgrdPOLinesView.Rows[RowIndex].Cells["ConvertedCost"].Value = Decimal.Round((_polinedetails.Cost * _currencyratemarket) / _currencyratepo, 2);
                dtgrdPOLinesView.Rows[RowIndex].Cells["ConvertedCost"].Value = cost;
            }
            else
            {
                Debug.Print("Add Item : While calculating converted cost, it was found that the PO Currency Rate is 0");
            }

            // //////////////////////////////
            CalculatePOSummary();

            //throw new Exception("The method or operation is not implemented.");
        }

        void DataGridClearItemnotfound(int rowindex)
        {

            dtgrdPOLinesView.Rows[rowindex].Cells["Retail"].Value = String.Empty;
            dtgrdPOLinesView.Rows[rowindex].Cells["Cost"].Value = String.Empty;
            dtgrdPOLinesView.Rows[rowindex].Cells["Character"].Value = String.Empty;
            dtgrdPOLinesView.Rows[rowindex].Cells["Season"].Value = String.Empty;
            dtgrdPOLinesView.Rows[rowindex].Cells["CasePackType"].Value = String.Empty;
            dtgrdPOLinesView.Rows[rowindex].Cells["TicketType"].Value = String.Empty;
            dtgrdPOLinesView.Rows[rowindex].Cells["Pack"].Value = String.Empty;
            dtgrdPOLinesView.Rows[rowindex].Cells["Quantity"].Value = String.Empty;

            // HK : 18-01-2010 : 
            dtgrdPOLinesView.Rows[rowindex].Cells["ConvertedCost"].Value = String.Empty;
            dtgrdPOLinesView.Rows[rowindex].Cells["LandedCost"].Value = String.Empty;

            _polinedetails.Itemlongdescription = String.Empty;
            _polinedetails.Retailprice = 0.00m;
            _polinedetails.Cost = 0.00m;
            _polinedetails.Characterdesc = String.Empty;
            _polinedetails.SeasonDesc = String.Empty;
            _polinedetails.Packdescription = String.Empty;
            _polinedetails.Tickettype = String.Empty;
            _polinedetails.APP1 = String.Empty;
            _polinedetails.Itemquantity = 0;

            // HK : 18-01-2010 : 
            _polinedetails.ConvertedCost = 0m;
            _polinedetails.LandedCost = 0m;

        }
        
        private void dtgrdPOLinesView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = String.Empty;
                              
        }

        private void dtgrdPOLinesView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
        
            Debug.Print("Rows Added Fired");
            Debug.Print("Row Index : " + e.RowIndex.ToString ());

            // //////////////////////////////////////////////////////////////////////////
            // HHK : 10-11-2009 : Dont know the purpose of below code i.e why increment 
            // _porder.NumofPOLines by 1 if rowcount in grid != 1. Since toal number of lines
            // on a PO Header is always reported 1, I have decided to comment the below 'if' 
            // code block
            // //////////////////////////////////////////////////////////////////////////
            
            if (e.RowCount != 1)
            { 
                _porder.NumofPOLines += 1;
            }
            
        }

        private void dtgrdPOLinesView_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {

            // ///////////////////////////////////////////////////////////////////
            // HK : 19-11-2009 : Bug 64
            // When user clicks buttton 'Create PO', the 'cell validating' and 
            // 'row validating' is triggered for the new row in the datagrid/
            // Solution is not to validate a row in the grid that has no 
            // valid entry in object porder.lstpoLineItemDetails

            // ///////////////////////////////////////////////////////////////////
            Debug.Print("Rows Validating Fired");
            Debug.Print("Row Index : " + e.RowIndex.ToString());

            // HK : Prevent Datagrid Validation if the user clicked Cancel button
            if (_bFormCancelClicked)
            {
                return;
            }

            // //////////////////////////////////////////////////////////////////////////////
            // HK : 22-01-2010 : Fix Bug 313 : Prevent Row level validation if user is deleting the row.
            // Dont bother with validation if user selects to delete a row.
            if (_bUserWantsToDeleteLine)
            {
                return;
            }
            // //////////////////////////////////////////////////////////////////////////////

            if (e.RowIndex < dtgrdPOLinesView.RowCount - 1)
            {

                //Probably a good place to pack and finalize PO Summary Collection
                if ((!_polinedetails.Isvalid) && IsOrderValid())
                {
                    dtgrdPOLinesView.Rows[e.RowIndex].ErrorText = "Please enter valid item";
                    e.Cancel = true;

                }
                else
                {
                    errPOEntry.SetError(dtgrdPOLinesView, "");
                    e.Cancel = false;

                    // Once all validation has been done on the row then reassign the Po Line details
                    // object to the collections to get all the lates updated and validated values
                    // HHK : 09-11-2009

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

        private void dtgrdPOLinesView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
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

        private void dtgrdPOLinesView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            //User enters the class cell hence a good place to create POLine BO

            short nextsequencenumber;

            Debug.Print("UserAddedRow fired");
            Debug.Print("Row Index : " + e.Row.Index.ToString());

            if (!e.Row.IsNewRow)
            {
                //Add to the dataset
            }
           
           // IF the row is NEW
           if (e.Row.IsNewRow)
            {
                //Add to the dataset

                // //////////////////////////////////////////////////////////////////////////////
                // HHK : 09-11-2009
                // When new row is added by user then Un-Subscribe the event handler on the cuatom 
                // class object which we are forcing out of scope
                // /////////////////////////////////////////////////////////////////////////////

                if (_polinedetails != null)
                {
                    _polinedetails.ItemQtyChanged -= new POItemDetails.delItemQtyChanged(_polinedetails_ItemQtyChanged);
                }

                // HHK : 09-11-2009 : Now null the _polinedetails and new it once again 
                // for subsequent use
                _polinedetails = null;

                _polinedetails = new POItemDetails(e.Row.Index);

                 // HK : CJ : 10-12-2009 : Property ItemIndex on class  POItemDetails should be 
                 // the same as the datagrid row index for that item
                 nextsequencenumber = GetNextSequenceNumber();
                 _polinedetails.Sequence = nextsequencenumber;

                _polinedetails.ItemQtyChanged += new POItemDetails.delItemQtyChanged(_polinedetails_ItemQtyChanged);

                // /////////////////////////////////////////////////////////////////////////
                // HHK : 09-11-2009 : Add the new PO Line Item object instance to the collection
                //
                // //////////////////////////////////////////////////////////////////////////

                if (_porder.lstpoLineItemDetails.IndexOf(_polinedetails) == -1)
                {
                    _porder.lstpoLineItemDetails.Insert( (e.Row.Index - 1), _polinedetails);
                }

                // /////////////////////////////////////////////////////////////////////////
                // HHK : 09-11-2009 : Increment the PO line count 
                //
                // //////////////////////////////////////////////////////////////////////////
                _porder.NumofPOLines += 1;

                Debug.Print("Event (UserAddedRow): Number of PO Lines:" + _porder.NumofPOLines.ToString());

                // ///////////////////////////////////////////////////////////////////////

            }
            
        }

        private void dtgrdPOLinesView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            //Validate all reqd fields notnull/empty
            Debug.Print("Row Enter fired");
            Debug.Print("Row Index:        " + e.RowIndex.ToString() + "======" + "Row Count: " + dtgrdPOLinesView.Rows.Count.ToString());
            Debug.Print("Collection Count: " + _porder.lstpoLineItemDetails.Count.ToString());

            // HK : Prevent Datagrid Validation if the user clicked Cancel button
            if (_bFormCancelClicked)
            {
                return;
            }

            // HK : 26-01-2010 : Since sorting is on when user clicks the column header,
            // the e.RowIndex will not match the exact item in the collection after 
            // the sort has occured.

            // /////////////////////////////////////////////////////////////////////////
            // HHK : 09-11-2009 : When user navigates to existing row we must re-assign  
            // to _polinedetails the value held in the collection (if one exists).
            // Since this is the very first event fired (agmongst row events), the collection 
            // may not have been initalised for the 1st row.

            //if (_porder.lstpoLineItemDetails.Count > 0  && _porder.lstpoLineItemDetails[e.RowIndex] != null)
            if (_porder.lstpoLineItemDetails.Count > 0 &&  e.RowIndex < _porder.lstpoLineItemDetails.Count)
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
                _polinedetails.ItemQtyChanged += new POItemDetails.delItemQtyChanged(_polinedetails_ItemQtyChanged);

                DisplayLineItemDetails(_polinedetails);
                
            }

            // //////////////////////////////////////////////////////////////////////////

        }

        private void dtgrdPOLinesView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {

            // HK : Prevent Datagrid Validation if the user clicked Cancel button
            if (_bFormCancelClicked == false)
            {

            if (!IsOrderValid())
            {
                errPOEntry.SetError(btnHelp, "Please enter valid order data");
                dtgrdPOLinesView.ReadOnly = true;


            }
            else
            {
                dtgrdPOLinesView.ReadOnly = false;
                errPOEntry.SetError(btnHelp, "");
            }

            }

        }

        #endregion Item DataGrid

        private void POEntryForm1_Load(object sender, EventArgs e)
        {
            // HK : 03-10-2009
            // Stub to fill in required default values
            //txtDept.Text = "11";

        }

        #region ActionButtons
         
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // ////////////////////////////////////////////////////////////////
            // HK : 16-11-2009
            // Prevent control validation when user clicks the Cancel button
            // ////////////////////////////////////////////////////////////////
            _bFormCancelClicked = true;

            DialogResult dlgResult = MessageBox.Show("Are you sure you want to Cancel this PO Modification?", "SPICE PO Modification", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (dlgResult == DialogResult.Yes)
            {
                this.Close();
            }
            else
            {
                _bFormCancelClicked = false;
            }
        }

        private void btnHits_Click(object sender, EventArgs e)
        {
            POHitsForm pohitsform = new POHitsForm(_porder, _pohitscollection);

            // Subscribe to event handlers
            pohitsform.OnOkButtonClicked += new POHitsForm.OkButtonClickedEventHandler(pohitsform_OnOkButtonClicked);
            pohitsform.OnCancelButtonClicked += new POHitsForm.CancelButtonClickedEventHandler(pohitsform_OnCancelButtonClicked);

            //this.Hide();
            //pohitsform.Show();
            pohitsform.ShowDialog ();


        }

        void pohitsform_OnCancelButtonClicked(object sender, POHitsForm.PoHitsEventArgs e)
        {
            _pohitscollection = e.poHitsCollection;
            //throw new Exception("The method or operation is not implemented.");
        }

        void pohitsform_OnOkButtonClicked(object sender, POHitsForm.PoHitsEventArgs e)
        {
            _pohitscollection = e.poHitsCollection;
            //throw new Exception("The method or operation is not implemented.");
        }

        // ////////////////////////////////////////////////////////////////
        // HK : FC : Cannot create PO if not enough info is available.
        // Validate Required fields
        // ////////////////////////////////////////////////////////////////
        
        private Boolean CheckRequiredFields()
        {
            // Check to see if the PO Line Items are valid
            Boolean bSuccess = true;

            // ///////////////////////////////////////////////////////////////
            // Check the Ship via field
            // ///////////////////////////////////////////////////////////////
            if (String.IsNullOrEmpty(txtShipVia.Text))
            {
                errPOEntry.SetError(txtShipVia, "Enter a valid ship via code");
                validationcls.HighlightErrControls(lblShipVia, txtShipVia, false);
                bSuccess = false;
            }

            // ///////////////////////////////////////////////////////////////
            // Check the Landing field
            // ///////////////////////////////////////////////////////////////
            if (String.IsNullOrEmpty(txtLanding.Text))
            {
                errPOEntry.SetError(txtLanding, "Enter a valid landing code");
                validationcls.HighlightErrControls(lblLanding, txtLanding, false);
                bSuccess = false;
            }

            // ///////////////////////////////////////////////////////////////
            // Check the Port of Departure field
            // ///////////////////////////////////////////////////////////////
            if (String.IsNullOrEmpty(txtPortofDeparture.Text) && (txtShipVia.Text == "OCN"))
            {
                errPOEntry.SetError(txtPortofDeparture, "Enter a port of departure code");
                validationcls.HighlightErrControls(lblPortofDeparture, txtPortofDeparture, false);
                bSuccess = false;
            }

            // ///////////////////////////////////////////////////////////////
            // Check the Port of Port of Entry field
            // ///////////////////////////////////////////////////////////////
            if (String.IsNullOrEmpty(txtPortofEntry.Text) && (txtShipVia.Text == "OCN"))
            {
                errPOEntry.SetError(txtPortofEntry, "Enter a port of entry code");
                validationcls.HighlightErrControls(lblPortofEntry, txtPortofEntry, false);
                bSuccess = false;
            }


            // ///////////////////////////////////////////////////////////////
            // Check the Delivery Terms field
            // ///////////////////////////////////////////////////////////////
            if (String.IsNullOrEmpty(txtDelTerms.Text) && (txtShipVia.Text == "OCN"))
            {
                errPOEntry.SetError(txtDelTerms, "Enter a valid delivery terms code");
                validationcls.HighlightErrControls(lblDelTerms, txtDelTerms, false);
                bSuccess = false;
            }

            // HK : 13-01-2010 : Fix Bug 251
            // ///////////////////////////////////////////////////////////////
            // Check that the Ship Date is less than Anticipate date
            // ///////////////////////////////////////////////////////////////
            if ((dtpkrAnticipateDate.Value.Date < dtpkrShipDate.Value.Date))
            {
                errPOEntry.SetError(dtpkrShipDate, "Shipping date cannot be after the Anticipate date");
                bSuccess = false;
            }
            
            // ///////////////////////////////////////////////////////////////
            // Check whether PO Items are entered
            // ///////////////////////////////////////////////////////////////
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
                if (item.Isvalid == false)
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

        private void btnCreatePO_Click(object sender, EventArgs e)
        {
            try
            {
                //this.ValidateChildren();

                // ////////////////////////////////////////////////////////////////
                // HK : FC : Cannot create PO if not enough info is available.
                if (CheckRequiredFields() == false)
                {
                    MessageBox.Show("Not enough information is available to modify the PO", "SPICE PO MANAGEMENT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (CheckItemQuantity() == false)
                {
                    MessageBox.Show("Item quantity cannot be 0", "SPICE PO MANAGEMENT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ////////////////////////////////////////////////////////////////

                int _numberofPurchaseOrders = 0;

                switch (_porder.Purchaseordertype)
                {

                    case PurchaseOrder.POType.StandardDCPO:
                        _numberofPurchaseOrders = 1;
                        break;
                    case PurchaseOrder.POType.DropShipSingle:
                        _numberofPurchaseOrders = dtSelectedStores.Rows.Count;
                        break;
                    case PurchaseOrder.POType.DropShipMultiple:
                        //Not implemented
                        break;

                }

                // HK : 05-11-2009 : Add CR and LF in the MessageBox
                if (MessageBox.Show("You are about to modify PO No. " + _spiceponumber
                                + "\r\n"
                                + "\r\n"
                                + "Do you want to continue? ", "SPICE - PO Modification - Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {

                    pwindow = new ProgressWindow(_numberofPurchaseOrders);
                    pwindow.Show();
                    bkgrndWorker.RunWorkerAsync();

                }

            }

            catch (Exception ex)
            {
                MessageBox.Show("An Error has occurred with the Purchase Order Please contact Support", "SPICE PO MANAGEMENT");
            }

            }

        private bool CreatePurchaseOrder(int iHitNumber)
        {

            if (_porder.CreatePoHeader(iHitNumber) && _porder.CreatePOComments(iHitNumber))
            {
                DataTable dtLines = PopulatePOLines(iHitNumber);

                if (_polinedetails.CreateOrderLines(_porder, dtLines) == true)
                {
                    //MessageBox.Show("Purchase Order Number" + _porder.Spiceponumber + "  created ");
                    Debug.Print(_porder.Spiceponumber + DateTime.Now.ToString());
                    return true;
                }

                return false;
            }
            else
            {
                return false;
            }


        }

        private bool ModifyPurchaseOrder(string spiceponumber)
        {
            short _spicepoversionprevious;

            // HK : CJ : 26-01-2010 : See if the lines exist before writing the PO
            /*
            DataTable dtLines = PopulatePOLines();
            if (dtLines.Rows.Count == 0)
            {
                MessageBox.Show("This PO cannot be modified as there are no lines on it.", "SPICE PO Modification");
                return false;
            }
            */
            if (_porder.ModifyPOHeader(spiceponumber, out _spicepoversionprevious) && _porder.ModifyPOComments(_spicepoversionprevious))
            {
                DataTable dtLines = PopulatePOLines();

                if (_polinedetails.ModifyOrderLines(_porder, dtLines, _spicepoversionprevious) == true)
                {
                    
                    Debug.Print(_porder.Spiceponumber + DateTime.Now.ToString()) ;
                    
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
                    
                    Debug.Print(_porder.Spiceponumber + DateTime.Now.ToString()) ;
                    
                    return true;
                }
                
                return false;
            }
            else
            {
                return false;
            }
        
        }
        

        #endregion ActionButtons

        private void SetupInitialValues()
        {
            //Depending on the default market enable/disable controls

            lblSSD.Visible = false;
            cmbSSD.Visible = false;

            cmbShipTo.Items.Add(MAGICDCSTORE);
            cmbShipTo.SelectedItem = cmbShipTo.Items[0];
            dtpkrShipDate.Value = DateTime.Now;
            dtpkrAnticipateDate.Value = DateTime.Now;

            lookupbo = new LookupBO(_porder.DbParamRef, _porder.UserName,_porder.Penvironment);

            validationcls = new Validation(_porder.DbParamRef,_porder.UserName,_porder.Penvironment);

        //  string dateformat = _porder.Penvironment.DateFormat;

            txtOrderDate.Text = DateTime.Now.ToString("D");

            lblMarketValue.Text = _porder.DefaultMarket + "-" + _porder.MarketDescription;

            //Disabling of buttons
            btnStores.Enabled = false;
            btnStores.Visible = false;

            rdBtnDropShipSingle.Visible = false;
            rdBtnDropShipMatrix.Visible = false;

            lblNumberofDrops.Visible = false;
            txtNumberofDrops.Visible = false;

            label30.Visible = false;
            txtNumberofHits.Visible = false;

            //Domain specific stuff confirm the prop with CJ

            //if (_porder.Penvironment.Name == "TDSE")
            // HK : JU : 18-12-2009 : Use the correct property (DomainName) to 
            // get the environment. Property Name is not the correct property
            if (_porder.Penvironment.Domain == "SWNA")
            {
                lblSSD.Visible = true;
                cmbSSD.Visible = true;
                cmbSSD.Enabled = true;
                PopulateSSD();        

                dtpkrAnticipateDate.CustomFormat = "MM/DD/YYYY";
                dtpkrShipDate.CustomFormat = "MM/DD/YYYY";

                //dtpkrSSD.CustomFormat = "MM/DD/YYYY";
                //txtOrderDate.Text = DateTime.Now.ToString("MM/DD/YYYY");

                // HK : BM : 18-11-2009
                // The field "Total Display Ex Vat" should only be displayed if 
                // domain is US ("TDSE")
                txtTotalRetailExVat.Visible = false;
                lblTotalRetailExVat.Visible = false;

                // HK : BM : 18-11-2009
                // The checkbod "This PO is for a new Line" should only be displayed if 
                // domain is US ("TDSE")
                chkNewLineSelection.Visible = true;

                // //////////////////////////////////////////////////////////////
                // //////////////////////////////////////////////////////////////
                // HK : 17-12-2009 : Freight
                // //////////////////////////////////////////////////////////////
                DataColumn dc = new DataColumn("FreightId", typeof(string));
                dtFreight.Columns.Add(dc);
                dc = new DataColumn("FreightDesc", typeof(string));
                dtFreight.Columns.Add(dc);

                dtFreight.Rows.Add("", "      ");
                dtFreight.Rows.Add("P", "Pre Pay");
                dtFreight.Rows.Add("C", "Collect");

                cbxFreight.DisplayMember = "FreightDesc";
                cbxFreight.ValueMember = "FreightId";

                // Has to be explicitly set after setting the datamember and displaymember
                cbxFreight.DataSource = dtFreight;

                lblFreightCharges.Visible = true;
                cbxFreight.Visible = true;
                // //////////////////////////////////////////////////////////////
                
            }

            //Since the default value selected is for DC.
            dtSelectedStores = GetEmptyStores();
            dtSelectedStores.Rows.Add(true, MAGICDCSTORE, "Distribution Centre");
            _porder.PoType = PurchaseOrder.POType.StandardDCPO; ;

            txtLanding.Enabled = false;
            txtPortofEntry.Enabled = false;
            txtPortofDeparture.Enabled = false;
            txtDelTerms.Enabled = false;

            txtDept.Focus();

            // /////////////////////////////////////////////////////////////////////////
            // HHK : 09-11-2009 : PO line count should start at 0 not at 1
            //
            // //////////////////////////////////////////////////////////////////////////
            // HK : 08-12-2009 : _porder.NumofPOLines will be assigned by GetPoHeader ()
            //_porder.NumofPOLines = 0;
            //_porder.NumofPOLines = 1;

            //TODO  DateFormats
            //string sdateformat = _porder.Penvironment.DateFormat;

            // HKJ : 08-12-2009 : Currency Rate of Market may not be needed for editing a PO. 
            // However checek with FC ???
            // Capture the Market currency Rate. (Market is selected on the Market Selection form)
            //_currencyratemarket = validationcls.GetCurrency(_porder.MarketCurrency);

            // Capture the Default and Alternating cell styles in instance variables
            dgvcsPoLinesnormal      = new DataGridViewCellStyle(dtgrdPOLinesView.DefaultCellStyle);
            dgvcsPoLinesalternate   = new DataGridViewCellStyle(dtgrdPOLinesView.AlternatingRowsDefaultCellStyle);

            
            // /////////////////////////////////////////////////
            // HK : 1-12-2009 : Resolve Bug 90
            dtgrdPOLinesView.Enabled = true;

            // /////////////////////////////////////////////////////////////////////////
            // HHK : 09-11-2009 : Call the stub to init commomn values
            //
            // //////////////////////////////////////////////////////////////////////////
            //SetupStub();

            // /////////////////////////////////////////////////////////////////////////
            // HHK : 08-12-2009 : Databind the properties of the PO Header to the 
            // visual controls. 
            //
            // //////////////////////////////////////////////////////////////////////////

            SetupSimpleDataBinding();


       }


        #region Dates

        private void dtpkrShipDate_ValueChanged(object sender, EventArgs e)
        {
            
            // HK : 13-01-2010 : If user decided to Cancel the form do not 
            // force validation (as this wil cause validation errors on 
            // incorrect business logic
            if (_bFormCancelClicked == false)
            {
                if (_bDataBindingsInitalised == true)
                {

                    _porder.ShippingDate = dtpkrShipDate.Value;
                    //if (_porder.Penvironment.Domain == "TDSNA")
                    if (_porder.Penvironment.Domain == "SWNA")
                    //For TDSNA
                    {
                        txtCancelDate.Text = dtpkrShipDate.Value.AddDays(7).ToString("D");
                        _porder.CancelDate = dtpkrShipDate.Value.AddDays(7);
                    }

                    //For TDSE
                    // HK : 19-11-2009 : Format date as [LondDate]
                    txtCancelDate.Text = dtpkrShipDate.Value.ToLongDateString();
                    _porder.CancelDate = dtpkrShipDate.Value;

                    //if (DateTime.Compare (dtpkrAnticipateDate.Value, dtpkrShipDate.Value) > 0)
                    Debug.Print("Anticipate Date:" + dtpkrAnticipateDate.Value.Date.ToString());
                    Debug.Print("Ship Date :" + dtpkrShipDate.Value.Date.ToString());

                    if ((dtpkrAnticipateDate.Value.Date < dtpkrShipDate.Value.Date))
                    {
                        //errPOEntry.SetError(dtpkrShipDate, "Anticipate date less than ship date");
                        errPOEntry.SetError(dtpkrShipDate, "Shipping date cannot be after the anticipate date");
                    }
                    else
                    {

                        // ///////////////////////////////////////////////////////////////
                        // HK : 19-01-2010 : Fix Bug 254, 289
                        // 
                        // PO is now modified so enable the "PO Modify" button
                        btnCreatePO.Enabled = true;
                        IsPoModified = true;

                        errPOEntry.SetError(dtpkrShipDate, "");

                    }
                }
            }

        }

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
                if (dtpkrAnticipateDate.Value < DateTime.Today)
                {
                    // validationcls.HighlightErrControls(lblAnticipateDate, dtpkrAnticipateDate, false);
                    errPOEntry.SetError(dtpkrAnticipateDate, "Please enter a date greater than Today");
                    _porder.AnticipateDate = dtpkrAnticipateDate.Value;
                    e.Cancel = true;
                }
                else
                {

                    // ///////////////////////////////////////////////////////////////
                    // HK : 19-01-2010 : Fix Bug 254, 289
                    // No need to validate if the user is just tabbing out of the field
                    // and has not changed anything.
                    if (_porder.AnticipateDate == dtpkrAnticipateDate.Value)
                    {
                        Debug.Print("Anticipate Date field. Just tabbing out .....");
                        e.Cancel = false;
                        return;
                    }

                    // validationcls.HighlightErrControls(lblAnticipateDate, dtpkrAnticipateDate, true);
                    errPOEntry.SetError(dtpkrAnticipateDate, "");

                    // HK : 30-12-2009 : Dont know why ASH set the Anticipate Date to DateTime.Now
                    // in this event. The Validating event will always fire after the ValueChanged 
                    // event. The Validating event will fire when the dattime picker control looses 
                    // focus because the user rabs out of this control or clicks elsewhere on the 
                    // form or in another control/
                    // _porder.AnticipateDate = DateTime.Now;
                    _porder.AnticipateDate = dtpkrAnticipateDate.Value;

                    e.Cancel = false;

                    // ///////////////////////////////////////////////////////////////
                    // HK : 19-01-2010 : Fix Bug 254, 289
                    // 
                    // PO is now modified so enable the "PO Modify" button
                    btnCreatePO.Enabled = true;
                    IsPoModified = true;
                }
            }
        }

        private void dtpkrShipDate_Validating(object sender, CancelEventArgs e)
        {
            if (!_bFormCancelClicked)
            {
                if (dtpkrShipDate.Value < DateTime.Today)
                {
                    //  validationcls.HighlightErrControls(lblShipDate, dtpkrShipDate, false);
                    errPOEntry.SetError(dtpkrShipDate, "Please enter a date greater than  Today");
                    _porder.ShippingDate = dtpkrShipDate.Value;
                    e.Cancel = true;
                }
                else
                {
                    // ///////////////////////////////////////////////////////////////
                    // HK : 19-01-2010 : Fix Bug 254, 289
                    // No need to validate if the user is just tabbing out of the field
                    // and has not changed anything.
                    if (_porder.ShippingDate == dtpkrShipDate.Value)
                    {
                        Debug.Print("Shipping Date field. Just tabbing out .....");
                        e.Cancel = false;
                        return;
                    }

                    // validationcls.HighlightErrControls(lblShipDate, dtpkrShipDate, true);
                    errPOEntry.SetError(dtpkrShipDate, "");

                    // HK : 30-12-2009 : Dont knwo why ASH set the ShippingDate to DateTime.Now
                    // in this event. The Validating event will always fire after the ValueChanged 
                    // event. The Validating event will fire when the dattime picker control looses 
                    // focus because the user rabs out of this control or clicks elsewhere on the 
                    // form or in another control/
                    //_porder.ShippingDate = DateTime.Now;
                    _porder.ShippingDate = dtpkrShipDate.Value;

                    e.Cancel = false;

                    // ///////////////////////////////////////////////////////////////
                    // HK : 19-01-2010 : Fix Bug 254, 289
                    // 
                    // PO is now modified so enable the "PO Modify" button
                    btnCreatePO.Enabled = true;
                    IsPoModified = true;
                }
            }
        }

        private void dtpkrAnticipateDate_ValueChanged(object sender, EventArgs e)
        {
            
            // HK : 13-01-2010 : If user decided to Cancel the form do not 
            // force validation (as this wil cause validation errors on 
            // incorrect business logic
            if (_bFormCancelClicked == false)
            {

                if (_bDataBindingsInitalised == true)
                {

                    Debug.Print("Anticipate Date:" + dtpkrAnticipateDate.Value.Date.ToString());
                    Debug.Print("Ship Date :" + dtpkrShipDate.Value.Date.ToString());

                    // HK : 29-12-2009 : Set the anticipate date, otherwise it will turn up as 
                    // 01-01-01 in the database
                    _porder.AnticipateDate = dtpkrAnticipateDate.Value;

                    if ((dtpkrAnticipateDate.Value.Date < dtpkrShipDate.Value.Date))
                    {
                        errPOEntry.SetError(dtpkrAnticipateDate, "Anticipate date cannot be before the ship date");
                    }
                    else
                    {
                        errPOEntry.SetError(dtpkrAnticipateDate, "");

                        // ///////////////////////////////////////////////////////////////
                        // HK : 19-01-2010 : Fix Bug 254, 289
                        // 
                        // PO is now modified so enable the "PO Modify" button
                        btnCreatePO.Enabled = true;
                        IsPoModified = true;
                    }

                }
            }

        }

        #endregion Dates

        // //////////////////////////////////////////////////////////////////
        // HK : 16-11-2009 : If the PO type is other than Standart PO then 
        // VAT calulation is irrelevant
        // //////////////////////////////////////////////////////////////////
        private void DisplayPOSummaryNA()
        {

            // Blank the Po Summary Fields
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
                ClearSummary();

                // Total Retail
                _porder.TotalRetail     = _porder.CalculateTotalRetail();
                Debug.Print("From PO Summary");
                Debug.Print ("=========================================");
                Debug.Print ("Total Retail: " + _porder.TotalRetail.ToString ());
                
                // Total Cost
                //_porder.TotalCost       = _porder.CalculateTotalCost();

                // HK : KM : FC : 26-01-2010 : Use CalculateTotalCostInPoCurrency
                _porder.TotalCost = _porder.CalculateTotalCostInPoCurrency();
                Debug.Print ("Total Cost: " + _porder.TotalCost.ToString ());
                
                // Total Units
                _porder.TotalUnits      = _porder.CalculateTotalUnit();

                // HK : BM : Default the Landing to 1 if no Landing specified 
                // both for summary calculation purposes and for sending to database
                // i.e assign _porder.Landing = 1
                // But do not show it on the Landing field on the form
                // This code will fire if user has not entered any landing in the
                // Landing textbox
                if (_porder.Landing == 0)
                {
                    _porder.Landing = 1;
                }
                Debug.Print("Landing: " + (_porder.Landing).ToString());
                
                // Total Landed Cost
                _porder.TotalLandedCost     = Decimal.Round((_porder.TotalCost * (_porder.Landing)),2);
                Debug.Print("Total Landed Cost: " + _porder.TotalLandedCost.ToString());

                //Magic No Alert # of stores for DC PO
                txtNumberofDrops.Text = "1";

                // HK : 17-11-2009 : Total retail should now display Total retail Ex Vat
                // Infact the textbox txtTotalRetail is actually total retail ex vat
                totalretailexvat = _porder.CalculateTotalRetailExVat(MAGICDCSTOREVATCODE);
                Debug.Print("Total retail Ex VAT: " + totalretailexvat.ToString());

                // HK : 19-11-2009 : Display Total Retail as totalretail
                txtTotalRetail.Text = Decimal.Round(_porder.TotalRetail, 2).ToString(_currencyformat);

                marginvalue = (totalretailexvat - _porder.TotalLandedCost);
                Debug.Print("Margin Value: " + marginvalue.ToString());
                _porder.MarginValue = marginvalue;

                // HK : 20-11-2009 : IF user has not entered any quantity then retail 
                // value ex vat will be 0. So a division by zero will cause an exception.
                // Hence if totalretailexvat == 0 then do not calculate the margin percentage.

                if (totalretailexvat != 0)
                {
                    _porder.MarginPercentage = (marginvalue / totalretailexvat) * 100;
                    Debug.Print("Margin percentage: " + _porder.MarginPercentage.ToString());
                }
                else
                {
                    _porder.MarginPercentage = 0;
                    Debug.Print("Margin percentage not calculated as totalretailexvat = 0:");
                    Debug.Print("..... So Margin percentage set to 0");
                }

                // 15-01-2010 : 
                // Use new methods to Calculate the Po Lines / Packs
                UpdatePoLinesPacks();

                // ////////////////////////////////////////////////////////////////////

                //txtTotalCost.Text    = Decimal.Round (_porder.TotalCost).ToString();
                txtTotalCost.Text = Decimal.Round(_porder.TotalLandedCost, 2).ToString(_currencyformat);

                txtTotalUnits.Text   = _porder.TotalUnits.ToString();

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
                txtNumberofDrops.Text =   "1";
                }
            
            catch (Exception ex)
            { 
                // HK : 05-10-2009 : Catch the exception
                MessageBox.Show ("It appears that the pack size is 0", "PO Entry");
            }
        }

        private void ClearSummary()
        { 
                
        
        }

        private DataTable PopulatePOLines(int iHitNUmber)
        {
            // ///////////////////////////////////////////////////////////////////
            // HHK : 06-10-2009 
            // 1. Send all lines (not just one as it is currently doing) to the PO 
            //    line creation objects
            // 2. If the line item is a pack then send all the items that belong to 
            //    the pack
            // ///////////////////////////////////////////////////////////////////

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
            // dtAllPOLines.Columns.Add("CheckDigit", typeof(Int16));
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
            dtAllPOLines.Columns.Add("MinimumPack", typeof(Int32));
            dtAllPOLines.Columns.Add("DistroLot", typeof(Int32));
            dtAllPOLines.Columns.Add("VendorCost", typeof(decimal));
            dtAllPOLines.Columns.Add("LandFactor", typeof(decimal));
            dtAllPOLines.Columns.Add("Character", typeof(string)); // Character code

            // Total rowcount in the grid (The rows to process is one less than total rows)
            //poLineCount = dtgrdPOLinesView.Rows.Count;
            poLineCount = _porder.lstpoLineItemDetails.Count;

            for (iCount = 0; iCount < poLineCount; iCount++)
            {

                _polinedetails = _porder.lstpoLineItemDetails[iCount];

                // ///////////////////////////////////////////////////////////////////
                // HK : CJ : 26-11-2009 : Check if thsi is a Drop Ship Matrix PO.
                // If it is then replace the item quantity in the PO Line Item with the 
                // one from the Drop Ship Matrix datatable.

                int qty = _porder.GetItemQuatityOnHit(iHitNUmber,
                                                            _polinedetails.Itemindex,
                                                            _polinedetails.Classcode,
                                                            _polinedetails.Vendorcode,
                                                            _polinedetails.Stylecode,
                                                            _polinedetails.Colorcode,
                                                            _polinedetails.Itemsize);

                // Explicitly change the _polinedetails.Itemquantity for  
                // this store on this PO Line Item
                _polinedetails.Itemquantity = qty;
                Debug.Print("Item Details: " + "Store: " + _porder.ShipTo.ToString() + " Item Quantity: " + _polinedetails.Itemquantity.ToString());

                // ///////////////////////////////////////////////////////////////////

                dtAllPOLines.Rows.Add(_porder.Spiceponumber,
                                      _porder.SpicePoVersion,   //Version
                                       _polinedetails.Sequence, //Sequence
                               _polinedetails.Classcode,
                               _polinedetails.Vendorcode,
                               _polinedetails.Stylecode,
                               _polinedetails.Colorcode,
                               _polinedetails.Itemsize,
                               _polinedetails.Sku,//SKU
                               _polinedetails.SkuChk,
                    //         0,//Check Digit
                               _polinedetails.Upc,//UPC
                               _polinedetails.Itemquantity,
                    //         (_polinedetails.Cost * _porder.Landing * _polinedetails.Itemquantity), //Landed cost = Cost*LF
                    //(_polinedetails.Cost * _porder.Landing), //Landed cost = Cost*LF
                    //(_polinedetails.Cost * _currencyratemarket) / _currencyratepo, //Landed cost = Cost*LF
                               Decimal.Round((_polinedetails.Cost * _currencyratemarket) * _porder.Landing, 2),
                    //         _polinedetails.Retailprice * _polinedetails.Itemquantity,
                               _polinedetails.Retailprice,
                               _polinedetails.Itemlongdescription,
                               _polinedetails.Itemshortdescription,
                               _polinedetails.Vendorstyle,
                               _polinedetails.SeasonDesc,
                               _polinedetails.Subclass,
                               _polinedetails.Tickettype,
                               _polinedetails.Casepackqty,
                               _polinedetails.MinPack,
                    //         _polinedetails.Cost * _polinedetails.Itemquantity, //Vendor Cost 
                               _polinedetails.Cost, //Simple Vendor Cost 
                               _porder.Landing,
                               _polinedetails.Charactercode
                                );

                Debug.Print(_porder.Spiceponumber + "  " + DateTime.Now.ToString() + "  "
                                                         + Convert.ToString(_polinedetails.Classcode) + "   "
                                                         + Convert.ToString(_polinedetails.Vendorcode) + "   "
                                                         + Convert.ToString(_polinedetails.Stylecode) + "   "
                                                         + Convert.ToString(_polinedetails.Colorcode)
                                                         );
            }

            return dtAllPOLines;
        }
        
        private DataTable PopulatePOLines()
        {
            
            // ///////////////////////////////////////////////////////////////////
            // HHK : 06-10-2009 
            // 1. Send all lines (not just one as it is currently doing) to the PO 
            //    line creation objects
            // 2. If the line item is a pack then send all the items that belong to 
            //    the pack
            // ///////////////////////////////////////////////////////////////////

            DataTable dtAllPOLines = new DataTable ();
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
            dtAllPOLines.Columns.Add("MinimumPack", typeof(Int32));
            dtAllPOLines.Columns.Add("DistroLot",   typeof(Int32));
            dtAllPOLines.Columns.Add("VendorCost",  typeof(decimal));
            dtAllPOLines.Columns.Add("LandFactor",  typeof(decimal));
            dtAllPOLines.Columns.Add("Character",   typeof(string)); // Character code

            // Total rowcount in the grid (The rows to process is one less than total rows)
            //poLineCount = dtgrdPOLinesView.Rows.Count;
            poLineCount = _porder.lstpoLineItemDetails.Count;

            for (iCount = 0; iCount < poLineCount ; iCount++)
            {

                _polinedetails = _porder.lstpoLineItemDetails[iCount];

                // HK : CJ : 26-01-2010 : Fix Bug 311. SeasonCode to be used instead of SeasonDesc
                dtAllPOLines.Rows.Add(_porder.Spiceponumber,
                                      _porder.SpicePoVersion, //Version
                                      _polinedetails.Sequence,
                                      _polinedetails.Classcode,
                                      _polinedetails.Vendorcode,
                                      _polinedetails.Stylecode,
                                      _polinedetails.Colorcode,
                                      _polinedetails.Itemsize,
                                      _polinedetails.Sku,//SKU
                                      _polinedetails.SkuChk,
                                      _polinedetails.Upc,//UPC
                                      _polinedetails.Itemquantity,
                                      _polinedetails.LandedCost,
                                      _polinedetails.Retailprice,
                                      _polinedetails.Itemlongdescription,
                                      _polinedetails.Itemshortdescription,
                                      _polinedetails.Vendorstyle,
                                      _polinedetails.SeasonCode,
                                      _polinedetails.Subclass,
                                      _polinedetails.Tickettype,
                                      _polinedetails.Casepackqty,
                                      _polinedetails.MinPack,
                                      _polinedetails.ConvertedCost,
                                      _porder.Landing,
                                      _polinedetails.Charactercode
                                );

                Debug.Print(_porder.Spiceponumber + "  " + DateTime.Now.ToString() + "  "
                                                         + Convert.ToString(_polinedetails.Classcode)  + "   "
                                                         + Convert.ToString(_polinedetails.Vendorcode) + "   "
                                                         + Convert.ToString(_polinedetails.Stylecode)  + "   "
                                                         + Convert.ToString(_polinedetails.Colorcode)
                                                         );
            }

            return dtAllPOLines;
            
        
        }

        private void POEntryForm1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            
            //if (MessageBox.Show("Are you sure you want to exit ?", "SPICE PO ENTRY MAINTAINENCE", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //{
                    
            //    e.Cancel = false;
             
            //}
        }
                
        private bool IsOrderValid()
        {

            bool bInvalid;
            //TODO

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
         
        #region Comments
        private void txtVendorComment1_Validating(object sender, CancelEventArgs e)
        {

            // ///////////////////////////////////////////////////////////////
            // HK : 19-01-2010 : Fix Bug 254, 289
            // No need to validate if the user is just tabbing out of the field
            // and has not changed anything.
            if (_porder.Vendorcomments1 == txtVendorComment1.Text)
            {
                Debug.Print("Vendor Comments 1 field. Just tabbing out .....");
                e.Cancel = false;
                return;
            }
            // ///////////////////////////////////////////////////////////////

            _porder.Vendorcomments1 = txtVendorComment1.Text;

            // ///////////////////////////////////////////////////////////////
            // HK : 19-01-2010 : Fix Bug 254, 289
            // 
            // PO is now modified so enable the "PO Modify" button
            btnCreatePO.Enabled = true;
            IsPoModified = true;
        }

        private void txtVendorComment2_Validating(object sender, CancelEventArgs e)
        {
            // ///////////////////////////////////////////////////////////////
            // HK : 19-01-2010 : Fix Bug 254, 289
            // No need to validate if the user is just tabbing out of the field
            // and has not changed anything.
            if (_porder.Vendorcomments2 == txtVendorComment2.Text)
            {
                Debug.Print("Vendor Comments 2 field. Just tabbing out .....");
                e.Cancel = false;
                return;
            }
            // ///////////////////////////////////////////////////////////////

            _porder.Vendorcomments2 = txtVendorComment2.Text;

            // ///////////////////////////////////////////////////////////////
            // HK : 19-01-2010 : Fix Bug 254, 289
            // 
            // PO is now modified so enable the "PO Modify" button
            btnCreatePO.Enabled = true;
            IsPoModified = true;
        }

        private void txtVendorComment3_Validating(object sender, CancelEventArgs e)
        {
            // ///////////////////////////////////////////////////////////////
            // HK : 19-01-2010 : Fix Bug 254, 289
            // No need to validate if the user is just tabbing out of the field
            // and has not changed anything.
            if (_porder.Vendorcomments3 == txtVendorComment3.Text)
            {
                Debug.Print("Vendor Comments 3 field. Just tabbing out .....");
                e.Cancel = false;
                return;
            }
            // ///////////////////////////////////////////////////////////////

            _porder.Vendorcomments3 = txtVendorComment3.Text;

            // ///////////////////////////////////////////////////////////////
            // HK : 19-01-2010 : Fix Bug 254, 289
            // 
            // PO is now modified so enable the "PO Modify" button
            btnCreatePO.Enabled = true;
            IsPoModified = true;
        }

        private void txtInternalComments1_Validating(object sender, CancelEventArgs e)
        {

            // ///////////////////////////////////////////////////////////////
            // HK : 19-01-2010 : Fix Bug 254, 289
            // No need to validate if the user is just tabbing out of the field
            // and has not changed anything.
            if (_porder.Internalcomments1 == txtInternalComments1.Text)
            {
                Debug.Print("Internal Comments 1 field. Just tabbing out .....");
                e.Cancel = false;
                return;
            }
            // ///////////////////////////////////////////////////////////////

            _porder.Internalcomments1 = txtInternalComments1.Text;

            // ///////////////////////////////////////////////////////////////
            // HK : 19-01-2010 : Fix Bug 254, 289
            // 
            // PO is now modified so enable the "PO Modify" button
            btnCreatePO.Enabled = true;
            IsPoModified = true;
        }

        private void txtInternalComments2_Validating(object sender, CancelEventArgs e)
        {
            // ///////////////////////////////////////////////////////////////
            // HK : 19-01-2010 : Fix Bug 254, 289
            // No need to validate if the user is just tabbing out of the field
            // and has not changed anything.
            if (_porder.Internalcomments2 == txtInternalComments2.Text)
            {
                Debug.Print("Internal Comments 2 field. Just tabbing out .....");
                e.Cancel = false;
                return;
            }
            // ///////////////////////////////////////////////////////////////
            
            _porder.Internalcomments2 = txtInternalComments2.Text;

            // ///////////////////////////////////////////////////////////////
            // HK : 19-01-2010 : Fix Bug 254, 289
            // 
            // PO is now modified so enable the "PO Modify" button
            btnCreatePO.Enabled = true;
            IsPoModified = true;
        }
        #endregion Comments

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

            // ///////////////////////////////////////////////////////////////
            // HK : 26-01-2010 : Fix Bug 254, 289, 361
            // 
            // PO is now modified so enable the "PO Modify" button
            btnCreatePO.Enabled = true;
            IsPoModified = true;
        }
       
        private void RefreshItemFromBO(int index)
        {

            dtgrdPOLinesView["Quantity",index].Value = _polinedetails.Itemquantity;
            dtgrdPOLinesView["Cost",index].Value = _polinedetails.Cost;
            CalculatePOSummary();
        }

        private void txtShipVia_Validating(object sender, CancelEventArgs e)
        {
            // HK : 01-12-2009 : Dont validate when user is trying to close the form
            if (!_bFormCancelClicked)
            {
                // HK : FC : Resolve Bug 96
                // Unhilight any validation errors that may have appeared
                // due to the click of the Create PO button
                validationcls.HighlightErrControls(lblLanding, txtLanding, true);
                errPOEntry.SetError(txtLanding, "");
                // ///////////////////////////////////////////////////////////////

                // ///////////////////////////////////////////////////////////////
                // HK : 19-01-2010 : Fix Bug 254, 289
                // No need to validate if the user is just tabbing out of the field
                // and has not changed anything.
                if (_porder.ShipViaCode == txtShipVia.Text)
                {
                
                    Debug.Print("Txt Ship Via field. Just tabbing out .....");
                    e.Cancel = false;
                    return;
                }
                // ///////////////////////////////////////////////////////////////

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

                    // ///////////////////////////////////////////////////////////////
                    // HK : 19-01-2010 : Fix Bug 254, 289
                    // 
                    // PO is now modified so enable the "PO Modify" button
                    btnCreatePO.Enabled = true;
                    IsPoModified = true;

                }
                else
                {

                    //Populate the error

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

                // /////////////////////////////////////////////////////////////////////////
                // HK : 14-01-2010 : Fix Bug 247
                // We could reset Validation highlights here. If we disable the controls
                // then we must un hilight the previous validation error hilighted.
                validationcls.HighlightErrControls(lblPortofDeparture, txtPortofDeparture, true);
                errPOEntry.SetError(txtPortofDeparture, "");

                validationcls.HighlightErrControls(lblPortofEntry, txtPortofEntry, true);
                errPOEntry.SetError(txtPortofEntry, "");

                validationcls.HighlightErrControls(lblDelTerms, txtDelTerms, true);
                errPOEntry.SetError(txtDelTerms, "");
                // /////////////////////////////////////////////////////////////////////////

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
       
        private void btnDeleteLine_Click(object sender, EventArgs e)
        {


            // HK : 27-11-2009 : To Do : Check that the row being tested in not a new row.
            // i.e skip any rows that have a status of IsNewRow = true

            // ////////////////////////////////////////////////////////////////////
            // HK : Bug 70 : A number of annoyances were observed in the navigation 
            // and validation of datagridview.
            // ////////////////////////////////////////////////////////////////////
            int iRowsDeleted = 0;
            int iRunningCountTotalRows = 0;
            int iLoopCounter;

            int iitemindex;
            short iClass;
            int iVendor;
            short iStyle;
            short iColor;
            short iSize;

            _bUserWantsToDeleteLine = true;

            // Init the looping counter
            iLoopCounter = 0;
            iRunningCountTotalRows = dtgrdPOLinesView.Rows.Count;

            // ////////////////////////////////////////////////
            // HK : FC : BM : 09-12-2009. Fix Bug 132
            //if (_polinedetails != null && _polinedetails.Isvalid == false)
            /*
            if (_polinedetails != null)
            {
                _bUserWantsToDeleteLine = false;
                return;
            }
            */
            // ////////////////////////////////////////////////

            do
            {
                if (!dtgrdPOLinesView.Rows[iLoopCounter].IsNewRow)
                {

                    // ////////////////////////////////////////////////
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

                    if (dtgrdPOLinesView.Rows[iLoopCounter].Cells[0].Value != null &&
                                Convert.ToBoolean(dtgrdPOLinesView.Rows[iLoopCounter].Cells[0].Value) == true)
                    {

                        Debug.Print("Data Grid row removed at index: " + iLoopCounter.ToString());
                        DisplayDataGridItems(iLoopCounter);

                        // HK : 30-11-2009 : The below RemoveAt will cause a row enter event to fire.
                        // As we delete the row at iLoopCounter, the nex valid row (if any) will become 
                        // the current row.
                        dtgrdPOLinesView.Rows.RemoveAt(iLoopCounter);

                        // HK : Bug : 70 : Remove the reference to this PO Line Item from 
                        // the POItems collection
                        // ////////////////////////////////////////////////////////
                        // HK : 30-11-2009 : If Item is a APP then decrement the 
                        // pack count
                        // ///////////////////////////////////////////////////////
                        if (_porder.lstpoLineItemDetails[iLoopCounter].APP1 == "Y")
                        {
                            _porder.NumofPOPacks = _porder.NumofPOPacks - 1;
                        }
                        // ////////////////////////////////////////////////////////

                        // Display the items on the class object in the collection about to be deleted
                        Debug.Print("Item Collection to be deleted: " + iLoopCounter.ToString());
                        DisplayLineItemDetails(_porder.lstpoLineItemDetails[iLoopCounter]);

                        _porder.lstpoLineItemDetails.RemoveAt(iLoopCounter);
                        Debug.Print("Item Collection count: " + _porder.lstpoLineItemDetails.Count.ToString());

                        iRowsDeleted++;

                        iRunningCountTotalRows = dtgrdPOLinesView.Rows.Count;

                    }
                    else
                    {
                        Debug.Print("Data Grid row skipped index: " + iLoopCounter.ToString());
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
            Debug.Print("DataGrid Items after delete");
            Debug.Print("=========================================");
            DisplayDataGridItems();
            Debug.Print("=========================================");

            Debug.Print("Items collection after delete");
            Debug.Print("=========================================");
            DisplayItemCollection();
            Debug.Print("=========================================");

            // /////////////////////////////////////////////////
            // ?? To Do ?? To Do
            // HK : 28-11-2009 : After rows have been deleted,  
            // certain row(s) that were duplicate will not be 
            // duplicate as the user must have deleted their 
            // duplicate(s). So we must un highlight them.


            // /////////////////////////////////////////////////

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

        private void ClearPoItemsDataGrid()
        {
            // HK : 01-01-2009 : Clears the records displayed in the datagrid

            // HK : 27-11-2009 : To Do : Check that the row being tested in not a new row.
            // i.e skip any rows that have a status of IsNewRow = true

            // ////////////////////////////////////////////////////////////////////
            // HK : Bug 70 : A number of annoyances were observed in the navigation 
            // and validation of datagridview.
            // ////////////////////////////////////////////////////////////////////
            int iRowsDeleted = 0;
            int iRunningCountTotalRows = 0;
            int iLoopCounter;

            int iitemindex;
            short iClass;
            int iVendor;
            short iStyle;
            short iColor;
            short iSize;

            _bUserWantsToDeleteLine = true;

            // Init the looping counter
            iLoopCounter = 0;
            iRunningCountTotalRows = dtgrdPOLinesView.Rows.Count;

            do
            {
                if (!dtgrdPOLinesView.Rows[iLoopCounter].IsNewRow)
                {

                    // ////////////////////////////////////////////////
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
                        dtgrdPOLinesView.Rows.RemoveAt(iLoopCounter);

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

                        iRunningCountTotalRows = dtgrdPOLinesView.Rows.Count;

                    }
                    else
                    {
                        //Debug.Print("Data Grid row skipped index: " + iitemindex.ToString());
                        //DisplayDataGridItems(iLoopCounter);
                        /*
                        Debug.Print("Class Code:" + iClass.ToString()
                                      + "Vendor Code:" + iVendor.ToString()
                                      + "Style Code:" + iStyle.ToString()
                                      + "Color Code:" + iColor.ToString()
                                      + "Size Code:" + iSize.ToString()
                                      + "");
                         */

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

        private void dtgrdPOLinesView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
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

        private void txtPortofDeparture_Validating(object sender, CancelEventArgs e)
        {
         
            // HK : 01-12-2009 : Dont validate when user is trying to close the form
            if (!_bFormCancelClicked)
            {

                // ///////////////////////////////////////////////////////////////
                // HK : 19-01-2010 : Fix Bug 254, 289
                // No need to validate if the user is just tabbing out of the field
                // and has not changed anything.
                if (_porder.Portofdeparturecode == Int32.Parse(txtPortofDeparture.Text))
                {
                    Debug.Print("Txt Port of Departure field. Just tabbing out .....");
                    e.Cancel = false;
                    return;
                }
                // ///////////////////////////////////////////////////////////////

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

                    // ///////////////////////////////////////////////////////////////
                    // HK : 19-01-2010 : Fix Bug 254, 289
                    // 
                    // PO is now modified so enable the "PO Modify" button
                    btnCreatePO.Enabled = true;
                    IsPoModified = true;

                }
                else
                {

                    //Populate the error

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

                // ///////////////////////////////////////////////////////////////
                // HK : 19-01-2010 : Fix Bug 254, 289
                // No need to validate if the user is just tabbing out of the field
                // and has not changed anything.
                if (_porder.Portofentrycode == Int32.Parse(txtPortofEntry.Text))
                {
                    Debug.Print("Txt Port of Entry field. Just tabbing out .....");
                    e.Cancel = false;
                    return;
                }
                // ///////////////////////////////////////////////////////////////


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

                    // ///////////////////////////////////////////////////////////////
                    // HK : 19-01-2010 : Fix Bug 254, 289
                    // 
                    // PO is now modified so enable the "PO Modify" button
                    btnCreatePO.Enabled = true;
                    IsPoModified = true;

                }
                else
                {

                    //Populate the error
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

                // ///////////////////////////////////////////////////////////////
                // HK : 19-01-2010 : Fix Bug 254, 289
                // No need to validate if the user is just tabbing out of the field
                // and has not changed anything.
                if (_porder.Deltermscode == txtDelTerms.Text)
                {
                    Debug.Print("Txt Delivery Terms field. Just tabbing out .....");
                    e.Cancel = false;
                    return;
                }
                // ///////////////////////////////////////////////////////////////

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

                    // ///////////////////////////////////////////////////////////////
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

        private void txtShipVia_TextChanged(object sender, EventArgs e)
        {
             
        }

        private void cmbShipTo_Validating(object sender, CancelEventArgs e)
        {
            
        }

        private void dtgrdPOLinesView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private bool ValidateQuantity(string svalue, int packQty, int rowindex)
        {
            bool bisValid = false;
            int itemqtyinput;

            // Fix Bug 94
            // HK : 11-01-2010 : Open the rounding form irrespective of 
            // whether the quantity entered is less than case pack quantity or not 

            //if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput) && itemqtyinput >= packQty )
            if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput))
            {

                if (itemqtyinput % packQty != 0)
                {
                    // HK : 04-01-2010 : Must send the datagridview row index, otherwise it will not know 
                    // which record to update
                    ItemQuantityForm itemqtyform = new ItemQuantityForm(itemqtyinput, packQty, ref _polinedetails, rowindex);

                    itemqtyform.OnQuantityRounded += new ItemQuantityForm.QuantityRoundedEventHandler(itemqtyform_OnQuantityRounded);
                    if (itemqtyform.ShowDialog(this) == DialogResult.OK)
                    {
                        bisValid = true;
                    }
                    else
                    {
                        bisValid = false;
                    }
                }
                else
                {
                    bisValid = true;
                }
            }
            else
            {
                bisValid = false;

            }

            return bisValid;
        
        }

        void itemqtyform_OnQuantityRounded(object sender, int iroundedquantity)
        {
            int iqty;

            _itemquantityrounded = iroundedquantity;
        }

        // /////////////////////////////////////////////////
        // Sums the quantity assigned to each item for 
        // the store in question
        // /////////////////////////////////////////////////
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

        private void bkgrndWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            string sStore = String.Empty;
            int istorequantity = 0;

            // /////////////////////////////////////////////////////////////////
            // HK : 02-12-2009 : Drop Ship Single, Drop Ship Matrix, Standard PO
            //if (!CreatePurchaseOrder())
            if (!ModifyPurchaseOrder(_spiceponumber))
            {
                e.Cancel = true;
                //break;
            }

            bkgrndWorker.ReportProgress(index);

            /*
            foreach (DataRow dtrow in dtSelectedStores.Rows)
            {

                // ///////////////////////////////////////////////////////////////////////////
                // HK : CJ : 26-11-2009 : Check if the quantity assigned every PO Line Item 
                // for that store in question is > 0. If not then skip this store (ie. do not 
                // create a PO for this store)
                // ///////////////////////////////////////////////////////////////////////////
                if (rdBtnDropShipMatrix.Checked)
                {
                    sStore = dtrow["clmStore"].ToString ();
                    istorequantity = SumStoreQuantity(sStore);

                    if (istorequantity == 0)
                    {
                        continue;
                    }

                }
                /////////////////////////////////////////////////////////////////////////////
                
                _porder.ShipTo = Convert.ToInt16(dtrow["clmStore"].ToString());
                index++;

                Debug.Print("Creating Po for Store:" + _porder.ShipTo.ToString() + " Store quantity:" + istorequantity.ToString ());
                
                // /////////////////////////////////////////////////////////////////
                // HK : 02-12-2009 : Drop Ship Single, Drop Ship Matrix, Standard PO
                //if (!CreatePurchaseOrder())
                if (!ModifyPurchaseOrder (_spiceponumber))
                {
                    e.Cancel = true;
                    break;
                }

                bkgrndWorker.ReportProgress(index);
                 
            }
            */
            
            Debug.Print("Finished processing" + DateTime.Now.ToString());                     
                    
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
            Debug.Print("Finished run completed" + DateTime.Now.ToString());                     
            pwindow.Close();
            this.Close();
             
        }

        private void txtPortofDeparture_EnabledChanged(object sender, EventArgs e)
        {
            
            errPOEntry.Clear();
        }


        // ////////////////////////////////////////////////////////////
        // HK : 20-11-2009 : Checks for duplicate items and return the 
        // first item found that is duplicate
        // The collection lstpoLineItemDetails will hold all the 
        // Po Line Items entered so far
        // ////////////////////////////////////////////////////////////
        private Boolean CheckForDuplicateLine(short itemclass, int vendor, short style, short colour, string size, ref int rowid)
        {
            // HK ?? Could be implemented using Find and Findall for custom collection
            // on the class POItemDetails and class PurchaseOrder

            // Find will return the index of the found item
            // FinaAll will return a List<POItemDetails> of all items
            // that match

            return false;

        }

        // ////////////////////////////////////////////////////////////
        // HK : 20-11-2009 : Checks for duplicate items 
        // 
        // The collection lstpoLineItemDetails will hold all the 
        // Po Line Items entered so far
        // ////////////////////////////////////////////////////////////
        private Boolean CheckForDuplicateLine(int itemindex, short itemclass, int vendor, short style, short colour, string size)
        {
            Boolean bSuccess = false;
            Int16 iClass;
            Int32 iVendor;
            Int16 iStyle;
            Int16 iColour;
            string ssize;
            short iSize;

            foreach (POItemDetails item in _porder.lstpoLineItemDetails)
            {

                iClass = item.Classcode;
                iVendor = item.Vendorcode;
                iStyle = item.Stylecode;
                iColour = item.Colorcode;
                ssize = item.Itemsize.ToString ();
                //iSize = item.;

                // Check against valid items only.
                // For a PO Item row that is being validated then the Size will be null 
                // as this method iscalled from the "Size" validation 
                if (item.Isvalid && (item.Itemindex != itemindex))
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

        private void btnAddItem_Click(object sender, EventArgs e)
        {

            short       itemclass;
            short       dept;
            short       vendor;
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

            itemclass   = 0;
            dept        = Convert.ToInt16 (txtDept.Text);
            vendor      = Convert.ToInt16(txtVendor.Text);

            Disney.Spice.ItemsUI.SelectItem frmSelectItem = new SelectItem(_porder.DbParamRef, _porder.UserName, 
                                                                            _porder.Penvironment, 
                                                                            _mdiparent,  itemclass, itemclasslck, 
                                                                            dept, deptlck, vendor, vendorlck);

            dtSelectedItems = frmSelectItem.GetSelectedItems();

            // HK : Fix Bug 215 : 12-01-2010
            char padchar = Convert.ToChar(" ");
            Boolean bItemLookupError = false;

            if (dtSelectedItems.Rows.Count > 0)
            {

                iStartRow = iTotalRowsInGrid = dtgrdPOLinesView.Rows.Count;

                foreach (DataRow dr in dtSelectedItems.Rows)
                {

                    // Insert the row and populate the Pk values. 
                    // Also make sure the Check Box in the first column is un checked

                    iClass = (Int16)dr["Class"];
                    Debug.Print("Class :" + iClass.ToString());

                    // HK : 25-01-2010 : Fix Bug 323 : 
                    // HK : 25-01-2010 : Add the new record only if the item lookup on the 
                    // selected item succeeds.
                    /*
                    iStartRow = dtgrdPOLinesView.Rows.Add(false, (Int16)dr["class"],
                                                                        dr["vendor"],
                                                                        dr["style"],
                                                                        dr["colour"],
                                                                        dr["size"]);
                    */
                    // Create a new item for the POItemDetails collection
                    POItemDetails poitem = new POItemDetails((short)dr["class"], (int)dr["vendor"], 
                                                    (short)dr["style"], (short)dr["colour"],
                                                    (short)dr["size"], iStartRow + 1);

                    // HK : CJ : 10-12-2009 : Property ItemIndex on class  POItemDetails should be 
                    // the same as the datagrid row index for that item
                    nextsequencenumber = GetNextSequenceNumber();
                    poitem.Sequence = nextsequencenumber;

                    // ////////////////////////////////////////////////////////////
                    // HK : 22-12-2009 : Some attributes of the PO item are populated 
                    // by the validation classes and not ItemLookup
                    // HK : 19-01-2010 : Fix Bug 270

                    // ///////////////////////////////////////////////////////////////
                    // ClassName
                    List<string> retValues = validationcls.ValidateClass(poitem.Classcode.ToString());
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

                    // ///////////////////////////////////////////////////////////////

                    // Lookup the item to populate the class with the rest of the details
                    if (poitem.ItemLookup(_porder.DbParamRef, _porder.UserName, _porder.Penvironment, _porder.DefaultMarket))
                    {

                        // HK : 25-01-2010 : Fix Bug 323 : 
                        // Add the new record to the end of the Rows collection of the datagrid
                        // HK : 25-01-2010 : Add the new record only if the item lookup on the 
                        // selected item succeeds.
                        iStartRow = dtgrdPOLinesView.Rows.Add  (false, (Int16)dr["class"],
                                                                            dr["vendor"],
                                                                            dr["style"],
                                                                            dr["colour"],
                                                                            dr["size"]);

                        dtgrdPOLinesView.Rows[iStartRow].Cells["Description"].Value     = poitem.Itemlongdescription;
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Retail"].Value          = poitem.Retailprice.ToString();
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Cost"].Value            = poitem.Cost.ToString();
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Character"].Value       = poitem.Characterdesc;
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Season"].Value          = poitem.SeasonDesc;
                        dtgrdPOLinesView.Rows[iStartRow].Cells["CasePackType"].Value    = poitem.Packdescription;
                        dtgrdPOLinesView.Rows[iStartRow].Cells["TicketType"].Value      = poitem.Tickettype;
                        //This will determine if the qty and cost can be changed 
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Pack"].Value            = poitem.APP1;

                        // Increment the number of lines
                        _porder.NumofPOLines += 1;

                        // HK : 14-01-2010 : Fix Bug 233
                        poitem.LandedCost = Decimal.Round((poitem.Cost * _currencyratemarket) * _porder.Landing, 2);
                        // Display Landed Cost in datagrid
                        dtgrdPOLinesView.Rows[iStartRow].Cells["LandedCost"].Value = poitem.LandedCost;

                        // ///////////////////////////////////////////////////////////////////////
                        // HK : 16-11-2009 : Display Converted cost and hide actual cost from database
                        // 
                        // ///////////////////////////////////////////////////////////////////////
                        
                        // Cannot divide by zero
                        if (_currencyratepo != 0)
                        {
                            // HK : 14-01-2010 : Fix Bug 233
                            poitem.ConvertedCost = Decimal.Round((poitem.Cost * _currencyratemarket) / _currencyratepo, 2);

                            dtgrdPOLinesView.Rows[iStartRow].Cells["ConvertedCost"].Value = poitem.ConvertedCost;
                        
                        }
                        else
                        {
                            Debug.Print ("Add Item : While calculating converted cost, it was found that the PO Currency Rate is 0"); 
                        }

                        if (poitem.APP1 == "Y")
                        {
                            //Make the UnitCost Readonly
                            dtgrdPOLinesView.Rows[iStartRow].Cells["ConvertedCost"].ReadOnly = true;

                        }
                        else
                        {
                            //Not an APP
                            //Enable cost
                            dtgrdPOLinesView.Rows[iStartRow].Cells["ConvertedCost"].ReadOnly = false;
                        }

                        _porder.lstpoLineItemDetails.Add(poitem);
                    }
                    else
                    {
                        /*
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Description"].Value = _polinedetails.Itemlongdescription;
                        DataGridClearItemnotfound(iStartRow);

                        _polinedetails.Isvalid = false;
                        
                        // ?????? HK : Check before implementing
                        poitem.Isvalid = false;
                        */
                        // //////////////////////////////////
                        // HK : Fix Bug 215 : 12-01-2010
                        // //////////////////////////////////

                        //_polinedetails.Isvalid = false;

                        // ?????? HK : Check before implementing
                        poitem.Isvalid = false;

                        // HK : 12-01-2010 : FC has managed to get an item into the PO Lines
                        // event though it does not exist in the database.
                        bItemLookupError = true;

                        iClass = poitem.Classcode;
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

                // HK : 22-01-2010 : Fix Bug 312 :
                btnCreatePO.Enabled = true;

            }

        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx");                     
             

        }

        private void dtgrdPOLinesView_SelectionChanged(object sender, EventArgs e)
        {
            //Debug.Print("Selection Changed fired");
        }


        // ///////////////////////////////////////////////////////////////////
        // HHK : 06-11-2009 : Initalise the form with common values, so as to
        // save typing
        // ///////////////////////////////////////////////////////////////////
        private void SetupStub()
        {
            txtDept.Text        = "9";
            txtVendor.Text      = "1584";
            txtCurrency.Text    = "US";
            txtShipVia.Text     = "OCN";
            txtLanding.Text     = "1";

            /*
            // Add first row
            dtgrdPOLinesView.Rows.Add();

            dtgrdPOLinesView.Rows[0].Cells[1].Value = "1104";
            dtgrdPOLinesView.Rows[0].Cells[2].Value = "3421";
            dtgrdPOLinesView.Rows[0].Cells[3].Value = "6321";
            dtgrdPOLinesView.Rows[0].Cells[4].Value = "0";
            dtgrdPOLinesView.Rows[0].Cells[12].Value = "9999";

            // add second row
            dtgrdPOLinesView.Rows.Add();

            dtgrdPOLinesView.Rows[0].Cells[1].Value = "1106";
            dtgrdPOLinesView.Rows[0].Cells[2].Value = "1584";
            dtgrdPOLinesView.Rows[0].Cells[3].Value = "1234";
            dtgrdPOLinesView.Rows[0].Cells[4].Value = "0";
            dtgrdPOLinesView.Rows[0].Cells[12].Value = "0";
            */

        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            dtgrdPOLinesView.Rows.Add();

            dtgrdPOLinesView.Rows[0].Cells[1].Value = "1104";
            dtgrdPOLinesView.Rows[0].Cells[2].Value = "3421";
            dtgrdPOLinesView.Rows[0].Cells[3].Value = "6321";
            dtgrdPOLinesView.Rows[0].Cells[4].Value = "0";
            dtgrdPOLinesView.Rows[0].Cells[12].Value = "777";
             */
             
            
            //dtgrdPOLinesView.CurrentCell = dtgrdPOLinesView[1, 1];


            // Cell coloring
            _cellbackgroundcolor = dtgrdPOLinesView[1, 1].Style.BackColor;
            dtgrdPOLinesView[1, 1].Style.BackColor = System.Drawing.Color.Red;

            Font newFont = new Font(dtgrdPOLinesView.Font, FontStyle.Strikeout);
            dtgrdPOLinesView[1, 1].Style.Font = newFont;


        }

        private void dtgrdPOLinesView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            // //////////////////////////////////////////////////////////////////
            // HK : Cater for quantity Rounding

            // //////////////////////////////////////////////////////////////////

            Debug.Print("Cell Validated Fired");
            
            if (dtgrdPOLinesView.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {
                if (_itemquantityrounded > 0)
                {
                    dtgrdPOLinesView.Rows[e.RowIndex].Cells["Quantity"].Value = _itemquantityrounded;
                    _itemquantityrounded = 0;
                }

                // /////////////////////////////////////////////////////////////////////
                // HK : 11-11-2009 : Dont know if CalculatePOSummary() is needed to be 
                // called from here
            }

            if (dtgrdPOLinesView.Columns[e.ColumnIndex].Name.Equals("Size"))
            {
                if (_bDuplicateItem)
                {

                    // Always in Red
                    SetItemColor (e.ColumnIndex, e.RowIndex, System.Drawing.Color.Red);

                    _bDuplicateItem = false;

                    // We need to blank out all the fields
                    /*
                    _polinedetails.Classname = String.Empty;
                    _polinedetails.Classcode = 0;
                    _polinedetails.Vendorcode = 0;
                    _polinedetails.Vendordesc = String.Empty;
                    _polinedetails.Stylecode = 0;
                    _polinedetails.Colorcode = 0;
                    _polinedetails.Colordesc = String.Empty;

                    dtgrdPOLinesView.Rows[e.RowIndex].Cells["Class"].Value = String.Empty;
                    dtgrdPOLinesView.Rows[e.RowIndex].Cells["Vendor"].Value = String.Empty;
                    dtgrdPOLinesView.Rows[e.RowIndex].Cells["Style"].Value = String.Empty;
                    dtgrdPOLinesView.Rows[e.RowIndex].Cells["Color"].Value = String.Empty;
                    dtgrdPOLinesView.Rows[e.RowIndex].Cells["Size"].Value = String.Empty;
                    */

                    /*
                    //dtgrdPOLinesView.EditMode = DataGridViewEditMode.EditProgrammatically;

                    try
                    {
                        //dtgrdPOLinesView.CurrentCell = this.dtgrdPOLinesView.Rows[e.RowIndex].Cells["Class"];
                        //dtgrdPOLinesView.CurrentCell = dtgrdPOLinesView[1, 1];
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, ex.Message);
                    }

                    //dtgrdPOLinesView.CurrentCell = this.dtgrdPOLinesView.Rows[0].Cells[0];
                    //dtgrdPOLinesView.EditMode = DataGridViewEditMode.EditOnF2;
                    */

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

        private void LoadCurrencyFromXMLDocument()
        {

            _dtCurrency = null;

            _dtCurrency = new DataTable("Currency");

            _dtCurrency.Columns.Add("CurrencyCode", typeof(string));
            _dtCurrency.Columns.Add("CurrencyName", typeof(string));
            _dtCurrency.Columns.Add("CurrencyRate", typeof(decimal));
            _dtCurrency.Columns.Add("CurrencyCodeName", typeof(decimal));

            /*
            try
            {
                //open the file using a Stream
                using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    //create the table with the appropriate column names
                    table.Columns.Add("Name", typeof(string));
                    table.Columns.Add("Power", typeof(int));
                    table.Columns.Add("Location", typeof(string));

                    //use ReadXml to read the XML stream
                    _dtCurrency.ReadXml(stream);

                    //return the results
                    //return table;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source);
                //return table;
            }
             */
        }

        private void dtgrdPOLinesView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

            // HK : 18-12-2009 : Do not allow this event if ColumnIndex  = -1
            if (e.ColumnIndex == -1)
            {
                return;
            }

            if (!dtgrdPOLinesView.Rows [e.RowIndex].IsNewRow && dtgrdPOLinesView.Columns[e.ColumnIndex].Name == "Vendor") 
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

        private void SetItemColor (int columnindex, int rowindex, Color color)
        {
            dtgrdPOLinesView["Class", rowindex].Style.BackColor = color;
            dtgrdPOLinesView["Vendor", rowindex].Style.BackColor = color;
            dtgrdPOLinesView["Style", rowindex].Style.BackColor = color;
            dtgrdPOLinesView["Color", rowindex].Style.BackColor = color;
            dtgrdPOLinesView["Size", rowindex].Style.BackColor = color;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //dtgrdPOLinesView[1, 1].Style.BackColor = _cellbackgroundcolor;

            dtgrdPOLinesView.Rows[0].Height = 50;

        }


        // HK : 1-12-2009 : Validate header fields
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
                    dtgrdPOLinesView.Enabled = true;
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
                    dtgrdPOLinesView.Enabled = true;
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

                dtgrdPOLinesView.Enabled = true;

                return true;

            }

            return false;
        }

        private void grpBxPOHeader_Enter(object sender, EventArgs e)
        {

        }

        private void dtgrdPOLinesView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // HK : 03-12-2009 : Fix Bug : 106
            if (dtgrdPOLinesView.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {
                if (_polinedetails.Isvalid == false)
                {
                    e.Cancel = true;
                }
            }
        }

        private void SetupClassObjects()
        {
            _porder = new PurchaseOrder(_dbparamref, _username, _paramenv);
            _porder.GetPOHeader(_spiceponumber);

            // Get the comments (both vendor and internal)
            _porder.GetPOComments(_spiceponumber, _porder.SpicePoVersion);

            // Get the PO Line Details
            _dtPoLines = _porder.GetPOItems(_spiceponumber, _porder.SpicePoVersion);

        }

        private void GetItemComponents(short spicepoversion)
        {
            
        }

        private void SetupSimpleDataBinding()
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

            txtDept.Text = _porder.Deptcode.ToString ();
            itembo.GetDepartment(_porder.Deptcode);
            lblDeptDesc.Text = itembo.DepartmentName;
            
            txtVendor.Text = _porder.Vendorcode.ToString();
            itembo.GetVendor(_porder.Vendorcode);
            lblVendorDesc.Text = itembo.VendorName;

            txtCurrency.Text = _porder.Currencycode;
            itembo.GetCurrency(_porder.Currencycode);
            
            // Currency rate of PO
            _currencyratepo = _porder.ExchangeRate;
            
            lblCurrencyDesc.Text = itembo.CurrencyName + " (" +_currencyratepo.ToString() + ")";

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

            dtpkrShipDate.Value         = _porder.ShippingDate;
            
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

            //if (_porder.Penvironment.Domain == "TDSNA")
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

            lblPONumber.Text = _porder.Spiceponumber;

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

            // HK : 19-01-2010 : Fix Bug 260
            chkNewLineSelection.Checked = _porder.IsPONewLine;

            _bDataBindingsInitalised = true;

            // Display the PO Items in the grid
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
            _currencyratepo = _porder.ExchangeRate;

            lblCurrencyDesc.Text = itembo.CurrencyName + " (" + _currencyratepo.ToString() + ")";

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

            //if (_porder.Penvironment.Domain == "TDSNA")
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

            lblPONumber.Text = _porder.Spiceponumber;

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

            // HK : 01-01-2010 : Re Display the Current version and previous version 
            // of the PO Items

            int iStartRow;
            int iTotalRowsInGrid;

            // Add the items to the datagrid and setup the collections list
            if (_porder.lstpoLineItemDetails.Count > 0)
            {
                iStartRow = iTotalRowsInGrid = dtgrdPOLinesView.Rows.Count;

                foreach (POItemDetails poitem in _porder.lstpoLineItemDetails)
                {

                    // Insert the row and populate the Pk values. 
                    // Also make sure the Check Box in the first column is un checked

                    iStartRow = dtgrdPOLinesView.Rows.Add(false, poitem.Classcode,
                                                                        poitem.Vendorcode,
                                                                        poitem.Stylecode,
                                                                        poitem.Colorcode,
                                                                        poitem.Itemsize);

                    // Lookup the item to populate the class with the rest of the details
                    if (poitem.Isvalid == true)
                    {
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Description"].Value = poitem.Itemlongdescription;
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Retail"].Value = poitem.Retailprice.ToString();
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Cost"].Value = poitem.Cost.ToString();
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Character"].Value = poitem.Characterdesc;
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Season"].Value = poitem.SeasonDesc;
                        dtgrdPOLinesView.Rows[iStartRow].Cells["CasePackType"].Value = poitem.Packdescription;
                        dtgrdPOLinesView.Rows[iStartRow].Cells["TicketType"].Value = poitem.Tickettype;
                        //This will determine if the qty and cost can be changed 
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Pack"].Value = poitem.APP1;

                        // Display the quantity and assign this quantity to the PO Line Item object
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Quantity"].Value = poitem.Itemquantity;

                        // ///////////////////////////////////////////////////////////////////////
                        // HK : 16-11-2009 : Display Converted cost and hide actual cost from database
                        // 
                        // ///////////////////////////////////////////////////////////////////////

                        // Cannot divide by zero
                        if (_currencyratepo != 0)
                        {
                            dtgrdPOLinesView.Rows[iStartRow].Cells["ConvertedCost"].Value = Decimal.Round((poitem.Cost * _currencyratemarket) / _currencyratepo, 2);
                        }
                        else
                        {
                            Debug.Print("Add Item : While calculating converted cost, it was found that the PO Currency Rate is 0");
                        }

                        if (poitem.APP1 == "Y")
                        {
                            //Make the UnitCost Readonly
                            dtgrdPOLinesView.Rows[iStartRow].Cells["Cost"].ReadOnly = true;

                        }
                        else
                        {
                            //Not an APP
                            //Enable cost
                            dtgrdPOLinesView.Rows[iStartRow].Cells["Cost"].ReadOnly = false;
                        }

                        // HK : 22-12-2009 : Make the entire row readonly
                        dtgrdPOLinesView.Rows[iStartRow].ReadOnly = true;

                    }

                }

            }

            /*
            Debug.Print("DataGrid Items after retrieve");
            Debug.Print("=========================================");
            DisplayDataGridItems();
            Debug.Print("=========================================");

            Debug.Print("Items collection after retrieve");
            Debug.Print("=========================================");
            DisplayItemCollection();
            Debug.Print("=========================================");
            */

            // Now calculate the PO Summary
            CalculatePOSummary();


        }

        private void DisplayPoItems()
        {
            // DataRow variables
            Int16 iClass;
            Int32 iVendor;
            Int16 iStyle;
            Int16 iColour;
            Int16 iSize;

            int iStartRow;
            int iTotalRowsInGrid;

            int     qty;
            short   sequence;

            // HK : Fix Bug 215 : 12-01-2010
            char padchar = Convert.ToChar(" ");
            Boolean bItemLookupError = false;

            DataTable dtComponents;

            // Add the items to the datagrid and setup the collections list
            if (_dtPoLines.Rows.Count > 0)
            {
                iStartRow = iTotalRowsInGrid = dtgrdPOLinesView.Rows.Count;

                foreach (DataRow dr in _dtPoLines.Rows)
                {

                    // Insert the row and populate the Pk values. 
                    // Also make sure the Check Box in the first column is un checked

                    iClass = (Int16)dr["Class"];
                    Debug.Print("Class :" + iClass.ToString());

                    iStartRow = dtgrdPOLinesView.Rows.Add(false, (Int16)dr["class"],
                                                                        dr["vendor"],
                                                                        dr["style"],
                                                                        dr["colour"],
                                                                        dr["size"]);

                    // Create a new item for the POItemDetails collection
                    POItemDetails poitem = new POItemDetails((short)dr["class"], (int)dr["vendor"],
                                                             (short)dr["style"], (short)dr["colour"],
                                                             (short)dr["size"],  iStartRow + 1);

                    // ////////////////////////////////////////////////////////////
                    // HK : 22-12-2009 : Some attributes of the PO item are populated 
                    // by the validation classes and not ItemLookup

                    // ///////////////////////////////////////////////////////////////
                    // ClassName
                    List<string> retValues = validationcls.ValidateClass(poitem.Classcode.ToString());
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

                    // ///////////////////////////////////////////////////////////////

                    // Lookup the item to populate the class with the rest of the details
                    if (poitem.ItemLookup(_porder.DbParamRef, _porder.UserName, _porder.Penvironment, _porder.DefaultMarket))
                    {

                        dtgrdPOLinesView.Rows[iStartRow].Cells["Description"].Value     = poitem.Itemlongdescription;
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Retail"].Value          = poitem.Retailprice.ToString();
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Cost"].Value            = poitem.Cost.ToString();
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Character"].Value       = poitem.Characterdesc;
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Season"].Value          = poitem.SeasonDesc;
                        dtgrdPOLinesView.Rows[iStartRow].Cells["CasePackType"].Value    = poitem.Packdescription;
                        dtgrdPOLinesView.Rows[iStartRow].Cells["TicketType"].Value      = poitem.Tickettype;
                        //This will determine if the qty and cost can be changed 
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Pack"].Value            = poitem.APP1;

                        // Display the quantity and assign this quantity to the PO Line Item object
                        qty = Convert.ToInt32 (_dtPoLines.Rows[iStartRow]["Quantity"]);
                        sequence = Convert.ToInt16 (_dtPoLines.Rows[iStartRow]["Sequence"]);
                        dtgrdPOLinesView.Rows[iStartRow].Cells["Quantity"].Value = qty;
                        poitem.Itemquantity = qty;
                        poitem.Sequence = sequence;

                        // For some strange reason 
                        //dtgrdPOLinesView.Rows[iStartRow].Cells["colSelect"].Value = false;

                        // ///////////////////////////////////////////////////////////////////////
                        // HK : 16-11-2009 : Display Converted cost and hide actual cost from database
                        // 
                        // ///////////////////////////////////////////////////////////////////////

                        // HK : Fix Bug : 276, 277, 283
                        // Grab landedCost as retrieved from database
                        // HK : 18-01-2010
                        poitem.LandedCost = Convert.ToDecimal (_dtPoLines.Rows[iStartRow]["LandedCost"]);
                        // Display Landed Cost in datagrid
                        dtgrdPOLinesView.Rows[iStartRow].Cells["LandedCost"].Value = poitem.LandedCost;

                        // Cannot divide by zero
                        if (_currencyratepo != 0)
                        {
                            // HK : Fix Bug : 276, 277, 283
                            // HK : 18-01-2010 : Display cost as retrieved from database. This will ne "ConvertedCost" as
                            // saved in the Create or last Modify of the PO.

                            // REMEMBER that the ItemLookup method will populate the "Cost" attribute with the 
                            // Simple vendor cost as set in the database.
                            poitem.ConvertedCost = Convert.ToDecimal(_dtPoLines.Rows[iStartRow]["VendorCost"]);
                            dtgrdPOLinesView.Rows[iStartRow].Cells["ConvertedCost"].Value = poitem.ConvertedCost;
                        }
                        else
                        {
                            Debug.Print("Add Item : While calculating converted cost, it was found that the PO Currency Rate is 0");
                        }

                        if (poitem.APP1 == "Y")
                        {
                            //Make the UnitCost Readonly
                            dtgrdPOLinesView.Rows[iStartRow].Cells["ConvertedCost"].ReadOnly = true;

                        }
                        else
                        {
                            //Not an APP
                            //Enable cost
                            dtgrdPOLinesView.Rows[iStartRow].Cells["ConvertedCost"].ReadOnly = false;
                        }

                        // ////////////////////////////////////////////////////////
                        // HK : 15-12-2009 : Get the components 
                        if (poitem.APP1 == "Y")
                        {
                            //Extract common params from ROW
                            SPICECommon spcommon = new SPICECommon(_porder.DbParamRef, _porder.UserName, _porder.Penvironment, _porder.DefaultMarket);

                            AssortedPrePack appbo = new AssortedPrePack(poitem, spcommon);

                            dtComponents = appbo.PopulateAPPComponents(_spiceponumber, _porder.SpicePoVersion, poitem.Sequence);

                            PopulateComponents(poitem, dtComponents);

                        }
                        // ////////////////////////////////////////////////////////

                        // HK : 15-12-2009 :  Add PO Item to Items list
                        _porder.lstpoLineItemDetails.Add(poitem);

                    }
                    else
                    {
                        // //////////////////////////////////
                        // HK : Fix Bug 215 : 12-01-2010
                        // //////////////////////////////////
                        
                        //_polinedetails.Isvalid = false;

                        // ?????? HK : Check before implementing
                        poitem.Isvalid = false;

                        // HK : 12-01-2010 : FC has managed to get an item into the PO Lines
                        // event though it does not exist in the database.
                        bItemLookupError = true;
                        
                        iClass      = poitem.Classcode;
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
                              + "", "Spice - Po Modification");

                    }

                }

            }

            if (bItemLookupError == false)
            {
                Debug.Print("DataGrid Items after retrieve");
                Debug.Print("=========================================");
                DisplayDataGridItems();
                Debug.Print("=========================================");

                Debug.Print("Items collection after retrieve");
                Debug.Print("=========================================");
                DisplayItemCollection();
                Debug.Print("=========================================");
            }

            // Now calculate the PO Summary
            CalculatePOSummary();
       
        }

        // /////////////////////////////////////////////////////////////////////////////
        // HK : 17-11-2009 : Populate the POComponents collection of POItemDetails
        // for each component in the PO Item
        // /////////////////////////////////////////////////////////////////////////////
        private void PopulateComponents(POItemDetails poitemdetails, DataTable dtInitialLoad)
        {

            for (int i = 0; i < dtInitialLoad.Rows.Count; i++)
            {

                POItemDetails pocomponent = new POItemDetails(i);
                pocomponent.ItemQtyChanged += new POItemDetails.delItemQtyChanged(pocomponent_ItemQtyChanged);

                pocomponent.Classcode = (short)dtInitialLoad.Rows[i]["ComponentClass"];
                pocomponent.Colorcode = (short)dtInitialLoad.Rows[i]["ComponentColour"];
                pocomponent.Stylecode = (short)dtInitialLoad.Rows[i]["ComponentStyle"];
                pocomponent.Vendorcode = (int)dtInitialLoad.Rows[i]["ComponentVendor"];
                pocomponent.Itemsize = (short)dtInitialLoad.Rows[i]["ComponentSize"];

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
                retValues = validationcls.ValidateSize(pocomponent.Itemsize.ToString());
                pocomponent.Sizename = retValues[1];

                // ///////////////////////////////////////////////////////////////

                //This function assumes the reqd fields have populated
                pocomponent.ItemLookup(_porder.DbParamRef, _porder.UserName, _porder.Penvironment, _porder.DefaultMarket);

                // Apply the ItemQuantity to the PO Component ( PO Item Details) business object 
                pocomponent.Itemquantity = (short)dtInitialLoad.Rows[i]["ComponentQuantity"];

                // HK : 18-01-2010 : 
                //pocomponent.Cost = (decimal)dtInitialLoad.Rows[i]["ComponentCost"];
                pocomponent.ConvertedCost = (decimal)dtInitialLoad.Rows[i]["ComponentCost"];
                pocomponent.LandedCost = Decimal.Round((pocomponent.Cost * _currencyratemarket) * _porder.Landing, 2);

                // Since we are loopin through the datatable in sequential order starting from 0
                // we just use the Add method instead of InsertAt
                poitemdetails.pocomponents.Add(pocomponent);

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

            // //////////////////////////////////////////////////////////////////////////////////////
            // HK : 12-01-2010 : Fix Bug 213. Need to do a TryParse to get values from the datagrid cells
            // into the variables.
            // //////////////////////////////////////////////////////////////////////////////////////
            /*
            iitemindex  = _porder.lstpoLineItemDetails[rowindex].Itemindex;
            iClass      = Convert.ToInt16(dtgrdPOLinesView.Rows[rowindex].Cells["Class"].Value);
            iVendor     = Convert.ToInt32(dtgrdPOLinesView.Rows[rowindex].Cells["Vendor"].Value);
            iStyle      = Convert.ToInt16(dtgrdPOLinesView.Rows[rowindex].Cells["Style"].Value);
            iColor      = Convert.ToInt16(dtgrdPOLinesView.Rows[rowindex].Cells["Color"].Value);
            iSize       = Convert.ToInt16(dtgrdPOLinesView.Rows[rowindex].Cells["Size"].Value);
            */
            // Try and convert Class, Vendor, Style, Color, Size
            iitemindex = rowindex;
            Int16.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[rowindex].Cells["Class"].Value), out iClass);
            Int32.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[rowindex].Cells["Vendor"].Value), out iVendor);
            Int16.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[rowindex].Cells["Style"].Value), out iStyle);
            Int16.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[rowindex].Cells["Color"].Value), out iColor);
            Int16.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[rowindex].Cells["Size"].Value), out iSize);
            description = Convert.ToString(dtgrdPOLinesView.Rows[rowindex].Cells["Description"].Value);

            if (dtgrdPOLinesView.Rows[rowindex].IsNewRow == false)
            {
                Debug.Print("Item Index: " + iitemindex.ToString().PadRight(6, padchar)
                              + " Class Code: " + iClass.ToString().PadRight(6, padchar)
                              + " Vendor Code: " + iVendor.ToString().PadRight(6, padchar)
                              + " Style Code: " + iStyle.ToString().PadRight(6, padchar)
                              + " Color Code: " + iColor.ToString().PadRight(6, padchar)
                              + " Size Code: " + iSize.ToString().PadRight(6, padchar)
                              + " Description: " + description.ToString()
                              + "");

            }

        }

        private void DisplayDataGridItems()
        {
            char padchar = Convert.ToChar(" ");
            int iitemindex;
            short iClass;
            int iVendor;
            short iStyle;
            short iColor;
            short iSize;
            String description;

            // //////////////////////////////////////////////////////////////////////////////////////
            // HK : 12-01-2010 : Fix Bug 213. Need to do a TryParse to get values from the datagrid cells
            // into the variables.
            // //////////////////////////////////////////////////////////////////////////////////////

            for (int i = 0; i < dtgrdPOLinesView.Rows.Count; i++)
            {
                if (dtgrdPOLinesView.Rows[i].IsNewRow == false)
                {
                    iitemindex = dtgrdPOLinesView.Rows[i].Index;
                    // Try and convert Class, Vendor, Style, Color, Size
                    Int16.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[i].Cells["Class"].Value), out iClass);
                    Int32.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[i].Cells["Vendor"].Value), out iVendor);
                    Int16.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[i].Cells["Style"].Value), out iStyle);
                    Int16.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[i].Cells["Color"].Value), out iColor);
                    Int16.TryParse(Convert.ToString(dtgrdPOLinesView.Rows[i].Cells["Size"].Value), out iSize);
                    description = Convert.ToString(dtgrdPOLinesView.Rows[i].Cells["Description"].Value);
                    /*
                    iClass = Convert.ToInt16(dtgrdPOLinesView.Rows[i].Cells["Class"].Value);
                    iVendor = Convert.ToInt32(dtgrdPOLinesView.Rows[i].Cells["Vendor"].Value);
                    iStyle = Convert.ToInt16(dtgrdPOLinesView.Rows[i].Cells["Style"].Value);
                    iColor = Convert.ToInt16(dtgrdPOLinesView.Rows[i].Cells["Color"].Value);
                    iSize = Convert.ToInt16(dtgrdPOLinesView.Rows[i].Cells["Size"].Value);
                    */

                    Debug.Print("Item Index: " + iitemindex.ToString().PadRight(6, padchar)
                              + " Class Code: " + iClass.ToString().PadRight(6, padchar)
                              + " Vendor Code: " + iVendor.ToString().PadRight(6, padchar)
                              + " Style Code: " + iStyle.ToString().PadRight(6, padchar)
                              + " Color Code: " + iColor.ToString().PadRight(6, padchar)
                              + " Size Code: " + iSize.ToString().PadRight(6, padchar)
                              + " Description: " + description.ToString()
                              + "");
                }
            }
        }

        private void DisplayLineItemDetails (POItemDetails item)
            {
                char padchar = Convert.ToChar(" ");

                Debug.Print("Item Index: "      + _polinedetails.Itemindex.ToString().PadRight(6, padchar)
                            + " Class Code: "   + _polinedetails.Classcode.ToString().PadRight(6, padchar)
                            + " Vendor Code: "  + _polinedetails.Vendorcode.ToString().PadRight(6, padchar)
                            + " Style Code: "   + _polinedetails.Stylecode.ToString().PadRight(6, padchar)
                            + " Color Code: "   + _polinedetails.Colorcode.ToString().PadRight(6, padchar)
                            + " Size Code: "    + _polinedetails.Itemsize.ToString().PadRight(6, padchar)
                            + " Description: "  + _polinedetails.Itemlongdescription
                            + "");
            }

        private void DisplayItemCollection(int collectionindex)
        {
            char padchar = Convert.ToChar(" ");

            if ( collectionindex < _porder.lstpoLineItemDetails.Count)
            {
                Debug.Print("Item Index: " + _porder.lstpoLineItemDetails[collectionindex].Itemindex.ToString().PadRight(6, padchar)
                            + " Class Code: " + _porder.lstpoLineItemDetails[collectionindex].Classcode.ToString().PadRight(6, padchar)
                            + " Vendor Code: " + _porder.lstpoLineItemDetails[collectionindex].Vendorcode.ToString().PadRight(6, padchar)
                            + " Style Code: " + _porder.lstpoLineItemDetails[collectionindex].Stylecode.ToString().PadRight(6, padchar)
                            + " Color Code: " + _porder.lstpoLineItemDetails[collectionindex].Colorcode.ToString().PadRight(6, padchar)
                            + " Size Code: " + _porder.lstpoLineItemDetails[collectionindex].Itemsize.ToString().PadRight(6, padchar)
                            + " Description: " + _porder.lstpoLineItemDetails[collectionindex].Itemlongdescription
                            + "");
            }
        }

        private void DisplayItemCollection()
        {
            char padchar = Convert.ToChar(" ");

            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                Debug.Print("Item Index: " + _porder.lstpoLineItemDetails[i].Itemindex.ToString().PadRight(6, padchar)
                            + " Class Code: " + _porder.lstpoLineItemDetails[i].Classcode.ToString().PadRight(6, padchar)
                            + " Vendor Code: " + _porder.lstpoLineItemDetails[i].Vendorcode.ToString().PadRight(6, padchar)
                            + " Style Code: " + _porder.lstpoLineItemDetails[i].Stylecode.ToString().PadRight(6, padchar)
                            + " Color Code: " + _porder.lstpoLineItemDetails[i].Colorcode.ToString().PadRight(6, padchar)
                            + " Size Code: " + _porder.lstpoLineItemDetails[i].Itemsize.ToString().PadRight(6, padchar)
                            + " Description: " + _porder.lstpoLineItemDetails[i].Itemlongdescription
                            + "");
            }


        }

        // //////////////////////////////////////////////
        // HK : CJ : 10-12-2009 : Get the sequence number 
        // for the new record

        // //////////////////////////////////////////////

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
                Debug.Print("Sequence Number on last item:" + sequencenumberonlastitem.ToString());

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
                Debug.Print("Selected Index: " + cbxFreight.SelectedIndex.ToString());
                Debug.Print("Selected Item: " + cbxFreight.Text);
                Debug.Print("Selected Item: " + cbxFreight.SelectedItem.ToString ());

                _porder.Freight = GetFreightId(cbxFreight.SelectedIndex);

            }
        }

        private void ReCalculateItemCost()
        {
            decimal cost;

            for (int i = 0; i < dtgrdPOLinesView.Rows.Count; i++)
            {
                // Cannot divide by zero
                if (_currencyratepo != 0 && dtgrdPOLinesView.Rows[i].IsNewRow == false)
                {
                    cost = Convert.ToDecimal(dtgrdPOLinesView.Rows[i].Cells["Cost"].Value);
                    dtgrdPOLinesView.Rows[i].Cells["ConvertedCost"].Value = Decimal.Round((cost * _currencyratemarket) / _currencyratepo, 2);

                }
                else
                {
                    Debug.Print("Add Item : While calculating converted cost, it was found that the PO Currency Rate is 0");
                }

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

        // HK : 26-01-2010 : Temp events to resolve a certain Bug caused due to sorting on column header
        // Delete this event handler
        private void dtgrdPOLinesView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            Debug.Print ("EVENT SortCompare fired .......");
        }

        // HK : 26-01-2010 : Temp events to resolve a certain Bug caused due to sorting on column header
        // Delete this event handler
        private void dtgrdPOLinesView_Sorted(object sender, EventArgs e)
        {
            Debug.Print("EVENT Sorted fired .......");
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

    }

}


