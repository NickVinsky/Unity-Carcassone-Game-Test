using System.Collections.Generic;
using System.Linq;
using Code.Game.Data;
using UnityEngine;

namespace Code.Game.FollowerSubs {
    public class FollowerInfo {
        private List<FollowerLocation> _possibleLocation = new List<FollowerLocation>();

        private Follower _follower = Follower.None;

        private GameObject _3DMeeple;

        public List<FollowerLocation> GetLocations() { return _possibleLocation; }

        public void AddLocation(Area type, List<byte> nodes, bool coatOfArms, Vector2 meeplePos) {
            var nextId = (byte)_possibleLocation.Count;
            _possibleLocation.Add(new FollowerLocation(nextId, type, nodes, coatOfArms, meeplePos));
        }

        public void AddLocation(Area type, List<byte> nodes, Vector2 meeplePos) {
            var nextId = (byte)_possibleLocation.Count;
            _possibleLocation.Add(new FollowerLocation(nextId, type, nodes, false, meeplePos));
        }

        public void AddLocation(Area type, Vector2 meeplePos) {
            var nextId = (byte)_possibleLocation.Count;
            _possibleLocation.Add(new FollowerLocation(nextId, type, meeplePos));
        }

        public bool SideFree(byte side) {
            return _possibleLocation.Where(loc => loc.IsBarrier()).Any(loc => loc.GetNodes().Any(node => node == side));
        }

        public void Opponent(GameObject o, PlayerColor owner, byte id) {
            foreach (var loc in _possibleLocation) {
                if (!loc.CompareID(id)) continue;
                loc.SetOwner(o, owner);
                return;
            }
        }

        public void Show(GameObject o, sbyte rotates) {
            foreach (var loc in _possibleLocation) {
                loc.Show(o, rotates);
            }
        }

        public void HideAll() {
            foreach (var loc in _possibleLocation) {
                loc.RemovePlacement();
            }
        }

        public void HideExcept(byte except) {
            foreach (var loc in _possibleLocation) {
                if (loc.CompareID(except)) {
                    loc.SetOwner();
                    continue;
                }
                loc.RemovePlacement();
            }
        }

        public void RotateNodes(byte rotate) {
            foreach (var loc in _possibleLocation) {
                loc.Rotate(rotate);
            }
        }

        public FollowerLocation GetFilled() { return _possibleLocation.FirstOrDefault(loc => loc.Filled); }
    }
}