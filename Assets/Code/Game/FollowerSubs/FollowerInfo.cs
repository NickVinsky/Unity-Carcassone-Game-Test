using System.Collections.Generic;
using System.Linq;
using Code.Network;
using UnityEngine;

namespace Code.Game.FollowerSubs {
    public class FollowerInfo {
        private List<FollowerLocation> _possibleLocation = new List<FollowerLocation>();

        private PlayerColor _owner = PlayerColor.NotPicked;
        private Follower _follower = Follower.None;

        private GameObject _3DMeeple;

        public void AddLocation(Area type, List<byte> nodes, bool coatOfArms, Vector2 meeplePos) {
            _possibleLocation.Add(new FollowerLocation(type, nodes, coatOfArms, meeplePos));
        }

        public void AddLocation(Area type, List<byte> nodes, Vector2 meeplePos) {
            _possibleLocation.Add(new FollowerLocation(type, nodes, false, meeplePos));
        }

        public void AddLocation(Area type, Vector2 meeplePos) {
            _possibleLocation.Add(new FollowerLocation(type, meeplePos));
        }

        public bool SideFree(byte side) {
            return _possibleLocation.Where(loc => loc.IsBarrier()).Any(loc => loc.GetNodes().Any(node => node == side));
        }

        public void Show(GameObject o, sbyte rotates) {
            foreach (var loc in _possibleLocation) {
                loc.Show(o, rotates);
            }
        }
    }
}