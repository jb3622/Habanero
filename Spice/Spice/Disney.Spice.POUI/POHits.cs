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
using Disney.Menu;

namespace Disney.Spice.POUI
{
    public partial class POHits : Form
    {
        public delegate void OkButtonClickedEventHandler(object sender, PoHitsEventArgs e);
        public event OkButtonClickedEventHandler OnOkButtonClicked;
        public delegate void CancelButtonClickedEventHandler(object sender, PoHitsEventArgs e);
        public event CancelButtonClickedEventHandler OnCancelButtonClicked;
        private PurchaseOrder           _porder;
        PurchaseOrder.PoHitsCollection  _pohitscollection;
        private Boolean                 _bUserWantsToDeleteLine = false;
        private int                     _itemquantityrounded = 0;
        private Boolean                 _bFormCancelClicked = false;
        private Boolean                 _bFormInitalised = false;
        private Disney.Spice.ErrorProvider.ErrorProviderWithCount ErrPro;
        private Environments environment;

        private void POHitsForm_Load(object sender, EventArgs e)
        {
            if (environment.DateFormat == "DMY")
            {
                dtpkrAnticipateDate_2Hit.CustomFormat = "d MMMM yyyy";
                dtpkrShipDate_2Hit.CustomFormat = "d MMMM yyyy";
                dtpkrAnticipateDate_3Hit.CustomFormat = "d MMMM yyyy";
                dtpkrShipDate_3Hit.CustomFormat = "d MMMM yyyy";
                dtpkrAnticipateDate_4Hit.CustomFormat = "d MMMM yyyy";
                dtpkrShipDate_4Hit.CustomFormat = "d MMMM yyyy";
                dtpkrAnticipateDate_5Hit.CustomFormat = "d MMMM yyyy";
                dtpkrShipDate_5Hit.CustomFormat = "d MMMM yyyy";
                dtpkrAnticipateDate_6Hit.CustomFormat = "d MMMM yyyy";
                dtpkrShipDate_6Hit.CustomFormat = "d MMMM yyyy";
            }
            else
            {
                dtpkrAnticipateDate_2Hit.CustomFormat = "MMMMd,  yyyy";
                dtpkrShipDate_2Hit.CustomFormat = "MMMMd,  yyyy";
                dtpkrAnticipateDate_3Hit.CustomFormat = "MMMMd,  yyyy";
                dtpkrShipDate_3Hit.CustomFormat = "MMMMd,  yyyy";
                dtpkrAnticipateDate_4Hit.CustomFormat = "MMMMd,  yyyy";
                dtpkrShipDate_4Hit.CustomFormat = "MMMMd,  yyyy";
                dtpkrAnticipateDate_5Hit.CustomFormat = "MMMMd,  yyyy";
                dtpkrShipDate_5Hit.CustomFormat = "MMMMd,  yyyy";
                dtpkrAnticipateDate_6Hit.CustomFormat = "MMMMd,  yyyy";
                dtpkrShipDate_6Hit.CustomFormat = "MMMMd,  yyyy";
            }
        }

        public POHits(PurchaseOrder porder, PurchaseOrder.PoHitsCollection pohitscollection,Environments environment)
        {
            InitializeComponent();

            ErrPro = new Disney.Spice.ErrorProvider.ErrorProviderWithCount(this.components);
            ErrPro.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            ErrPro.ContainerControl = this;

            _bFormInitalised = true;

            _porder = porder;
            _pohitscollection = pohitscollection;
            this.environment = environment;

            dtpkrAnticipateDate_2Hit.DataBindings.CollectionChanging += new CollectionChangeEventHandler(DataBindings_CollectionChanging);
            dtpkrAnticipateDate_2Hit.DataBindings.CollectionChanged += new CollectionChangeEventHandler(DataBindings_CollectionChanged);

            SetupHits();
        }

        void DataBindings_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            //_bDataBindingsInitalised = false;
        }

        void DataBindings_CollectionChanging(object sender, CollectionChangeEventArgs e)
        {
            //_bDataBindingsInitalised = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _bFormCancelClicked = true;

            // Any data entry (quantity) done should be rolled
            // back if the user selects the 'Cancel' button.
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
            dtgridview.Columns["Quantity"].Width = 80;

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
            dtgridview.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        private void InitalizeDataBingings(int iHitNumber)
        {
            if (iHitNumber == 2)
            {
                txtVendorComment1_2Hit.DataBindings.Add("Text", _pohitscollection[0], "Vendorcomments1");
                txtVendorComment2_2Hit.DataBindings.Add("Text", _pohitscollection[0], "Vendorcomments2");
                txtVendorComment3_2Hit.DataBindings.Add("Text", _pohitscollection[0], "Vendorcomments3");
                txtInternalComments1_2Hit.DataBindings.Add("Text", _pohitscollection[0], "Internalcomments1");
                txtInternalComments2_2Hit.DataBindings.Add("Text", _pohitscollection[0], "Internalcomments2");

                dtpkrAnticipateDate_2Hit.Value = _pohitscollection[0].AnticipateDate;
                dtpkrShipDate_2Hit.Value = _pohitscollection[0].ShippingDate;
                txtCancelDate_2Hit.Text = _pohitscollection[0].CancelDate;
            }

            if (iHitNumber == 3)
            {
                txtVendorComment1_3Hit.DataBindings.Add("Text", _pohitscollection[1], "Vendorcomments1");
                txtVendorComment2_3Hit.DataBindings.Add("Text", _pohitscollection[1], "Vendorcomments2");
                txtVendorComment3_3Hit.DataBindings.Add("Text", _pohitscollection[1], "Vendorcomments3");
                txtInternalComments1_3Hit.DataBindings.Add("Text", _pohitscollection[1], "Internalcomments1");
                txtInternalComments2_3Hit.DataBindings.Add("Text", _pohitscollection[1], "Internalcomments2");

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

                dtpkrAnticipateDate_6Hit.Value  = _pohitscollection[4].AnticipateDate;
                dtpkrShipDate_6Hit.Value        = _pohitscollection[4].ShippingDate;
                txtCancelDate_6Hit.Text         = _pohitscollection[4].CancelDate;
            }

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


                        if (item.HitActivated == true)
                        {
                            RefreshItems(item.HitNUmber);
                        }

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
            if (_pohitscollection.Count > 0)
            {
                foreach (PurchaseOrder.POHits item in _pohitscollection)
                {
                    // Hit 2
                    if (item.HitNUmber == 2 && item.HitInitalised == true)
                    {
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

                    // Hit 3
                    if (item.HitNUmber == 3 && item.HitInitalised == true)
                    {
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

                    // Hit 4
                    if (item.HitNUmber == 4 && item.HitInitalised == true)
                    {
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

                    // Hit 5
                    if (item.HitNUmber == 5 && item.HitInitalised == true)
                    {
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

                    // Hit 6
                    if (item.HitNUmber == 6 && item.HitInitalised == true)
                    {
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
            int ihitscollectionindex;
            ihitscollectionindex = GetHitsCollectionIndexForSpecificHit(2);

            if (chkBxActive_2Hit.Checked)
            {
                tabCtlPOHits.SelectedTab.Text= tabCtlPOHits.TabPages[1].Text + " on " + dtpkrAnticipateDate_2Hit.Text;
                
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
            _pohitscollection[ihitscollectionindex].dtPoHits.Clear();
        }

        private void ReactivateHit(int ihitnumber)
        {
            int ihitscollectionindex;
            ihitscollectionindex = GetHitsCollectionIndexForSpecificHit(ihitnumber);

            _pohitscollection[ihitscollectionindex].HitActivated = true;
        }

        private void CreateHit(int ihitnumber)
        {
            int ihitscollectionindex;
            ihitscollectionindex = GetHitsCollectionIndexForSpecificHit(ihitnumber);

            _pohitscollection[ihitscollectionindex].HitActivated = true;

            if (_pohitscollection[ihitscollectionindex].HitInitalised == true)
            {
                RefreshItems(ihitnumber);

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
            }

            if (_pohitscollection[ihitscollectionindex].HitInitalised == false)
            {
               for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
                {
                    if (_porder.lstpoLineItemDetails[i].IsDeleted == false && _porder.lstpoLineItemDetails[i].IsValid == true)
                    {
                        _pohitscollection[ihitscollectionindex].dtPoHits.Rows.Add(false,
                                                                _porder.lstpoLineItemDetails[i].Sequence,
                                                                _porder.lstpoLineItemDetails[i].APP1,
                                                                _porder.lstpoLineItemDetails[i].ClassCode,
                                                                _porder.lstpoLineItemDetails[i].Vendorcode,
                                                                _porder.lstpoLineItemDetails[i].Stylecode,
                                                                _porder.lstpoLineItemDetails[i].Colorcode,
                                                                _porder.lstpoLineItemDetails[i].Itemsize,
                                                                _porder.lstpoLineItemDetails[i].Itemlongdescription,
                                                                0);
                    }
                }

                _pohitscollection[ihitscollectionindex].HitInitalised = true;
                _pohitscollection[ihitscollectionindex].CreateCopyOfPoHit();

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
                
        private void AddPoItemsToDataGrid(int ihitnumber)
        {
        }

        private void DisableControls(int iSelectedTab)
        {
            // Tab Page 1 (2nd Hit)
            if (iSelectedTab == 0)
            {
                //Dates
                dtpkrAnticipateDate_2Hit.Enabled = false;
                dtpkrShipDate_2Hit.Enabled = false;

                // Gridview
                dtgrdViewPOHits_2Hit.Enabled = false;
                
                // Group box
                grpBxComments.Enabled = false;

                // Comments
                txtVendorComment1_2Hit.Enabled = false;
                txtVendorComment2_2Hit.Enabled = false;
                txtVendorComment3_2Hit.Enabled = false;
                txtInternalComments1_2Hit.Enabled = false;
                txtInternalComments2_2Hit.Enabled = false;

                // Buttons
                btnReset_2Hit.Enabled = false;
                btnDelete_2Hit.Enabled = false;

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

        private void EnableControls(int iSelectedTab)
        {
            if (iSelectedTab == 0)
            {
                //Dates
                dtpkrAnticipateDate_2Hit.Enabled = true;
                dtpkrShipDate_2Hit.Enabled = true;

                // Gridview
                dtgrdViewPOHits_2Hit.Enabled = true;
                
                // Group box
                grpBxComments.Enabled = true;

                // Vendor Comments
                txtVendorComment1_2Hit.Enabled = true;
                txtVendorComment2_2Hit.Enabled = true;
                txtVendorComment3_2Hit.Enabled = true;
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
            
            // Tab Page 1 (3nd Hit)
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

                tabCtlPOHits.TabPages[1].Text = GetOriginalTabPageText(iSelectedTab) + " on " +
                                                dtpkrAnticipateDate_3Hit.Value.ToLongDateString();

            }

            // Tab Page 2 (4th Hit)
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

                tabCtlPOHits.TabPages[2].Text = GetOriginalTabPageText(iSelectedTab) + " on " +
                                                dtpkrAnticipateDate_4Hit.Value.ToLongDateString();
            }

            // Tab Page 3 (5th Hit)
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

            // Tab Page 4 (6th Hit)
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

                tabCtlPOHits.TabPages[4].Text = GetOriginalTabPageText(iSelectedTab) + " on " +
                                                dtpkrAnticipateDate_6Hit.Value.ToLongDateString();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //ErrPro.ClearAll();
            ValidateChildren();

            if (ErrPro.ErrorCount == 0)
            {
                PoHitsEventArgs e1 = new PoHitsEventArgs(_pohitscollection);
                this.DialogResult = DialogResult.OK;
                RaiseOkButtonClickedEvent(e1);

                Close();
            }
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
            foreach (Control control in tcol)
            {
                control.Enabled = true;
            }
        }

        private void EnableControls(TabPage tabpage)
        {
            foreach (Control control in tabpage.Controls)
            {
                control.Enabled = true;
            }
        }

        private void DeleteDataGridRow(int iHitNumber)
        {
            int iRowsDeleted = 0;
            int iRunningCountTotalRows = 0;
            int iLoopCounter;
            DataGridView dtgridview;

            dtgridview = GetDataGridViewForHit(iHitNumber);
                        
            // Init the looping counter
            iLoopCounter = 0;
            iRunningCountTotalRows = dtgridview.Rows.Count;

            if (iRunningCountTotalRows == 0)
            {
                return;
            }

            do
            {
                if (!dtgridview.Rows[iLoopCounter].IsNewRow)
                {
                    if (dtgridview.Rows[iLoopCounter].Cells[0].Value != null &&
                                Convert.ToBoolean(dtgridview.Rows[iLoopCounter].Cells[0].Value) == true)
                    {
                        Debug.Print("Data Grid row removed at index: " + iLoopCounter.ToString());
            
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

        private bool ValidateQuantity(string svalue, int packQty)
        {
            bool bisValid = false;
            int itemqtyinput;

            if (!String.IsNullOrEmpty(svalue) && Int32.TryParse(svalue, out itemqtyinput))
            {
                if ((itemqtyinput % packQty != 0))
                {
                    ItemQuantityForm itemqtyform = new ItemQuantityForm(itemqtyinput, packQty);
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
            _itemquantityrounded = iroundedquantity;
        }

        private int FindHitItemCasePackQty(int iitemindex, short iClass, int iVendor, 
                                            short iStyle, short iColor, short iSize, 
                                            ref int iCasepackqty)
        {
            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {   
                if ((_porder.lstpoLineItemDetails[i].IsDeleted == false) &&
                    (_porder.lstpoLineItemDetails[i].IsValid == true) &&
                    (_porder.lstpoLineItemDetails[i].Sequence == iitemindex) &&
                    (_porder.lstpoLineItemDetails[i].ClassCode == iClass) &&
                    (_porder.lstpoLineItemDetails[i].Vendorcode == iVendor) &&
                    (_porder.lstpoLineItemDetails[i].Stylecode == iStyle) &&
                    (_porder.lstpoLineItemDetails[i].Colorcode == iColor) &&
                    (_porder.lstpoLineItemDetails[i].Itemsize == iSize))
                {
                    iCasepackqty = _porder.lstpoLineItemDetails[i].CasePackQty;
                    return i;
                }
            }

            iCasepackqty = 0;
            return -1;
        }

        private int GetItemCasePackQty(int iitemindex, short iClass, int iVendor,
                                    short iStyle, short iColor, short iSize,
                                    ref int iCasepackqty)
        {
            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {   
                if ((_porder.lstpoLineItemDetails[i].IsDeleted == false) &&
                    (_porder.lstpoLineItemDetails[i].IsValid == true) &&
                    (_porder.lstpoLineItemDetails[i].Sequence == iitemindex) &&
                    (_porder.lstpoLineItemDetails[i].ClassCode == iClass) &&
                    (_porder.lstpoLineItemDetails[i].Vendorcode == iVendor) &&
                    (_porder.lstpoLineItemDetails[i].Stylecode == iStyle) &&
                    (_porder.lstpoLineItemDetails[i].Colorcode == iColor) &&
                    (_porder.lstpoLineItemDetails[i].Itemsize == iSize))
                {
                    iCasepackqty = _porder.lstpoLineItemDetails[i].CasePackQty;
                    return i;
                }
            }

            iCasepackqty = 0;
            return -1;
        }

        private int GetHitsCollectionIndexForSpecificHit(int iHitNumber)
        {
            if (iHitNumber >=2 && iHitNumber <= 6)
            {
                return (iHitNumber - 2);
            }

            return -1;
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


        #region Hit 1
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

            btnDeactivate_Hit2.Enabled = true;
        }
        #endregion
        
        #region Hit 2
        
        private void btnDeactivate_Hit2_Click(object sender, EventArgs e)
        {
            DisableControls(tabCtlPOHits.SelectedIndex);

            DeactivateHit(2);

            btnDeactivate_Hit3.Enabled = false;
            btnDeactivate_Hit3.Visible = false;
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
        
        private void dtgrdViewPOHits_2Hit_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Prevent Datagrid Validation if the user clicked Cancel button
            if (_bFormCancelClicked && _bUserWantsToDeleteLine)
            {
                return;
            }

            if (dtgrdViewPOHits_2Hit.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {
                dtgrdViewPOHits_2Hit[e.ColumnIndex, e.RowIndex].ErrorText = String.Empty;
                Int32 itemqtyinput = 0;
                if (!String.IsNullOrEmpty(e.FormattedValue.ToString()))
                {
                    if (!Int32.TryParse(e.FormattedValue.ToString(), out itemqtyinput))
                    {
                        dtgrdViewPOHits_2Hit[e.ColumnIndex, e.RowIndex].ErrorText = "Invalid value entered";
                        ErrPro.SetControlError(dtgrdViewPOHits_2Hit, "Cell errors");
                        e.Cancel = true;
                    }
                }
            
                // Get the Index of this Hit in the PO Hits collection
                Int32 ihitcollectionindex = GetHitsCollectionIndexForSpecificHit(2);

                // Since this grid is databound we can read the Item Index, Class, Vendor,
                // Style, Color and Size from the datagrid itself rather than 
                // looping through the datatable in the PO Hits collection to which 
                // this datagrid is bound
                Int16 iitemindex  = Convert.ToInt16(dtgrdViewPOHits_2Hit.Rows[e.RowIndex].Cells["ItemIndex"].Value);
                Int16 iClass  = Convert.ToInt16(dtgrdViewPOHits_2Hit["Class",e.RowIndex].Value);
                Int32 iVendor = Convert.ToInt32(dtgrdViewPOHits_2Hit.Rows[e.RowIndex].Cells["Vendor"].Value);
                Int16 iStyle  = Convert.ToInt16(dtgrdViewPOHits_2Hit.Rows[e.RowIndex].Cells["Style"].Value);
                Int16 iColor  = Convert.ToInt16(dtgrdViewPOHits_2Hit.Rows[e.RowIndex].Cells["Color"].Value);
                Int16 iSize   = Convert.ToInt16(dtgrdViewPOHits_2Hit.Rows[e.RowIndex].Cells["Size"].Value);

                // Find the case pack for the PO Hit item in question
                // The case pack qty is on the main PO Line Items collection
                Int32 icasepackqty = 0;
                Int32 iitemrowindex = FindHitItemCasePackQty(iitemindex, iClass, iVendor, iStyle, iColor, iSize, ref icasepackqty);

                if (!ValidateQuantity(e.FormattedValue.ToString(), icasepackqty))
                {
                    dtgrdViewPOHits_2Hit[e.ColumnIndex, e.RowIndex].ErrorText = "Enter a quantity rounded to the nearest CasePackQty";
                    dtgrdViewPOHits_2Hit.EndEdit();
                    ErrPro.SetControlError(dtgrdViewPOHits_2Hit, "Cell errors");
                    e.Cancel = true;
                }
                else
                {
                    ErrPro.SetControlError(dtgrdViewPOHits_2Hit, "");
                    e.Cancel = false;
                }
            }
        }

        private void dtgrdViewPOHits_2Hit_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (dtgrdViewPOHits_2Hit.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {
                if (_itemquantityrounded > 0)
                {
                    dtgrdViewPOHits_2Hit.Rows[e.RowIndex].Cells["Quantity"].Value = _itemquantityrounded;
                    _itemquantityrounded = 0;
                }
            }
        }
        
        private void btnDelete_2Hit_Click(object sender, EventArgs e)
        {
            _bUserWantsToDeleteLine = true;

            DeleteDataGridRow(2);

            _bUserWantsToDeleteLine = false;
        }

        private void btnReset_2Hit_Click(object sender, EventArgs e)
        {   
            _pohitscollection[0].dtPoHits.Clear();

            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                if (_porder.lstpoLineItemDetails[i].IsDeleted == false && _porder.lstpoLineItemDetails[i].IsValid == true)
                {
                    _pohitscollection[0].dtPoHits.Rows.Add(false,
                                                _porder.lstpoLineItemDetails[i].Sequence,
                                                _porder.lstpoLineItemDetails[i].APP1,
                                                _porder.lstpoLineItemDetails[i].ClassCode,
                                                _porder.lstpoLineItemDetails[i].Vendorcode,
                                                _porder.lstpoLineItemDetails[i].Stylecode,
                                                _porder.lstpoLineItemDetails[i].Colorcode,
                                                _porder.lstpoLineItemDetails[i].Itemsize,
                                                _porder.lstpoLineItemDetails[i].Itemlongdescription,
                                                0);
                }
            }
        }

        private void dtpkrShipDate_2Hit_ValueChanged(object sender, EventArgs e)
        {
            _pohitscollection[0].ShippingDate = dtpkrShipDate_2Hit.Value;

            //if (_porder.Penvironment.Domain == "SWNA")
            if (DataCache.DisplayPOHitsCancelDateWithDayFormat == true)
            {
                txtCancelDate_2Hit.Text = dtpkrShipDate_2Hit.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate).ToString("D");
                _pohitscollection[0].CancelDate = dtpkrShipDate_2Hit.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate).ToString("D");
            }
            else
            {
                txtCancelDate_2Hit.Text = dtpkrShipDate_2Hit.Value.ToLongDateString();
                _pohitscollection[0].CancelDate = dtpkrShipDate_2Hit.Value.ToLongDateString(); ;
            }

            if ((dtpkrAnticipateDate_2Hit.Value.Date < dtpkrShipDate_2Hit.Value.Date))
            {
                errPOHits.SetError(dtpkrShipDate_2Hit, "Shipping date cannot be after the anticipate date");
            }
            else
            {
                errPOHits.SetError(dtpkrShipDate_2Hit, "");
            }            
        }

        private void dtpkrAnticipateDate_2Hit_Validating(object sender, CancelEventArgs e)
        {
            if (_bFormInitalised)
            {
                ErrPro.SetControlError(dtpkrAnticipateDate_2Hit, string.Empty);
                if (dtpkrAnticipateDate_2Hit.Value < DateTime.Today)
                {
                    ErrPro.SetError(dtpkrAnticipateDate_2Hit,"Please enter a date greater than Today");
                }
            }
        }

        private void dtpkrAnticipateDate_2Hit_ValueChanged(object sender, EventArgs e)
        {
            _pohitscollection[0].AnticipateDate = dtpkrAnticipateDate_2Hit.Value;
            tabCtlPOHits.TabPages[0].Text = GetOriginalTabPageText(0) + " on " +
                                dtpkrAnticipateDate_2Hit.Value.ToLongDateString();

            if ((dtpkrAnticipateDate_2Hit.Value.Date < dtpkrShipDate_2Hit.Value.Date))
            {
                ErrPro.SetError(dtpkrAnticipateDate_2Hit,"Anticipate date cannot be before the ship date");
            }
            else
            {
                ErrPro.SetError(dtpkrAnticipateDate_2Hit, string.Empty);
            }
            
        }

        private void dtpkrShipDate_2Hit_Validating(object sender, CancelEventArgs e)
        {
            
            ErrPro.SetControlError(dtpkrShipDate_2Hit, string.Empty);
            if (dtpkrShipDate_2Hit.Value < DateTime.Today)
            {
                ErrPro.SetControlError(dtpkrShipDate_2Hit, "Please enter a date greater than Today's date");
            }
            if (dtpkrAnticipateDate_2Hit.Value < dtpkrShipDate_2Hit.Value)
            {
                ErrPro.SetControlError(dtpkrShipDate_2Hit, "Anticipate date must be greater than Ship date");
            }
            
        }
        #endregion

        #region Hit 3
        private void dtpkrAnticipateDate_3Hit_ValueChanged(object sender, EventArgs e)
        {
            // Assign the value of the Anticipate date to our hits collection
            // If the validation fails, still the value of this control
            // will be the value selected by the user although not quite correct
            // according to the business rules. However for the control it is 
            // a valid date.
            _pohitscollection[1].AnticipateDate = dtpkrAnticipateDate_3Hit.Value;
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

        private void dtpkrShipDate_3Hit_ValueChanged(object sender, EventArgs e)
        {
            
            _pohitscollection[1].ShippingDate = dtpkrShipDate_3Hit.Value;

            //if (_porder.Penvironment.Domain == "SWNA")
            if (DataCache.DisplayPOHitsCancelDateWithDayFormat == true)
            {
                txtCancelDate_3Hit.Text = dtpkrShipDate_3Hit.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate).ToString("D");
                _pohitscollection[1].CancelDate = dtpkrShipDate_3Hit.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate).ToString("D");
            }
            else
            {
                txtCancelDate_3Hit.Text = dtpkrShipDate_3Hit.Value.ToLongDateString();
                _pohitscollection[1].CancelDate = dtpkrShipDate_3Hit.Value.ToLongDateString(); ;
            }

            if ((dtpkrAnticipateDate_3Hit.Value.Date < dtpkrShipDate_3Hit.Value.Date))
            {
                errPOHits.SetError(dtpkrShipDate_3Hit, "Shipping date cannot be after the anticipate date");
            }
            else
            {
                errPOHits.SetError(dtpkrShipDate_3Hit, "");
            }
            
        }

        private void btnReset_3Hit_Click(object sender, EventArgs e)
        {
            // Clear the Items on Hit 3
            _pohitscollection[1].dtPoHits.Clear();

            // Add again
            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                if (_porder.lstpoLineItemDetails[i].IsDeleted == false && _porder.lstpoLineItemDetails[i].IsValid == true)
                {
                    _pohitscollection[1].dtPoHits.Rows.Add(false,
                                                _porder.lstpoLineItemDetails[i].Sequence,
                                                _porder.lstpoLineItemDetails[i].APP1,
                                                _porder.lstpoLineItemDetails[i].ClassCode,
                                                _porder.lstpoLineItemDetails[i].Vendorcode,
                                                _porder.lstpoLineItemDetails[i].Stylecode,
                                                _porder.lstpoLineItemDetails[i].Colorcode,
                                                _porder.lstpoLineItemDetails[i].Itemsize,
                                                _porder.lstpoLineItemDetails[i].Itemlongdescription,
                                                0);
                }
            }
        }

        private void btnDelete_3Hit_Click(object sender, EventArgs e)
        {
            _bUserWantsToDeleteLine = true;

            DeleteDataGridRow(3);

            _bUserWantsToDeleteLine = false;
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

        private void dtpkrShipDate_3Hit_Validating(object sender, CancelEventArgs e)
        {
            
            if (dtpkrShipDate_3Hit.Value < DateTime.Today)
            {
                errPOHits.SetError(dtpkrShipDate_3Hit, "Please enter a date greater than  Today");
                e.Cancel = true;
            }
            else
            {
                    
                errPOHits.SetError(dtpkrShipDate_3Hit, "");
                e.Cancel = false;
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
            int icasepackqty = 0;
            int iitemrowindex;
            int ihitcollectionindex;

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
                                
                if (!ValidateQuantity(e.FormattedValue.ToString(), icasepackqty))
                {
                    dtgrdViewPOHits_3Hit[e.ColumnIndex, e.RowIndex].ErrorText = "Enter a quantity rounded to the nearest CasePackQty";
                    dtgrdViewPOHits_3Hit.EndEdit();
                    ErrPro.SetControlError(dtgrdViewPOHits_3Hit, "Cell errors");
                    e.Cancel = true;
                }
                else
                {
                    ErrPro.SetControlError(dtgrdViewPOHits_3Hit, "");
                    e.Cancel = false;
                }
            }
        }
        
        private void dtgrdViewPOHits_3Hit_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (dtgrdViewPOHits_3Hit.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {
                if (_itemquantityrounded > 0)
                {
                    dtgrdViewPOHits_3Hit.Rows[e.RowIndex].Cells["Quantity"].Value = _itemquantityrounded;
                    _itemquantityrounded = 0;
                }
            }
        }
        #endregion

        #region Hit 4
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
                if (_porder.lstpoLineItemDetails[i].IsDeleted == false && _porder.lstpoLineItemDetails[i].IsValid == true)
                {
                    _pohitscollection[2].dtPoHits.Rows.Add(false,
                                                _porder.lstpoLineItemDetails[i].Sequence,
                                                _porder.lstpoLineItemDetails[i].APP1,
                                                _porder.lstpoLineItemDetails[i].ClassCode,
                                                _porder.lstpoLineItemDetails[i].Vendorcode,
                                                _porder.lstpoLineItemDetails[i].Stylecode,
                                                _porder.lstpoLineItemDetails[i].Colorcode,
                                                _porder.lstpoLineItemDetails[i].Itemsize,
                                                _porder.lstpoLineItemDetails[i].Itemlongdescription,
                                                0);
                }
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

        private void dtpkrAnticipateDate_4Hit_ValueChanged(object sender, EventArgs e)
        {
            _pohitscollection[2].AnticipateDate = dtpkrAnticipateDate_4Hit.Value;
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

        private void dtpkrAnticipateDate_4Hit_Validating(object sender, CancelEventArgs e)
        {
            if (_bFormInitalised)
            {
                if (dtpkrAnticipateDate_4Hit.Value < DateTime.Today)
                {
                    errPOHits.SetError(dtpkrAnticipateDate_4Hit, "Please enter a date greater than Today");
                    e.Cancel = true;
                }
                else
                {
                    errPOHits.SetError(dtpkrAnticipateDate_4Hit, "");
                    e.Cancel = false;
                }
            }
        }

        private void dtpkrShipDate_4Hit_ValueChanged(object sender, EventArgs e)
        {
            _pohitscollection[2].ShippingDate = dtpkrShipDate_4Hit.Value;

            //if (_porder.Penvironment.Domain == "SWNA")
            if (DataCache.DisplayPOHitsCancelDateWithDayFormat == true)
            {
                txtCancelDate_4Hit.Text = dtpkrShipDate_4Hit.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate).ToString("D");
                _pohitscollection[2].CancelDate = dtpkrShipDate_4Hit.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate).ToString("D");
            }
            else
            {
                txtCancelDate_4Hit.Text = dtpkrShipDate_4Hit.Value.ToLongDateString();
                _pohitscollection[2].CancelDate = dtpkrShipDate_4Hit.Value.ToLongDateString(); ;
            }

            if ((dtpkrAnticipateDate_4Hit.Value.Date < dtpkrShipDate_4Hit.Value.Date))
            {
                errPOHits.SetError(dtpkrShipDate_4Hit, "Shipping date cannot be after the anticipate date");
            }
            else
            {
                errPOHits.SetError(dtpkrShipDate_4Hit, "");
            }
            
        }

        private void dtpkrShipDate_4Hit_Validating(object sender, CancelEventArgs e)
        {
            if (dtpkrShipDate_4Hit.Value < DateTime.Today)
            {
                errPOHits.SetError(dtpkrShipDate_4Hit, "Please enter a date greater than  Today");
                e.Cancel = true;
            }
            else
            {
                errPOHits.SetError(dtpkrShipDate_4Hit, "");
                e.Cancel = false;
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
            int icasepackqty = 0;
            int iitemrowindex;
            int ihitcollectionindex;

            // Prevent Datagrid Validation if the user clicked Cancel button
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
                                
                if (!ValidateQuantity(e.FormattedValue.ToString(), icasepackqty))
                {
                    dtgrdViewPOHits_4Hit[e.ColumnIndex, e.RowIndex].ErrorText = "Enter a quantity rounded to the nearest CasePackQty";
                    dtgrdViewPOHits_4Hit.EndEdit();
                    ErrPro.SetControlError(dtgrdViewPOHits_4Hit, "Cell errors");
                    e.Cancel = true;
                }
                else
                {
                    ErrPro.SetControlError(dtgrdViewPOHits_4Hit, "");
                    e.Cancel = false;
                }
            }
        }

        private void dtgrdViewPOHits_4Hit_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (dtgrdViewPOHits_4Hit.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {
                if (_itemquantityrounded > 0)
                {
                    dtgrdViewPOHits_4Hit.Rows[e.RowIndex].Cells["Quantity"].Value = _itemquantityrounded;
                    _itemquantityrounded = 0;
                }
            }
        }
        #endregion

        #region Hit 5
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
                if (_porder.lstpoLineItemDetails[i].IsDeleted == false && _porder.lstpoLineItemDetails[i].IsValid == true)
                {
                    _pohitscollection[3].dtPoHits.Rows.Add(false,
                                                _porder.lstpoLineItemDetails[i].Sequence,
                                                _porder.lstpoLineItemDetails[i].APP1,
                                                _porder.lstpoLineItemDetails[i].ClassCode,
                                                _porder.lstpoLineItemDetails[i].Vendorcode,
                                                _porder.lstpoLineItemDetails[i].Stylecode,
                                                _porder.lstpoLineItemDetails[i].Colorcode,
                                                _porder.lstpoLineItemDetails[i].Itemsize,
                                                _porder.lstpoLineItemDetails[i].Itemlongdescription,
                                                0);
                }
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

        private void dtpkrAnticipateDate_5Hit_ValueChanged(object sender, EventArgs e)
        {
            _pohitscollection[3].AnticipateDate = dtpkrAnticipateDate_5Hit.Value;
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

        private void dtpkrAnticipateDate_5Hit_Validating(object sender, CancelEventArgs e)
        {
            if (dtpkrShipDate_5Hit.Value < DateTime.Today)
            {   
                errPOHits.SetError(dtpkrShipDate_5Hit, "Please enter a date greater than  Today");
                e.Cancel = true;
            }
            else
            {
                errPOHits.SetError(dtpkrShipDate_5Hit, "");
                e.Cancel = false;
            }
        }

        private void dtpkrShipDate_5Hit_ValueChanged(object sender, EventArgs e)
        {
            _pohitscollection[3].ShippingDate = dtpkrShipDate_5Hit.Value;
                        
            //if (_porder.Penvironment.Domain == "SWNA")
            if (DataCache.DisplayPOHitsCancelDateWithDayFormat == true)
            {
                txtCancelDate_5Hit.Text = dtpkrShipDate_5Hit.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate).ToString("D");
                _pohitscollection[3].CancelDate = dtpkrShipDate_5Hit.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate).ToString("D");
            }
            else
            {
                txtCancelDate_5Hit.Text = dtpkrShipDate_5Hit.Value.ToLongDateString();
                _pohitscollection[3].CancelDate = dtpkrShipDate_5Hit.Value.ToLongDateString(); ;
            }

            if ((dtpkrAnticipateDate_5Hit.Value.Date < dtpkrShipDate_5Hit.Value.Date))
            {
                errPOHits.SetError(dtpkrShipDate_5Hit, "Shipping date cannot be after the anticipate date");
            }
            else
            {
                errPOHits.SetError(dtpkrShipDate_5Hit, "");

            }
            
        }

        private void dtpkrShipDate_5Hit_Validating(object sender, CancelEventArgs e)
        {
            if (dtpkrShipDate_5Hit.Value < DateTime.Today)
            {
                errPOHits.SetError(dtpkrShipDate_5Hit, "Please enter a date greater than  Today");
                e.Cancel = true;
            }
            else
            {   
                errPOHits.SetError(dtpkrShipDate_5Hit, "");
                e.Cancel = false;
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
            int icasepackqty = 0;
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
                                
                if (!ValidateQuantity(e.FormattedValue.ToString(), icasepackqty))
                {
                    dtgrdViewPOHits_5Hit[e.ColumnIndex, e.RowIndex].ErrorText = "Enter a quantity rounded to the nearest CasePackQty";
                    dtgrdViewPOHits_5Hit.EndEdit();
                    ErrPro.SetControlError(dtgrdViewPOHits_5Hit, "Cell errors");
                    e.Cancel = true;
                }
                else
                {
                    ErrPro.SetControlError(dtgrdViewPOHits_5Hit, "");
                    e.Cancel = false;
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
        #endregion

        #region Hit 6
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
            // Clear the Items on Hit 6
            _pohitscollection[4].dtPoHits.Clear();

            // Add again
            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                if (_porder.lstpoLineItemDetails[i].IsDeleted == false && _porder.lstpoLineItemDetails[i].IsValid == true)
                {
                    _pohitscollection[4].dtPoHits.Rows.Add(false,
                                                _porder.lstpoLineItemDetails[i].Sequence,
                                                _porder.lstpoLineItemDetails[i].APP1,
                                                _porder.lstpoLineItemDetails[i].ClassCode,
                                                _porder.lstpoLineItemDetails[i].Vendorcode,
                                                _porder.lstpoLineItemDetails[i].Stylecode,
                                                _porder.lstpoLineItemDetails[i].Colorcode,
                                                _porder.lstpoLineItemDetails[i].Itemsize,
                                                _porder.lstpoLineItemDetails[i].Itemlongdescription,
                                                0);
                }
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

        private void dtpkrAnticipateDate_6Hit_ValueChanged(object sender, EventArgs e)
        {
            _pohitscollection[4].AnticipateDate = dtpkrAnticipateDate_6Hit.Value;
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

        private void dtpkrAnticipateDate_6Hit_Validating(object sender, CancelEventArgs e)
        {
            if (dtpkrShipDate_6Hit.Value < DateTime.Today)
            {
                errPOHits.SetError(dtpkrShipDate_6Hit, "Please enter a date greater than  Today");
                e.Cancel = true;
            }
            else
            {
                errPOHits.SetError(dtpkrShipDate_6Hit, "");
                e.Cancel = false;
            }
            
        }

        private void dtpkrShipDate_6Hit_ValueChanged(object sender, EventArgs e)
        {
            _pohitscollection[4].ShippingDate = dtpkrShipDate_6Hit.Value;
                        
            //if (_porder.Penvironment.Domain == "SWNA")
            if (DataCache.DisplayPOHitsCancelDateWithDayFormat == true)
            {
                txtCancelDate_6Hit.Text = dtpkrShipDate_6Hit.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate).ToString("D");
                _pohitscollection[4].CancelDate = dtpkrShipDate_6Hit.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate).ToString("D");
            }
            else
            {
                txtCancelDate_6Hit.Text = dtpkrShipDate_6Hit.Value.ToLongDateString();
                _pohitscollection[4].CancelDate = dtpkrShipDate_6Hit.Value.ToLongDateString(); ;
            }

            if ((dtpkrAnticipateDate_6Hit.Value.Date < dtpkrShipDate_6Hit.Value.Date))
            {
                errPOHits.SetError(dtpkrShipDate_6Hit, "Shipping date cannot be after the anticipate date");
            }
            else
            {
                errPOHits.SetError(dtpkrShipDate_6Hit, "");
            }
            
        }

        private void dtpkrShipDate_6Hit_Validating(object sender, CancelEventArgs e)
        {
            if (dtpkrShipDate_6Hit.Value < DateTime.Today)
            {
                errPOHits.SetError(dtpkrShipDate_6Hit, "Please enter a date greater than  Today");
                e.Cancel = true;
            }
            else
            {
                errPOHits.SetError(dtpkrShipDate_6Hit, "");
                e.Cancel = false;
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
            int icasepackqty = 0;
            int iitemrowindex;
            int ihitcollectionindex;

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

            if (dtgrdViewPOHits_6Hit.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {
                string svalue;
                int itemqtyinput;

                // A quantity of 0 is a valid quantity. So no validation
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
                                
                if (!ValidateQuantity(e.FormattedValue.ToString(), icasepackqty))
                {
                    dtgrdViewPOHits_6Hit[e.ColumnIndex, e.RowIndex].ErrorText = "Enter a quantity rounded to the nearest CasePackQty";
                    dtgrdViewPOHits_6Hit.EndEdit();
                    ErrPro.SetControlError(dtgrdViewPOHits_6Hit, "Cell errors");
                    e.Cancel = true;
                }
                else
                {
                    ErrPro.SetControlError(dtgrdViewPOHits_6Hit, "");
                    e.Cancel = false;
                }
            }
        }

        private void dtgrdViewPOHits_6Hit_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (dtgrdViewPOHits_6Hit.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {
                if (_itemquantityrounded > 0)
                {
                    dtgrdViewPOHits_6Hit.Rows[e.RowIndex].Cells["Quantity"].Value = _itemquantityrounded;
                    _itemquantityrounded = 0;
                }
            }
        }

        #endregion

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx"); 
        }

        // Refresh the items in the Hits item list
        private void RefreshItems(int iHitNumber)
        {
            int     itemindex;
            short   classcode;
            int     vendor;
            short   style;
            short   color;
            short   size;
            int     qty;

            DataTable dtPoHitsOriginal;

            int ihitscollectionindex;

            ihitscollectionindex = GetHitsCollectionIndexForSpecificHit(iHitNumber);

            dtPoHitsOriginal = _pohitscollection[ihitscollectionindex].dtPoHits.Copy();

            // HK : 16-01-2010 : Fix Bug 219 :
            // Make a copy if there are row in the Hits table (dtHits)
            //if (dtPoHitsOriginal.Rows.Count > 0)
            //{
                _pohitscollection[ihitscollectionindex].CreateCopyOfPoHit();
            //}

            // Clear the datatable associated with this Hit number
            _pohitscollection[ihitscollectionindex].dtPoHits.Clear();

            // Now reinsert the records as if we are 'Reset'ing the Hit
            for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
            {
                if (_porder.lstpoLineItemDetails[i].IsDeleted == false && _porder.lstpoLineItemDetails[i].IsValid == true)
                {
                    itemindex = _porder.lstpoLineItemDetails[i].Sequence;
                    classcode = _porder.lstpoLineItemDetails[i].ClassCode;
                    vendor = _porder.lstpoLineItemDetails[i].Vendorcode;
                    style = _porder.lstpoLineItemDetails[i].Stylecode;
                    color = _porder.lstpoLineItemDetails[i].Colorcode;
                    size = _porder.lstpoLineItemDetails[i].Itemsize;

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
                                                            _porder.lstpoLineItemDetails[i].Sequence,
                                                            _porder.lstpoLineItemDetails[i].APP1,
                                                            _porder.lstpoLineItemDetails[i].ClassCode,
                                                            _porder.lstpoLineItemDetails[i].Vendorcode,
                                                            _porder.lstpoLineItemDetails[i].Stylecode,
                                                            _porder.lstpoLineItemDetails[i].Colorcode,
                                                            _porder.lstpoLineItemDetails[i].Itemsize,
                                                            _porder.lstpoLineItemDetails[i].Itemlongdescription,
                                                            qty);
                }
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

        private void button1_Click(object sender, EventArgs e)
        {
            _pohitscollection[0].dtPoHits.Clear();
        }

        private void POHitsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            btnOK.Select();
        }
    }
}