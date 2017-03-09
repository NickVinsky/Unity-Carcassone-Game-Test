using System.Collections.Generic;
using UnityEngine;

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
        private bool _coatOfArms;

        private Vector2 _meeplePos;
        private GameObject _sprite;

        public FollowerLocation(Area type, List<byte> nodes, bool coatOfArms, Vector2 meeplePos) {
            _type = type;
            _coatOfArms = coatOfArms;
            _meeplePos = meeplePos;
            var nLen = nodes.Count;
            _nodes = new byte[nLen];
            for (int i = 0; i < nLen; i++) {
                _nodes[i] = nodes[i];
            }
        }

        public FollowerLocation(Area type, Vector2 meeplePos) {
            _type = type;
            _coatOfArms = false;
            _meeplePos = meeplePos;
        }

        public byte[] GetNodes() { return _nodes; }

        public bool IsBarrier() {
            return _type == Area.Road;
        }

        public void Show(GameObject o, sbyte rotates) {
            _sprite = new GameObject();
            _sprite.name = _type + "(" +_meeplePos.x + ";" + _meeplePos.y + ")";
            _sprite.transform.SetParent(o.transform);
            _sprite.transform.localScale = new Vector3(0.08f, 0.08f, 0);
            _sprite.transform.localPosition = _meeplePos;
            _sprite.AddComponent<SpriteRenderer>();
            _sprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("3dMeeple");
            _sprite.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }
    }
}