using System.Collections.Generic;

namespace Code.Game.FollowerSubs {
    public class FollowerLocation {
        private Area _type;

        // Towns || Roads |
        // | 0 | || | 0 | |
        // |3 1| || |3 1| |
        // | 2 | || | 2 | |
        //
        //     Fields
        //    | 0  1 |
        //    |7    2|
        //    |      |
        //    |6    3|
        //    | 5  4 |
        private byte[] _nodes;

        public FollowerLocation(Area type, List<byte> nodes) {
            _type = type;
            var nLen = nodes.Count;
            _nodes = new byte[nLen];
            for (int i = 0; i < nLen; i++) {
                _nodes[i] = nodes[i];
            }
        }

        public FollowerLocation(Area type) {
            _type = type;
        }

        public byte[] GetNodes() { return _nodes; }

        public bool IsBarrier() {
            return _type == Area.Road;
        }
    }
}