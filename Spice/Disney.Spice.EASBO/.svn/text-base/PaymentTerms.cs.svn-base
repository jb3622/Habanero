using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;
using Disney.Spice.DAL;

namespace Disney.Spice.EASBO
{
    public class PaymentTerms
    {
        private iDB2Connection DB2connect;
        private DAL.PaymentTerms terms;
        private String m_termsCode;
        private String m_description;

        public String TermsCode
        {
            get { return m_termsCode; }
            set { m_termsCode = value; }
        }

        public String Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        public PaymentTerms(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public Boolean GetPaymentTerms(String Terms)
        {
            if (terms == null)
            {
                terms = new DAL.PaymentTerms(DB2connect);
            }

            DataTable dt = terms.GetPaymentTerms(Terms);
            if (dt != null)
            {
                m_termsCode = Terms;
                m_description = (String)dt.Rows[0]["TDES"];

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}