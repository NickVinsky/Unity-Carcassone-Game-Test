using System;
using System.Linq;
using Code.Game.Building;
using Code.Game.Data;
using Code.Game.FollowerSubs;
using Code.Network;
using Code.Network.Commands;
using UnityEngine;
using static Code.MainGame;

namespace Code.Game {
    public static class ScoreCalc {

        public static void Final() {
            foreach (var c in Builder.Monasteries) Monastery(c, true);
            foreach (var c in Builder.Cities) City(c, true);
            foreach (var c in Builder.Roads) Road(c, true);
            foreach (var c in Builder.Fields) Field(c);
        }

        // After tile putting
        public static void Count(GameObject cell) {
            Builder.Assimilate(cell);
            UpdateGUI();
        }

        //After follower assignment
        public static void ApplyFollower(Location loc) {
            if (Net.Game.IsOnline()) Net.Client.SubtractFollower(loc.GetOwner());
            Player.FollowersNumber--;
            Builder.SetOwner(loc);
            UpdateGUI();
        }

        public static void ApplyOpponentFollower(Location loc) {
            Builder.SetOwner(loc);
            UpdateGUI();
        }

        private static void UpdateGUI() {
            if (Net.Game.IsOnline()) {
                if (Net.IsServer) Net.Server.RefreshInGamePlayersList();
            } else {
                UpdateLocalPlayer();
            }
        }

        private static void AddScoreServer(PlayerColor playerColor, byte pFollowersQuantity, byte followersToControl, int score) {
            var player = Net.PlayersList.First(p => p.Color == playerColor);
            var index = Net.PlayersList.IndexOf(player);
            if (pFollowersQuantity == followersToControl) {
                player.Score += score;
            }
            player.FollowersNumber += pFollowersQuantity;
            Net.PlayersList[index] = player;
        }

        private static void AddScoreLocal(byte myFollowersQuantity, byte followersToControl, int score) {
            if (myFollowersQuantity == followersToControl) Player.Score += score;
            Player.FollowersNumber += myFollowersQuantity;
        }

        private static void AddScore(byte[] ownerFollowers, byte followersToControl, int score) {
            for (var i = 0; i < ownerFollowers.Length; i++) {
                if (ownerFollowers[i] == 0) continue;
                var curPlayer = (PlayerColor) i;
                if (Net.Game.IsOnline()) {
                    if (Net.IsServer) AddScoreServer(curPlayer, ownerFollowers[i], followersToControl, score);
                }
                if (curPlayer != Player.Color) continue;
                AddScoreLocal(ownerFollowers[i], followersToControl, score);
            }
        }

        public static void Monastery(Monastery monastery, bool final = false) {
            if (final && monastery.Finished) return;
            var owner = monastery.Owner;
            if (owner == PlayerColor.NotPicked) return;
            if (!final) RemovePlacement(monastery);

            var score = monastery.SurroundingsCount;

            if (Net.Game.IsOnline() && Net.IsServer) AddScoreServer(owner, 1, 1, score);
            if (owner == Player.Color) AddScoreLocal(1, 1, score);

            if (!final) monastery.Finished = true;
        }

        public static void Road(Road road, bool final = false) {
            if (final && road.FinishedByPlayer) return;
            var oArray = OwnersArray(road);
            CalcExtraPoints(road);

            var score = road.LinkedTiles.Count;
            AddScore(oArray, oArray.Max(), score);

            if (final) return;
            road.FinishedByPlayer = true;
            RemovePlacements(road);
        }

        public static void City(City city, bool final = false) {
            if (final && city.FinishedByPlayer) return;
            var oArray = OwnersArray(city);
            CalcExtraPoints(city);

            var score = city.ExtraPoints + city.LinkedTiles.Count * 2;
            if (final) score /= 2;
            AddScore(oArray, oArray.Max(), score);

            if (final) return;
            city.FinishedByPlayer = true;
            RemovePlacements(city);
        }

        public static void Field(Field field) {
            foreach (var linkedCell in field.LinkedTiles) {
                var linkedTile = linkedCell.Get();
                foreach (var loc in linkedTile.GetLocations()) {
                    if (loc.Type != Area.Field) continue;
                    if (loc.Link != field.ID) continue;

                    if (loc.LinkedToCity == null) continue;
                    foreach (var linkToCity in loc.LinkedToCity) {
                        //Net.Client.ChatMessage("linkToCity => " + linkToCity);
                        var city = Builder.GetCity(linkedTile.GetLocation(linkToCity));
                        if (!city.Finished()) continue;
                        //Net.Client.ChatMessage("finished");
                        if (Enumerable.Contains(field.LinkedCities, (byte) city.ID)) continue;
                        field.LinkedCities.Add(city.ID);
                    }
                }
            }

            //Net.Client.ChatMessage("field.LinkedCities.Count: " + field.LinkedCities.Count);

            var oArray = OwnersArray(field);

            var score = field.LinkedCities.Count * 3;
            AddScore(oArray, oArray.Max(), score);
        }

        private static byte[] OwnersArray(Construction construct) {
            var oArray = new byte[Enum.GetNames(typeof(PlayerColor)).Length - 1];
            foreach (var owner in construct.Owners) {
                oArray[(int) owner]++;
            }
            return oArray;
        }

        private static void RemovePlacements(Construction construct) {
            foreach (var tile in construct.LinkedTiles) {
                foreach (var loc in tile.Get().GetLocations()) {
                    if (loc.Type == construct.Type && loc.IsLinkedTo(construct.ID)) {
                        if (Net.Game.IsOnline()) {
                            Net.Client.Action(Command.RemovePlacement, tile, loc.GetID());
                        } else {
                            loc.RemovePlacement();
                            //loc.MakeTransparent();
                        }
                    }
                }
            }
        }

        private static void CalcExtraPoints(Construction construct) {
            foreach (var tile in construct.LinkedTiles) {
                foreach (var loc in tile.Get().GetLocations()) {
                    if (loc.Type == construct.Type && loc.IsLinkedTo(construct.ID)) {
                        construct.AddExtraPoints(loc);
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