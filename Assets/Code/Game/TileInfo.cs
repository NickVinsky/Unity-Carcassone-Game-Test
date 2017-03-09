using System.Collections.Generic;
using Code.Game.FollowerSubs;
using UnityEngine;

namespace Code.Game {
    public class TileInfo : MonoBehaviour {

        public int X, Y;

        public int Type;
        private FollowerInfo _follower = new FollowerInfo();

        private readonly Area[] _side = {Area.Empty, Area.Empty, Area.Empty, Area.Empty};
        public sbyte Rotates { get; set; } // 0 - 0 grad, 1 - 90 grad, 2 - 180 grad, 3 - 270 grad ; Clockwise

        //public bool[] Field = new bool[8];

        public Area GetSide(int side) { return _side[side]; }

        public void InitTile(int type) {
            Type = type;
            switch (Type) {
                case 0:
                    break;
                case 1:
                    AddMonastery();
                    AddFields("01234567");
                    break;
                case 2:
                    AddRoads("2");
                    AddMonastery();
                    AddFields("0134567");
                    break;
                case 3:
                    AddCities("0123");
                    break;
                case 4:
                case 5:
                    AddCities("013");
                    break;
                case 6:
                case 7:
                    AddCities("013");
                    AddRoads("2");
                    break;
                case 8:
                case 9:
                case 14:
                    _side[(int) Side.Top] = Area.City;
                    _side[(int) Side.Left] = Area.City;
                    break;
                case 10:
                case 11:
                    _side[(int) Side.Top] = Area.City;
                    _side[(int) Side.Right] = Area.Road;
                    _side[(int) Side.Bot] = Area.Road;
                    _side[(int) Side.Left] = Area.City;
                    break;
                case 12:
                case 13:
                    _side[(int) Side.Right] = Area.City;
                    _side[(int) Side.Left] = Area.City;
                    break;
                case 15:
                    _side[(int) Side.Top] = Area.City;
                    _side[(int) Side.Bot] = Area.City;
                    break;
                case 16:
                    _side[(int) Side.Top] = Area.City;
                    break;
                case 17:
                    _side[(int) Side.Top] = Area.City;
                    _side[(int) Side.Bot] = Area.Road;
                    _side[(int) Side.Left] = Area.Road;
                    break;
                case 18:
                    _side[(int) Side.Top] = Area.City;
                    _side[(int) Side.Right] = Area.Road;
                    _side[(int) Side.Bot] = Area.Road;
                    break;
                case 19:
                    _side[(int) Side.Top] = Area.City;
                    _side[(int) Side.Right] = Area.Road;
                    _side[(int) Side.Bot] = Area.Road;
                    _side[(int) Side.Left] = Area.Road;
                    break;
                case 20:
                    _side[(int) Side.Top] = Area.City;
                    _side[(int) Side.Right] = Area.Road;
                    _side[(int) Side.Left] = Area.Road;
                    break;
                case 21:
                    _side[(int) Side.Top] = Area.Road;
                    _side[(int) Side.Bot] = Area.Road;
                    break;
                case 22:
                    _side[(int) Side.Bot] = Area.Road;
                    _side[(int) Side.Left] = Area.Road;
                    break;
                case 23:
                    _side[(int) Side.Right] = Area.Road;
                    _side[(int) Side.Bot] = Area.Road;
                    _side[(int) Side.Left] = Area.Road;
                    break;
                case 24:
                    _side[(int) Side.Top] = Area.Road;
                    _side[(int) Side.Right] = Area.Road;
                    _side[(int) Side.Bot] = Area.Road;
                    _side[(int) Side.Left] = Area.Road;
                    break;
            }
        }

        private void AddMonastery() {
            _follower.AddLocation(Area.Monastery);
        }

        private void AddCities(string s) {
            var nodes = new List<byte>();
            foreach (var c in s) {
                var i = (int) c;
                _side[i] = Area.City;
                nodes.Add((byte) i);
            }
            _follower.AddLocation(Area.City, nodes);
        }

        private void AddRoads(string s) {
            var nodes = new List<byte>();
            foreach (var c in s) {
                var i = (int) c;
                _side[i] = Area.Road;
                nodes.Add((byte) i);
            }
            _follower.AddLocation(Area.Road, nodes);
        }

        private void AddFields(string s) {
            var nodes = new List<byte>();
            var lastChar = 'E';
            foreach (var c in s) {
                var i = (byte) c;
                nodes.Add(i);
                if (lastChar != 'E') {
                    var last = (byte) lastChar;
                    var sum = last + i;
                    if (last + 1 == i) {
                        switch (sum) {
                            case 1:
                                if (_follower.SideFree(0)) _side[0] = Area.Field;
                                break;
                            case 5:
                                if (_follower.SideFree(1)) _side[1] = Area.Field;
                                break;
                            case 9:
                                if (_follower.SideFree(2)) _side[2] = Area.Field;
                                break;
                            case 13:
                                if (_follower.SideFree(3)) _side[3] = Area.Field;
                                break;
                        }
                    }
                }
                lastChar = c;
            }
            _follower.AddLocation(Area.Field, nodes);
        }
    }
}
