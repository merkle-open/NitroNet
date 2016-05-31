using System;
using System.Collections.Generic;
using System.IO;

namespace Veil
{
    public class RenderingContext
    {
	    private readonly Dictionary<string, object> _data = new Dictionary<string, object>();
        public TextWriter Writer { get; private set; }

	    public RenderingContext(TextWriter writer, RenderingContext parentContext)
	    {
			Writer = writer;

			if (parentContext != null)
			{
				foreach (var dataEntry in parentContext.Data)
					this.Data.Add(dataEntry);
			}
	    }

        public RenderingContext(TextWriter writer) : this(writer, null)
        {
        }

        public T GetData<T>(string key, Func<T> defaultValueFactory) 
        {
            T result;
            if (!TryGetData(key, out result))
            {
                result = defaultValueFactory();
                this.Data.Add(key, result);
            }
            return result;
        }

        public bool TryGetData<T>(string key, out T obj)
        {
            obj = default(T);

            object raw;
            if (!this.Data.TryGetValue(key, out raw))
                return false;

            obj = (T)raw;
            return obj != null;
        }

		public IDictionary<string, object> Data { get { return _data; } }
    }
}