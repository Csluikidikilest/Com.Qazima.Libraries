using System;

namespace Com.Qazima.Libraries.Server.Cache
{
  public struct CacheKeyValue<KeyType, ValueType>
  {
    public KeyType Key { get; private set; }

    public ValueType Value { get; set; }

    public DateTime Expires { get; set; }

    public CacheKeyValue(KeyType key, ValueType value)
      : this(key, value, DateTime.MaxValue)
    {
    }

    public CacheKeyValue(KeyType key, ValueType value, TimeSpan duration)
    {
      Key = key;
      Value = value;
      Expires = DateTime.Now.Add(duration);
    }

    public CacheKeyValue(KeyType key, ValueType value, DateTime expires)
    {
      Key = key;
      Value = value;
      Expires = expires;
    }
  }
}
