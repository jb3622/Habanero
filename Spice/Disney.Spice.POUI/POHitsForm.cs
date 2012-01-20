using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using Disney.Spice.POBO;

namespace Disney.Spice.POUI
{
    public partial class POHitsForm : Form
    {

        public delegate void OkButtonClickedEventHandler(object sender, PoHitsEventArgs e);
        public event OkButtonClickedEventHandler OnOkButtonClicked;

        public delegate void CancelButtonClickedEventHandler(object sender, PoHitsEventArgs e);
        public event CancelButtonClickedEventHandler OnCancelButtonClicked;

        //private DataTable               _dtPoHits;
        private PurchaseOrder           _porder;
        PurchaseOrder.PoHitsCollection  _pohitscollection;

        private Boolean                 _bUserWantsToDeleteLine = false;
        private int                     _itemquantityrounded = 0;

        private Boolean                 _bFormCancelClicked = false;
        private Boolean                 _bFormInitalised = false;

        private Boolean                 _bDataBindingsInitalised;

        // HK : 08-01-2010 : Keep original copy of the datatable. In case user clicks
        // 'Cancel' button, then discard the changes made to the Hits

        // Default constructor
        public POHitsForm()
        {
            InitializeComponent();
        }

        public POHitsForm (PurchaseOrder porder, PurchaseOrder.PoHitsCollection pohitscollection)
        {
            InitializeComponent();

            _bFormInitalised = true;

            //_dtPoHits = dtpohits;
            _porder = porder;
            _pohitscollection = pohitscollection;

            dtpkrAnticipateDate_2Hit.DataBindings.CollectionChanging += new CollectionChangeEventHandler(DataBindings_CollectionChanging);
            dtpkrAnticipateDate_2Hit.DataBindings.CollectionChanged += new CollectionChangeEventHandler(DataBindings_CollectionChanged);

            SetupHits();

        }

        void DataBindings_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            _bDataBindingsInitalised = false;
            //throw new Exception("The method or operation is not implemented.");
        }

        void DataBindings_CollectionChanging(object sender, CollectionChangeEventArgs e)
        {
            _bDataBindingsInitalised = true;
            //throw new Exception("The method or operation is not implemented.");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // ////////////////////////////////////////////////////////////////
            // HK : 16-11-2009
            // Prevent control validation when user clicks the Cancel button
            // ////////////////////////////////////////////////////////////////
            _bFormCancelClicked = true;

            // HK : 11-01-2010 : Any data entry (quantiy) done should be rolled
            // back if the user select the 'Cancel' button.

            for (int i = 0; i < _pohitscollection.Count; i++)
            {
                // If the Hits is active then revert the copy of data.
                // It makes no sense reverting the copy of data for 
                // Deactivated Hits because deactivating a non reversible action.
                if (_pohitscollection[i].HitActivated == true)
                {
                    _pohitscollection[i].RevertCopyOfPoHit();
                }
            }

            // Prepare to close and raise relavant events
            PoHitsEventArgs e1 = new PoHitsEventArgs(_pohitscollection);
            this.DialogResult = DialogResult.OK;
            RaiseCancelButtonClickedEvent(e1);
            Close();
        }

        // Pass the hit number to get the datagridview associated
        // with the hit
        private DataGridView GetDataGridViewForHit(int iHitNumber)
        {
            switch (iHitNumber)
            {
                case 2:
                    return dtgrdViewPOHits_2Hit;

                case 3:
                    return dtgrdViewPOHits_3Hit;

                case 4:
                    return dtgrdViewPOHits_4Hit;
                
                case 5:
                    return dtgrdViewPOHits_5Hit;

                case 6:
                    return dtgrdViewPOHits_6Hit;


                default :
                    return null;


            }
        }

        // Setup individual hit grids
        private void SetupGrid(int iHitNumber)
        {
            DataGridView dtgridview;

            dtgridview = GetDataGridViewForHit(iHitNumber);

            // Header Text
            dtgridview.Columns["colSelect"].HeaderText = "Select";
            dtgridview.Columns["Class"].HeaderText = "Class";
            dtgridview.Columns["Vendor"].HeaderText = "Vendor";
            dtgridview.Columns["Style"].HeaderText = "Style";
            dtgridview.Columns["Color"].HeaderText = "Color";
            dtgridview.Columns["Size"].HeaderText = "Size";
            dtgridview.Columns["Description"].HeaderText = "Description";
            dtgridview.Columns["Quantity"].HeaderText = "Quantity";

            // Width
            dtgridview.Columns["colSelect"].Width = 40;
            dtgridview.Columns["Class"].Width = 50;
            dtgridview.Columns["Vendor"].Width = 50;
            dtgridview.Columns["Style"].Width = 50;
            dtgridview.Columns["Color"].Width = 50;
            dtgridview.Columns["Size"].Width = 50;
            dtgridview.Columns["Description"].Width = 160;
            dtgridview.Columns["Quantity"].Width = 50;

            // Readonly
            dtgridview.Columns["Class"].ReadOnly = true;
            dtgridview.Columns["Vendor"].ReadOnly = true;
            dtgridview.Columns["Style"].ReadOnly = true;
            dtgridview.Columns["Color"].ReadOnly = true;
            dtgridview.Columns["Size"].ReadOnly = true;
            dtgridview.Columns["Description"].ReadOnly = true;

            // Visible 
            dtgridview.Columns["ItemIndex"].Visible = false;
            dtgridview.Columns["Pack"].Visible = false;

            // Column Alignment
            dtgridview.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        }

        private void InitalizeDataBingings(int iHitNumber)
        {
            // ///////////////////////////////////////////////////////////////
            // HK : 1-12-2009
            // DataBinding
            // ///////////////////////////////////////////////////////////////
            if (iHitNumber == 2)
            {
                txtVendorComment1_2Hit.DataBindings.Add("Text", _pohitscollection[0], "Vendorcomments1");
                txtVendorComment2_2Hit.DataBindings.Add("Text", _pohitscollection[0], "Vendorcomments2");
                txtVendorComment3_2Hit.DataBindings.Add("Text", _pohitscollection[0], "Vendorcomments3");

                txtInternalComments1_2Hit.DataBindings.Add("Text", _pohitscollection[0], "Internalcomments1");
                txtInternalComments2_2Hit.DataBindings.Add("Text", _pohitscollection[0], "Internalcomments2");

                // Manually copy the Anticipate date, Shipping date and Cancel date
                // as simple databinding with datatime picker cause problems
                dtpkrAnticipateDate_2Hit.Value  = _pohitscollection[0].AnticipateDate;
                dtpkrShipDate_2Hit.Value        = _pohitscollection[0].ShippingDate;
                txtCancelDate_2Hit.Text         = _pohitscollection[0].CancelDate;

                //dtpkrAnticipateDate_2Hit.DataBindings.Add( ("Value"),_pohitscollection[0], "AnticipateDate", false);
                //dtpkrShipDate_2Hit.DataBindings.Add("Value", _pohitscollection[0], "ShippingDate", false);
                //txtCancelDate_2Hit.DataBindings.Add("Text", _pohitscollection[0], "CancelDate");
                
            }

            if (iHitNumber == 3)
            {
                txtVendorComment1_3Hit.DataBindings.Add("Text", _pohitscollection[1], "Vendorcomments1");
                txtVendorComment2_3Hit.DataBindings.Add("Text", _pohitscollection[1], "Vendorcomments2");
                txtVendorComment3_3Hit.DataBindings.Add("Text", _pohitscollection[1], "Vendorcomments3");

                txtInternalComments1_3Hit.DataBindings.Add("Text", _pohitscollection[1], "Internalcomments1");
                txtInternalComments2_3Hit.DataBindings.Add("Text", _pohitscollection[1], "Internalcomments2");


                // Manually copy the Anticipate date, Shipping date and Cancel date
                // as simple databinding with datatime picker cause problems
                dtpkrAnticipateDate_3Hit.Value  = _pohitscollection[1].AnticipateDate;
                dtpkrShipDate_3Hit.Value        = _pohitscollection[1].ShippingDate;
                txtCancelDate_3Hit.Text         = _pohitscollection[1].CancelDate;

            }

            if (iHitNumber == 4)
            {
                txtVendorComment1_4Hit.DataBindings.Add("Text", _pohitscollection[2], "Vendorcomments1");
                txtVendorComment2_4Hit.DataBindings.Add("Text", _pohitscollection[2], "Vendorcomments2");
                txtVendorComment3_4Hit.DataBindings.Add("Text", _pohitscollection[2], "Vendorcomments3");

                txtInternalComments1_4Hit.DataBindings.Add("Text", _pohitscollection[2], "Internalcomments1");
                txtInternalComments2_4Hit.DataBindings.Add("Text", _pohitscollection[2], "Internalcomments2");

                // Manually copy the Anticipate date, Shipping date and Cancel date
                // as simple databinding with datatime picker cause problems
                dtpkrAnticipateDate_4Hit.Value  = _pohitscollection[2].AnticipateDate;
                dtpkrShipDate_4Hit.Value        = _pohitscollection[2].ShippingDate;
                txtCancelDate_4Hit.Text         = _pohitscollection[2].CancelDate;

            }

            if (iHitNumber == 5)
            {
                txtVendorComment1_5Hit.DataBindings.Add("Text", _pohitscollection[3], "Vendorcomments1");
                txtVendorComment2_5Hit.DataBindings.Add("Text", _pohitscollection[3], "Vendorcomments2");
                txtVendorComment3_5Hit.DataBindings.Add("Text", _pohitscollection[3], "Vendorcomments3");

                txtInternalComments1_5Hit.DataBindings.Add("Text", _pohitscollection[3], "Internalcomments1");
                txtInternalComments2_5Hit.DataBindings.Add("Text", _pohitscollection[3], "Internalcomments2");


                // Manually copy the Anticipate date, Shipping date and Cancel date
                // as simple databinding with datatime picker cause problems
                dtpkrAnticipateDate_5Hit.Value  = _pohitscollection[3].AnticipateDate;
                dtpkrShipDate_5Hit.Value        = _pohitscollection[3].ShippingDate;
                txtCancelDate_5Hit.Text         = _pohitscollection[3].CancelDate;

            }

            if (iHitNumber == 6)
            {
                txtVendorComment1_6Hit.DataBindings.Add("Text", _pohitscollection[4], "Vendorcomments1");
                txtVendorComment2_6Hit.DataBindings.Add("Text", _pohitscollection[4], "Vendorcomments2");
                txtVendorComment3_6Hit.DataBindings.Add("Text", _pohitscollection[4], "Vendorcomments3");

                txtInternalComments1_6Hit.DataBindings.Add("Text", _pohitscollection[4], "Internalcomments1");
                txtInternalComments2_6Hit.DataBindings.Add("Text", _pohitscollection[4], "Internalcomments2");


                // Manually copy the Anticipate date, Shipping date and Cancel date
                // as simple databinding with datatime picker cause problems
                dtpkrAnticipateDate_6Hit.Value  = _pohitscollection[4].AnticipateDate;
                dtpkrShipDate_6Hit.Value        = _pohitscollection[4].ShippingDate;
                txtCancelDate_6Hit.Text         = _pohitscollection[4].CancelDate;

            }

            _bDataBindingsInitalised = true;

        }

        private void SetupHits()
        {
            int itabpageindexforhit;
            DataGridView dtgridview;

            if (_pohitscollection.Count > 0)
            {
                foreach (PurchaseOrder.POHits item in _pohitscollection)
                {
                    // Hit = 2
                    if (item.HitInitalised == true) //item.HitNUmber == 2 && 
                    {
                        // Get the datagrid view associated with this grid
                        dtgridview = GetDataGridViewForHit(item.HitNUmber);

                        // ///////////////////////////////////////////////////////////////////
                        // HK : FC : 16-12-2009 : If the user modifies the items on the main 
                        // PO Entry |Form, we must reflect those changes on the Hit that is  
                        // active. We dont want any items on the main PO window, not to be on 
                        // the Hit list. Also a item on the Hit list that is not on the 
                        // main PO Entry form must be removed.

                        // HK : 14-01-2010 : Fix Bug 217 : Refresh Item only if Hit is 
                        // active
                        if (item.HitActivated == true)
                        {
                            RefreshItems(item.HitNUmber);
                        }

                        // ///////////////////////////////////////////////////////////////////
                        
                        // Assign the datasource. Item on the main PO
                        // will have already been copied into this datatable 
                        // when the form is first called and user clicks 'Activate'
                        dtgridview.DataSource = item.dtPoHits;

                        InitalizeDataBingings(item.HitNUmber);

                        SetupGrid(item.HitNUmber);

                        // Get the tab page index for the hit in question
                        itabpageindexforhit = GetTabPageIndexForHit(item.HitNUmber);

                        if (item.HitActivated == true)
                        {
                            EnableControls(itabpageindexforhit);
                       }
                        if (item.HitActivated == false)
                        {
                            DisableControls(itabpageindexforhit);
                        }
                    }
                }
            }
        }

        private void SetupHits(Boolean bRun)
        {
            // If Hits have been previously activated then them must have 
            // been added to the Po Hits Collection

            if (_pohitscollection.Count > 0)
            {
                foreach (PurchaseOrder.POHits item in _pohitscollection)
                {
                    // Hit = 2
                    if (item.HitNUmber == 2 && item.HitInitalised == true) //item.HitNUmber == 2 && 
                    {
                        // Copy all items on the PO into the Hits datagrid
                        dtgrdViewPOHits_2Hit.DataSource = item.dtPoHits;

                        InitalizeDataBingings(item.HitNUmber);

                        SetupGrid(item.HitNUmber);

                        if (item.HitActivated == true)
                        {
                            EnableControls(GetTabPageIndexForHit(item.HitNUmber));
                        }
                        if (item.HitActivated == false)
                        {
                            DisableControls(GetTabPageIndexForHit(item.HitNUmber));
                        }
                    }

                    // Hit = 3
                    if (item.HitNUmber == 3 && item.HitInitalised == true) //item.HitNUmber == 2 && 
                    {
                        // Copy all items on the PO into the Hits datagrid
                        dtgrdViewPOHits_3Hit.DataSource = item.dtPoHits;

                        InitalizeDataBingings(item.HitNUmber);

                        SetupGrid(item.HitNUmber);

                        if (item.HitActivated == true)
                        {
                            EnableControls(GetTabPageIndexForHit(item.HitNUmber));
                        }
                        if (item.HitActivated == false)
                        {
                            DisableControls(GetTabPageIndexForHit(item.HitNUmber));
                        }
                    }

                    // Hit = 4
                    if (item.HitNUmber == 4 && item.HitInitalised == true) //item.HitNUmber == 2 && 
                    {
                        // Copy all items on the PO into the Hits datagrid
                        dtgrdViewPOHits_4Hit.DataSource = item.dtPoHits;

                        InitalizeDataBingings(item.HitNUmber);

                        SetupGrid(item.HitNUmber);

                        if (item.HitActivated == true)
                        {
                            EnableControls(GetTabPageIndexForHit(item.HitNUmber));
                        }
                        if (item.HitActivated == false)
                        {
                            DisableControls(GetTabPageIndexForHit(item.HitNUmber));
                        }
                    }

                    // Hit = 5
                    if (item.HitNUmber == 5 && item.HitInitalised == true) //item.HitNUmber == 2 && 
                    {
                        // Copy all items on the PO into the Hits datagrid
                        dtgrdViewPOHits_5Hit.DataSource = item.dtPoHits;

                        InitalizeDataBingings(item.HitNUmber);

                        SetupGrid(item.HitNUmber);

                        if (item.HitActivated == true)
                        {
                            EnableControls(GetTabPageIndexForHit(item.HitNUmber));
                        }
                        if (item.HitActivated == false)
                        {
                            DisableControls(GetTabPageIndexForHit(item.HitNUmber));
                        }
                    }

                    // Hit = 6
                    if (item.HitNUmber == 6 && item.HitInitalised == true) //item.HitNUmber == 2 && 
                    {
                        // Copy all items on the PO into the Hits datagrid
                        dtgrdViewPOHits_6Hit.DataSource = item.dtPoHits;

                        InitalizeDataBingings(item.HitNUmber);

                        SetupGrid(item.HitNUmber);

                        if (item.HitActivated == true)
                        {
                            EnableControls(GetTabPageIndexForHit(item.HitNUmber));
                        }
                        if (item.HitActivated == false)
                        {
                            DisableControls(GetTabPageIndexForHit(item.HitNUmber));
                        }
                    }
                        
                }
            }
        }

        // HK : Gets the tab page index for specified hit number
        private int GetTabPageIndexForHit(int ihitnumber)
        {
            switch (ihitnumber)
            {
                case 2:
                    return 0;

                case 3:
                    return 1;

                case 4:
                    return 2;

                case 5:
                    return 3;

                case 6:
                    return 4;

                default:
                    return -1;

            }

        }

        private void chkBxActive2ndHit_CheckedChanged(object sender, EventArgs e)
        {
            Debug.Print("Checked Changed fired");
            Debug.Print("Checked state:" + chkBxActive_2Hit.Checked.ToString());

            int ihitscollectionindex;
            ihitscollectionindex = GetHitsCollectionIndexForSpecificHit(2);

            if (chkBxActive_2Hit.Checked)
            {
                tabCtlPOHits.SelectedTab.Text= tabCtlPOHits.TabPages[1].Text + " on " + dtpkrAnticipateDate_2Hit.Text;
                //EnableControls(tabCtlPOHits.SelectedTab);
                
                EnableControls(tabCtlPOHits.SelectedIndex);

                CreateHit(2);

            }

            // Deactivate the hit
            if (chkBxActive_2Hit.Checked == false)
            {
                _pohitscollection[ihitscollectionindex].HitActivated = false;

                DisableControls(tabCtlPOHits.SelectedIndex);
            }
        }

        private void DeactivateHit(int ihitnumber)
        {
            int ihitscollectionindex;
            ihitscollectionindex = GetHitsCollectionIndexForSpecificHit(ihitnumber);

            _pohitscollection[ihitscollectionindex].HitActivated = false;

            // HK : 15-01-2010 : Fix Bug 216 : 
            _pohitscollection[ihitscollectionindex].dtPoHits.Clear();

        }

        private void ReactivateHit(int ihitnumber)
        {
            int ihitscollectionindex;
            ihitscollectionindex = GetHitsCollectionIndexForSpecificHit(ihitnumber);

            _pohitscollection[ihitscollectionindex].HitActivated = true;

        }

        // /////////////////////////////////////////////////////
        // HK : 1-12-2009 : Initalize the new hit
        // /////////////////////////////////////////////////////
        private void CreateHit (int ihitnumber)
        {
            int ihitscollectionindex;
            ihitscollectionindex = GetHitsCollectionIndexForSpecificHit(ihitnumber);

            _pohitscollection[ihitscollectionindex].HitActivated = true;

            // ///////////////////////////////////////////////////////////////////
            // HK : FC : 16-12-2009 : If the user Re Activates the Hit after it was 
            // activated once originally then we must refresh the items on the Hit 
            // with the items on the main PO Entry form.
            if (_pohitscollection[ihitscollectionindex].HitInitalised == true)
            {
                RefreshItems(ihitnumber);

                // Set the datasource for the grid
                switch (ihitnumber)
                {
                    case 2:
                        dtgrdViewPOHits_2Hit.DataSource = _pohitscollection[ihitscollectionindex].dtPoHits;
                        break;

                    case 3:
                        dtgrdViewPOHits_3Hit.DataSource = _pohitscollection[ihitscollectionindex].dtPoHits;
                        break;
                    case 4:
                        dtgrdViewPOHits_4Hit.DataSource = _pohitscollection[ihitscollectionindex].dtPoHits;
                        break;
                    case 5:
                        dtgrdViewPOHits_5Hit.DataSource = _pohitscollection[ihitscollectionindex].dtPoHits;
                        break;
                    case 6:
                        dtgrdViewPOHits_6Hit.DataSource = _pohitscollection[ihitscollectionindex].dtPoHits;
                        break;
                    default:
                        break;

                }

                // HK : 22-12-2009 : No need to call InitaliseDataBindiings ();

            }


            // ///////////////////////////////////////////////////////////////////

            if (_pohitscollection[ihitscollectionindex].HitInitalised == false)
            {

                for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
                {
                    _pohitscollection[ihitscollectionindex].dtPoHits.Rows.Add(false,
                                                            _porder.lstpoLineItemDetails[i].Itemindex,
                                                            _porder.lstpoLineItemDetails[i].APP1,
                                                            _porder.lstpoLineItemDetails[i].Classcode,
                                                            _porder.lstpoLineItemDetails[i].Vendorcode,
                                                            _porder.lstpoLineItemDetails[i].Stylecode,
                                                            _porder.lstpoLineItemDetails[i].Colorcode,
                                                            _porder.lstpoLineItemDetails[i].Itemsize,
                                                            _porder.lstpoLineItemDetails[i].Itemlongdescription,
                                                            0);

                }

                _pohitscollection[ihitscollectionindex].HitInitalised = true;

                // HK : 16-01-2010 : Fix Bug 219
                // Make a copy now
                _pohitscollection[ihitscollectionindex].CreateCopyOfPoHit();

                // Set the datasource for the grid
                // dtgrdViewPOHits_2Hit.DataSource = _pohitscollection[ihitscollectionindex].dtPoHits;
                // SetupDataSourceForGrid(ihitnumber);
                switch (ihitnumber)
                {
                    case 2:
                        dtgrdViewPOHits_2Hit.DataSource = _pohitscollection[ihitscollectionindex].dtPoHits;
                        break;

                    case 3:
                        dtgrdViewPOHits_3Hit.DataSource = _pohitscollection[ihitscollectionindex].dtPoHits;
                        break;
                    case 4:
                        dtgrdViewPOHits_4Hit.DataSource = _pohitscollection[ihitscollectionindex].dtPoHits;
                        break;
                    case 5:
                        dtgrdViewPOHits_5Hit.DataSource = _pohitscollection[ihitscollectionindex].dtPoHits;
                        break;
                    case 6:
                        dtgrdViewPOHits_6Hit.DataSource = _pohitscollection[ihitscollectionindex].dtPoHits;
                        break;
                    default:
                        break;

                }

                InitalizeDataBingings(ihitnumber);

            }

            SetupGrid(ihitnumber);
            
        }

        private void SetupDataSourceForGrid(int ihitnumber)
        {
            switch (ihitnumber)
            {
                case 2:
                    //dtgrdViewPOHits_2Hit.DataSource = _pohitscollection[ihitscollectionindex].dtPoHits;
                    break;
                case 3:
                    //return dtgrdViewPOHits_3Hit;
                    break;
                case 4:
                    //return dtgrdViewPOHits_4Hit;
                    break;
                case 5:
                    //return dtgrdViewPOHits_5Hit;
                    break;
                case 6:
                    //return dtgrdViewPOHits_6Hit;
                    break;

                default:
                    //return null;
                    break;

            }

        }

        private void AddPoItemsToDataGrid(int ihitnumber)
        {


        }

        private void DisableControls(int iSelectedTab)
        {
            ////////////////////////////////////////
            // HK : 30-11-2009
            // Do not loop through the collection on 
            // the tab page. Instead harcode the control
            // to be enabled / disabled


            // ///////////////////////////////////////
            // Tab Page 1 (2nd Hit)
            // ///////////////////////////////////////
            if (iSelectedTab == 0)
            {
                //Dates
                dtpkrAnticipateDate_2Hit.Enabled = false;
                dtpkrShipDate_2Hit.Enabled = false;

                // Gridview
                dtgrdViewPOHits_2Hit.Enabled = false;
                //dtgrdViewPOHits_2Hit.BackgroundColor = System.Drawing.SystemColors.Control;

                // Group box
                grpBxComments.Enabled = false;

                // Vendor Comments
                txtVendorComment1_2Hit.Enabled = false;
                txtVendorComment2_2Hit.Enabled = false;
                txtVendorComment3_2Hit.Enabled = false;

                // Internal Comments
                txtInternalComments1_2Hit.Enabled = false;
                txtInternalComments2_2Hit.Enabled = false;

                // Buttons
                btnReset_2Hit.Enabled = false;
                btnDelete_2Hit.Enabled = false;

                // Check the checkbox
                //chkBxActive_2Hit.Checked = false;

                btnActivate_Hit2.Enabled = true;
                btnActivate_Hit2.Visible = true;

                btnDeactivate_Hit2.Enabled = false;
                btnDeactivate_Hit2.Visible = false;

                tabCtlPOHits.TabPages[0].Text = GetOriginalTabPageText(iSelectedTab);

            }

            if (iSelectedTab == 1)
            {
                dtpkrAnticipateDate_3Hit.Enabled = false;
                dtpkrShipDate_3Hit.Enabled = false;

                dtgrdViewPOHits_3Hit.Enabled = false;

                grpBxComments_3Hit.Enabled = false;

                txtVendorComment1_3Hit.Enabled = false;
                txtVendorComment2_3Hit.Enabled = false;
                txtVendorComment3_3Hit.Enabled = false;

                txtInternalComments1_3Hit.Enabled = false;
                txtInternalComments2_3Hit.Enabled = false;

                btnReset_3Hit.Enabled = false;
                btnDelete_3Hit.Enabled = false;

                btnActivate_Hit3.Enabled = true;
                btnActivate_Hit3.Visible = true;

                btnDeactivate_Hit3.Enabled = false;
                btnDeactivate_Hit3.Visible = false;

                tabCtlPOHits.TabPages[1].Text = GetOriginalTabPageText(iSelectedTab);

            }

            if (iSelectedTab == 2)
            {
                dtpkrAnticipateDate_4Hit.Enabled = false;
                dtpkrShipDate_4Hit.Enabled = false;

                dtgrdViewPOHits_4Hit.Enabled = false;

                grpBxComments_4Hit.Enabled = false;

                txtVendorComment1_4Hit.Enabled = false;
                txtVendorComment2_4Hit.Enabled = false;
                txtVendorComment3_4Hit.Enabled = false;

                txtInternalComments1_4Hit.Enabled = false;
                txtInternalComments2_4Hit.Enabled = false;

                btnReset_4Hit.Enabled = false;
                btnDelete_4Hit.Enabled = false;

                btnActivate_Hit4.Enabled = true;
                btnActivate_Hit4.Visible = true;

                btnDeactivate_Hit4.Enabled = false;
                btnDeactivate_Hit4.Visible = false;

                tabCtlPOHits.TabPages[2].Text = GetOriginalTabPageText(iSelectedTab);

            }

            if (iSelectedTab == 3)
            {
                dtpkrAnticipateDate_5Hit.Enabled = false;
                dtpkrShipDate_5Hit.Enabled = false;

                dtgrdViewPOHits_5Hit.Enabled = false;

                grpBxComments_5Hit.Enabled = false;

                txtVendorComment1_5Hit.Enabled = false;
                txtVendorComment2_5Hit.Enabled = false;
                txtVendorComment3_5Hit.Enabled = false;

                txtInternalComments1_5Hit.Enabled = false;
                txtInternalComments2_5Hit.Enabled = false;

                btnReset_5Hit.Enabled = false;
                btnDelete_5Hit.Enabled = false;

                btnActivate_Hit5.Enabled = true;
                btnActivate_Hit5.Visible = true;

                btnDeactivate_Hit5.Enabled = false;
                btnDeactivate_Hit5.Visible = false;

                tabCtlPOHits.TabPages[3].Text = GetOriginalTabPageText(iSelectedTab);

            }

            if (iSelectedTab == 4)
            {
                dtpkrAnticipateDate_6Hit.Enabled = false;
                dtpkrShipDate_6Hit.Enabled = false;

                dtgrdViewPOHits_6Hit.Enabled = false;

                grpBxComments_6Hit.Enabled = false;

                txtVendorComment1_6Hit.Enabled = false;
                txtVendorComment2_6Hit.Enabled = false;
                txtVendorComment3_6Hit.Enabled = false;

                txtInternalComments1_6Hit.Enabled = false;
                txtInternalComments2_6Hit.Enabled = false;

                btnReset_6Hit.Enabled = false;
                btnDelete_6Hit.Enabled = false;

                btnActivate_Hit6.Enabled = true;
                btnActivate_Hit6.Visible = true;

                btnDeactivate_Hit6.Enabled = false;
                btnDeactivate_Hit6.Visible = false;

                tabCtlPOHits.TabPages[4].Text = GetOriginalTabPageText(iSelectedTab);
            }
        }

        // HK : 1-12--2009 : Should be EnableDisableControls(Boolean bEnable, int iSelectedTab)
        private void EnableControls(int iSelectedTab)
        {
            ////////////////////////////////////////
            // HK : 30-11-2009
            // Do not loop through the collection on 
            // the tab page. Instead harcode the control
            // to be enabled / disabled

            // ///////////////////////////////////////
            // Tab Page 0 (2nd Hit)
            // ///////////////////////////////////////
            if (iSelectedTab == 0)
            {
                //Dates
                dtpkrAnticipateDate_2Hit.Enabled = true;
                dtpkrShipDate_2Hit.Enabled = true;

                // Gridview
                dtgrdViewPOHits_2Hit.Enabled = true;
                //dtgrdViewPOHits_2Hit.BackgroundColor = DefaultBackColor;

                // Group box
                grpBxComments.Enabled = true;

                // Vendor Comments
                txtVendorComment1_2Hit.Enabled = true;
                txtVendorComment2_2Hit.Enabled = true;
                txtVendorComment3_2Hit.Enabled = true;

                // Internal Comments
                txtInternalComments1_2Hit.Enabled = true;
                txtInternalComments2_2Hit.Enabled = true;

                btnReset_2Hit.Enabled = true;
                btnDelete_2Hit.Enabled = true;

                btnActivate_Hit2.Enabled = !_pohitscollection[0].HitActivated;
                btnActivate_Hit2.Visible = !_pohitscollection[0].HitActivated;
                
                btnDeactivate_Hit2.Enabled = _pohitscollection[0].HitActivated;
                btnDeactivate_Hit2.Visible = _pohitscollection[0].HitActivated;

                // Show the correct anticipate date in the tab header
                tabCtlPOHits.TabPages[0].Text = GetOriginalTabPageText(iSelectedTab) + " on " +
                                                dtpkrAnticipateDate_2Hit.Value.ToLongDateString () ;

            }
            
            // ///////////////////////////////////////
            // Tab Page 1 (3nd Hit)
            // ///////////////////////////////////////
            if (iSelectedTab == 1)
            {
                dtpkrAnticipateDate_3Hit.Enabled = true;
                dtpkrShipDate_3Hit.Enabled = true;

                dtgrdViewPOHits_3Hit.Enabled = true;

                grpBxComments_3Hit.Enabled = true;

                txtVendorComment1_3Hit.Enabled = true;
                txtVendorComment2_3Hit.Enabled = true;
                txtVendorComment3_3Hit.Enabled = true;

                txtInternalComments1_3Hit.Enabled = true;
                txtInternalComments2_3Hit.Enabled = true;

                btnReset_3Hit.Enabled = true;
                btnDelete_3Hit.Enabled = true;

                btnActivate_Hit3.Enabled = !_pohitscollection[1].HitActivated;
                btnActivate_Hit3.Visible = !_pohitscollection[1].HitActivated;

                btnDeactivate_Hit3.Enabled = _pohitscollection[1].HitActivated;
                btnDeactivate_Hit3.Visible = _pohitscollection[1].HitActivated;

                // Show the correct anticipate date in the tab header
                tabCtlPOHits.TabPages[1].Text = GetOriginalTabPageText(iSelectedTab) + " on " +
                                                dtpkrAnticipateDate_3Hit.Value.ToLongDateString();

            }

            // ///////////////////////////////////////
            // Tab Page 2 (4th Hit)
            // ///////////////////////////////////////
            if (iSelectedTab == 2)
            {
                dtpkrAnticipateDate_4Hit.Enabled = true;
                dtpkrShipDate_4Hit.Enabled = true;

                dtgrdViewPOHits_4Hit.Enabled = true;

                grpBxComments_4Hit.Enabled = true;

                txtVendorComment1_4Hit.Enabled = true;
                txtVendorComment2_4Hit.Enabled = true;
                txtVendorComment3_4Hit.Enabled = true;

                txtInternalComments1_4Hit.Enabled = true;
                txtInternalComments2_4Hit.Enabled = true;

                btnReset_4Hit.Enabled = true;
                btnDelete_4Hit.Enabled = true;

                btnActivate_Hit4.Enabled = !_pohitscollection[2].HitActivated;
                btnActivate_Hit4.Visible = !_pohitscollection[2].HitActivated;

                btnDeactivate_Hit4.Enabled = _pohitscollection[2].HitActivated;
                btnDeactivate_Hit4.Visible = _pohitscollection[2].HitActivated;

                // Show the correct anticipate date in the tab header
                tabCtlPOHits.TabPages[2].Text = GetOriginalTabPageText(iSelectedTab) + " on " +
                                                dtpkrAnticipateDate_4Hit.Value.ToLongDateString();

            }

            // ///////////////////////////////////////
            // Tab Page 3 (5th Hit)
            // ///////////////////////////////////////
            if (iSelectedTab == 3)
            {
                dtpkrAnticipateDate_5Hit.Enabled = true;
                dtpkrShipDate_5Hit.Enabled = true;

                dtgrdViewPOHits_5Hit.Enabled = true;

                grpBxComments_5Hit.Enabled = true;

                txtVendorComment1_5Hit.Enabled = true;
                txtVendorComment2_5Hit.Enabled = true;
                txtVendorComment3_5Hit.Enabled = true;

                txtInternalComments1_5Hit.Enabled = true;
                txtInternalComments2_5Hit.Enabled = true;

                btnReset_5Hit.Enabled = true;
                btnDelete_5Hit.Enabled = true;

                btnActivate_Hit5.Enabled = !_pohitscollection[3].HitActivated;
                btnActivate_Hit5.Visible = !_pohitscollection[3].HitActivated;

                btnDeactivate_Hit5.Enabled = _pohitscollection[3].HitActivated;
                btnDeactivate_Hit5.Visible = _pohitscollection[3].HitActivated;

                // Show the correct anticipate date in the tab header
                tabCtlPOHits.TabPages[3].Text = GetOriginalTabPageText(iSelectedTab) + " on " +
                                                dtpkrAnticipateDate_5Hit.Value.ToLongDateString();

            }

            // ///////////////////////////////////////
            // Tab Page 4 (6th Hit)
            // ///////////////////////////////////////
            if (iSelectedTab == 4)
            {
                dtpkrAnticipateDate_6Hit.Enabled = true;
                dtpkrShipDate_6Hit.Enabled = true;

                dtgrdViewPOHits_6Hit.Enabled = true;

                grpBxComments_6Hit.Enabled = true;

                txtVendorComment1_6Hit.Enabled = true;
                txtVendorComment2_6Hit.Enabled = true;
                txtVendorComment3_6Hit.Enabled = true;

                txtInternalComments1_6Hit.Enabled = true;
                txtInternalComments2_6Hit.Enabled = true;

                btnReset_6Hit.Enabled = true;
                btnDelete_6Hit.Enabled = true;

                btnActivate_Hit6.Enabled = !_pohitscollection[4].HitActivated;
                btnActivate_Hit6.Visible = !_pohitscollection[4].HitActivated;

                btnDeactivate_Hit6.Enabled = _pohitscollection[4].HitActivated;
                btnDeactivate_Hit6.Visible = _pohitscollection[4].HitActivated;

                // Show the correct anticipate date in the tab header
                tabCtlPOHits.TabPages[4].Text = GetOriginalTabPageText(iSelectedTab) + " on " +
                                                dtpkrAnticipateDate_6Hit.Value.ToLongDateString();

            }

            ////////////////////////////////////////
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Prepare to close and raise relavant events
            PoHitsEventArgs e1 = new PoHitsEventArgs(_pohitscollection);
            this.DialogResult = DialogResult.OK;
            RaiseCancelButtonClickedEvent(e1);
            Close();
        }

        private void POHitsForm_Load(object sender, EventArgs e)
        {

        }

        private void RaiseOkButtonClickedEvent(PoHitsEventArgs e)
        {

            if (OnOkButtonClicked != null)
            {
                OnOkButtonClicked(this, e);
            }
        }

        private void RaiseCancelButtonClickedEvent(PoHitsEventArgs e)
        {
            if (OnCancelButtonClicked != null)
            {
                OnCancelButtonClicked(this, e);
            }
        }

        public class PoHitsEventArgs : EventArgs
        {

            private DataTable _dtpohits = null;
            private PurchaseOrder.PoHitsCollection _pohitscollection;

            public PurchaseOrder.PoHitsCollection poHitsCollection
            {
                get
                {
                    return _pohitscollection;
                }

                set
                {
                    _pohitscollection = value;
                }
            }


            public DataTable dtpohits
            {
                get
                {
                    return _dtpohits;
                }

                set
                {
                    _dtpohits  = value;
                }

            }

            public PoHitsEventArgs(PurchaseOrder.PoHitsCollection pohitscollection)
            {
                _pohitscollection = pohitscollection;
            }

        }

        private void EnableControls(TabControl.TabPageCollection tcol)
        {
            // ///////////////////////////////////////
            // HK : 30-11-2009
            //Loop thru the controls to enable state
            // May have to enable selected controls
            foreach (Control control in tcol)
            {
                control.Enabled = true;
            }

        }

        private void EnableControls(TabPage tabpage)
        {
            // ///////////////////////////////////////
            // HK : 30-11-2009
            //Loop thru the controls to enable state
            // May have to enable selected controls

            //TabControl.TabPageCollection tcol;

            //tcol = tabpage.Controls;

            foreach (Control control in tabpage.Controls)
            {
                Debug.Print("Control Name:)" + control.Name);
                control.Enabled = true;
            }

        }

        private void DeleteDataGridRow (int iHitNumber)
        {
            // HK : 27-11-2009 : To Do : Check that the row being tested in not a new row.
            // i.e skip any rows that have a status of IsNewRow = true
            // TODO lok at this
            int iRowsDeleted = 0;
            int iRunningCountTotalRows = 0;
            int iLoopCounter;
            DataGridView dtgridview;

            dtgridview = GetDataGridViewForHit(iHitNumber);

            //int iitemindex;
            //short iClass;
            //int iVendor;
            //short iStyle;
            //short iColor;
            //short iSize;

            // Init the looping counter
            iLoopCounter = 0;
            iRunningCountTotalRows = dtgridview.Rows.Count;

            // HK : 17-12-2009 
            // HK : No need to delete rows if there are no rows in the datagrid.
            // The condition for the execution of the do { } while () is at the 
            // end of thw do while. So the do while will execute at least once 
            // before testing the continuity condition.
            if (iRunningCountTotalRows == 0)
            {
                return;
            }

            do
            {
                if (!dtgridview.Rows[iLoopCounter].IsNewRow)
                {
                    // HK : 11-12-2009 : Item Index is not on the datagrid. So we must use the 
                    // item index on the PO Items Collection
                    //iitemindex = _porder.lstpoLineItemDetails[iLoopCounter].Itemindex;
                    //iClass = Convert.ToInt16(dtgridview.Rows[iLoopCounter].Cells["Class"].Value);
                    //iVendor = Convert.ToInt32(dtgridview.Rows[iLoopCounter].Cells["Vendor"].Value);
                    //iStyle = Convert.ToInt16(dtgridview.Rows[iLoopCounter].Cells["Style"].Value);
                    //iColor = Convert.ToInt16(dtgridview.Rows[iLoopCounter].Cells["Color"].Value);
                    //iSize = Convert.ToInt16(dtgridview.Rows[iLoopCounter].Cells["Size"].Value);

                    if (dtgridview.Rows[iLoopCounter].Cells[0].Value != null &&
                                Convert.ToBoolean(dtgridview.Rows[iLoopCounter].Cells[0].Value) == true)
                    {

                        Debug.Print("Data Grid row removed at index: " + iLoopCounter.ToString());
                        //DisplayDataGridItems(iLoopCounter);
            
                        dtgridview.Rows.RemoveAt(iLoopCounter);

                        iRowsDeleted++;

                        iRunningCountTotalRows = dtgridview.Rows.Count;

                    }
                    else
                    {

                        iLoopCounter++;
                    }
                }
                else
                {
                    iLoopCounter++;
                }

            } while (iLoopCounter < iRunningCountTotalRows);
        }

        private void btnDelete_2Hit_Click(object sender, EventArgs e)
        {

            // Stop datadrid validation from occuring
            _bUserWantsToDeleteLine = true;

            // Item on Hit 2 deleted
            DeleteDataGridRow(2);

            // Enable datadrid validation again
            _bUserWantsToDeleteLine = false;

        }

        private void btnReset_2Hit_Click(object sender, EventArgs e)
        {
            // Clear the Items on Hit 2
            _pohitscollection[0].dtPoHits.Clear();

            // Add again
            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                _pohitscollection[0].dtPoHits.Rows.Add(false,
                                            _porder.lstpoLineItemDetails[i].Itemindex,
                                            _porder.lstpoLineItemDetails[i].APP1,
                                            _porder.lstpoLineItemDetails[i].Classcode,
                                            _porder.lstpoLineItemDetails[i].Vendorcode,
                                            _porder.lstpoLineItemDetails[i].Stylecode,
                                            _porder.lstpoLineItemDetails[i].Colorcode,
                                            _porder.lstpoLineItemDetails[i].Itemsize,
                                            _porder.lstpoLineItemDetails[i].Itemlongdescription,
                                            0);

            }

        }

        private void dtpkrShipDate_2Hit_ValueChanged(object sender, EventArgs e)
        {
            
            if (_bDataBindingsInitalised)
            {
                //_porder.ShippingDate = dtpkrShipDate.Value;
                // Assign the value of the Anticipate date to our hits collection
                _pohitscollection[0].ShippingDate = dtpkrShipDate_2Hit.Value;
                
                if (_porder.Penvironment.Domain == "TDSNA")
                //For TDSNA
                {
                    txtCancelDate_2Hit.Text = dtpkrShipDate_2Hit.Value.AddDays(7).ToString("D");
                    //_porder.CancelDate = dtpkrShipDate.Value.AddDays(7);
                    _pohitscollection[0].CancelDate = dtpkrShipDate_2Hit.Value.AddDays(7).ToString("D");
                }

                //For TDSE
                // HK : 19-11-2009 : Format date as [LondDate]
                txtCancelDate_2Hit.Text = dtpkrShipDate_2Hit.Value.ToLongDateString();
                _pohitscollection[0].CancelDate = dtpkrShipDate_2Hit.Value.ToLongDateString(); ;
                //_porder.CancelDate = dtpkrShipDate.Value;

                //if (DateTime.Compare (dtpkrAnticipateDate.Value, dtpkrShipDate.Value) > 0)
                Debug.Print("Anticipate Date:" + dtpkrAnticipateDate_2Hit.Value.Date.ToString());
                Debug.Print("Ship Date :" + dtpkrShipDate_2Hit.Value.Date.ToString());

                if ((dtpkrAnticipateDate_2Hit.Value.Date < dtpkrShipDate_2Hit.Value.Date))
                {
                    //errPOEntry.SetError(dtpkrShipDate, "Anticipate date less than ship date");
                    errPOHits.SetError(dtpkrShipDate_2Hit, "Shipping date cannot be after the anticipate date");
                }
                else
                {
                    errPOHits.SetError(dtpkrShipDate_2Hit, "");

                }
            }
            
        }

        private void dtpkrAnticipateDate_2Hit_Validating(object sender, CancelEventArgs e)
        {
            if (_bFormInitalised)
            {
                if (dtpkrAnticipateDate_2Hit.Value < DateTime.Today)
                {
                    // validationcls.HighlightErrControls(lblAnticipateDate, dtpkrAnticipateDate, false);
                    errPOHits.SetError(dtpkrAnticipateDate_2Hit, "Please enter a date greater than Today");
                    //_porder.AnticipateDate = dtpkrAnticipateDate.Value;
                    e.Cancel = true;
                }
                else
                {
                    // validationcls.HighlightErrControls(lblAnticipateDate, dtpkrAnticipateDate, true);
                    errPOHits.SetError(dtpkrAnticipateDate_2Hit, "");
                    //_porder.AnticipateDate = DateTime.Now;
                    e.Cancel = false;
                }
            }
        }

        private void dtpkrAnticipateDate_2Hit_ValueChanged(object sender, EventArgs e)
        {

            Debug.Print("Anticipate Date:" + dtpkrAnticipateDate_2Hit.Value.Date.ToString());
            Debug.Print("Ship Date :" + dtpkrAnticipateDate_2Hit.Value.Date.ToString());
            
            if (_bDataBindingsInitalised)
            {

                // Assign the value of the Anticipate date to our hits collection
                // If the validation fails, still the value of this control
                // will be the value selected by the user although not quite correct
                // according to the business rules. However for the control it is 
                // a valid date.
                _pohitscollection[0].AnticipateDate = dtpkrAnticipateDate_2Hit.Value;
                //tabCtlPOHits.SelectedTab.Text = GetOriginalTabPageText(tabCtlPOHits.SelectedIndex) + " on " + 
                //                                        dtpkrAnticipateDate_2Hit.Value.ToLongDateString ();
                // HK : 07-12-2009: Never use selected index ever with multi tabs
                //tabCtlPOHits.TabPages[0].Text = GetOriginalTabPageText(tabCtlPOHits.SelectedIndex) + " on " +
                //                                        dtpkrAnticipateDate_3Hit.Value.ToLongDateString();
                tabCtlPOHits.TabPages[0].Text = GetOriginalTabPageText(0) + " on " +
                                                        dtpkrAnticipateDate_2Hit.Value.ToLongDateString();

                if ((dtpkrAnticipateDate_2Hit.Value.Date < dtpkrShipDate_2Hit.Value.Date))
                {
                    errPOHits.SetError(dtpkrAnticipateDate_2Hit, "Anticipate date cannot be before the ship date");
                }
                else
                {
                    errPOHits.SetError(dtpkrAnticipateDate_2Hit, "");
                }
            }
           
        }

        private void dtpkrShipDate_2Hit_Validating(object sender, CancelEventArgs e)
        {
            if (_bDataBindingsInitalised)
            {
                if (dtpkrShipDate_2Hit.Value < DateTime.Today)
                {
                    //  validationcls.HighlightErrControls(lblShipDate, dtpkrShipDate, false);
                    errPOHits.SetError(dtpkrShipDate_2Hit, "Please enter a date greater than  Today");
                    //_porder.ShippingDate = dtpkrShipDate.Value;
                    e.Cancel = true;
                }
                else
                {
                    //  validationcls.HighlightErrControls(lblShipDate, dtpkrShipDate, true);
                    errPOHits.SetError(dtpkrShipDate_2Hit, "");
                    //_porder.ShippingDate = DateTime.Now;
                    e.Cancel = false;
                }
            }
        }
               
        private bool ValidateQuantity(string svalue, int packQty)
        {
            bool bisValid;
            int itemqtyinput;

            if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput) && itemqtyinput >= packQty)
            {
                if (itemqtyinput % packQty != 0)
                {
                    ItemQuantityForm itemqtyform = new ItemQuantityForm(itemqtyinput, packQty);

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

        // ///////////////////////////////////////////////////////////////////
        // HK : 2-12-2009 : Loops through the main items list collection (_porder.lstpoLineItemDetails)
        // and gets the CasePackQty for that item if it is found
        // To Do :Must be a Find implementation of the collection rather than looping 
        // in the form
        // ///////////////////////////////////////////////////////////////////
        private int FindHitItemCasePackQty (int iitemindex, short iClass, int iVendor, 
                                            short iStyle, short iColor, short iSize, 
                                            ref short iCasepackqty)
        {

            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                // HK : 28-11-2009 : Add ItemIndex
                if ((_porder.lstpoLineItemDetails[i].Itemindex == iitemindex) &&
                    (_porder.lstpoLineItemDetails[i].Classcode == iClass) &&
                    (_porder.lstpoLineItemDetails[i].Vendorcode == iVendor) &&
                    (_porder.lstpoLineItemDetails[i].Stylecode == iStyle) &&
                    (_porder.lstpoLineItemDetails[i].Colorcode == iColor) &&
                    (_porder.lstpoLineItemDetails[i].Itemsize == iSize))
                {
                    iCasepackqty = _porder.lstpoLineItemDetails[i].Casepackqty;
                    return i;
                }

            }

            iCasepackqty = 0;
            return -1;
        }

        // HK : 07-12-2009 :  Same as FindHitItemCasePackQty. However the 
        // name has to be changed to give more meaning to the logic 
        // performed by the method
        private int GetItemCasePackQty(int iitemindex, short iClass, int iVendor,
                                    short iStyle, short iColor, short iSize,
                                    ref short iCasepackqty)
        {

            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                // HK : 28-11-2009 : Add ItemIndex
                if ((_porder.lstpoLineItemDetails[i].Itemindex == iitemindex) &&
                    (_porder.lstpoLineItemDetails[i].Classcode == iClass) &&
                    (_porder.lstpoLineItemDetails[i].Vendorcode == iVendor) &&
                    (_porder.lstpoLineItemDetails[i].Stylecode == iStyle) &&
                    (_porder.lstpoLineItemDetails[i].Colorcode == iColor) &&
                    (_porder.lstpoLineItemDetails[i].Itemsize == iSize))
                {
                    iCasepackqty = _porder.lstpoLineItemDetails[i].Casepackqty;
                    return i;
                }

            }

            iCasepackqty = 0;
            return -1;
        }


        // ///////////////////////////////////////////////////////////////////
        // HK : 02-12-2009 : Returns the correct POHit Collection index number
        // for the specified Hit
        // ie.  HIT         Index
        // ========================
        //      2nd         0
        //      3rd         1
        //      4th         2
        //      5th         3
        //      6th         4
        // ///////////////////////////////////////////////////////////////////
        private int GetHitsCollectionIndexForSpecificHit(int iHitNumber)
        {

            if (iHitNumber >=2 && iHitNumber <= 6)
            {
                return (iHitNumber - 2);
            }

            // Invalid hit number
            return -1;
        }

        private void dtgrdViewPOHits_2Hit_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Get the case pack 
            int     iitemindex;
            short   iClass;
            int     iVendor;
            short   iStyle;
            short   iColor;
            short   iSize;
            short   icasepackqty = 0;
            int     iitemrowindex;
            int     ihitcollectionindex;

            
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

            if (dtgrdViewPOHits_2Hit.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {

                string svalue;
                int itemqtyinput;

                // HK : FC : 02-12-2009 : A quantity of 0 is a valid quantity. So no validation
                svalue = e.FormattedValue.ToString();
                
                if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput))
                {
                    if (itemqtyinput == 0)
                    {
                        dtgrdViewPOHits_2Hit.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;

                        return;
                    }
                }

            
                // Get the Index of this Hit in the PO Hits collection
                // HK : 07-12-2009 : Now obeolete
                ihitcollectionindex = GetHitsCollectionIndexForSpecificHit (2);

                // Since this grid is databound we can read the Item Index, Class, Vendor,
                // Style, Color and Size from the datagrid itself rather than 
                // looping through the datatable in the PO Hits collection to which 
                // this datagrid is bound
                iitemindex  = Convert.ToInt16(dtgrdViewPOHits_2Hit.Rows[e.RowIndex].Cells["ItemIndex"].Value);
                iClass      = Convert.ToInt16(dtgrdViewPOHits_2Hit.Rows[e.RowIndex].Cells["Class"].Value);
                iVendor     = Convert.ToInt32(dtgrdViewPOHits_2Hit.Rows[e.RowIndex].Cells["Vendor"].Value);
                iStyle      = Convert.ToInt16(dtgrdViewPOHits_2Hit.Rows[e.RowIndex].Cells["Style"].Value);
                iColor      = Convert.ToInt16(dtgrdViewPOHits_2Hit.Rows[e.RowIndex].Cells["Color"].Value);
                iSize       = Convert.ToInt16(dtgrdViewPOHits_2Hit.Rows[e.RowIndex].Cells["Size"].Value);

                // Find the case pack for the PO Hit item in question
                // HK : 07-12-2009 : The case pack qty is on the main PO Line Items collection
                iitemrowindex = FindHitItemCasePackQty(iitemindex, iClass, iVendor, iStyle, iColor, iSize, ref icasepackqty);
                
                //iitemindex = Convert.ToInt16(_pohitscollection[ihitcollectionindex].dtPoHits.Rows[iitemrowindex]["ItemIndex"]);
                //iClass = Convert.ToInt16(_pohitscollection[ihitcollectionindex].dtPoHits.Rows[iitemrowindex]["Class"]);
                //iVendor = Convert.ToInt32(_pohitscollection[ihitcollectionindex].dtPoHits.Rows[iitemrowindex]["Vendor"]);
                //iStyle = Convert.ToInt16(_pohitscollection[ihitcollectionindex].dtPoHits.Rows[iitemrowindex]["Style"]);
                //iColor = Convert.ToInt16(_pohitscollection[ihitcollectionindex].dtPoHits.Rows[iitemrowindex]["Color"]);
                //iSize = Convert.ToInt16(_pohitscollection[ihitcollectionindex].dtPoHits.Rows[iitemrowindex]["Size"]);

                //Calculate the Totals in terms of total cost etc
                //if (ValidateQuantity(e.FormattedValue.ToString(), _polinedetails.Casepackqty))
                if (ValidateQuantity(e.FormattedValue.ToString(), icasepackqty))
                {
                    dtgrdViewPOHits_2Hit.Rows[e.RowIndex].ErrorText = "";

                    // ///////////////////////////////////////////////////////////////////
                    // HK : 11-11-2009 : If no rounding was done then continue validating 
                    // as usual
                    if (_itemquantityrounded == 0)
                    {
                        // ???????????_polinedetails.Itemquantity = int.Parse(e.FormattedValue.ToString());

                        // /////////////////////////////////////////////////////////////////
                        // HH: 05-11-2009 : Display the suggested quantity in the datagrid
                        // /////////////////////////////////////////////////////////////////

                        // ????????????????????dtgrdViewPOHits_2Hit.Rows[e.RowIndex].Cells["Quantity"].Value = _polinedetails.Itemquantity;

                        // /////////////////////////////////////////////////////////////////
                    }

                    // If rounding was done then capture the value so that the "CellValidated" EVENT 
                    // can display the rounded value
                    if (_itemquantityrounded > 0)
                    {
                        // ???????????????????????_polinedetails.Itemquantity = _itemquantityrounded;

                        // HK : 02-12-2009 : Will not the datatable in the collection be updated 
                        //automatically as the grid is databound. Come back and check.
                        //_pohitscollection[ihitcollectionindex].dtPoHits.Rows[iitemrowindex]["Quantity"] = _itemquantityrounded;

                    }

                    e.Cancel = false;

                }
                else
                {
                    dtgrdViewPOHits_2Hit.Rows[e.RowIndex].ErrorText = "Please enter valid " + dtgrdViewPOHits_2Hit.Columns[e.ColumnIndex].Name;
                    e.Cancel = true;

                }

            }

        }

        private void dtgrdViewPOHits_2Hit_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            // ////////////////////////////////////////////////////////////////////////
            // HK : Cater for quantity Rounding. When the rounding form is called from 
            // the Validating event and the user selects a suggested rounded value and
            // hits the 'Ok' button we must display this suggested value in the 
            // appropriate quantity cell in the grid. We cant do this in the Validating 
            // event as you cannot change the value typed by the user in the Validating
            // event, you can only flag as boolean whether the value is valid or invalid.
            // However the suggested rounded value can be displayed in the grid in the 
            // CValidated event

            // ////////////////////////////////////////////////////////////////////////

            Debug.Print("Cell Validated Fired");

            if (dtgrdViewPOHits_2Hit.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {
                if (_itemquantityrounded > 0)
                {
                    dtgrdViewPOHits_2Hit.Rows[e.RowIndex].Cells["Quantity"].Value = _itemquantityrounded;
                    _itemquantityrounded = 0;
                }
                
            }

        }

        private void tabCtlPOHits_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dtgrdViewPOHits_3Hit_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void chkBxActive_2Hit_CheckStateChanged(object sender, EventArgs e)
        {
            Debug.Print("Check State Changed fired");
            Debug.Print("Check state: " + chkBxActive_2Hit.Checked.ToString());
        }

        private void btnActivate_Hit1_Click(object sender, EventArgs e)
        {

            EnableControls(tabCtlPOHits.SelectedIndex);

            CreateHit(2);

            // Enable the Deactivate button
            btnDeactivate_Hit2.Enabled = true;
            btnDeactivate_Hit2.Visible = true;

            // Disable this button
            btnActivate_Hit2.Enabled = false;
            btnActivate_Hit2.Visible = false;
            
        }

        private void btnDeactivate_Hit1_Click(object sender, EventArgs e)
        {
            
            DisableControls(tabCtlPOHits.SelectedIndex);

            DeactivateHit(2);

            // Disable this button
            btnDeactivate_Hit2.Enabled = false;
            btnDeactivate_Hit2.Visible = false;


        }

        private void btnReactivate_Hit1_Click(object sender, EventArgs e)
        {
            
            EnableControls(tabCtlPOHits.SelectedIndex);

            ReactivateHit(2);

            // Enable the deactivate button
            btnDeactivate_Hit2.Enabled = true;

            // Disable this button
            //Enabled = false;

        }

        private string GetOriginalTabPageText (int itabpageindex)
        {
            switch (itabpageindex)
            {
                case 0:
                    return "2nd Hit";

                case 1:
                    return "3rd Hit";

                case 2:
                    return "4rd Hit";

                case 3:
                    return "5rd Hit";

                case 4:
                    return "6rd Hit";

                default:
                    return "Not a valid Hit";

            }

        }

        private void btnActivate_Hit2_Click(object sender, EventArgs e)
        {
            EnableControls(tabCtlPOHits.SelectedIndex);

            CreateHit(3);

            // Enable the Deactivate button
            btnDeactivate_Hit3.Enabled = true;
            btnDeactivate_Hit3.Visible = true;

            // Disable this button
            btnActivate_Hit3.Enabled = false;
            btnActivate_Hit3.Visible = false;
        }

        private void btnDeactivate_Hit2_Click(object sender, EventArgs e)
        {
            DisableControls(tabCtlPOHits.SelectedIndex);

            DeactivateHit(2);

            // Disable this button
            btnDeactivate_Hit3.Enabled = false;
            btnDeactivate_Hit3.Visible = false;

        }

        private void dtpkrAnticipateDate_3Hit_ValueChanged(object sender, EventArgs e)
        {
            Debug.Print("Anticipate Date:" + dtpkrAnticipateDate_3Hit.Value.Date.ToString());
            Debug.Print("Ship Date :" + dtpkrAnticipateDate_3Hit.Value.Date.ToString());

            if (_bDataBindingsInitalised)
            {

                // Assign the value of the Anticipate date to our hits collection
                // If the validation fails, still the value of this control
                // will be the value selected by the user although not quite correct
                // according to the business rules. However for the control it is 
                // a valid date.
                _pohitscollection[1].AnticipateDate = dtpkrAnticipateDate_3Hit.Value;
                //tabCtlPOHits.SelectedTab.Text = GetOriginalTabPageText(tabCtlPOHits.SelectedIndex) + " on " +
                //                                        dtpkrAnticipateDate_3Hit.Value.ToLongDateString();
                tabCtlPOHits.TabPages[1].Text = GetOriginalTabPageText(1) + " on " +
                                                        dtpkrAnticipateDate_3Hit.Value.ToLongDateString();

                if ((dtpkrAnticipateDate_3Hit.Value.Date < dtpkrShipDate_3Hit.Value.Date))
                {
                    errPOHits.SetError(dtpkrAnticipateDate_3Hit, "Anticipate date cannot be before the ship date");
                }
                else
                {
                    errPOHits.SetError(dtpkrAnticipateDate_3Hit, "");
                }
            }
        }

        private void dtpkrShipDate_3Hit_ValueChanged(object sender, EventArgs e)
        {
            if (_bDataBindingsInitalised)
            {
                //_porder.ShippingDate = dtpkrShipDate.Value;
                // Assign the value of the Anticipate date to our hits collection
                _pohitscollection[1].ShippingDate = dtpkrShipDate_3Hit.Value;

                if (_porder.Penvironment.Domain == "TDSNA")
                //For TDSNA
                {
                    txtCancelDate_3Hit.Text = dtpkrShipDate_3Hit.Value.AddDays(7).ToString("D");
                    //_porder.CancelDate = dtpkrShipDate.Value.AddDays(7);
                    _pohitscollection[1].CancelDate = dtpkrShipDate_3Hit.Value.AddDays(7).ToString("D");
                }

                //For TDSE
                // HK : 19-11-2009 : Format date as [LondDate]
                txtCancelDate_3Hit.Text = dtpkrShipDate_3Hit.Value.ToLongDateString();
                _pohitscollection[1].CancelDate = dtpkrShipDate_3Hit.Value.ToLongDateString(); ;
                //_porder.CancelDate = dtpkrShipDate.Value;

                //if (DateTime.Compare (dtpkrAnticipateDate.Value, dtpkrShipDate.Value) > 0)
                Debug.Print("Anticipate Date:" + dtpkrAnticipateDate_3Hit.Value.Date.ToString());
                Debug.Print("Ship Date :" + dtpkrShipDate_3Hit.Value.Date.ToString());

                if ((dtpkrAnticipateDate_3Hit.Value.Date < dtpkrShipDate_3Hit.Value.Date))
                {
                    //errPOEntry.SetError(dtpkrShipDate, "Anticipate date less than ship date");
                    errPOHits.SetError(dtpkrShipDate_3Hit, "Shipping date cannot be after the anticipate date");
                }
                else
                {
                    errPOHits.SetError(dtpkrShipDate_3Hit, "");

                }
            }
        }

        private void btnReset_3Hit_Click(object sender, EventArgs e)
        {
            // Clear the Items on Hit 2
            _pohitscollection[1].dtPoHits.Clear();

            // Add again
            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                _pohitscollection[1].dtPoHits.Rows.Add(false,
                                            _porder.lstpoLineItemDetails[i].Itemindex,
                                            _porder.lstpoLineItemDetails[i].APP1,
                                            _porder.lstpoLineItemDetails[i].Classcode,
                                            _porder.lstpoLineItemDetails[i].Vendorcode,
                                            _porder.lstpoLineItemDetails[i].Stylecode,
                                            _porder.lstpoLineItemDetails[i].Colorcode,
                                            _porder.lstpoLineItemDetails[i].Itemsize,
                                            _porder.lstpoLineItemDetails[i].Itemlongdescription,
                                            0);

            }
        }

        private void btnDelete_3Hit_Click(object sender, EventArgs e)
        {

            // Stop datadrid validation from occuring
            _bUserWantsToDeleteLine = true;

            // Item on Hit 3 deleted
            DeleteDataGridRow(3);

            // Enable datadrid validation again
            _bUserWantsToDeleteLine = false;
        }

        private void btnActivate_Hit4_Click(object sender, EventArgs e)
        {
            EnableControls(tabCtlPOHits.SelectedIndex);

            CreateHit(4);

            // Enable the Deactivate button
            btnDeactivate_Hit4.Enabled = true;
            btnDeactivate_Hit4.Visible = true;

            // Disable this button
            btnActivate_Hit4.Enabled = false;
            btnActivate_Hit4.Visible = false;

        }

        private void btnActivate_Hit2_Click_1(object sender, EventArgs e)
        {
            EnableControls(tabCtlPOHits.SelectedIndex);

            CreateHit(2);

            // Enable the Deactivate button
            btnDeactivate_Hit2.Enabled = true;
            btnDeactivate_Hit2.Visible = true;

            // Disable this button
            btnActivate_Hit2.Enabled = false;
            btnActivate_Hit2.Visible = false;

        }

        private void btnDeactivate_Hit2_Click_1(object sender, EventArgs e)
        {
            DisableControls(tabCtlPOHits.SelectedIndex);

            DeactivateHit(2);

            // Disable this button
            btnDeactivate_Hit2.Enabled = false;
            btnDeactivate_Hit2.Visible = false;


        }

        private void btnActivate_Hit3_Click(object sender, EventArgs e)
        {
            EnableControls(tabCtlPOHits.SelectedIndex);

            CreateHit(3);

            // Enable the Deactivate button
            btnDeactivate_Hit3.Enabled = true;
            btnDeactivate_Hit3.Visible = true;

            // Disable this button
            btnActivate_Hit3.Enabled = false;
            btnActivate_Hit3.Visible = false;


        }

        private void btnDeactivate_Hit3_Click(object sender, EventArgs e)
        {
            DisableControls(tabCtlPOHits.SelectedIndex);

            DeactivateHit(3);

            // Disable this button
            btnDeactivate_Hit3.Enabled = false;
            btnDeactivate_Hit3.Visible = false;

        }

        private void btnDeactivate_Hit4_Click(object sender, EventArgs e)
        {
            DisableControls(tabCtlPOHits.SelectedIndex);

            DeactivateHit(4);

            // Disable this button
            btnDeactivate_Hit4.Enabled = false;
            btnDeactivate_Hit4.Visible = false;


        }

        private void btnReset_4Hit_Click(object sender, EventArgs e)
        {
            // Clear the Items on Hit 4
            _pohitscollection[2].dtPoHits.Clear();

            // Add again
            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                _pohitscollection[2].dtPoHits.Rows.Add(false,
                                            _porder.lstpoLineItemDetails[i].Itemindex,
                                            _porder.lstpoLineItemDetails[i].APP1,
                                            _porder.lstpoLineItemDetails[i].Classcode,
                                            _porder.lstpoLineItemDetails[i].Vendorcode,
                                            _porder.lstpoLineItemDetails[i].Stylecode,
                                            _porder.lstpoLineItemDetails[i].Colorcode,
                                            _porder.lstpoLineItemDetails[i].Itemsize,
                                            _porder.lstpoLineItemDetails[i].Itemlongdescription,
                                            0);

            }

        }

        private void btnDelete_4Hit_Click(object sender, EventArgs e)
        {
            // Stop datadrid validation from occuring
            _bUserWantsToDeleteLine = true;

            // Item on Hit 4 deleted
            DeleteDataGridRow(4);

            // Enable datadrid validation again
            _bUserWantsToDeleteLine = false;
        }

        private void btnActivate_Hit5_Click(object sender, EventArgs e)
        {
            EnableControls(tabCtlPOHits.SelectedIndex);

            CreateHit(5);

            // Enable the Deactivate button
            btnDeactivate_Hit5.Enabled = true;
            btnDeactivate_Hit5.Visible = true;

            // Disable this button
            btnActivate_Hit5.Enabled = false;
            btnActivate_Hit5.Visible = false;

        }

        private void btnDeactivate_Hit5_Click(object sender, EventArgs e)
        {
            DisableControls(tabCtlPOHits.SelectedIndex);

            DeactivateHit(5);

            // Disable this button
            btnDeactivate_Hit5.Enabled = false;
            btnDeactivate_Hit5.Visible = false;
        }

        private void btnReset_5Hit_Click(object sender, EventArgs e)
        {
            // Clear the Items on Hit 5
            _pohitscollection[3].dtPoHits.Clear();

            // Add again
            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                _pohitscollection[3].dtPoHits.Rows.Add(false,
                                            _porder.lstpoLineItemDetails[i].Itemindex,
                                            _porder.lstpoLineItemDetails[i].APP1,
                                            _porder.lstpoLineItemDetails[i].Classcode,
                                            _porder.lstpoLineItemDetails[i].Vendorcode,
                                            _porder.lstpoLineItemDetails[i].Stylecode,
                                            _porder.lstpoLineItemDetails[i].Colorcode,
                                            _porder.lstpoLineItemDetails[i].Itemsize,
                                            _porder.lstpoLineItemDetails[i].Itemlongdescription,
                                            0);

            }

        }

        private void btnDelete_5Hit_Click(object sender, EventArgs e)
        {
            // Stop datadrid validation from occuring
            _bUserWantsToDeleteLine = true;

            // Item on Hit 5 deleted
            DeleteDataGridRow(5);

            // Enable datadrid validation again
            _bUserWantsToDeleteLine = false;

        }

        private void btnActivate_Hit6_Click(object sender, EventArgs e)
        {
            EnableControls(tabCtlPOHits.SelectedIndex);

            CreateHit(6);

            // Enable the Deactivate button
            btnDeactivate_Hit6.Enabled = true;
            btnDeactivate_Hit6.Visible = true;

            // Disable this button
            btnActivate_Hit6.Enabled = false;
            btnActivate_Hit6.Visible = false;

        }

        private void btnDeactivate_Hit6_Click(object sender, EventArgs e)
        {
            DisableControls(tabCtlPOHits.SelectedIndex);

            DeactivateHit(6);

            // Disable this button
            btnDeactivate_Hit6.Enabled = false;
            btnDeactivate_Hit6.Visible = false;
        }

        private void btnReset_6Hit_Click(object sender, EventArgs e)
        {
            // Clear the Items on Hit 2
            _pohitscollection[4].dtPoHits.Clear();

            // Add again
            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                _pohitscollection[4].dtPoHits.Rows.Add(false,
                                            _porder.lstpoLineItemDetails[i].Itemindex,
                                            _porder.lstpoLineItemDetails[i].APP1,
                                            _porder.lstpoLineItemDetails[i].Classcode,
                                            _porder.lstpoLineItemDetails[i].Vendorcode,
                                            _porder.lstpoLineItemDetails[i].Stylecode,
                                            _porder.lstpoLineItemDetails[i].Colorcode,
                                            _porder.lstpoLineItemDetails[i].Itemsize,
                                            _porder.lstpoLineItemDetails[i].Itemlongdescription,
                                            0);

            }

        }

        private void btnDelete_6Hit_Click(object sender, EventArgs e)
        {

            // Stop datadrid validation from occuring
            _bUserWantsToDeleteLine = true;

            // Item on Hit 6 deleted
            DeleteDataGridRow(6);

            // Enable datadrid validation again
            _bUserWantsToDeleteLine = false;

        }

        private void dtpkrAnticipateDate_4Hit_ValueChanged(object sender, EventArgs e)
        {

            Debug.Print("Anticipate Date:" + dtpkrAnticipateDate_4Hit.Value.Date.ToString());
            Debug.Print("Ship Date :" + dtpkrAnticipateDate_4Hit.Value.Date.ToString());

            if (_bDataBindingsInitalised)
            {

                // Assign the value of the Anticipate date to our hits collection
                // If the validation fails, still the value of this control
                // will be the value selected by the user although not quite correct
                // according to the business rules. However for the control it is 
                // a valid date.
                _pohitscollection[2].AnticipateDate = dtpkrAnticipateDate_4Hit.Value;
                //tabCtlPOHits.SelectedTab.Text = GetOriginalTabPageText(tabCtlPOHits.SelectedIndex) + " on " + 
                //                                        dtpkrAnticipateDate_2Hit.Value.ToLongDateString ();
                tabCtlPOHits.TabPages[2].Text = GetOriginalTabPageText(2) + " on " +
                                                        dtpkrAnticipateDate_4Hit.Value.ToLongDateString();

                if ((dtpkrAnticipateDate_4Hit.Value.Date < dtpkrShipDate_4Hit.Value.Date))
                {
                    errPOHits.SetError(dtpkrAnticipateDate_4Hit, "Anticipate date cannot be before the ship date");
                }
                else
                {
                    errPOHits.SetError(dtpkrAnticipateDate_4Hit, "");
                }
            }

        }

        private void dtpkrAnticipateDate_4Hit_Validating(object sender, CancelEventArgs e)
        {
            if (_bFormInitalised)
            {
                if (dtpkrAnticipateDate_4Hit.Value < DateTime.Today)
                {
                    // validationcls.HighlightErrControls(lblAnticipateDate, dtpkrAnticipateDate, false);
                    errPOHits.SetError(dtpkrAnticipateDate_4Hit, "Please enter a date greater than Today");
                    //_porder.AnticipateDate = dtpkrAnticipateDate.Value;
                    e.Cancel = true;
                }
                else
                {
                    // validationcls.HighlightErrControls(lblAnticipateDate, dtpkrAnticipateDate, true);
                    errPOHits.SetError(dtpkrAnticipateDate_4Hit, "");
                    //_porder.AnticipateDate = DateTime.Now;
                    e.Cancel = false;
                }
            }
        }

        private void dtpkrShipDate_3Hit_Validating(object sender, CancelEventArgs e)
        {
            if (_bDataBindingsInitalised)
            {
                if (dtpkrShipDate_3Hit.Value < DateTime.Today)
                {
                    //  validationcls.HighlightErrControls(lblShipDate, dtpkrShipDate, false);
                    errPOHits.SetError(dtpkrShipDate_3Hit, "Please enter a date greater than  Today");
                    //_porder.ShippingDate = dtpkrShipDate.Value;
                    e.Cancel = true;
                }
                else
                {
                    //  validationcls.HighlightErrControls(lblShipDate, dtpkrShipDate, true);
                    errPOHits.SetError(dtpkrShipDate_3Hit, "");
                    //_porder.ShippingDate = DateTime.Now;
                    e.Cancel = false;
                }
            }
        }

        private void dtpkrShipDate_4Hit_ValueChanged(object sender, EventArgs e)
        {
            if (_bDataBindingsInitalised)
            {
                //_porder.ShippingDate = dtpkrShipDate.Value;
                // Assign the value of the Anticipate date to our hits collection
                _pohitscollection[2].ShippingDate = dtpkrShipDate_4Hit.Value;

                if (_porder.Penvironment.Domain == "TDSNA")
                //For TDSNA
                {
                    txtCancelDate_4Hit.Text = dtpkrShipDate_4Hit.Value.AddDays(7).ToString("D");
                    //_porder.CancelDate = dtpkrShipDate.Value.AddDays(7);
                    _pohitscollection[2].CancelDate = dtpkrShipDate_4Hit.Value.AddDays(7).ToString("D");
                }

                //For TDSE
                // HK : 19-11-2009 : Format date as [LondDate]
                txtCancelDate_4Hit.Text = dtpkrShipDate_4Hit.Value.ToLongDateString();
                _pohitscollection[2].CancelDate = dtpkrShipDate_4Hit.Value.ToLongDateString(); ;
                //_porder.CancelDate = dtpkrShipDate.Value;

                //if (DateTime.Compare (dtpkrAnticipateDate.Value, dtpkrShipDate.Value) > 0)
                Debug.Print("Anticipate Date:" + dtpkrAnticipateDate_4Hit.Value.Date.ToString());
                Debug.Print("Ship Date :" + dtpkrShipDate_4Hit.Value.Date.ToString());

                if ((dtpkrAnticipateDate_4Hit.Value.Date < dtpkrShipDate_4Hit.Value.Date))
                {
                    //errPOEntry.SetError(dtpkrShipDate, "Anticipate date less than ship date");
                    errPOHits.SetError(dtpkrShipDate_4Hit, "Shipping date cannot be after the anticipate date");
                }
                else
                {
                    errPOHits.SetError(dtpkrShipDate_4Hit, "");

                }
            }
        }

        private void dtpkrShipDate_4Hit_Validating(object sender, CancelEventArgs e)
        {
            if (_bDataBindingsInitalised)
            {
                if (dtpkrShipDate_4Hit.Value < DateTime.Today)
                {
                    //  validationcls.HighlightErrControls(lblShipDate, dtpkrShipDate, false);
                    errPOHits.SetError(dtpkrShipDate_4Hit, "Please enter a date greater than  Today");
                    //_porder.ShippingDate = dtpkrShipDate.Value;
                    e.Cancel = true;
                }
                else
                {
                    //  validationcls.HighlightErrControls(lblShipDate, dtpkrShipDate, true);
                    errPOHits.SetError(dtpkrShipDate_4Hit, "");
                    //_porder.ShippingDate = DateTime.Now;
                    e.Cancel = false;
                }
            }
        }

        private void dtpkrAnticipateDate_5Hit_ValueChanged(object sender, EventArgs e)
        {
            Debug.Print("Anticipate Date:" + dtpkrAnticipateDate_5Hit.Value.Date.ToString());
            Debug.Print("Ship Date :" + dtpkrAnticipateDate_5Hit.Value.Date.ToString());

            if (_bDataBindingsInitalised)
            {

                // Assign the value of the Anticipate date to our hits collection
                // If the validation fails, still the value of this control
                // will be the value selected by the user although not quite correct
                // according to the business rules. However for the control it is 
                // a valid date.
                _pohitscollection[3].AnticipateDate = dtpkrAnticipateDate_5Hit.Value;
                //tabCtlPOHits.SelectedTab.Text = GetOriginalTabPageText(tabCtlPOHits.SelectedIndex) + " on " + 
                //                                        dtpkrAnticipateDate_2Hit.Value.ToLongDateString ();
                // HK : 07-12-2009: Never use selected index ever with multi tabs
                //tabCtlPOHits.TabPages[0].Text = GetOriginalTabPageText(tabCtlPOHits.SelectedIndex) + " on " +
                //                                        dtpkrAnticipateDate_3Hit.Value.ToLongDateString();
                tabCtlPOHits.TabPages[3].Text = GetOriginalTabPageText(3) + " on " +
                                                        dtpkrAnticipateDate_5Hit.Value.ToLongDateString();

                if ((dtpkrAnticipateDate_5Hit.Value.Date < dtpkrShipDate_5Hit.Value.Date))
                {
                    errPOHits.SetError(dtpkrAnticipateDate_5Hit, "Anticipate date cannot be before the ship date");
                }
                else
                {
                    errPOHits.SetError(dtpkrAnticipateDate_5Hit, "");
                }
            }
        }

        private void dtpkrAnticipateDate_5Hit_Validating(object sender, CancelEventArgs e)
        {
            if (_bDataBindingsInitalised)
            {
                if (dtpkrShipDate_5Hit.Value < DateTime.Today)
                {
                    //  validationcls.HighlightErrControls(lblShipDate, dtpkrShipDate, false);
                    errPOHits.SetError(dtpkrShipDate_5Hit, "Please enter a date greater than  Today");
                    //_porder.ShippingDate = dtpkrShipDate.Value;
                    e.Cancel = true;
                }
                else
                {
                    //  validationcls.HighlightErrControls(lblShipDate, dtpkrShipDate, true);
                    errPOHits.SetError(dtpkrShipDate_5Hit, "");
                    //_porder.ShippingDate = DateTime.Now;
                    e.Cancel = false;
                }
            }
        }

        private void dtpkrShipDate_5Hit_ValueChanged(object sender, EventArgs e)
        {
            if (_bDataBindingsInitalised)
            {
                //_porder.ShippingDate = dtpkrShipDate.Value;
                // Assign the value of the Anticipate date to our hits collection
                _pohitscollection[3].ShippingDate = dtpkrShipDate_5Hit.Value;

                if (_porder.Penvironment.Domain == "TDSNA")
                //For TDSNA
                {
                    txtCancelDate_5Hit.Text = dtpkrShipDate_5Hit.Value.AddDays(7).ToString("D");
                    //_porder.CancelDate = dtpkrShipDate.Value.AddDays(7);
                    _pohitscollection[3].CancelDate = dtpkrShipDate_5Hit.Value.AddDays(7).ToString("D");
                }

                //For TDSE
                // HK : 19-11-2009 : Format date as [LondDate]
                txtCancelDate_5Hit.Text = dtpkrShipDate_5Hit.Value.ToLongDateString();
                _pohitscollection[3].CancelDate = dtpkrShipDate_5Hit.Value.ToLongDateString(); ;
                //_porder.CancelDate = dtpkrShipDate.Value;

                //if (DateTime.Compare (dtpkrAnticipateDate.Value, dtpkrShipDate.Value) > 0)
                Debug.Print("Anticipate Date:" + dtpkrAnticipateDate_5Hit.Value.Date.ToString());
                Debug.Print("Ship Date :" + dtpkrShipDate_5Hit.Value.Date.ToString());

                if ((dtpkrAnticipateDate_5Hit.Value.Date < dtpkrShipDate_5Hit.Value.Date))
                {
                    //errPOEntry.SetError(dtpkrShipDate, "Anticipate date less than ship date");
                    errPOHits.SetError(dtpkrShipDate_5Hit, "Shipping date cannot be after the anticipate date");
                }
                else
                {
                    errPOHits.SetError(dtpkrShipDate_5Hit, "");

                }
            }
        }

        private void dtpkrShipDate_5Hit_Validating(object sender, CancelEventArgs e)
        {
            if (_bDataBindingsInitalised)
            {
                if (dtpkrShipDate_5Hit.Value < DateTime.Today)
                {
                    //  validationcls.HighlightErrControls(lblShipDate, dtpkrShipDate, false);
                    errPOHits.SetError(dtpkrShipDate_5Hit, "Please enter a date greater than  Today");
                    //_porder.ShippingDate = dtpkrShipDate.Value;
                    e.Cancel = true;
                }
                else
                {
                    //  validationcls.HighlightErrControls(lblShipDate, dtpkrShipDate, true);
                    errPOHits.SetError(dtpkrShipDate_5Hit, "");
                    //_porder.ShippingDate = DateTime.Now;
                    e.Cancel = false;
                }
            }
        }

        private void dtpkrAnticipateDate_6Hit_ValueChanged(object sender, EventArgs e)
        {
            Debug.Print("Anticipate Date:" + dtpkrAnticipateDate_6Hit.Value.Date.ToString());
            Debug.Print("Ship Date :" + dtpkrAnticipateDate_6Hit.Value.Date.ToString());

            if (_bDataBindingsInitalised)
            {

                // Assign the value of the Anticipate date to our hits collection
                // If the validation fails, still the value of this control
                // will be the value selected by the user although not quite correct
                // according to the business rules. However for the control it is 
                // a valid date.
                _pohitscollection[4].AnticipateDate = dtpkrAnticipateDate_6Hit.Value;
                //tabCtlPOHits.SelectedTab.Text = GetOriginalTabPageText(tabCtlPOHits.SelectedIndex) + " on " + 
                //                                        dtpkrAnticipateDate_2Hit.Value.ToLongDateString ();
                // HK : 07-12-2009: Never use selected index ever with multi tabs
                //tabCtlPOHits.TabPages[0].Text = GetOriginalTabPageText(tabCtlPOHits.SelectedIndex) + " on " +
                //                                        dtpkrAnticipateDate_3Hit.Value.ToLongDateString();
                tabCtlPOHits.TabPages[4].Text = GetOriginalTabPageText(4) + " on " +
                                                        dtpkrAnticipateDate_6Hit.Value.ToLongDateString();

                if ((dtpkrAnticipateDate_6Hit.Value.Date < dtpkrShipDate_6Hit.Value.Date))
                {
                    errPOHits.SetError(dtpkrAnticipateDate_6Hit, "Anticipate date cannot be before the ship date");
                }
                else
                {
                    errPOHits.SetError(dtpkrAnticipateDate_6Hit, "");
                }
            }
        }

        private void dtpkrAnticipateDate_6Hit_Validating(object sender, CancelEventArgs e)
        {
            if (_bDataBindingsInitalised)
            {
                if (dtpkrShipDate_6Hit.Value < DateTime.Today)
                {
                    //  validationcls.HighlightErrControls(lblShipDate, dtpkrShipDate, false);
                    errPOHits.SetError(dtpkrShipDate_6Hit, "Please enter a date greater than  Today");
                    //_porder.ShippingDate = dtpkrShipDate.Value;
                    e.Cancel = true;
                }
                else
                {
                    //  validationcls.HighlightErrControls(lblShipDate, dtpkrShipDate, true);
                    errPOHits.SetError(dtpkrShipDate_6Hit, "");
                    //_porder.ShippingDate = DateTime.Now;
                    e.Cancel = false;
                }
            }
        }

        private void dtpkrShipDate_6Hit_ValueChanged(object sender, EventArgs e)
        {
            if (_bDataBindingsInitalised)
            {
                //_porder.ShippingDate = dtpkrShipDate.Value;
                // Assign the value of the Anticipate date to our hits collection
                _pohitscollection[4].ShippingDate = dtpkrShipDate_6Hit.Value;

                if (_porder.Penvironment.Domain == "TDSNA")
                //For TDSNA
                {
                    txtCancelDate_6Hit.Text = dtpkrShipDate_6Hit.Value.AddDays(7).ToString("D");
                    //_porder.CancelDate = dtpkrShipDate.Value.AddDays(7);
                    _pohitscollection[4].CancelDate = dtpkrShipDate_6Hit.Value.AddDays(7).ToString("D");
                }

                //For TDSE
                // HK : 19-11-2009 : Format date as [LondDate]
                txtCancelDate_6Hit.Text = dtpkrShipDate_6Hit.Value.ToLongDateString();
                _pohitscollection[4].CancelDate = dtpkrShipDate_6Hit.Value.ToLongDateString(); ;
                //_porder.CancelDate = dtpkrShipDate.Value;

                //if (DateTime.Compare (dtpkrAnticipateDate.Value, dtpkrShipDate.Value) > 0)
                Debug.Print("Anticipate Date:" + dtpkrAnticipateDate_6Hit.Value.Date.ToString());
                Debug.Print("Ship Date :" + dtpkrShipDate_6Hit.Value.Date.ToString());

                if ((dtpkrAnticipateDate_6Hit.Value.Date < dtpkrShipDate_6Hit.Value.Date))
                {
                    //errPOEntry.SetError(dtpkrShipDate, "Anticipate date less than ship date");
                    errPOHits.SetError(dtpkrShipDate_6Hit, "Shipping date cannot be after the anticipate date");
                }
                else
                {
                    errPOHits.SetError(dtpkrShipDate_6Hit, "");

                }
            }
        }

        private void dtpkrShipDate_6Hit_Validating(object sender, CancelEventArgs e)
        {
            if (_bDataBindingsInitalised)
            {
                if (dtpkrShipDate_6Hit.Value < DateTime.Today)
                {
                    //  validationcls.HighlightErrControls(lblShipDate, dtpkrShipDate, false);
                    errPOHits.SetError(dtpkrShipDate_6Hit, "Please enter a date greater than  Today");
                    //_porder.ShippingDate = dtpkrShipDate.Value;
                    e.Cancel = true;
                }
                else
                {
                    //  validationcls.HighlightErrControls(lblShipDate, dtpkrShipDate, true);
                    errPOHits.SetError(dtpkrShipDate_6Hit, "");
                    //_porder.ShippingDate = DateTime.Now;
                    e.Cancel = false;
                }
            }
        }

        private void dtgrdViewPOHits_3Hit_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Get the case pack 
            int iitemindex;
            short iClass;
            int iVendor;
            short iStyle;
            short iColor;
            short iSize;
            short icasepackqty = 0;
            int iitemrowindex;
            int ihitcollectionindex;


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

            if (dtgrdViewPOHits_3Hit.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {

                string svalue;
                int itemqtyinput;

                // HK : FC : 02-12-2009 : A quantity of 0 is a valid quantity. So no validation
                svalue = e.FormattedValue.ToString();

                if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput))
                {
                    if (itemqtyinput == 0)
                    {
                        dtgrdViewPOHits_2Hit.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;

                        return;
                    }
                }


                // Get the Index of this Hit in the PO Hits collection
                ihitcollectionindex = GetHitsCollectionIndexForSpecificHit(3);

                // Since this grid is databound we can read the Item Index, Class, Vendor,
                // Style, Color and Size from the datagrid itself rather than 
                // looping through the datatable in the PO Hits collection to which 
                // this datagrid is bound
                iitemindex = Convert.ToInt16(dtgrdViewPOHits_3Hit.Rows[e.RowIndex].Cells["ItemIndex"].Value);
                iClass = Convert.ToInt16(dtgrdViewPOHits_3Hit.Rows[e.RowIndex].Cells["Class"].Value);
                iVendor = Convert.ToInt32(dtgrdViewPOHits_3Hit.Rows[e.RowIndex].Cells["Vendor"].Value);
                iStyle = Convert.ToInt16(dtgrdViewPOHits_3Hit.Rows[e.RowIndex].Cells["Style"].Value);
                iColor = Convert.ToInt16(dtgrdViewPOHits_3Hit.Rows[e.RowIndex].Cells["Color"].Value);
                iSize = Convert.ToInt16(dtgrdViewPOHits_3Hit.Rows[e.RowIndex].Cells["Size"].Value);

                // Find the case pack for the PO Hit item in question
                iitemrowindex = FindHitItemCasePackQty(iitemindex, iClass, iVendor, iStyle, iColor, iSize, ref icasepackqty);

                //Calculate the Totals in terms of total cost etc
                if (ValidateQuantity(e.FormattedValue.ToString(), icasepackqty))
                {
                    dtgrdViewPOHits_3Hit.Rows[e.RowIndex].ErrorText = "";

                    e.Cancel = false;

                }
                else
                {
                    dtgrdViewPOHits_3Hit.Rows[e.RowIndex].ErrorText = "Please enter valid " + dtgrdViewPOHits_2Hit.Columns[e.ColumnIndex].Name;
                    e.Cancel = true;

                }

            }
        }

        private void dtgrdViewPOHits_3Hit_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            Debug.Print("Cell Validated Fired");

            if (dtgrdViewPOHits_3Hit.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {
                if (_itemquantityrounded > 0)
                {
                    dtgrdViewPOHits_3Hit.Rows[e.RowIndex].Cells["Quantity"].Value = _itemquantityrounded;
                    _itemquantityrounded = 0;
                }

            }
        }

        private void dtgrdViewPOHits_4Hit_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Get the case pack 
            int iitemindex;
            short iClass;
            int iVendor;
            short iStyle;
            short iColor;
            short iSize;
            short icasepackqty = 0;
            int iitemrowindex;
            int ihitcollectionindex;


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

            if (dtgrdViewPOHits_4Hit.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {

                string svalue;
                int itemqtyinput;

                // HK : FC : 02-12-2009 : A quantity of 0 is a valid quantity. So no validation
                svalue = e.FormattedValue.ToString();

                if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput))
                {
                    if (itemqtyinput == 0)
                    {
                        dtgrdViewPOHits_2Hit.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;

                        return;
                    }
                }


                // Get the Index of this Hit in the PO Hits collection
                ihitcollectionindex = GetHitsCollectionIndexForSpecificHit(4);

                // Since this grid is databound we can read the Item Index, Class, Vendor,
                // Style, Color and Size from the datagrid itself rather than 
                // looping through the datatable in the PO Hits collection to which 
                // this datagrid is bound
                iitemindex = Convert.ToInt16(dtgrdViewPOHits_4Hit.Rows[e.RowIndex].Cells["ItemIndex"].Value);
                iClass = Convert.ToInt16(dtgrdViewPOHits_4Hit.Rows[e.RowIndex].Cells["Class"].Value);
                iVendor = Convert.ToInt32(dtgrdViewPOHits_4Hit.Rows[e.RowIndex].Cells["Vendor"].Value);
                iStyle = Convert.ToInt16(dtgrdViewPOHits_4Hit.Rows[e.RowIndex].Cells["Style"].Value);
                iColor = Convert.ToInt16(dtgrdViewPOHits_4Hit.Rows[e.RowIndex].Cells["Color"].Value);
                iSize = Convert.ToInt16(dtgrdViewPOHits_4Hit.Rows[e.RowIndex].Cells["Size"].Value);

                // Find the case pack for the PO Hit item in question
                iitemrowindex = FindHitItemCasePackQty(iitemindex, iClass, iVendor, iStyle, iColor, iSize, ref icasepackqty);

                //Calculate the Totals in terms of total cost etc
                if (ValidateQuantity(e.FormattedValue.ToString(), icasepackqty))
                {
                    dtgrdViewPOHits_4Hit.Rows[e.RowIndex].ErrorText = "";

                    e.Cancel = false;

                }
                else
                {
                    dtgrdViewPOHits_4Hit.Rows[e.RowIndex].ErrorText = "Please enter valid " + dtgrdViewPOHits_2Hit.Columns[e.ColumnIndex].Name;
                    e.Cancel = true;

                }

            }
        }

        private void dtgrdViewPOHits_4Hit_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            Debug.Print("Cell Validated Fired");

            if (dtgrdViewPOHits_4Hit.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {
                if (_itemquantityrounded > 0)
                {
                    dtgrdViewPOHits_4Hit.Rows[e.RowIndex].Cells["Quantity"].Value = _itemquantityrounded;
                    _itemquantityrounded = 0;
                }

            }
        }

        private void dtgrdViewPOHits_5Hit_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Get the case pack 
            int iitemindex;
            short iClass;
            int iVendor;
            short iStyle;
            short iColor;
            short iSize;
            short icasepackqty = 0;
            int iitemrowindex;
            int ihitcollectionindex;


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

            if (dtgrdViewPOHits_5Hit.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {

                string svalue;
                int itemqtyinput;

                // HK : FC : 02-12-2009 : A quantity of 0 is a valid quantity. So no validation
                svalue = e.FormattedValue.ToString();

                if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput))
                {
                    if (itemqtyinput == 0)
                    {
                        dtgrdViewPOHits_5Hit.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;

                        return;
                    }
                }


                // Get the Index of this Hit in the PO Hits collection
                ihitcollectionindex = GetHitsCollectionIndexForSpecificHit(5);

                // Since this grid is databound we can read the Item Index, Class, Vendor,
                // Style, Color and Size from the datagrid itself rather than 
                // looping through the datatable in the PO Hits collection to which 
                // this datagrid is bound
                iitemindex = Convert.ToInt16(dtgrdViewPOHits_5Hit.Rows[e.RowIndex].Cells["ItemIndex"].Value);
                iClass = Convert.ToInt16(dtgrdViewPOHits_5Hit.Rows[e.RowIndex].Cells["Class"].Value);
                iVendor = Convert.ToInt32(dtgrdViewPOHits_5Hit.Rows[e.RowIndex].Cells["Vendor"].Value);
                iStyle = Convert.ToInt16(dtgrdViewPOHits_5Hit.Rows[e.RowIndex].Cells["Style"].Value);
                iColor = Convert.ToInt16(dtgrdViewPOHits_5Hit.Rows[e.RowIndex].Cells["Color"].Value);
                iSize = Convert.ToInt16(dtgrdViewPOHits_5Hit.Rows[e.RowIndex].Cells["Size"].Value);

                // Find the case pack for the PO Hit item in question
                iitemrowindex = FindHitItemCasePackQty(iitemindex, iClass, iVendor, iStyle, iColor, iSize, ref icasepackqty);

                //Calculate the Totals in terms of total cost etc
                if (ValidateQuantity(e.FormattedValue.ToString(), icasepackqty))
                {
                    dtgrdViewPOHits_5Hit.Rows[e.RowIndex].ErrorText = "";

                    e.Cancel = false;

                }
                else
                {
                    dtgrdViewPOHits_5Hit.Rows[e.RowIndex].ErrorText = "Please enter valid " + dtgrdViewPOHits_2Hit.Columns[e.ColumnIndex].Name;
                    e.Cancel = true;

                }

            }
        }

        private void dtgrdViewPOHits_5Hit_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            Debug.Print("Cell Validated Fired");

            if (dtgrdViewPOHits_5Hit.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {
                if (_itemquantityrounded > 0)
                {
                    dtgrdViewPOHits_5Hit.Rows[e.RowIndex].Cells["Quantity"].Value = _itemquantityrounded;
                    _itemquantityrounded = 0;
                }

            }
        }

        private void dtgrdViewPOHits_6Hit_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Get the case pack 
            int iitemindex;
            short iClass;
            int iVendor;
            short iStyle;
            short iColor;
            short iSize;
            short icasepackqty = 0;
            int iitemrowindex;
            int ihitcollectionindex;


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

            if (dtgrdViewPOHits_6Hit.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {

                string svalue;
                int itemqtyinput;

                // HK : FC : 02-12-2009 : A quantity of 0 is a valid quantity. So no validation
                svalue = e.FormattedValue.ToString();

                if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput))
                {
                    if (itemqtyinput == 0)
                    {
                        dtgrdViewPOHits_6Hit.Rows[e.RowIndex].ErrorText = "";
                        e.Cancel = false;

                        return;
                    }
                }


                // Get the Index of this Hit in the PO Hits collection
                ihitcollectionindex = GetHitsCollectionIndexForSpecificHit(6);

                // Since this grid is databound we can read the Item Index, Class, Vendor,
                // Style, Color and Size from the datagrid itself rather than 
                // looping through the datatable in the PO Hits collection to which 
                // this datagrid is bound
                iitemindex = Convert.ToInt16(dtgrdViewPOHits_6Hit.Rows[e.RowIndex].Cells["ItemIndex"].Value);
                iClass = Convert.ToInt16(dtgrdViewPOHits_6Hit.Rows[e.RowIndex].Cells["Class"].Value);
                iVendor = Convert.ToInt32(dtgrdViewPOHits_6Hit.Rows[e.RowIndex].Cells["Vendor"].Value);
                iStyle = Convert.ToInt16(dtgrdViewPOHits_6Hit.Rows[e.RowIndex].Cells["Style"].Value);
                iColor = Convert.ToInt16(dtgrdViewPOHits_6Hit.Rows[e.RowIndex].Cells["Color"].Value);
                iSize = Convert.ToInt16(dtgrdViewPOHits_6Hit.Rows[e.RowIndex].Cells["Size"].Value);

                // Find the case pack for the PO Hit item in question
                iitemrowindex = FindHitItemCasePackQty(iitemindex, iClass, iVendor, iStyle, iColor, iSize, ref icasepackqty);

                //Calculate the Totals in terms of total cost etc
                if (ValidateQuantity(e.FormattedValue.ToString(), icasepackqty))
                {
                    dtgrdViewPOHits_6Hit.Rows[e.RowIndex].ErrorText = "";

                    e.Cancel = false;

                }
                else
                {
                    dtgrdViewPOHits_6Hit.Rows[e.RowIndex].ErrorText = "Please enter valid " + dtgrdViewPOHits_2Hit.Columns[e.ColumnIndex].Name;
                    e.Cancel = true;

                }

            }
        }

        private void dtgrdViewPOHits_6Hit_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            Debug.Print("Cell Validated Fired");

            if (dtgrdViewPOHits_6Hit.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {
                if (_itemquantityrounded > 0)
                {
                    dtgrdViewPOHits_6Hit.Rows[e.RowIndex].Cells["Quantity"].Value = _itemquantityrounded;
                    _itemquantityrounded = 0;
                }

            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx"); 
        }

        // HK : 17-12-2009 : Refresh the items in the Hits item list
        private void RefreshItems(int iHitNumber)
        {
            // First remove any items(s) the user has removed / deleted
            // on the main PO Entry form

            // To do this loop through the hits grid and see 
            // if the itmes in hits grid still exists in the 
            // PO Items collection

            int     itemindex;
            short   classcode;
            int     vendor;
            short   style;
            short   color;
            short   size;
            int     qty;

            //string  sselectexpr;

            DataTable dtPoHitsOriginal;

            int ihitscollectionindex;

            ihitscollectionindex = GetHitsCollectionIndexForSpecificHit(iHitNumber);

            dtPoHitsOriginal = _pohitscollection[ihitscollectionindex].dtPoHits.Copy();

            // ////////////////////////////////////////////////////////////
            // HK : 16-01-2010 : Fix Bug 219 :
            // Make a copy if there are row in the Hits table (dtHits)
            //if (dtPoHitsOriginal.Rows.Count > 0)
            //{
                _pohitscollection[ihitscollectionindex].CreateCopyOfPoHit();
            //}
            // ////////////////////////////////////////////////////////////

            // Clear the datatable associated with this Hit number
            _pohitscollection[ihitscollectionindex].dtPoHits.Clear();

            // Now reinsert the records as if we are 'Reset'ing the Hit
            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                itemindex   = _porder.lstpoLineItemDetails[i].Itemindex;
                classcode   = _porder.lstpoLineItemDetails[i].Classcode;
                vendor      = _porder.lstpoLineItemDetails[i].Vendorcode;
                style       = _porder.lstpoLineItemDetails[i].Stylecode;
                color       = _porder.lstpoLineItemDetails[i].Colorcode;
                size        = _porder.lstpoLineItemDetails[i].Itemsize;

                // Adjust the quantity for existing items
                Object[] objprimarykey = new object[6];
                objprimarykey[0] = itemindex;
                objprimarykey[1] = classcode;
                objprimarykey[2] = vendor;
                objprimarykey[3] = style;
                objprimarykey[4] = color;
                objprimarykey[5] = size;

                dtPoHitsOriginal.PrimaryKey = new DataColumn[] {dtPoHitsOriginal.Columns["ItemIndex"],
                                                                    dtPoHitsOriginal.Columns["Class"],
                                                                    dtPoHitsOriginal.Columns["Vendor"],
                                                                    dtPoHitsOriginal.Columns["Style"],
                                                                    dtPoHitsOriginal.Columns["Color"],
                                                                    dtPoHitsOriginal.Columns["Size"]};

                DataRow dr;
                dr = dtPoHitsOriginal.Rows.Find(objprimarykey);

                if (dr != null)
                {
                    qty = Convert.ToInt32(dr["Quantity"]);
                }
                else
                {
                    qty = 0;
                }
                _pohitscollection[ihitscollectionindex].dtPoHits.Rows.Add(false,
                                                        _porder.lstpoLineItemDetails[i].Itemindex,
                                                        _porder.lstpoLineItemDetails[i].APP1,
                                                        _porder.lstpoLineItemDetails[i].Classcode,
                                                        _porder.lstpoLineItemDetails[i].Vendorcode,
                                                        _porder.lstpoLineItemDetails[i].Stylecode,
                                                        _porder.lstpoLineItemDetails[i].Colorcode,
                                                        _porder.lstpoLineItemDetails[i].Itemsize,
                                                        _porder.lstpoLineItemDetails[i].Itemlongdescription,
                                                        qty);

            }

            dtPoHitsOriginal.Dispose ();
        }

        private int GetQuantityOfItemOnHit(int iHitNumber, int itemindex, short classcode,
                                                            int vendorcode, short stylecode,
                                                            short colorcode, short itemsize)
        {
            int qty;
            int ihitscollectionindex;
            DataTable dtPoHitsOriginal;

            ihitscollectionindex = GetHitsCollectionIndexForSpecificHit(iHitNumber);

            dtPoHitsOriginal = _pohitscollection[ihitscollectionindex].dtPoHits.Copy();


            // Adjust the quantity for existing items
            Object[] objprimarykey = new object[5];
            objprimarykey[0] = itemindex;
            objprimarykey[1] = classcode;
            objprimarykey[2] = vendorcode;
            objprimarykey[3] = stylecode;
            objprimarykey[4] = colorcode;
            objprimarykey[5] = itemsize;

            DataRow dr;
            dr = dtPoHitsOriginal.Rows.Find(objprimarykey);

            if (dr != null)
            {
                qty = Convert.ToInt32(dr["Quantity"]);
            }
            else
            {
                qty = 0;
            }

            dtPoHitsOriginal.Dispose();

            return qty;

        }

        private void RevertQuattityToOriginal(int hitnumber)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _pohitscollection[0].dtPoHits.Clear();
        }

    }
}
