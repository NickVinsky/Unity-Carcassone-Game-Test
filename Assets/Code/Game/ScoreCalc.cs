using System;
using System.Linq;
using Code.Game.Building;
using Code.Game.Data;
using Code.Game.FollowerSubs;
using Code.Network;
using UnityEngine;
using static Code.Network.PlayerSync;

namespace Code.Game {
    public static class ScoreCalc {

        // After tile putting
        public static void Count(GameObject cell) {
            Builder.Check(cell);
        }

        //After follower assignment
        public static void ApplyFollower(FollowerLocation loc) {
            PlayerInfo.FollowersNumber--;
            Builder.SetOwner(loc);
        }

        public static void Monastery(Monastery monastery) {
            //Debug.Log("SCORE COUNTER:");
            //Debug.Log("Owner = " + Owner + "; XY " + Cell.XY() + "; ID#" + ID);
            if (monastery.Owner == PlayerInfo.Color) {
                PlayerInfo.Score += 9;
                PlayerInfo.FollowersNumber++;
                MainGame.UpdateLocalPlayer();
                Tile.Get(monastery.Cell).RemovePlacement(monastery.ID);
            }
            //Debug.logger.Log(LogType.Error, "Monastery Complited!!!");
            monastery.Delete();
        }

        public static void Road(Road road) {
            var oArray = OwnersArray(road);
            RemovePlacementsAndCalcExtraPoints(road);

            var max = oArray.Max();
            var score = road.LinkedTiles.Count;

            for (var i = 0; i < oArray.Length; i++) {
                if (oArray[i] == 0) continue;
                if ((PlayerColor) i == PlayerInfo.Color) {
                    PlayerInfo.Score += score;
                    PlayerInfo.FollowersNumber += max;
                    MainGame.UpdateLocalPlayer();
                }
            }
            road.Delete();
        }

        public static void City(City city) {
            var oArray = OwnersArray(city);
            RemovePlacementsAndCalcExtraPoints(city);

            var max = oArray.Max();
            var score = city.ExtraPoints + city.LinkedTiles.Count * 2;

            for (var i = 0; i < oArray.Length; i++) {
                if (oArray[i] == 0) continue;
                if ((PlayerColor) i == PlayerInfo.Color) {
                    PlayerInfo.Score += score;
                    PlayerInfo.FollowersNumber += max;
                    MainGame.UpdateLocalPlayer();
                }
            }
            city.Delete();
        }

        private static byte[] OwnersArray(Construction construct) {
            var oArray = new byte[Enum.GetNames(typeof(PlayerColor)).Length - 1];
            foreach (var owner in construct.Owners) {
                oArray[(int) owner]++;
            }
            return oArray;
        }

        private static void RemovePlacementsAndCalcExtraPoints(Construction construct) {
            foreach (var tile in construct.LinkedTiles) {
                foreach (var loc in tile.Get().GetLocations()) {
                    if (loc.Type == construct.Type && loc.IsLinkedTo(construct.ID)) {
                        construct.AddExtraPoints(loc);
                        loc.RemovePlacement();
                    }
                }
            }
        }
    }
}