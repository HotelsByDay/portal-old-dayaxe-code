using System;
using System.Collections.Generic;

namespace DayaxeDal.Compare
{
    public class ProductComparer : IEqualityComparer<Products>
    {

        public bool Equals(Products x, Products y)
        {
            //Check whether the objects are the same object. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether the products' properties are equal. 
            return x != null && y != null && x.ProductId.Equals(y.ProductId);
        }

        public int GetHashCode(Products obj)
        {
            //Get hash code for the Code field. 
            int hashProductCode = obj.ProductId.GetHashCode();

            //Calculate the hash code for the product. 
            return hashProductCode;
        }
    }
}
