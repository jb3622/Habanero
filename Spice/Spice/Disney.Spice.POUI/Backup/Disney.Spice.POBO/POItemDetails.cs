using System;
using System.Collections.Generic;
using System.Text;
using Disney.DA.IP400;
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

namespace Disney.Spice.POBO
{

    public class POItemDetails : IComparable
    {

        private int itemindex;
        private bool isvalid;

        public delegate void delItemQtyChanged(int qty, decimal cost, int rowindex);

        public event delItemQtyChanged ItemQtyChanged;

        // All private val;iable that hold Public property values
        private short       classcode;
        private string      classname;
        private int         vendorcode;
        private string      vendordesc;
        private short       stylecode;
        private string      stylename;
        private short       colorcode;
        private string      colordesc;
        private string      characterdesc;
        private string      seasondesc;
        private short       itemsize;
        private string      _sizename;
        private decimal     cost;
        private string      _vendorstyle;
        private string      _subclass;
        private string      _tickettype;
        private int         _minPack;
        private decimal     retailprice;
        private string      itemshortdescription;
        private int         _distrolot;
        private string      _upc;
        private int         _sku;
        private short       _casepackqty;
        private string      itemlongdescription;
        private string      _packdescription;
        private string      app;
        private int         itemquantity;
        private string      _charactercode;
        private decimal     height;
        private decimal     length;
        private decimal     width;
        private decimal     weight;
        private string      _vendorRef;
        private string      _vatcode;

        private short       _poindex                = 0;
        private short       _skuchk;

        // HK : 
        private Boolean     _isduplicate;

        // HK : CJ : 10-12-2009 : Add Sequence NUmber property
        private short       _sequence;
        
        // HK : 15-01-2010
        private decimal     _landedcost;
        private decimal _convertedcost;

        // HK : CJ : 26-01-2010 : Fix Bug 311
        private string      _seasoncode;

        // All public properties

        public bool Isvalid
        {
            get { return isvalid; }
            set { isvalid = value; }
        }

        public int Itemindex
        {
            get { return itemindex; }
            set { itemindex = value; }
        }

        public short Classcode
        {
            get { return classcode; }
            set { classcode = value; }
        }

        public string Classname
        {
            get { return classname; }
            set { classname = value; }
        }

        public int Vendorcode
        {
            get { return vendorcode; }
            set { vendorcode = value; }
        }

        public string Vendordesc
        {
            get { return vendordesc; }
            set { vendordesc = value; }
        }

        public short Stylecode
        {
            get { return stylecode; }
            set { stylecode = value; }
        }

        public string Stylename
        {
            get { return stylename; }
            set { stylename = value; }
        }

        public short Colorcode
        {
            get { return colorcode; }
            set { colorcode = value; }
        }

        public string Colordesc
        {
            get { return colordesc; }
            set { colordesc = value; }
        }

        public string Characterdesc
        {
            get { return characterdesc; }
            set { characterdesc = value; }
        }

        public string SeasonDesc
        {
            get { return seasondesc; }
            set { seasondesc = value; }
        }

        public short Itemsize
        {
            get { return itemsize; }
            set { itemsize = value; }
        }

        public string Sizename
        {
            get { return _sizename; }
            set { _sizename = value; }
        }

        public decimal Cost
        {
            get { return cost; }
            set { cost = value; }
        }

        public string Vendorstyle
        {
            get { return _vendorstyle; }
            set { _vendorstyle = value; }
        }

        public string Subclass
        {
            get { return _subclass; }
            set { _subclass = value; }
        }

        public string Tickettype
        {
            get { return _tickettype; }
            set { _tickettype = value; }
        }

        public int MinPack
        {
            get { return _minPack; }
            set { _minPack = value; }
        }

        public int Distrolot
        {
            get { return _distrolot; }
            set { _distrolot = value; }
        }

        public string Upc
        {
            get { return _upc; }
            set { _upc = value; }
        }
        
        public int Sku
        {
            get { return _sku; }
            set { _sku = value; }
        }
        
        public short Casepackqty
        {
            get { return _casepackqty; }
            set { _casepackqty = value; }
        }
        
        public string Itemlongdescription
        {
            get { return itemlongdescription; }
            set { itemlongdescription = value; }
        }
        
        public decimal Retailprice
        {
            get { return retailprice; }
            set { retailprice = value; }
        }

        public string Itemshortdescription
        {
            get { return itemshortdescription; }
            set { itemshortdescription = value; }
        }

        public string Packdescription
        {
            get { return _packdescription; }
            set { _packdescription = value; }
        }

        public string Charactercode
        {
            get { return _charactercode; }
            set { _charactercode = value; }
        }

        public int Itemquantity
        {
            get { return itemquantity; }
            set { itemquantity = value; }
        }

        public string APP1
        {
            get { return app; }
            set { app = value; }
        }

        public decimal Height
        {
            get { return height; }
            set { height = value; }
        }

        public decimal Length
        {
            get { return length; }
            set { length = value; }
        }

        public decimal Width
        {
            get { return width; }
            set { width = value; }
        }

        public decimal Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        public string VendorRef
        {
            get { return _vendorRef; }
            set { _vendorRef = value; }
        }

        // HK / FC : 16-11-2009 : Add Vat Code property
        public string VatCode
        {
            get { return _vatcode; }
            set { _vatcode = value; }
        }

        public short SkuChk
        {
            get { return _skuchk; }
            set { _skuchk = value; }
        }

        public bool IsDuplicate
        {
            get { return _isduplicate; }
            set { _isduplicate = value; }
        }

        public short Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
                
        }

        public decimal ConvertedCost
        {
            get { return _convertedcost; }
            set { _convertedcost = value; }
        }

        public decimal LandedCost
        {
            get { return _landedcost; }
            set { _landedcost = value; }
        }

        public string SeasonCode
        {
            get { return _seasoncode; }
            set { _seasoncode = value; }
        }

        public POItemDetails(int index)
        { 
            itemindex = index;

            // //////////////////////////////////////////////////////////
            // HK : 14-11-2009 : Initalize the Components collection

            // //////////////////////////////////////////////////////////

            pocomponents = new POComponents();
        
        }

        // List of PO Line Details (class POItemDetails)
        List<POItemDetails> _lstComponentDetails;

        public List<POItemDetails> lstComponentDetails
        {
            get { return _lstComponentDetails; }
            set { _lstComponentDetails = value; }
        }

        // Collection of Po Item Details as Components
        public POComponents pocomponents;

        public POItemDetails(short sclassCode, int iVendorCode, short sstyle, short scolourcode, short ssize, int index)
        {

            classcode = sclassCode;
            vendorcode = iVendorCode;
            stylecode = sstyle;
            colorcode = scolourcode;
            itemsize = ssize;
            itemindex = index;

            // //////////////////////////////////////////////////////////
            // HK : 14-11-2009 : Initalize the Components collection
            _lstComponentDetails = new List<POItemDetails>();
            
            // //////////////////////////////////////////////////////////

            pocomponents = new POComponents();
        
        }

        public bool ItemLookup(ASNA.VisualRPG.Runtime.Database dbparamref, Disney.Menu.Users username, Disney.Menu.Environments penvironment, string defaultmarket)
        {

            // itemlookup from the BO
            bool bSuccess;
            try
            {

                ItemsBO.Items itembo = new Disney.Spice.ItemsBO.Items(dbparamref, username, penvironment);

                //Since we know the default market make use of this.
                bSuccess = itembo.GetItem(classcode, vendorcode, stylecode, colorcode, itemsize, defaultmarket);



                if (bSuccess)
                {
                    //Pack values into the item bo

                    cost = itembo.ItemCost;
                    itemlongdescription = itembo.ItemLongDescription;
                    retailprice = itembo.ItemRetail;
                    itemshortdescription = itembo.ItemShortDescription;
                    if (itembo.APP != "APP")
                    { app = "N"; }
                    else
                    { app = "Y"; }


                    characterdesc = itembo.CharacterName;
                    seasondesc = itembo.SeasonDesc;
                    //_sizename = itembo.SizeName; //Already populated previously.
                    height = itembo.PackHeight;
                    length = itembo.PackLength;
                    width = itembo.PackWidth;
                    weight = itembo.PackWeight;
                    _packdescription = itembo.PackDescription;
                    _upc = itembo.UPC.ToString(); //Long wtf ?
                    _tickettype = itembo.TicketType;
                    _subclass = itembo.SubClass;
                    _vendorstyle = itembo.VendorStyle;
                    _distrolot = itembo.DistroQuantity;
                    _casepackqty = itembo.CaseQuantity;
                    _charactercode = itembo.CharacterCode;
                    //Vendor Ref;

                    // HK / FC : 16-11-2009 : Capture Vat Code 
                    _vatcode = itembo.VatCode;
                    _sku = itembo.SKU;
                    _skuchk = itembo.SKUcheck;

                    _minPack = itembo.DistroQuantity;
                    _casepackqty = itembo.CaseQuantity;

                    // HK : CJ : 26-01-2010 : Fix Bug 311 :
                    // Expose seasoncode
                    _seasoncode = itembo.SeasonCode;

                    isvalid = true;
                }

                else
                {
                    itemlongdescription = "NO ITEM DESCRIPTION FOUND";
                    isvalid = false;

                }

                return bSuccess;


            }

            catch (Exception ex)
            {
                MessageBox.Show("Spice PO", ex.Message);
                isvalid = false;
                return false;

            }


        }

        public void RaiseItemorQtyChanged(int rowindex)
        {
            //ItemQtyChanged(itemquantity, cost, rowindex);
            ItemQtyChanged(itemquantity, _convertedcost, rowindex);
        }

        public bool ModifyOrderLines(PurchaseOrder _porder, DataTable dtOrderLines, short spicepoversionprevious)
        {
            Boolean bSuccessPoItem = false;
            Boolean bSuccessComponent = false;
            string sapp;
            short sequence1;

            DSSPPOIcls poline = new DSSPPOIcls(_porder.DbParamRef);

            // Write the Line
            bSuccessPoItem = poline.UpdatePOlines (_porder.Spiceponumber, dtOrderLines);

            if (bSuccessPoItem)
            {

                // HK : 14-12-2009 : Having a conversation with CJ just 
                // hilighted that by using existing logic, only one APP component
                // will ever get written to the database. So do a for loop
                // on the datatable to loop through item.

                for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
                {

                    // //////////////////////////////////////////////////////
                    // HK : 17-11-2009
                    // Instance of the class that writes components
                    DSSPPOA pocomponent = new DSSPPOA(_porder.DbParamRef);

                    // ////////////////////////////////////////////////////////////////////
                    // HK : 17-11-2009 : Write the POComponents
                    // ////////////////////////////////////////////////////////////////////
                    DataTable dtComponents = null;

                    sapp = _porder.lstpoLineItemDetails[i].APP1;
                    sequence1 = _porder.lstpoLineItemDetails[i].Sequence;

                    //if (APP1 == "Y")
                    if (sapp == "Y")
                    {
                        dtComponents = new DataTable();
                        dtComponents.Columns.Add("ComponentClass",      typeof(Int16));
                        dtComponents.Columns.Add("ComponentVendor",     typeof(Int32));
                        dtComponents.Columns.Add("ComponentStyle",      typeof(Int16));
                        dtComponents.Columns.Add("ComponentColour",     typeof(Int16));
                        dtComponents.Columns.Add("ComponentSize",       typeof(Int16));
                        dtComponents.Columns.Add("ComponentLongDesc",   typeof(String));
                        dtComponents.Columns.Add("ComponentQuantity",   typeof(Int16));
                        dtComponents.Columns.Add("ComponentCost",       typeof(Decimal));

                        foreach (POItemDetails poitem in _porder.lstpoLineItemDetails[i].pocomponents)
                        {

                            // ///////////////////////////////////////
                            // HK : 19-01-2010 : Fix Bug 276, 277, 283
                            // Send the ConvertedCost for the components and not the 
                            // Simple Vendort Cost i.e "Cost" column, attribute
                            dtComponents.Rows.Add(poitem.Classcode,
                                                    poitem.Vendorcode,
                                                    poitem.Stylecode,
                                                    poitem.Colorcode,
                                                    poitem.Itemsize,
                                                    poitem.Itemlongdescription,
                                                    poitem.Itemquantity,
                                                    poitem.ConvertedCost);

                            Debug.Print("Components:");
                            Debug.Print("==============================================================");
                            Debug.Print(_porder.Spiceponumber + "  " + DateTime.Now.ToString() + "  "
                                                             + Convert.ToString(poitem.Classcode) + "   "
                                                             + Convert.ToString(poitem.Vendorcode) + "   "
                                                             + Convert.ToString(poitem.Stylecode) + "   "
                                                             + Convert.ToString(poitem.Colorcode) + "   "
                                                             + Convert.ToString(poitem.Itemsize)  + "   "
                                                             + Convert.ToString(poitem.Itemquantity) + "   "
                                                             );

                        }


                        // HK : 11-12-2009 : Need a UpdateAPPComponents. Please ask CJ to write one.
                        bSuccessComponent = pocomponent.UpdateAPPcomponentCost(_porder.Spiceponumber, spicepoversionprevious, _porder.SpicePoVersion, sequence1, dtComponents);

                    }

                }

                _poindex++;

            }

            return (bSuccessPoItem || bSuccessComponent);

        }

        public bool CreateOrderLines(PurchaseOrder _porder, DataTable dtOrderLines)
        {

            Boolean bSuccessPoItem = false;
            Boolean bSuccessComponent = false;
            string sapp;
            short sequence1;

            DSSPPOIcls poline = new DSSPPOIcls(_porder.DbParamRef);

            // Write the Line
            bSuccessPoItem = poline.WritePOlines (dtOrderLines);

            if (bSuccessPoItem)
            {

                // HK : 14-12-2009 : Having a conversation with CJ just 
                // hilighted that by using existing logic, only one APP component
                // will ever get written to the database. So do a for loop
                // on the datatable to loop through item.

                for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
                {

                    // //////////////////////////////////////////////////////
                    // HK : 17-11-2009
                    // Instance of the class that writes components
                    DSSPPOA pocomponent = new DSSPPOA(_porder.DbParamRef);

                    // ////////////////////////////////////////////////////////////////////
                    // HK : 17-11-2009 : Write the POComponents
                    // ////////////////////////////////////////////////////////////////////
                    DataTable dtComponents = null;

                    sapp = _porder.lstpoLineItemDetails[i].APP1;
                    sequence1 = _porder.lstpoLineItemDetails[i].Sequence;

                    if (sapp == "Y")
                    {
                        dtComponents = new DataTable();
                        dtComponents.Columns.Add("ComponentClass",      typeof(Int16));
                        dtComponents.Columns.Add("ComponentVendor",     typeof(Int32));
                        dtComponents.Columns.Add("ComponentStyle",      typeof(Int16));
                        dtComponents.Columns.Add("ComponentColour",     typeof(Int16));
                        dtComponents.Columns.Add("ComponentSize",       typeof(Int16));
                        dtComponents.Columns.Add("ComponentLongDesc",   typeof(String));
                        dtComponents.Columns.Add("ComponentQuantity",   typeof(Int16));
                        dtComponents.Columns.Add("ComponentCost",       typeof(Decimal));

                        //foreach (POItemDetails poitem in pocomponents)
                        foreach (POItemDetails poitem in _porder.lstpoLineItemDetails[i].pocomponents)
                        {
                            // HK : 18-01-2010 : Instead of Cost send the ConvertedCost
                            // ///////////////////////////////////////
                            // HK : 19-01-2010 : Fix Bug 276, 277, 283
                            // Send the ConvertedCost for the components and not the 
                            // Simple Vendort Cost i.e "Cost" column, attribute
                            dtComponents.Rows.Add(poitem.Classcode,
                                                    poitem.Vendorcode,
                                                    poitem.Stylecode,
                                                    poitem.Colorcode,
                                                    poitem.Itemsize,
                                                    poitem.Itemlongdescription,
                                                    poitem.Itemquantity,
                                                    poitem.ConvertedCost);

                            Debug.Print("Components:");
                            Debug.Print("==============================================================");
                            Debug.Print(_porder.Spiceponumber + "  " + DateTime.Now.ToString() + "  "
                                                             + Convert.ToString(poitem.Classcode) + "   "
                                                             + Convert.ToString(poitem.Vendorcode) + "   "
                                                             + Convert.ToString(poitem.Stylecode) + "   "
                                                             + Convert.ToString(poitem.Colorcode) + "   "
                                                             + Convert.ToString(poitem.Itemsize)
                                                             );



                        }

                        bSuccessComponent = pocomponent.WriteAPPcomponents(_porder.Spiceponumber, _porder.SpicePoVersion, sequence1, dtComponents);

                    }

                }

                _poindex++;

            }

            return (bSuccessPoItem || bSuccessComponent);

        }

        public override string ToString()
        {
            return String.Format("Class ={1} Vendor  ={2} Style ={3} Color :{4} Size :{5}", Classname, Vendorcode, Stylecode, Colorcode, Itemsize);
            //return String.Format("{1}{2}{3}{4)");
        }
        
        #region IComparable Members
        public int CompareTo(object obj)
        {
            if (!(obj is POItemDetails))
            {
                throw new ArgumentException("Object provided is wrong type");
            }

            POItemDetails poitemdetails = (POItemDetails)obj;

            int cmpl = this.Classcode.CompareTo(poitemdetails.Classcode);
            int cmp2 = this.Vendorcode.CompareTo(poitemdetails.Vendorcode);
            int cmp3 = this.Stylecode.CompareTo(poitemdetails.Stylecode);
            int cmp4 = this.Colorcode.CompareTo(poitemdetails.Colorcode);
            int cmp5 = this.Itemsize.CompareTo(poitemdetails.Itemsize);

            // ClassCode in no the same so dont bother going any further.
            // Just say that the objects are different
            if (!(cmpl == 0))
            {
                return cmpl;
            }

            if (!(cmp2 == 0))
            {
                return cmp2;
            }

            if (!(cmp3 == 0))
            {
                return cmp3;
            }

            if (!(cmp4 == 0))
            {
                return cmp4;
            }

            if (!(cmp5 == 0))
            {
                return cmp5;
            }

            // At this stage ClassCode, Vendorcode, Stylecode, Colorcode and Itemsize 
            // are all found. So just return 0

            return (this.Classcode.CompareTo(poitemdetails.Classcode));
        }
        #endregion

        public override int GetHashCode()
        {
            return string.Concat(Classcode, Vendorcode, Stylecode, Colorcode, Itemsize).GetHashCode();
        }

    }


    public class POComponents : CollectionBase
    {

        /// <summary>
        /// Indexer for our collection
        /// </summary>
        /// <param name="idx">Index of the collection</param>
        /// <returns>POItemDetails at that index</returns>
        public POItemDetails this[int index]
        {
            get { return (POItemDetails) List[index]; }

            set { List[index] = value; }

        }

        public void Add(POItemDetails poitemdetails)
        {
            this.List.Add(poitemdetails);
        }

        public void Remove(POItemDetails poitemdetails)
        {
            this.List.Remove(poitemdetails);
        }

        public int IndexOf(POItemDetails poitemdetails)
        {
            return (this.List.IndexOf(poitemdetails));
        }

        public void Insert(int index, POItemDetails poitemdetails)
        {
            this.List.Insert(index, poitemdetails);

        }

        public bool Contains(POItemDetails poitemdetails)
        {
            // If value is not of type poitemdetails, this will return false.
            //return (this.List.Contains(poitemdetails));

            foreach (POItemDetails item in this.InnerList)
            {
                if (item.CompareTo(poitemdetails) == 0)
                {
                    // return true
                    return true;
                }
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is POItemDetails)
            {
                POItemDetails temp = (POItemDetails)obj;

                foreach (POItemDetails item in this.InnerList)
                {
                    if (temp.Classcode == item.Classcode &&
                        temp.Vendorcode == item.Vendorcode &&
                        temp.Stylecode == item.Stylecode &&
                        temp.Colorcode == item.Colorcode &&
                        temp.Itemsize == item.Itemsize)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                throw new ArgumentException("Object is not PO Item");
            }
            return false;
        }

    }

    

}