using System;
using System.Collections.Generic;

namespace DayaxeDal.Compare
{
    public class SubscriptionComparer : IEqualityComparer<Subscriptions>
    {

        public bool Equals(Subscriptions x, Subscriptions y)
        {
            //Check whether the objects are the same object. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether the products' properties are equal. 
            return x != null && y != null && x.Id.Equals(y.Id);
        }

        public int GetHashCode(Subscriptions obj)
        {
            //Get hash code for the Code field. 
            int hashProductCode = obj.Id.GetHashCode();

            //Calculate the hash code for the product. 
            return hashProductCode;
        }
    }
}
