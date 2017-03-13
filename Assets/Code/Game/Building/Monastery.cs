using Code.Game.Data;
using Code.Game.FollowerSubs;
using Code.Network;
using UnityEngine;

namespace Code.Game.Building {
    public class Monastery {
        public int ID { get; }
        public Cell Cell { get; }
        public byte SurroundingsCount { get; set; }
        public PlayerColor Owner { get; set; }

        public Monastery(int id, FollowerLocation loc) {
            ID = id;
            Cell = loc.Parent.IntVector();
            Owner = PlayerColor.NotPicked;
            loc.Link = ID;
            SurroundingsCount = 0;
        }

        public void SetOwner(FollowerLocation construct) {
            Owner = construct.GetOwner();
            CalcSurroundings();
        }

        public void CalcSurroundings() {
            //Debug.logger.Log(LogType.Error, "Init CalcSurroundings");
            SurroundingsCount = 0;
            var corner = Cell.Corner();
            for (var iX = 0; iX < 3; iX++) {
                for (var iY = 0; iY < 3; iY++) {
                    if (iX == 1 && iY == 1) continue;
                    if (Tile.Exist(new Cell(corner, iX, iY))) SurroundingsCount++;
                }
            }
            if (SurroundingsCount != 8) return;
            if (Owner == PlayerColor.NotPicked) return;
            CalcScore();
        }

        private void CalcScore() {
            //Debug.Log("SCORE COUNTER:");
            //Debug.Log("Owner = " + Owner + "; XY " + Cell.XY() + "; ID#" + ID);
            if (Owner == PlayerSync.PlayerInfo.Color) {
                PlayerSync.PlayerInfo.Score += 9;
                PlayerSync.PlayerInfo.FollowersNumber++;
                MainGame.UpdateLocalPlayer();
                Tile.Get(Cell).RemovePlacement(ID);
            }
            //Debug.logger.Log(LogType.Error, "Monastery Complited!!!");
            Delete();
        }

        private void Delete() {
            Builder.Monasteries.Remove(this);
        }

        public void Debugger(Area type) {
            var s = Owner;
            var vs = "(" + Cell.X + ";" + Cell.Y + ")";
            Debug.Log("[" + type + "#" + ID + "][" + SurroundingsCount + "] " + s + "/" + vs);
        }
    }
}