 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
        public struct MarketData
        {
            public string dIPCode;
            public string dIPMktDesc;
            public string dIPCurrency;
            public string dISOCountryCode;
            public string dISOCurrencyCode;
        }

        private Validation validationcls;
        private LookupBO lookupbo;

        public MarketSelectionForm(ASNA.VisualRPG.Runtime.Database dbparamref, Disney.Menu.Users username, Disney.Menu.Environments paramenv, Form Form1)
        {
            InitializeComponent();
            
            try
            {
                _purchaseorder = new PurchaseOrder(dbparamref, username,paramenv);

                validationcls = new Validation(dbparamref, username,paramenv);      

                //lookupbo = new LookupBO(dbparamref,username);

                List<string> retValues = new List<string>();
                retValues = LoadDefaultMarket(paramenv.Domain,username);
                
                //Display fields
                txtMarketSelection.Text = retValues[0];
                lblMarketSelectionDesc.Text = retValues[1];
                                                            

                //Stuff values to the PO Object

                _purchaseorder.DefaultMarket = retValues[0];
                _purchaseorder.MarketDescription = retValues[1];

                // /////////////////////////////////////////////////////////////////
                // HK : Clayton : BM : Do not send the base currency.
                // 
                // 
                //This is the base currency now.
                //_purchaseorder.BaseCurrency = retValues[2];
                
                mdiparent = Form1;                            
                             
            }
            catch (Exception ex)
            {

                MessageBox.Show("An exception has occured " + ex.Message + "in " + ex.Source);
            
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {


            _purchaseorder.DefaultMarket = txtMarketSelection.Text;
            _purchaseorder.MarketDescription = lblMarketSelectionDesc.Text;

            POEntryForm1 poentryform = new POEntryForm1(_purchaseorder, mdiparent);

            this.Close();
            poentryform.Show();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();

        }

        private void pctBxMarketLookup_Click(object sender, EventArgs e)
        {
            //Call generic market lookup functionality

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

        private void txtMarketSelection_TextChanged(object sender, EventArgs e)
        {
           
        }

        private List<string> LoadDefaultMarket(string domain, Disney.Menu.Users username)
        {

            List<string> lstRetValues = new List<string>();

            try
            {
                //Load XML using "Domain" to find default market
                                
                _dsMarkets = new DataSet();

                string spath = Application.ExecutablePath;

                /// HK : 05-01-2010 : Fix Bug ?? Check for the .XML in iDash and not 
                /// imn iDash\XML
                //_dsMarkets.ReadXml(spath.Replace("DisneyMenu.exe","XML\\Markets.xml"), XmlReadMode.InferSchema);
                _dsMarkets.ReadXml(spath.Replace("DisneyMenu.exe","\\Markets.xml"), XmlReadMode.InferSchema);
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
            catch (Exception ex)
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

                if (datarow["Domain"].ToString() == domainname)
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

                // /////////////////////////////////////////////////////////////////////
                // HK : 17-09-2009 : Capture Market Currency
                _purchaseorder.MarketCurrency = retValues[2];

                // /////////////////////////////////////////////////////////////////////

            }
            else 
            {
                errMarketSelection.SetError(txtMarketSelection,"Please enter a valid market code");
                lblMarketSelectionDesc.Text = "";
                e.Cancel = true;
                validationcls.HighlightErrControls(lblMarket, txtMarketSelection, false);

             }

        }

    }
}