using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LK.Helpers
{
    public class Grouping<K, T> : ObservableCollection<T>
    {
        public K DateFilter { get; private set; }

        public Grouping(K key, IEnumerable<T> items)
        {
            DateFilter = key;
            foreach (var item in items)
                this.Items.Add(item);
        }
    }
}
