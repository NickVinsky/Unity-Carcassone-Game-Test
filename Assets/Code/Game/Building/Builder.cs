using System;
using System.Collections.Generic;
using System.Linq;
using Code.Game.Data;
using UnityEngine;

namespace Code.Game.Building {
    public static class Builder {
        private static List<City> _cities = new List<City>();
        private static List<Road> _roads = new List<Road>();
        private static List<Field> _fields = new List<Field>();
        private static List<Monastery> _monasteries = new List<Monastery>();

        public static List<City> Cities {get { return _cities; }}
        public static List<Road> Roads {get { return _roads; }}
        public static List<Field> Fields {get { return _fields;}}
        public static List<Monastery> Monasteries {get { return _monasteries; }}

        private static byte[] GetNodes(Area type, Cell neighbor, Cell source, byte side) {
            switch (type) {
                case Area.Field:
                    return (from f in Fields where f.Exist(neighbor) select f.AttachedTo(side, source)).FirstOrDefault();
                case Area.Road:
                    return (from r in Roads where r.Exist(neighbor) select r.AttachedTo(side, source)).FirstOrDefault();
                case Area.City:
                    return (from c in Cities where c.Exist(neighbor) select c.AttachedTo(side, source)).FirstOrDefault();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static void Init() {
            var startingTile = Tile.GetStarting();
            var v = startingTile.IntVector();
            foreach (var loc in startingTile.GetLocations()) {
                switch (loc.Type) {
                    case Area.Field:
                        Fields.Add(new Field(v, loc.GetNodes()));
                        break;
                    case Area.Road:
                        Roads.Add(new Road(v, loc.GetNodes()));
                        break;
                    case Area.City:
                        Cities.Add(new City(v, loc.GetNodes()));
                        break;
                    case Area.Monastery:
                        Monasteries.Add(new Monastery());
                        break;
                }
            }
            foreach (var c in Cities) c.Debugger(Area.City);
            foreach (var c in Roads) c.Debugger(Area.Road);
            foreach (var c in Fields) c.Debugger(Area.Field);
        }

        public static void Check(GameObject cell) { // Cell - координаты только что поставленного тайла
            var v = Tile.GetCoordinates(cell);
            // Проверка каждой из четырех сторон, начиная сверху
            for (byte i = 0; i < 3; i++) {
                if (Tile.Nearby.Exist(v.X, v.Y, i)) {
                    var neighborTile = Tile.Nearby.GetLast();
                    foreach (var loc in neighborTile.GetLocations()) {
                        var nodes = GetNodes(loc.Type, v, neighborTile.IntVector(), i);
                    }
                }
            }
        }

    }
}