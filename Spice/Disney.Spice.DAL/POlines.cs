using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class POlines
    {
        private iDB2Connection DB2connect;

        public POlines(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetPOlines(String POnumber, Int16 POversion)
        {
            StringBuilder SelectCmd = new StringBuilder();

            SelectCmd.Append("Select POISPO, POIVER, POISEQ, POICLS, POIVEN, POISTY, POICLR, POISIZ, POIDES, POISDS, POICPG, POIQTY, POIVNC, POIRET,");
            SelectCmd.Append(" ISACHR, ISACGP, ISACPT, ISAPRE, IIAAPP, ISADES, CHDESC, CDES, PTCODE, CADES, POILNC ");
            SelectCmd.Append(" From DSSPPOIL1");

            SelectCmd.Append(" Left Outer Join DSSPISAL1 On POICLS = ISACLS And POIVEN = ISAVEN And POISTY = ISASTY And POICLR = ISACLR");
            SelectCmd.Append(" Left Outer Join DSSPIIAL1 On POICLS = IIACLS And POIVEN = IIAVEN And POISTY = IIASTY And POICLR = IIACLR And POISIZ = IIASIZ");
            SelectCmd.Append(" Left Outer Join DSCHARS   On ISACHR = CHCODE");
            SelectCmd.Append(" Left Outer Join DSCORGPJ  On ISACGP = CCDE");
            SelectCmd.Append(" Left Outer Join DSPTCKT   On ISAPRE = PTCODE");
            SelectCmd.Append(" Left Outer Join DSCASPK   On IIACPT = CACOD");

            SelectCmd.Append(" Where POISPO = '");
            SelectCmd.Append(POnumber);
            SelectCmd.Append("' And POIVER = '");
            SelectCmd.Append(POversion.ToString());
            SelectCmd.Append("'");

            iDB2DataAdapter Adapter = new iDB2DataAdapter(SelectCmd.ToString(), DB2connect);

            DataTable polinestbl = new DataTable("POlines");
            Adapter.Fill(polinestbl);

            return polinestbl;
        }
    }
}