using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Disney.Spice.POBO
{
    public class POcomponents : CollectionBase
    {
        public POcomponents this[int index]
        {
            get { return (POcomponents)List[index]; }
            set { List[index] = value; }
        }

        public void Add(APPcomponent component)
        {
            this.List.Add(component);
        }

        public void Remove(APPcomponent component)
        {
            this.List.Remove(component);
        }

        public int IndexOf(APPcomponent component)
        {
            return (this.List.IndexOf(component));
        }

        public void Insert(int index, APPcomponent component)
        {
            this.List.Insert(index, component);
        }

        public bool Contains(APPcomponent component)
        {
            foreach (APPcomponent item in this.InnerList)
            {
                if (item.CompareTo(component) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is APPcomponent)
            {
                APPcomponent temp = (APPcomponent)obj;

                foreach (APPcomponent item in this.InnerList)
                {
                    if (temp.ComponentClass  == item.ComponentClass  &&
                        temp.ComponentVendor == item.ComponentVendor &&
                        temp.ComponentStyle  == item.ComponentStyle  &&
                        temp.ComponentColour == item.ComponentColour &&
                        temp.ComponentSize   == item.ComponentSize)
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

        public decimal GetTotalCost(Int16 orderquantity)
        {
            decimal total = 0;
            foreach (APPcomponent item in this.InnerList)
            {
                total += item.Cost * item.RatioQuantity;
            }

            return total;
        }
    }
}