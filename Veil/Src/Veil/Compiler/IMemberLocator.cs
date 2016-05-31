using System;
using System.Reflection;

namespace Veil.Compiler
{
    public interface IMemberLocator
    {
        MemberInfo FindMember(Type modelType, string name, MemberTypes types);
    }
}