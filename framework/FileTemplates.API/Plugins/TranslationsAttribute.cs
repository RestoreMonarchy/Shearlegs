using System;
using System.Collections.Generic;
using System.Text;

namespace FileTemplates.API.Plugins
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class DefaultTranslationsAttribute : Attribute
    {
    }
}
