using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.EASBO
{
    public class DeliveryTerms
    {
        private iDB2Connection DB2connection;
        private DAL.DeliveryTerms deliveryterms;
        private DataTable deliverytermstable;
        private String m_deliveryTermsCode;
        private String m_description;


        public DeliveryTerms(iDB2Connection DB2connection)
        {
            this.DB2connection = DB2connection;
        }

        public String DeliveryTermsCode
        {
            get { return m_deliveryTermsCode; }
            set { m_deliveryTermsCode = value; }
        }

        public String Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        public Boolean GetDeliveryTerms(String DeliveryTermsCode)
        {
            if (deliveryterms == null)
            {
                deliveryterms = new DAL.DeliveryTerms(DB2connection);
            }

            if (deliverytermstable == null)
            {
                deliverytermstable = deliveryterms.GetDeliveryTermsTable();

                DataColumn[] key = new DataColumn[] { deliverytermstable.Columns["DTDLVR"] };
                deliverytermstable.PrimaryKey = key;
            }

            if (deliverytermstable != null)
            {
                DataRow dr = deliverytermstable.Rows.Find(DeliveryTermsCode);
                if (dr != null)
                {
                    m_deliveryTermsCode = DeliveryTermsCode;
                    m_description       = (String)dr["DTDESC"];

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