using System;
using System.Globalization;

namespace HelperFunctions
{
    public class StringHelper
    {
        public static string ToTitleCase(string str, bool removeUnderscore = false)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            if (removeUnderscore)
            {
                str = str.Replace('_', ' ');
            }
            return textInfo.ToTitleCase(str.ToLower());
        }
    }

    public class EnumHelper
    {
        public static TEnum GetEnum<TEnum>(string str) where TEnum : struct
        {
            TEnum returnEnum;
            if (!Enum.TryParse(str.ToUpper(), out returnEnum))
            {
                throw new Exception("No " + typeof(TEnum) + " enum value found for string: " + str);
            }
            return returnEnum;
        }

        public static int GetLength<T>() where T : struct
        {
            return Enum.GetNames(typeof(T)).Length;
        }

        public static T GetRandom<T>() where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("Generic type " + typeof(T).GetType() + " is not an Enum!");
            }
            return (T)(object)UnityEngine.Random.Range(0, GetLength<T>());
        }
    }
}

