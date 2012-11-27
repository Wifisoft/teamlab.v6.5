
namespace System
{
    public static class EnumExtension
    {
        public static T TryParseEnum<T>(this Type enumType, string value, T defaultValue) where T : struct
        {
            bool isDefault;
            return TryParseEnum<T>(enumType, value, defaultValue, out isDefault);            
        }

        public static T TryParseEnum<T>(this Type enumType, string value, T defaultValue, out bool isDefault) where T : struct
        {
            isDefault = false;
            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch
            {
                isDefault = true;
                return defaultValue;
            }
        }
    }
}
