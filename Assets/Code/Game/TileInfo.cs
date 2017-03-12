using System.Collections.Generic;
using Code.Game.Data;
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

        public List<FollowerLocation> GetLocations() { return _follower.GetLocations(); }

        public Cell IntVector() {
            return new Cell(X, Y);
        }

        public void InitTile(int type) {
            Type = type;
            switch (Type) {
                case 0:
                    break;
                case 1:
                    AddMonastery();
                    AddFields("01234567", 0.261f, -0.241f);
                    break;
                case 2:
                    AddRoads("2", -0.0033f, -0.282f);
                    AddMonastery();
                    AddFields("01234567", 0.279f, 0.271f);
                    break;
                case 3:
                    AddCities("0123", true, 0f, 0f);
                    break;
                case 4:
                    AddCities("013", 0f, 0.2f);
                    AddFields("45", 0.052f, -0.302f);
                    break;
                case 5:
                    AddCities("013", true, 0f, 0.2f);
                    AddFields("45", 0.052f, -0.302f);
                    break;
                case 6:
                    AddCities("013", 0f, 0.2f);
                    AddRoads("2", 0.024f, -0.349f);
                    AddFields("4", 0.25f, -0.444f);
                    AddFields("5", -0.206f, -0.433f);
                    break;
                case 7:
                    AddCities("013", true, 0f, 0.2f);
                    AddRoads("2", 0.024f, -0.349f);
                    AddFields("4", 0.25f, -0.444f);
                    AddFields("5", -0.206f, -0.433f);
                    break;
                case 8:
                    AddCities("03", -0.144f, 0.395f);
                    AddFields("2345", 0.128f, -0.051f);
                    break;
                case 9:
                    AddCities("03", true, -0.144f, 0.395f);
                    AddFields("2345", 0.128f, -0.051f);
                    break;
                case 10:
                    AddCities("03", -0.144f, 0.395f);
                    AddRoads("12", 0.075f, -0.244f);
                    AddFields("25", 0.195f, 0.122f);
                    AddFields("34", 0.316f, -0.244f);
                    break;
                case 11:
                    AddCities("03", true, -0.144f, 0.395f);
                    AddRoads("12", 0.075f, -0.244f);
                    AddFields("25", 0.195f, 0.122f);
                    AddFields("34", 0.316f, -0.244f);
                    break;
                case 12:
                    AddCities("13", -0.064f, 0.083f);
                    AddFields("01", 0.078f, 0.464f);
                    AddFields("45", 0.042f, -0.31f);
                    break;
                case 13:
                    AddCities("13", true, -0.064f, 0.083f);
                    AddFields("01", 0.078f, 0.464f);
                    AddFields("45", 0.042f, -0.31f);
                    break;
                case 14:
                    AddCities("0", -0.021f, 0.423f);
                    AddCities("3", -0.477f, 0.05f);
                    AddFields("2345", 0.095f, -0.089f);
                    break;
                case 15:
                    AddCities("0", -0.021f, 0.423f);
                    AddCities("2", 0f, -0.336f);
                    AddFields("2367", 0f, 0f);
                    break;
                case 16:
                    AddCities("0", -0.021f, 0.423f);
                    AddFields("234567", 0f, -0.073f);
                    break;
                case 17:
                    AddCities("0", -0.102f, 0.436f);
                    AddRoads("23", -0.089f, -0.113f);
                    AddFields("2347", 0.221f, 0.005f);
                    AddFields("56", -0.293f, -0.279f);
                    break;
                case 18:
                    AddCities("0", 0.005f, 0.449f);
                    AddRoads("12", 0.066f, -0.1f);
                    AddFields("2567", -0.216f, -0.012f);
                    AddFields("34", 0.286f, -0.268f);
                    break;
                case 19:
                    AddCities("0", 0.023f, 0.434f);
                    AddRoads("1", 0.278f, -0.012f);
                    AddRoads("2", -0.02f, -0.274f);
                    AddRoads("3", -0.236f, 0.043f);
                    AddFields("27", 0.019f, 0.131f);
                    AddFields("34", 0.279f, -0.284f);
                    AddFields("56", -0.302f, -0.268f);
                    break;
                case 20:
                    AddCities("0", 0.023f, 0.434f);
                    AddRoads("13", 0.06f, 0.094f);
                    AddFields("27", 0.357f, 0.196f);
                    AddFields("3456", -0.138f, -0.241f);
                    break;
                case 21:
                    AddRoads("02", 0f, -0.027f);
                    AddFields("1234", 0.279f, 0.219f);
                    AddFields("5670", -0.239f, -0.218f);
                    break;
                case 22:
                    AddRoads("23", -0.06f, -0.04f);
                    AddFields("012347", 0.279f, 0.219f);
                    AddFields("56", -0.239f, -0.218f);
                    break;
                case 23:
                    AddRoads("1", 0.389f, 0.104f);
                    AddRoads("2", -0.029f, -0.244f);
                    AddRoads("3", -0.37f, 0.073f);
                    AddFields("0127", -0.056f, 0.302f);
                    AddFields("34", 0.286f, -0.268f);
                    AddFields("56", -0.31f, -0.283f);
                    break;
                case 24:
                    AddRoads("0", 0.006f, 0.406f);
                    AddRoads("1", 0.373f, 0.015f);
                    AddRoads("2", -0.052f, -0.288f);
                    AddRoads("3", -0.37f, 0.073f);
                    AddFields("12", 0.266f, 0.302f);
                    AddFields("34", 0.286f, -0.268f);
                    AddFields("56", -0.31f, -0.283f);
                    AddFields("70", -0.293f, 0.372f);
                    break;
            }
        }

        private void AddMonastery() {
            _follower.AddLocation(Area.Monastery, GameRegulars.FollowerPositionCenter);
        }

        private void AddCities(string s, float x, float y) {
            var meeplePos = new Vector2(x, y);
            var nodes = new List<byte>();
            foreach (var c in s) {
                var i = (int) char.GetNumericValue(c);
                _side[i] = Area.City;
                nodes.Add((byte) i);
            }
            _follower.AddLocation(Area.City, nodes, meeplePos);
        }

        private void AddCities(string s, bool coatOfArms, float x, float y) {
            var meeplePos = new Vector2(x, y);
            var nodes = new List<byte>();
            foreach (var c in s) {
                var i = (int) char.GetNumericValue(c);
                _side[i] = Area.City;
                nodes.Add((byte) i);
            }
            _follower.AddLocation(Area.City, nodes, coatOfArms, meeplePos);
        }

        private void AddRoads(string s, float x, float y) {
            var meeplePos = new Vector2(x, y);
            var nodes = new List<byte>();
            foreach (var c in s) {
                var i = (int) char.GetNumericValue(c);
                _side[i] = Area.Road;
                nodes.Add((byte) i);
            }
            _follower.AddLocation(Area.Road, nodes, meeplePos);
        }

        private void AddFields(string s, float x, float y) {
            var meeplePos = new Vector2(x, y);
            var nodes = new List<byte>();
            var lastChar = 'E';
            foreach (var c in s) {
                var i = (byte) char.GetNumericValue(c);
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
            _follower.AddLocation(Area.Field, nodes, meeplePos);
        }

        public void AssignFollower(byte id) {
            _follower.HideExcept(id);
        }

        public void AssignOpponentFollower(PlayerColor owner, byte id) {
            GameObject o = gameObject;
            _follower.Opponent(o, owner, id);
        }

        public void HideAll() {
            _follower.HideAll();
        }

        public void ShowPossibleFollowersLocations(GameObject o) {
            _follower.Show(o, Rotates);
        }

        public void ApplyRotation() {
            _follower.RotateNodes((byte) Rotates);
        }

        public FollowerLocation GetFilledLoc() { return _follower.GetFilled(); }
    }
}
