using System;
using System.Collections.Generic;
using System.Text;
using Disney.DA.IP400;
using System.Data;
using Disney.Spice.ItemsBO;

namespace Disney.Spice.POBO
{
    public class AssortedPrePack
    {

        SPICECommon _commonparam;
        POItemDetails _poline;

        public AssortedPrePack(POItemDetails poline, SPICECommon commonparams)
        {
            _commonparam = commonparams;
            _poline = poline;

        }

        public DataTable PopulateAPPComponents(string spiceponumber, short spicepoversion, short posequence)
        {
            
            DataTable dtAPPComponents = new DataTable();

            DSSPPOA clsAPP = new DSSPPOA(_commonparam.Dbparamref);

            dtAPPComponents = clsAPP.GetAPPcomponents (spiceponumber, spicepoversion, posequence);

            return dtAPPComponents;

        }

        public DataTable PopulateAPPStructure(int Qty)
        {

            DSSPPOA clsAPP = new DSSPPOA(_commonparam.Dbparamref);

            DataTable dtAPPComponents = clsAPP.GetAPPstructure(_poline.Classcode, _poline.Vendorcode, _poline.Stylecode, _poline.Colorcode, _poline.Itemsize);

            //Copy for use later.
            DataTable dtAPPActualComponents = dtAPPComponents.Copy();

            dtAPPComponents.Columns.Add("OrderQuantity", typeof(int));
            dtAPPComponents.Columns.Add("Retail",        typeof(decimal));

            // ///////////////////////////////////////////////////////////////
            // HK : 18-01-2010 : Fix Bug 233

            dtAPPComponents.Columns.Add("ConvertedCost",    typeof(decimal));
            dtAPPComponents.Columns.Add("LandedCost",       typeof(decimal));

            // ///////////////////////////////////////////////////////////////

            foreach (DataRow dtrow in dtAPPComponents.Rows)
            {

                //Look through each item to get the unit cost associated with the item via lookup.
                ItemsBO.Items itembo = new Items(_commonparam.Dbparamref, _commonparam.Username, _commonparam.Penvironment);

                if (itembo.GetItem((short)dtrow["ComponentClass"], (Int32)dtrow["ComponentVendor"], (short)(short)dtrow["ComponentStyle"], (short)dtrow["ComponentColour"], (short)(short)dtrow["ComponentSize"], _commonparam.DefaultMarket))
                {
                    short nominalqty = short.Parse(dtrow["ComponentQuantity"].ToString());

                    dtrow["ComponentCost"]  = itembo.ItemCost;
                    dtrow["Retail"]         = itembo.ItemRetail;
                    dtrow["OrderQuantity"]  = (nominalqty == 0) ? 0 : (nominalqty * Qty);

                    // ////////////////////////////////////////////////////////////////
                    // HHK : 13-11-2009 : 
                    dtrow["ComponentLongDesc"] = itembo.ItemLongDescription;
                    // ////////////////////////////////////////////////////////////////
                    
                }
            }

            return dtAPPComponents;
        }
    }
}