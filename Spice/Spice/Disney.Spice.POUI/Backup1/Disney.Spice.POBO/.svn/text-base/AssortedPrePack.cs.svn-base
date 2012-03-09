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
        private PurchaseOrder purchaseOrderObject;
        private POItemDetails purchaseOrderLine;

        public AssortedPrePack(POItemDetails poLine, PurchaseOrder purchaseOrder)
        {
            purchaseOrderLine = poLine;
            purchaseOrderObject = purchaseOrder;
        }

        public DataTable PopulateAPPComponents(string spiceponumber, short spicepoversion, short posequence)
        {
            DataTable dtAPPComponents = new DataTable();

            DSSPPOA clsAPP = new DSSPPOA(purchaseOrderObject.DbParamRef);

            dtAPPComponents = clsAPP.GetAPPcomponents(spiceponumber, spicepoversion, posequence);
            
            dtAPPComponents.Columns.Add("Retail", typeof(decimal));

            foreach (DataRow dtrow in dtAPPComponents.Rows)
            {
                //Look through each item to get the unit cost associated with the item via lookup.
                ItemsBO.Items itembo = new Items(purchaseOrderObject.DbParamRef, purchaseOrderObject.UserName, purchaseOrderObject.Penvironment);

                if (itembo.GetItem((short)dtrow["ComponentClass"], (Int32)dtrow["ComponentVendor"], (short)(short)dtrow["ComponentStyle"], (short)dtrow["ComponentColour"], (short)(short)dtrow["ComponentSize"], purchaseOrderObject.DefaultMarket))
                {
                    dtrow["Retail"] = itembo.ItemRetail;
                    dtrow["ComponentLongDesc"] = itembo.ItemLongDescription;
                }
            }

            return dtAPPComponents;
        }

        public DataTable PopulateAPPStructure()
        {
            DSSPPOA clsAPP = new DSSPPOA(purchaseOrderObject.DbParamRef);

            DataTable dtAPPComponents = clsAPP.GetAPPstructure(purchaseOrderLine.ClassCode, purchaseOrderLine.Vendorcode, purchaseOrderLine.Stylecode, purchaseOrderLine.Colorcode, purchaseOrderLine.Itemsize);

            DataTable dtAPPActualComponents = dtAPPComponents.Copy();

            //dtAPPComponents.Columns.Add("OrderQuantity", typeof(int));
            dtAPPComponents.Columns.Add("Retail",typeof(decimal));
            //dtAPPComponents.Columns.Add("ConvertedCost", typeof(decimal));
            //dtAPPComponents.Columns.Add("LandedCost",    typeof(decimal));

            foreach (DataRow dtrow in dtAPPComponents.Rows)
            {
                //Look through each item to get the unit cost associated with the item via lookup.
                ItemsBO.Items itembo = new Items(purchaseOrderObject.DbParamRef, purchaseOrderObject.UserName, purchaseOrderObject.Penvironment);

                if (itembo.GetItem((short)dtrow["ComponentClass"], (Int32)dtrow["ComponentVendor"], (short)(short)dtrow["ComponentStyle"], (short)dtrow["ComponentColour"], (short)(short)dtrow["ComponentSize"], purchaseOrderObject.DefaultMarket))
                {
                    //short nominalqty = short.Parse(dtrow["ComponentQuantity"].ToString());

                    dtrow["ComponentCost"]     = itembo.ItemCost;
                    dtrow["Retail"]            = itembo.ItemRetail;
                    dtrow["ComponentLongDesc"] = itembo.ItemLongDescription;
                }
            }

            return dtAPPComponents;
        }
    }
}