using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class ClassDetails
    {
        private iDB2Connection DB2connect;

        public ClassDetails(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetClassDetails(Int16 Class)
        {
            StringBuilder cmd = new StringBuilder();
            cmd.Append("Select CLNM from IPCLASS where CCLS = ");
            cmd.Append(Class.ToString());
                                    
            iDB2DataAdapter Adapter = new iDB2DataAdapter(cmd.ToString(), DB2connect);

            DataTable dtClass = new DataTable("ClassDetails");
            Adapter.Fill(dtClass);
            
            return dtClass;
        }
    }
}