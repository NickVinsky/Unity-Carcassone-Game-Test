using System;
using System.Collections.Generic;
using System.Linq;
using Code.Game.Data;
using Code.Network;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Game.FollowerSubs {
    public class Location {
        public TileInfo Parent { get; }
        public District District { get; }

        private readonly byte _id;
        public Area Type { get; }

        public int Link { get; set; }
        public byte[] LinkedToCity { get; set; } // Указывает, с какими городами на текущем тайле соприкасается это поле

        public bool CoatOfArms { get; }
        public bool HasCathedral { get; }
        public bool HasInn { get; }

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

        private PlayerColor _owner = PlayerColor.NotPicked;
        public Follower FollowerType { get; set; }
        public bool ReadyForMeeple { get; set; }
        public bool ReadyForPigOrBuilder { get; set; }

        // 0     1
        //  |---|
        //  |---|
        //  |---|
        // 3     2
        public bool[] ReadyForBarn { get; set; }

        private Vector2 _meeplePos;
        private GameObject _sprite;

        public string GetMeeplePos() { return "[" + _meeplePos.x + ";" + _meeplePos.y + "]"; }

        public int GetDistrictSize() { return District.GetSize(); }
        public bool LastInDistrict() { return District.GetSize() == _id + 1; }

        public Location(TileInfo parent, District district, LocationInfo locInfo) {
            Parent = parent;
            District = district;

            _id = locInfo.LocID;
            Type = locInfo.Type;
            _meeplePos = locInfo.MeeplePos;

            if (locInfo.Nodes == null) locInfo.Nodes = new List<byte>();
            var nLen = locInfo.Nodes.Count;
            _nodes = new byte[nLen];
            for (var i = 0; i < nLen; i++) _nodes[i] = locInfo.Nodes[i];

            LinkedToCity = locInfo.LinkedToCity;

            CoatOfArms = locInfo.CoatOfArms;
            HasCathedral = locInfo.HasCathedral;
            HasInn = locInfo.HasInn;

            Link = -1;
            ReadyForMeeple = true;
            ReadyForBarn = new bool[4];
        }

        public byte[] GetNodes() { return _nodes; }
        public bool CompareID(byte id){ return id == _id;}
        public bool IsLinkedTo(int id) { return Link == id; }
        public byte GetID() { return _id; }

        public PlayerColor GetOwner() { return _owner; }
        public Ownership GetOwnership() { return new Ownership{Color = _owner, FollowerType = FollowerType }; }

        public bool IsBarrier() { return Type == Area.Road;}

        public bool Contains(byte[] pattern) {
            /*foreach (var p in pattern) {
                bool founded = _nodes.Any(n => n == p);
                if (!founded) return false;
            }*/
            //return pattern.All(p => _nodes.Contains(p));
            return pattern.Select(p => _nodes.Any(n => n == p)).All(founded => founded);
        }

        public bool ContainsAnyOf(byte[] pattern) {
            return _nodes.Any(n => pattern.Any(p => n == p));
        }

        public bool ContainsOnly(byte[] pattern) {
            /*foreach (var n in _nodes) {
                bool founded = pattern.Any(p => n == p);
                if (!founded) return false;
            }*/
            return _nodes.Select(n => pattern.Any(p => n == p)).All(founded => founded);
        }

        public bool Conform(byte[] pattern) {
            /*foreach (var n in _nodes) {
                bool founded = pattern.Any(p => n == p);
                if (!founded) return false;
            }
            return true;*/
            //return _nodes.All(pattern.Contains);
            return _nodes.Select(n => pattern.Any(p => n == p)).All(founded => founded);
        }

        private byte Trim(byte node, byte rotate) {
            var rNode = node;
            byte cap, add;
            if (Type != Area.Field) {
                if (Type == Area.Road || Type == Area.City) {
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

        public void SetOwner(PlayerColor owner, Follower type) {
            //if (owner == _owner) return;
            ReadyForMeeple = false;
            _owner = owner;
            FollowerType = type;
            ScoreCalc.ApplyOpponentFollower(this);
            SpriteInit(type);
            _sprite.GetComponent<SpriteRenderer>().color = Net.Color(_owner);
        }

        public void SetOwner(Follower type) {
            ReadyForMeeple = false;
            FollowerType = type;
            _owner = MainGame.Player.Color;
            //if (_sprite.GetComponent<Rigidbody2D>() != null) {Object.Destroy(_sprite.GetComponent<Rigidbody2D>());}
            //if (_sprite.GetComponent<BoxCollider2D>() != null) {Object.Destroy(_sprite.GetComponent<BoxCollider2D>());}
            Object.Destroy(_sprite.GetComponent<Rigidbody2D>());
            Object.Destroy(_sprite.GetComponent<BoxCollider2D>());
            _sprite.GetComponent<SpriteRenderer>().color = Net.Color(_owner);

            ScoreCalc.ApplyFollower(this, type);
            MainGame.ChangeGameStage(GameStage.Finish);

            Tile.Cache.Last().LocactionID = (sbyte) _id;
            Tile.Cache.Last().LocationOwner = _owner;
            Tile.Cache.Last().FollowerType = FollowerType;

            if (!Net.Game.IsOnline()) return;
            var name = _sprite.transform.parent.gameObject.name;
            Net.Client.SendFollower(_owner, _id, name, FollowerType);
        }

        public void ShowMeeple(sbyte rotates, Follower type) {
            if (type == Follower.Barn) {
                for (var i = 0; i < 4; i++) {
                    if (!ReadyForBarn[i]) continue;
                    ShowBarn(i, rotates);
                    return;
                }
            } else {
                SpriteInit(type);
                AddHook(type);
            }
        }

        private void SpriteInit(Follower type) { SpriteInit(type, _meeplePos); }
        private void SpriteInit(Follower type, Vector2 position) {
            _sprite = new GameObject {name = type + "//" + Type + "(" + _meeplePos.x + ";" + _meeplePos.y + ")"};
            _sprite.transform.SetParent(Parent.gameObject.transform);
            _sprite.transform.localScale = GetSpriteScale(type);
            _sprite.transform.localPosition = position;
            _sprite.AddComponent<SpriteRenderer>();
            _sprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(GetSpriteName(type, Type == Area.Field));
            _sprite.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }

        private void AddHook(Follower type) {
            _sprite.AddComponent<BoxCollider2D>();
            _sprite.GetComponent<BoxCollider2D>().size = new Vector2(3f, 3f);
            _sprite.AddComponent<Rigidbody2D>();
            _sprite.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            _sprite.AddComponent<FollowerHook>();
            _sprite.GetComponent<FollowerHook>().Id = _id;
            _sprite.GetComponent<FollowerHook>().Type = type;
        }

        private void ShowBarn(int placement, sbyte rotates) {
            var space = GameRegulars.TileSizeX / 200;
            Vector2 position;
            placement -= rotates;
            if (placement < 0) placement += 4;
            if (placement == 0) position = new Vector2(-space, space);
            else if (placement == 1) position = new Vector2(space, space);
            else if (placement == 2) position = new Vector2(space, -space);
            else if (placement == 3) position = new Vector2(-space, -space);
            else position = new Vector2(0, 0);

            SpriteInit(Follower.Barn, position);
            AddHook(Follower.Barn);
        }

        public void RemovePlacement() {
            //{Object.Destroy(_sprite.GetComponent<Rigidbody2D>());}
            //{Object.Destroy(_sprite.GetComponent<BoxCollider2D>());}
            //Object.Destroy(_sprite.GetComponent<SpriteRenderer>());
            //_sprite.GetComponent<SpriteRenderer>().color = Net.Color(_owner);
            //Object.DestroyImmediate(_sprite);
            Object.Destroy(_sprite);
        }

        private static string GetSpriteName(Follower type, bool variation = false) {
            switch (type) {
                case Follower.Meeple:
                    if (variation) return "3dMeepleFarmer";
                    return "3dMeeple";
                case Follower.BigMeeple:
                    if (variation) return "3dMeepleFarmer";
                    return "3dMeeple";
                case Follower.Mayor:
                    return "3dMayor";
                case Follower.Pig:
                    return "3dPig";
                case Follower.Builder:
                    return "3dBuilder";
                case Follower.Barn:
                    return "3dBarn";
                case Follower.Wagon:
                    return "";
                default:
                    return "";
            }
        }

        private Vector3 GetSpriteScale(Follower type) {
            switch (type) {
                case Follower.BigMeeple:
                    return new Vector3(0.3f, 0.3f, 0);
                case Follower.Mayor:
                    return new Vector3(0.26f, 0.26f, 0);
                case Follower.Pig:
                    return new Vector3(0.28f, 0.28f, 0);
                case Follower.Builder:
                    return new Vector3(0.28f, 0.28f, 0);
                case Follower.Barn:
                    return new Vector3(0.5f, 0.5f, 0);
                default:
                    return new Vector3(0.22f, 0.22f, 0);
            }
        }

        public void MakeTransparent() {
            if (_owner == PlayerColor.NotPicked) return;
            var c = _sprite.GetComponent<SpriteRenderer>().color;
            var transColor = new Color(c.r, c.g, c.b, 0.6f);
            _sprite.GetComponent<SpriteRenderer>().color = Color.black;
        }
    }
}