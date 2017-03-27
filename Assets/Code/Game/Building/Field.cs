using System.Collections.Generic;
using Code.Game.Data;
using Code.Game.FollowerSubs;

namespace Code.Game.Building {
    public class Field : Construction {
        public List<int> LinkedCities = new List<int>();
        public bool Gathered { get; set; }

        public Field(int id, Cell v) : base(id, v) {
            Type = Area.Field;
            Gathered = false;
        }

        protected override void PostAddingAction(Location location) {
            if (location.Type != Area.Field || HasBarn()) return;

            var center = location.Parent.IntVector();
            var hasLinks = new bool[4, 3];


            /*for (var x = -1; x <= 1; x++) {
                for (var y = -1; y <= 1; y++) {
                    if (x == 0 && y == 0) continue;
                    var cell = center.Offset(x, y);
                    if (HasCell(cell)) {
                        foreach (var loc in Tile.Get(cell).GetLocations()) {
                            if (loc.Type != Area.Field) continue;
                            if (loc.Contains(new byte[] {2})) hasLinks[0, 1] = true;
                        }
                    }
                }
            }*/


            if (location.Contains(new byte[] {0, 7})) {
                if (HasCells(center.Left(), center.CornerLeftTop(), center.Top())) {
                    foreach (var l in Tile.Get(center.Left()).GetLocations()) {
                        if (l.Type != Area.Field) continue;
                        if (l.Contains(new byte[] {2})) hasLinks[0, 0] = true;
                    }

                    foreach (var l in Tile.Get(center.CornerLeftTop()).GetLocations()) {
                        if (l.Type != Area.Field) continue;
                        if (l.Contains(new byte[] {3, 4})) hasLinks[0, 1] = true;
                    }

                    foreach (var l in Tile.Get(center.Top()).GetLocations()) {
                        if (l.Type != Area.Field) continue;
                        if (l.Contains(new byte[] {5})) hasLinks[0, 2] = true;
                    }
                }
            }
            if (location.Contains(new byte[] {1, 2})) {
                if (HasCells(center.Top(), center.CornerRightTop(), center.Right())) {
                    foreach (var l in Tile.Get(center.Top()).GetLocations()) {
                        if (l.Type != Area.Field) continue;
                        if (l.Contains(new byte[] {4})) hasLinks[1, 0] = true;
                    }

                    foreach (var l in Tile.Get(center.CornerRightTop()).GetLocations()) {
                        if (l.Type != Area.Field) continue;
                        if (l.Contains(new byte[] {5, 6})) hasLinks[1, 1] = true;
                    }

                    foreach (var l in Tile.Get(center.Right()).GetLocations()) {
                        if (l.Type != Area.Field) continue;
                        if (l.Contains(new byte[] {7})) hasLinks[1, 2] = true;
                    }
                }
            }
            if (location.Contains(new byte[] {3, 4})) {
                if (HasCells(center.Right(), center.CornerRightBot(), center.Bot())) {
                    foreach (var l in Tile.Get(center.Right()).GetLocations()) {
                        if (l.Type != Area.Field) continue;
                        if (l.Contains(new byte[] {6})) hasLinks[2, 0] = true;
                    }

                    foreach (var l in Tile.Get(center.CornerRightBot()).GetLocations()) {
                        if (l.Type != Area.Field) continue;
                        if (l.Contains(new byte[] {7, 0})) hasLinks[2, 1] = true;
                    }

                    foreach (var l in Tile.Get(center.Bot()).GetLocations()) {
                        if (l.Type != Area.Field) continue;
                        if (l.Contains(new byte[] {1})) hasLinks[2, 2] = true;
                    }
                }
            }
            if (location.Contains(new byte[] {5, 6})) {
                if (HasCells(center.Bot(), center.CornerLeftBot(), center.Left())) {
                    foreach (var l in Tile.Get(center.Bot()).GetLocations()) {
                        if (l.Type != Area.Field) continue;
                        if (l.Contains(new byte[] {0})) hasLinks[3, 0] = true;
                    }

                    foreach (var l in Tile.Get(center.CornerLeftBot()).GetLocations()) {
                        if (l.Type != Area.Field) continue;
                        if (l.Contains(new byte[] {1, 2})) hasLinks[3, 1] = true;
                    }

                    foreach (var l in Tile.Get(center.Left()).GetLocations()) {
                        if (l.Type != Area.Field) continue;
                        if (l.Contains(new byte[] {3})) hasLinks[3, 2] = true;
                    }
                }
            }

            for (var i = 0; i < 4; i++) {
                var readyForBarn = true;
                for (var j = 0; j < 3; j++) {
                    if (!hasLinks[i, j]) readyForBarn = false;
                }
                if (readyForBarn) location.ReadyForBarn[i] = true;
            }
        }

        public void RecalcBarn() {
            if (!NeedBarnRecalc) return;

            ScoreCalc.Field(this);
            NeedBarnRecalc = false;
        }

        public void Abandon() {
            for (var i = 0; i < Owners.Count; i++) {
                if (Owners[i].FollowerType != Follower.Barn) Owners.RemoveAt(i);
            }
        }

        protected override void Merge(Location construct) {
            base.Merge(construct);
            base.Merge(Builder.GetField(construct), construct);
        }

        protected override void Delete(Construction construct) {
            Builder.Fields.Remove((Field)construct);
        }
    }
}