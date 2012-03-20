using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.EASBO
{
    public class Vendor
    {
        private iDB2Connection DB2connect;
        private DAL.Vendor vendor;

        public Vendor(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public Int32 VendorNumber
        {
            get;
            set;
        }

        public String VendorName
        {
            get;
            set;
        }

        public Boolean GetVendorName(Int32 VendorNumber)
        {
            if (vendor == null)
            {
                vendor = new DAL.Vendor(DB2connect);
            }

            DataTable vendortbl = vendor.GetVendorDetails(VendorNumber);

            if (vendortbl != null)
            {
                this.VendorNumber = VendorNumber;
                this.VendorName   = (String)vendortbl.Rows[0]["VNAM"];

                return true;
            }
            else return false;
        }
    }
}