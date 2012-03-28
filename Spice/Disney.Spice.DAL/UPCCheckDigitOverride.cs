using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class UPCCheckDigitOverride
    {
        private iDB2Connection DB2connect;

        public UPCCheckDigitOverride(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public bool OverrideUPCCDValidation(string upc)
        {
            StringBuilder cmd = new StringBuilder();
            cmd.Append("Select JWUPC12 from DS406JW WHERE JWUPC12 = '");
            cmd.Append(upc.Trim());
            cmd.Append("'");
                        
            iDB2DataAdapter Adapter = new iDB2DataAdapter(cmd.ToString(), DB2connect);

            DataTable dt = new DataTable("UPCCheckDigits");
            Adapter.Fill(dt);

            if (dt.Rows.Count > 1)
            {
                return false;   
            }
            else
            {                
                return true; 
            }
        }

    }
}