using System.Collections.Generic;
using Code.Game.Data;

namespace Code.Game.Building {
    public class Construction {
        public int ID { get; }
        protected int TilesMerged;
        protected List<PlayerColor> Owners;
        protected bool Finished { get; }

        protected Construction(int id) {
            ID = id;
            Finished = false;
            Owners = new List<PlayerColor>();
            TilesMerged = 0;
        }

        /*public void Debugger(Area type) {
            foreach (var n in _nodes) {
                var s = n.Value.Aggregate("", (current, t) => current + t);
                Debug.Log(type + "[" + s + "] on (" + n.Key.X + ";" + n.Key.Y + ")");
            }
        }*/
    }
}