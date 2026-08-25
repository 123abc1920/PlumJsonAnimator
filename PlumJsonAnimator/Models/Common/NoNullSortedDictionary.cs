using System.Collections.Generic;

namespace PlumJsonAnimator.Models.Common;

public class NoNullSortedDictionary<TKey, TValue> : SortedDictionary<TKey, TValue>
    where TValue : class
{
    public new TValue this[TKey key]
    {
        get => base[key];
        set
        {
            if (value == null)
                base.Remove(key);
            else
                base[key] = value;
        }
    }

    public new void Add(TKey key, TValue value)
    {
        if (value != null)
            base.Add(key, value);
    }
}
