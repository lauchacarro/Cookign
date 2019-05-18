using Cookign.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Cookign.Message
{
    public enum ErrorMessagesEnum
    {
        [SectionNotFoundMessage(nameof(Cookign))]
        SectionNotFound,

        [KeyNotFoundMessage(nameof(CookingSettings.Issuer))]
        IssuerNotFound,

        [KeyNotFoundMessage(nameof(CookingSettings.Audience))]
        AudienceNotFound,

        [KeyNotFoundMessage(nameof(CookingSettings.SecretKey))]
        SecretKeyNotFound
    }
}
