using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Qazima.Libraries.Server.Cache
{
  public class CacheManager : CacheManager<string, object> { }

  public class CacheManager<KeyType, ValueType>
  {
    protected List<CacheKeyValue<KeyType, ValueType>> _innerList { get; set; }

    public CacheManager()
    {
      _innerList = new List<CacheKeyValue<KeyType, ValueType>>();
    }

    public CacheKeyValue<KeyType, ValueType> this[KeyType key]
    {
      get
      {
        Flush();
        return _innerList.FirstOrDefault(inner => inner.Key.Equals(key));
      }
    }

    public int Count
    {
      get
      {
        return _innerList.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public void Add(CacheKeyValue<KeyType, ValueType> item)
    {
      if (!_innerList.Any(inner => inner.Key.Equals(item.Key)))
      {
        _innerList.Add(item);
        Flush();
      }
    }

    public void Add(KeyType key, ValueType value)
    {
      Add(new CacheKeyValue<KeyType, ValueType>(key, value));
    }

    public void Add(KeyType key, ValueType value, DateTime expires)
    {
      Add(new CacheKeyValue<KeyType, ValueType>(key, value, expires));
    }

    public void Add(KeyType key, ValueType value, TimeSpan duration)
    {
      Add(new CacheKeyValue<KeyType, ValueType>(key, value, duration));
    }

    public void Clear()
    {
      _innerList.Clear();
    }

    public bool ContainsKey(string Key)
    {
      return _innerList.Any(inner => inner.Key.Equals(Key));
    }

    public void CopyTo(CacheKeyValue<KeyType, ValueType>[] array, int arrayIndex)
    {
      Flush();
      _innerList.CopyTo(array, arrayIndex);
    }

    public IEnumerator<CacheKeyValue<KeyType, ValueType>> GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    public bool Remove(CacheKeyValue<KeyType, ValueType> item)
    {
      Flush();
      return _innerList.Remove(item);
    }

    public bool Remove(string key)
    {
      Flush();
      try
      {
        return _innerList.RemoveAll(inner => inner.Key.Equals(key)) > 0;
      }
      catch
      {
        return false;
      }
    }

    protected void Flush()
    {
      _innerList.RemoveAll(inner => inner.Expires > DateTime.Now);
    }
  }
}
