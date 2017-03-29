using Code.Game.Data;
using Code.Game.FollowerSubs;
using Code.Network;
using UnityEngine;

namespace Code.Game.Building {
    public class Monastery {
        public const Area Type = Area.Monastery;
        public int ID { get; }
        public Cell Cell { get; }
        public byte SurroundingsCount { get; set; }
        public PlayerColor Owner { get; set; }
        public bool Finished { get; set; }

        public Monastery(int id, Location loc) {
            ID = id;
            Cell = loc.Parent.IntVector;
            Owner = PlayerColor.NotPicked;
            loc.Link = ID;
            SurroundingsCount = 0;
        }

        public void SetOwner(Location construct) {
            Owner = construct.Owner;
            CalcSurroundings();
        }

        public void CalcSurroundings() {
            SurroundingsCount = 0;
            var corner = Cell.CornerLeftBot;
            for (var iX = 0; iX < 3; iX++) {
                for (var iY = 0; iY < 3; iY++) {
                    if (Tile.Exist(new Cell(corner, iX, iY))) SurroundingsCount++;
                }
            }

            if (SurroundingsCount != 9) return;
            if (Owner == PlayerColor.NotPicked) return;
            CalcScore();
        }

        private void CalcScore() {
            ScoreCalc.Monastery(this);
        }

        public void Delete() {
            Builder.Monasteries.Remove(this);
        }

        public void Debugger() {
            var s = Owner;
            var vs = "(" + Cell.X + ";" + Cell.Y + ")";
            var log = "[" + Type + "#" + ID + "][" + SurroundingsCount + "] " + s + "/" + vs;
            Debug.Log(log);
            if (Net.Game.IsOnline) Net.Client.ChatMessage(log);
        }
    }
}