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
            public static int Cities { get; private set; }
            public static int Roads { get; private set; }
            public static int Fields { get; private set; }
            public static int Monasteries { get; private set; }

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

        public static readonly List<City> Cities = new List<City>();
        public static readonly List<Road> Roads = new List<Road>();
        public static readonly List<Field> Fields = new List<Field>();
        public static readonly List<Monastery> Monasteries = new List<Monastery>();

        public static City LastCity { get; set; }
        public static Road LastRoad { get; set; }
        public static Field LastField { get; set; }

        public static string ArrayToString(byte[] array) {
            return array.Length == 0 ? "Array is empty!" : array.Aggregate(string.Empty, (current, a) => current + a);
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
        private static byte[] Opposite(Location loc) {
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
        public static City GetCity(Location loc) {
            var city = Cities.FirstOrDefault(p => p.ID == loc.Link);
            if (city != null) return city;
            Cities.Add(new City(Counter.Next(Area.City), loc.Parent.IntVector(), loc));
            loc.Link = Counter.Cities;
            return Cities.Last();
        }
        public static Road GetRoad(Location loc) {
            var road = Roads.FirstOrDefault(p => p.ID == loc.Link);
            if (road != null) return road;
            Roads.Add(new Road(Counter.Next(Area.Road), loc.Parent.IntVector(), loc));
            loc.Link = Counter.Roads;
            return Roads.Last();
        }
        public static Field GetField(Location loc) {
            var field = Fields.FirstOrDefault(p => p.ID == loc.Link);
            if (field != null) return field;
            Fields.Add(new Field(Counter.Next(Area.Field), loc.Parent.IntVector()));
            loc.Link = Counter.Fields;
            return Fields.Last();
        }
        public static Monastery GetMonastery(Location loc) {
            var monastery = Monasteries.FirstOrDefault(p => p.ID == loc.Link);
            return monastery;
        }
        public static Construction GetConstruction(Location loc) {
            switch (loc.Type) {
                case Area.Field:
                    return GetField(loc);
                case Area.Road:
                    return GetRoad(loc);
                case Area.City:
                    return GetCity(loc);
            }
            return null;
        }

        public static void Assimilate(GameObject putedTileGameObject, PlayerColor founder) { // putedTile - координаты только что поставленного тайла
            var v = Tile.GetCoordinates(putedTileGameObject);
            var putedTile = Tile.Get(putedTileGameObject);
            var freeSides = new List<byte>();
            // Проверка каждой из четырех сторон, начиная сверху
            for (byte i = 0; i < 4; i++) {
                var side = (Side) i;
                if (Tile.Nearby.Exist(v, side)) {
                    var neighborTile = Tile.Nearby.GetLast();
                    foreach (var loc in neighborTile.GetLocations()) {
                        Connect(putedTile, side, loc, founder);
                    }
                } else {
                    freeSides.Add(i);
                }
            }
            Create(putedTile, freeSides);
            MonasteriesChecker(putedTile);
            //Debug.Log("[=======================] NEXT TURN [=======================]");
            LogConstructions();
        }

        public static void SetOwner(Location construct) {
            //Net.Client.ChatMessage("" + construct.GetOwner());
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
                case Area.Monastery:
                    GetMonastery(construct).SetOwner(construct);
                    break;
            }
        }

        private static void LogConstructions() {
            //if (true) return;
            //foreach (var c in Cities) c.Debugger();
            //foreach (var c in Roads) c.Debugger();
            //foreach (var c in Fields) c.Debugger();
            //foreach (var c in Monasteries) c.Debugger();
            //Debug.Log("[======================================]");
        }

        public static void Init() {
            var startingTile = Tile.GetStarting();
            var v = startingTile.IntVector();
            foreach (var loc in startingTile.GetLocations()) {
                Add(loc, v);
            }
            LogConstructions();
        }

        private static void Add(Location loc, Cell v) {
            switch (loc.Type) {
                case Area.Field:
                    Fields.Add(new Field(Counter.Next(Area.Field), v));
                    loc.Link = Counter.Fields;
                    break;
                case Area.Road:
                    Roads.Add(new Road(Counter.Next(Area.Road), v, loc));
                    loc.Link = Counter.Roads;
                    break;
                case Area.City:
                    Cities.Add(new City(Counter.Next(Area.City), v, loc));
                    loc.Link = Counter.Cities;
                    break;
                case Area.Monastery:
                    if (loc.Link != -1) return;
                    Monasteries.Add(new Monastery(Counter.Next(Area.Monastery), loc));
                    loc.Link = Counter.Monasteries;
                    break;
            }
        }

        private static byte[] FindCommonNodes(Location pLoc, Location nLoc, Side nSide) {
            var pattern = ApplyPattern(pLoc.Type, Tile.Nearby.GetOppositeSide(nSide));
            var reversedNLoc = Opposite(nLoc);
            return (from pNode in pLoc.GetNodes()
                    let founded = reversedNLoc.Any(nNode => pNode == nNode)
                    where founded
                    where pattern.Any(p => pNode == p)
                    select pNode).ToArray();
        }

        private static void MonasteriesChecker(TileInfo pivotTile) {
            //Debug.Log("pivot " + pivotTile.IntVector().XY());
            var corner = pivotTile.IntVector().CornerLeftBot();
            for (var iX = 0; iX < 3; iX++) {
                for (var iY = 0; iY < 3; iY++) {
                    //var d = new Cell(corner, iX, iY);
                    //Debug.logger.Log("X" + d.X + ";Y" + d.Y);
                    if (!Tile.Exist(new Cell(corner, iX, iY))) continue;
                    foreach (var loc in Tile.LastCheckedTile.GetLocations()) {
                        if (loc.Type != Area.Monastery) continue;
                        GetMonastery(loc).CalcSurroundings();
                    }
                }
            }
        }


        private static void Connect(TileInfo pTile, Side side, Location nLoc, PlayerColor founder) {
            //if (nLoc.Type == Area.Monastery) GetMonastery(nLoc).CalcSurroundings();
            foreach (var pLoc in pTile.GetLocations()) {

                if (pLoc.Type == Area.Monastery) {
                    Add(pLoc, pTile.IntVector());
                    continue;
                }

                if (pLoc.Type != nLoc.Type) continue;
                var commonNodes = FindCommonNodes(pLoc, nLoc, side);
                if (commonNodes.Length == 0) continue;
                Link(pLoc, nLoc, founder);
            }
            LastField?.RecalcBarn();
        }

        private static void Create(TileInfo tile, List<byte> freeSides) {
            foreach (var loc in tile.GetLocations()) {
                var pattern = ApplyPattern(loc.Type, freeSides);
                if (loc.Type == Area.Monastery) continue;
                if (loc.Conform(pattern)) {
                    Add(loc, tile.IntVector());
                }
            }
        }

        private static void Link(Location p, Location n, PlayerColor founder) {
            switch (p.Type) {
                case Area.Field:
                    var field = GetField(n);
                    field.Add(p, founder);
                    LastField = field;
                    break;
                case Area.Road:
                    var road = GetRoad(n);
                    road.Add(p, founder);
                    LastRoad = road;
                    break;
                case Area.City:
                    var city = GetCity(n);
                    city.Add(p, founder);
                    LastCity = city;
                    break;
                case Area.Monastery:
                    break;
            }
        }
    }
}