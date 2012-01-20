using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class POcomponents
    {
        private iDB2Connection DB2connection;

        public POcomponents(iDB2Connection DB2connection)
        {
            this.DB2connection = DB2connection;
        }

        public DataTable GetPOcomponents(String POnumber, short POversion, short POsequence)
        {
            StringBuilder SelectCmd = new StringBuilder();

            SelectCmd.Append("Select POACLS, POAVEN, POASTY, POACLR, POASIZ,  ISADES, POAQTY, POAVNC, IMIRP, (POIQTY * POAQTY) as ComponentQty  From DSSPPOAL1");

            SelectCmd.Append(" Left Outer Join DSSPPOHL1 On POASPO = POHSPO And POAVER = POHVER");
            SelectCmd.Append(" Left Outer Join DSSPPOIL1 On POASPO = POISPO And POAVER = POIVER And POASEQ = POISEQ");
            SelectCmd.Append(" Left Outer Join DSSPISAL1 On POACLS = ISACLS And POAVEN = ISAVEN And POASTY = ISASTY And POACLR = ISACLR");
            SelectCmd.Append(" Left Outer Join DSSPIMIL1 On POACLS = IMICLS And POAVEN = IMIVEN And POASTY = IMISTY And POACLR = IMICLR And POASIZ = IMISIZ And POHMKT = IMIMKT");

            SelectCmd.Append(" Where POASPO = '");
            SelectCmd.Append(POnumber);
            SelectCmd.Append("' And POAVER = '");
            SelectCmd.Append(POversion.ToString());
            SelectCmd.Append("' And POASEQ = '");
            SelectCmd.Append(POsequence.ToString());
            SelectCmd.Append("'");

            iDB2DataAdapter Adapter = new iDB2DataAdapter(SelectCmd.ToString(), DB2connection);

            DataTable poComponents = new DataTable("POcomponents");
            Adapter.Fill(poComponents);

            return poComponents;
        }
    }
}