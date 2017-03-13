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
            if (array.Length == 0) return "Array is empty!";
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

        private static byte[] Opposite(byte[] loc, Area type) {
            var nodes = loc;
            var l = nodes.Length;
            var output = new byte[l];
            for (var i = 0; i < l; i++) {
                var n = nodes[i];
                switch (type) {
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
            Cities.Add(new City(Counter.Next(Area.City), loc.Parent.IntVector()));
            loc.Link = Counter.Cities;
            return Cities.Last();
        }

        public static Road GetRoad(FollowerLocation loc) {
            var road = Roads.FirstOrDefault(p => p.ID == loc.Link);
            if (road != null) return road;
            Roads.Add(new Road(Counter.Next(Area.Road), loc.Parent.IntVector()));
            loc.Link = Counter.Roads;
            return Roads.Last();
        }

        public static Field GetField(FollowerLocation loc) {
            var field = Fields.FirstOrDefault(p => p.ID == loc.Link);
            if (field != null) return field;
            Fields.Add(new Field(Counter.Next(Area.Field), loc.Parent.IntVector()));
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

        private static void LogConstructions() {
            foreach (var c in Cities) c.Debugger(Area.City);
            foreach (var c in Roads) c.Debugger(Area.Road);
            foreach (var c in Fields) c.Debugger(Area.Field);
        }

        public static void Init() {
            var startingTile = Tile.GetStarting();
            var v = startingTile.IntVector();
            foreach (var loc in startingTile.GetLocations()) {
                Add(loc, v);
            }
            LogConstructions();
        }

        private static void Add(FollowerLocation loc, Cell v) {
            switch (loc.Type) {
                case Area.Field:
                    Fields.Add(new Field(Counter.Next(Area.Field), v));
                    loc.Link = Counter.Fields;
                    break;
                case Area.Road:
                    Roads.Add(new Road(Counter.Next(Area.Road), v));
                    loc.Link = Counter.Roads;
                    break;
                case Area.City:
                    Cities.Add(new City(Counter.Next(Area.City), v));
                    loc.Link = Counter.Cities;
                    break;
                case Area.Monastery:
                    Monasteries.Add(new Monastery());
                    loc.Link = Counter.Monasteries;
                    break;
            }
        }

        private static byte[] LimitOppositePattern(FollowerLocation loc, byte[] pattern) {
            var oppPattern = Opposite(pattern, loc.Type);
            var output = new List<byte>();
            foreach (var node in loc.GetNodes()) {
                bool founded = oppPattern.Any(p => node == p);
                if (founded) output.Add(node);
            }
            return output.ToArray();
        }

        private static byte[] FindCommonNodes(FollowerLocation pLoc, FollowerLocation nLoc, Side nSide) {
            var pattern = ApplyPattern(pLoc.Type, Tile.Nearby.GetOppositeSide(nSide));
            var reversedNLoc = Opposite(nLoc);
            var output = new List<byte>();
            foreach (var pNode in pLoc.GetNodes()) {
                bool founded = reversedNLoc.Any(nNode => pNode == nNode);
                if (!founded) continue;
                if (pattern.Any(p => pNode == p))
                    output.Add(pNode);
            }
            return output.ToArray();
        }

        public static void Check(GameObject putedTileGameObject) { // putedTile - координаты только что поставленного тайла
            var v = Tile.GetCoordinates(putedTileGameObject);
            var putedTile = Tile.Get(putedTileGameObject);
            var freeSides = new List<byte>();
            // Проверка каждой из четырех сторон, начиная сверху
            for (byte i = 0; i < 4; i++) {
                var side = (Side) i;
                if (Tile.Nearby.Exist(v, side)) {
                    var neighborTile = Tile.Nearby.GetLast();
                    foreach (var loc in neighborTile.GetLocations()) {
                        Connect(putedTile, side, loc);
                    }
                    //MergeTemp(putedTile.GetFilledLoc());
                } else {
                    freeSides.Add(i);
                }
            }
            Create(putedTile, freeSides);
            Debug.Log("[=======================] NEXT TURN [=======================]");
            LogConstructions();
        }


        private static void Connect(TileInfo pTile, Side side, FollowerLocation nLoc) {
            var pattern = ApplyPattern(nLoc.Type, side);
            foreach (var pLoc in pTile.GetLocations()) {
                if (pLoc.Type == nLoc.Type) {
                    if (pLoc.Type == Area.Monastery) continue;
                    //Debug.Log("result1 " + nLoc.Contains(pattern));
                    //Debug.Log("result2 " + pLoc.ContainsAnyOf(Opposite(nLoc)));

                    var commonNodes = FindCommonNodes(pLoc, nLoc, side);
                    if (commonNodes.Length == 0) {
                        //if (nLoc.Indexed) continue;
                        //Add(nLoc, pTile.IntVector());
                        //nLoc.Indexed = true;
                        //Debug.logger.Log(LogType.Warning, "Not indexed: " + ArrayToString(pLoc.GetNodes()));
                    } else {
                        Link(pLoc, nLoc);
                    }

                    /*var limitedPattern = LimitOppositePattern(pLoc, pattern);

                    Debug.Log(pLoc.Type + ": Checking pLoc " + ArrayToString(pLoc.GetNodes()) + ", nLoc " + ArrayToString(nLoc.GetNodes()) + ", with pattern " + ArrayToString(pattern) + ":");
                    Debug.Log("COMMON NODES: " + ArrayToString(FindCommonNodes(pLoc, nLoc, side)));
                    Debug.logger.Log(LogType.Warning, "Puted [" + ArrayToString(pLoc.GetNodes()) + "] ContainsAnyOf; Opposite Neighbor [" + ArrayToString(Opposite(nLoc)) + "]");
                    Debug.logger.Log(LogType.Warning, "Neighbor [" + ArrayToString(nLoc.GetNodes()) + "] ContainsAnyOf; Pattern[" + ArrayToString(pattern) + "]");
                    Debug.logger.Log(LogType.Warning, "Puted [" + ArrayToString(pLoc.GetNodes()) + "] Contains; limitedPattern [" + ArrayToString(limitedPattern) + "]");

                    if (pLoc.Filled) {
                        _temp.Add(nLoc);
                        continue;
                    }

                    if (pLoc.ContainsAnyOf(Opposite(nLoc))) {
                        Debug.Log("pLoc.ContainsAnyOf(Opposite(nLoc)) = " + pLoc.ContainsAnyOf(Opposite(nLoc)));
                        if (nLoc.ContainsAnyOf(pattern)) { // Соприкасаются ли стороны?
                            Debug.Log("nLoc.ContainsAnyOf(pattern) = " + nLoc.ContainsAnyOf(pattern));
                            if (pLoc.Contains(limitedPattern)) { // Содержат ли противоположные стороны элементы, которые можно соединить?
                                Link(pLoc, nLoc);
                                Debug.Log("[" + pLoc.Type + "] Puted: " + ArrayToString(pLoc.GetNodes()) + "; Neighbor: " +
                                          ArrayToString(nLoc.GetNodes()) + " pattern = " +
                                          ArrayToString(pattern) + " side = " + side);

                                Debug.logger.Log(LogType.Error, "СОВЕРШЕНО ОДНО СЛИЯНИЕ");
                            }
                        }
                    }*/
                }
            }
        }

        private static void MergeTemp(FollowerLocation initiator) {
            if (_temp.Count == 0) return;
            foreach (var c in _temp) {
                GetField(c).Add(initiator);
            }
            _temp.Clear();
        }

        private static void Create(TileInfo tile, List<byte> freeSides) {
            foreach (var loc in tile.GetLocations()) {
                var pattern = ApplyPattern(loc.Type, freeSides);
                if (loc.Type == Area.Monastery) continue;

                Debug.Log(loc.Type + ": FreeLocs " + ArrayToString(loc.GetNodes()) + ", with pattern " + ArrayToString(pattern) + ":");
                Debug.logger.Log(LogType.Warning, "Puted [" + ArrayToString(loc.GetNodes()) + "] Conform; Pattern[" + ArrayToString(pattern) + "] = " + loc.Conform(pattern));

                if (loc.Conform(pattern)) {
                    Add(loc, tile.IntVector());
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