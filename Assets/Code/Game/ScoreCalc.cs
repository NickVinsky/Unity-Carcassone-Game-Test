﻿using System;
using System.Collections.Generic;
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

        //var player = Net.IsServer ? Net.PlayersList.First(p => p.Color == Builder.BiggestCityFounder) : Player;
        public static void UpdateKingsPatronage() { foreach (var t in Net.PlayersList) t.HasKingsPatronage = t.Color == Builder.BiggestCityFounder; }
        public static void UpdateAtamansPatronage() { foreach (var t in Net.PlayersList) t.HasAtamansPatronage = t.Color == Builder.BiggestRoadFounder; }

        public static void PatronBonus(PlayerColor color, List<Construction> list) {
            if (color == PlayerColor.NotPicked) return;
            var player = Net.PlayersList.First(p => p.Color == color);
            var index = Net.PlayersList.IndexOf(player);

            player.Score += list.Count(c => c.Finished);

            Net.PlayersList[index] = player;
        }

        public static void Final() {
            foreach (var c in Builder.Monasteries) Monastery(c, true);
            foreach (var c in Builder.Cities) City(c, true);
            foreach (var c in Builder.Roads) Road(c, true);
            foreach (var c in Builder.Fields) Field(c, true);
            PatronBonus(Builder.BiggestCityFounder, Builder.Cities.Cast<Construction>().ToList());
            PatronBonus(Builder.BiggestRoadFounder, Builder.Roads.Cast<Construction>().ToList());
            //UpdateGUI();
            Net.Server.GameResults();
        }

        // After tile putting
        public static void Count(GameObject cell, PlayerColor founder) {
            Builder.Assimilate(cell, founder);
            UpdateKingsPatronage();
            UpdateAtamansPatronage();
            UpdateGUI();
            if (Net.IsServer) Net.Server.AllowPlacement();
        }

        //After follower assignment
        public static void ApplyFollower(Location loc, Follower type) {
            if (Net.Game.IsOnline) Net.Client.SubtractFollower(loc.Owner, type);
            RecalcFollowersNumber(type, Player, -1);
            Builder.SetOwner(loc);
            UpdateGUI();
        }

        public static void ApplyOpponentFollower(Location loc) {
            Builder.SetOwner(loc);
            UpdateGUI();
        }

        private static void UpdateGUI() {
            if (Net.Game.IsOnline) {
                if (Net.IsServer) Net.Server.RefreshInGamePlayersList();
            } else {
                UpdateLocalPlayer();
            }
        }

        private static void AddScoreServer(PlayerColor playerColor, byte pFollowersQuantity, byte followersToControl, int score, Construction construct = null) {
            if (playerColor == PlayerColor.NotPicked) return;
            var player = Net.PlayersList.First(p => p.Color == playerColor);

            if (pFollowersQuantity != followersToControl) return;
            if (construct != null && construct.GetType() == typeof(Field))
                if (construct.HasPigOrBuilder(playerColor)) score += construct.GetAsField.LinkedCities.Count;
            player.Score += score;
        }

        private static void AddScoreLocal(byte myFollowersQuantity, byte followersToControl, int score, Construction construct = null) {
            if (myFollowersQuantity == followersToControl) Player.Score += score;
            if (construct != null && construct.GetType() == typeof(Field))
                if (construct.HasPigOrBuilder(Player.Color)) Player.Score += construct.GetAsField.LinkedCities.Count;
        }

        private static void AddScore(byte[] ownerFollowers, byte followersToControl, int score, Construction construct = null) {
            for (var i = 0; i < ownerFollowers.Length; i++) {
                if (ownerFollowers[i] == 0) continue;
                var curPlayer = (PlayerColor) i;
                if (Net.Game.IsOnline) {
                    if (Net.IsServer) AddScoreServer(curPlayer, ownerFollowers[i], followersToControl, score, construct);
                }
                if (curPlayer != Player.Color) continue;
                AddScoreLocal(ownerFollowers[i], followersToControl, score, construct);
            }
        }

        public static void ReturnFollower(Location location) {
            var owner = location.Ownership;

            if (owner.Color == PlayerColor.NotPicked) return;
            if (Net.Game.IsOnline) {

                var player = Net.PlayersList.First(p => p.Color == owner.Color);
                var index = Net.PlayersList.IndexOf(player);
                RecalcFollowersNumber(owner.FollowerType, player, 1);
                Net.PlayersList[index] = player;

            } else {

                if (owner.Color != Player.Color) return;
                RecalcFollowersNumber(owner.FollowerType, Player, 1);

            }
            location.Cleanup();
        }

        public static void Monastery(Monastery monastery, bool final = false) {
            if (final && monastery.Finished) return;
            var owner = monastery.Owner;
            if (owner == PlayerColor.NotPicked) return;

            var score = monastery.SurroundingsCount;

            if (Net.Game.IsOnline && Net.IsServer) AddScoreServer(owner, 1, 1, score);
            if (owner == Player.Color) AddScoreLocal(1, 1, score);

            if (final) return;
            monastery.Finished = true;
            RemovePlacement(monastery);
        }

        public static void Road(Road road, bool final = false) {
            if (final && road.FinishedByPlayer) return;
            var oArray = OwnersArray(road);
            var innCoefficient = Convert.ToInt32(road.HasInn);
            CalcExtraPoints(road, 0);

            var score = road.LinkedTiles.Count;
            score += innCoefficient * road.Size;
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


            var score = city.ExtraPoints + city.Size * (2 + cathedralCoefficient);
            if (final) {
                if (city.HasCathedral) score = 0;
                else score /= 2;
            }
            AddScore(oArray, oArray.Max(), score);

            if (final) return;
            city.FinishedByPlayer = true;
            RemovePlacements(city);
        }

        public static void Field(Field field, bool final = false) {
            foreach (var linkedCell in field.LinkedTiles) {
                var linkedTile = linkedCell.GetTile;
                foreach (var loc in linkedTile.GetLocations) {
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

            var notGathered = Convert.ToInt32(!field.Gathered);
            var score = field.LinkedCities.Count * (1 + 2 * notGathered);
            //Debug.Log("Cities: " + field.LinkedCities.Count + " ; (1 + 2 * notGathered) = " + (1 + 2 * notGathered) + " ; k = " + notGathered);
            AddScore(oArray, oArray.Max(), score, field);

            field.Gathered = true;
            field.Abandon();
            if (final) return;
            field.FinishedByPlayer = true;
            RemovePlacements(field);
        }

        private static byte[] OwnersArray(Construction construct) {
            var oArray = new byte[Net.ColorsCount + 1];
            foreach (var owner in construct.Owners) {
                var curOwner = (int) owner.Color;
                if (owner.FollowerType == Follower.Meeple) oArray[curOwner]++;
                if (owner.FollowerType == Follower.BigMeeple) oArray[curOwner] += 2;
                if (owner.FollowerType == Follower.Mayor) oArray[curOwner] += construct.GetAsCity.CoatOfArmsQuantity;
            }
            return oArray;
        }

        private static void RemovePlacements(Construction construct) {
            foreach (var tile in construct.LinkedTiles) {
                foreach (var loc in tile.GetTile.GetLocations) {
                    if (loc.Type == construct.Type && loc.IsLinkedTo(construct.ID)) {
                        if (loc.Ownership.FollowerType == Follower.Barn) continue;
                        if (Net.Game.IsOnline) {
                            if (Net.IsServer) ReturnFollower(loc);
                            Net.Client.Action(Command.RemovePlacement, tile, loc.ID);
                        } else {
                            loc.RemovePlacement();
                            //loc.MakeTransparent();
                        }
                    }
                }
            }
        }

        private static void CalcExtraPoints(Construction construct, int extraPoints) {
//            construct.LinkedTiles.ForEach(tile => tile.GetTile.GetLocations.Any(loc => loc.Type == construct.Type));
            foreach (var tile in construct.LinkedTiles) {
                foreach (var loc in tile.GetTile.GetLocations) {
                    if (loc.Type == construct.Type && loc.IsLinkedTo(construct.ID)) {
                        construct.AddExtraPoints(loc, extraPoints);
                    }
                }
            }
        }

        private static void RemovePlacement(Monastery monastery) {
            if (Net.Game.IsOnline) {
                if (Net.IsServer) ReturnFollower(Tile.Get(monastery.Cell).GetLocation(monastery.LocID));
                Net.Client.Action(Command.RemovePlacement, monastery.Cell, monastery.LocID);
            } else {
                Tile.Get(monastery.Cell).GetLocation(monastery.LocID).RemovePlacement();
            }
        }
    }
}