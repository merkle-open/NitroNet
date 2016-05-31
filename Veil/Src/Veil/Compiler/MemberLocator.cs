using System;
using System.Linq;
using System.Reflection;

namespace Veil.Compiler
{
    public class MemberLocator : IMemberLocator
    {
        public static readonly IMemberLocator Default = new MemberLocator();

        protected MemberLocator()
        {
        }

        public virtual MemberInfo FindMember(Type modelType, string name, MemberTypes types)
        {
            return modelType
                .FindMembers(types, BindingFlags.Instance | BindingFlags.Public, Type.FilterNameIgnoreCase, name)
                .FirstOrDefault(x => x.MemberType == MemberTypes.Property && name.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
        }

    }
}