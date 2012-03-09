using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using Disney.DA.IP400;
using ASNA.VisualRPG.Runtime;
using Disney.Spice.ItemsBO;


namespace Disney.Spice.POBO
{
    public class LookupBO
    {
        private ASNA.VisualRPG.Runtime.Database _dbparamref;
        private Disney.Menu.Users _username;
        private Disney.Menu.Environments _paramenv;
        private DataSet _dtMarkets;

        public LookupBO(ASNA.VisualRPG.Runtime.Database dbparamref, Disney.Menu.Users username, Disney.Menu.Environments paramenv)
        {
            _dbparamref = dbparamref;
            _username = username;
            _paramenv = paramenv;


        }

        public DataTable ShipviaLookup()
        {

            Items itembo = new Items(_dbparamref, _username, _paramenv);
            DataSet dsTerms = itembo.GetShipViaTbl();
            return dsTerms.Tables["ShipVia"];
        }

        public DataTable DeliveryTermsLookup()
        {

            Items itembo = new Items(_dbparamref, _username, _paramenv);
            DataSet dsTerms = itembo.GetDelTermsTbl();
            return dsTerms.Tables["DelTerms"];


        }

        public DataTable PortofDepartureLookup()
        {
            Items itembo = new Items(_dbparamref, _username, _paramenv);
            DataSet dsPorts = itembo.GetPortsTbl();
            return dsPorts.Tables["Ports"];

        }

        public DataTable DepartmentLookup()
        {

            IPDEPTS ipdeptscls = new IPDEPTS(_dbparamref);
            DataTable dtdepttable = ipdeptscls.GetDepartmentTbl();
            DataTable dtAuthoriseddepts = _username.AuthorisedDepartments;

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

        public DataTable CurrencyLookup()
        {

            IPCURCY ipcurrencycls = new IPCURCY(_dbparamref);

            return ipcurrencycls.GetCurrencyTbl();

        }

        public DataSet VendorLookup()
        {
            //Returns a dataset unlike others
            Disney.Spice.ItemsBO.Items ibovendor = new Items(_dbparamref, _username, _paramenv);
            return ibovendor.GetVendorTbl();

        }

        public DataTable PopulateSSD()
        {
            DSSPSSDcls ssddata = new DSSPSSDcls(_dbparamref);

            DataTable dsSSDData = ssddata.GetStageDataSet();

            return dsSSDData;
        }
    }
}