using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using Disney.DA.IP400;
using ASNA.VisualRPG.Runtime;
using Disney.Spice.ItemsBO;
using System.Drawing;
using System.Windows.Forms; //Highlight Controls

namespace Disney.Spice.POBO
{
    public class Validation
    {

        private IPCURCY ipcurrencycls;
        private IPCLASScls ipcls;

        private ASNA.VisualRPG.Runtime.Database _dbinternalparam;
        private Disney.Menu.Users _username;
        private Disney.Menu.Environments _paramenv;

        public Validation(ASNA.VisualRPG.Runtime.Database dbparamref, Disney.Menu.Users username, Disney.Menu.Environments paramenv)
        {

            _dbinternalparam = dbparamref;
            _username = username;
            _paramenv = paramenv;

        }

        public List<string> ValidateClass(string svalue, string dept)
        {

            // HK : 27-01-2010 : However this does not work. So Bug 356 was raised.
            // Clayton Jones to provide new Da with overloaded method.
            // I have created a new overload for the ValidateClass method so 
            // that other calls may be unaffected.

            // /////////////////////////////////////////////////////////////
            // HK : CJ : Fix Bug 33 : 
            // Before checking that a Class is a valid Class we need to determine
            // that the the Class is valid for the Department

            ItemsBO.Items itembo = new Disney.Spice.ItemsBO.Items(_dbinternalparam, _username, _paramenv);

            // /////////////////////////////////////////////////////////////

            List<string> lstRetValues = new List<string>();
            Int16 classcode;
            Int16 deptcode;

            if (Int16.TryParse(svalue, out classcode) && (Int16.TryParse(dept, out deptcode)))
            {
                if (itembo.GetClass (classcode, deptcode) == false)
                {
                    lstRetValues.Add("False");
                    return lstRetValues; ;
                }
            }

            // /////////////////////////////////////////////
            // 04-12-2009 : Rest of the original validation
            // /////////////////////////////////////////////
            if (String.IsNullOrEmpty(svalue) || !Int16.TryParse(svalue, out classcode))
            {

                lstRetValues.Add("False");

            }
            else
            {

                //Validate that the class is valid via  lookup.
                if (ipcls == null) { ipcls = new IPCLASScls(_dbinternalparam); }
                lstRetValues.Add(ipcls.GetClass(Int16.Parse(svalue)).ToString());
                lstRetValues.Add(ipcls.ClassName);

            }

            return lstRetValues;

        }

        public List<string> ValidateClass(string svalue)
        {

            // /////////////////////////////////////////////////////////////
            // HK : CJ : Fix Bug 33 : 
            // Before checking that a Class is a valid Class we need to determine
            // that the the Class is valid for the Department

            // HK : 27-01-2010 : However this does not work. So Bug 356 was raised.
            // Clayton Jones to provide new Da with overloaded method.
            ItemsBO.Items itembo = new Disney.Spice.ItemsBO.Items(_dbinternalparam, _username, _paramenv);

            // /////////////////////////////////////////////////////////////

            List<string> lstRetValues = new List<string>();
            Int16 classcode;

            if (Int16.TryParse(svalue, out classcode))
            {
                if (itembo.GetClass (classcode) == false)
                {
                    lstRetValues.Add("False");
                    return lstRetValues; ;
                }
            }

            // /////////////////////////////////////////////
            // 04-12-2009 : Rest of the original validation
            // /////////////////////////////////////////////
            if (String.IsNullOrEmpty(svalue) || !Int16.TryParse(svalue, out classcode))
            {

                lstRetValues.Add("False");

            }
            else
            {

                //Validate that the class is valid via  lookup.
                if (ipcls == null) { ipcls = new IPCLASScls(_dbinternalparam); }
                lstRetValues.Add(ipcls.GetClass(Int16.Parse(svalue)).ToString());
                lstRetValues.Add(ipcls.ClassName);

            }

            return lstRetValues;

        }

        public List<string> ValidateVendor(string svalue, bool itemlevel)
        {

            // To : HK : 04-12-2009 : Apply similar validation as applied 
            // for "Class" validation

            //Checks for nulls/irregular chars and validates against the database
            List<string> lstretVendor = new List<string>();
            Int32 vendorcode;
            bool isvalid;

            if (String.IsNullOrEmpty(svalue) || !Int32.TryParse(svalue, out vendorcode))
            {
                lstretVendor.Clear();
                lstretVendor.Add("False");
            }

            else
            {
                //Dont reinvent the wheel
                Items ibovendor = new Items(_dbinternalparam, _username, _paramenv);

                if (ibovendor.GetVendor(Int32.Parse(svalue)))
                {
                    isvalid = true;
                    lstretVendor.Add(ibovendor.GetVendor(Int32.Parse(svalue)).ToString());
                    lstretVendor.Add(ibovendor.VendorName);

                    //Ship Via Terms and Currency
                    lstretVendor.Add(String.IsNullOrEmpty(ibovendor.VendorShipVia) ? String.Empty : ibovendor.VendorShipVia);
                    lstretVendor.Add(String.IsNullOrEmpty(ibovendor.VendorTerms) ? String.Empty : ibovendor.VendorTerms);
                    lstretVendor.Add(String.IsNullOrEmpty(ibovendor.VendorCurrency) ? String.Empty : ibovendor.VendorCurrency);
                    lstretVendor.Add(String.IsNullOrEmpty(ibovendor.VendorTermsDescription) ? String.Empty : ibovendor.VendorTermsDescription);

                    if ((String.IsNullOrEmpty(ibovendor.VendorTerms)) && (itemlevel == false))
                    { //Uh oh we cant use this vendor then

                        lstretVendor.Clear();
                        lstretVendor.Add("False");

                    }
                }
                else
                {
                    lstretVendor.Clear();
                    lstretVendor.Add("False");

                }


            }

            return lstretVendor;


        }

        public List<string> ValidateSize(string svalue)
        {
            List<string> lstretSize = new List<string>();
            Int16 sizecode;

            if (String.IsNullOrEmpty(svalue) || !Int16.TryParse(svalue, out sizecode))
            {
                lstretSize.Add("False");

            }
            else
            {

                Items ipsize = new Items(_dbinternalparam, _username, _paramenv);
                //IPSIZEScls ipsizecls = new IPSIZEScls(dbparamref);

                lstretSize.Add(ipsize.GetSize(Int16.Parse(svalue)).ToString());
                lstretSize.Add(ipsize.SizeName);
            }
            return lstretSize;

        }

        public List<string> ValidateColour(string svalue)
        {

            List<string> lstretColour = new List<string>();
            Int16 colourcode;

            if (String.IsNullOrEmpty(svalue) || !Int16.TryParse(svalue, out colourcode))
            {
                lstretColour.Add("False");

            }
            else
            {

                ItemsBO.Items item = new Items(_dbinternalparam, _username, _paramenv);
                lstretColour.Add(item.GetColour(Int16.Parse(svalue)).ToString());
                lstretColour.Add(item.ColourName);
            }
            return lstretColour;

        }

        public decimal GetCurrency(string scurrency)
        {
            if (ipcurrencycls == null) ipcurrencycls = new IPCURCY(_dbinternalparam);

            if (ipcurrencycls.GetCurrency(scurrency)) //We have got a valid currency
            {

                if (ipcurrencycls.CurrencyRate == 0)
                {
                    throw new Exception("Currency Rate cannot be zero (0)!");
                }

                return ipcurrencycls.CurrencyRate;
            }

            throw new Exception("Currency Rate cannot be zero (0)!");
            
        }


        public List<string> CurrencyValidate(string svalue, string sBaseCurrency)
        {

            List<string> lstCurrency = new List<string>();
            decimal dExchangeRate;
            if (ipcurrencycls == null) ipcurrencycls = new IPCURCY(_dbinternalparam);

            if (ipcurrencycls.GetCurrency(svalue)) //We have got a valid currency
            {
                // //////////////////////////////////////////////////////////////
                // HHK : 16-11-2009
                // Method GetXchangeRate is now obsolete. If no occurances found in 
                // solution remove call the GetXchangeRate
                //dExchangeRate = ipcurrencycls.GetXchangeRate(sBaseCurrency, svalue);
                // //////////////////////////////////////////////////////////////

                lstCurrency.Add(ipcurrencycls.CurrencyCode);
                lstCurrency.Add(ipcurrencycls.CurrencyName);

                // //////////////////////////////////////////////////////////////
                // HHK : 16-11-2009
                // Add property CurrencyRate to list
                //lstCurrency.Add(dExchangeRate.ToString());
                lstCurrency.Add(ipcurrencycls.CurrencyRate.ToString());

                // //////////////////////////////////////////////////////////////
            }

            return lstCurrency;

        }

        public List<string> ValidateStyle(string svalue)
        {

            List<string> lstretStyle = new List<string>();
            Int16 stylecode;

            if (String.IsNullOrEmpty(svalue) || !Int16.TryParse(svalue, out stylecode))
            {
                lstretStyle.Add("False");

            }
            else
            {
                if (stylecode >= 1) { lstretStyle.Add("True"); }
                else { lstretStyle.Add("False"); }
            }
            return lstretStyle;


        }


        public List<string> ValidateMarkets(DataTable dtMarkets, string svalue)
        {
            List<string> lstretValues = new List<string>();

            lstretValues.Add("False");

            foreach (DataRow dtrow in dtMarkets.Rows)
            {

                if (dtrow["MarketCode"].ToString() == svalue)
                {
                    lstretValues.Clear();
                    lstretValues.Add("True");
                    lstretValues.Add(dtrow["MarketDescription"].ToString());

                    // ///////////////////////////////////////////////////////////
                    // HK : 17-11-2009 : Capture the Market currency code and send 
                    // it back to caller
                    lstretValues.Add(dtrow["MarketCurrencyCode"].ToString());

                    // ///////////////////////////////////////////////////////////
                }

            }

            return lstretValues;
        }

        public List<string> ValidatePort(string svalue)
        {

            List<string> lstretPort = new List<string>();
            Int32 portcode;

            if (String.IsNullOrEmpty(svalue) || !Int32.TryParse(svalue, out portcode))
            {
               lstretPort.Add("False");

            }
            else
            {
                Items itemboport = new Items(_dbinternalparam, _username, _paramenv);

                if (itemboport.GetPort(Int32.Parse(svalue)))
                {
                    lstretPort.Clear();
                    lstretPort.Add("True");
                    lstretPort.Add(itemboport.PortDescription);

                }
                else
                {

                    lstretPort.Add("False");

                }
            }
            return lstretPort;
        }

        public List<string> ValidateShipVia(string svalue)
        {
            List<string> lstretShipVia = new List<string>();

            lstretShipVia.Add("False");
            if (String.IsNullOrEmpty(svalue))
            {
                lstretShipVia.Add("False");

            }
            else
            {
                Items itemshipvia = new Items(_dbinternalparam, _username, _paramenv);

                if (itemshipvia.GetShipVia(svalue))
                {
                    lstretShipVia.Clear();
                    lstretShipVia.Add("True");
                    lstretShipVia.Add(itemshipvia.ShipViaDescription);
                }
            }
            return lstretShipVia;



        }

        public List<string> ValidateDeliveryTerms(string svalue)
        {

            List<string> retDelTermsValues = new List<string>();

            retDelTermsValues.Add("False");
            if (String.IsNullOrEmpty(svalue))
            {
                retDelTermsValues.Add("False");


            }
            else
            {
                Items itemdelterms = new Items(_dbinternalparam, _username, _paramenv);



                if (itemdelterms.GetDelTerms(svalue))
                {
                    retDelTermsValues.Clear();
                    retDelTermsValues.Add("True");
                    retDelTermsValues.Add(itemdelterms.DelTermsDescription);
                }
            }
            return retDelTermsValues;




        }

        /*public DataTable DepartmentLookup(ASNA.VisualRPG.Runtime.Database dbparamref, Disney.Menu.Users username)
        {

            IPDEPTS ipdeptscls = new IPDEPTS(dbparamref);
            DataTable dtdepttable = ipdeptscls.GetDepartmentTbl();
            DataTable dtAuthoriseddepts = username.AuthorisedDepartments;

            //Now all we need is the relevant dept description added to the dataset

            if (dtAuthoriseddepts.Columns.Count == 1)
            {
                dtAuthoriseddepts.Columns.Add(new DataColumn("Description", typeof(string)));
            }
            foreach (DataRow row in dtAuthoriseddepts.Rows)
            {
                if (ipdeptscls.GetDepartment(Int16.Parse(row["Department"].ToString())))
                {
                    //Retrive the dept description
                    row["Description"] = ipdeptscls.DepartmentName;

                }
            }
            return dtAuthoriseddepts;
        }
        */

        public List<string> ValidateDeptCode(string svalue)
        {


            List<string> lstDept = new List<string>();

            //Aargh... 
            LookupBO lookupbo = new LookupBO(_dbinternalparam, _username, _paramenv);


            DataTable dtAuthoriseddepts = lookupbo.DepartmentLookup();

            foreach (DataRow row in dtAuthoriseddepts.Rows)
            {
                if ((row["Department"].ToString() == svalue))
                {

                    lstDept.Add(row["Department"].ToString());
                    lstDept.Add(row["Description"].ToString());


                }
            }

            return lstDept;

        }

        public void HighlightErrControls(Control labelcontrol, Control valuecontrol, bool bReset)
        {


            if (!bReset)
            {
                labelcontrol.ForeColor = System.Drawing.Color.Tomato;
                labelcontrol.Font = new Font(labelcontrol.Font, FontStyle.Bold);
                valuecontrol.Font = new Font(valuecontrol.Font, FontStyle.Bold);

            }
            else
            {

                labelcontrol.ForeColor = System.Drawing.Color.Black;
                labelcontrol.Font = new Font(labelcontrol.Font, FontStyle.Regular);
                valuecontrol.Font = new Font(valuecontrol.Font, FontStyle.Regular);

            }

        }


    }

}