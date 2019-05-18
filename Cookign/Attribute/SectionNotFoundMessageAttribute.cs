using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Cookign.Attribute
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class SectionNotFoundMessageAttribute : SpanishEnglishDescription
    {


        internal const string SpanishMessage = "No se encontró la sección '{0}' en el archivo de configuración.";
        internal const string EnglishMessage = "Section '{0}' was not found in the configuration file.";


        internal SectionNotFoundMessageAttribute(string key) : base(GetMessageByLenguage(SpanishMessage, EnglishMessage, key))
        {
           
        }


  

    }
}
