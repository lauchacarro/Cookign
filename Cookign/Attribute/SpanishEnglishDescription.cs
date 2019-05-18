using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Cookign.Attribute
{
    internal abstract class SpanishEnglishDescription : DescriptionAttribute
    {
        internal enum LenguageEnum { ES, EN }



        public SpanishEnglishDescription(string message) : base(message)
        {

        }



        internal static string GetMessageByLenguage(string SpanishMessage, string EnglishMessage, params string[] args)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;

            string message;
            if (culture.TwoLetterISOLanguageName.ToUpper() == LenguageEnum.ES.ToString())
            {
                message = string.Format(SpanishMessage, args);
            }
            else
            {
                message = string.Format(EnglishMessage, args);
            }
            return message;
        }







    }




}
