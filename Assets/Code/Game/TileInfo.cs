using System.Collections.Generic;
using System.Net.Sockets;
using Code.Game.Data;
using Code.Game.FollowerSubs;
using UnityEngine;
using LocationInfo = Code.Game.FollowerSubs.LocationInfo;

namespace Code.Game {
    public class TileInfo : MonoBehaviour {

        public int X, Y;

        public int Type;
        private District _follower = new District();

        private readonly Area[] _side = {Area.Empty, Area.Empty, Area.Empty, Area.Empty};

        public sbyte Rotates { get; set; } // 0 - 0 grad, 1 - 90 grad, 2 - 180 grad, 3 - 270 grad ; Clockwise

        //public bool[] Field = new bool[8];

        public Area GetSide(int side) { return _side[side]; }

        public List<Location> GetLocations() { return _follower.GetLocations(); }

        public Location GetLocation(byte id) { return _follower.GetLocation(id); }

        public Cell IntVector() {
            return new Cell(X, Y);
        }

        private byte[] A(byte a) { return new[] {a}; }
        private byte[] A(byte a, byte b) { return new[] {a, b}; }
        private byte[] A(byte a, byte b, byte c) { return new[] {a, b, c}; }
        private byte[] A(byte a, byte b, byte c, byte d) { return new[] {a, b, c, d}; }

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
                    AddCities("0123", 0f, 0f, true);
                    break;
                case 4:
                    AddCities("013", 0f, 0.2f); // id# 0
                    AddFields("45", 0.052f, -0.302f, A(0)); // linket to city#0 ^^
                    break;
                case 5:
                    AddCities("013", 0f, 0.2f, true);
                    AddFields("45", 0.052f, -0.302f, A(0));
                    break;
                case 6:
                    AddCities("013", 0f, 0.2f);
                    AddRoads("2", 0.024f, -0.349f);
                    AddFields("4", 0.25f, -0.444f, A(0));
                    AddFields("5", -0.206f, -0.433f, A(0));
                    break;
                case 7:
                    AddCities("013", 0f, 0.2f, true);
                    AddRoads("2", 0.024f, -0.349f);
                    AddFields("4", 0.25f, -0.444f, A(0));
                    AddFields("5", -0.206f, -0.433f, A(0));
                    break;
                case 8:
                    AddCities("03", -0.144f, 0.395f);
                    AddFields("2345", 0.128f, -0.051f, A(0));
                    break;
                case 9:
                    AddCities("03", -0.144f, 0.395f, true);
                    AddFields("2345", 0.128f, -0.051f, A(0));
                    break;
                case 10:
                    AddCities("03", -0.144f, 0.395f);
                    AddRoads("12", 0.075f, -0.244f);
                    AddFields("25", 0.195f, 0.122f, A(0));
                    AddFields("34", 0.316f, -0.244f);
                    break;
                case 11:
                    AddCities("03", -0.144f, 0.395f, true);
                    AddRoads("12", 0.075f, -0.244f);
                    AddFields("25", 0.195f, 0.122f, A(0));
                    AddFields("34", 0.316f, -0.244f);
                    break;
                case 12:
                    AddCities("13", -0.064f, 0.083f);
                    AddFields("01", 0.078f, 0.464f, A(0));
                    AddFields("45", 0.042f, -0.31f, A(0));
                    break;
                case 13:
                    AddCities("13", -0.064f, 0.083f, true);
                    AddFields("01", 0.078f, 0.464f, A(0));
                    AddFields("45", 0.042f, -0.31f, A(0));
                    break;
                case 14:
                    AddCities("0", -0.021f, 0.423f);
                    AddCities("3", -0.477f, 0.05f);
                    AddFields("2345", 0.095f, -0.089f, A(0, 1));
                    break;
                case 15:
                    AddCities("0", -0.021f, 0.423f);
                    AddCities("2", 0f, -0.336f);
                    AddFields("2367", 0f, 0f, A(0, 1));
                    break;
                case 16:
                    AddCities("0", -0.021f, 0.423f);
                    AddFields("234567", 0f, -0.073f, A(0));
                    break;
                case 17:
                    AddCities("0", -0.102f, 0.436f);
                    AddRoads("23", -0.089f, -0.113f);
                    AddFields("2347", 0.221f, 0.005f, A(0));
                    AddFields("56", -0.293f, -0.279f);
                    break;
                case 18:
                    AddCities("0", 0.005f, 0.449f);
                    AddRoads("12", 0.066f, -0.1f);
                    AddFields("2567", -0.216f, -0.012f, A(0));
                    AddFields("34", 0.286f, -0.268f);
                    break;
                case 19:
                    AddCities("0", 0.023f, 0.434f);
                    AddRoads("1", 0.278f, -0.012f);
                    AddRoads("2", -0.02f, -0.274f);
                    AddRoads("3", -0.236f, 0.043f);
                    AddFields("27", 0.019f, 0.131f, A(0));
                    AddFields("34", 0.279f, -0.284f);
                    AddFields("56", -0.302f, -0.268f);
                    break;
                case 20:
                    AddCities("0", 0.023f, 0.434f);
                    AddRoads("13", 0.06f, 0.094f);
                    AddFields("27", 0.357f, 0.196f, A(0));
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


                // Inns And Cathedrals
                case 25:
                    AddMonastery();
                    AddRoads("1", 0.389f, 0.086f);
                    AddRoads("3", -0.411f, 0.027f);
                    break;
                case 26:
                    AddCathedral(0.298f, -0.211f);
                    break;
                case 27:
                    AddCities("0", -0.015f, 0.439f);
                    AddCities("1", 0.463f, 0.078f);
                    AddCities("2", 0f, -0.395f);
                    AddCities("3", -0.477f, 0.038f);
                    AddFields("", -0.016f, 0.037f, A(0, 1, 2, 3));
                    break;
                case 28:
                    AddCities("0", 0f, 0.474f);
                    AddCities("1", 0.463f, 0.078f);
                    AddCities("3", -0.477f, 0.038f);
                    AddFields("45", -0.005f, -0.167f, A(0, 1, 2));
                    break;
                case 29:
                    AddCities("0", 0f, 0.449f);
                    AddCities("2", 0f, -0.409f);
                    AddRoads("1", 0.191f, 0.036f);
                    AddRoads("3", -0.252f, 0f);
                    AddFields("2", 0.444f, 0.213f, A(0));
                    AddFields("3", 0.442f, -0.229f, A(1));
                    AddFields("6", -0.475f, -0.219f, A(1));
                    AddFields("7", -0.454f, 0.211f, A(0));
                    break;
                case 30:
                    AddCities("03", -0.351f, 0.357f, true);
                    AddCities("2", 0f, -0.409f);
                    AddFields("23", 0.202f, -0.005f, A(0, 1));
                    break;
                case 31:
                    AddCities("0", -0.012f, 0.305f);
                    AddFields("23", 0.442f, 0.114f, A(0));
                    AddFields("4567", -0.303f, -0.256f, A(0));
                    break;
                case 32:
                    AddCities("0", 0.027f, 0.445f);
                    AddRoads("2", 0.01f, -0.092f);
                    AddFields("234", 0.284f, -0.055f, A(0));
                    AddFields("567", -0.292f, -0.098f, A(0));
                    break;
                case 33:
                    AddCities("03", -0.353f, 0.376f);
                    AddRoads("1", 0.112f, 0.013f);
                    AddFields("2", 0.41f, 0.215f, A(0));
                    AddFields("345", 0.19f, -0.27f, A(0));
                    break;
                case 34:
                    AddCities("13", 0f, 0.055f, true);
                    AddRoads("0", 0.277f, 0.511f);
                    AddRoads("2", 0f, 0.5f);
                    AddFields("0", -0.236f, 0.511f, A(0));
                    AddFields("1", 0.277f, 0.511f, A(0));
                    AddFields("4", 0.254f, -0.408f, A(0));
                    AddFields("5", -0.267f, -0.407f, A(0));
                    break;
                case 35:
                    AddCities("03", -0.309f, 0.309f);
                    AddRoads("2", -0.036f, -0.21f, true);
                    AddFields("234", 0.459f, -0.232f, A(0));
                    AddFields("5", -0.196f, -0.453f, A(0));
                    break;
                case 36:
                    AddCities("0", 0f, 0.42f);
                    AddRoads("23", -0.141f, -0.176f, true);
                    AddFields("2347", 0.454f, -0.184f, A(0));
                    AddFields("56", -0.217f, -0.426f);
                    break;
                case 37:
                    AddCities("03", -0.345f, 0.336f, true);
                    AddRoads("12", 0.184f, -0.164f, true);
                    AddFields("25", -0.181f, -0.435f, A(0));
                    AddFields("34", 0.356f, -0.391f);
                    break;
                case 38:
                    AddRoads("1", 0.293f, -0.032f, true);
                    AddRoads("2", 0f, -0.285f);
                    AddRoads("3", -0.261f, 0.027f);
                    AddFields("0127", -0.188f, 0.293f);
                    AddFields("34", 0.345f, -0.364f);
                    AddFields("56", -0.352f, -0.308f);
                    break;
                case 39:
                    AddRoads("13", 0f, 0f, true);
                    AddFields("0127", 0f, -0.3f);
                    AddFields("3456", 0.233f, 0.409f);
                    break;
                case 40:
                    AddRoads("23", -0.032f, 0f, true);
                    AddFields("012347", -0.218f, 0.297f);
                    AddFields("56", -0.275f, -0.281f);
                    break;
                case 41:
                    AddRoads("01", 0.157f, 0.064f);
                    AddRoads("23", -0.27f, -0.093f);
                    AddFields("12", 0.3f, 0.3f);
                    AddFields("0347", -0.272f, 0.258f);
                    AddFields("56", -0.375f, -0.402f);
                    break;


                // Kings
                case 52:
                    AddCities("0", -0.03f, 0.4f);
                    AddMonastery(-0.069f, -0.126f);
                    AddFields("234567", 0.358f, -0.208f, A(0));
                    break;
                case 53:
                    AddCities("1", 0.448f, 0.052f);
                    AddRoads("0", 0.022f, 0.213f);
                    AddRoads("23", -0.162f, -0.144f);
                    AddFields("1", 0.218f, 0.462f, A(0));
                    AddFields("047", -0.372f, 0.334f, A(0));
                    AddFields("56", -0.385f, -0.375f);
                    break;
                case 54:
                    AddCities("1", 0.448f, 0.052f);
                    AddRoads("0", 0.01f, 0.216f);
                    AddFields("1", 0.218f, 0.462f, A(0));
                    AddFields("04567", -0.186f, -0.111f, A(0));
                    break;
                case 55:
                    AddCities("03", -0.195f, 0.225f);
                    AddRoads("2", -0.02f, -0.264f);
                    AddRoads("1", 0.283f, 0.127f);
                    AddFields("2", 0.477f, 0.282f, A(0));
                    AddFields("3", 0.467f, -0.174f, A(0));
                    AddFields("4", 0.222f, -0.452f, A(0));
                    AddFields("5", -0.25f, -0.432f, A(0));
                    break;
                case 56:
                    AddCities("02", 0f, 0.445f);
                    AddCities("13", 0.03f, 0.114f);
                    break;

                //Traders And Builders
                case 57:
                    AddMonastery();
                    AddRoads("1", 0.396f, 0.041f);
                    AddRoads("2", -0.015f, -0.378f);
                    AddRoads("3", -0.43f, 0f);
                    break;
                case 58:
                    AddCities("0", 0f, 0.44f);
                    AddRoads("1", 0.321f, 0.03f);
                    AddFields("2", 0.472f, 0.264f, A(0));
                    AddFields("3456", -0.163f, -0.179f, A(0));
                    break;
                case 59:
                    AddCities("0", 0f, 0.44f);
                    AddRoads("2", 0.005f, -0.211f);
                    AddRoads("3", -0.382f, 0.055f);
                    AddFields("234", 0.295f, -0.3f);
                    AddFields("56", -0.279f, -0.284f);
                    AddFields("7", -0.476f, 0.28f);
                    break;
                case 60:
                    AddRoads("02", 0f, 0.44f);
                    AddRoads("13", 0.192f, 0.067f);
                    AddFields("12", 0.357f, 0.378f);
                    AddFields("34", 0.261f, -0.308f);
                    AddFields("56", -0.3f, -0.304f);
                    AddFields("70", -0.386f, 0.381f);
                    break;

                //Custom Tiles
                case 81:
                    AddRoads("0", 0.006f, 0.406f, true);
                    AddFields("01234567", 0.286f, -0.268f);
                    break;
                case 82:
                    AddRoads("0123", 0.006f, 0.406f);
                    AddFields("12", 0.266f, 0.302f);
                    AddFields("34", 0.286f, -0.268f);
                    AddFields("56", -0.31f, -0.283f);
                    AddFields("70", -0.293f, 0.372f);
                    break;
            }
        }

        private void AddMonastery(float x = 0f, float y = 0f) {
            var meeplePos = new Vector2(x, y);
            var newLoc = new LocationInfo {
                Type = Area.Monastery,
                MeeplePos = meeplePos,
            };

            _follower.AddLocation(this, newLoc);
        }

        private void AddCathedral(float x, float y) {
            var s = "0123";
            var meeplePos = new Vector2(x, y);
            var nodes = new List<byte>();
            foreach (var c in s) {
                var i = (int) char.GetNumericValue(c);
                _side[i] = Area.City;
                nodes.Add((byte) i);
            }

            var newLoc = new LocationInfo {
                Type = Area.City,
                MeeplePos = meeplePos,
                Nodes = nodes,
                HasCathedral = true
            };

            _follower.AddLocation(this, newLoc);
        }

        private void AddCities(string s, float x, float y, bool coatOfArms = false) {
            var meeplePos = new Vector2(x, y);
            var nodes = new List<byte>();
            foreach (var c in s) {
                var i = (int) char.GetNumericValue(c);
                _side[i] = Area.City;
                nodes.Add((byte) i);
            }

            var newLoc = new LocationInfo {
                Type = Area.City,
                MeeplePos = meeplePos,
                Nodes = nodes,
                CoatOfArms = coatOfArms
            };

            _follower.AddLocation(this, newLoc);
        }

        private void AddRoads(string s, float x, float y, bool hasInn = false) {
            var meeplePos = new Vector2(x, y);
            var nodes = new List<byte>();
            foreach (var c in s) {
                var i = (int) char.GetNumericValue(c);
                _side[i] = Area.Road;
                nodes.Add((byte) i);
            }

            var newLoc = new LocationInfo {
                Type = Area.Road,
                MeeplePos = meeplePos,
                Nodes = nodes,
                HasInn = hasInn
            };

            _follower.AddLocation(this, newLoc);
        }

        private void AddFields(string s, float x, float y, byte[] linkToCity = null) {
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

            var newLoc = new LocationInfo {
                Type = Area.Field,
                MeeplePos = meeplePos,
                Nodes = nodes,
                LinkedToCity = linkToCity
            };

            _follower.AddLocation(this, newLoc);
        }

        public void AssignFollower(byte id, Follower type) {
            _follower.HideExcept(id, type);
        }

        public void AssignOpponentFollower(PlayerColor owner, byte id, Follower type) {
            GameObject o = gameObject;
            _follower.Opponent(owner, id, type);
        }

        public void HideAll() {
            _follower.HideAll();
        }

        public void RemovePlacement(int constructID) {
            _follower.RemovePlacement(constructID);
        }

        public void ShowPossibleLocations(Follower type) {
            _follower.Show(Rotates, type);
            if (gameObject.transform.childCount == 0) MainGame.ChangeGameStage(GameStage.Finish);
        }

        public void ApplyRotation() {
            _follower.RotateNodes((byte) Rotates);
        }
    }
}
