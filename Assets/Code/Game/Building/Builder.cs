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
                        Fields++;
                        return Fields;
                    case Area.Road:
                        Roads++;
                        return Roads;
                    case Area.City:
                        Cities++;
                        return Cities;
                    case Area.Monastery:
                        Monasteries++;
                        return Monasteries;
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

        private static List<FollowerLocation> _temp = new List<FollowerLocation>();

        public static string ArrayToString(byte[] array) {
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

        private static byte[] ApplyPattern(Area area, List<byte> freeSides) {
            SetPattern(area);
            var l = 0;
            var pattern = new List<byte[]>();
            foreach (var side in freeSides) {
                switch ((Side) side) {
                    case Side.Bot:
                        pattern.Add(Pattern.Bot);
                        l += Pattern.Bot.Length;
                        break;
                    case Side.Left:
                        pattern.Add(Pattern.Left);
                        l += Pattern.Left.Length;
                        break;
                    case Side.Top:
                        pattern.Add(Pattern.Top);
                        l += Pattern.Top.Length;
                        break;
                    case Side.Right:
                        pattern.Add(Pattern.Right);
                        l += Pattern.Right.Length;
                        break;
                }
            }
            var output = new byte[l];
            var i = 0;
            foreach (var p in pattern) {
                foreach (var b in p) {
                    output[i] = b;
                    i++;
                }
            }
            return output;
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

        public static City GetCity(FollowerLocation loc) {
            var city = Cities.FirstOrDefault(p => p.ID == loc.Link);
            if (city != null) return city;
            Cities.Add(new City(Counter.Next(Area.City)));
            loc.Link = Counter.Cities;
            return Cities.Last();
        }

        public static Road GetRoad(FollowerLocation loc) {
            var road = Roads.FirstOrDefault(p => p.ID == loc.Link);
            if (road != null) return road;
            Roads.Add(new Road(Counter.Next(Area.Road)));
            loc.Link = Counter.Roads;
            return Roads.Last();
        }

        public static Field GetField(FollowerLocation loc) {
            var field = Fields.FirstOrDefault(p => p.ID == loc.Link);
            if (field != null) return field;
            Fields.Add(new Field(Counter.Next(Area.Field)));
            loc.Link = Counter.Fields;
            return Fields.Last();
        }

        public static void SetOwner(FollowerLocation construct) {
            switch (construct.Type) {
                case Area.Field:
                    GetField(construct).SetOwner(construct);
                    break;
                case Area.Road:
                    GetRoad(construct).SetOwner(construct);
                    break;
                case Area.City:
                    GetCity(construct).SetOwner(construct);
                    break;
            }
        }

        public static void Init() {
            var startingTile = Tile.GetStarting();
            var v = startingTile.IntVector();
            foreach (var loc in startingTile.GetLocations()) {
                Add(loc);
            }
            foreach (var c in Cities) c.Debugger(Area.City);
            foreach (var c in Roads) c.Debugger(Area.Road);
            foreach (var c in Fields) c.Debugger(Area.Field);
        }

        private static void Add(FollowerLocation loc) {
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

        public static void Check(GameObject putedTileGameObject) { // putedTile - координаты только что поставленного тайла
            var v = Tile.GetCoordinates(putedTileGameObject);
            var putedTile = Tile.Get(putedTileGameObject);
            var freeSides = new List<byte>();
            // Проверка каждой из четырех сторон, начиная сверху
            for (byte i = 0; i < 4; i++) {
                var side = (Side) i;
                if (Tile.Nearby.Exist(v.X, v.Y, i)) {
                    var neighborTile = Tile.Nearby.GetLast();
                    foreach (var loc in neighborTile.GetLocations()) {
                        Connect(putedTile, side, loc);
                    }
                    MergeTemp(putedTile.GetFilledLoc());
                } else {
                    freeSides.Add(i);
                }
            }
            Create(putedTile, freeSides);
            Debug.Log("/////////////////////////////////////////////////////////");
            foreach (var c in Cities) c.Debugger(Area.City);
            foreach (var c in Roads) c.Debugger(Area.Road);
            foreach (var c in Fields) c.Debugger(Area.Field);
        }

        private static void Connect(TileInfo pTile, Side side, FollowerLocation nLoc) {
            var pattern = ApplyPattern(nLoc.Type, side);
            foreach (var pLoc in pTile.GetLocations()) {
                if (pLoc.Type == nLoc.Type) {
                    if (pLoc.Type == Area.Monastery) continue;
                    //Debug.Log("result1 " + nLoc.Contains(pattern));
                    //Debug.Log("result2 " + pLoc.ContainsAnyOf(Opposite(nLoc)));

                    Debug.logger.Log(LogType.Warning, "Puted [" + ArrayToString(pLoc.GetNodes()) + "] ContainsAnyOf Neighbor[" + ArrayToString(nLoc.GetNodes()) + "]");
                    Debug.logger.Log(LogType.Warning, "Puted [" + ArrayToString(nLoc.GetNodes()) + "] ContainsAllOf Pattern[" + ArrayToString(pattern) + "]");
                    if (pLoc.Filled) {
                        _temp.Add(nLoc);
                        continue;
                    }
                    if (pLoc.ContainsAnyOf(nLoc.GetNodes()) && nLoc.ContainsAnyOf(pattern)) {
                        Link(pLoc, nLoc);
                        // [1] Содержат ли противоположные стороны элементы, которые можно соединить?
                        // [2] Соприкасаются ли стороны?
                        /*Debug.Log("[" + pLoc.Type + "] Puted: " + ArrayToString(pLoc.GetNodes()) + "; Neighbor: " +
                                  ArrayToString(nLoc.GetNodes()) + " pattern = " +
                                  ArrayToString(pattern) + " side = " + side);*/

                        //Debug.Log("==============================================================================");
                    }
                }
            }
        }

        private static void MergeTemp(FollowerLocation initiator) {
            if (_temp.Count == 0) return;
            var applyCorrection = false;
            var last = _temp.First().Link;
            foreach (var c in _temp) {
                GetField(c).Add(initiator);
                if (last != c.Link) applyCorrection = true;
            }
            if (applyCorrection) GetField(_temp.First()).Correction(_temp.Count);
            _temp.Clear();
        }

        private static void Create(TileInfo tile, List<byte> freeSides) {
            foreach (var loc in tile.GetLocations()) {
                var pattern = ApplyPattern(loc.Type, freeSides);
                if (loc.Type == Area.Monastery) continue;
                if (loc.ContainsOnly(pattern)) {
                    Add(loc);
                }
            }
        }

        private static void Link(FollowerLocation p, FollowerLocation n) {
            switch (p.Type) {
                case Area.Field:
                    GetField(n).Add(p);
                    break;
                case Area.Road:
                    GetRoad(n).Add(p);
                    break;
                case Area.City:
                    GetCity(n).Add(p);
                    break;
                case Area.Monastery:
                    break;
            }
        }
    }
}