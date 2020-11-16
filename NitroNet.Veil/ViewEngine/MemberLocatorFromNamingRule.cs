using System;
using System.Reflection;
using NitroNet.ViewEngine;

namespace NitroNet.Veil.ViewEngine
{
    [Obsolete("will be removed as soon final switch to handlebarsnet is completed")]
    public class MemberLocatorFromNamingRule : FilteredMemberLocator
    {
        private readonly INamingRule _namingRule;

        public MemberLocatorFromNamingRule(INamingRule namingRule)
        {
            _namingRule = namingRule;
        }

        public override MemberInfo FindMember(Type modelType, string name, MemberTypes types)
        {
            return base.FindMember(modelType, _namingRule.GetPropertyName(name), types);
        }
    }
}