using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.DAL
{
    public class LocalLanguage
    {
        private iDB2Connection DB2connect;

        public LocalLanguage(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public DataTable GetItemStyleDescription(Int16 Class, Int32 Vendor, Int16 Style, Int16 Colour, string localLanguage)
        {
            StringBuilder cmd = new StringBuilder();

            cmd.Append("SELECT * FROM DSSPISD ");
            cmd.Append("WHERE ISDLAN = '" + localLanguage.ToUpper() + "' ");
            cmd.Append("AND ISDCLS = " + Class.ToString() + " ");
            cmd.Append("AND ISDVEN = " + Vendor.ToString() + " ");
            cmd.Append("AND ISDSTY = " + Style.ToString() + " ");
            cmd.Append("AND ISDCLR = " + Colour.ToString() + " ");

            iDB2DataAdapter Adapter = new iDB2DataAdapter(cmd.ToString(), DB2connect);

            DataTable dt = new DataTable("LangTranslationTbl");
            Adapter.Fill(dt);

            return dt;
        }

        public String GetItemStyleDescription(Int16 Class, Int32 Vendor, Int16 Style, Int32 Colour, string localLanguage, out Boolean found)
        {
            StringBuilder cmd = new StringBuilder();
            String description = String.Empty;

            found = false;

            cmd.Append("SELECT ISDDES FROM DSSPISD ");
            cmd.Append("WHERE ISDLAN = '" + localLanguage.ToUpper() + "' ");
            cmd.Append("AND ISDCLS = " + Class.ToString() + " ");
            cmd.Append("AND ISDVEN = " + Vendor.ToString() + " ");
            cmd.Append("AND ISDSTY = " + Style.ToString() + " ");
            cmd.Append("AND ISDCLR = " + Colour.ToString() + " ");

            try
            {
                IBM.Data.DB2.iSeries.iDB2Command selectCommand = new iDB2Command(cmd.ToString(), CommandType.Text, DB2connect);

                IBM.Data.DB2.iSeries.iDB2DataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                {
                    description = reader[0].ToString();
                    found = true;
                }
            }
            catch (Exception ex)
            {                
                throw ex;
            }
            
            return description;
        }

        public DataTable GetItemSizeDescription(Int16 Class, Int32 Vendor, Int16 Style, Int16 Colour, Int16 Size, string localLanguage)
        {
            StringBuilder cmd = new StringBuilder();

            cmd.Append("SELECT * FROM DSSPILD ");
            cmd.Append("WHERE ILDLAN = '" + localLanguage.ToUpper() + "' " );
            cmd.Append("AND ILDCLS = " + Class.ToString() + " ");
            cmd.Append("AND ILDVEN = " + Vendor.ToString() + " ");
            cmd.Append("AND ILDSTY = " + Style.ToString() + " ");
            cmd.Append("AND ILDCLR = " + Colour.ToString() + " ");
            cmd.Append("AND ILDSIZ = " + Size.ToString() + " ");

            iDB2DataAdapter Adapter = new iDB2DataAdapter(cmd.ToString(), DB2connect);

            DataTable dt = new DataTable("LangTranslationTbl");
            Adapter.Fill(dt);

            return dt;
        }

        public String GetItemSizeDescription(Int16 Class, Int32 Vendor, Int16 Style, Int32 Colour, Int16 Size, string localLanguage, out Boolean found)
        {
            StringBuilder cmd = new StringBuilder();
            String description = String.Empty;

            found = false;

            cmd.Append("SELECT ILDDES FROM DSSPILD ");
            cmd.Append("WHERE ILDLAN = '" + localLanguage.ToUpper() + "' ");
            cmd.Append("AND ILDCLS = " + Class.ToString() + " ");
            cmd.Append("AND ILDVEN = " + Vendor.ToString() + " ");
            cmd.Append("AND ILDSTY = " + Style.ToString() + " ");
            cmd.Append("AND ILDCLR = " + Colour.ToString() + " ");
            cmd.Append("AND ILDSIZ = " + Size.ToString() + " ");

            try
            {
                iDB2Command selectCommand = new iDB2Command(cmd.ToString(), CommandType.Text, DB2connect);

                iDB2DataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                {
                    description = reader[0].ToString();
                    found = true;
                }
            }
            catch (Exception ex)
            {                
                throw ex;
            }
            
            return description;
        }

        public Boolean SaveItemStyleDescription(Int16 Class, Int32 Vendor, Int16 Style, Int16 Colour, string localLanguage, string description)
        {
            Boolean result = false;

            IBM.Data.DB2.iSeries.iDB2Parameter param;

            try
            {
                if (description != String.Empty && description != "" )
                {
                    iDB2Command updateCommand = new iDB2Command("DS405PS1", CommandType.StoredProcedure, DB2connect);
                    //iDB2Command updateCommand = new iDB2Command("DSSPLLS_IU", CommandType.StoredProcedure, DB2connect);

                    param = new iDB2Parameter("P_CLASS", IBM.Data.DB2.iSeries.iDB2DbType.iDB2Decimal, 4);
                    param.Direction = ParameterDirection.Input;
                    param.Value = Class;
                    updateCommand.Parameters.Add(param);

                    param = new iDB2Parameter("P_VENDOR", IBM.Data.DB2.iSeries.iDB2DbType.iDB2Decimal, 5);
                    param.Direction = ParameterDirection.Input;
                    param.Value = Vendor;
                    updateCommand.Parameters.Add(param);

                    param = new iDB2Parameter("P_STYLE", IBM.Data.DB2.iSeries.iDB2DbType.iDB2Decimal, 4);
                    param.Direction = ParameterDirection.Input;
                    param.Value = Style;
                    updateCommand.Parameters.Add(param);

                    param = new iDB2Parameter("P_COLOUR", IBM.Data.DB2.iSeries.iDB2DbType.iDB2Decimal, 3);
                    param.Direction = ParameterDirection.Input;
                    param.Value = Colour;
                    updateCommand.Parameters.Add(param);

                    param = new iDB2Parameter("P_LANG", IBM.Data.DB2.iSeries.iDB2DbType.iDB2Char, 12);
                    param.Direction = ParameterDirection.Input;
                    param.Value = localLanguage.ToUpper();
                    updateCommand.Parameters.Add(param);

                    param = new iDB2Parameter("P_DESC", IBM.Data.DB2.iSeries.iDB2DbType.iDB2Graphic, 30);
                    param.Direction = ParameterDirection.Input;
                    param.Value = description;
                    updateCommand.Parameters.Add(param);

                    updateCommand.ExecuteNonQuery();
                }

                result = true;
            }
            catch (Exception ex)
            {                
                throw ex;
            }

            return result;
        }

        public Boolean SaveItemSizeDescription(Int16 Class, Int32 Vendor, Int16 Style, Int16 Colour, 
            Int16 Size, string localLanguage, string description)
        {
            Boolean result = false;

            IBM.Data.DB2.iSeries.iDB2Parameter param;

            try
            {
                if (description != String.Empty && description != "")
                {
                    iDB2Command updateCommand = new iDB2Command("DS405QS1", CommandType.StoredProcedure, DB2connect);
                    //iDB2Command updateCommand = new iDB2Command("DSSPLLI_IU", CommandType.StoredProcedure, DB2connect);

                    param = new iDB2Parameter("P_CLASS", IBM.Data.DB2.iSeries.iDB2DbType.iDB2Decimal);
                    param.Direction = ParameterDirection.Input;
                    param.Value = Class;
                    updateCommand.Parameters.Add(param);

                    param = new iDB2Parameter("P_VENDOR", IBM.Data.DB2.iSeries.iDB2DbType.iDB2Decimal);
                    param.Direction = ParameterDirection.Input;
                    param.Value = Vendor;
                    updateCommand.Parameters.Add(param);

                    param = new iDB2Parameter("P_STYLE", IBM.Data.DB2.iSeries.iDB2DbType.iDB2Decimal);
                    param.Direction = ParameterDirection.Input;
                    param.Value = Style;
                    updateCommand.Parameters.Add(param);

                    param = new iDB2Parameter("P_COLOUR", IBM.Data.DB2.iSeries.iDB2DbType.iDB2Decimal);
                    param.Direction = ParameterDirection.Input;
                    param.Value = Colour;
                    updateCommand.Parameters.Add(param);

                    param = new iDB2Parameter("P_SIZE", IBM.Data.DB2.iSeries.iDB2DbType.iDB2Decimal);
                    param.Direction = ParameterDirection.Input;
                    param.Value = Size;
                    updateCommand.Parameters.Add(param);

                    param = new iDB2Parameter("P_LANG", IBM.Data.DB2.iSeries.iDB2DbType.iDB2Char, 12);
                    param.Direction = ParameterDirection.Input;
                    param.Value = localLanguage.ToUpper();
                    updateCommand.Parameters.Add(param);

                    param = new iDB2Parameter("P_DESC", IBM.Data.DB2.iSeries.iDB2DbType.iDB2Graphic);
                    param.Direction = ParameterDirection.Input;
                    param.Value = description;
                    updateCommand.Parameters.Add(param);

                    updateCommand.ExecuteNonQuery();
                }

                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

    }
}