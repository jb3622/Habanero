using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.EASBO
{
    public class Departments
    {
        private iDB2Connection  DB2connection;
        private DAL.Departments DepartmentsDAclass;
        private DataTable DepartmentsTbl;
        private int m_departmentNumber;
        private String m_departmentName;
        private int m_departmentDivision;

        public Departments(iDB2Connection DB2connection)
        {
            this.DB2connection = DB2connection;
        }

        public int DepartmentNumber
        {
            get { return m_departmentNumber; }
            set { m_departmentNumber = value; }
        }

        public String DepartmentName
        {
            get { return m_departmentName; }
            set { m_departmentName = value; }
        }

        public int DepartmentDivision
        {
            get { return m_departmentDivision; }
            set { m_departmentDivision = value; }
        }

        public Boolean GetDepartmentDetails(int DepartmentNumber)
        {
            if (DepartmentsDAclass == null) DepartmentsDAclass = new DAL.Departments(DB2connection);

            if (DepartmentsTbl == null)
            {
                DepartmentsTbl = DepartmentsDAclass.GetDepartmentsTable();

                DataColumn[] key = new DataColumn[] { DepartmentsTbl.Columns["DDPT"] };
                DepartmentsTbl.PrimaryKey = key;
            }

            if (DepartmentsTbl != null)
            {
                DataRow dr = DepartmentsTbl.Rows.Find(DepartmentNumber);
                if (dr != null)
                {
                    m_departmentNumber = DepartmentNumber;
                    m_departmentName = (String)dr["DNAM"];
                    m_departmentDivision = Convert.ToInt32(dr["DDIV"]);

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