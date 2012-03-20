using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.EASBO
{
    public class Ports
    {
        private iDB2Connection DB2connection;
        private DAL.Ports portsClass;
        private DataTable portsTable;
        private Int32 m_portNumber;
        private String m_name;

        public Ports(iDB2Connection DB2connection)
        {
            this.DB2connection = DB2connection;
        }

        public Int32 PortNumber
        {
            get { return m_portNumber; }
            set { m_portNumber = value; }
        }

        public String Name
        {
            get { return m_name; }
            set { m_name = value; }        
        }

        public Boolean GetPortDetails(Int32 PortNumber)
        {
            if (portsClass == null)
            {
                portsClass = new DAL.Ports(DB2connection);
            }

            if (portsTable == null)
            {
                portsTable = portsClass.GetPortsTable();

                DataColumn[] key = new DataColumn[] { portsTable.Columns["PCDE"] };
                portsTable.PrimaryKey = key;
            }

            if (portsTable != null)
            {
                DataRow dr = portsTable.Rows.Find(PortNumber);
                if (dr != null)
                {
                    m_portNumber = PortNumber;
                    m_name = (String)dr["PDSC"];

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