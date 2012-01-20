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
using Disney.Menu;

namespace Disney.Spice.POUI
{
    public partial class POmodification : Form
    {
        private PurchaseOrder  _porder;
        private LookupBO       lookupbo;
        private Validation     validationcls;
        private POItemDetails  _polinedetails;

        private const int      MINLANDINGVALUEFOROCN = 1;
        private const string   OCEANSHIPVIACODE = "OCN";

        private POLineDetails  polineform;
        private ProgressWindow pwindow;

        private int _itemquantityrounded = 0;

        private const string MAGICDCSTOREVATCODE = "A";

        private Boolean _bFormCancelClicked;
        private Boolean _bUserWantsToDeleteLine;
        private Boolean _bDuplicateItem;
        private Boolean _bDataBindingsInitalised;
        private Boolean IsItemClassChanged;
        private Boolean IsItemVendorChanged;
        private Boolean IsItemStyleChanged;
        private Boolean IsItemColorChanged;
        private Boolean IsItemSizeChanged;
        private Boolean IsPoModified;

        private DataTable dtSSD;
        private DataTable _dtPoLines;
        private DataTable dtFreight = new DataTable("Freight");

        private decimal _currencyratemarket;
        private decimal _ccyratemarket;
        private Color       _cellbackgroundcolor;
        private DataGridViewCellStyle dgvcsPoLinesnormal;
        private DataGridViewCellStyle dgvcsPoLinesalternate;
       
        private string _defaultshipvia = String.Empty;
        private Form _mdiparent;
        
        private ASNA.VisualRPG.Runtime.Database pgmDB;
        private Disney.Menu.Users               _username;
        private Disney.Menu.Environments        environment;

        private string     _spiceponumber;
        private int        gridcollectionindex;
        private decimal    _ccyrateprev;
        private decimal    TotalCost;
        private decimal    TotalConvertedCost;
        private decimal    TotalComponentCost;
        private decimal    TotalComponentCvtCost;
        private decimal    TotalComponentRetail;
        private decimal    Exchangerate;
                        
        #region constructors and form events
        public POmodification(ASNA.VisualRPG.Runtime.Database pgmDB, Disney.Menu.Users username, Disney.Menu.Environments paramenv, string spiceponumber)
        {
            InitializeComponent();

            this.MaximizeBox = false;

            _mdiparent = null;

            this.pgmDB = pgmDB;
            _username   = username;
            environment = paramenv;
            _spiceponumber = spiceponumber;

            SetupClassObjects();
            SetupInitialValues();
        }
        
        private void SetupInitialValues()
        {
            lblSSD.Visible = false;
            cmbSSD.Visible = false;

            cmbShipTo.Text = _porder.ShipTo.ToString();
                        
            lookupbo = new LookupBO(_porder.DbParamRef, _porder.UserName, _porder.Penvironment);

            validationcls = new Validation(_porder.DbParamRef,_porder.UserName,_porder.Penvironment);

            if (_porder.Penvironment.DateFormat == "DMY")
            {
                dtpkrAnticipateDate.CustomFormat = "d MMMM yyyy";
                dtpkrShipDate.CustomFormat = "d MMMM yyyy";
            }
            else
            {
                dtpkrAnticipateDate.CustomFormat = "MMMMd, yyyy";
                dtpkrShipDate.CustomFormat = "MMMMd, yyyy";
            }

            lblMarketValue.Text = _porder.DefaultMarket + "-" + _porder.MarketDescription;
                                                
            btnStores.Enabled = false;
            btnStores.Visible = false;

            rdBtnDropShipSingle.Visible = false;
            rdBtnDropShipMatrix.Visible = false;
            
            //if (_porder.Penvironment.Domain == "SWNA")
            if(DataCache.AreStageSetDatesChangeable == true)
            {
                lblSSD.Visible = true;
                cmbSSD.Visible = true;
                cmbSSD.Enabled = true;
                PopulateSSD();

                txtTotalRetailExVat.Visible = false;
                lblTotalRetailExVat.Visible = false;

                chkNewLineSelection.Visible = true;

                DataColumn dc = new DataColumn("FreightId", typeof(String));
                dtFreight.Columns.Add(dc);
                dc = new DataColumn("FreightDesc", typeof(String));
                dtFreight.Columns.Add(dc);

                dtFreight.Rows.Add("C", "Collect");
                dtFreight.Rows.Add("P", "Pre Pay");

                cbxFreight.DataSource = dtFreight;
                cbxFreight.DisplayMember = "FreightDesc";
                cbxFreight.ValueMember = "FreightId";
                if (_porder.Freight != String.Empty)
                {
                    cbxFreight.SelectedValue = _porder.Freight;
                }
                this.cbxFreight.SelectedIndexChanged += new System.EventHandler(this.cbxFreight_SelectedIndexChanged);

                lblFreightCharges.Visible = true;
                cbxFreight.Visible = true;
            }
            
            
            //New Rounding Functionality
            switch (_porder.ShipToMethod.ToString())
            {
                case "DC":
                    _porder.PurchaseOrderType = PurchaseOrder.POtype.StandardDCPO;
                    break;
                case "DS":
                    _porder.PurchaseOrderType = PurchaseOrder.POtype.DropShipSingle;
                    break;
                case "DM":
                    _porder.PurchaseOrderType = PurchaseOrder.POtype.DropShipMultiple;
                    break;
                default :
                    _porder.PurchaseOrderType = PurchaseOrder.POtype.StandardDCPO;
                    break;
            }
            
            //if (_porder.Penvironment.Domain == "SWNA")
            if (DataCache.IsLandingFactorRequiredForRoadDelivery == true)
            {
                txtLanding.Enabled = true;
            }
            else
            {
                txtLanding.Enabled = false;
            }
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
        private void pctBoxCurrency_Click(object sender, EventArgs e)
        {
            DataTable dt = lookupbo.CurrencyLookup();
            
            Enquiry currlookup = new Enquiry(dt, "CurrencyLookup");

            currlookup.ShowGrid();

            if (currlookup.DialogResult == DialogResult.OK)
            {
                txtCurrency.Text = currlookup.SelectedValue[0];
                lblCurrencyDesc.Text = currlookup.SelectedValue[1];
                errPOEntry.SetError(txtCurrency, string.Empty);
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
                if (_bDataBindingsInitalised == true)
                {
                    try
                    {
                        if (_porder.Currencycode == txtCurrency.Text)
                        {
                            return;
                        }

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

                            btnCreatePO.Enabled = true;
                            IsPoModified = true;

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

                            _ccyrateprev = _porder.ExchangeRate ;
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

        private void txtCurrency_Validated_1(object sender, EventArgs e)
        {
            if (!_bFormCancelClicked)
            {
                //Currency may change - recalculate summary
                if (_polinedetails != null)
                {
                    _polinedetails.LandedCost = Decimal.Round(_polinedetails.Cost * _porder.Landing, 2);
                    CalculatePOSummary();
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
                
        #endregion

        #region shipping
        private void txtShipVia_Validating(object sender, CancelEventArgs e)
        {
            if (!_bFormCancelClicked)
            {
                validationcls.HighlightErrControls(lblLanding, txtLanding, true);
                errPOEntry.SetError(txtLanding, "");

                if (_porder.ShipViaCode == txtShipVia.Text)
                {
                    e.Cancel = false;
                    return;
                }

                List<string> lstretvalues = new List<string>();

                lstretvalues = validationcls.ValidateShipVia(txtShipVia.Text);

                if (lstretvalues[0] == "True")
                {
                    lblShipViaDesc.Text = lstretvalues[1];
                    errPOEntry.SetError(txtShipVia, "");
                    validationcls.HighlightErrControls(lblShipVia, txtShipVia, true);
                    e.Cancel = false;
                    _porder.ShipViaCode = txtShipVia.Text;
                    txtLanding.Enabled = true;

                    ValidateHeaderFields(sender);

                    btnCreatePO.Enabled = true;
                    IsPoModified = true;
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
                CalculatePOSummary();
            }
            ImportControlChanges();
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

                btnCreatePO.Enabled = true;

                // Update Landed Cost values in Collection
                
                    for (int RowNbr = 0; RowNbr < dgvPOlines.Rows.Count - 1; RowNbr++)
                    {
                        short collectionindex = Convert.ToInt16(dgvPOlines["CollectionIndex", RowNbr].Value);
                        _polinedetails = _porder.lstpoLineItemDetails[collectionindex];
                                                
                        _polinedetails.LandedCost = Decimal.Round(_polinedetails.Cost * _porder.Landing, 2);
                    }
                                
                //Calculate PO Summary if not empty
                if (!String.IsNullOrEmpty(txtMarginPercent.Text)) CalculatePOSummary();
            }
        }

        private void cbxFreight_SelectedIndexChanged(object sender, EventArgs e)
        {
            _porder.Freight = GetFreightCode(cbxFreight.Text);

            btnCreatePO.Enabled = true;
            IsPoModified = true;
        }

        private String GetFreightCode(String FreightDescription)
        {
            switch (FreightDescription)
            {
                case "Collect":
                    return "C";
                case "Pre Pay":
                    return "P";
                default:
                    return "";
            }
        }
        #endregion

        #region Import details
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

        private void txtPortofDeparture_Validating(object sender, CancelEventArgs e)
        {
            if (!_bFormCancelClicked)
            {
                if (!txtPortofDeparture.Enabled) return;

                Int32 portnumber;
                if (!Int32.TryParse(txtPortofDeparture.Text, out portnumber))
                {
                    e.Cancel = true;
                    errPOEntry.SetError(txtPortofDeparture,"Invalid number");
                    return;
                }

                if (_porder.Portofdeparturecode == portnumber)
                {
                    e.Cancel = false;
                    return;
                }

                List<string> lstretvalues = new List<string>();

                lstretvalues = validationcls.ValidatePort(txtPortofDeparture.Text);

                if (lstretvalues[0] == "True")
                {
                    lblDeparturePortDesc.Text = lstretvalues[1];
                    validationcls.HighlightErrControls(lblPortofDeparture, txtPortofDeparture, true);
                    errPOEntry.SetError(txtPortofDeparture, "");
                    e.Cancel = false;
                    _porder.Portofdeparturecode = Int32.Parse(txtPortofDeparture.Text);

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

        private void txtPortofDeparture_Leave(object sender, EventArgs e)
        {
            CancelEventArgs args = new CancelEventArgs();
            txtPortofDeparture_Validating(txtPortofDeparture, args);
        }

        private void pctBxPortofEntry_Click(object sender, EventArgs e)
        {
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

        private void txtPortofEntry_Validating(object sender, CancelEventArgs e)
        {
            if (!_bFormCancelClicked)
            {
                if (!txtPortofEntry.Enabled) return;

                Int32 portnumber;
                if (!Int32.TryParse(txtPortofEntry.Text, out portnumber))
                {
                    e.Cancel = true;
                    errPOEntry.SetError(txtPortofEntry, "Invalid number");
                    return;
                }


                if (_porder.Portofentrycode == portnumber)
                {
                    e.Cancel = false;
                    return;
                }

                List<string> lstretvalues = new List<string>();

                lstretvalues = validationcls.ValidatePort(txtPortofEntry.Text);

                if (lstretvalues[0] == "True")
                {
                    lblEntryPortDesc.Text = lstretvalues[1];
                    errPOEntry.SetError(txtPortofEntry, "");
                    validationcls.HighlightErrControls(lblPortofEntry, txtPortofEntry, true);
                    e.Cancel = false;
                    _porder.Portofentrycode = Int32.Parse(txtPortofEntry.Text);

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

        private void txtPortofEntry_Leave(object sender, EventArgs e)
        {
            CancelEventArgs args = new CancelEventArgs();
            txtPortofEntry_Validating(txtPortofEntry, args);
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

        private void txtDelTerms_Validating(object sender, CancelEventArgs e)
        {
            if (!_bFormCancelClicked)
            {
                if (!txtDelTerms.Enabled) return;

                if (txtShipVia.Text != "HKU" && txtShipVia.Text != "BLP" && txtShipVia.Text != "ROD")
                {
                    if (String.IsNullOrEmpty(txtDelTerms.Text))
                    {
                        errPOEntry.SetError(txtDelTerms, "Please enter a valid Delivery Term");
                        e.Cancel = true;
                        return;
                    }
                }

                if (_porder.Deltermscode == txtDelTerms.Text)
                {
                    e.Cancel = false;
                    return;
                }

                List<string> lstretvalues = new List<string>();

                lstretvalues = validationcls.ValidateDeliveryTerms(txtDelTerms.Text);
                if (lstretvalues[0] == "True")
                {

                    lblDeliveryTermsDesc.Text = lstretvalues[1];
                    errPOEntry.SetError(txtDelTerms, "");
                    validationcls.HighlightErrControls(lblDelTerms, txtDelTerms, true);
                    _porder.Deltermscode = txtDelTerms.Text;
                    e.Cancel = false;

                    ValidateHeaderFields(sender);

                    btnCreatePO.Enabled = true;
                    IsPoModified = true;
                }
                else
                {
                    lblDeliveryTermsDesc.Text = "";
                    validationcls.HighlightErrControls(lblDelTerms, txtDelTerms, false);
                    errPOEntry.SetError(txtDelTerms, "Please enter a valid delivery term");
                    _porder.Deltermscode = "";
                    e.Cancel = true;
                    _porder.Deltermscode = "";
                }
            }
        }

        private void txtDelTerms_Leave(object sender, EventArgs e)
        {
            CancelEventArgs args = new CancelEventArgs();
            txtDelTerms_Validating(txtDelTerms, args);
        }

        private void ImportControlChanges()
        {
            switch (txtShipVia.Text)
            {
                case "AIR":
                case OCEANSHIPVIACODE:
                    txtPortofDeparture.Enabled   = true;
                    pctBxPortofDeparture.Enabled = true;
                    pctBxPortofDeparture.Visible = true;
                    txtPortofEntry.Enabled       = true;
                    pctBxPortofEntry.Enabled     = true;
                    pctBxPortofEntry.Visible     = true;
                    txtDelTerms.Enabled          = true;
                    pctBxDelTerms.Enabled        = true;
                    pctBxDelTerms.Visible        = true;
                    txtLanding.Enabled           = true;
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
                        txtLanding.Text = "0";
                        txtLanding.Enabled = false;
                    }
                    break;
                default:
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
                    break;
            }
        }

        private void txtPortofDeparture_EnabledChanged(object sender, EventArgs e)
        {
            errPOEntry.Clear();
        }
        #endregion

        #region DataGrid
        private void dgvPOlines_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1) return;

            Int16 collectionIndex = Convert.ToInt16(dgvPOlines["CollectionIndex", e.RowIndex].Value);
            _polinedetails = _porder.lstpoLineItemDetails[collectionIndex];

            if (_polinedetails.IsValid && _polinedetails.APP1 == "N")
            {
                Cursor.Current = Cursors.WaitCursor;

                polineform = new POLineDetails(_porder, ref _polinedetails, false);
                polineform.ShowDialog(this);


                dgvPOlines["Quantity", e.RowIndex].Value = _polinedetails.Itemquantity;
                dgvPOlines["ConvertedCost", e.RowIndex].Value = Decimal.Round(_polinedetails.ConvertedCost, 2).ToString("#.00");
                dgvPOlines["SavedConvertedCost", e.RowIndex].Value = Decimal.Round(_polinedetails.ConvertedCost, 2);
                dgvPOlines["ConvertedCost6", e.RowIndex].Value = Decimal.Round(_polinedetails.ConvertedCost, 6);

                _polinedetails.Cost = Decimal.Round(_polinedetails.ConvertedCost * _porder.ExchangeRate, 2);
                dgvPOlines["Cost", e.RowIndex].Value = _polinedetails.Cost;

                if (_porder.Landing == 0) _porder.Landing = 1;

                _polinedetails.LandedCost = Decimal.Round(_polinedetails.Cost * _porder.Landing, 2);

                CalculatePOSummary();

                polineform = null;

            }
            else if (_polinedetails.IsValid && _polinedetails.APP1 == "Y")
            {
                Cursor.Current = Cursors.WaitCursor;

                POLineDetailsPack polinedetailspack = new POLineDetailsPack(_polinedetails, _porder, e.RowIndex);

                polinedetailspack.OnAppQuantityChanged += new POLineDetailsPack.AppQuantityChangedEventHandler(polinedetailspack_OnAppQuantityChanged);
                polinedetailspack.ShowDialog(this);

                if (Convert.ToDecimal(dgvPOlines["ConvertedCost", e.RowIndex].Value) != _polinedetails.ConvertedCost)
                {
                    btnCreatePO.Enabled = true;
                    IsPoModified = true;
                }

                dgvPOlines["Quantity", e.RowIndex].Value = _polinedetails.Itemquantity;
                dgvPOlines["ConvertedCost", e.RowIndex].Value = Decimal.Round(_polinedetails.ConvertedCost, 2).ToString("#.00");
                dgvPOlines["SavedConvertedCost", e.RowIndex].Value = Decimal.Round(_polinedetails.ConvertedCost, 2);
                dgvPOlines["ConvertedCost6", e.RowIndex].Value = Decimal.Round(_polinedetails.ConvertedCost, 6);

                CalculateAPPComponentCost();
                _polinedetails.Cost = Decimal.Round(TotalCost, 2);
                dgvPOlines["Cost", e.RowIndex].Value = _polinedetails.Cost;

                if (_porder.Landing == 0) _porder.Landing = 1;

                _polinedetails.LandedCost = Decimal.Round(_polinedetails.Cost * _porder.Landing, 2);

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

            dgvPOlines.Rows[e.rowindex].Cells["ConvertedCost"].Value = Decimal.Round(e.poline.ConvertedCost, 2).ToString("#.00");
            dgvPOlines.Rows[e.rowindex].Cells["SavedConvertedCost"].Value = Decimal.Round(_polinedetails.ConvertedCost, 2);

            CalculatePOSummary();
        }

        private void dgvPOlines_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (_bFormCancelClicked) return;

            if (_bUserWantsToDeleteLine) return;
                        
            if (dgvPOlines.Rows[e.RowIndex].IsNewRow) return;

            if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Class"))
            {
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

                    short itemclass = Convert.ToInt16(dgvPOlines["Class", e.RowIndex].Value);
                    if (itemclass != Int16.Parse(e.FormattedValue.ToString()))
                    {
                        IsItemClassChanged = true;
                    }
                }
            }

            else if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Vendor"))
            {
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
                    _polinedetails.Vendorcode = Int32.Parse(e.FormattedValue.ToString());
                    _polinedetails.Vendordesc = retValues[1].ToString();
                    e.Cancel = false;

                    int vendor = Convert.ToInt32(dgvPOlines["Vendor", e.RowIndex].Value);
                    if (vendor != Int32.Parse(e.FormattedValue.ToString()))
                    {
                        IsItemVendorChanged = true;
                    }
                }

            }

            else if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Style"))
            {
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

                    short color = Convert.ToInt16(dgvPOlines["Color", e.RowIndex].Value);
                    if (color != Int16.Parse(e.FormattedValue.ToString()))
                    {
                        IsItemColorChanged = true;
                    }
                }
            }

            else if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("Size"))
            {
                if (String.IsNullOrEmpty(e.FormattedValue.ToString()))
                {
                    dgvPOlines.Rows[e.RowIndex].ErrorText = string.Empty;
                    return;
                }

                List<string> retValues = validationcls.ValidateSize(e.FormattedValue.ToString());
                if ("False" == retValues[0])
                {
                    dgvPOlines.Rows[e.RowIndex].ErrorText = "Please enter valid Size";
                    e.Cancel = true;
                    return;
                }

                _polinedetails.Itemsize = Int16.Parse(e.FormattedValue.ToString());
                _polinedetails.Sizename = retValues[1];

                if (dgvPOlines["Size", e.RowIndex].Value != null)
                {
                    Int16 size = Convert.ToInt16(dgvPOlines["Size", e.RowIndex].Value);
                    if (size != _polinedetails.Itemsize)
                    {
                        IsItemSizeChanged = true;
                    }
                }
                else
                {
                    IsItemSizeChanged = true;
                }

                short iItem;
                int iVendor;
                short iStyle;
                short iColour;
                short iSize;

                Int16.TryParse(Convert.ToString(dgvPOlines.Rows[e.RowIndex].Cells["Class"].Value), out iItem);
                Int32.TryParse(Convert.ToString(dgvPOlines.Rows[e.RowIndex].Cells["Vendor"].Value), out iVendor);
                Int16.TryParse(Convert.ToString(dgvPOlines.Rows[e.RowIndex].Cells["Style"].Value), out iStyle);
                Int16.TryParse(Convert.ToString(dgvPOlines.Rows[e.RowIndex].Cells["Color"].Value), out iColour);
                Int16.TryParse(Convert.ToString(dgvPOlines.Rows[e.RowIndex].Cells["Size"].Value), out iSize);

                _bDuplicateItem = CheckForDuplicateLine(e.RowIndex, iItem, iVendor, iStyle, iColour, _polinedetails.Itemsize);

                
                if (!IsOkToDoItemLookup(e.RowIndex, iItem, iVendor, iStyle, iColour, iStyle))
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
                    dgvPOlines.Rows[e.RowIndex].Cells["Season"].Value       = _polinedetails.SeasonDesc;
                    dgvPOlines.Rows[e.RowIndex].Cells["CasePackType"].Value = _polinedetails.Packdescription;
                    dgvPOlines.Rows[e.RowIndex].Cells["TicketType"].Value   = _polinedetails.Tickettype;
                    dgvPOlines.Rows[e.RowIndex].Cells["Pack"].Value         = _polinedetails.APP1;
                    dgvPOlines.Rows[e.RowIndex].Cells["Sequence"].Value     = _polinedetails.Sequence;

                    if (_polinedetails.APP1 == "Y")         
                    {
                        _porder.NumofPOPacks ++;

                        dgvPOlines.Rows[e.RowIndex].Cells["ConvertedCost"].ReadOnly = true;

                        AssortedPrePack AssPrePack = new AssortedPrePack(_polinedetails, _porder);
                        DataTable prePackTbl = AssPrePack.PopulateAPPStructure();

                        _polinedetails.Components.Clear();

                        TotalComponentCost = 0;
                        TotalComponentCvtCost = 0;
                        TotalComponentRetail = 0;

                        for (int i = 0; i < prePackTbl.Rows.Count; i++)
                        {
                            APPcomponent component = new APPcomponent();

                            component.ComponentClass  =   (Int16)prePackTbl.Rows[i]["ComponentClass"];
                            component.ComponentVendor =   (Int32)prePackTbl.Rows[i]["ComponentVendor"];
                            component.ComponentStyle  =   (Int16)prePackTbl.Rows[i]["ComponentStyle"];
                            component.ComponentColour =   (Int16)prePackTbl.Rows[i]["ComponentColour"];
                            component.ComponentSize   =   (Int16)prePackTbl.Rows[i]["ComponentSize"];
                            component.RatioQuantity   =   (Int16)prePackTbl.Rows[i]["ComponentQuantity"];
                            component.Cost            = Decimal.Round((Decimal)prePackTbl.Rows[i]["ComponentCost"] / _porder.ExchangeRate, 2);
                            component.UnConvertedCost = Decimal.Round((Decimal)prePackTbl.Rows[i]["ComponentCost"], 2);
                            component.Retail          = (Decimal)prePackTbl.Rows[i]["Retail"];
                            component.ItemDescription =  (String)prePackTbl.Rows[i]["ComponentLongDesc"];

                            _polinedetails.Components.Add(component);

                            //Re-Calculate Cost, Converted Cost and Retail from the Components
                            TotalComponentCost += Decimal.Round(component.Cost * component.RatioQuantity, 2);
                            TotalComponentCvtCost += Decimal.Round((Decimal)prePackTbl.Rows[i]["ComponentCost"] * component.RatioQuantity, 2);
                            TotalComponentRetail += component.Retail * component.RatioQuantity;
                        }

                        //Re -Assign values
                        _polinedetails.Cost       = TotalComponentCvtCost;
                        _polinedetails.LandedCost = TotalComponentCvtCost;
                        dgvPOlines.Rows[e.RowIndex].Cells["Cost"].Value = _polinedetails.Cost.ToString();

                        _polinedetails.ConvertedCost = TotalComponentCost;
                        dgvPOlines.Rows[e.RowIndex].Cells["ConvertedCost"].Value = Decimal.Round(_polinedetails.ConvertedCost, 2).ToString("#.00");
                        dgvPOlines.Rows[e.RowIndex].Cells["ConvertedCost6"].Value = _polinedetails.ConvertedCost;
                        dgvPOlines.Rows[e.RowIndex].Cells["SavedConvertedCost"].Value = _polinedetails.ConvertedCost;
                        dgvPOlines.Rows[e.RowIndex].Cells["LandedCost"].Value = _polinedetails.LandedCost;

                        _polinedetails.Retailprice = TotalComponentRetail;
                        dgvPOlines.Rows[e.RowIndex].Cells["Retail"].Value = _polinedetails.Retailprice.ToString();
                    }
                    else
                    {
                        dgvPOlines.Rows[e.RowIndex].Cells["ConvertedCost"].ReadOnly = false;
                        dgvPOlines.Rows[e.RowIndex].Cells["ConvertedCost"].ReadOnly = false;
                        _polinedetails.ConvertedCost = _polinedetails.Cost / _porder.ExchangeRate;
                        dgvPOlines.Rows[e.RowIndex].Cells["ConvertedCost6"].Value = _polinedetails.ConvertedCost;
                        dgvPOlines.Rows[e.RowIndex].Cells["ConvertedCost"].Value = Decimal.Round(_polinedetails.ConvertedCost, 2).ToString("#.00");
                        dgvPOlines.Rows[e.RowIndex].Cells["SavedConvertedCost"].Value = Decimal.Round(_polinedetails.ConvertedCost, 2);
                    } 
                        
                    if (_porder.Landing == 0)
                    {
                        _porder.Landing = 1;
                    }

                    _polinedetails.LandedCost = Decimal.Round(_polinedetails.Cost * _porder.Landing, 2);

                    _polinedetails.IsValid = true;
                    e.Cancel = false;

                    IsItemClassChanged  = false;
                    IsItemVendorChanged = false;
                    IsItemStyleChanged  = false;
                    IsItemColorChanged  = false;
                    IsItemSizeChanged   = false;

                    btnCreatePO.Enabled = true;
                }
                else
                {
                    dgvPOlines.Rows[e.RowIndex].Cells["Description"].Value   = _polinedetails.Itemlongdescription;
                    dgvPOlines.Rows[e.RowIndex].Cells["Retail"].Value        = String.Empty;
                    dgvPOlines.Rows[e.RowIndex].Cells["Cost"].Value          = String.Empty;
                    dgvPOlines.Rows[e.RowIndex].Cells["Character"].Value     = String.Empty;
                    dgvPOlines.Rows[e.RowIndex].Cells["Season"].Value        = String.Empty;
                    dgvPOlines.Rows[e.RowIndex].Cells["CasePackType"].Value  = String.Empty;
                    dgvPOlines.Rows[e.RowIndex].Cells["TicketType"].Value    = String.Empty;
                    dgvPOlines.Rows[e.RowIndex].Cells["Pack"].Value          = String.Empty;
                    dgvPOlines.Rows[e.RowIndex].Cells["Quantity"].Value      = String.Empty;
                    dgvPOlines.Rows[e.RowIndex].Cells["ConvertedCost"].Value = String.Empty;

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
                }
            }

            else    if (dgvPOlines.Columns[e.ColumnIndex].Name.Equals("ConvertedCost"))
            {
                if (_polinedetails.IsValid == false) return;

                if (String.IsNullOrEmpty(e.FormattedValue.ToString()))
                {
                    dgvPOlines.Rows[e.RowIndex].ErrorText = "Please enter a cost value";
                    e.Cancel = true;
                    return;
                }
                
                decimal convertedcost;

                if (!Decimal.TryParse(e.FormattedValue.ToString(), out convertedcost))
                {
                    dgvPOlines.Rows[e.RowIndex].ErrorText = "You have entered an invalid cost value";
                    e.Cancel = true;
                    return;
                }

                //Only two decimal places will get written to the DB
                //Added 06/05/2011 Joseph Urbina 
                decimal DecTest = Math.Round(convertedcost, 2);
                if (DecTest == 0)
                {
                    dgvPOlines.Rows[e.RowIndex].ErrorText = "You have entered an invalid cost value";
                    e.Cancel = true;
                    return;
                }

                if (convertedcost <= 0)
                {
                    dgvPOlines.Rows[e.RowIndex].ErrorText = "Cost value cannot be zero or negative";
                    e.Cancel = true;
                    return;
                }


                if (Convert.ToDecimal(dgvPOlines.Rows[e.RowIndex].Cells["SavedConvertedCost"].Value) == convertedcost)
                {
                    if (_polinedetails.APP1 != "Y")
                    {
                        _polinedetails.ConvertedCost = Convert.ToDecimal(dgvPOlines.Rows[e.RowIndex].Cells["ConvertedCost6"].Value);
                    }
                }
                else
                {
                    _polinedetails.ConvertedCost = convertedcost;
                    dgvPOlines.Rows[e.RowIndex].Cells["ConvertedCost"].Value = Decimal.Round(_polinedetails.ConvertedCost, 2).ToString("#.00");
                    dgvPOlines.Rows[e.RowIndex].Cells["SavedConvertedCost"].Value = Decimal.Round(_polinedetails.ConvertedCost, 2);
                    dgvPOlines.Rows[e.RowIndex].Cells["ConvertedCost6"].Value = _polinedetails.ConvertedCost;
                                            
                    _polinedetails.Cost = Decimal.Round(_polinedetails.ConvertedCost * _porder.ExchangeRate, 2);
                    
                    dgvPOlines.Rows[e.RowIndex].Cells["Cost"].Value = _polinedetails.Cost;
                                           
                    _polinedetails.LandedCost = Decimal.Round(_polinedetails.Cost * _porder.Landing, 2);

                    CalculatePOSummary();
                }

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
                                    
                if (_porder.ShipToRounding.ToString() == "C")
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
                            dgvPOlines.Rows[e.RowIndex].ErrorText = "Quantity must be rounded to the nearest CasePack quantity";
                            e.Cancel = true;
                            return;
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
                            dgvPOlines.Rows[e.RowIndex].ErrorText = "Quantity must be rounded to the nearest Distro quantity";
                            e.Cancel = true;
                            return;
                        }
                    }
                }

                CalculatePOSummary();

                btnCreatePO.Enabled = true;
                IsPoModified = true;
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
                    if ((!_polinedetails.IsValid) && IsOrderValid())
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
            catch (Exception)
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
                _polinedetails.Sequence = GetNextPOlineSequence();
                
                _porder.lstpoLineItemDetails.Add(_polinedetails); 
                 _polinedetails.IsNewLine = true;
                                
                int index = _porder.lstpoLineItemDetails.IndexOf(_polinedetails);
                dgvPOlines.Rows[e.Row.Index - 1].Cells["CollectionIndex"].Value = index;
            }
        }

        private short GetNextPOlineSequence()
        {
            short sequenceindex = 0;
            if (_porder.lstpoLineItemDetails.Count == 0)
            {
                return 1;
            }
            else
            {
                sequenceindex = _porder.lstpoLineItemDetails[_porder.lstpoLineItemDetails.Count -1].Sequence;
                return ++sequenceindex;
            }
        }

        private short GetLastPOlineSequence()
        {
            if (_porder.lstpoLineItemDetails.Count == 0)
            {
                return 0;
            }
            else
            {
                return _porder.lstpoLineItemDetails[_porder.lstpoLineItemDetails.Count - 1].Sequence;
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
                int gridcollectionindex = Convert.ToInt16(dgvPOlines["CollectionIndex", e.RowIndex].Value);
                _polinedetails = _porder.lstpoLineItemDetails[gridcollectionindex];
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
        }
        
        private void dgvPOlines_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == -1 || dgvPOlines.Rows[e.RowIndex].IsNewRow)
            {
                return;
            }
                        

            if (_bDuplicateItem = CheckForDuplicateLine(e.RowIndex, _polinedetails.ClassCode, _polinedetails.Vendorcode, _polinedetails.Stylecode, _polinedetails.Colorcode, _polinedetails.Itemsize))
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

        #endregion Item DataGrid
                
        private Boolean CheckRequiredFields()
        {
            // Check to see if the PO Line Items are valid
            Boolean bSuccess = true;

            if (!String.IsNullOrEmpty(errPOEntry.GetError(txtPortofDeparture)))
            {
                bSuccess = false;
            }
            
            if (!String.IsNullOrEmpty(errPOEntry.GetError(txtPortofEntry)))
            {
                bSuccess = false;
            }

            if (!String.IsNullOrEmpty(errPOEntry.GetError(txtDelTerms)))
            {
                bSuccess = false;
            }

            // Check that the Ship Date is less than Anticipate date
            if ((dtpkrAnticipateDate.Value.Date < dtpkrShipDate.Value.Date))
            {
                errPOEntry.SetError(dtpkrShipDate, "Shipping date cannot be after the Anticipate date");
                bSuccess = false;
            }

            // Check that the Ship via is not empty
            if (txtShipVia.Text == string.Empty)
            {
                errPOEntry.SetError(txtShipVia, "Ship Via cannot be blank");
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
                if (item.IsDeleted == false)
                {
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
            cmbSSD.Items.Add(string.Empty);

            dtSSD = lookupbo.PopulateSSD();

            foreach (DataRow drow in dtSSD.Rows)
            {
               cmbSSD.Items.Add(drow["clmSSDDATMDY"].ToString());
            }
            cmbSSD.SelectedIndex = 0;

            DataColumn[] key = new DataColumn[] { dtSSD.Columns["clmSSDDATMDY"] };
            dtSSD.PrimaryKey = key;
                       
        }

        private void dtpkrAnticipateDate_Validating(object sender, CancelEventArgs e)
        {
            if (!_bFormCancelClicked)
            {
                errPOEntry.SetError(dtpkrAnticipateDate, "");
                
                _porder.AnticipateDate = dtpkrAnticipateDate.Value;
                btnCreatePO.Enabled = true;
                IsPoModified = true;
            }
        }
        
        private void dtpkrShipDate_Validating(object sender, CancelEventArgs e)
        {
            if (!_bFormCancelClicked)
            {
                errPOEntry.SetError(dtpkrShipDate, string.Empty);
                
                txtCancelDate.Text = dtpkrShipDate.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate).ToString(DateFormats.PO_CancelDate);
                _porder.CancelDate = dtpkrShipDate.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate);


                btnCreatePO.Enabled = true;
                IsPoModified = true;
            }
        }

        private void grpBoxDates_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(errPOEntry.GetError(dtpkrAnticipateDate)) & string.IsNullOrEmpty(errPOEntry.GetError(dtpkrShipDate)))
            {
                _porder.AnticipateDate = dtpkrAnticipateDate.Value;
                _porder.ShippingDate = dtpkrShipDate.Value;
                if (dtpkrAnticipateDate.Value.Date < dtpkrShipDate.Value.Date)
                {
                    errPOEntry.SetError(dtpkrAnticipateDate, "Anticipate date cannot be before the ship date");
                    return;
                }
                
                txtCancelDate.Text = dtpkrShipDate.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate).ToString(DateFormats.PO_CancelDate);
                _porder.CancelDate = dtpkrShipDate.Value.AddDays(DataCache.DaysBetweenShipDateAndCancelDate);
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

                btnCreatePO.Enabled = true;
                IsPoModified = true;
            }
        }
        #endregion Dates

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

            _porder.TotalRetail     = _porder.CalculateTotalRetail();
            _porder.TotalCost       = _porder.CalculateTotalCost();
            _porder.TotalUnits      = _porder.CalculateTotalUnit();
            _porder.TotalCostPOCurr = _porder.CalculateTotalCostInPoCurrency();

            if (_porder.Landing == 0)
            {
                _porder.Landing = 1;
            }
                
            _porder.TotalLandedCost = Decimal.Round(_porder.CalculateTotalLandedCost(), 2);

            totalretailexvat = Decimal.Round(_porder.CalculateTotalRetailExVat(MAGICDCSTOREVATCODE), 2);
            txtTotalRetail.Text = Decimal.Round(_porder.TotalRetail, 2).ToString("N");

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

           
            txtTotalCost.Text = _porder.TotalLandedCost.ToString("N");
           
            txtTotalUnits.Text = _porder.TotalUnits.ToString("G");

            txtTotalRetailExVat.Text = totalretailexvat.ToString("N");
                
            if (_porder.MarginValue < 0)
            {
                validationcls.HighlightErrControls(lblMarginValue, txtMarginValue, false);                     
            }
            else
            {
                validationcls.HighlightErrControls(lblMarginValue, txtMarginValue, true);  
            }

            txtMarginValue.Text   = Decimal.Round(_porder.MarginValue, 2).ToString("N");
            txtMarginPercent.Text = _porder.MarginPercentage.ToString("N");
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
                cost = Decimal.Round(_polinedetails.Components[iCount].Cost * _porder.ExchangeRate, 2);

                TotalCost += cost * _polinedetails.Components[iCount].RatioQuantity;
            }
        }
        
        private DataTable PopulatePOLines()
        {
            DataTable dtAllPOLines = new DataTable();

            dtAllPOLines.Columns.Add("POnumber",    typeof(string));
            dtAllPOLines.Columns.Add("Version",     typeof(Int16));
            dtAllPOLines.Columns.Add("Sequence",    typeof(Int16));
            dtAllPOLines.Columns.Add("Class",       typeof(Int16));
            dtAllPOLines.Columns.Add("Vendor",      typeof(Int32));
            dtAllPOLines.Columns.Add("Style",       typeof(Int16));
            dtAllPOLines.Columns.Add("Colour",      typeof(Int16));
            dtAllPOLines.Columns.Add("Size",        typeof(Int16));
            dtAllPOLines.Columns.Add("SKU",         typeof(Int32));
            dtAllPOLines.Columns.Add("SKUCHK",      typeof(Int16));       
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
            dtAllPOLines.Columns.Add("Character",   typeof(string));

            for (int iCount = 0; iCount < _porder.lstpoLineItemDetails.Count; iCount++)
            {
                _polinedetails = _porder.lstpoLineItemDetails[iCount];

                if (_polinedetails.IsDeleted == false)
                {
                    dtAllPOLines.Rows.Add(_porder.SpicePOnumber,
                                          _porder.SpicePOversion,
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
                                          _polinedetails.ConvertedCost,
                                          _porder.Landing,
                                          _polinedetails.Charactercode
                                    );
                }
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

            btnCreatePO.Enabled = true;
            IsPoModified = true;
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
                    if (1==1)
                    {
                        dgvPOlines.Rows.RemoveAt(iLoopCounter);

                        iRowsDeleted++;
                        iRunningCountTotalRows = dgvPOlines.Rows.Count;
                    }
                }
                else
                {
                    iLoopCounter++;
                }

            } while (iLoopCounter < iRunningCountTotalRows);

            _bUserWantsToDeleteLine = false;

            if (iRowsDeleted > 0)
            {
            }
        }

        void itemqtyform_OnQuantityRounded(object sender, int iroundedquantity)
        {
            _itemquantityrounded = iroundedquantity;
        }

        private Boolean CheckForDuplicateLine(int currentrow, short classcode, int vendor, short style, short colour, short size)
        {
            for (int index = 0; index < _porder.lstpoLineItemDetails.Count; index++)
            {
                if (_porder.lstpoLineItemDetails[index].IsValid && (index != currentrow))
                {
                    if (_porder.lstpoLineItemDetails[index].ClassCode     == classcode
                        && _porder.lstpoLineItemDetails[index].Vendorcode == vendor
                        && _porder.lstpoLineItemDetails[index].Stylecode  == style
                        && _porder.lstpoLineItemDetails[index].Colorcode  == colour
                        && _porder.lstpoLineItemDetails[index].Itemsize   == size
                        && _porder.lstpoLineItemDetails[index].IsDeleted  == false)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
                
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

            if (txtBox.Name == "txtCurrency")
            {
                if (String.IsNullOrEmpty(txtDept.Text))
                {
                    return false;
                }

                if (String.IsNullOrEmpty(txtVendor.Text))
                {
                    return false;
                }

                if (String.IsNullOrEmpty(txtCurrency.Text))
                {
                    return false;
                }

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

                if (String.IsNullOrEmpty(txtPortofDeparture.Text))
                {
                    return false;
                }

                if (String.IsNullOrEmpty(txtPortofEntry.Text))
                {
                    return false;
                }

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

                return true;
            }

            return false;
        }

        private void SetupClassObjects()
        {
            _porder = new PurchaseOrder(pgmDB, _username, environment);
            _porder.GetPOHeader(_spiceponumber);

            _porder.GetPOComments(_spiceponumber, _porder.SpicePOversion);

            _dtPoLines = _porder.GetPOItems(_spiceponumber, _porder.SpicePOversion);
        }

        private void SetupSimpleDataBinding()
        {
            Boolean bSuccess;

            ItemsBO.Items itembo = new Items(pgmDB, _username, environment);

            itembo.GetMarket(_porder.DefaultMarket);

            lblMarketValue.Text = _porder.DefaultMarket + "-" + itembo.MarketName;

            _porder.MarketCurrency = itembo.MarketCurrency;
            lblCurrVal1.Text = "(" + _porder.MarketCurrency + ")";
            lblCurrValue.Text = "(" + _porder.MarketCurrency + ")";

            itembo.GetCurrency(itembo.MarketCurrency);

            _currencyratemarket = itembo.CurrencyRate;
            _ccyratemarket      = itembo.CurrencyRate;

            txtDept.Text = _porder.Deptcode.ToString ();
            itembo.GetDepartment(_porder.Deptcode);
            lblDeptDesc.Text = itembo.DepartmentName;
            
            txtVendor.Text = _porder.Vendorcode.ToString();
            itembo.GetVendor(_porder.Vendorcode);
            lblVendorDesc.Text = itembo.VendorName;

            txtCurrency.Text = _porder.Currencycode;
            itembo.GetCurrency(_porder.Currencycode);

            Decimal XChgRateDec = Decimal.Round(_porder.ExchangeRate, 6);
            String XChgRate = XChgRateDec.ToString();
            
            if (XChgRate == "1")
            { XChgRate = "1.000000"; };
            
            lblCurrencyDesc.Text = itembo.CurrencyName + " (" + XChgRate + ")";
            _ccyrateprev = _porder.ExchangeRate;

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
            
            decimal idisplaylandingfactor = _porder.Landing - 1;

            txtLanding.Text = Math.Round(idisplaylandingfactor, 5).ToString();
            
            txtPortofDeparture.Text = _porder.Portofdeparturecode.ToString();
            itembo.GetPort(_porder.Portofdeparturecode);
            lblDeparturePortDesc.Text = itembo.PortDescription;
            
            txtPortofEntry.Text = _porder.Portofentrycode.ToString(); ;
            itembo.GetPort(_porder.Portofentrycode);
            lblEntryPortDesc.Text = itembo.PortDescription;
            
            txtDelTerms.Text = _porder.Deltermscode;
            itembo.GetDelTerms (_porder.Deltermscode);
            lblDeliveryTermsDesc.Text = itembo.DelTermsDescription;

            ImportControlChanges();

            dtpkrShipDate.Value = _porder.ShippingDate;
            
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

            if (_porder.Penvironment.DateFormat == "DMY")
            {
                txtOrderDate.Text = _porder.OrderDate.ToLongDateString();
                txtCancelDate.Text = _porder.CancelDate.ToLongDateString();
            }
            else
            {
                txtOrderDate.Text = _porder.OrderDate.ToString("MMMM d, yyyy");
                txtCancelDate.Text = _porder.CancelDate.ToString("MMMM d, yyyy");
            }

            txtVendorComment1.Text = _porder.Vendorcomments1;
            txtVendorComment2.Text = _porder.Vendorcomments2;
            txtVendorComment3.Text = _porder.Vendorcomments3;
            txtInternalComments1.Text = _porder.Internalcomments1;
            txtInternalComments2.Text = _porder.Internalcomments2;

            lblPONumber.Text = _porder.SpicePOnumber;

            //if (_porder.Penvironment.Domain == "SWNA")
            if (DataCache.AreStageSetDatesChangeable == true)
            {
                StringBuilder date = new StringBuilder();
                date.Append(GetMonthDesc(_porder.StageSetDate.Month));
                date.Append(_porder.StageSetDate.Day.ToString("00"));
                date.Append(" - ");
                date.Append(_porder.StageSetDate.Year.ToString());

                int i = cmbSSD.FindStringExact(date.ToString());
                cmbSSD.SelectedIndex = i;

                this.cmbSSD.SelectedIndexChanged += new System.EventHandler(this.cmbSSD_SelectedIndexChanged);
            }

            if (!String.IsNullOrEmpty(_porder.IPPOnumber.Trim()))
            {
                lblIP.Visible = true;
                lblIPPoNumber.Text = _porder.IPPOnumber;
                btnDisplayEDIdates.Visible = true;
            }
            else
            {
                btnDisplayEDIdates.Visible = false;
            }
            
            chkNewLineSelection.Checked = _porder.IsPONewLine;
            this.chkNewLineSelection.CheckedChanged += new System.EventHandler(this.chkNewLineSelection_CheckedChanged);
            
            _bDataBindingsInitalised = true;

            DisplayPoItems();
        }

        private string GetMonthDesc(int monthnumber)
        {
            switch (monthnumber)
            {
                case 1:
                    return "JAN - ";
                case 2:
                    return "FEB - ";
                case 3:
                    return "MAR - ";
                case 4:
                    return "APR - ";
                case 5:
                    return "MAY - ";
                case 6:
                    return "JUN - ";
                case 7:
                    return "JUL - ";
                case 8:
                    return "AUG - ";
                case 9:
                    return "SEP - ";
                case 10:
                    return "OCT - ";
                case 11:
                    return "NOV - ";
                case 12:
                    return "DEC - ";
                default:
                    throw new NotImplementedException();
            }
        }

        private void DisplayOriginalPoHeader()
        {
            Boolean bSuccess;

            ItemsBO.Items itembo = new Items(pgmDB, _username, environment);
            itembo.GetMarket(_porder.DefaultMarket);

            lblMarketValue.Text = _porder.DefaultMarket + "-" + itembo.MarketName;

            _porder.MarketCurrency = itembo.MarketCurrency;
            lblCurrVal1.Text = "(" + _porder.MarketCurrency + ")";
            lblCurrValue.Text = "(" + _porder.MarketCurrency + ")";

            itembo.GetCurrency(itembo.MarketCurrency);

            _currencyratemarket = itembo.CurrencyRate;

            txtDept.Text = _porder.Deptcode.ToString();
            itembo.GetDepartment(_porder.Deptcode);
            lblDeptDesc.Text = itembo.DepartmentName;

            txtVendor.Text = _porder.Vendorcode.ToString();
            itembo.GetVendor(_porder.Vendorcode);
            lblVendorDesc.Text = itembo.VendorName;

            txtCurrency.Text = _porder.Currencycode;
            itembo.GetCurrency(_porder.Currencycode);

            Decimal XChgRateDec = Decimal.Round(_porder.ExchangeRate, 6);
            String XChgRate = XChgRateDec.ToString();
           
            if (XChgRate == "1")
            { XChgRate = "1.000000"; };
                        
            lblCurrencyDesc.Text = itembo.CurrencyName + " (" + XChgRate + ")";
            _ccyrateprev = _porder.ExchangeRate;

            txtTerms.Text = _porder.Termscode;
            bSuccess = itembo.GetTerms(_porder.Termscode);
            if (bSuccess == true)
            {
                lblTermsDesc.Text = itembo.VendorTermsDescription;
            }

            txtShipVia.Text = _porder.ShipViaCode;
            itembo.GetShipVia(_porder.ShipViaCode);
            lblShipViaDesc.Text = itembo.ShipViaDescription;

            decimal idisplaylandingfactor = _porder.Landing - 1;

            txtLanding.Text = Math.Round(idisplaylandingfactor, 5).ToString();

            txtPortofDeparture.Text = _porder.Portofdeparturecode.ToString();
            itembo.GetPort(_porder.Portofdeparturecode);
            lblDeparturePortDesc.Text = itembo.PortDescription;

            txtPortofEntry.Text = _porder.Portofentrycode.ToString(); ;
            itembo.GetPort(_porder.Portofentrycode);
            lblEntryPortDesc.Text = itembo.PortDescription;

            txtDelTerms.Text = _porder.Deltermscode;
            itembo.GetDelTerms(_porder.Deltermscode);
            lblDeliveryTermsDesc.Text = itembo.DelTermsDescription;

            ImportControlChanges();

            dtpkrShipDate.Value = _porder.ShippingDate;

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

            txtVendorComment1.Text = _porder.Vendorcomments1;
            txtVendorComment2.Text = _porder.Vendorcomments2;
            txtVendorComment3.Text = _porder.Vendorcomments3;

            txtInternalComments1.Text = _porder.Internalcomments1;
            txtInternalComments2.Text = _porder.Internalcomments2;

            lblPONumber.Text = _porder.SpicePOnumber;
                        
            //if (_porder.Penvironment.Domain == "SWNA")
            if (DataCache.AreStageSetDatesChangeable == true)
            {
                int i = cmbSSD.FindStringExact(_porder.StageSetDate.ToLongDateString());
                cmbSSD.SelectedIndex = i;
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
                            dgvPOlines.Rows[iStartRow].Cells["Cost"].ReadOnly = false;
                        }

                        dgvPOlines.Rows[iStartRow].ReadOnly = true;
                    }
                }
            }

            CalculatePOSummary();
        }

        private void DisplayPoItems()
        {
            char padchar = Convert.ToChar(" ");
            Boolean bItemLookupError = false;

            DataTable dtComponents;

            //if (_dtPoLines.Rows.Count > 0)
            if (_dtPoLines != null)
            {
                //iStartRow = iTotalRowsInGrid = dgvPOlines.Rows.Count;

                foreach (DataRow dr in _dtPoLines.Rows)
                {
                    int RowIndex = dgvPOlines.Rows.Add(false, dr["Class"], dr["Vendor"], dr["Style"], dr["Colour"], dr["Size"]);

                    // Create a new item for the POItemDetails collection
                    POItemDetails poitem = new POItemDetails();

                    poitem.ClassCode  = (short)dr["Class"];
                    poitem.Vendorcode =   (int)dr["Vendor"];
                    poitem.Stylecode  = (short)dr["Style"];
                    poitem.Colorcode  = (short)dr["Colour"];
                    poitem.Itemsize   = (short)dr["Size"];
                    
                    //poitem.Sequence = Convert.ToInt16(iStartRow + 1);
                    poitem.Sequence   = (short)dr["Sequence"];
                    dgvPOlines.Rows[RowIndex].Cells["Sequence"].Value = poitem.Sequence;
                    dgvPOlines.Rows[RowIndex].Cells["CollectionIndex"].Value = RowIndex;

                    List<string> retValues = validationcls.ValidateClass(poitem.ClassCode.ToString());
                    poitem.Classname = retValues[1].ToString();

                    retValues = validationcls.ValidateVendor(poitem.Vendorcode.ToString(), true);
                    poitem.Vendordesc = retValues[1].ToString();

                    retValues = validationcls.ValidateColour(poitem.Colorcode.ToString());
                    poitem.Colordesc = retValues[1];

                    retValues = validationcls.ValidateSize(poitem.Itemsize.ToString());
                    poitem.Sizename = retValues[1];

                    // Lookup the item to populate the class with the rest of the details
                    if (poitem.ItemLookup(_porder.DbParamRef, _porder.UserName, _porder.Penvironment, _porder.DefaultMarket))
                    {
                        dgvPOlines.Rows[RowIndex].Cells["Description"].Value = poitem.Itemlongdescription;
                        dgvPOlines.Rows[RowIndex].Cells["Character"].Value = poitem.Characterdesc;
                        dgvPOlines.Rows[RowIndex].Cells["Season"].Value = poitem.SeasonDesc;
                        dgvPOlines.Rows[RowIndex].Cells["CasePackType"].Value = poitem.Packdescription;
                        dgvPOlines.Rows[RowIndex].Cells["TicketType"].Value = poitem.Tickettype;
                        dgvPOlines.Rows[RowIndex].Cells["Pack"].Value = poitem.APP1;

                        Decimal retail = Convert.ToDecimal(_dtPoLines.Rows[RowIndex]["Retail"]);
                        dgvPOlines.Rows[RowIndex].Cells["Retail"].Value = retail;
                        poitem.Retailprice = retail;

                        Int32 qty = Convert.ToInt32(_dtPoLines.Rows[RowIndex]["Quantity"]);
                        dgvPOlines.Rows[RowIndex].Cells["Quantity"].Value = qty;
                        poitem.Itemquantity = qty;
                        
                        decimal SVcost = Convert.ToDecimal(_dtPoLines.Rows[RowIndex]["VendorCost"]);
                        poitem.ConvertedCost = Decimal.Round(SVcost,2);
                        dgvPOlines.Rows[RowIndex].Cells["ConvertedCost"].Value = Decimal.Round(SVcost,2);
                        dgvPOlines.Rows[RowIndex].Cells["SavedConvertedCost"].Value = Decimal.Round(SVcost, 2);
                        dgvPOlines.Rows[RowIndex].Cells["ConvertedCost6"].Value = Decimal.Round(SVcost, 6);

                        decimal cost = SVcost * _porder.ExchangeRate;
                        poitem.Cost = Decimal.Round(cost, 2);
                                               

                        dgvPOlines.Rows[RowIndex].Cells["Cost"].Value = cost;

                        decimal landedcost = cost * _porder.Landing;
                        
                        poitem.LandedCost = Decimal.Round(landedcost, 2);
                        dgvPOlines.Rows[RowIndex].Cells["LandedCost"].Value = landedcost;
                        
                        if (poitem.APP1 == "Y")
                        {
                            dgvPOlines.Rows[RowIndex].Cells["ConvertedCost"].ReadOnly = true;
                        }
                        else
                        {
                            dgvPOlines.Rows[RowIndex].Cells["ConvertedCost"].ReadOnly = false;
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
                        poitem.IsValid = false;
                        bItemLookupError = true;
                        
                        MessageBox.Show("Item Lookup failed on : "
                              + "\r\n"
                              + "Item Index: "     + poitem.Sequence.ToString().PadRight(6, padchar)
                              + "\r\n"
                              + "\r\n"
                              + "Class Code: "     + poitem.ClassCode.ToString("0000")
                              + "\r\n"
                              + "\r\n"
                              + "Vendor Code: "    + poitem.Vendorcode.ToString("00000")
                              + "\r\n"
                              + "\r\n"
                              + "Style Code: "     + poitem.Stylecode.ToString("0000")
                              + "\r\n"
                              + "\r\n"
                              + "Color Code: "     + poitem.Colorcode.ToString("000")
                              + "\r\n"
                              + "\r\n"
                              + "Size Code: "      + poitem.Itemsize.ToString("0000")
                              + "", "Spice - PO Modification");
                    }
                }
            }

            CalculatePOSummary();
        }

        private void PopulateComponents(POItemDetails poitemdetails, DataTable dtInitialLoad)
        {
            Validation validationcls = new Validation(_porder.DbParamRef, _porder.UserName, _porder.Penvironment);

            for (int i = 0; i < dtInitialLoad.Rows.Count; i++)
            {
                APPcomponent component = new APPcomponent();

                component.ComponentClass  =   (Int16)dtInitialLoad.Rows[i]["ComponentClass"];
                component.ComponentVendor =   (Int32)dtInitialLoad.Rows[i]["ComponentVendor"];
                component.ComponentStyle  =   (Int16)dtInitialLoad.Rows[i]["ComponentStyle"];
                component.ComponentColour =   (Int16)dtInitialLoad.Rows[i]["ComponentColour"];
                component.ComponentSize   =   (Int16)dtInitialLoad.Rows[i]["ComponentSize"];
                component.RatioQuantity   =   (Int16)dtInitialLoad.Rows[i]["ComponentQuantity"];
                component.Cost            = (Decimal)dtInitialLoad.Rows[i]["ComponentCost"];
                component.Retail          = (Decimal)dtInitialLoad.Rows[i]["Retail"];

                component.ItemDescription = (String)dtInitialLoad.Rows[i]["ComponentLongDesc"];

                poitemdetails.Components.Add(component);
            }
        }

        private short GetNextSequenceNumber()
        {
            int totalrowsinitemslist;
            short sequencenumberonlastitem;
            short nextsequencenumber = 1;

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
                if (poitem.IsDeleted == false)
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
            string sippo = _porder.IPPOnumber;

            History historylookup;

            historylookup = new History(pgmDB, "PO", _spiceponumber, sippo);
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
            string      itemclasslck        = "N";
            string      deptlck             = "Y";
            string      vendorlck           = "N";
            DataTable   dtSelectedItems;
            int         iTotalRowsInGrid;
            int         iStartRow;
            
            itemclass = 0;
            short dept   = Convert.ToInt16(txtDept.Text);
            int   vendor = Convert.ToInt32(txtVendor.Text);

            Disney.Spice.ItemsUI.SelectItem frmSelectItem = new SelectItem(_porder.DbParamRef, _porder.UserName, 
                                                                            _porder.Penvironment, 
                                                                            _mdiparent,  itemclass, itemclasslck, 
                                                                            dept, deptlck, vendor, vendorlck);

            dtSelectedItems = frmSelectItem.GetSelectedItems();

            if (dtSelectedItems != null && dtSelectedItems.Rows.Count > 0)
            {
                iStartRow = iTotalRowsInGrid = dgvPOlines.Rows.Count;
                short linesequence = GetLastPOlineSequence();

                foreach (DataRow dr in dtSelectedItems.Rows)
                {
                    POItemDetails poitem = new POItemDetails();
                    poitem.Sequence = ++linesequence;
                    _porder.lstpoLineItemDetails.Add(poitem);
                    int indexOfNewInstance = _porder.lstpoLineItemDetails.IndexOf(poitem);

                    poitem.ClassCode  = (short)dr["class"];
                    poitem.Vendorcode =   (int)dr["vendor"];
                    poitem.Stylecode  = (short)dr["style"];
                    poitem.Colorcode  = (short)dr["colour"];
                    poitem.Itemsize   = (short)dr["size"];

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

                        dgvPOlines["Sequence", iStartRow].Value = poitem.Sequence;

                        dgvPOlines.Rows[iStartRow].Cells["Description"].Value     = poitem.Itemlongdescription;
                        dgvPOlines.Rows[iStartRow].Cells["Retail"].Value          = poitem.Retailprice.ToString();
                        dgvPOlines.Rows[iStartRow].Cells["Cost"].Value            = poitem.Cost.ToString();
                        dgvPOlines.Rows[iStartRow].Cells["Character"].Value       = poitem.Characterdesc;
                        dgvPOlines.Rows[iStartRow].Cells["Season"].Value          = poitem.SeasonDesc;
                        dgvPOlines.Rows[iStartRow].Cells["CasePackType"].Value    = poitem.Packdescription;
                        dgvPOlines.Rows[iStartRow].Cells["TicketType"].Value      = poitem.Tickettype;
                        dgvPOlines.Rows[iStartRow].Cells["Pack"].Value            = poitem.APP1;
                        dgvPOlines.Rows[iStartRow].Cells["CollectionIndex"].Value = indexOfNewInstance;

                        if (_bDuplicateItem)
                        {
                            // Always in Red
                            SetItemColor(1, iStartRow, System.Drawing.Color.Red);
                            _bDuplicateItem = false;
                        }

                        if (poitem.APP1 == "Y")
                        {
                            _porder.NumofPOPacks ++;
                            dgvPOlines.Rows[iStartRow].Cells["ConvertedCost"].ReadOnly = true;

                            AssortedPrePack AssPrePack = new AssortedPrePack(poitem, _porder);
                            DataTable prePackTbl = AssPrePack.PopulateAPPStructure();

                            TotalComponentCost    = 0;
                            TotalComponentCvtCost = 0;
                            TotalComponentRetail  = 0;

                            for (int i = 0; i < prePackTbl.Rows.Count; i++)
                            {
                                APPcomponent component = new APPcomponent();
                                component.ComponentClass  = (Int16)prePackTbl.Rows[i]["ComponentClass"];
                                component.ComponentVendor = (Int32)prePackTbl.Rows[i]["ComponentVendor"];
                                component.ComponentStyle  = (Int16)prePackTbl.Rows[i]["ComponentStyle"];
                                component.ComponentColour = (Int16)prePackTbl.Rows[i]["ComponentColour"];
                                component.ComponentSize   = (Int16)prePackTbl.Rows[i]["ComponentSize"];
                                component.RatioQuantity   = (Int16)prePackTbl.Rows[i]["ComponentQuantity"];

                                component.Cost = Decimal.Round((Decimal)prePackTbl.Rows[i]["ComponentCost"] / _porder.ExchangeRate, 2);

                                component.Retail = (Decimal)prePackTbl.Rows[i]["Retail"];
                                component.ItemDescription = (String)prePackTbl.Rows[i]["ComponentLongDesc"];
                                
                                poitem.Components.Add(component);

                                //Re-Calculate Cost, Converted Cost and Retail from the Components
                                TotalComponentCost += Decimal.Round(component.Cost * component.RatioQuantity, 2);
                                TotalComponentCvtCost += Decimal.Round((Decimal)prePackTbl.Rows[i]["ComponentCost"] * component.RatioQuantity, 2);
                                TotalComponentRetail += component.Retail * component.RatioQuantity;
                            }

                            //Re -Assign values
                            poitem.Cost      = TotalComponentCvtCost;
                            poitem.LandedCost = TotalComponentCvtCost;
                            dgvPOlines.Rows[iStartRow].Cells["Cost"].Value = poitem.Cost.ToString();

                            poitem.IsNewLine = true;

                            poitem.ConvertedCost = TotalComponentCost;
                            dgvPOlines.Rows[iStartRow].Cells["ConvertedCost"].Value = Decimal.Round(poitem.ConvertedCost, 2).ToString("#.00");
                            dgvPOlines.Rows[iStartRow].Cells["ConvertedCost6"].Value = poitem.ConvertedCost;
                            dgvPOlines.Rows[iStartRow].Cells["SavedConvertedCost"].Value = poitem.ConvertedCost;
                            dgvPOlines.Rows[iStartRow].Cells["LandedCost"].Value = poitem.LandedCost;

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

                        //_porder.lstpoLineItemDetails.Add(poitem);
                    }
                    else
                    {
                        poitem.IsValid = false;

                        MessageBox.Show("Skipping the following item as it does not belong to the current market."
                              + "\r\n"
                              + "\r\n"
                              + " Item Index: " + poitem.Sequence.ToString("000")
                              + " Class Code: " + poitem.ClassCode.ToString("0000")
                              + " Vendor Code: " + poitem.Vendorcode.ToString("00000")
                              + " Style Code: " + poitem.Stylecode.ToString("0000")
                              + " Color Code: " + poitem.Colorcode.ToString("000")
                              + " Size Code: " + poitem.Itemsize.ToString("0000")
                              + "", "Spice - PO Create");
                    }
                }

                btnCreatePO.Enabled = true;
            }
        }
        
        private void btnDeleteLine_Click(object sender, EventArgs e)
        {
            int iRowsDeleted = 0;
            int iRunningCountTotalRows = 0;

            _bUserWantsToDeleteLine = true;
            iRunningCountTotalRows = dgvPOlines.Rows.Count;

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
                btnCreatePO.Enabled = true;
                CalculatePOSummary();
            }
        }

        private void btnCreatePO_Click(object sender, EventArgs e)
        {
            if (CheckGridCount())
            {
                try
                {
                    ValidateChildren();

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

                    
                    if (MessageBox.Show("You are about to modify PO No. " + _spiceponumber
                                    + "\r\n"
                                    + "\r\n"
                                    + "Do you want to continue? ", "SPICE - PO Modification - Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {

                        if (isSpicePOlocked(_porder.SpicePOnumber, _porder.IPPOnumber, _porder.OrderStatus))
                        {
                            MessageBox.Show("This Order is locked against modification",
                                            "Update not possible",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Exclamation);
                            return;
                        }
                        else
                        {
                            pwindow = new ProgressWindow(1);
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
            else
            {
                MessageBox.Show("This PO cannot be created as there are no Po Lines.", "SPICE PO MANAGEMENT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        #endregion

        #region Background worker
        private void bkgrndWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            string sStore = String.Empty;
            if (!ModifyPurchaseOrder(_spiceponumber))
            {
                e.Cancel = true;
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

        private void btnDisplayEDIdates_Click(object sender, EventArgs e)
        {
            LookUpEDIdates dates = new LookUpEDIdates(pgmDB);
            EdiDates edidates = dates.GetEdiDates(_porder.IPPOnumber);

            String DateFmt = (environment.DateFormat == "MDY") ? "MMM - dd - yyyy" : "dd - MMM - yyyy";

            EDIdates edidatesfrm = new EDIdates(edidates.SCBsendDate.ToString(DateFmt), edidates.OOCLsendDate.ToString(DateFmt), edidates.AverySendDate.ToString(DateFmt));
            edidatesfrm.ShowDialog();
            edidatesfrm = null;
        }

        private void grpBoxDates_Enter(object sender, EventArgs e)
        {

        }

        private void POmodification_Load(object sender, EventArgs e)
        {

        }
    }
}
