using System.Collections.Generic;
using Code.Game.Data;
using UnityEngine;

namespace Code.Game.FollowerSubs {
    public struct LocationInfo {
        public byte LocID;
        public Area Type;
        public Vector2 MeeplePos;

        public List<byte> Nodes;
        public byte[] LinkedToCity;

        public bool CoatOfArms;
        public bool HasCathedral;
        public bool HasInn;
    }
}