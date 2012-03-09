using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using Disney.DA.IP400;
using System.Diagnostics;
using Disney.Spice.ItemsBO;
using Disney.Spice.POBO;

namespace Disney.Spice.EASUI
{
    public partial class frmApproveRejectItemSizeChangeCaseDetails : Form
    {
        private ASNA.VisualRPG.Runtime.Database     _dbparamref;
        private Disney.Menu.Users                   _username;
        private Disney.Menu.Environments            _paramenv;
        private POItemDetails                       _polinedetails;
        private ItemsBO.Items                       _itemsbo;

        short   _classcode;
        int     _vendorcode;
        short   _stylecode;
        short   _colourcode;
        short   _itemsize;

        private Boolean _bFormCancelClicked;

        public frmApproveRejectItemSizeChangeCaseDetails()
        {
            InitializeComponent();
        }


        public frmApproveRejectItemSizeChangeCaseDetails(ASNA.VisualRPG.Runtime.Database dbparamref, Disney.Menu.Users username, Disney.Menu.Environments paramenv, short classcode, int vendorcode, short stylecode, short colourcode, short itemsize)
        {
            InitializeComponent();

            _dbparamref = dbparamref;
            _username = username;
            _paramenv = paramenv;

            _classcode = classcode;
            _vendorcode = vendorcode;
            _stylecode = stylecode;
            _colourcode = colourcode;
            _itemsize = itemsize;

            _polinedetails = new POItemDetails(1);

            _itemsbo = new Disney.Spice.ItemsBO.Items(_dbparamref, _username, _paramenv);

            SetupClassObjects();
            
            SetupSimpleDataBinding();

        }

        private Boolean SetupClassObjects()
        {
            Boolean bSuccess;

            // Get the Domain Markets table to get the Base Currency
            _itemsbo.GetDomainMarketsTbl();

            bSuccess = _itemsbo.GetItem(_classcode, _vendorcode, _stylecode, _colourcode, _itemsize, _itemsbo.BaseMarket);

            return bSuccess;
        }

        private void SetupSimpleDataBinding ()
        {
            _itemsbo.GetClass(_classcode);
            lblClass.Text = _itemsbo.ClassName;

            _itemsbo.GetVendor(_vendorcode);
            lblVendor.Text = _itemsbo.VendorName;

            //_itemsbo.GetItemStyle(_classcode, _vendorcode, _stylecode, _colourcode);
            lblStyle.Text = _itemsbo.SizeName;

            _itemsbo.GetColour(_colourcode);
            lblColour.Text = _itemsbo.ColourName;
            
            _itemsbo.GetSize (_itemsize);
            lblSize.Text = _itemsbo.SizeName;

            _itemsbo.GetSubClass (_itemsbo.SubClass, _classcode);
            lblSubClass.Text = _itemsbo.SubClassName;

            lblLongDesc.Text = _itemsbo.ItemLongDescription;

            lblShortDesc.Text = _itemsbo.ItemShortDescription;

            lblVendorStyle.Text = _itemsbo.VendorStyle;

            //??
            lblDevBy.Text = "";
            lblCOD.Text = "";

            //??
            //_itemsbo.GetCompositionCode(_itemsbo.CompositionCode);
            lblComposition.Text = _itemsbo.CompositionDescription;

            _itemsbo.GetChar(_itemsbo.CharacterCode);
            lblCharacter.Text = _itemsbo.CharacterName;

            _itemsbo.GetSeason(_itemsbo.SeasonCode);
            lblSeason.Text = _itemsbo.SeasonDesc;

            lblCasePackType.Text = "";
            lblCasepackQty.Text = _itemsbo.CaseQuantity.ToString();

            lblDistro.Text = _itemsbo.DistroQuantity.ToString ();

            lblWeight.Text = _itemsbo.PackWeight.ToString();
            lblHeight.Text = _itemsbo.PackHeight.ToString();
            lblLength.Text = _itemsbo.PackLength.ToString();
            lblWidth.Text = _itemsbo.PackWidth.ToString();

            //
            lblRetail.Text = _itemsbo.ItemRetail.ToString();
            lblCost.Text = _itemsbo.ItemCost.ToString();
            lblVatCode.Text = _itemsbo.VatCode;

            //
            _itemsbo.GetAgeGroup(_itemsbo.AgeGroupID);
            lblSex.Text = _itemsbo.AgeGroupDescription;
            lblExclusive.Text = _itemsbo.OnlineInd;

            lblLocation.Text = "";
            lblAge.Text = "";
            lblTicketType.Text = "";

            // Audit
            lblCrtDate.Text = "";
            lblCrtTime.Text = "";
            lblCrtUser.Text = "";

            lblChgDate.Text = "";
            lblChgTime.Text = "";
            lblChgUser.Text = "";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // HK : 16-11-2009
            // Prevent control validation when user clicks the Cancel button
            _bFormCancelClicked = true;

            DialogResult dlgResult = MessageBox.Show("Are you sure you want to Cancel this Approve / Reject Item Size", "SPIC Approve/Reject PO Creation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (dlgResult == DialogResult.Yes)
            {
                this.Close();
            }
            else
            {
                _bFormCancelClicked = false;
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx");                     
        }

        //private void btnApprove_Click(object sender, EventArgs e)
        //{
        //}
        //private void btnViewHistory_Click(object sender, EventArgs e)
        //{
        //}
        //private void btnViewPos_Click(object sender, EventArgs e)
        //{
        //}

        private void btnEAS_Click(object sender, EventArgs e)
        {
            // HK : 19-12-2009 : Hardcode Request Number
            //Int64 requestid = 1;

            // hourglass cursor
            Cursor.Current = Cursors.WaitCursor;

            //frmEASRequest frmeasrequest = new frmEASRequest(_dbparamref, _username, _paramenv, _mdiparent, _requestid);

            //frmeasrequest.ShowDialog();

            //frmeasrequest = null;
        }
    }
}