using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using Disney.DA.IP400;
using ASNA.VisualRPG.Runtime;
using Disney.Spice.ItemsBO;
using System.Drawing;
using System.Windows.Forms;

namespace Disney.Spice.POBO
{
    public class Validation
    {
        private IPCURCY ipcurrencycls;
        private DSSPAPH ChkAppointments;
        private ASNA.VisualRPG.Runtime.Database _PgmDB;
        private Disney.Menu.Users _username;
        private Disney.Menu.Environments _paramenv;

        public Validation(ASNA.VisualRPG.Runtime.Database pgmdb, Disney.Menu.Users username, Disney.Menu.Environments paramenv)
        {
            _PgmDB    = pgmdb;
            _username = username;
            _paramenv = paramenv;
        }

        public List<string> ValidateClass(String ClassCode, String Dept)
        {
            Int16 classcode;
            Int16 dept;

            ItemsBO.Items itembo = new Disney.Spice.ItemsBO.Items(_PgmDB, _username, _paramenv);
            List<string> listRetValues = new List<string>();

            if (!Int16.TryParse(Dept, out dept))
            {
                listRetValues.Add("False");
                listRetValues.Add("Department is an invalid number");
            }

            if (!Int16.TryParse(ClassCode, out classcode))
            {
                if (listRetValues.Count == 0)
                {
                    listRetValues.Add("False");
                }

                listRetValues.Add("ClassCode is an invalid number)");
            }

            if (listRetValues.Count > 0) return listRetValues;

            if (!itembo.GetClass(classcode, dept))
            {
                listRetValues.Add("False");
                listRetValues.Add("You are not authorised to the class");
                return listRetValues;
            }

            listRetValues.Add(itembo.Class.ToString());
            listRetValues.Add(itembo.ClassName);
            return listRetValues;
        }

        public List<string> ValidateClass(string ClassCode)
        {
            Int16 classcode;

            ItemsBO.Items itembo = new Disney.Spice.ItemsBO.Items(_PgmDB, _username, _paramenv);
            List<string> listRetValues = new List<string>();

            if (!Int16.TryParse(ClassCode, out classcode))
            {
                listRetValues.Add("False");
                listRetValues.Add("ClassCode is an invalid number)");
                return listRetValues;
            }

            if (!itembo.GetClass(classcode))
            {
                listRetValues.Add("False");
                listRetValues.Add("You are not authorised to the class");
                return listRetValues;
            }

            listRetValues.Add(itembo.Class.ToString());
            listRetValues.Add(itembo.ClassName);
            return listRetValues;
        }

        public List<string> ValidateVendor(string svalue, bool itemlevel)
        {
            //Checks for nulls/irregular chars and validates against the database
            List<string> lstretVendor = new List<string>();
            Int32 vendorcode;
           
            if (String.IsNullOrEmpty(svalue) || !Int32.TryParse(svalue, out vendorcode))
            {
                lstretVendor.Clear();
                lstretVendor.Add("False");
            }

            else
            {
                Items ibovendor = new Items(_PgmDB, _username, _paramenv);
                if (ibovendor.GetVendor(vendorcode))
                {
                    lstretVendor.Add(ibovendor.GetVendor(Int32.Parse(svalue)).ToString());
                    lstretVendor.Add(ibovendor.VendorName);

                    //Ship Via Terms and Currency
                    lstretVendor.Add(String.IsNullOrEmpty(ibovendor.VendorShipVia) ? String.Empty : ibovendor.VendorShipVia);
                    lstretVendor.Add(String.IsNullOrEmpty(ibovendor.VendorTerms) ? String.Empty : ibovendor.VendorTerms);
                    lstretVendor.Add(String.IsNullOrEmpty(ibovendor.VendorCurrency) ? String.Empty : ibovendor.VendorCurrency);
                    lstretVendor.Add(String.IsNullOrEmpty(ibovendor.VendorTermsDescription) ? String.Empty : ibovendor.VendorTermsDescription);

                    if ((String.IsNullOrEmpty(ibovendor.VendorTerms)) && (itemlevel == false))
                    {
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
                Items ipsize = new Items(_PgmDB, _username, _paramenv);

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
                ItemsBO.Items item = new Items(_PgmDB, _username, _paramenv);
                lstretColour.Add(item.GetColour(Int16.Parse(svalue)).ToString());
                lstretColour.Add(item.ColourName);
            }
            
            return lstretColour;
        }

        public decimal GetCurrency(string scurrency)
        {
            if (ipcurrencycls == null) ipcurrencycls = new IPCURCY(_PgmDB, _paramenv);

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
            if (ipcurrencycls == null) ipcurrencycls = new IPCURCY(_PgmDB, _paramenv);

            if (ipcurrencycls.GetCurrency(svalue)) //We have got a valid currency
            {
                lstCurrency.Add(ipcurrencycls.CurrencyCode);
                lstCurrency.Add(ipcurrencycls.CurrencyName);

                // Add property CurrencyRate to list
                lstCurrency.Add(ipcurrencycls.CurrencyRate.ToString());
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

                    // HK : 17-11-2009 : Capture the Market currency code and send 
                    // it back to caller
                    lstretValues.Add(dtrow["MarketCurrencyCode"].ToString());
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
                Items itemboport = new Items(_PgmDB, _username, _paramenv);

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
                Items itemshipvia = new Items(_PgmDB, _username, _paramenv);

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
                Items itemdelterms = new Items(_PgmDB, _username, _paramenv);

                if (itemdelterms.GetDelTerms(svalue))
                {
                    retDelTermsValues.Clear();
                    retDelTermsValues.Add("True");
                    retDelTermsValues.Add(itemdelterms.DelTermsDescription);
                }
            }
            return retDelTermsValues;
        }

        public List<string> ValidateDeptCode(string svalue)
        {
            List<string> lstDept = new List<string>();

            LookupBO lookupbo = new LookupBO(_PgmDB, _username, _paramenv);

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

        public bool CheckIPPOstatus(string IPPOnumber)
        {
            if (ChkAppointments == null) ChkAppointments = new DSSPAPH(_PgmDB);
            return ChkAppointments.ChkPOappointment(IPPOnumber);
        }

        public Boolean ValidateItemNumber(Int16 Class,Int32 Vendor,Int16 Style,Int16 Colour,Int16 Size, String Market)
        {
            ItemsBO.Items itembo = new Disney.Spice.ItemsBO.Items(_PgmDB, _username, _paramenv);
            return itembo.ChkItemExists(Class, Vendor, Style, Colour, Size, Market);
        }

    }
}