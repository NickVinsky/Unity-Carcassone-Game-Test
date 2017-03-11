using System;
using System.Collections.Generic;
using System.Linq;
using Code.Game.Data;
using Code.Game.FollowerSubs;
using UnityEngine;

namespace Code.Game.Building {
    public static class Builder {
        private struct Pattern {
            public static byte[] Top { get; set; }
            public static byte[] Right { get; set; }
            public static byte[] Bot { get; set; }
            public static byte[] Left { get; set; }
        }
        private struct Counter {
            public static int Cities { get; set; }
            public static int Roads { get; set; }
            public static int Fields { get; set; }
            public static int Monasteries { get; set; }

            public static int Next(Area type) {
                switch (type) {
                    case Area.Field:
                        return ++Fields;
                    case Area.Road:
                        return ++Roads;
                    case Area.City:
                        return ++Cities;
                    case Area.Monastery:
                        return ++Monasteries;
                    default:
                        return -1;
                }
            }
        }

        private static List<City> _cities = new List<City>();
        private static List<Road> _roads = new List<Road>();
        private static List<Field> _fields = new List<Field>();
        private static List<Monastery> _monasteries = new List<Monastery>();

        public static List<City> Cities {get { return _cities; }}
        public static List<Road> Roads {get { return _roads; }}
        public static List<Field> Fields {get { return _fields;}}
        public static List<Monastery> Monasteries {get { return _monasteries; }}

        private static string ArrayToString(byte[] array) {
            return array.Aggregate(string.Empty, (current, a) => current + a);
        }

        private static void SetPattern(Area area) {
            switch (area) {
                case Area.Field:
                    Pattern.Top = new byte[] {0, 1};
                    Pattern.Right = new byte[] {2, 3};
                    Pattern.Bot = new byte[] {4, 5};
                    Pattern.Left = new byte[] {6, 7};
                    break;
                case Area.City:
                case Area.Road:
                    Pattern.Top = new byte[] {0};
                    Pattern.Right = new byte[] {1};
                    Pattern.Bot = new byte[] {2};
                    Pattern.Left = new byte[] {3};
                    break;
            }
        }

        private static byte[] ApplyPattern(Area area, Side side) {
            SetPattern(area);
            switch (side) {
                case Side.Top:
                    return Pattern.Bot;
                case Side.Right:
                    return Pattern.Left;
                case Side.Bot:
                    return Pattern.Top;
                case Side.Left:
                    return Pattern.Right;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }

        private static byte[] Opposite(FollowerLocation loc) {
            var nodes = loc.GetNodes();
            var l = nodes.Length;
            var output = new byte[l];
            for (var i = 0; i < l; i++) {
                var n = nodes[i];
                switch (loc.Type) {
                    case Area.Field:
                        if (n == 0) output[i] = 5;
                        else if (n == 1) output[i] = 4;
                        else if (n == 2) output[i] = 7;
                        else if (n == 3) output[i] = 6;
                        else if (n == 4) output[i] = 1;
                        else if (n == 5) output[i] = 0;
                        else if (n == 6) output[i] = 3;
                        else if (n == 7) output[i] = 2;
                        break;
                    case Area.Road:
                    case Area.City:
                        if (n == 0) output[i] = 2;
                        else if (n == 1) output[i] = 3;
                        else if (n == 2) output[i] = 0;
                        else if (n == 3) output[i] = 1;
                        break;
                }
            }
            return output;
        }

        private static City GetCity(int id) { return Cities.FirstOrDefault(p => p.ID == id); }
        private static Road GetRoad(int id) { return Roads.FirstOrDefault(p => p.ID == id); }
        private static Field GetField(int id) { return Fields.FirstOrDefault(p => p.ID == id); }

        public static void Init() {
            var startingTile = Tile.GetStarting();
            var v = startingTile.IntVector();
            foreach (var loc in startingTile.GetLocations()) {
                switch (loc.Type) {
                    case Area.Field:
                        Fields.Add(new Field(Counter.Next(Area.Field)));
                        loc.Link = Counter.Fields;
                        break;
                    case Area.Road:
                        Roads.Add(new Road(Counter.Next(Area.Road)));
                        loc.Link = Counter.Roads;
                        break;
                    case Area.City:
                        Cities.Add(new City(Counter.Next(Area.City)));
                        loc.Link = Counter.Cities;
                        break;
                    case Area.Monastery:
                        Monasteries.Add(new Monastery());
                        loc.Link = Counter.Monasteries;
                        break;
                }
            }
            //foreach (var c in Cities) c.Debugger(Area.City);
            //foreach (var c in Roads) c.Debugger(Area.Road);
            //foreach (var c in Fields) c.Debugger(Area.Field);
        }

        public static void Check(GameObject putedTileGameObject) { // putedTile - координаты только что поставленного тайла
            var v = Tile.GetCoordinates(putedTileGameObject);
            var putedTile = Tile.Get(putedTileGameObject);
            // Проверка каждой из четырех сторон, начиная сверху
            for (byte i = 0; i < 4; i++) {
                var side = (Side) i;
                if (Tile.Nearby.Exist(v.X, v.Y, i)) {
                    var neighborTile = Tile.Nearby.GetLast();
                    foreach (var loc in neighborTile.GetLocations()) {
                        Merge(putedTile, side, loc);
                    }
                }
            }
        }

        private static void Merge(TileInfo pTile, Side side, FollowerLocation nLoc) {
            var pattern = ApplyPattern(nLoc.Type, side);
            foreach (var pLoc in pTile.GetLocations()) {
                if (pLoc.Type == nLoc.Type) {
                    if (nLoc.ContainsAnyOf(pattern)) { // Содержат ли противоположные стороны элементы, которые можно соединить?
                        if (pLoc.ContainsAnyOf(Opposite(nLoc))) { // Соприкасаются ли стороны?
                            Debug.Log("Puted: " + ArrayToString(pLoc.GetNodes()) +" as " + pLoc.Type + "; Connected: " +
                                      ArrayToString(nLoc.GetNodes()) + " as " + nLoc.Type + " pattern = " +
                                      ArrayToString(pattern) + " side = " + side);
                        }
                    }

                }

            }
        }

    }
}