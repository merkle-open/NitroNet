using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Veil.Compiler;

namespace NitroNet.Veil.ViewEngine
{
    public class FilteredMemberLocator : MemberLocator
    {
        private readonly IEnumerable<IMemberFilter> _filters;
      
        public FilteredMemberLocator(): this(new MemberFilterFactory())
        {

        }

        public FilteredMemberLocator(IMemberFilterFactory memberFilterFactory)
        {
            _filters = memberFilterFactory.Create();
        }

        public override MemberInfo FindMember(Type modelType, string name, MemberTypes types)
        {
            return _filters.Where(filter => filter.IsTypeSupported(modelType)).All(filter => filter.IsValid(name))
                ? base.FindMember(modelType, name, types)
                : null;
        }
    }
}