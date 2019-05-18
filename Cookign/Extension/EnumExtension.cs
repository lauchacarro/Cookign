using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Cookign.Extension
{
    internal static class EnumExtension
    {
        public static string GetDescription(this Enum element)
        {
            Type type = element.GetType();

            MemberInfo[] memberInfos = type.GetMember(element.ToString());

            if(!CookignHelpers.IsArrayEmply(memberInfos))
            {
                object[] attributes = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if(!CookignHelpers.IsArrayEmply(attributes))
                {
                    return ((DescriptionAttribute)attributes[0]).Description;
                }
            }

            return element.ToString();
        }
    }
}
