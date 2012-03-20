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
        private short _poindex = 0;

        #region Properties
        private bool isvalid;
        public bool IsValid
        {
            get { return isvalid; }
            set { isvalid = value; }
        }

        private Boolean isnewline;
        public Boolean IsNewLine
        {
            get { return isnewline; }
            set { isnewline = value; }
        }

        private int itemindex;
        public int Itemindex
        {
            get { return itemindex; }
            set { itemindex = value; }
        }

        private Int16 classcode;
        public Int16 ClassCode
        {
            get { return classcode; }
            set { classcode = value; }
        }

        private string classname;
        public string Classname
        {
            get { return classname; }
            set { classname = value; }
        }

        private int vendorcode;
        public int Vendorcode
        {
            get { return vendorcode; }
            set { vendorcode = value; }
        }

        private string vendordesc;
        public string Vendordesc
        {
            get { return vendordesc; }
            set { vendordesc = value; }
        }

        private short stylecode;
        public short Stylecode
        {
            get { return stylecode; }
            set { stylecode = value; }
        }

        private string stylename;
        public string Stylename
        {
            get { return stylename; }
            set { stylename = value; }
        }

        private short colorcode;
        public short Colorcode
        {
            get { return colorcode; }
            set { colorcode = value; }
        }

        private string colordesc;
        public string Colordesc
        {
            get { return colordesc; }
            set { colordesc = value; }
        }

        private string characterdesc;
        public string Characterdesc
        {
            get { return characterdesc; }
            set { characterdesc = value; }
        }

        private string seasondesc;
        public string SeasonDesc
        {
            get { return seasondesc; }
            set { seasondesc = value; }
        }

        private short itemsize;
        public short Itemsize
        {
            get { return itemsize; }
            set { itemsize = value; }
        }

        private string _sizename;
        public string Sizename
        {
            get { return _sizename; }
            set { _sizename = value; }
        }

        private decimal cost;
        public decimal Cost
        {
            get { return cost; }
            set { cost = value; }
        }

        private string _vendorstyle;
        public string Vendorstyle
        {
            get { return _vendorstyle; }
            set { _vendorstyle = value; }
        }

        private string _subclass;
        public string Subclass
        {
            get { return _subclass; }
            set { _subclass = value; }
        }

        private string _tickettype;
        public string Tickettype
        {
            get { return _tickettype; }
            set { _tickettype = value; }
        }

        private int _casepackqty;
        public int CasePackQty
        {
            get { return _casepackqty; }
            set { _casepackqty = value; }
        }

        private int _distroqty;
        public int DistroQty
        {
            get { return _distroqty; }
            set { _distroqty = value; }
        }

        private string _upc;
        public string UPC
        {
            get { return _upc; }
            set { _upc = value; }
        }

        private int _sku;
        public int Sku
        {
            get { return _sku; }
            set { _sku = value; }
        }

        private string itemlongdescription;
        public string Itemlongdescription
        {
            get { return itemlongdescription; }
            set { itemlongdescription = value; }
        }

        private decimal retailprice;
        public decimal Retailprice
        {
            get { return retailprice; }
            set { retailprice = value; }
        }

        private string itemshortdescription;
        public string Itemshortdescription
        {
            get { return itemshortdescription; }
            set { itemshortdescription = value; }
        }

        private string _packdescription;
        public string Packdescription
        {
            get { return _packdescription; }
            set { _packdescription = value; }
        }

        private string _charactercode;
        public string Charactercode
        {
            get { return _charactercode; }
            set { _charactercode = value; }
        }

        private int itemquantity;
        public int Itemquantity
        {
            get { return itemquantity; }
            set { itemquantity = value; }
        }

        private string app;
        public string APP1
        {
            get { return app; }
            set { app = value; }
        }

        private decimal height;
        public decimal Height
        {
            get { return height; }
            set { height = value; }
        }

        private decimal length;
        public decimal Length
        {
            get { return length; }
            set { length = value; }
        }

        private decimal width;
        public decimal Width
        {
            get { return width; }
            set { width = value; }
        }

        private decimal weight;
        public decimal Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        private string _vendorRef;
        public string VendorRef
        {
            get { return _vendorRef; }
            set { _vendorRef = value; }
        }

        private string _vatcode;
        public string VatCode
        {
            get { return _vatcode; }
            set { _vatcode = value; }
        }

        private short _skuchk;
        public short SkuChk
        {
            get { return _skuchk; }
            set { _skuchk = value; }
        }

        private Boolean _isduplicate;
        public bool IsDuplicate
        {
            get { return _isduplicate; }
            set { _isduplicate = value; }
        }

        private short _sequence;
        public short Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }

        private decimal _convertedcost;
        public decimal ConvertedCost
        {
            get { return _convertedcost; }
            set { _convertedcost = value; }
        }

        private decimal _landedcost;
        public decimal LandedCost
        {
            get { return _landedcost; }
            set { _landedcost = value; }
        }

        private string _seasoncode;
        public string SeasonCode
        {
            get { return _seasoncode; }
            set { _seasoncode = value; }
        }

        private List<APPcomponent> _component;
        public List<APPcomponent> Components
        {
            get { return _component; }
            set { _component = value; }
        }
        #endregion

        //public POcomponents pocomponents;


        #region Constructors
        public POItemDetails(int index)
        {
            itemindex = index;

            _component = new List<APPcomponent>();
        }

        public POItemDetails(short ClassCode, int VendorCode, short Style, short ColourCode, short Size, int index)
        {
            classcode = ClassCode;
            vendorcode = VendorCode;
            stylecode = Style;
            colorcode = ColourCode;
            itemsize = Size;
            itemindex = index;

            _component = new List<APPcomponent>();
        }
        #endregion

        public bool ItemLookup(ASNA.VisualRPG.Runtime.Database dbparamref, Disney.Menu.Users username, Disney.Menu.Environments penvironment, string defaultmarket)
        {
            bool bSuccess;
            try
            {
                ItemsBO.Items itembo = new Disney.Spice.ItemsBO.Items(dbparamref, username, penvironment);

                //Since we know the default market make use of this.
                bSuccess = itembo.GetItem(classcode, vendorcode, stylecode, colorcode, itemsize, defaultmarket);
                if (bSuccess)
                {
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
                    height = itembo.PackHeight;
                    length = itembo.PackLength;
                    width = itembo.PackWidth;
                    weight = itembo.PackWeight;
                    _packdescription = itembo.PackDescription;
                    _upc = itembo.UPC.ToString();
                    _tickettype = itembo.TicketType;
                    _subclass = itembo.SubClass;
                    _vendorstyle = itembo.VendorStyle;
                    _distroqty = itembo.DistroQuantity;
                    _casepackqty = itembo.CaseQuantity;
                    _charactercode = itembo.CharacterCode;
                    _vatcode = itembo.VatCode;
                    _sku = itembo.SKU;
                    _skuchk = itembo.SKUcheck;
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

        public bool ModifyOrderLines(PurchaseOrder _porder, DataTable dtOrderLines, short spicepoversionprevious)
        {
            Boolean bSuccessPoItem = false;
            Boolean bSuccessComponent = false;
            string sapp;
            short sequence1;

            DSSPPOIcls poline = new DSSPPOIcls(_porder.DbParamRef);

            // Write the Line
            bSuccessPoItem = poline.UpdatePOlines(_porder.SpicePOnumber, dtOrderLines);

            if (bSuccessPoItem)
            {
                // Having a conversation with CJ just 
                // hilighted that by using existing logic, only one APP component
                // will ever get written to the database. So do a for loop
                // on the datatable to loop through item.
                for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
                {
                    // Instance of the class that writes components
                    DSSPPOA pocomponent = new DSSPPOA(_porder.DbParamRef);

                    DataTable dtComponents = null;

                    sapp = _porder.lstpoLineItemDetails[i].APP1;
                    sequence1 = _porder.lstpoLineItemDetails[i].Sequence;

                    //if (APP1 == "Y")
                    if (sapp == "Y")
                    {
                        dtComponents = new DataTable();
                        dtComponents.Columns.Add("ComponentClass",   typeof(Int16));
                        dtComponents.Columns.Add("ComponentVendor",  typeof(Int32));
                        dtComponents.Columns.Add("ComponentStyle",   typeof(Int16));
                        dtComponents.Columns.Add("ComponentColour",  typeof(Int16));
                        dtComponents.Columns.Add("ComponentSize",    typeof(Int16));
                        dtComponents.Columns.Add("ComponentLongDesc",typeof(String));
                        dtComponents.Columns.Add("ComponentQuantity",typeof(Int16));
                        dtComponents.Columns.Add("ComponentCost",    typeof(Decimal));

                        foreach (APPcomponent component in _porder.lstpoLineItemDetails[i]._component)
                        {
                            // Send the ConvertedCost for the components and not the 
                            // Simple Vendor Cost i.e "Cost" column, attribute
                            dtComponents.Rows.Add(component.ComponentClass,
                                                  component.ComponentVendor,
                                                  component.ComponentStyle,
                                                  component.ComponentColour,
                                                  component.ComponentSize,
                                                  component.ItemDescription,
                                                  component.RatioQuantity,
                                                  component.Cost);
                        }

                        if (_porder.lstpoLineItemDetails[i].isnewline)
                        {
                            bSuccessComponent = pocomponent.WriteAPPcomponents(_porder.SpicePOnumber, _porder.SpicePOversion, sequence1, dtComponents);
                        }
                        else
                        {
                            bSuccessComponent = pocomponent.UpdateAPPcomponentCost(_porder.SpicePOnumber, spicepoversionprevious, _porder.SpicePOversion, sequence1, dtComponents);
                        }
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
            bSuccessPoItem = poline.WritePOlines(dtOrderLines);

            if (bSuccessPoItem)
            {
                for (int i = 0; i < _porder.lstpoLineItemDetails.Count; i++)
                {
                    // Instance of the class that writes components
                    DSSPPOA pocomponent = new DSSPPOA(_porder.DbParamRef);

                    DataTable dtComponents = null;

                    sapp = _porder.lstpoLineItemDetails[i].APP1;
                    sequence1 = _porder.lstpoLineItemDetails[i].Sequence;

                    if (sapp == "Y")
                    {
                        dtComponents = new DataTable();
                        dtComponents.Columns.Add("ComponentClass",    typeof(Int16));
                        dtComponents.Columns.Add("ComponentVendor",   typeof(Int32));
                        dtComponents.Columns.Add("ComponentStyle",    typeof(Int16));
                        dtComponents.Columns.Add("ComponentColour",   typeof(Int16));
                        dtComponents.Columns.Add("ComponentSize",     typeof(Int16));
                        dtComponents.Columns.Add("ComponentLongDesc", typeof(String));
                        dtComponents.Columns.Add("ComponentQuantity", typeof(Int16));
                        dtComponents.Columns.Add("ComponentCost",     typeof(Decimal));

                        foreach (APPcomponent component in _porder.lstpoLineItemDetails[i].Components)
                        {
                            // Instead of Cost send the ConvertedCost
                            // Send the ConvertedCost for the components and not the
                            // Simple Vendort Cost i.e "Cost" column, attribute
                            dtComponents.Rows.Add(component.ComponentClass,
                                                  component.ComponentVendor,
                                                  component.ComponentStyle,
                                                  component.ComponentColour,
                                                  component.ComponentSize,
                                                  component.ItemDescription,
                                                  component.RatioQuantity,
                                                  component.Cost);
                        }

                        bSuccessComponent = pocomponent.WriteAPPcomponents(_porder.SpicePOnumber, _porder.SpicePOversion, sequence1, dtComponents);
                    }
                }

                _poindex++;
            }

            return (bSuccessPoItem || bSuccessComponent);
        }

        public override string ToString()
        {
            return String.Format("Class ={1} Vendor  ={2} Style ={3} Color :{4} Size :{5}", Classname, Vendorcode, Stylecode, Colorcode, Itemsize);
        }
        
        #region IComparable Members
        public int CompareTo(object obj)
        {
            if (!(obj is POItemDetails))
            {
                throw new ArgumentException("Object provided is wrong type");
            }

            POItemDetails poitemdetails = (POItemDetails)obj;

            int cmpl = this.ClassCode.CompareTo(poitemdetails.ClassCode);
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

            return (this.ClassCode.CompareTo(poitemdetails.ClassCode));
        }
        #endregion

        public override int GetHashCode()
        {
            return string.Concat(ClassCode, Vendorcode, Stylecode, Colorcode, Itemsize).GetHashCode();
        }
    }
}