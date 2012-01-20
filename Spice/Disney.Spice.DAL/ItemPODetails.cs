using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class ItemPODetails
    {
        private iDB2Connection DB2connect;

        public ItemPODetails(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetItemPODetails( Int16 Class, Int32 Vendor, Int16 Style, Int16 Colour, Int16 Size, String SpicePO, Int16 SpicePOVersion, Int16 LineSequence )
        {
            StringBuilder cmd = new StringBuilder();
            cmd.Append("Select POIQTY, POIRET, POIVNC, POILNF, POHCYC, POHCYR, POILNC, CSCUR# ");
           cmd.Append("from DSSPPOIL2");

            cmd.Append(" Left Outer Join DSSPPOHL1 On POHSPO = POISPO and POHVER = POIVER ");
            cmd.Append(" Left Outer Join DSCNTAP   On POHMKT = CSCODE ");

            cmd.Append("where POICLS = ");
            cmd.Append(Class.ToString());
            cmd.Append(" and POIVEN = ");
            cmd.Append(Vendor.ToString());
            cmd.Append(" and POISTY = ");
            cmd.Append(Style.ToString());
            cmd.Append(" and POICLR = ");
            cmd.Append(Colour.ToString());
            cmd.Append(" and POISIZ = ");
            cmd.Append(Size.ToString());
            cmd.Append(" and POISPO = '");
            cmd.Append(SpicePO.ToString());
            cmd.Append("'");
            cmd.Append(" and POIVER = ");
            cmd.Append(SpicePOVersion.ToString());
            cmd.Append(" and POISEQ = ");
            cmd.Append(LineSequence.ToString());
                                    
            iDB2DataAdapter Adapter = new iDB2DataAdapter(cmd.ToString(), DB2connect);

            DataTable dtItemPO = new DataTable("ItemPODetails");
            Adapter.Fill(dtItemPO);
            
            return dtItemPO;
        }
    }
}