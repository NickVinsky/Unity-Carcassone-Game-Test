using System;
using System.Linq;
using Code.Game.Building;
using Code.Game.Data;
using Code.Game.FollowerSubs;
using Code.Network;
using Code.Network.Commands;
using UnityEngine;
using static Code.Network.PlayerSync;

namespace Code.Game {
    public static class ScoreCalc {

        // After tile putting
        public static void Count(GameObject cell) {
            Builder.Check(cell);
            UpdateGUI();
        }

        //After follower assignment
        public static void ApplyFollower(FollowerLocation loc) {
            if (Net.Game.IsOnline())
                Net.Client.SubtractFollower(loc.GetOwner());
            PlayerInfo.FollowersNumber--;
            Builder.SetOwner(loc);
            UpdateGUI();
        }

        public static void ApplyOpponentFollower(FollowerLocation loc) {
            Builder.SetOwner(loc);
            UpdateGUI();
        }

        private static void UpdateGUI() {
            if (Net.Game.IsOnline()) {
                if (Net.IsServer)
                    Net.Server.RefreshInGamePlayersList();
            } else {
                MainGame.UpdateLocalPlayer();
            }
        }

        private static void AddScoreServer(PlayerColor playerColor, byte pFollowersQuantity, byte followersToControl, int score) {
            var player = Net.Player.First(p => p.Color == playerColor);
            var index = Net.Player.IndexOf(player);
            if (pFollowersQuantity == followersToControl) {
                player.Score += score;
            }
            player.FollowersNumber += pFollowersQuantity;
            Net.Player[index] = player;
        }

        private static void AddScoreLocal(byte myFollowersQuantity, byte followersToControl, int score) {
            if (myFollowersQuantity == followersToControl) PlayerInfo.Score += score;
            PlayerInfo.FollowersNumber += myFollowersQuantity;
        }

        private static void AddScore(byte[] ownerFollowers, byte followersToControl, int score) {
            for (var i = 0; i < ownerFollowers.Length; i++) {
                if (ownerFollowers[i] == 0) continue;
                var curPlayer = (PlayerColor) i;
                if (Net.Game.IsOnline()) {
                    if (Net.IsServer) AddScoreServer(curPlayer, ownerFollowers[i], followersToControl, score);
                }
                if (curPlayer != PlayerInfo.Color) continue;
                AddScoreLocal(ownerFollowers[i], followersToControl, score);
            }
        }

        public static void Monastery(Monastery monastery) {
            var owner = monastery.Owner;
            RemovePlacement(monastery);

            var score = monastery.SurroundingsCount;

            if (Net.Game.IsOnline() && Net.IsServer) AddScoreServer(owner, 1, 1, score);
            if (owner == PlayerInfo.Color) AddScoreLocal(1, 1, score);

            monastery.Finished = true;
        }

        public static void Road(Road road) {
            var oArray = OwnersArray(road);
            RemovePlacementsAndCalcExtraPoints(road);

            var score = road.LinkedTiles.Count;
            AddScore(oArray, oArray.Max(), score);

            road.Finished = true;
        }

        public static void City(City city) {
            var oArray = OwnersArray(city);
            RemovePlacementsAndCalcExtraPoints(city);

            var score = city.ExtraPoints + city.LinkedTiles.Count * 2;
            AddScore(oArray, oArray.Max(), score);

            city.Finished = true;
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
                        if (Net.Game.IsOnline()) {
                            Net.Client.Action(Command.RemovePlacement, tile, loc.GetID());
                        } else {
                            loc.RemovePlacement();
                        }
                    }
                }
            }
        }

        private static void RemovePlacement(Monastery monastery) {
            if (Net.Game.IsOnline()) {
                Net.Client.Action(Command.RemovePlacementMonk, monastery.Cell, monastery.ID);
            } else {
                Tile.Get(monastery.Cell).RemovePlacement(monastery.ID);
            }

            /*(var locs = Tile.Get(monastery.Cell).GetLocations();
            foreach (var loc in locs) {
                if (loc.Type != Area.Monastery) continue;
                loc.RemovePlacement();
            }*/
        }
    }
}