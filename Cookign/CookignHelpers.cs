using System;
using System.Collections.Generic;
using System.Text;

namespace Cookign
{
    internal class CookignHelpers
    {
        public static bool IsArrayEmply(object[] array)
        {
            return array == null || array.Length == 0;
        }
    }
}
