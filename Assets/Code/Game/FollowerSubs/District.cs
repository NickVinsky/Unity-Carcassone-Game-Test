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

        public Location GetMonastery() {
            return _possibleLocation.First(l => l.Type == Area.Monastery);
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

        public void Show(sbyte rotates, Placements placement, params Follower[] followers) {
            var counter = 0;
            foreach (var loc in _possibleLocation) {
                foreach (var follower in followers) {
                    if (placement == Placements.BarnAndWagons) {
                        if (loc.Type != Area.Field) continue;
                        if (loc.ReadyForBarn.All(p => !p)) continue;
                    }
                    if (placement == Placements.MeeplesPigsAndBuilders) {
                        if (follower == Follower.Meeple && !loc.ReadyForMeeple) continue;
                        if (follower == Follower.Pig && !loc.ReadyForPigOrBuilder) continue;
                    }
                    if (placement == Placements.BigMeeples) {
                        if (!loc.ReadyForMeeple) continue;
                    }
                    if (placement == Placements.Mayor) {
                        if (loc.Type != Area.City) continue;
                        if (!loc.ReadyForMeeple) continue;
                    }

                    loc.ShowMeeple(rotates, follower);
                    counter++;
                }
            }

            if (counter > 0) return;
            switch (placement) {
                case Placements.MeeplesPigsAndBuilders:
                    _parent.PlacementBlocked[(int) Placements.MeeplesPigsAndBuilders] = true;
                    break;
                case Placements.BigMeeples:
                    _parent.PlacementBlocked[(int) Placements.BigMeeples] = true;
                    break;
                case Placements.Mayor:
                    _parent.PlacementBlocked[(int) Placements.Mayor] = true;
                    break;
                case Placements.BarnAndWagons:
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