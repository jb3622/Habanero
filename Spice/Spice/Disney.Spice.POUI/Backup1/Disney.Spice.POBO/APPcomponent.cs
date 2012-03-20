using System;
using System.Collections.Generic;
using System.Text;

namespace Disney.Spice.POBO
{
    public class APPcomponent
    {
        public APPcomponent()
        {
        }

        private Int16 _componentClass;
        public Int16 ComponentClass
        {
            get
            {
                return _componentClass;
            }

            set
            {
                _componentClass = value;
            }
        }

        private Int32 _componentVendor;
        public Int32 ComponentVendor
        {
            get
            {
                return _componentVendor;
            }

            set
            {
                _componentVendor = value;
            }
        }

        private Int16 _componentStyle;
        public Int16 ComponentStyle
        {
            get
            {
                return _componentStyle;
            }

            set
            {
                _componentStyle = value;
            }
        }

        private Int16 _componentColour;
        public Int16 ComponentColour
        {
            get
            {
                return _componentColour;
            }

            set
            {
                _componentColour = value;
            }
        }

        private Int16 _componentSize;
        public Int16 ComponentSize
        {
            get
            {
                return _componentSize;
            }

            set
            {
                _componentSize = value;
            }
        }

        private Int16 _RatioQuantity;
        public Int16 RatioQuantity
        {
            get
            {
                return _RatioQuantity;
            }

            set
            {
                _RatioQuantity = value;
            }
        }

        private Decimal _cost;
        public Decimal Cost
        {
            get
            {
                return _cost;
            }

            set
            {
                _cost = value;
            }
        }

        private Decimal _retail;
        public Decimal Retail
        {
            get
            {
                return _retail;
            }

            set
            {
                _retail = value;
            }
        }

        private String _itemDescription;
        public String ItemDescription
        {
            get
            {
                return _itemDescription;
            }
            set
            {
                _itemDescription = value;
            }
        }

        public int CompareTo(object obj)
        {
            if (!(obj is APPcomponent))
            {
                throw new ArgumentException("Object provided is wrong type");
            }

            APPcomponent component = (APPcomponent)obj;

            int cmpl = this.ComponentClass.CompareTo(component.ComponentClass);
            int cmp2 = this.ComponentVendor.CompareTo(component.ComponentVendor);
            int cmp3 = this.ComponentStyle.CompareTo(component.ComponentStyle);
            int cmp4 = this.ComponentColour.CompareTo(component.ComponentColour);
            int cmp5 = this.ComponentSize.CompareTo(component.ComponentSize);

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

            return 0;
        }
    }
}