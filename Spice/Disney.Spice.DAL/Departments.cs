using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class Departments
    {
        private iDB2Connection DB2connect;

        public Departments(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetDepartmentsTable()
        {
            String SelectCmd = "Select DDIV,DDPT,DNAM from IPDEPTS";
            iDB2DataAdapter Adapter = new iDB2DataAdapter(SelectCmd, DB2connect);

            DataTable dt = new DataTable("Departments");
            Adapter.Fill(dt);

            return dt;
        }
    }
}