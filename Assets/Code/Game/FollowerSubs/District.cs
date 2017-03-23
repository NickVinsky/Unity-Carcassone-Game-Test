using System;
using System.Collections.Generic;
using System.Linq;
using Code.Game.Data;
using UnityEngine;

namespace Code.Game.FollowerSubs {
    public class District {
        private TileInfo _parent;
        private readonly List<Location> _possibleLocation = new List<Location>();

        //private Follower _follower = Follower.None;

        //private GameObject _3DMeeple;

        public List<Location> GetLocations() { return _possibleLocation; }

        public int GetSize() { return _possibleLocation.Count; }

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

        public void Opponent(PlayerColor owner, byte id, Follower type) {
            foreach (var loc in _possibleLocation) {
                if (!loc.CompareID(id)) continue;
                loc.SetOwner(owner, type);
                return;
            }
        }

        public void Show(sbyte rotates, Follower type) {
            var counter = 0;
            foreach (var loc in _possibleLocation) {
                if (type == Follower.Barn) {
                    if (loc.Type != Area.Field) continue;
                    if (loc.ReadyForBarn.All(p => !p)) continue;
                }
                if (type == Follower.Pig) {
                    if (loc.Type != Area.Field) continue;
                    if (!loc.ReadyForPigOrBuilder) continue;
                }
                if (type == Follower.Meeple || type == Follower.BigMeeple || type == Follower.Mayor) {
                    if (!loc.ReadyForMeeple) continue;
                }
                if (type == Follower.Mayor) {
                    if (loc.Type != Area.City) continue;
                }
                loc.ShowMeeple(rotates, type);
                counter++;
            }


            if (counter > 0) return;
            switch (type) {
                case Follower.Meeple:
                    _parent.PlacementBlocked[(int) Placements.Meeples] = true;
                    break;
                case Follower.BigMeeple:
                    _parent.PlacementBlocked[(int) Placements.BigMeeples] = true;
                    break;
                case Follower.Mayor:
                    _parent.PlacementBlocked[(int) Placements.Mayor] = true;
                    break;
                case Follower.Pig:
                    _parent.PlacementBlocked[(int) Placements.PigsAndBuilders] = true;
                    break;
                case Follower.Builder:
                    _parent.PlacementBlocked[(int) Placements.PigsAndBuilders] = true;
                    break;
                case Follower.Barn:
                    _parent.PlacementBlocked[(int) Placements.BarnAndWagons] = true;
                    break;
                case Follower.Wagon:
                    _parent.PlacementBlocked[(int) Placements.BarnAndWagons] = true;
                    break;
            }

            _parent.ShowNextPossiblePlacement();
        }

        public void HideAll() {
            foreach (var loc in _possibleLocation) {
                loc.RemovePlacement();
            }
        }

        public void HideExcept(byte except, Follower type) {
            foreach (var loc in _possibleLocation) {
                if (loc.CompareID(except)) {
                    loc.SetOwner(type);
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