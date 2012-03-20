using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class ColourDetails
    {
        private iDB2Connection DB2connect;

        public ColourDetails(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetColourDetails(Int16 Colour)
        {
            StringBuilder cmd = new StringBuilder();
            cmd.Append("Select CLRN from IPCOLOR where CCLR = ");
            cmd.Append(Colour.ToString());
                                    
            iDB2DataAdapter Adapter = new iDB2DataAdapter(cmd.ToString(), DB2connect);

            DataTable dtColour = new DataTable("ColourDetails");
            Adapter.Fill(dtColour);
            
            return dtColour;
        }
    }
}