using System;
using System.Collections.Generic;

namespace Simplify.ViewModel
{
    public class Bag
    {
        Dictionary<string,object> _objectsDictionary = new Dictionary<string, object>();

        public T GetObject<T>(string objectName)
        {
            if (_objectsDictionary.ContainsKey(objectName))
            {
                var obj = _objectsDictionary[objectName];
                if (obj.GetType() == typeof(T))
                {
                    return (T) obj;
                }
            }
            throw new Exception("Object Not found");
        }

        public void RemoveObject<T>(string objectName)
        {
            T t = GetObject<T>(objectName);
            _objectsDictionary.Remove(objectName);
        }

        public void AddObject<T>(string objectName, T obj)
        {
            _objectsDictionary.Add(objectName, obj);
        }
    }
}