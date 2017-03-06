using System;
using System.Collections.Generic;

namespace Code {
    public class Deck<T> : List<T> {

        public event EventHandler OnAddOrRemove;

        public new void Add(T item) {
            base.Add(item);
            OnAddOrRemove?.Invoke(this, null);
        }

        public new void RemoveAt(int index) {
            base.RemoveAt(index);
            OnAddOrRemove?.Invoke(this, null);
        }
    }
}