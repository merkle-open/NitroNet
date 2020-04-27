using System;
using System.Reflection;

namespace Veil.Compiler
{
    [Obsolete("will be removed as soon final switch to handlebarsnet is completed")]
    public interface IMemberLocator
    {
        MemberInfo FindMember(Type modelType, string name, MemberTypes types);
    }
}