using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Cookign.Attribute
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class KeyNotFoundMessageAttribute : SpanishEnglishDescription
    {


        internal const string SpanishMessage = "No se encontró un valor para la propiedad '{0}' dentro de la sección '" + nameof(Cookign) + "' en el archivo de configuración.";
        internal const string EnglishMessage = "A value for the '{0}' property was not found inside the '" + nameof(Cookign) + "' section in the configuration file.";


        internal KeyNotFoundMessageAttribute(string key) : base(GetMessageByLenguage(SpanishMessage, EnglishMessage, key))
        {
           
        }

    }
}
