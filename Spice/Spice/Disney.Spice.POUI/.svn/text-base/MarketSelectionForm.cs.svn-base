 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Disney.Spice.POBO;
using Disney.Spice.ItemsBO;
using Disney.Spice.LookUp;
using System.Xml;
using System.Diagnostics;
using Disney.Menu;

namespace Disney.Spice.POUI
{
    public partial class MarketSelectionForm : Form
    {
        private PurchaseOrder _purchaseorder;
        private DataSet _dsMarkets;
        private DataTable dtAuthorisedMarkets;
        private Form mdiparent;
        private Disney.Menu.Environments environment;

        private Validation validationcls;

        public MarketSelectionForm(ASNA.VisualRPG.Runtime.Database dbparamref, Disney.Menu.Users username, Disney.Menu.Environments environment, Form MDIparent)
        {
            InitializeComponent();

            this.AutoValidate = AutoValidate.Disable;

            mdiparent = MDIparent;
            this.MdiParent = MDIparent;
            this.environment = environment;

            _purchaseorder = new PurchaseOrder(dbparamref, username, environment);
            validationcls = new Validation(dbparamref, username, environment);      

            List<string> retValues = new List<string>();
            retValues = LoadDefaultMarket(environment.Domain, username);

            _purchaseorder.DomainMarket = retValues[0];
            txtMarketSelection.Text = retValues[0];
            lblMarketSelectionDesc.Text = retValues[1];
                                                            
            _purchaseorder.DefaultMarket     = retValues[0];
            _purchaseorder.MarketDescription = retValues[1];
            _purchaseorder.BaseCurrency      = retValues[2];
                       
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.ValidateChildren();

            if (errMarketSelection.GetError(txtMarketSelection) == "")
            {
                _purchaseorder.DefaultMarket = txtMarketSelection.Text;
                _purchaseorder.MarketDescription = lblMarketSelectionDesc.Text;

                POEntryForm1 poentryform = new POEntryForm1(_purchaseorder, mdiparent);

                this.Close();
                poentryform.Show();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.errMarketSelection.Clear();
            this.Close();
        }
        
        private void pctBxMarketLookup_Click(object sender, EventArgs e)
        {
            Enquiry mktlookup = new Enquiry(dtAuthorisedMarkets, "MarketLookup");

            mktlookup.ShowGrid();

            if (mktlookup.DialogResult == DialogResult.OK)
            {
                txtMarketSelection.Text = mktlookup.SelectedValue[0];
                lblMarketSelectionDesc.Text = mktlookup.SelectedValue[1];
                _purchaseorder.DefaultMarket = txtMarketSelection.Text;
                _purchaseorder.MarketDescription = lblMarketSelectionDesc.Text;
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("IExplore.exe", "http://teamshareemea.emea.wdpr.disney.com/dcpsites/spice/SpiceUserManual/Pages/default.aspx"); 
        }

        private List<string> LoadDefaultMarket(string domain, Disney.Menu.Users username)
        {
            List<string> lstRetValues = new List<string>();

            try
            {           
                _dsMarkets = new DataSet();
                
                string spath = Path.Combine(environment.PathToEnvironmentXML,"Markets.xml");
                _dsMarkets.ReadXml(spath, XmlReadMode.InferSchema);
                DataTable dtmarketsTable = _dsMarkets.Tables[0];
                
                dtAuthorisedMarkets = GetDomainMarkets(domain,dtmarketsTable,username);
                
                foreach (DataRow var in dtmarketsTable.Select())
                {
                    if ((var["Domain"].ToString() == domain) && (var["IPMasterMarket"].ToString() == "Y"))
                    {
                        lstRetValues.Add(var["IPMarket"].ToString());
                        lstRetValues.Add(var["IPMarketDesc"].ToString());
                        lstRetValues.Add(var["IPCurrencyCode"].ToString());
                    }
                }

                return lstRetValues;
            }
            catch (Exception)
            {
                return lstRetValues;
            }
        }

        private DataTable GetDomainMarkets(string domainname,DataTable dtAllMarkets,Disney.Menu.Users username)
        {
            DataTable dtDomainMarkets = new DataTable();
            
            dtDomainMarkets.Columns.Add(new DataColumn("MarketCode",typeof(string)));
            dtDomainMarkets.Columns.Add(new DataColumn("MarketDescription",typeof(string)));

            dtDomainMarkets.Columns.Add(new DataColumn("MarketCurrencyCode", typeof(string)));

            foreach (DataRow datarow in dtAllMarkets.Select())
            {

                if (datarow["Domain"].ToString() == domainname && datarow["Status"].ToString() == "1")
                {  //Only select the right records now lets check if the IPMarket matches
                                                        
                    for(int i =0 ; i <username.AuthorisedMarkets.Length; i++)
                    {
                        
                      if(datarow["IPMarket"].ToString()==username.AuthorisedMarkets[i])
                      {
                          dtDomainMarkets.Rows.Add(datarow["IPMarket"].ToString(), 
                              datarow["IPMarketDesc"].ToString(),
                              datarow["IPCurrencyCode"].ToString());
                      }                  
                    }
                                 
                }
                                
            }
            return dtDomainMarkets;
        }

        private void txtMarketSelection_Validating(object sender, CancelEventArgs e)
        {
            
            List<string> retValues = new List<string>();

            retValues = validationcls.ValidateMarkets(dtAuthorisedMarkets, txtMarketSelection.Text);

            if (retValues[0] == "True")
            {
                lblMarketSelectionDesc.Text = retValues[1];
                errMarketSelection.SetError(txtMarketSelection, "");
                e.Cancel =false;
                validationcls.HighlightErrControls(lblMarket, txtMarketSelection, true);

                // Capture Market Currency
                _purchaseorder.MarketCurrency = retValues[2];
            }
            else 
            {
                errMarketSelection.SetError(txtMarketSelection,"Please enter a valid market code");
                lblMarketSelectionDesc.Text = "";
                e.Cancel = true;
                validationcls.HighlightErrControls(lblMarket, txtMarketSelection, false);
             }
        }

        private void MarketSelectionForm_Validating(object sender, CancelEventArgs e)
        {

        }
    }
}