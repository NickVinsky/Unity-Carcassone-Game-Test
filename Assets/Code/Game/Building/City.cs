using System;
using System.Linq;
using Code.Game.Data;
using Code.Game.FollowerSubs;
using Code.Network;
using UnityEngine;

namespace Code.Game.Building {
    public class City : Construction {
        protected int NodesToFinish;

        public City(int id, Cell v, FollowerLocation loc) : base(id, v) { NodesToFinish = loc.GetNodes().Length; }

        private void CalcScore() {
            var oArray = new byte[Enum.GetNames(typeof(PlayerColor)).Length - 1];
            foreach (var owner in Owners) {
                oArray[(int) owner]++;
            }
            //Debug.Log("Extra Points Before = " + ExtraPoints);
            foreach (var tile in LinkedTiles) {
                foreach (var loc in tile.Get().GetLocations()) {
                    if (loc.Type == Area.City && loc.IsLinkedTo(ID)) {
                        AddExtraPoints(loc);
                        loc.RemovePlacement();
                    }
                }
            }
            var max = oArray.Max();
            var score = ExtraPoints + LinkedTiles.Count * 2;
            //Debug.Log("Score debug: Final = " + score + ", Extra = " + ExtraPoints + ", TilesCount = " + LinkedTiles.Count);

            for (var i = 0; i < oArray.Length; i++) {
                if (oArray[i] == 0) continue;
                if ((PlayerColor) i == PlayerSync.PlayerInfo.Color) {
                    PlayerSync.PlayerInfo.Score += score;
                    PlayerSync.PlayerInfo.FollowersNumber += max;
                    MainGame.UpdateLocalPlayer();
                }
            }
            //Debug.logger.Log(LogType.Error, "City Complited!!!");
            Delete(this);
        }

        protected override void AddNodesToFinish(int value) {
            NodesToFinish -= 2;
            NodesToFinish += value;
            Debug.Log("City#" + ID + " AddNodesToFinish => NodesToFinish = " + NodesToFinish);
        }

        protected override void CalcNodesToFinish() { FinalNodesCalcToFinish(); }

        protected override void CalcNodesToFinish(int value) {
            AddNodesToFinish(value);
            FinalNodesCalcToFinish();
        }

        private void FinalNodesCalcToFinish() {
            if (NodesToFinish == 0) {
                if (Owners.Count == 0) return;
                CalcScore();
            }
            Debug.Log("City#" + ID + " CalcNodesToFinish => NodesToFinish = " + NodesToFinish);
        }

        protected override void AddExtraPoints(FollowerLocation loc) {
            if (loc.CoatOfArms) ExtraPoints += 2;
        }

        protected override void MergeExtraPoints(int value) { ExtraPoints += value; }

        protected override bool Equals(Area type) { return type == Area.City;}

        protected override void Merge(FollowerLocation construct) {
            base.Merge(construct);
            base.Merge(Builder.GetCity(construct), construct);
        }

        protected override void Delete(Construction construct) {
            Builder.Cities.Remove((City)construct);
        }
    }
}