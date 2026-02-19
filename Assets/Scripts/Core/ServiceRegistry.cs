using System;
using System.Collections.Generic;

namespace EntropySyndicate.Core
{
    public class ServiceRegistry
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>(32);

        public void Register<T>(T instance) where T : class
        {
            _services[typeof(T)] = instance;
        }

        public T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out object value))
            {
                return value as T;
            }

            return null;
        }
    }
}
