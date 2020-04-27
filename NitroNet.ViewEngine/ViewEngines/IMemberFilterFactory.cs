using System.Collections.Generic;

namespace NitroNet.ViewEngine.ViewEngines
{
    public interface IMemberFilterFactory
    {
        IEnumerable<IMemberFilter> Create();
    }

    public class MemberFilterFactory : IMemberFilterFactory
    {
        public IEnumerable<IMemberFilter> Create()
        {
            return new[] {new DictionaryOwnPropertiesFilter()};
        }
    }
}