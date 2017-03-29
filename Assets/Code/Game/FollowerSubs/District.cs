using System.Collections.Generic;
using System.Linq;
using Code.Game.Data;

namespace Code.Game.FollowerSubs {
    public class District {
        private TileInfo _parent;

        public List<Location> LocationsList { get; } = new List<Location>();

        public int Size => LocationsList.Count;

        public void AddLocation(TileInfo parent, LocationInfo locInfo) {
            var nextId = (byte) LocationsList.Count;
            locInfo.LocID = nextId;
            LocationsList.Add(new Location(parent, this, locInfo));
            _parent = parent;
        }

        public Location GetLocation(byte id) => LocationsList.First(l => l.CompareID(id));

        public Location GetMonastery => LocationsList.First(l => l.Type == Area.Monastery);

        public bool SideFree(byte side) => LocationsList.Where(loc => loc.IsBarrier).Any(loc => loc.Nodes.Any(node => node == side));

        public void Opponent(PlayerColor owner, byte id, Follower type) {
            foreach (var loc in LocationsList) {
                if (!loc.CompareID(id)) continue;
                loc.SetOwner(owner, type);
                return;
            }
        }

        public void Show(sbyte rotates, Placements placement, params Follower[] followers) {
            var counter = 0;
            foreach (var loc in LocationsList) {
                foreach (var follower in followers) {
                    if (placement == Placements.BarnAndWagons) {
                        if (loc.Type != Area.Field) continue;
                        if (loc.ReadyForBarn.All(p => !p)) continue;
                    }

                    if (placement == Placements.MeeplesPigsAndBuilders) {
                        switch (follower) {
                            case Follower.Pig:
                                if (MainGame.Player.PigsQuantity <= 0) continue;
                                if (loc.Type != Area.Field) continue;
                                if (!loc.ReadyForPigOrBuilder) continue;
                                break;
                            case Follower.Builder:
                                if (MainGame.Player.BuildersQuantity <= 0) continue;
                                if (!(loc.Type == Area.City || loc.Type == Area.Road)) continue;
                                if (!loc.ReadyForPigOrBuilder) continue;
                                break;
                            case Follower.Meeple:
                                if (MainGame.Player.MeeplesQuantity <= 0) continue;
                                if (!loc.ReadyForMeeple) continue;
                                break;
                        }
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
            foreach (var loc in LocationsList) {
                loc.RemovePlacement();
            }
        }

        public void HideExcept(byte except, Follower type) {
            foreach (var loc in LocationsList) {
                if (loc.CompareID(except)) {
                    loc.SetOwner(type);
                    continue;
                }
                loc.RemovePlacement();
            }
        }

        public void RemovePlacement(int constructID) {
            foreach (var loc in LocationsList) {
                if (!loc.IsLinkedTo(constructID)) continue;
                loc.RemovePlacement();
                return;
            }
        }

        public void RotateNodes(byte rotate) {
            foreach (var loc in LocationsList) {
                loc.Rotate(rotate);
            }
        }
    }
}