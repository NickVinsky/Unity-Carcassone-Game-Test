using System.Collections.Generic;
using System.Linq;
using Code.Game.Data;
using UnityEngine;

namespace Code.Game.Building {
    public class Construction {
        protected byte[] Top { get; set; }
        protected byte[] Right { get; set; }
        protected byte[] Bot { get; set; }
        protected byte[] Left { get; set; }

        protected List<PlayerColor> Owners;
        protected bool Finished { get; }

        private readonly Dictionary<Cell,byte[]> _nodes = new Dictionary<Cell, byte[]>();

        //private byte[] _nodes;

        //protected Construction(){ Init();}
        protected Construction(Cell cell, byte[] nodes) { _nodes.Add(cell, nodes); Init();}
        protected Construction(Cell cell, PlayerColor owner, byte[] nodes) { Owners.Add(owner); _nodes.Add(cell, nodes); Init();}

        public void Debugger(Area type) {
            foreach (var n in _nodes) {
                var s = n.Value.Aggregate("", (current, t) => current + t);
                Debug.Log(type + "[" + s + "] on (" + n.Key.X + ";" + n.Key.Y + ")");
            }
        }

        protected virtual void Init() {
            Top = new byte[] {0};
            Right = new byte[] {1};
            Bot = new byte[] {2};
            Left = new byte[] {3};
        }

        public bool Exist(Cell cell) {
            return _nodes.Any(n => Equals(n.Key, cell));
        }

        public byte[] Nodes(Cell cell) {
            return (from n in _nodes where Equals(n.Key, cell) select n.Value).FirstOrDefault();
        }

        public byte[] AttachableNodes(Cell cell) {
            return (from n in _nodes where Equals(n.Key, cell) select n.Value).FirstOrDefault();
        }

        public byte[] AttachedTo(int side, Cell cell) { return AttachedTo((Side) side, cell); }
        public byte[] AttachedTo(Side side, Cell cell) {
            var output = new List<byte>();
            byte[] pattern = null;
            switch (side) {
                case Side.Top: pattern = Bot; break;
                case Side.Right: pattern = Left; break;
                case Side.Bot: pattern = Top; break;
                case Side.Left: pattern = Right; break;
            }
            output.AddRange(from node in Nodes(cell) from n in pattern where n == node select node);
            var l = output.Count;
            if (l == 0) return null;
            var castedOut = new byte[l];
            for (var i = 0; i < l; i++) {
                castedOut[i] = output[i];
            }
            return castedOut;
        }
    }
}