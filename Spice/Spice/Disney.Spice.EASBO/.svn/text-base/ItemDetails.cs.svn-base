using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Disney.Spice.EASBO
{
    public class ItemDetails
    {
        private iDB2Connection DB2connect;
        private DataTable itemMarkettbl;
        private DataTable itemtbl;
        private DataTable itemSizetbl;
        private DataTable Classtbl;
        private DataTable Colourtbl;
        private DataTable Sizetbl;

        private Int16 m_Class;
        private Int32 m_Vendor;
        private Int16 m_Style;
        private Int16 m_Colour;
        private Int16 m_Size;
        private String m_Description;
        private String m_CharacterCode;
        private String m_VendorStyle;
        private decimal m_VendorUPC;
        private String m_CharacterDescription;
        private decimal m_SKUNumber;
        private string m_ClassName;
        private string m_ColourName;
        private string m_SizeName;
        private string m_CasePackType;
        private string m_CasePackTypeDes;
        private decimal m_CasePackQty;
        private decimal m_DistroInnerQty;
        private decimal m_Height;
        private decimal m_Width;
        private decimal m_Length;
        private decimal m_Weight;
        private string m_Market;
        private string m_ItemVatCode;


        public ItemDetails(iDB2Connection DB2connect)
        {
            this.DB2connect = DB2connect;
        }

        public Int16 Class
        {
            get { return m_Class; }
            set { m_Class = value; }        
        }

        public Int32 Vendor
        {
            get { return m_Vendor; }
            set { m_Vendor = value; }
        }

        public Int16 Style
        {
            get { return m_Style; }
            set { m_Style = value; }
        }

        public Int16 Colour
        {
            get { return m_Colour; }
            set { m_Colour = value; }
        }

        public Int16 Size   
        {
            get { return m_Size; }
            set { m_Size = value; }
        }

        public String Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        public String CharacterCode
        {
            get { return m_CharacterCode; }
            set { m_CharacterCode = value; }
        }

        public String VendorStyle
        {
            get { return m_VendorStyle; }
            set { m_VendorStyle = value; }
        }

        public decimal VendorUPC
        {
            get { return m_VendorUPC; }
            set { m_VendorUPC = value; }
        }

        public String CharacterDescription
        {
            get { return m_CharacterDescription; }
            set { m_CharacterDescription = value; }        
        }

        public decimal SKUNumber
        {
            get { return m_SKUNumber; }
            set { m_SKUNumber = value; }
        }

        public string ClassName
        {
            get { return m_ClassName; }
            set { m_ClassName = value; }
        }

        public string ColourName
        {
            get { return m_ColourName; }
            set { m_ColourName = value; }
        }

        public string SizeName
        {
            get { return m_SizeName; }
            set { m_SizeName = value; }
        }

        public string CasePackType
        {
            get { return m_CasePackType; }
            set { m_CasePackType = value; }
        }

        public string CasePackTypeDes
        {
            get { return m_CasePackTypeDes; }
            set { m_CasePackTypeDes = value; }
        }

        public decimal CasePackQty
        {
            get { return m_CasePackQty; }
            set { m_CasePackQty = value; }
        }

        public decimal DistroInnerQty
        {
            get { return m_DistroInnerQty; }
            set { m_DistroInnerQty = value; }
        }

        public decimal Height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }

        public decimal Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }

        public decimal Length
        {
            get { return m_Length; }
            set { m_Length = value; }
        }

        public decimal Weight
        {
            get { return m_Weight; }
            set { m_Weight = value; }
        }

        public string Market
        {
            get { return m_Market; }
            set { m_Market = value; }
        }
                
        public string ItemVatCode
        {
            get { return m_ItemVatCode; }
            set { m_ItemVatCode = value; }
        }

                        
        public Boolean GetItemDetails(Int16 Class, Int32 Vendor, Int16 Style, Int16 Colour, Int16 Size, string Market)
        {
            if (itemMarkettbl == null)
            {
                DAL.ItemMarket itemmarket = new DAL.ItemMarket(DB2connect);
                itemMarkettbl = itemmarket.GetItemMarketDetails(Class, Vendor, Style, Colour, Size, Market);
            }

            if (itemtbl == null)
            {
                DAL.ItemStyle itemstyle = new DAL.ItemStyle(DB2connect);
                itemtbl = itemstyle.GetItemStyleDetails(Class, Vendor, Style, Colour);
            }

            if (itemSizetbl == null)
            {
                DAL.ItemSize itemsize = new DAL.ItemSize(DB2connect);
                itemSizetbl = itemsize.GetItemSizeDetails(Class, Vendor, Style, Colour, Size);
            }

            
            if (Classtbl == null)
            {
                DAL.ClassDetails classDetails = new DAL.ClassDetails(DB2connect);
                Classtbl = classDetails.GetClassDetails(Class);
            }

            if (Colourtbl == null)
            {
                DAL.ColourDetails colourDetails = new DAL.ColourDetails(DB2connect);
                Colourtbl = colourDetails.GetColourDetails(Colour);
            }

            if (Sizetbl == null)
            {
                DAL.SizeDetails sizeDetails = new DAL.SizeDetails(DB2connect);
                Sizetbl = sizeDetails.GetSizeDetails(Size);
            }

            
            if (itemtbl != null) 
            {
                m_Class = Class;
                m_Vendor = Vendor;
                m_Style = Style;
                m_Colour = Colour;
                m_Description = (String)itemtbl.Rows[0]["ISADES"];
                m_CharacterCode = (String)itemtbl.Rows[0]["ISACHR"];
                m_VendorStyle = (String)itemtbl.Rows[0]["ISAVST"];
                m_ItemVatCode = (String)itemMarkettbl.Rows[0]["IMIVAT"];
                m_VendorUPC = (Decimal)itemMarkettbl.Rows[0]["IMIVUP"];

                if (m_VendorUPC == 0)
                { m_VendorUPC = (Decimal)itemSizetbl.Rows[0]["IIAVUP"]; }

                if (m_VendorUPC == 0)
                { m_VendorUPC = (Decimal)itemSizetbl.Rows[0]["IIADUP"]; }

                if (m_VendorUPC == 0)
                { m_VendorUPC = (Decimal)itemtbl.Rows[0]["ISAVUP"]; }

                m_SKUNumber = (Decimal)itemSizetbl.Rows[0]["IIASKU"];
                m_ClassName = (String)Classtbl.Rows[0]["CLNM"];
                m_ColourName = (String)Colourtbl.Rows[0]["CLRN"];
                m_SizeName = (String)Sizetbl.Rows[0]["SNAM"];
                m_CasePackType = (String)itemSizetbl.Rows[0]["IIACPT"];
                m_CasePackTypeDes = (String)itemSizetbl.Rows[0]["CADES"];
                m_CasePackQty = (Decimal)itemSizetbl.Rows[0]["IIAMIN"];
                m_DistroInnerQty = (Decimal)itemSizetbl.Rows[0]["IIAMLT"];
                m_Height = (Decimal)itemSizetbl.Rows[0]["IIACHT"];
                m_Width = (Decimal)itemSizetbl.Rows[0]["IIACWI"];
                m_Length = (Decimal)itemSizetbl.Rows[0]["IIACLN"];
                m_Weight = (Decimal)itemSizetbl.Rows[0]["IIACWG"];
                m_CharacterDescription = (String)itemtbl.Rows[0]["CHDESC"];

                return true;
            }
            else return false;
          }
       }
}
