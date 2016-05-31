using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Veil
{
    internal static class DelegateBuilder
    {
        private static readonly MethodInfo GetValueFromDictionaryMethod = typeof(DelegateBuilder).GetMethod("GetValueFromDictionary");
        private static readonly ConcurrentDictionary<Type, MethodInfo> GetValueFromDictionaryMethodDictionary = new ConcurrentDictionary<Type, MethodInfo>();

        public static Func<object, object> FunctionCall(Type modelType, MethodInfo function)
        {
            var model = Expression.Parameter(typeof(object));
            var castModel = Expression.Convert(model, modelType);
            var call = Expression.Call(castModel, function);
            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(call, typeof(object)),
                model
            ).Compile();
        }

        public static Func<object, object> Property(Type modelType, PropertyInfo property)
        {
            var model = Expression.Parameter(typeof(object));
            var castModel = Expression.Convert(model, modelType);
            var call = Expression.Property(castModel, property);
            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(call, typeof(object)),
                model
            ).Compile();
        }

        public static Func<object, object> Field(Type modelType, FieldInfo field)
        {
            var model = Expression.Parameter(typeof(object));
            var castModel = Expression.Convert(model, modelType);
            var call = Expression.Field(castModel, field);
            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(call, typeof(object)),
                model
            ).Compile();
        }

        public static Func<object, object> Dictionary(Type modelType, string key)
        {
            var model = Expression.Parameter(typeof(object));
            var castModel = Expression.Convert(model, modelType);

            var itemType = modelType.GetGenericArguments()[1];
            var method = GetValueFromDictionaryMethodDictionary
                .GetOrAdd(itemType, type => GetValueFromDictionaryMethod.MakeGenericMethod(type));

            var call = Expression.Call(method, castModel, Expression.Constant(key));

            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(call, typeof(object)),
                model
            ).Compile();
        }

        public static T GetValueFromDictionary<T>(IDictionary<string, T> dict, string key)
        {
            T value;
            if (!dict.TryGetValue(key, out value))
                return default(T);

            return value;
        } 
    }
}