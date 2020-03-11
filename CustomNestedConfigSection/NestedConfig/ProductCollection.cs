using System;
using System.Configuration;

namespace CustomNestedConfigSections.NestedConfig
{
    public class ProductCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Product();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Product) element).Id;
        }

        protected override string ElementName
        {
            get
            {
                return "product";
            }
        }

        protected override bool IsElementName(string elementName)
        {
            return !String.IsNullOrEmpty(elementName) && elementName == "product";
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        public Product this[int index]
        {
            get
            {
                return base.BaseGet(index) as Product;
            }
        }

        public new Product this[string key]
        {
            get
            {
                return base.BaseGet(key) as Product;
            }
        }
    }
}