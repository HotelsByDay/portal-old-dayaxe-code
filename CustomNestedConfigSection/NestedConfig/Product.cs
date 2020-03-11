using System.Configuration;

namespace CustomNestedConfigSections.NestedConfig
{
    public class Product : ConfigurationElement
    {
        [ConfigurationProperty("id", IsKey = true)]
        public string Id { get { return (string)this["id"]; } }

        [ConfigurationProperty("maxValue", IsKey = false)]
        public int MaxValue { get { return (int)this["maxValue"]; } }

        [ConfigurationProperty("increase", IsKey = false)]
        public int Increase { get { return (int)this["increase"]; } }

    }
}