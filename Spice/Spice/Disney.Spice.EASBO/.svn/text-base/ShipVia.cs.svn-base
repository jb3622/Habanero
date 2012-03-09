using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;
using Disney.Spice.DAL;

namespace Disney.Spice.EASBO
{
    public class ShipVia
    {
        private iDB2Connection DB2connect;
        private DAL.ShipVia shipvia;
        private DataTable shipviatbl;
        private String m_shipViaCode;
        private String m_description;

        public String ShipViaCode
        {
            get { return m_shipViaCode; }
            set { m_shipViaCode = value; }   
        }

        public String Description
        {
            get { return m_description; }
            set { m_description = value; }   
        }

        public ShipVia(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public Boolean GetShipViaDescription(String ShipViaCode)
        {
            if (shipvia == null)
            {
                shipvia = new DAL.ShipVia(DB2connect);
            }

            if (shipviatbl == null)
            {
                shipviatbl = shipvia.GetShipViaTable();

                DataColumn[] key = new DataColumn[] { shipviatbl.Columns["SHCDE"] };
                shipviatbl.PrimaryKey = key;
            }

            if (shipviatbl != null)
            {
                DataRow dr = shipviatbl.Rows.Find(ShipViaCode);
                if (dr != null)
                {
                    m_shipViaCode = ShipViaCode;
                    m_description = (String)dr["SHDSC"];

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}