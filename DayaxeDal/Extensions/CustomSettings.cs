using Newtonsoft.Json;

namespace DayaxeDal.Extensions
{
    public static class CustomSettings
    {
        public static JsonSerializerSettings SerializerSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new CustomResolver(),
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None,
                DateFormatString = "yyyy/MM/dd"
            };

            return settings;
        }

        public static JsonSerializerSettings SerializerSettingsWithFullDateTime()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new CustomResolver(),
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };

            return settings;
        }
    }
}
