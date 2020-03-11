using System.Configuration;

namespace CustomNestedConfigSections.NestedConfig
{
    public class ProductsConfigSection : ConfigurationSection
    {
        //If you replace "employeeCollection" with "" then you do not need "employeeCollection" element as a wrapper node over employee nodes in config file.
        [ConfigurationProperty("productCollection", IsDefaultCollection = true, IsKey = false, IsRequired = true)]
        public ProductCollection Members
        {
            get
            {
                return base["productCollection"] as ProductCollection;
            }

            set
            {
                base["productCollection"] = value;
            }
        }

    }
}