using System.Collections.Generic;
using System.Linq;
using Code.Game.Building;
using Code.Game.Data;
using Code.Network;
using UnityEngine;
using static Code.Network.PlayerSync;
using Object = UnityEngine.Object;

namespace Code.Game.FollowerSubs {
    public class FollowerLocation {
        private byte _id;
        private Area _type;
        public Area Type {get { return _type; }}
        public int Link { get; set; }

        private PlayerColor _owner = PlayerColor.NotPicked;

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
        public bool Filled { get; }
        private bool _coatOfArms;

        public bool Indexed { get; set; }

        public bool PosFree { get; set; }

        private Vector2 _meeplePos;
        private GameObject _sprite;

        public TileInfo Parent { get; }

        public FollowerLocation(TileInfo parent, byte id, Area type, List<byte> nodes, bool coatOfArms, Vector2 meeplePos) {
            Parent = parent;
            _id = id;
            _type = type;
            _coatOfArms = coatOfArms;
            _meeplePos = meeplePos;
            var nLen = nodes.Count;
            _nodes = new byte[nLen];
            for (int i = 0; i < nLen; i++) {
                _nodes[i] = nodes[i];
            }
            Link = -1;
            PosFree = true;
            if (_type == Area.Field && Contains(new byte[] {0, 1, 2, 3, 4, 5, 6, 7})) {
                Filled = true;
            } else {
                Filled = false;
            }
        }

        public FollowerLocation(TileInfo parent, byte id, Area type, Vector2 meeplePos) {
            Parent = parent;
            _id = id;
            _type = type;
            _coatOfArms = false;
            _meeplePos = meeplePos;
            Link = -1;
            PosFree = true;
            Filled = false;
        }

        public bool Contains(byte[] pattern) {
            //Debug.Log("## _nodes[" + Builder.ArrayToString(_nodes) + "]/ ##pattern[" + Builder.ArrayToString(pattern) + "]");
            foreach (var p in pattern) {
                bool founded = _nodes.Any(n => n == p);
                if (!founded) return false;
            }
            //Debug.Log("^true^");
            return true;
        }

        public bool ContainsAnyOf(byte[] pattern) {
            return _nodes.Any(n => pattern.Any(p => n == p));
        }

        public bool ContainsOnly(byte[] pattern) {
            foreach (var n in _nodes) {
                bool founded = pattern.Any(p => n == p);
                if (!founded) return false;
            }
            return true;
        }

        public bool Conform(byte[] pattern) {
            foreach (var n in _nodes) {
                bool founded = pattern.Any(p => n == p);
                if (!founded) return false;
            }
            return true;
        }

        public byte[] GetNodes() { return _nodes; }
        public bool CompareID(byte id){ return id == _id;}

        public bool IsBarrier() {
            return _type == Area.Road;
        }

        private byte Trim(byte node, byte rotate) {
            var rNode = node;
            byte cap, add;
            if (_type != Area.Field) {
                if (_type == Area.Road || _type == Area.City) {
                    cap = 3;
                    add = 1;
                } else {
                    cap = 0;
                    add = 0;
                }
            } else {
                cap = 7;
                add = 2;
            }
            if (cap == 0) return node;
            rNode += (byte) (add * rotate);
            while (rNode > cap) {
                rNode -= (byte) (cap + 1);
            }
            //Debug.Log("r" + rotate + ": " + node + " => " + rNode + " " + _type);
            return rNode;
        }

        public void Rotate(byte rotate) {
            if (_nodes == null) return;
            var l = _nodes.Length;
            var rNodes = new byte[l];
            for (var i = 0; i < l; i++) {
                rNodes[i] = Trim(_nodes[i], rotate);
            }
            _nodes = rNodes;
        }

        public PlayerColor GetOwner() { return _owner; }

        public void SetOwner(GameObject o, PlayerColor owner) {
            PosFree = false;
            _owner = owner;
            Builder.SetOwner(this);
            SpriteInit(o);
            _sprite.GetComponent<SpriteRenderer>().color = Net.Color(_owner);
        }

        public void SetOwner() {
            PosFree = false;
            _owner = PlayerInfo.Color;
            Builder.SetOwner(this);
            if (_sprite.GetComponent<Rigidbody2D>() != null) {Object.Destroy(_sprite.GetComponent<Rigidbody2D>());}
            if (_sprite.GetComponent<BoxCollider2D>() != null) {Object.Destroy(_sprite.GetComponent<BoxCollider2D>());}
            //_sprite.GetComponent<BoxCollider2D>().enabled = false;
            //_sprite.GetComponent<FollowerHook>().enabled = false;
            _sprite.GetComponent<SpriteRenderer>().color = Net.Color(_owner);
            ScoreCalc.ApplyFollower();
            MainGame.ChangeGameStage(GameStage.Finish);
            if (Net.Game.IsOnline()) {
                var name = _sprite.transform.parent.gameObject.name;
                Net.Client.SendFollower(_owner, _id, name);
            }
        }

        public void Show(GameObject o, sbyte rotates) {
            if (!PosFree) return;
            SpriteInit(o);
            _sprite.AddComponent<BoxCollider2D>();
            _sprite.GetComponent<BoxCollider2D>().size = new Vector2(3f, 3f);
            _sprite.AddComponent<Rigidbody2D>();
            _sprite.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            _sprite.AddComponent<FollowerHook>();
            _sprite.GetComponent<FollowerHook>().Id = _id;
        }

        private void SpriteInit(GameObject o) {
            _sprite = new GameObject {name = _type + "(" + _meeplePos.x + ";" + _meeplePos.y + ")"};
            _sprite.transform.SetParent(o.transform);
            _sprite.transform.localScale = new Vector3(0.08f, 0.08f, 0);
            _sprite.transform.localPosition = _meeplePos;
            _sprite.AddComponent<SpriteRenderer>();
            _sprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("3dMeeple");
            _sprite.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }

        public void RemovePlacement() {
            Object.Destroy(_sprite);
        }
    }
}