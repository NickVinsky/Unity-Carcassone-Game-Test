using System;
using System.Linq;
using Code.Game.Data;
using Code.Game.FollowerSubs;
using Code.Network;

namespace Code.Game.Building {
    public class Road : Construction {
        protected byte Plugs;

        public Road(int id, Cell v, FollowerLocation loc) : base(id, v) { Plugs = (byte) loc.GetNodes().Length; }

        private void CalcScore() {
            var oArray = new byte[Enum.GetNames(typeof(PlayerColor)).Length - 1];
            foreach (var owner in Owners) {
                oArray[(int) owner]++;
            }
            //Debug.Log("Extra Points Before = " + ExtraPoints);
            foreach (var tile in LinkedTiles) {
                foreach (var loc in tile.Get().GetLocations()) {
                    if (loc.Type == Area.Road && loc.IsLinkedTo(ID)) {
                        loc.RemovePlacement();
                    }
                }
            }
            var max = oArray.Max();
            var score = LinkedTiles.Count;
            //Debug.Log("Score debug: Final = " + score + ", Extra = " + ExtraPoints + ", TilesCount = " + LinkedTiles.Count);

            for (var i = 0; i < oArray.Length; i++) {
                if (oArray[i] == 0) continue;
                if ((PlayerColor) i == PlayerSync.PlayerInfo.Color) {
                    PlayerSync.PlayerInfo.Score += score;
                    PlayerSync.PlayerInfo.FollowersNumber += max;
                    MainGame.UpdateLocalPlayer();
                }
            }
            //Debug.logger.Log(LogType.Error, "Road Complited!!!");
            Delete(this);
        }

        protected override void AddNodesToFinish(int value) {
            if (value == 1) Plugs--;
        }

        protected override void CalcNodesToFinish() { FinalNodesCalcToFinish(); }

        protected override void CalcNodesToFinish(int value) {
            AddNodesToFinish(value);
            FinalNodesCalcToFinish();
        }

        private void FinalNodesCalcToFinish() {
            if (Plugs == 0) {
                if (Owners.Count == 0) return;
                CalcScore();
            }
        }

        protected override bool Equals(Area type) { return type == Area.Road;}

        protected override void Merge(FollowerLocation construct) {
            base.Merge(construct);
            base.Merge(Builder.GetRoad(construct), construct);
        }

        protected override void Delete(Construction construct) {
            Builder.Roads.Remove((Road)construct);
        }
    }
}