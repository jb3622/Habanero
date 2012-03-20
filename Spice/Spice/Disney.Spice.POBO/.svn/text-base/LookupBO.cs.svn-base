using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Disney.DA.IP400;
using ASNA.VisualRPG.Runtime;
using Disney.Spice.ItemsBO;

namespace Disney.Spice.POBO
{
    public class LookupBO
    {
        private ASNA.VisualRPG.Runtime.Database pgmDB;
        private Disney.Menu.Users _username;
        private Disney.Menu.Environments environment;
        private SpiceAPPItem _spiceAPPItem;

        public LookupBO(ASNA.VisualRPG.Runtime.Database pgmDB, Disney.Menu.Users username, Disney.Menu.Environments environment)
        {
            this.pgmDB = pgmDB;
            _username = username;
            this.environment = environment;
        }

        public DataTable ShipviaLookup()
        {
            Items itembo = new Items(pgmDB, _username, environment);
            DataSet dsTerms = itembo.GetShipViaTbl();
            return dsTerms.Tables["ShipVia"];
        }

        public Boolean ItemIsBundle(short itemClass,int vendor,short style,short colour,short size)
        {
            Boolean itemIsBundle = false;  

            _spiceAPPItem = new SpiceAPPItem(this.pgmDB, this._username);

            try
            {
                _spiceAPPItem.GetAPPMasterRead(itemClass, vendor, style, colour, size);
                if(_spiceAPPItem.APPType == "B")
                {
                    itemIsBundle = true;
                }

            }
            catch (Exception ex)
            {
            }

            return itemIsBundle;
        }

        public DataTable DeliveryTermsLookup()
        {
            Items itembo = new Items(pgmDB, _username, environment);
            DataSet dsTerms = itembo.GetDelTermsTbl();
            return dsTerms.Tables["DelTerms"];
        }

        public DataTable PortofDepartureLookup()
        {
            Items itembo = new Items(pgmDB, _username, environment);
            DataSet dsPorts = itembo.GetPortsTbl();
            return dsPorts.Tables["Ports"];
        }

        public DataTable DepartmentLookup()
        {
            IPDEPTS ipdeptscls = new IPDEPTS(pgmDB, environment);
            DataTable dtdepttable = ipdeptscls.GetDepartmentTbl();
            DataTable dtAuthoriseddepts = _username.AuthorisedDepartments;

            if (dtAuthoriseddepts.Columns.Count == 1)
            {
                dtAuthoriseddepts.Columns.Add(new DataColumn("Description", typeof(string)));
            }
            foreach (DataRow row in dtAuthoriseddepts.Rows)
            {
                if (ipdeptscls.GetDepartment(Int16.Parse(row["Department"].ToString())))
                {
                    row["Description"] = ipdeptscls.DepartmentName;
                }
            }
            return dtAuthoriseddepts;
        }

        public DataTable CurrencyLookup()
        {
            IPCURCY ipcurrencycls = new IPCURCY(pgmDB, environment);
            return ipcurrencycls.GetCurrencyTbl();
        }

        public DataSet VendorLookup()
        {
            Disney.Spice.ItemsBO.Items ibovendor = new Items(pgmDB, _username, environment);
            return ibovendor.GetVendorTbl();
        }

        public DataTable PopulateSSD()
        {
            DSSPSSDcls ssddata = new DSSPSSDcls(pgmDB);
            DataTable dsSSDData = ssddata.GetStageDataSet();

            return dsSSDData;
        }
    }

    public class LookUpDefaultDC
    {
        public LookUpDefaultDC()
        {            
        }

        public static DataView GetDefaultDCDV(Disney.Menu.Environments environment, String Market)
        {
            DataTable MarketDCDT = new DataTable();
            DataSet MarketDS = new DataSet();
            MarketDS.ReadXml(Path.Combine(environment.PathToEnvironmentXML, "MarketDC.xml"));

            MarketDCDT = MarketDS.Tables["MarketDC"];
            
            DataView MarketDCDV = new DataView(MarketDCDT);
            StringBuilder searchexpression = new StringBuilder();
            searchexpression.Append("Domain='");
            searchexpression.Append(environment.Domain.Trim());
            searchexpression.Append("' and IPMarket='");
            searchexpression.Append(Market.Trim());
            searchexpression.Append("'");
            //searchexpression.Append("' and @Default='1']");

            MarketDCDV.RowFilter = searchexpression.ToString();                        

            
            return MarketDCDV;
        }


        public static String GetDefaultDC(Disney.Menu.Environments environment, String Market)
        {
            String DocumentName = Path.Combine(environment.PathToEnvironmentXML, "MarketDC.xml");

            XPathDocument xmldoc = new XPathDocument(DocumentName);
            XPathNavigator nav = xmldoc.CreateNavigator();

            StringBuilder searchexpression = new StringBuilder();
            searchexpression.Append("//MarketDC[@Domain='");
            searchexpression.Append(environment.Domain.Trim());
            searchexpression.Append("' and @IPMarket='");
            searchexpression.Append(Market.Trim());
            searchexpression.Append("' and @Default='1']");

            XPathNodeIterator nodeiter = nav.Select(searchexpression.ToString());
            nodeiter.MoveNext();
            String dc = nodeiter.Current.GetAttribute("DCCode", String.Empty);

            return dc;
        }
    }

    public class LookUpEDIdates
    {
        private ASNA.VisualRPG.Runtime.Database pgmDB;

        public LookUpEDIdates(ASNA.VisualRPG.Runtime.Database pgmDB)
        {
            this.pgmDB = pgmDB;
        }

        internal DataTable GetSCBEDIdate(String IPpoNumber)
        {
            DSpoSND scbedi = new DSpoSND(pgmDB);
            return scbedi.GetEDIdata(IPpoNumber);
        }

        internal DataTable GetOrderEDIdates(String IPpoNumber)
        {
            dspohdr DisneyOrderHeader = new dspohdr(pgmDB);
            return DisneyOrderHeader.GetHeader(IPpoNumber);
        }

        public EdiDates GetEdiDates(String IPpoNumber)
        {
            EdiDates dates = new EdiDates();

            DataTable dtSCB = GetSCBEDIdate(IPpoNumber);
            if (dtSCB != null)
            {
                dates.SCBsendDate = (System.DateTime)dtSCB.Rows[0]["SCBdate"];
            }

            DataTable dtEDI = GetOrderEDIdates(IPpoNumber);
            if (dtEDI != null)
            {
                dates.OOCLsendDate  = (System.DateTime)dtEDI.Rows[0]["OOCLdate"];
                dates.AverySendDate = (System.DateTime)dtEDI.Rows[0]["AveryDate"];
            }

            return dates;
        }
    }

    public class EdiDates
    {
        public EdiDates()
        {
        }

        private System.DateTime m_SCBsendDate;
        public System.DateTime SCBsendDate
        {
            get { return m_SCBsendDate; }
            set { m_SCBsendDate = value; }
        }
        private System.DateTime m_OOCLsendDate;
        public System.DateTime OOCLsendDate
        {
            get { return m_OOCLsendDate; }
            set { m_OOCLsendDate = value; }
        }
        private System.DateTime m_AverySendDate;
        public System.DateTime AverySendDate
        {
            get { return m_AverySendDate; }
            set { m_AverySendDate = value; }
        }
    }
}
