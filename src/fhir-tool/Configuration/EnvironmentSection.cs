using System.Configuration;
using System.Linq;

namespace FhirTool.Configuration
{
    public class EnvironmentSection : ConfigurationSection
    {
        [ConfigurationProperty("environments", IsDefaultCollection = false)]
        public EnvironmentElementCollection Items
        {
            get
            {
                return ((EnvironmentElementCollection)base["environments"]);
            }
        }
    }

    public class EnvironmentElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new EnvironmentElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EnvironmentElement)element).Name;
        }

        public ConfigurationElement this[int index]
        {
            get
            {
                return BaseGet(index);
            }
        }

        public ConfigurationElement this[object key]
        {
            get
            {
                return BaseGet(key);
            }
        }

        public bool Exists(object key)
        {
            object[] keys = BaseGetAllKeys();
            return keys.Any(k => k.Equals(key));
        }
    }

    public class EnvironmentElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("proxyBaseUrl", IsRequired = false)]
        public string ProxyBaseUrl
        {
            get { return (string)base["proxyBaseUrl"]; }
            set { base["proxyBaseUrl"] = value; }
        }

        [ConfigurationProperty("fhirBaseUrl", IsRequired = false)]
        public string FhirBaseUrl
        {
            get { return (string)base["fhirBaseUrl"]; }
            set { base["fhirBaseUrl"] = value; }
        }
    }
}
