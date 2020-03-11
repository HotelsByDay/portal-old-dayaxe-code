using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DayaxeDal.Extensions
{
    public class CustomResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop = base.CreateProperty(member, memberSerialization);

            if (prop.PropertyType.IsClass &&
                prop.PropertyType != typeof(string) && prop.PropertyType.Name.Contains("EntitySet"))
            {
                prop.Ignored = true;
                prop.ShouldSerialize = obj => false;
                prop.ShouldDeserialize = obj => false;
            }

            return prop;
        }
    }
}
