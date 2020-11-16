using System;
using System.Collections.Generic;
using System.Linq;

namespace NitroNet.Veil.ViewEngine
{
    public class DictionaryOwnPropertiesFilter : IMemberFilter
    {
        private static readonly HashSet<string> ownProperties =
            new HashSet<string>(typeof(Dictionary<string, object>).GetProperties().Select(info => info.Name.ToLowerInvariant()));
        public bool IsTypeSupported(Type modelType)
        {
            return modelType == typeof(Dictionary<string, object>);
        }

        public bool IsValid(string memberName)
        {
            return !ownProperties.Contains(memberName.ToLowerInvariant());
        }
    }

    public interface IMemberFilter
    {
        bool IsTypeSupported(Type modelType);
        bool IsValid(string memberName);
    }
}
