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

        public static void RecalcFollowersNumber(Follower type, PlayerInfo player, int shift) {
            switch (type) {
                case Follower.Meeple:
                    player.MeeplesQuantity = (byte) (player.MeeplesQuantity + shift);
                    break;
                case Follower.BigMeeple:
                    player.BigMeeplesQuantity = (byte) (player.BigMeeplesQuantity + shift);
                    break;
                case Follower.Mayor:
                    player.MayorsQuantity = (byte) (player.MayorsQuantity + shift);
                    break;
                case Follower.Pig:
                    player.PigsQuantity = (byte) (player.PigsQuantity + shift);
                    break;
                case Follower.Builder:
                    player.BuildersQuantity = (byte) (player.BuildersQuantity + shift);
                    break;
                case Follower.Barn:
                    player.BarnsQuantity = (byte) (player.BarnsQuantity + shift);
                    break;
                case Follower.Wagon:
                    player.WagonsQuantity = (byte) (player.WagonsQuantity + shift);
                    break;
            }
        }

        public static void Final() {
            foreach (var c in Builder.Monasteries) Monastery(c, true);
            foreach (var c in Builder.Cities) City(c, true);
            foreach (var c in Builder.Roads) Road(c, true);
            foreach (var c in Builder.Fields) Field(c);
            //UpdateGUI();
        }

        // After tile putting
        public static void Count(GameObject cell) {
            Builder.Assimilate(cell);
            UpdateGUI();
        }

        //After follower assignment
        public static void ApplyFollower(Location loc, Follower type) {
            if (Net.Game.IsOnline()) Net.Client.SubtractFollower(loc.GetOwner(), type);
            RecalcFollowersNumber(type, Player, -1);
            //Player.MeeplesQuantity--;
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

        private static void AddScoreServer(PlayerColor playerColor, byte pFollowersQuantity, byte followersToControl, int score, Construction construct = null) {
            var player = Net.PlayersList.First(p => p.Color == playerColor);
            var index = Net.PlayersList.IndexOf(player);
            if (pFollowersQuantity == followersToControl) {
                if (construct != null && construct.GetType() == typeof(Field)) {
                    var field = (Field) construct;
                    if (construct.HasPigOrBuilder(player.Color)) score += field.LinkedCities.Count;
                }
                player.Score += score;
            }
            Net.PlayersList[index] = player;
        }

        private static void AddScoreLocal(byte myFollowersQuantity, byte followersToControl, int score, Construction construct = null) {
            if (myFollowersQuantity == followersToControl) Player.Score += score;
        }

        private static void AddScore(byte[] ownerFollowers, byte followersToControl, int score, Construction construct = null) {
            for (var i = 0; i < ownerFollowers.Length; i++) {
                if (ownerFollowers[i] == 0) continue;
                var curPlayer = (PlayerColor) i;
                if (Net.Game.IsOnline()) {
                    if (Net.IsServer) AddScoreServer(curPlayer, ownerFollowers[i], followersToControl, score, construct);
                }
                if (curPlayer != Player.Color) continue;
                AddScoreLocal(ownerFollowers[i], followersToControl, score, construct);
            }
        }

        public static void ReturnFollower(Ownership owner) {
            if (owner.Color == PlayerColor.NotPicked) return;
            if (Net.Game.IsOnline()) {

                var player = Net.PlayersList.First(p => p.Color == owner.Color);
                var index = Net.PlayersList.IndexOf(player);
                RecalcFollowersNumber(owner.FollowerType, player, 1);
                Net.PlayersList[index] = player;

            } else {

                if (owner.Color != Player.Color) return;
                RecalcFollowersNumber(owner.FollowerType, Player, 1);

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
            var innCoefficient = Convert.ToInt32(road.HasInn);
            CalcExtraPoints(road, 0);

            var score = road.LinkedTiles.Count;
            score += innCoefficient * road.LinkedTiles.Count;
            if (final) if (road.HasInn) score = 0;
            AddScore(oArray, oArray.Max(), score);

            if (final) return;
            road.FinishedByPlayer = true;
            RemovePlacements(road);
        }

        public static void City(City city, bool final = false) {
            if (final && city.FinishedByPlayer) return;
            var cathedralCoefficient = Convert.ToInt32(city.HasCathedral);
            CalcExtraPoints(city, cathedralCoefficient);
            var oArray = OwnersArray(city);


            var score = city.ExtraPoints + city.LinkedTiles.Count * (2 + cathedralCoefficient);
            if (final) {
                if (city.HasCathedral) score = 0;
                else score /= 2;
            }
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
                        var city = Builder.GetCity(linkedTile.GetLocation(linkToCity));
                        if (!city.Finished) continue;
                        if (Enumerable.Contains(field.LinkedCities, (byte) city.ID)) continue;
                        field.LinkedCities.Add(city.ID);
                    }
                }
            }

            var oArray = OwnersArray(field);

            var score = field.LinkedCities.Count * 3;
            AddScore(oArray, oArray.Max(), score);
        }

        private static byte[] OwnersArray(Construction construct) {
            var oArray = new byte[Enum.GetNames(typeof(PlayerColor)).Length - 1];
            foreach (var owner in construct.Owners) {
                var curOwner = (int) owner.Color;
                if (owner.FollowerType == Follower.Meeple) oArray[curOwner]++;
                if (owner.FollowerType == Follower.BigMeeple) oArray[curOwner] += 2;
                if (owner.FollowerType == Follower.Mayor) oArray[curOwner] += construct.GetAsCity().CoatOfArmsQuantity;
            }
            return oArray;
        }

        private static void RemovePlacements(Construction construct) {
            foreach (var tile in construct.LinkedTiles) {
                foreach (var loc in tile.Get().GetLocations()) {
                    if (loc.Type == construct.Type && loc.IsLinkedTo(construct.ID)) {
                        if (Net.Game.IsOnline()) {
                            if (Net.IsServer) ReturnFollower(loc.GetOwnership());
                            Net.Client.Action(Command.RemovePlacement, tile, loc.GetID());
                        } else {
                            loc.RemovePlacement();
                            //loc.MakeTransparent();
                        }
                    }
                }
            }
        }

        private static void CalcExtraPoints(Construction construct, int extraPoints) {
            foreach (var tile in construct.LinkedTiles) {
                foreach (var loc in tile.Get().GetLocations()) {
                    if (loc.Type == construct.Type && loc.IsLinkedTo(construct.ID)) {
                        construct.AddExtraPoints(loc, extraPoints);
                    }
                }
            }
        }

        private static void RemovePlacement(Monastery monastery) {
            if (Net.Game.IsOnline()) {
                if (Net.IsServer) ReturnFollower(Tile.Get(monastery.Cell).GetLocation(monastery.ID).GetOwnership());
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