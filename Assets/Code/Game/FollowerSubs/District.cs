﻿using System.Collections.Generic;
using System.Linq;
using Code.Game.Data;
using UnityEngine;

namespace Code.Game.FollowerSubs {
    public class District {
        private TileInfo _parent;
        private List<Location> _possibleLocation = new List<Location>();

        //private Follower _follower = Follower.None;

        //private GameObject _3DMeeple;

        public List<Location> GetLocations() { return _possibleLocation; }

        public void AddLocation(TileInfo parent, LocationInfo locInfo) {
            var nextId = (byte) _possibleLocation.Count;
            locInfo.LocID = nextId;
            _possibleLocation.Add(new Location(parent, this, locInfo));
            _parent = parent;
        }

        public Location GetLocation(byte id) {
            return _possibleLocation.First(l => l.CompareID(id));
        }

        public bool SideFree(byte side) {
            return _possibleLocation.Where(loc => loc.IsBarrier()).Any(loc => loc.GetNodes().Any(node => node == side));
        }

        public void Opponent(PlayerColor owner, byte id) {
            foreach (var loc in _possibleLocation) {
                if (!loc.CompareID(id)) continue;
                loc.SetOwner(owner);
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

        public void RemovePlacement(int constructID) {
            foreach (var loc in _possibleLocation) {
                if (!loc.IsLinkedTo(constructID)) continue;
                loc.RemovePlacement();
                return;
            }
        }

        public void RotateNodes(byte rotate) {
            foreach (var loc in _possibleLocation) {
                loc.Rotate(rotate);
            }
        }
    }
}