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
using Disney.DA.IP400;
using Disney.Menu;

namespace Disney.Spice.POUI
{
    public partial class POEntryForm1 : Form
    {
        #region Class Variables
        private PurchaseOrder _porder;
        private Enquiry _vendorlookup;
        private Enquiry _deptlookup;
        private LookupBO lookupbo;
        private Validation validationcls;
        private POItemDetails _polinedetails;
        private POLineDetails polineform;
        private ProgressWindow pwindow;

        private Boolean _bFormCancelClicked;
        private Boolean _bUserWantsToDeleteLine;
        private Boolean _bDuplicateItem;
        private Boolean IsItemNumberChanged;

        private DataTable _dtCurrency;
        private DataTable dtSelectedStores;
        private DataTable dtSSD;
        private DataTable dtFreight = new DataTable("Freight");
        private DataTable dtDropShipMatrix;
        private DataView MarketDCDV;

        private decimal _ccyratemarket;
        private decimal _ccyrateprev;
        private int gridcollectionindex;
        private decimal TotalCost;
        private decimal TotalConvertedCost;
        private decimal TotalComponentCvtCost;
        private decimal TotalComponentCost;
        private decimal TotalComponentRetail;
        private decimal Exchangerate;
        private int NextSequence;

        private DataGridViewCellStyle dgvcsPoLinesnormal;
        private DataGridViewCellStyle dgvcsPoLinesalternate;

        private PurchaseOrder.PoHitsCollection _pohitscollection;

        private string _defaultshipvia = String.Empty;
        private string _sStoreColumnNamePrefix = "Store_";

        private Form _mdiparent;

        private const int MINLANDINGVALUEFOROCN = 1;
        private const string OCEANSHIPVIACODE = "OCN";
        private const string MAGICDCSTOREVATCODE = "A";
        #endregion

        #region Constructors

        public POEntryForm1(PurchaseOrder purchaseorder, Form owner)
        {
            InitializeComponent();
            _porder = purchaseorder;
            
            _mdiparent = owner;
            this.MdiParent = owner;

            SetupInitialValues();
        }

        #endregion

        #region Class methods

        #region Summary
        private void InitalisePOsummary()
        {
            txtNumberofHits.Text = "0".ToString();
            txtNumberofDrops.Text = "0".ToString();
            txtPOLines.Text = "0".ToString();
            txtPOPacks.Text = "0".ToString();

            txtTotalUnits.Text = "0".ToString();
            txtTotalCost.Text = "0.00".ToString();
            txtTotalRetail.Text = "0.00".ToString();
            txtMarginValue.Text = "0.00".ToString();
            txtTotalRetailExVat.Text = "0.00".ToString();
            txtMarginPercent.Text = "0.00".ToString();
        }

        private void DisplayPOSummaryNA()
        {
            // Blank the Po Summary Fields
            txtTotalUnits.Text = "N/A";
            txtTotalCost.Text = "N/A";
            txtTotalRetail.Text = "N/A";
            txtMarginValue.Text = "N/A";
            txtMarginPercent.Text = "N/A";
            txtTotalRetailExVat.Text = "N/A";
            lblCurrValue.Text = "";
            lblCurrVal1.Text = "";
            validationcls.HighlightErrControls(lblMarginValue, txtMarginValue, true);
        }

        private void CalculatePOsummary()
        {
            decimal totalretailexvat = 0;
            decimal marginvalue = 0;

            _porder.TotalRetail = _porder.CalculateTotalRetail();
            _porder.TotalCost = _porder.CalculateTotalCost();
            _porder.TotalUnits = _porder.CalculateTotalUnit();
            _porder.TotalCostPOCurr = _porder.CalculateTotalCostInPoCurrency();

            if (_porder.Landing == 0)
            {
                _porder.Landing = 1;
            }

            _porder.TotalLandedCost = Decimal.Round(_porder.CalculateTotalLandedCost(), 2);

            totalretailexvat = Decimal.Round(_porder.CalculateTotalRetailExVat(MAGICDCSTOREVATCODE), 2);


            if (totalretailexvat == 0)
            {
                if (_porder.TotalRetail != 0)
                {
                    marginvalue = _porder.TotalRetail - _porder.TotalLandedCost;
                    _porder.MarginValue = marginvalue;
                    _porder.MarginPercentage = Decimal.Round(marginvalue / _porder.TotalRetail * 100, 2);
                }
                else
                {
                    _porder.MarginValue = 0;
                    _porder.MarginPercentage = 0;
                }
            }
            else
            {
                marginvalue = totalretailexvat - _porder.TotalLandedCost;
                _porder.MarginValue = marginvalue;
                _porder.MarginPercentage = Decimal.Round(marginvalue / totalretailexvat * 100, 2);
            }

            UpdatePoLinesPacks();


            if (_porder.MarginValue < 0 && rdBtnDropShipMatrix.Checked == false)
            {
                validationcls.HighlightErrControls(lblMarginValue, txtMarginValue, false);
            }
            else
            {
                validationcls.HighlightErrControls(lblMarginValue, txtMarginValue, true);
            }

            //Only Display Values if Not Drop Ship Matrix 
            if (rdBtnDropShipMatrix.Checked == false)
            {
                txtTotalCost.Text = _porder.TotalLandedCost.ToString("N");
                txtTotalUnits.Text = _porder.TotalUnits.ToString("G");
                txtTotalRetailExVat.Text = totalretailexvat.ToString("N");
                txtTotalRetail.Text = Decimal.Round(_porder.TotalRetail, 2).ToString("N");
                txtMarginValue.Text = Decimal.Round(_porder.MarginValue, 2).ToString("N");
                txtMarginPercent.Text = _porder.MarginPercentage.ToString("N");
            }
            else
            {
                DisplayPOSummaryNA();
            }

            UpdateDrops();
        }

        private void CalculateAPPComponentCvtCost()
        {
            TotalConvertedCost = 0;

            for (int iCount = 0; iCount < _polinedetails.Components.Count; iCount++)
            {
                _polinedetails.Components[iCount].Cost = Decimal.Round(_polinedetails.Components[iCount].UnConvertedCost / _porder.ExchangeRate, 2);
                TotalConvertedCost += _polinedetails.Components[iCount].Cost * _polinedetails.Components[iCount].RatioQuantity;
            }
        }

        private void CalculateAPPComponentCost()
        {
            TotalCost = 0;
            Decimal cost = 0;

            for (int iCount = 0; iCount < _polinedetails.Components.Count; iCount++)
            {
                cost = Decimal.Round(_polinedetails.Components[iCount].UnConvertedCost, 2);

                TotalCost += cost * _polinedetails.Components[iCount].RatioQuantity;
            }
        }
        #endregion

        #region Shipping

        private void UpdateDrops()
        {
            if (rdBtnDropShipMatrix.Checked)
            {
                txtNumberofDrops.Text = GetDrops().ToString();
            }

            if (dtSelectedStores != null && dtSelectedStores.Rows.Count != 0)
            {
                if (rdBtnDropShipSingle.Checked)
                {
                    txtNumberofDrops.Text = dtSelectedStores.Rows.Count.ToString();
                }
            }

            if (rdBtnDCPO.Checked)
            {
                if (_porder.lstpoLineItemDetails.Count == 0)
                {
                    txtNumberofDrops.Text = "0";
                }
                else
                {
                    foreach (POItemDetails item in _porder.lstpoLineItemDetails)
                    {
                        if (item.IsDeleted == false && item.IsValid == true)
                        {
                            txtNumberofDrops.Text = "1";
                            return;
                        }

                        txtNumberofDrops.Text = "0";
                    }
                }
            }
        }

        private int GetDrops()
        {
            string sStore = String.Empty;
            int istorequantity = 0;
            int drops = 0;

            if (dtSelectedStores != null && dtSelectedStores.Rows.Count != 0)
            {
                foreach (DataRow dtrow in dtSelectedStores.Rows)
                {
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
            }

            return drops;
        }

        private DataTable GetEmptyStores()
        {
            DataTable dtStores = new DataTable();

            dtStores.Columns.Add("clmSelected", typeof(bool));
            dtStores.Columns.Add("clmStore", typeof(string));
            dtStores.Columns.Add("clmStoreName", typeof(string));

            return dtStores;
        }
        #endregion

        #region Other
        private void PopulateSSD()
        {
            cmbSSD.Items.Add(string.Empty);

            dtSSD = lookupbo.PopulateSSD();
            foreach (DataRow drow in dtSSD.Rows)
            {
                cmbSSD.Items.Add(drow["clmSSDDATMDY"].ToString());
            }
            cmbSSD.SelectedIndex = 0;

            DataColumn[] key = new DataColumn[] { dtSSD.Columns["clmSSDDATMDY"] };
            dtSSD.PrimaryKey = key;

            this.cmbSSD.SelectedIndexChanged += new System.EventHandler(this.cmbSSD_SelectedIndexChanged);
        }

        private void SetupInitialValues()
        {
            lblSSD.Visible = false;
            cmbSSD.Visible = false;

            lookupbo = new LookupBO(_porder.DbParamRef, _porder.UserName, _porder.Penvironment);

            validationcls = new Validation(_porder.DbParamRef, _porder.UserName, _porder.Penvironment);

            //Get the Domain Market
            String DomainMarket = _porder.DomainMarket;

            _porder.Landing = 1;

            dtpkrAnticipateDate.CustomFormat = DateFormats.PO_AnticipateDate;
            dtpkrShipDate.CustomFormat = DateFormats.PO_ShipDate;

            dtpkrAnticipateDate.Value = DateTime.Now;
            dtpkrShipDate.Value = DateTime.Now;

            txtOrderDate.Text = DateTime.Now.ToString(DateFormats.PO_OrderDate);
            txtCancelDate.Text = dtpkrShipDate.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate).ToString(DateFormats.PO_CancelDate);
            _porder.CancelDate = dtpkrShipDate.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate);


            lblMarketValue.Text = _porder.DefaultMarket + "-" + _porder.MarketDescription;

            if (_porder.DefaultMarket != _porder.DomainMarket)
            {
                rdBtnDCPO.Checked = false;
                rdBtnDCPO.Enabled = false;
                cmbShipTo.Enabled = false;
                rdBtnDropShipSingle.Checked = true;
            }

            if (_porder.DefaultMarket == _porder.DomainMarket)
            {
                MarketDCDV = LookUpDefaultDC.GetDefaultDCDV(_porder.Penvironment, _porder.DefaultMarket);

                foreach (DataRowView drow in MarketDCDV)
                {
                    cmbShipTo.Items.Add(drow["DCCode"].ToString());
                }

                cmbShipTo.SelectedItem = cmbShipTo.Items[0];

                this.cmbShipTo.SelectedIndexChanged += new System.EventHandler(this.cmbShipTo_SelectedIndexChanged);

                dtSelectedStores = GetEmptyStores();

                foreach (DataRowView drow in MarketDCDV)
                {
                    if (drow["DCCode"].ToString() == _porder.ShipTo.ToString())
                    {
                        dtSelectedStores.Rows.Add(true, drow["DCCode"].ToString(), "Distribution Centre");
                    }
                    else
                    {
                        dtSelectedStores.Rows.Add(false, drow["DCCode"].ToString(), "Distribution Centre");
                    }
                }

                DataView dvSelectedStores = new DataView(dtSelectedStores);
                StringBuilder searchexpression = new StringBuilder();
                searchexpression.Append("clmSelected ='True'");

                dvSelectedStores.RowFilter = searchexpression.ToString();

                btnStores.Enabled = false;
            }
                       
            //if (_porder.Penvironment.Domain == "SWNA")
            if (DataCache.AreStageSetDatesChangeable == true)
            {
                lblSSD.Visible = true;
                cmbSSD.Visible = true;
                cmbSSD.Enabled = true;
                PopulateSSD();

                txtTotalRetailExVat.Visible = false;
                lblTotalRetailExVat.Visible = false;

                chkNewLineSelection.Visible = true;

                // Freight
                DataColumn dc = new DataColumn("FreightId", typeof(string));
                dtFreight.Columns.Add(dc);
                dc = new DataColumn("FreightDesc", typeof(string));
                dtFreight.Columns.Add(dc);

                dtFreight.Rows.Add("C", "Collect");
                dtFreight.Rows.Add("P", "Pre Pay");

                cbxFreight.DisplayMember = "FreightDesc";
                cbxFreight.ValueMember = "FreightId";

                cbxFreight.DataSource = dtFreight;

                cbxFreight.SelectedIndex = 0;

                lblFreightCharges.Visible = true;
                cbxFreight.Visible = true;
            }

            _porder.PoType = PurchaseOrder.POtype.StandardDCPO;
            _porder.PurchaseOrderType = PurchaseOrder.POtype.StandardDCPO;

            txtLanding.Enabled = true;
            txtPortofEntry.Enabled = false;
            txtPortofDeparture.Enabled = false;
            txtDelTerms.Enabled = false;
            pctBxPortofDeparture.Visible = false;
            pctBxPortofEntry.Visible = false;
            pctBxDelTerms.Visible = false;

            txtDept.Focus();

            _porder.NumofPOLines = 0;

            _ccyratemarket = validationcls.GetCurrency(_porder.MarketCurrency);

            dgvcsPoLinesnormal = new DataGridViewCellStyle(dgvPOlines.DefaultCellStyle);
            dgvcsPoLinesalternate = new DataGridViewCellStyle(dgvPOlines.AlternatingRowsDefaultCellStyle);

            dtDropShipMatrix = new DataTable();

            dtDropShipMatrix.Columns.Add("ItemIndex", typeof(String));
            dtDropShipMatrix.Columns.Add("Pack", typeof(String));
            dtDropShipMatrix.Columns.Add("Class", typeof(String));
            dtDropShipMatrix.Columns.Add("Vendor", typeof(String));
            dtDropShipMatrix.Columns.Add("Style", typeof(String));
            dtDropShipMatrix.Columns.Add("Color", typeof(String));
            dtDropShipMatrix.Columns.Add("Size", typeof(String));
            dtDropShipMatrix.Columns.Add("Description", typeof(String));
            dtDropShipMatrix.Columns.Add("Quantity", typeof(String));
            dtDropShipMatrix.Columns.Add("QuantityAssigned", typeof(String));

            btnHits.Enabled = true;
            _pohitscollection = new PurchaseOrder.PoHitsCollection();

            for (int i = 0; i <= 4; i++)
            {
                PurchaseOrder.POHits pohits;
                pohits = new PurchaseOrder.POHits();

                pohits.HitNUmber = i + 2;

                _pohitscollection.Insert(i, pohits);
            }

            dgvPOlines.Enabled = false;

            InitalisePOsummary();
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


        private Boolean CheckRequiredFields()
        {
            Boolean bSuccess = true;

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

            if (!(  (txtShipVia.Text == "HKU" || txtShipVia.Text == "BLP" || txtShipVia.Text == "ROD") && DataCache.IsLandingFactorRequiredForRoadDelivery == false))
            {
                if (String.IsNullOrEmpty(txtLanding.Text))
                {
                    errPOEntry.SetError(txtLanding, "Enter a valid landing code");
                    validationcls.HighlightErrControls(lblLanding, txtLanding, false);
                    bSuccess = false;
                }  
            }

            // Check the Port of Departure field
            if (String.IsNullOrEmpty(txtPortofDeparture.Text) && (txtShipVia.Text == "OCN") ||
                (String.IsNullOrEmpty(txtPortofDeparture.Text) && (txtShipVia.Text == "AIR")))
            {
                errPOEntry.SetError(txtPortofDeparture, "Enter a port of departure code");
                validationcls.HighlightErrControls(lblPortofDeparture, txtPortofDeparture, false);
                bSuccess = false;
            }

            // Check the Port of Port of Entry field
            if (String.IsNullOrEmpty(txtPortofEntry.Text) && (txtShipVia.Text == "OCN") ||
                (String.IsNullOrEmpty(txtPortofEntry.Text) && (txtShipVia.Text == "AIR")))
            {
                errPOEntry.SetError(txtPortofEntry, "Enter a port of entry code");
                validationcls.HighlightErrControls(lblPortofEntry, txtPortofEntry, false);
                bSuccess = false;
            }

            // Check the Delivery Terms field
            if (String.IsNullOrEmpty(txtDelTerms.Text) && (txtShipVia.Text == "OCN") ||
                (String.IsNullOrEmpty(txtDelTerms.Text) && (txtShipVia.Text == "AIR")))
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
            //Not to be used if lines have been entered and deleted.
            //Lines not deleted from this class. Just flagged as deleted.
            if (_porder.lstpoLineItemDetails.Count == 0)
            {
                bSuccess = false;
            }

            return bSuccess;
        }

        private Boolean CheckGridCount()
        {
            short gridcount = 0;
            Boolean bSuccess = true;

            // Check whether the grid has any lines.
            foreach (DataGridViewRow dgvRow in dgvPOlines.Rows)
            {
                if (dgvRow.Cells["Class"].Value != null)
                {
                    gridcount++;
                }
            }

            if (gridcount == 0)
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
                if (item.IsDeleted == false && item.IsValid == true)
                {
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
                iquantity = Convert.ToInt32(dtDropShipMatrix.Rows[i]["Quantity"]);
                iquantityassigned = Convert.ToInt32(dtDropShipMatrix.Rows[i]["QuantityAssigned"]);

                if (iquantity != iquantityassigned)
                {
                    return false;
                }
            }

            return true;
        }

        private int GetInvoicesCountForDroShipMatrix()
        {
            int iquantity;
            int invoicecount = 0;
            string columnname;

            if (dtDropShipMatrix.Rows.Count == 0)
            {
                return 0;
            }

            DataView dvSelectedStores = new DataView(dtSelectedStores);
            StringBuilder searchexpression = new StringBuilder();
            searchexpression.Append("clmSelected ='True'");

            dvSelectedStores.RowFilter = searchexpression.ToString();

            foreach (DataRow dr in dvSelectedStores.Table.Rows)
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
                if (poitem.IsDeleted == false && poitem.IsValid == true)
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
            }

            txtPOLines.Text = numofpolines.ToString();
            txtPOPacks.Text = numofpacks.ToString();
            _porder.NumofPOLines = numofpolines;
            _porder.NumofPOPacks = numofpacks;
        }

        private DataTable PopulatePOLines(int iHitNUmber)
        {
            DataTable dtAllPOLines = new DataTable();

            dtAllPOLines.Columns.Add("POnumber", typeof(string));
            dtAllPOLines.Columns.Add("Version", typeof(Int16));
            dtAllPOLines.Columns.Add("Sequence", typeof(Int16));
            dtAllPOLines.Columns.Add("Class", typeof(Int16));
            dtAllPOLines.Columns.Add("Vendor", typeof(Int32));
            dtAllPOLines.Columns.Add("Style", typeof(Int16));
            dtAllPOLines.Columns.Add("Colour", typeof(Int16));
            dtAllPOLines.Columns.Add("Size", typeof(Int16));
            dtAllPOLines.Columns.Add("SKU", typeof(Int32));
            dtAllPOLines.Columns.Add("SKUCHK", typeof(Int16));
            dtAllPOLines.Columns.Add("UPC", typeof(string));
            dtAllPOLines.Columns.Add("Quantity", typeof(Int32));
            dtAllPOLines.Columns.Add("LandedCost", typeof(decimal));
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
            dtAllPOLines.Columns.Add("Character", typeof(string));

            short LineSequence = 0;
            for (int iCount = 0; iCount < _porder.lstpoLineItemDetails.Count; iCount++)
            {
                _polinedetails = _porder.lstpoLineItemDetails[iCount];
                if (_polinedetails.IsDeleted == false && _polinedetails.IsValid == true)
                {

                    int qty = _porder.GetItemQuantityOnHit(iHitNUmber,
                                                           _polinedetails.Sequence,
                                                           _polinedetails.ClassCode,
                                                           _polinedetails.Vendorcode,
                                                           _polinedetails.Stylecode,
                                                           _polinedetails.Colorcode,
                                                           _polinedetails.Itemsize);

                    _polinedetails.Itemquantity = qty;

                    LineSequence++;
                    dtAllPOLines.Rows.Add(_porder.SpicePOnumber,
                                          _porder.SpicePOversion,
                                          LineSequence,
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
            }

            return dtAllPOLines;
        }

        private DataTable PopulatePOLines()
        {
            DataTable dtAllPOLines = new DataTable();

            dtAllPOLines.Columns.Add("POnumber", typeof(string));
            dtAllPOLines.Columns.Add("Version", typeof(Int16));
            dtAllPOLines.Columns.Add("Sequence", typeof(Int16));
            dtAllPOLines.Columns.Add("Class", typeof(Int16));
            dtAllPOLines.Columns.Add("Vendor", typeof(Int32));
            dtAllPOLines.Columns.Add("Style", typeof(Int16));
            dtAllPOLines.Columns.Add("Colour", typeof(Int16));
            dtAllPOLines.Columns.Add("Size", typeof(Int16));
            dtAllPOLines.Columns.Add("SKU", typeof(Int32));
            dtAllPOLines.Columns.Add("SKUCHK", typeof(Int16));
            dtAllPOLines.Columns.Add("UPC", typeof(string));
            dtAllPOLines.Columns.Add("Quantity", typeof(Int32));
            dtAllPOLines.Columns.Add("LandedCost", typeof(decimal));
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
            dtAllPOLines.Columns.Add("Character", typeof(string));

            short LineSequence = 0;
            for (int iCount = 0; iCount < _porder.lstpoLineItemDetails.Count; iCount++)
            {
                _polinedetails = _porder.lstpoLineItemDetails[iCount];
                if (_polinedetails.IsDeleted == false && _polinedetails.IsValid == true)
                {
                    if (rdBtnDropShipMatrix.Checked)
                    {
                        int qty = _porder.GetItemQuantityForStore(_porder.ShipTo.ToString(),
                                                                          _polinedetails.Sequence,
                                                                          _polinedetails.ClassCode,
                                                                          _polinedetails.Vendorcode,
                                                                          _polinedetails.Stylecode,
                                                                          _polinedetails.Colorcode,
                                                                          _polinedetails.Itemsize);
                        _polinedetails.Itemquantity = qty;
                    }

                    LineSequence++;
                    dtAllPOLines.Rows.Add(_porder.SpicePOnumber,
                                          _porder.SpicePOversion,
                                          LineSequence,
                                          _polinedetails.ClassCode,
                                          _polinedetails.Vendorcode,
                                          _polinedetails.Stylecode,
                                          _polinedetails.Colorcode,
                                          _polinedetails.Itemsize,
                                          _polinedetails.Sku,
                                          _polinedetails.SkuChk,
                                          _polinedetails.UPC,
                                          _polinedetails.Itemquantity,
                                          Math.Round(_polinedetails.LandedCost, 2),
                                          _polinedetails.Retailprice,
                                          _polinedetails.Itemlongdescription,
                                          _polinedetails.Itemshortdescription,
                                          _polinedetails.Vendorstyle,
                                          _polinedetails.SeasonCode,
                                          _polinedetails.Subclass,
                                          _polinedetails.Tickettype,
                                          _polinedetails.CasePackQty,
                                          _polinedetails.DistroQty,
                                          Decimal.Round(_polinedetails.ConvertedCost, 2),
                                          _porder.Landing,
                                          _polinedetails.Charactercode
                                          );
                }
            }

            return dtAllPOLines;
        }

        private void ImportControlChanges()
        {
            switch (txtShipVia.Text)
            {
                case "AIR":
                case OCEANSHIPVIACODE:
                    txtPortofDeparture.Enabled = true;
                    pctBxPortofDeparture.Visible = true;
                    pctBxPortofDeparture.Enabled = true;
                    txtPortofEntry.Enabled = true;
                    pctBxPortofEntry.Visible = true;
                    pctBxPortofEntry.Enabled = true;
                    txtDelTerms.Enabled = true;
                    pctBxDelTerms.Visible = true;
                    pctBxDelTerms.Enabled = true;
                    txtLanding.Enabled = true;
                    txtLanding.Focus();
                    break;
                case "HKU":
                case "BLP":
                case "ROD":
                    txtPortofDeparture.Text = "";
                    txtPortofEntry.Text = "";
                    txtDelTerms.Text = "";
                    txtPortofDeparture.Enabled = false;
                    txtPortofEntry.Enabled = false;
                    txtDelTerms.Enabled = false;
                    pctBxPortofDeparture.Visible = false;
                    pctBxPortofDeparture.Enabled = false;
                    pctBxPortofEntry.Visible = false;
                    pctBxPortofEntry.Enabled = false;
                    pctBxDelTerms.Visible = false;
                    pctBxDelTerms.Enabled = false;
                    lblDeparturePortDesc.Text = "";
                    lblEntryPortDesc.Text = "";
                    lblDeliveryTermsDesc.Text = "";
                    _porder.Portofdeparturecode = 0;
                    _porder.Portofentrycode = 0;
                    _porder.Deltermscode = String.Empty;

                    if (DataCache.IsLandingFactorRequiredForRoadDelivery == true)
                    {
                        txtLanding.Enabled = true;
                        txtLanding.Focus();
                    }
                    else
                    {
                        txtLanding.Enabled = false;
                    }

                    break;
                default:                                        
                    txtPortofDeparture.Text = "";
                    txtPortofEntry.Text = "";
                    txtDelTerms.Text = "";
                    txtPortofDeparture.Enabled = false;
                    pctBxPortofDeparture.Visible = false;
                    pctBxPortofDeparture.Enabled = false;
                    txtPortofEntry.Enabled = false;
                    pctBxPortofEntry.Visible = false;
                    pctBxPortofEntry.Enabled = false;
                    txtDelTerms.Enabled = false;
                    pctBxDelTerms.Visible = false;
                    pctBxDelTerms.Enabled = false;                   
                    break;
            }
        }

        private short GetNextCollectionIndex()
        {
            Int32 totalRowsInItemsCollection = _porder.lstpoLineItemDetails.Count;

            if (totalRowsInItemsCollection == 0)
            {
                return 0;
            }
            else
            {
                for (int iCount = 0; iCount < _porder.lstpoLineItemDetails.Count; iCount++)
                {
                    NextSequence = _porder.lstpoLineItemDetails[iCount].Sequence;
                }
                return Convert.ToInt16(NextSequence + 1);
            }
        }

        private int SumStoreQuantity(string sstore)
        {
            int isummedquantity = 0;
            string sstorecolumnname;

            // Convert passed store (sstore) to a valid store column name
            sstorecolumnname = GetStoreColumnNameFromStore(sstore);

            for (int i = 0; i < dtDropShipMatrix.Rows.Count; i++)
            {
                isummedquantity = isummedquantity + Convert.ToInt32(dtDropShipMatrix.Rows[i][sstorecolumnname]);
            }

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

        private Boolean CheckForDuplicateLine(int currentrow, short classcode, int vendor, short style, short colour, short size)
        {
            if (dgvPOlines.Rows.Count > 1)
            {
                for (int index = 0; index < _porder.lstpoLineItemDetails.Count; index++)
                {
                    if (_porder.lstpoLineItemDetails[index].IsValid && (index != currentrow))
                    {
                        if (_porder.lstpoLineItemDetails[index].ClassCode == classcode
                            && _porder.lstpoLineItemDetails[index].Vendorcode == vendor
                            && _porder.lstpoLineItemDetails[index].Stylecode == style
                            && _porder.lstpoLineItemDetails[index].Colorcode == colour
                            && _porder.lstpoLineItemDetails[index].Itemsize == size
                            && _porder.lstpoLineItemDetails[index].IsDeleted == false
                            && _porder.lstpoLineItemDetails[index].IsValid == true)
                        {
                            return true;
                        }

                    }
                }
            }

            return false;
        }

        private void AddItemsControler(DataTable dtSelectedItems)
        {
            if (dtSelectedItems.Rows.Count > 0)
            {
                int iStartRow = dgvPOlines.Rows.Count;

                foreach (DataRow dr in dtSelectedItems.Rows)
                {
                    POItemDetails poitem = new POItemDetails();
                    poitem.ClassCode = (short)dr["class"];
                    poitem.Vendorcode = (int)dr["vendor"];
                    poitem.Stylecode = (short)dr["style"];
                    poitem.Colorcode = (short)dr["colour"];
                    poitem.Itemsize = (short)dr["size"];
                    poitem.Sequence = GetNextCollectionIndex();

                    ParameterizedThreadStart job = new ParameterizedThreadStart(ItemLookUp);
                    Thread myThread = new Thread(job);

                    myThreadParms mythreadparms = new myThreadParms(iStartRow, poitem);
                    myThread.Start(mythreadparms);
                }
            }
        }

        private void ItemLookUp(object myThreadParms)
        {
            myThreadParms mythreadparms = myThreadParms as myThreadParms;

            int rowindex = mythreadparms.RowIndex;
            POItemDetails item = mythreadparms.PoItem;

            object[] o = (object[])myThreadParms;

            Boolean bSuccess = item.ItemLookup(_porder.DbParamRef, _porder.UserName,
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
                dgvPOlines.Rows[rowindex].Cells["Pack"].Value = item.APP1;

                dgvPOlines.Rows[rowindex].Cells["ConvertedCost"].Value = Decimal.Round((item.Cost * _ccyratemarket) / _porder.ExchangeRate, 2).ToString("#.00");
                dgvPOlines.Rows[rowindex].Cells["ConvertedCost6"].Value = Decimal.Round((item.Cost * _ccyratemarket) / _porder.ExchangeRate, 6);
                dgvPOlines.Rows[rowindex].Cells["SavedConvertedCost6"].Value = Decimal.Round(_polinedetails.ConvertedCost, 2);

                if (item.APP1 == "Y")
                {
                    dgvPOlines.Rows[rowindex].Cells["Cost"].ReadOnly = true;
                }
                else
                {
                    dgvPOlines.Rows[rowindex].Cells["Cost"].ReadOnly = false;
                }

                _porder.lstpoLineItemDetails.Add(item);
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

        private void SetItemColor(int columnindex, int rowindex, Color color)
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
                if (txtDept.Text.Trim().Length == 0)
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
                if (!String.IsNullOrEmpty(txtShipVia.Text))
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


                dgvPOlines.Enabled = true;

                // Can Add item only after Header is valid
                btnAddItem.Enabled = true;
                dtpkrAnticipateDate.Focus();

                return true;
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
                                
                //if (_porder.Penvironment.Domain == "SWNA")
                if (DataCache.AreStageSetDatesChangeable == true)
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

        #endregion 

        #endregion

        #region Form and Control Events

        #region POHeader
        private void pctBoxVendor_Click(object sender, EventArgs e)
        {
            errPOEntry.SetError(txtVendor, String.Empty);

            DataSet dsVendors = lookupbo.VendorLookup();

            if (_vendorlookup == null) _vendorlookup = new Enquiry(dsVendors.Tables["FilteredVendors"], "Vendors");

            _vendorlookup.ShowGrid();

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
                        lblVendorDesc.Text = String.Empty;
                        lblTermsDesc.Text = String.Empty;
                        txtTerms.Text = String.Empty;
                        validationcls.HighlightErrControls(lblVendor, txtVendor, false);
                        errPOEntry.SetError(txtVendor, "Enter a valid Vendor code");

                        e.Cancel = true;
                        _porder.Vendorcode = 0;
                    }
                    else
                    {
                        lblVendorDesc.Text = lstReturn[1];

                        List<string> lstretvalues_1 = new List<string>();

                        lstretvalues_1 = validationcls.ValidateShipVia(lstReturn[2]);

                        if (lstretvalues_1[0] == "True")
                        {
                            if (_defaultshipvia != lstretvalues_1[1])
                            {
                                lblShipViaDesc.Text = lstretvalues_1[1];
                                _defaultshipvia = lstretvalues_1[1];

                                txtShipVia.Text = lstReturn[2];
                            }

                            errPOEntry.SetError(txtShipVia, "");
                            validationcls.HighlightErrControls(lblShipVia, txtShipVia, true);
                            e.Cancel = false;
                            _porder.ShipViaCode = txtShipVia.Text;
                        }


                        txtTerms.Text = lstReturn[3];
                        _porder.Termscode = lstReturn[3];

                        if (lstReturn[4].Length != 0)
                        {
                            txtCurrency.Text = lstReturn[4];
                        }

                        lblTermsDesc.Text = lstReturn[5];
                        validationcls.HighlightErrControls(lblVendor, txtVendor, true);
                        errPOEntry.SetError(txtVendor, String.Empty);
                        e.Cancel = false;
                        _porder.Vendorcode = Int32.Parse(txtVendor.Text);
                    }
                }

                catch (System.Exception)
                {
                    validationcls.HighlightErrControls(lblVendor, txtVendor, false);
                    errPOEntry.SetError(txtVendor, "Enter a valid Vendor Code");
                    lblVendorDesc.Text = String.Empty;
                    lblTermsDesc.Text = String.Empty;
                    txtTerms.Text = String.Empty;
                    txtVendor.Focus();
                }
            }
        }

        private void pctBoxDept_Click(object sender, EventArgs e)
        {
            string[] strretvalues;

            errPOEntry.SetError(txtDept, String.Empty);

            DataTable dtAuthorized = lookupbo.DepartmentLookup();

            _deptlookup = new Enquiry(dtAuthorized, "DepartmentLookup");

            _deptlookup.ShowGrid();

            strretvalues = _deptlookup.SelectedValue;

            if (strretvalues != null)
            {
                strretvalues = _deptlookup.SelectedValue;

                txtDept.Text = strretvalues[0];
                lblDeptDesc.Text = strretvalues[1];
                errPOEntry.SetError(txtDept, String.Empty);
                txtDept.Focus();
                validationcls.HighlightErrControls(lblDept, txtDept, true);
            }
            else
            {
                txtDept.Focus();
            }
        }

        private void txtDept_Validating(object sender, CancelEventArgs e)
        {
            if (!_bFormCancelClicked)
            {
                try
                {
                    List<string> lstReturn = new List<string>();

                    lstReturn = validationcls.ValidateDeptCode(txtDept.Text);

                    if (lstReturn[0].Trim().Length == 0)
                    {
                        txtDept.Clear();
                        lblDeptDesc.Text = String.Empty;
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
                    lblDeptDesc.Text = String.Empty;
                    errPOEntry.SetError(txtDept, "Enter a valid dept code");
                    validationcls.HighlightErrControls(lblDept, txtVendor, false);
                    e.Cancel = true;
                }

                //Check Class/Dept again in case user has re-input the department.
                if (CheckItemCount())
                {
                    foreach (DataGridViewRow dgvRow in dgvPOlines.Rows)
                    {
                        if (dgvRow.Cells["Class"].Value != null)
                        {
                            List<string> retValues = validationcls.ValidateClass(dgvRow.Cells["Class"].Value.ToString(), txtDept.Text);
                            if ("False" == retValues[0])
                            {
                                dgvPOlines.Rows[dgvRow.Index].ErrorText = "Please enter a valid Class";
                            }
                            else
                            {
                                dgvPOlines.Rows[dgvRow.Index].ErrorText = String.Empty;
                            }
                        }
                    }
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
                errPOEntry.SetError(txtCurrency, String.Empty);
                validationcls.HighlightErrControls(lblCurrency, txtCurrency, true);
                grpBxPOHeader.Focus();
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
                try
                {
                    List<string> lstReturn = new List<string>();
                    List<string> lstReturn2 = new List<string>();

                    lstReturn = validationcls.CurrencyValidate(txtCurrency.Text, _porder.MarketCurrency);
                    lstReturn2 = validationcls.CurrencyValidate(_porder.MarketCurrency, _porder.MarketCurrency);

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

                        if (_porder.DomainMarket == _porder.DefaultMarket)
                        {
                            lblCurrVal1.Text = "(" + _porder.MarketCurrency + ")";
                            lblCurrValue.Text = "(" + _porder.MarketCurrency + ")";

                            _porder.Currencycode = txtCurrency.Text;
                            _porder.ExchangeRate = Decimal.Parse(lstReturn[2]);

                            if (_porder.ExchangeRate.Equals(_ccyratemarket))
                            {
                                _porder.ExchangeRate = 1.000000m;
                            }

                            lblCurrencyDesc.Text = lstReturn[1] + " (" + _porder.ExchangeRate + ")";
                        }
                        else
                        {
                            lblCurrVal1.Text = "(" + _porder.MarketCurrency + ")";
                            lblCurrValue.Text = "(" + _porder.MarketCurrency + ")";

                            _porder.Currencycode = txtCurrency.Text;

                            if (_porder.BaseCurrency != _porder.Currencycode)
                            {
                                Exchangerate = 1 / (Decimal.Parse(lstReturn2[2]) / Decimal.Parse(lstReturn[2]));
                            }
                            else
                            {
                                Exchangerate = 1 / (Decimal.Parse(lstReturn2[2]));
                            }

                            _porder.ExchangeRate = Exchangerate;

                            Decimal XChgRateDec = Decimal.Round(Exchangerate, 6);

                            String XChgRate = XChgRateDec.ToString();

                            lblCurrencyDesc.Text = lstReturn[1] + " (" + XChgRate + ")";
                        }

                        validationcls.HighlightErrControls(lblCurrency, txtCurrency, true);
                        ValidateHeaderFields(sender);

                        for (int RowNbr = 0; RowNbr < dgvPOlines.Rows.Count - 1; RowNbr++)
                        {
                            short collectionindex = Convert.ToInt16(dgvPOlines["CollectionIndex", RowNbr].Value);
                            _polinedetails = _porder.lstpoLineItemDetails[collectionindex];

                            if (_polinedetails.APP1 == "Y")
                            {
                                CalculateAPPComponentCvtCost();
                                _polinedetails.ConvertedCost = TotalConvertedCost;
                                dgvPOlines["ConvertedCost6", RowNbr].Value = Decimal.Round(TotalConvertedCost, 2);
                            }
                            else
                            {
                                TotalConvertedCost = 0;
                                TotalConvertedCost = Convert.ToDecimal(dgvPOlines["ConvertedCost6", RowNbr].Value);
                                TotalConvertedCost = Decimal.Round((TotalConvertedCost * _ccyrateprev) / _porder.ExchangeRate, 6);
                                _polinedetails.ConvertedCost = TotalConvertedCost;
                                dgvPOlines["ConvertedCost6", RowNbr].Value = TotalConvertedCost;

                            }

                            dgvPOlines["ConvertedCost", RowNbr].Value = Decimal.Round(TotalConvertedCost, 2).ToString("#.00");
                            dgvPOlines["SavedConvertedCost", RowNbr].Value = Decimal.Round(TotalConvertedCost, 2);

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

        private void txtCurrency_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // Implement as part of next release - 8.3.3
                //this.dgvPOlines.Columns["ConvertedCost"].HeaderText = "Cost [" + _porder.Currencycode + "]";
                //this.dgvPOlines.Columns["Retail"].HeaderText = "Retail [" + _porder.MarketCurrency + "]";
            }
            catch (Exception)
            {
                // do nothing here                
            }
        }

        private void txtCurrency_Validated(object sender, EventArgs e)
        {
            if (!_bFormCancelClicked)
            {
                //Currency may change - recalculate summary
                if (_polinedetails != null)
                {
                    _polinedetails.LandedCost = Decimal.Round(_polinedetails.Cost * _porder.Landing, 2);
                    CalculatePOsummary();
                }
            }

            try
            {
                // Implement as part of next release - 8.3.3
                //this.dgvPOlines.Columns["ConvertedCost"].HeaderText = "Cost [" + _porder.Currencycode + "]";
                //this.dgvPOlines.Columns["Retail"].HeaderText = "Retail [" + _porder.MarketCurrency + "]";
            }
            catch (Exception)
            {
                // do nothing here                
            }
                       
        }

        private void txtVendor_Validated(object sender, EventArgs e)
        {
            ImportControlChanges();

            txtCurrency.Focus();
        }
        #endregion POHeader

        #region Other
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
            //Store Selector Lookup 
            frmStoreSelection frmStoreSelection = null;

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

                if (frmStoreSelection.DialogResult == DialogResult.OK)
                {

                    frmStoreSelection = null;

                    // Drop Ship (Matrix) PO 
                    frmDropShipMatrix frmdropshipmatrix = null;


                    //Added due to new multiple Dc's functionality.
                    if (dtSelectedStores.Rows.Count > 0)
                    {
                        foreach (DataRow drow in dtSelectedStores.Rows)
                        {
                            drow["clmSelected"] = "True";
                        }
                    }

                    // If no stores selected then do not show the Drop Ship Matrix form 
                    if (rdBtnDropShipMatrix.Checked && dtSelectedStores.Rows.Count > 0)
                    {
                        // Pass in the Purchase Order class object, any previously drop ship matrix datatable and the selected stores
                        frmdropshipmatrix = new frmDropShipMatrix(_porder, dtDropShipMatrix, dtSelectedStores);
                        frmdropshipmatrix.OnOkButtonClicked += new frmDropShipMatrix.OkButtonClickedEventHandler(frmdropshipmatrix_OnOkButtonClicked);
                        frmdropshipmatrix.OnCancelButtonClicked += new frmDropShipMatrix.CancelButtonClickedEventHandler(frmdropshipmatrix_OnCancelButtonClicked);

                        frmdropshipmatrix.ShowDialog();
                    }

                    if (rdBtnDropShipMatrix.Checked && dtSelectedStores.Rows.Count == 0)
                    {
                        //Delete Rows in Drop Ship Matrix
                        int idx = 0;
                        while (idx < dtDropShipMatrix.Rows.Count)
                        {
                            int curCount = dtDropShipMatrix.Rows.Count;
                            dtDropShipMatrix.Rows[idx].Delete();
                            if (curCount == dtDropShipMatrix.Rows.Count) idx++;
                        }

                        //Delete Store Columns
                        idx = 10;
                        while (idx < dtDropShipMatrix.Columns.Count)
                        {
                            int curCount = dtDropShipMatrix.Columns.Count;
                            dtDropShipMatrix.Columns.RemoveAt(idx);
                            if (curCount == dtDropShipMatrix.Columns.Count) idx++;
                        }

                        //Delete Rows in Drop Ship Matrix
                        idx = 0;
                        while (idx < dtSelectedStores.Rows.Count)
                        {
                            int curCount = dtSelectedStores.Rows.Count;
                            dtSelectedStores.Rows[idx].Delete();
                            if (curCount == dtSelectedStores.Rows.Count) idx++;
                        }

                        dtSelectedStores = null;
                    }

                    frmdropshipmatrix = null;
                }
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

        void frmdropshipmatrix_OnCancelButtonClicked(object sender, frmDropShipMatrix.DropShipMatrixEventArgs e)
        {
            dtDropShipMatrix = e.dtdropdhipmatrix;
        }

        void frmdropshipmatrix_OnOkButtonClicked(object sender, frmDropShipMatrix.DropShipMatrixEventArgs e)
        {
            dtDropShipMatrix = e.dtdropdhipmatrix;
        }

        private void rdBtnDCPO_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdBtnDCPO.Checked)
            {
                dtSelectedStores = GetEmptyStores();

                foreach (DataRowView drow in MarketDCDV)
                {
                    if (drow["DCCode"].ToString() == _porder.ShipTo.ToString())
                    {
                        dtSelectedStores.Rows.Add(true, drow["DCCode"].ToString(), "Distribution Centre");
                    }
                    else
                    {
                        dtSelectedStores.Rows.Add(false, drow["DCCode"].ToString(), "Distribution Centre");
                    }
                }

                _porder.PurchaseOrderType = PurchaseOrder.POtype.StandardDCPO;
                btnStores.Enabled = false;
                btnHits.Enabled = true;
                lblCurrVal1.Text = "(" + _porder.MarketCurrency + ")";
                lblCurrValue.Text = "(" + _porder.MarketCurrency + ")";

                CalculatePOsummary();
                cmbShipTo.Enabled = true;
                rdBCase.Checked = true;
            }
        }

        private void rdBtnDropShipSingle_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnDropShipSingle.Checked)
            {
                _porder.PurchaseOrderType = PurchaseOrder.POtype.DropShipSingle;
                btnStores.Enabled = true;
                btnHits.Enabled = false;

                dtSelectedStores = null;
                lblCurrVal1.Text = "(" + _porder.MarketCurrency + ")";
                lblCurrValue.Text = "(" + _porder.MarketCurrency + ")";

                CalculatePOsummary();
                cmbShipTo.Enabled = false;
                rdBDistro.Checked = true;
                _porder.ShipToMethod = "DS";
                _porder.ShipToRounding = "D";
            }
        }

        private void rdBtnDropShipMatrix_CheckedChanged(object sender, EventArgs e)
        {
            DisplayPOSummaryNA();
            cmbShipTo.Enabled = false;
            rdBDistro.Checked = true;

            btnStores.Enabled = true;

            btnHits.Enabled = false;

            dtSelectedStores = null;

            _porder.PurchaseOrderType = PurchaseOrder.POtype.DropShipMultiple;
            _porder.ShipToMethod = "DM";
            _porder.ShipToRounding = "D";
        }

        private void rdBCase_CheckedChanged(object sender, EventArgs e)
        {
            _porder.ShipToRounding = "C";
        }

        private void rdBDistro_CheckedChanged(object sender, EventArgs e)
        {
            _porder.ShipToRounding = "D";
        }

        private void txtLanding_Validating(object sender, CancelEventArgs e)
        {
            decimal dLanding;

            if (!_bFormCancelClicked)
            {
                if ((Decimal.TryParse(txtLanding.Text, out dLanding)) && (dLanding > 0))
                {
                    errPOEntry.SetError(txtLanding, "");
                    _porder.Landing = dLanding + 1;

                    validationcls.HighlightErrControls(lblLanding, txtLanding, true);
                }
                else
                {
                    if (txtShipVia.Text != "HKU" && txtShipVia.Text != "BLP" && txtShipVia.Text != "ROD")
                    {
                        validationcls.HighlightErrControls(lblLanding, txtLanding, false);
                        errPOEntry.SetError(txtLanding, "Enter a value greater than zero");
                        e.Cancel = true; 
                    }
                }

                // Update Landed Cost values in Grid
                for (int RowNbr = 0; RowNbr < dgvPOlines.Rows.Count - 1; RowNbr++)
                {
                    short collectionindex = Convert.ToInt16(dgvPOlines["CollectionIndex", RowNbr].Value);
                    _polinedetails = _porder.lstpoLineItemDetails[collectionindex];

                    _polinedetails.LandedCost = Decimal.Round(_polinedetails.Cost * _porder.Landing, 2);
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
                validationcls.HighlightErrControls(lblLanding, txtLanding, true);
                errPOEntry.SetError(txtLanding, "");

                List<string> lstretvalues = new List<string>();

                lstretvalues = validationcls.ValidateShipVia(txtShipVia.Text);

                if (lstretvalues[0] == "True")
                {
                    lblShipViaDesc.Text = lstretvalues[1];
                    errPOEntry.SetError(txtShipVia, "");
                    validationcls.HighlightErrControls(lblShipVia, txtShipVia, true);
                    e.Cancel = false;
                    _porder.ShipViaCode = txtShipVia.Text;

                    ValidateHeaderFields(sender);
                }
                else
                {
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
            if (_polinedetails != null)
            {
                _polinedetails.LandedCost = Decimal.Round(_polinedetails.Cost * _porder.Landing, 2);
                CalculatePOsummary();
            }

            ImportControlChanges();
        }

        private void dtpkrAnticipateDate_Validating(object sender, CancelEventArgs e)
        {
            if (_bFormCancelClicked == false)
            {
                errPOEntry.SetError(dtpkrAnticipateDate, string.Empty);

                _porder.AnticipateDate = dtpkrAnticipateDate.Value;
            }
        }

        private void dtpkrShipDate_Validating(object sender, CancelEventArgs e)
        {
            if (_bFormCancelClicked == false)
            {
                errPOEntry.SetError(dtpkrShipDate, "");
                if (dtpkrShipDate.Value < DateTime.Today)
                {
                    errPOEntry.SetError(dtpkrShipDate, "Please enter a date greater than  Today");
                    e.Cancel = true;
                    return;
                }

                //if (_porder.Penvironment.Domain == "SWNA")
                if (DataCache.DisplayPOHitsCancelDateWithDayFormat == true)
                {
                    txtCancelDate.Text = dtpkrShipDate.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate).ToString(DateFormats.PO_CancelDate);
                    _porder.CancelDate = dtpkrShipDate.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate);
                }
                else
                {
                    txtCancelDate.Text = dtpkrShipDate.Value.ToLongDateString();
                    _porder.CancelDate = dtpkrShipDate.Value;
                }
            }
        }

        private void grpBoxDates_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(errPOEntry.GetError(dtpkrAnticipateDate)) & string.IsNullOrEmpty(errPOEntry.GetError(dtpkrShipDate)))
            {
                _porder.AnticipateDate = dtpkrAnticipateDate.Value;
                _porder.ShippingDate = dtpkrShipDate.Value;
                if ((dtpkrAnticipateDate.Value.Date < dtpkrShipDate.Value.Date))
                {
                    errPOEntry.SetError(dtpkrAnticipateDate, "Anticipate date cannot be before the ship date");
                    return;
                }

                //if (_porder.Penvironment.Domain == "SWNA")
                if (DataCache.DisplayPOHitsCancelDateWithDayFormat == true)
                {
                    txtCancelDate.Text = dtpkrShipDate.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate).ToString(DateFormats.PO_CancelDate);
                    _porder.CancelDate = dtpkrShipDate.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate);
                }
                else
                {
                    txtCancelDate.Text = dtpkrShipDate.Value.ToLongDateString();
                    _porder.CancelDate = dtpkrShipDate.Value;
                }
            }
        }

        private void cmbSSD_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSSD.SelectedItem != null)
            {
                DateTime dt;
                if (DateTime.TryParse(cmbSSD.SelectedItem.ToString(), out dt))
                {
                    _porder.StageSetDate = dt;
                    DataRow dr = dtSSD.Rows.Find(cmbSSD.SelectedItem);
                    if (dr != null)
                    {
                        _porder.StageSetDateID = (int)dr["clmSSDSSI"];
                    }
                    else
                    {
                        _porder.StageSetDateID = 0;
                    }
                }
                else
                {
                    _porder.StageSetDate = DateTime.MinValue;
                    _porder.StageSetDateID = 0;
                }
            }
        }

        private void cmbShipTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            short dc;
            if (short.TryParse(cmbShipTo.SelectedItem.ToString(), out dc))
            {
                _porder.ShipTo = dc;
                _porder.ShipToMethod = "DC";
                _porder.ShipToRounding = "C";
            }
        }


        private void dgvPOLines_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1) return;

            Int16 CollectionIndex = Convert.ToInt16(dgvPOlines["CollectionIndex", e.RowIndex].Value);
            _polinedetails = _porder.lstpoLineItemDetails[CollectionIndex];

            if (_polinedetails.IsValid && _polinedetails.APP1 == "N")
            {
                Cursor.Current = Cursors.WaitCursor;

                _polinedetails.ConvertedCost = Decimal.Parse(dgvPOlines["ConvertedCost", e.RowIndex].Value.ToString());
                _polinedetails.Retailprice = Decimal.Parse(dgvPOlines["Retail", e.RowIndex].Value.ToString());

                polineform = new POLineDetails(_porder, ref _polinedetails, false);
                polineform.ShowDialog(this);

                dgvPOlines["Quantity", e.RowIndex].Value = _polinedetails.Itemquantity;
                dgvPOlines["ConvertedCost", e.RowIndex].Value = Decimal.Round(_polinedetails.ConvertedCost, 2).ToString("#.00");
                dgvPOlines["SavedConvertedCost", e.RowIndex].Value = Decimal.Round(_polinedetails.ConvertedCost, 2);

                _polinedetails.Cost = Decimal.Round(_polinedetails.ConvertedCost * _porder.ExchangeRate, 2);
                dgvPOlines["Cost", e.RowIndex].Value = _polinedetails.Cost;

                if (_porder.Landing == 0) _porder.Landing = 1;

                _polinedetails.LandedCost = Decimal.Round(_polinedetails.Cost * _porder.Landing, 2);

                CalculatePOsummary();

                polineform = null;
            }
            else if (_polinedetails.IsValid && _polinedetails.APP1 == "Y")
            {
                Cursor.Current = Cursors.WaitCursor;

                POLineDetailsPack polinedetailspack = new POLineDetailsPack(_polinedetails, _porder, e.RowIndex);

                polinedetailspack.OnAppQuantityChanged += new POLineDetailsPack.AppQuantityChangedEventHandler(polinedetailspack_OnAppQuantityChanged);
                polinedetailspack.ShowDialog(this);

                dgvPOlines["Quantity", e.RowIndex].Value = _polinedetails.Itemquantity;
                dgvPOlines["ConvertedCost", e.RowIndex].Value = Decimal.Round(_polinedetails.ConvertedCost, 2).ToString("#.00");
                dgvPOlines["SavedConvertedCost", e.RowIndex].Value = Decimal.Round(_polinedetails.ConvertedCost, 2);
                dgvPOlines["ConvertedCost6", e.RowIndex].Value = Decimal.Round(_polinedetails.ConvertedCost, 6);

                CalculateAPPComponentCost();
                _polinedetails.Cost = Decimal.Round(TotalCost, 2);
                dgvPOlines["Cost", e.RowIndex].Value = _polinedetails.Cost;

                if (_porder.Landing == 0) _porder.Landing = 1;

                _polinedetails.LandedCost = Decimal.Round(_polinedetails.Cost * _porder.Landing, 2);

                CalculatePOsummary();

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

            dgvPOlines.Rows[e.rowindex].Cells["ConvertedCost"].Value = Decimal.Round(e.poline.ConvertedCost, 2).ToString("#.00");
            dgvPOlines.Rows[e.rowindex].Cells["SavedConvertedCost"].Value = Decimal.Round(_polinedetails.ConvertedCost, 2);

            CalculatePOsummary();
        }

        private void dgvPOlines_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (_bFormCancelClicked)
            {
                return;
            }

            if (_porder.lstpoLineItemDetails.Count > 0 && e.RowIndex < _porder.lstpoLineItemDetails.Count)
            {
                int gridcollectionindex = Convert.ToInt16(dgvPOlines["CollectionIndex", e.RowIndex].Value);
                _polinedetails = _porder.lstpoLineItemDetails[gridcollectionindex];
            }


        }

        private void dgvPOlines_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (_bFormCancelClicked == false)
            {
                dgvPOlines.ReadOnly = false;
                errPOEntry.SetError(btnHelp, "");
            }
        }

        private void dgvPOlines_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (_bFormCancelClicked)
            {
                return;
            }

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
                String message = String.Empty;
                switch (dgvPOlines.Columns[e.ColumnIndex].Name)
                {
                    case "Class":
                        {
                            e.Cancel = ValidateClassCellValue(e.FormattedValue.ToString().Trim(), dgvPOlines["Class", e.RowIndex], out message);
                            break;
                        }
                    case "Vendor":
                        {
                            e.Cancel = ValidateVendorCellValue(e.FormattedValue.ToString().Trim(), dgvPOlines["Vendor", e.RowIndex], out message);
                            break;
                        }
                    case "Style":
                        {
                            e.Cancel = ValidateStyleCellValue(e.FormattedValue.ToString().Trim(), dgvPOlines["Style", e.RowIndex], out message);
                            break;
                        }
                    case "Color":
                        {
                            e.Cancel = ValidateColorCellValue(e.FormattedValue.ToString().Trim(), dgvPOlines["Color", e.RowIndex], out message);
                            break;
                        }
                    case "Size":
                        {
                            e.Cancel = ValidateSizeCellValue(e.FormattedValue.ToString().Trim(), dgvPOlines.Rows[e.RowIndex], out message);
                            break;
                        }
                    case "ConvertedCost":
                        {
                            e.Cancel = ValidateConvertedCostCellValue(e.FormattedValue.ToString().Trim(), dgvPOlines.Rows[e.RowIndex], out message);
                            break;
                        }
                    case "Quantity":
                        {
                            e.Cancel = ValidateQuantityCellValue(e.FormattedValue.ToString().Trim(), dgvPOlines["Quantity", e.RowIndex], out message);
                            break;
                        }
                }

                dgvPOlines.Rows[e.RowIndex].ErrorText = message;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source);
            }
        }

        private void dgvPOlines_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPOlines.Rows[e.RowIndex].IsNewRow == true) return;

            if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Quantity"))
            {
                gridcollectionindex = Convert.ToInt32(dgvPOlines["CollectionIndex", e.RowIndex].Value);
                if (_porder.lstpoLineItemDetails[gridcollectionindex].IsDeleted == false && dgvPOlines.Rows[e.RowIndex].IsNewRow == false
                    && _porder.lstpoLineItemDetails[gridcollectionindex].IsValid == true)
                {
                    dgvPOlines[e.ColumnIndex, e.RowIndex].Value = _polinedetails.Itemquantity;
                }
            }

            if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Size"))
            {
                if (_bDuplicateItem)
                {
                    // Always in Red
                    SetItemColor(e.ColumnIndex, e.RowIndex, System.Drawing.Color.Red);
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

        private void dgvPOlines_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            Boolean bundleSelected = false;
            short itemClass = 0;
            int vendor = 0;
            short style = 0;
            short colour = 0;
            short size = 0;

            try
            {
                short.TryParse(dgvPOlines.Rows[e.RowIndex].Cells[1].Value.ToString(), out itemClass);
                int.TryParse(dgvPOlines.Rows[e.RowIndex].Cells[2].Value.ToString(), out vendor);
                short.TryParse(dgvPOlines.Rows[e.RowIndex].Cells[3].Value.ToString(), out style);
                short.TryParse(dgvPOlines.Rows[e.RowIndex].Cells[4].Value.ToString(), out colour);
                short.TryParse(dgvPOlines.Rows[e.RowIndex].Cells[5].Value.ToString(), out size);

                //if (itemClass > 0 || vendor > 0 || style > 0)
                //{
                //    bundleSelected = lookupbo.ItemIsBundle(itemClass, vendor, style, colour, size);
                //    if (bundleSelected == true)
                //    {
                //        dgvPOlines.Rows[e.RowIndex].ErrorText = "Bundles can not be selected on PO's.";

                //        for (int i = 1; i < 15; i++)
                //        {
                //            dgvPOlines.Rows[e.RowIndex].Cells[i].Value = "";
                //        }
                //        e.Cancel = true;
                //        return;
                //    }
                //}

                if (_bFormCancelClicked)
                {
                    return;
                }

                if (_bUserWantsToDeleteLine)
                {
                    return;
                }

                if (e.RowIndex < dgvPOlines.RowCount - 1)
                {
                    if (!_polinedetails.IsValid)
                    {
                        dgvPOlines.Rows[e.RowIndex].ErrorText = "Please enter valid item";
                        e.Cancel = true;
                    }
                    else
                    {
                        errPOEntry.SetError(dgvPOlines, "");
                        e.Cancel = false;

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
            catch (Exception ex)
            {
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
            if (e.Row.IsNewRow)
            {
                _polinedetails = new POItemDetails();
                _polinedetails.Sequence = GetNextCollectionIndex();
                _porder.lstpoLineItemDetails.Add(_polinedetails);
            }
        }

        private void dgvPOlines_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (_porder.NumofPOLines >= 1)
            {
                _porder.NumofPOLines = _porder.NumofPOLines - 1;
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx");
        }

        private void btnDeleteLine_Click(object sender, EventArgs e)
        {
            int iRowsDeleted = 0;
            _bUserWantsToDeleteLine = true;

            //Ooops.....Removing lines from the grid resequences the index of the grid.
            //Using this index as a method of updating this collection does not work.
            //I have split the work this loop does into two loops.
            //one to update the collection and the other to remove the rows from the grid....

            //Update the collection
            for (int iLoopCounter = 0; iLoopCounter < dgvPOlines.Rows.Count; iLoopCounter++)
            {
                if (!dgvPOlines.Rows[iLoopCounter].IsNewRow)
                {
                    if (dgvPOlines.Rows[iLoopCounter].Cells[0].Value != null &&
                    Convert.ToBoolean(dgvPOlines.Rows[iLoopCounter].Cells[0].Value) == true)
                    {
                        gridcollectionindex = Convert.ToInt16(dgvPOlines.Rows[iLoopCounter].Cells["CollectionIndex"].Value);
                        _porder.lstpoLineItemDetails[gridcollectionindex].IsDeleted = true;
                        if (_porder.lstpoLineItemDetails[gridcollectionindex].APP1 == "Y")
                        {
                            _porder.NumofPOPacks = _porder.NumofPOPacks - 1;
                        }
                    }
                }
            }

            //Remove Rows from the grid...As the RemoveAt command resequences the index
            //it cannot be used in a straightforward manner. Looping backwards fixes the problem.
            int x = dgvPOlines.Rows.Count - 1;

            for (int iLoopCounter2 = x; iLoopCounter2 >= 0; iLoopCounter2--)
            {
                if (!dgvPOlines.Rows[iLoopCounter2].IsNewRow)
                {
                    if (dgvPOlines.Rows[iLoopCounter2].Cells[0].Value != null &&
                        Convert.ToBoolean(dgvPOlines.Rows[iLoopCounter2].Cells[0].Value) == true)
                    {
                        short index = Convert.ToInt16(dgvPOlines.Rows[iLoopCounter2].Cells["CollectionIndex"].Value);
                        dgvPOlines.Rows.RemoveAt(iLoopCounter2);
                        iRowsDeleted++;
                    }
                }
            }

            _bUserWantsToDeleteLine = false;
            if (iRowsDeleted > 0)
            {
                CalculatePOsummary();
            }
        }

        private Boolean ValidateClassCellValue(String Newcellvalue, DataGridViewCell ClassCell, out String message)
        {
            if (String.IsNullOrEmpty(Newcellvalue))
            {
                message = string.Empty;
                return false;
            }

            List<string> retValues = validationcls.ValidateClass(Newcellvalue, txtDept.Text);
            if ("False" == retValues[0])
            {
                message = "Please enter valid Class";
                return true;
            }

            message = string.Empty;

            _polinedetails.ClassCode = Int16.Parse(Newcellvalue);
            _polinedetails.Classname = retValues[1].ToString();

            if (ClassCell.Value != null)
            {
                short itemclass = Convert.ToInt16(ClassCell.Value);
                if (itemclass != _polinedetails.ClassCode)
                {
                    IsItemNumberChanged = true;
                }
            }
            else
            {
                IsItemNumberChanged = true;
            }

            return false;
        }

        private Boolean ValidateVendorCellValue(String NewCellValue, DataGridViewCell VendorCell, out String message)
        {
            if (String.IsNullOrEmpty(NewCellValue))
            {
                message = string.Empty;
                return false;
            }

            List<string> retValues = validationcls.ValidateVendor(NewCellValue, true);
            if (retValues[0] == "False")
            {
                message = "Please enter valid Vendor code";
                return true;
            }

            message = string.Empty;

            _polinedetails.Vendorcode = Int32.Parse(NewCellValue);
            _polinedetails.Vendordesc = retValues[1].ToString();

            if (VendorCell.Value != null)
            {
                int vendor = Convert.ToInt32(VendorCell.Value);
                if (vendor != _polinedetails.Vendorcode)
                {
                    IsItemNumberChanged = true;
                }
            }
            else
            {
                IsItemNumberChanged = true;
            }

            return false;
        }

        private Boolean ValidateStyleCellValue(String NewCellValue, DataGridViewCell StyleCell, out String message)
        {
            if (String.IsNullOrEmpty(NewCellValue))
            {
                message = string.Empty;
                return false;
            }

            List<string> retValues = validationcls.ValidateStyle(NewCellValue);
            if ("False" == retValues[0])
            {
                message = "Please enter valid Style code";
                return true;
            }

            message = String.Empty;

            _polinedetails.Stylecode = Int16.Parse(NewCellValue);

            if (StyleCell.Value != null)
            {
                short style = Convert.ToInt16(StyleCell.Value);
                if (style != _polinedetails.Stylecode)
                {
                    IsItemNumberChanged = true;
                }
            }
            else
            {
                IsItemNumberChanged = true;
            }

            return false;
        }

        private Boolean ValidateColorCellValue(String NewCellValue, DataGridViewCell ColorCell, out String message)
        {
            if (String.IsNullOrEmpty(NewCellValue))
            {
                message = String.Empty;
                return false;
            }

            List<string> retValues = validationcls.ValidateColour(NewCellValue);
            if (("False" == retValues[0]))
            {
                message = "Please enter valid Color";
                return true;
            }

            message = String.Empty;

            _polinedetails.Colorcode = Int16.Parse(NewCellValue);
            _polinedetails.Colordesc = retValues[1];

            if (ColorCell.Value != null)
            {
                Int16 color = Convert.ToInt16(ColorCell.Value);
                if (color != _polinedetails.Colorcode)
                {
                    IsItemNumberChanged = true;
                }
            }
            else
            {
                IsItemNumberChanged = true;
            }

            return false;
        }

        private Boolean ValidateSizeCellValue(String NewCellValue, DataGridViewRow GridRow, out String message)
        {
            if (String.IsNullOrEmpty(NewCellValue))
            {
                message = string.Empty;
                return false;
            }

            List<string> retValues = validationcls.ValidateSize(NewCellValue);
            if ("False" == retValues[0])
            {
                message = "Please enter valid Size";
                return true;
            }

            message = string.Empty;

            _polinedetails.Itemsize = Int16.Parse(NewCellValue);
            _polinedetails.Sizename = retValues[1];

            if (GridRow.Cells["Size"].Value != null)
            {
                Int16 size = Convert.ToInt16(GridRow.Cells["Size"].Value);
                if (size != _polinedetails.Itemsize)
                {
                    IsItemNumberChanged = true;
                }
            }
            else
            {
                IsItemNumberChanged = true;
            }

            if (!IsItemNumberChanged)
            {
                message = String.Empty;
                return false;
            }

            Int16 iItem;
            Int32 iVendor;
            Int16 iStyle;
            Int16 iColour;

            Int16.TryParse(Convert.ToString(GridRow.Cells["Class"].Value), out iItem);
            Int32.TryParse(Convert.ToString(GridRow.Cells["Vendor"].Value), out iVendor);
            Int16.TryParse(Convert.ToString(GridRow.Cells["Style"].Value), out iStyle);
            Int16.TryParse(Convert.ToString(GridRow.Cells["Color"].Value), out iColour);

            _bDuplicateItem = CheckForDuplicateLine(_polinedetails.Sequence, iItem, iVendor, iStyle, iColour, _polinedetails.Itemsize);

            if (_polinedetails.ItemLookup(_porder.DbParamRef, _porder.UserName, _porder.Penvironment, _porder.DefaultMarket))
            {
                GridRow.Cells["Description"].Value = _polinedetails.Itemlongdescription;
                GridRow.Cells["Retail"].Value = _polinedetails.Retailprice.ToString();
                GridRow.Cells["Cost"].Value = _polinedetails.Cost.ToString();
                GridRow.Cells["Character"].Value = _polinedetails.Characterdesc;
                GridRow.Cells["Season"].Value = _polinedetails.SeasonDesc;
                GridRow.Cells["CasePackType"].Value = _polinedetails.Packdescription;
                GridRow.Cells["TicketType"].Value = _polinedetails.Tickettype;
                GridRow.Cells["Pack"].Value = _polinedetails.APP1;
                GridRow.Cells["CollectionIndex"].Value = _polinedetails.Sequence.ToString();

                if (_polinedetails.APP1 == "Y")
                {
                    _porder.NumofPOPacks++;
                    GridRow.Cells["ConvertedCost"].ReadOnly = true;

                    AssortedPrePack AssPrePack = new AssortedPrePack(_polinedetails, _porder);
                    DataTable prePackTbl = AssPrePack.PopulateAPPStructure();

                    _polinedetails.Components.Clear();

                    TotalComponentCost = 0;
                    TotalComponentCvtCost = 0;
                    TotalComponentRetail = 0;

                    for (int i = 0; i < prePackTbl.Rows.Count; i++)
                    {
                        APPcomponent component = new APPcomponent();
                        component.ComponentClass = (Int16)prePackTbl.Rows[i]["ComponentClass"];
                        component.ComponentVendor = (Int32)prePackTbl.Rows[i]["ComponentVendor"];
                        component.ComponentStyle = (Int16)prePackTbl.Rows[i]["ComponentStyle"];
                        component.ComponentColour = (Int16)prePackTbl.Rows[i]["ComponentColour"];
                        component.ComponentSize = (Int16)prePackTbl.Rows[i]["ComponentSize"];
                        component.RatioQuantity = (Int16)prePackTbl.Rows[i]["ComponentQuantity"];

                        component.Cost = Decimal.Round((Decimal)prePackTbl.Rows[i]["ComponentCost"] / _porder.ExchangeRate, 2);
                        component.UnConvertedCost = Decimal.Round((Decimal)prePackTbl.Rows[i]["ComponentCost"], 2);

                        component.Retail = (Decimal)prePackTbl.Rows[i]["Retail"];
                        component.ItemDescription = (String)prePackTbl.Rows[i]["ComponentLongDesc"];

                        _polinedetails.Components.Add(component);

                        //Re-Calculate Cost, Converted Cost and Retail from the Components
                        TotalComponentCost += Decimal.Round(component.Cost * component.RatioQuantity, 2);
                        TotalComponentCvtCost += Decimal.Round((Decimal)prePackTbl.Rows[i]["ComponentCost"] * component.RatioQuantity, 2);
                        TotalComponentRetail += component.Retail * component.RatioQuantity;
                    }

                    //Re -Assign values
                    _polinedetails.Cost = TotalComponentCvtCost;
                    GridRow.Cells["Cost"].Value = _polinedetails.Cost.ToString();

                    _polinedetails.ConvertedCost = TotalComponentCost;
                    GridRow.Cells["ConvertedCost"].Value = Decimal.Round(_polinedetails.ConvertedCost, 2).ToString("#.00");
                    GridRow.Cells["ConvertedCost6"].Value = _polinedetails.ConvertedCost;
                    GridRow.Cells["SavedConvertedCost"].Value = _polinedetails.ConvertedCost;

                    _polinedetails.Retailprice = TotalComponentRetail;
                    GridRow.Cells["Retail"].Value = _polinedetails.Retailprice.ToString();
                }
                else
                {
                    GridRow.Cells["ConvertedCost"].ReadOnly = false;
                    _polinedetails.ConvertedCost = _polinedetails.Cost / _porder.ExchangeRate;
                    GridRow.Cells["ConvertedCost6"].Value = _polinedetails.ConvertedCost;
                    GridRow.Cells["ConvertedCost"].Value = Decimal.Round(_polinedetails.ConvertedCost, 2).ToString("#.00");
                    GridRow.Cells["SavedConvertedCost"].Value = Decimal.Round(_polinedetails.ConvertedCost, 2);
                }

                if (_porder.Landing == 0)
                {
                    _porder.Landing = 1;
                }

                _polinedetails.LandedCost = Decimal.Round(_polinedetails.Cost * _porder.Landing, 2);

                _polinedetails.IsValid = true;
                IsItemNumberChanged = false;

                message = string.Empty;
                return false;
            }
            else
            {
                GridRow.Cells["Description"].Value = _polinedetails.Itemlongdescription;
                GridRow.Cells["Retail"].Value = String.Empty;
                GridRow.Cells["Cost"].Value = String.Empty;
                GridRow.Cells["Character"].Value = String.Empty;
                GridRow.Cells["Season"].Value = String.Empty;
                GridRow.Cells["CasePackType"].Value = String.Empty;
                GridRow.Cells["TicketType"].Value = String.Empty;
                GridRow.Cells["Pack"].Value = String.Empty;
                GridRow.Cells["Quantity"].Value = String.Empty;
                GridRow.Cells["ConvertedCost"].Value = String.Empty;

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

                _polinedetails.IsValid = false;
                message = string.Empty;
                return false;
            }
        }

        private Boolean ValidateQuantityCellValue(String NewCellValue, DataGridViewCell QuantityCell, out String message)
        {
            if (_polinedetails.IsValid == false)
            {
                message = String.Empty;
                return false;
            }

            if (String.IsNullOrEmpty(NewCellValue))
            {
                message = "Please enter a quantity";
                return true;
            }

            Int32 quantity;
            if (!Int32.TryParse(NewCellValue, out quantity))
            {
                message = "You have entered an invalid quantity value";
                return true;
            }

            if (quantity > 999999 || quantity <= 0)
            {
                message = "Quantity must not be greater than 999,999, Zero, or Negative";
                return true;
            }

            _polinedetails.Itemquantity = quantity;

            int DropShipCount = dtDropShipMatrix.Rows.Count;

            if (DropShipCount > 0)
            {
                DataView dvDropShipMatrix = new DataView(dtDropShipMatrix);

                string ItemClass = Convert.ToString(_polinedetails.ClassCode);
                string ItemVendor = Convert.ToString(_polinedetails.Vendorcode);
                string ItemStyle = Convert.ToString(_polinedetails.Stylecode);
                string ItemColour = Convert.ToString(_polinedetails.Colorcode);
                string ItemSize = Convert.ToString(_polinedetails.Itemsize);

                StringBuilder Filter = new StringBuilder();
                Filter.Append("Class = ");
                Filter.Append(ItemClass);
                Filter.Append(" and Vendor = ");
                Filter.Append(ItemVendor);
                Filter.Append(" and Style = ");
                Filter.Append(ItemStyle);
                Filter.Append(" and Color = ");
                Filter.Append(ItemColour);
                Filter.Append(" and Size = ");
                Filter.Append(ItemSize);

                dvDropShipMatrix.RowFilter = Filter.ToString();

                foreach (DataRowView drvDropShipMatrix in dvDropShipMatrix)
                {
                    drvDropShipMatrix.Row["Quantity"] = quantity;
                }
            }


            if (rdBCase.Checked)
            {
                if (quantity % _polinedetails.CasePackQty != 0)
                {
                    QuantityRounding roundingform = new QuantityRounding(quantity, _polinedetails.CasePackQty);
                    if (roundingform.ShowDialog(this) == DialogResult.OK)
                    {
                        _polinedetails.Itemquantity = roundingform.RoundedQuantity;
                    }
                    else
                    {
                        message = "Quantity must be rounded to the nearest CasePack Quantity";
                        return true;
                    }
                }
            }
            else
            {
                if (quantity % _polinedetails.DistroQty != 0)
                {
                    QuantityRounding roundingform = new QuantityRounding(quantity, _polinedetails.DistroQty);
                    if (roundingform.ShowDialog(this) == DialogResult.OK)
                    {
                        _polinedetails.Itemquantity = roundingform.RoundedQuantity;
                    }
                    else
                    {
                        message = "Quantity must be rounded to the nearest Distro quantity";
                        return true;
                    }
                }
            }

            CalculatePOsummary();

            message = string.Empty;
            return false;

        }

        private Boolean ValidateConvertedCostCellValue(String NewCellValue, DataGridViewRow GridRow, out String message)
        {
            if (_polinedetails.IsValid == false)
            {
                message = string.Empty;
                return false;
            }

            if (String.IsNullOrEmpty(NewCellValue))
            {
                message = "Please enter a cost value";
                return true;
            }

            decimal convertedcost;

            if (!Decimal.TryParse(NewCellValue, out convertedcost))
            {
                message = "You have entered an invalid cost value";
                return true;
            }

            //Only two decimal places will get written to the DB
            //Added 06/05/2011 Joseph Urbina 
            decimal DecTest = Math.Round(convertedcost, 2);
            if (DecTest == 0)
            {
                message = "You have entered an invalid cost value";
                return true;
            }

            if (convertedcost <= 0)
            {
                message = "Cost value cannot be zero or negative";
                return true;
            }

            if (Convert.ToDecimal(GridRow.Cells["SavedConvertedCost"].Value) == convertedcost)
            {
                if (_polinedetails.APP1 != "Y")
                {
                    _polinedetails.ConvertedCost = Convert.ToDecimal(GridRow.Cells["ConvertedCost6"].Value);
                }
            }
            else
            {
                _polinedetails.ConvertedCost = convertedcost;
                GridRow.Cells["ConvertedCost"].Value = Decimal.Round(_polinedetails.ConvertedCost, 2).ToString("#.00");
                GridRow.Cells["SavedConvertedCost"].Value = Decimal.Round(_polinedetails.ConvertedCost, 2);
                GridRow.Cells["ConvertedCost6"].Value = _polinedetails.ConvertedCost;

                _polinedetails.Cost = Decimal.Round(_polinedetails.ConvertedCost * _porder.ExchangeRate, 2);
                GridRow.Cells["Cost"].Value = _polinedetails.Cost;

                _polinedetails.LandedCost = Decimal.Round(_polinedetails.Cost * _porder.Landing, 2);

                CalculatePOsummary();
            }


            message = String.Empty;
            return false;
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

            DataTable dtSelectedItems;

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

            if (dtSelectedItems.Rows.Count > 0)
            {
                iStartRow = iTotalRowsInGrid = dgvPOlines.Rows.Count;

                foreach (DataRow dr in dtSelectedItems.Rows)
                {
                    POItemDetails poitem = new POItemDetails();

                    poitem.ClassCode = (short)dr["class"];
                    poitem.Vendorcode = (int)dr["vendor"];
                    poitem.Stylecode = (short)dr["style"];
                    poitem.Colorcode = (short)dr["colour"];
                    poitem.Itemsize = (short)dr["size"];

                    poitem.Sequence = GetNextCollectionIndex();

                    List<string> retValues = validationcls.ValidateClass(poitem.ClassCode.ToString());
                    poitem.Classname = retValues[1].ToString();

                    retValues = validationcls.ValidateVendor(poitem.Vendorcode.ToString(), true);
                    poitem.Vendordesc = retValues[1].ToString();

                    retValues = validationcls.ValidateColour(poitem.Colorcode.ToString());
                    poitem.Colordesc = retValues[1];

                    retValues = validationcls.ValidateSize(poitem.Itemsize.ToString());
                    poitem.Sizename = retValues[1];

                    _bDuplicateItem = CheckForDuplicateLine(poitem.Sequence, poitem.ClassCode, poitem.Vendorcode, poitem.Stylecode, poitem.Colorcode, poitem.Itemsize);

                    if (poitem.ItemLookup(_porder.DbParamRef, _porder.UserName, _porder.Penvironment, _porder.DefaultMarket))
                    {
                        iStartRow = dgvPOlines.Rows.Add(false, dr["class"],
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
                        dgvPOlines.Rows[iStartRow].Cells["CollectionIndex"].Value = poitem.Sequence.ToString();

                        _porder.NumofPOLines++;

                        if (_bDuplicateItem)
                        {
                            // Always in Red
                            SetItemColor(1, iStartRow, System.Drawing.Color.Red);
                            _bDuplicateItem = false;
                        }

                        if (poitem.APP1 == "Y")
                        {
                            _porder.NumofPOPacks++;
                            dgvPOlines.Rows[iStartRow].Cells["ConvertedCost"].ReadOnly = true;

                            AssortedPrePack AssPrePack = new AssortedPrePack(poitem, _porder);
                            DataTable prePackTbl = AssPrePack.PopulateAPPStructure();

                            TotalComponentCost = 0;
                            TotalComponentCvtCost = 0;
                            TotalComponentRetail = 0;

                            for (int i = 0; i < prePackTbl.Rows.Count; i++)
                            {
                                APPcomponent component = new APPcomponent();

                                component.ComponentClass = (Int16)prePackTbl.Rows[i]["ComponentClass"];
                                component.ComponentVendor = (Int32)prePackTbl.Rows[i]["ComponentVendor"];
                                component.ComponentStyle = (Int16)prePackTbl.Rows[i]["ComponentStyle"];
                                component.ComponentColour = (Int16)prePackTbl.Rows[i]["ComponentColour"];
                                component.ComponentSize = (Int16)prePackTbl.Rows[i]["ComponentSize"];
                                component.RatioQuantity = (Int16)prePackTbl.Rows[i]["ComponentQuantity"];

                                component.Cost = Decimal.Round((Decimal)prePackTbl.Rows[i]["ComponentCost"] / _porder.ExchangeRate, 2);
                                component.UnConvertedCost = Decimal.Round((Decimal)prePackTbl.Rows[i]["ComponentCost"], 2);
                                component.Retail = (Decimal)prePackTbl.Rows[i]["Retail"];
                                component.ItemDescription = (String)prePackTbl.Rows[i]["ComponentLongDesc"];

                                poitem.Components.Add(component);

                                //Re-Calculate Cost, Converted Cost and Retail from the Components
                                TotalComponentCost += Decimal.Round(component.Cost * component.RatioQuantity, 2);
                                TotalComponentCvtCost += Decimal.Round((Decimal)prePackTbl.Rows[i]["ComponentCost"] * component.RatioQuantity, 2);
                                TotalComponentRetail += component.Retail * component.RatioQuantity;
                            }

                            //Re -Assign values
                            poitem.Cost = TotalComponentCvtCost;
                            dgvPOlines.Rows[iStartRow].Cells["Cost"].Value = poitem.Cost.ToString();

                            poitem.ConvertedCost = TotalComponentCost;
                            dgvPOlines.Rows[iStartRow].Cells["ConvertedCost"].Value = Decimal.Round(poitem.ConvertedCost, 2).ToString("#.00");
                            dgvPOlines.Rows[iStartRow].Cells["ConvertedCost6"].Value = poitem.ConvertedCost;
                            dgvPOlines.Rows[iStartRow].Cells["SavedConvertedCost"].Value = poitem.ConvertedCost;

                            poitem.Retailprice = TotalComponentRetail;
                            dgvPOlines.Rows[iStartRow].Cells["Retail"].Value = poitem.Retailprice.ToString();
                        }
                        else
                        {
                            dgvPOlines.Rows[iStartRow].Cells["ConvertedCost"].ReadOnly = false;
                            poitem.ConvertedCost = poitem.Cost / _porder.ExchangeRate;
                            dgvPOlines.Rows[iStartRow].Cells["ConvertedCost6"].Value = poitem.ConvertedCost;
                            dgvPOlines.Rows[iStartRow].Cells["ConvertedCost"].Value = Decimal.Round(poitem.ConvertedCost, 2).ToString("#.00");
                            dgvPOlines.Rows[iStartRow].Cells["SavedConvertedCost"].Value = Decimal.Round(poitem.ConvertedCost, 2);
                        }

                        if (_porder.Landing == 0)
                        {
                            _porder.Landing = 1;
                        }

                        poitem.LandedCost = Decimal.Round(poitem.Cost * _porder.Landing, 2);

                        _porder.lstpoLineItemDetails.Add(poitem);
                    }
                    else
                    {
                        poitem.IsValid = false;

                        MessageBox.Show("Skipping the following item as it does not belong to the current market."
                              + "\r\n"
                              + "\r\n"
                              + " Class Code: " + poitem.ClassCode.ToString("0000")
                              + " Vendor Code: " + poitem.Vendorcode.ToString("00000")
                              + " Style Code: " + poitem.Stylecode.ToString("0000")
                              + " Color Code: " + poitem.Colorcode.ToString("000")
                              + " Size Code: " + poitem.Itemsize.ToString("0000")
                              + "", "Spice - PO Create");
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _bFormCancelClicked = true;

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
            if (CheckGridCount())
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
                        if (dtSelectedStores == null || dtSelectedStores.Rows.Count == 0)
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

                    // Set Freight code value
                    switch (cbxFreight.Text)
                    {
                        case "":
                            _porder.Freight = "";
                            break;
                        case "Pre Pay":
                            _porder.Freight = "P";
                            break;
                        case "Collect":
                            _porder.Freight = "C";
                            break;
                        default:
                            _porder.Freight = "";
                            break;
                    }

                    //Check Class/Dept again in case user has re-inputted the department.
                    if (CheckItemCount())
                    {
                        Boolean classerror = false;

                        foreach (DataGridViewRow dgvRow in dgvPOlines.Rows)
                        {
                            if (dgvRow.Cells["Class"].Value != null)
                            {
                                List<string> retValues = validationcls.ValidateClass(dgvRow.Cells["Class"].Value.ToString(), txtDept.Text);
                                if ("False" == retValues[0])
                                {
                                    dgvPOlines.Rows[dgvRow.Index].ErrorText = "Please enter a valid Class";
                                    classerror = true;
                                }
                                else
                                {
                                    dgvPOlines.Rows[dgvRow.Index].ErrorText = String.Empty;
                                }
                            }
                        }

                        if (classerror == true)
                        {
                            return;
                        }
                    }


                    //Check Quantity Rounding again in case user has re-Selected Ship To Radio Buttons.
                    if (CheckItemCount())
                    {
                        Boolean RoundingError = false;

                        foreach (DataGridViewRow dgvRow in dgvPOlines.Rows)
                        {
                            if (dgvRow.Cells["Quantity"].Value != null)
                            {
                                if (rdBCase.Checked)
                                {
                                    int quantity = Convert.ToInt32((dgvRow.Cells["Quantity"].Value));
                                    int CollectionIndex = Convert.ToInt16((dgvRow.Cells["CollectionIndex"].Value));
                                    _polinedetails = _porder.lstpoLineItemDetails[CollectionIndex];
                                    if (quantity % _polinedetails.CasePackQty != 0)
                                    {
                                        dgvPOlines.Rows[dgvRow.Index].ErrorText = "Please re-input Quantity";
                                        RoundingError = true;
                                    }
                                    else
                                    {
                                        dgvPOlines.Rows[dgvRow.Index].ErrorText = String.Empty;
                                    }
                                }
                                else
                                {
                                    int quantity = Convert.ToInt32((dgvRow.Cells["Quantity"].Value));
                                    int CollectionIndex = Convert.ToInt16((dgvRow.Cells["CollectionIndex"].Value));
                                    _polinedetails = _porder.lstpoLineItemDetails[CollectionIndex];
                                    if (quantity % _polinedetails.DistroQty != 0)
                                    {
                                        dgvPOlines.Rows[dgvRow.Index].ErrorText = "Please re-input Quantity";
                                        RoundingError = true;
                                    }
                                    else
                                    {
                                        dgvPOlines.Rows[dgvRow.Index].ErrorText = String.Empty;
                                    }
                                }
                            }
                        }

                        if (RoundingError == true)
                        { return; }
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

                        //The sequence number used to create component lines in DSSPPOA is used to create POhits.
                        //But to create DSSPPOA lines the sequence has to start at 1.
                        //This is in direct conflict to the way hits are created.
                        //I have added a new property to this collection that resolves this problem.

                        short sequenceCount = 0;
                        foreach (POItemDetails item in _porder.lstpoLineItemDetails)
                        {
                            if (item.IsDeleted == false && item.IsValid == true)
                            {
                                sequenceCount++;
                                item.PoaSequence = Convert.ToInt16(sequenceCount);
                            }
                        }

                        bkgrndWorker.RunWorkerAsync();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("An Error has occurred with the Purchase Order Please contact Support", "SPICE PO MANAGEMENT");
                }
            }
            else
            {
                MessageBox.Show("This PO cannot be created as there are no Po Lines.", "SPICE PO MANAGEMENT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

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

        private void dgvPOlines_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

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

        private void btnHits_Click(object sender, EventArgs e)
        {
            Boolean bDisallowHits = false;
            int poitemcount = 0;

            foreach (DataGridViewRow dgvRow in dgvPOlines.Rows)
            {
                if (dgvRow.IsNewRow == false)
                {
                    poitemcount++;
                }
            }


            if (poitemcount == 0)
            {
                bDisallowHits = true;
                MessageBox.Show("There are no lines on the PO. This prevents Hits being created.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Is there a invalid (half or incompletely entered) item
            if (bDisallowHits == true)
            {
                return;
            }

            POHits pohitsform = new POHits(_porder, _pohitscollection, _porder.Penvironment);

            // Subscribe to event handlers
            pohitsform.OnOkButtonClicked += new POHits.OkButtonClickedEventHandler(pohitsform_OnOkButtonClicked);
            pohitsform.OnCancelButtonClicked += new POHits.CancelButtonClickedEventHandler(pohitsform_OnCancelButtonClicked);

            pohitsform.ShowDialog();

            pohitsform = null;

            UpdateNumberOfHits();
        }

        void pohitsform_OnCancelButtonClicked(object sender, POHits.PoHitsEventArgs e)
        {
            _pohitscollection = e.poHitsCollection;
        }

        void pohitsform_OnOkButtonClicked(object sender, POHits.PoHitsEventArgs e)
        {
            _pohitscollection = e.poHitsCollection;
        }

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
                lblDeparturePortDesc.Text = "";
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
            if (!_bFormCancelClicked && txtShipVia.Text == OCEANSHIPVIACODE || (txtShipVia.Text == "AIR"))
            {
                List<string> lstretvalues = new List<string>();
                lstretvalues = validationcls.ValidatePort(txtPortofDeparture.Text);

                if (lstretvalues[0] == "True")
                {
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
            if (!_bFormCancelClicked && txtShipVia.Text == OCEANSHIPVIACODE || (txtShipVia.Text == "AIR"))
            {
                List<string> lstretvalues = new List<string>();
                lstretvalues = validationcls.ValidatePort(txtPortofEntry.Text);
                if (lstretvalues[0] == "True")
                {
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

        private void txtDelTerms_Validating(object sender, CancelEventArgs e)
        {
            if (!_bFormCancelClicked && txtShipVia.Text == OCEANSHIPVIACODE || (txtShipVia.Text == "AIR"))
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
        #endregion

        #endregion

        #region Background worker
        private void bkgrndWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            string sStore = String.Empty;
            int istorequantity = 0;

            DataView dvSelectedStores = new DataView(dtSelectedStores);
            StringBuilder searchexpression = new StringBuilder();
            searchexpression.Append("clmSelected ='True'");

            dvSelectedStores.RowFilter = searchexpression.ToString();

            foreach (DataRowView dtrow in dvSelectedStores)
            {
                // Check if the quantity assigned every PO Line Item 
                // for that store in question is > 0. If not then skip this store (ie. do not 
                // create a PO for this store)
                if (rdBtnDropShipMatrix.Checked)
                {
                    sStore = dtrow["clmStore"].ToString();
                    istorequantity = SumStoreQuantity(sStore);

                    if (istorequantity == 0)
                    {
                        continue;
                    }
                }

                if (rdBtnDCPO.Checked == false)
                {
                    _porder.ShipTo = Convert.ToInt16(dtrow["clmStore"].ToString());
                    index++;
                }

                // Assign our Drop Ship Matrix datatable
                _porder.dtDropShipMatrix = dtDropShipMatrix;

                if (rdBtnDCPO.Checked)
                {
                    if (_pohitscollection.Count > 0)
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

                if (rdBtnDCPO.Checked)
                {
                    foreach (PurchaseOrder.POHits item in _porder.poHitsCollection)
                    {
                        int ihitnumber = item.HitNUmber;
                        int itotalquantityonhit;

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


    public class myThreadParms
    {
        POItemDetails _poitem;
        int _rowindex;

        public myThreadParms(int rowindex, POItemDetails poitem)
        {
            _rowindex = rowindex;
            _poitem = poitem;
        }

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
    }


}
