using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.EASBO
{
    public class Vendors
    {
        private iDB2Connection DB2connect;
        private DAL.Vendors vendor;
        private int m_vendorNumber;
        private string m_name;

        public Vendors(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public int VendorNumber
        {
            get { return m_vendorNumber; }
            set { m_vendorNumber = value; }
        }

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public Boolean GetVendorName(int VendorNumber)
        {
            if (vendor == null)
            {
                vendor = new DAL.Vendors(DB2connect);
            }

            DataTable vendortbl = vendor.GetVendorDetails(VendorNumber);

            if (vendortbl != null)
            {
                m_vendorNumber    = VendorNumber;
                m_name            = (String)vendortbl.Rows[0]["VNAM"];

                return true;
            }
            else return false;
        }
    }
}