using System.Collections.Generic;

namespace NitroNet.Veil.ViewEngine
{
    public interface IMemberFilterFactory
    {
        IEnumerable<Veil.ViewEngine.IMemberFilter> Create();
    }

    public class MemberFilterFactory : IMemberFilterFactory
    {
        public IEnumerable<Veil.ViewEngine.IMemberFilter> Create()
        {
            return new[] {new Veil.ViewEngine.DictionaryOwnPropertiesFilter()};
        }
    }
}