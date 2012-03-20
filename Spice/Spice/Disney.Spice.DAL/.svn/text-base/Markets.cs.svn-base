using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using Disney.Menu;

namespace Disney.Spice.DAL
{
    public class Markets
    {
        private DataTable MarketTbl;

        public Markets(Environments environment)
        {
            DataSet MarketDS = new DataSet();
		    MarketDS.ReadXml(Path.Combine(environment.PathToEnvironmentXML,"Markets.xml"));

		    this.MarketTbl = new DataTable();
            MarketTbl = MarketDS.Tables["Market"].Clone();

		    DataRow[] drMarkets = MarketDS.Tables["Market"].Select("Status = '1'");
            foreach (DataRow MktRow in drMarkets)
            {
                MarketTbl.ImportRow(MktRow);
            }
		
		    MarketTbl.TableName = "AllMarkets";
            DataColumn[] Keys = new DataColumn[] {MarketTbl.Columns["IPMarket"]};
		    MarketTbl.PrimaryKey = Keys;
        }

        public string GetMarketDescription(string MarketCode)
        {
            DataRow dr = MarketTbl.Rows.Find(MarketCode);
            if (dr != null)
            {
                return (string)dr["IPMarketDesc"];
            }
            return string.Empty;
        }
    }
}