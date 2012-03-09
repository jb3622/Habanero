using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class SizeDetails
    {
        private iDB2Connection DB2connect;

        public SizeDetails(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetSizeDetails(Int16 Size)
        {
            StringBuilder cmd = new StringBuilder();
            cmd.Append("Select SNAM from IPSIZES where SSIZ = ");
            cmd.Append(Size.ToString());
                                    
            iDB2DataAdapter Adapter = new iDB2DataAdapter(cmd.ToString(), DB2connect);

            DataTable dtSize = new DataTable("SizeDetails");
            Adapter.Fill(dtSize);
            
            return dtSize;
        }
    }
}