using System.Collections.Generic;
using System.Linq;
using Code.Network;
using UnityEngine;

namespace Code.Game.FollowerSubs {
    public class FollowerInfo {

        // |1 2 3|
        // |8 9 4|
        // |7 6 5|
        //
        private List<FollowerLocation> _possibleLocation = new List<FollowerLocation>();
        private bool[] _placeIsAvailable = {false,false,false,false,false,false,false,false,false};
        private PlayerColor _owner = PlayerColor.NotPicked;
        private Follower _follower = Follower.None;
        private sbyte _location;
        private GameObject _3DMeeple;

        public void AddLocation(Area type, List<byte> nodes) {
            _possibleLocation.Add(new FollowerLocation(type, nodes));
        }

        public void AddLocation(Area type) {
            _possibleLocation.Add(new FollowerLocation(type));
        }

        public bool SideFree(byte side) {
            return _possibleLocation.Where(loc => loc.IsBarrier()).Any(loc => loc.GetNodes().Any(node => node == side));
        }
    }
}