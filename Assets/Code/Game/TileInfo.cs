using System.Collections.Generic;
using Code.Game.Data;
using Code.Game.FollowerSubs;
using UnityEngine;
using LocationInfo = Code.Game.FollowerSubs.LocationInfo;

namespace Code.Game {
    public class TileInfo : MonoBehaviour {

        public int X, Y;

        public int Type;
        public PlayerColor Founder { get; set; }
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
            const float k = 2.909f;
            switch (Type) {
                case 0:
                    break;
                case 1:
                    AddMonastery();
                    AddFields("01234567", 0.261f * k, -0.241f * k);
                    break;
                case 2:
                    AddRoads("2", -0.0033f * k, -0.282f * k);
                    AddMonastery();
                    AddFields("01234567", 0.279f * k, 0.271f * k);
                    break;
                case 3:
                    AddCities("0123", 0f, 0f, true);
                    break;
                case 4:
                    AddCities("013", 0f, 0.2f * k); // id# 0
                    AddFields("45", 0.052f * k, -0.302f * k, A(0)); // linket to city#0 ^^
                    break;
                case 5:
                    AddCities("013", 0f, 0.2f * k, true);
                    AddFields("45", 0.052f * k, -0.302f * k, A(0));
                    break;
                case 6:
                    AddCities("013", 0f, 0.2f * k);
                    AddRoads("2", 0.024f * k, -0.349f * k);
                    AddFields("4", 0.25f * k, -0.444f * k, A(0));
                    AddFields("5", -0.206f * k, -0.433f * k, A(0));
                    break;
                case 7:
                    AddCities("013", 0f, 0.2f * k, true);
                    AddRoads("2", 0.024f * k, -0.349f * k);
                    AddFields("4", 0.25f * k, -0.444f * k, A(0));
                    AddFields("5", -0.206f * k, -0.433f * k, A(0));
                    break;
                case 8:
                    AddCities("03", -0.144f * k, 0.395f * k);
                    AddFields("2345", 0.128f * k, -0.051f * k, A(0));
                    break;
                case 9:
                    AddCities("03", -0.144f * k, 0.395f * k, true);
                    AddFields("2345", 0.128f * k, -0.051f * k, A(0));
                    break;
                case 10:
                    AddCities("03", -0.144f * k, 0.395f * k);
                    AddRoads("12", 0.075f * k, -0.244f * k);
                    AddFields("25", 0.195f * k, 0.122f * k, A(0));
                    AddFields("34", 0.316f * k, -0.244f * k);
                    break;
                case 11:
                    AddCities("03", -0.144f * k, 0.395f * k, true);
                    AddRoads("12", 0.075f * k, -0.244f * k);
                    AddFields("25", 0.195f * k, 0.122f * k, A(0));
                    AddFields("34", 0.316f * k, -0.244f * k);
                    break;
                case 12:
                    AddCities("13", -0.064f * k, 0.083f * k);
                    AddFields("01", 0.078f * k, 0.464f * k, A(0));
                    AddFields("45", 0.042f * k, -0.31f * k, A(0));
                    break;
                case 13:
                    AddCities("13", -0.064f * k, 0.083f * k, true);
                    AddFields("01", 0.078f * k, 0.464f * k, A(0));
                    AddFields("45", 0.042f * k, -0.31f * k, A(0));
                    break;
                case 14:
                    AddCities("0", -0.021f * k, 0.423f * k);
                    AddCities("3", -0.477f * k, 0.05f * k);
                    AddFields("2345", 0.095f * k, -0.089f * k, A(0, 1));
                    break;
                case 15:
                    AddCities("0", -0.021f * k, 0.423f * k);
                    AddCities("2", 0f, -0.336f * k);
                    AddFields("2367", 0f, 0f, A(0, 1));
                    break;
                case 16:
                    AddCities("0", -0.021f * k, 0.423f * k);
                    AddFields("234567", 0f, -0.073f * k, A(0));
                    break;
                case 17:
                    AddCities("0", -0.102f * k, 0.436f * k);
                    AddRoads("23", -0.089f * k, -0.113f * k);
                    AddFields("2347", 0.221f * k, 0.005f * k, A(0));
                    AddFields("56", -0.293f * k, -0.279f * k);
                    break;
                case 18:
                    AddCities("0", 0.005f * k, 0.449f * k);
                    AddRoads("12", 0.066f * k, -0.1f * k);
                    AddFields("2567", -0.216f * k, -0.012f * k, A(0));
                    AddFields("34", 0.286f * k, -0.268f * k);
                    break;
                case 19:
                    AddCities("0", 0.023f * k, 0.434f * k);
                    AddRoads("1", 0.278f * k, -0.012f * k);
                    AddRoads("2", -0.02f * k, -0.274f * k);
                    AddRoads("3", -0.236f * k, 0.043f * k);
                    AddFields("27", 0.019f * k, 0.131f * k, A(0));
                    AddFields("34", 0.279f * k, -0.284f * k);
                    AddFields("56", -0.302f * k, -0.268f * k);
                    break;
                case 20:
                    AddCities("0", 0.023f * k, 0.434f * k);
                    AddRoads("13", 0.06f * k, 0.094f * k);
                    AddFields("27", 0.357f * k, 0.196f * k, A(0));
                    AddFields("3456", -0.138f * k, -0.241f * k);
                    break;
                case 21:
                    AddRoads("02", 0f, -0.027f * k);
                    AddFields("1234", 0.279f * k, 0.219f * k);
                    AddFields("5670", -0.239f * k, -0.218f * k);
                    break;
                case 22:
                    AddRoads("23", -0.06f * k, -0.04f * k);
                    AddFields("012347", 0.279f * k, 0.219f * k);
                    AddFields("56", -0.239f * k, -0.218f * k);
                    break;
                case 23:
                    AddRoads("1", 0.389f * k, 0.104f * k);
                    AddRoads("2", -0.029f * k, -0.244f * k);
                    AddRoads("3", -0.37f * k, 0.073f * k);
                    AddFields("0127", -0.056f * k, 0.302f * k);
                    AddFields("34", 0.286f * k, -0.268f * k);
                    AddFields("56", -0.31f * k, -0.283f * k);
                    break;
                case 24:
                    AddRoads("0", 0.006f * k, 0.406f * k);
                    AddRoads("1", 0.373f * k, 0.015f * k);
                    AddRoads("2", -0.052f * k, -0.288f * k);
                    AddRoads("3", -0.37f * k, 0.073f * k);
                    AddFields("12", 0.266f * k, 0.302f * k);
                    AddFields("34", 0.286f * k, -0.268f * k);
                    AddFields("56", -0.31f * k, -0.283f * k);
                    AddFields("70", -0.293f * k, 0.372f * k);
                    break;


                // Inns And Cathedrals
                case 25:
                    AddMonastery();
                    AddRoads("1", 0.389f * k, 0.086f * k);
                    AddRoads("3", -0.411f * k, 0.027f * k);
                    break;
                case 26:
                    AddCathedral(0.298f * k, -0.211f * k);
                    break;
                case 27:
                    AddCities("0", -0.015f * k, 0.439f * k);
                    AddCities("1", 0.463f * k, 0.078f * k);
                    AddCities("2", 0f, -0.395f * k);
                    AddCities("3", -0.477f * k, 0.038f * k);
                    AddFields("", -0.016f * k, 0.037f * k, A(0, 1, 2, 3));
                    break;
                case 28:
                    AddCities("0", 0f, 0.474f * k);
                    AddCities("1", 0.463f * k, 0.078f * k);
                    AddCities("3", -0.477f * k, 0.038f * k);
                    AddFields("45", -0.005f * k, -0.167f * k, A(0, 1, 2));
                    break;
                case 29:
                    AddCities("0", 0f, 0.449f * k);
                    AddCities("2", 0f, -0.409f * k);
                    AddRoads("1", 0.191f * k, 0.036f * k);
                    AddRoads("3", -0.252f * k, 0f);
                    AddFields("2", 0.444f * k, 0.213f * k, A(0));
                    AddFields("3", 0.442f * k, -0.229f * k, A(1));
                    AddFields("6", -0.475f * k, -0.219f * k, A(1));
                    AddFields("7", -0.454f * k, 0.211f * k, A(0));
                    break;
                case 30:
                    AddCities("03", -0.351f * k, 0.357f * k, true);
                    AddCities("2", 0f, -0.409f * k);
                    AddFields("23", 0.202f * k, -0.005f * k, A(0, 1));
                    break;
                case 31:
                    AddCities("0", -0.012f * k, 0.305f * k);
                    AddFields("23", 0.442f * k, 0.114f * k, A(0));
                    AddFields("4567", -0.303f * k, -0.256f * k, A(0));
                    break;
                case 32:
                    AddCities("0", 0.027f * k, 0.445f * k);
                    AddRoads("2", 0.01f * k, -0.092f * k);
                    AddFields("234", 0.284f * k, -0.055f * k, A(0));
                    AddFields("567", -0.292f * k, -0.098f * k, A(0));
                    break;
                case 33:
                    AddCities("03", -0.353f * k, 0.376f * k);
                    AddRoads("1", 0.112f * k, 0.013f * k);
                    AddFields("2", 0.41f * k, 0.215f * k, A(0));
                    AddFields("345", 0.19f * k, -0.27f * k, A(0));
                    break;
                case 34:
                    AddCities("13", 0f, 0.055f * k, true);
                    AddRoads("0", 0.277f * k, 0.511f * k);
                    AddRoads("2", 0f, 0.5f * k);
                    AddFields("0", -0.236f * k, 0.511f * k, A(0));
                    AddFields("1", 0.277f * k, 0.511f * k, A(0));
                    AddFields("4", 0.254f * k, -0.408f * k, A(0));
                    AddFields("5", -0.267f * k, -0.407f * k, A(0));
                    break;
                case 35:
                    AddCities("03", -0.309f * k, 0.309f * k);
                    AddRoads("2", -0.036f * k, -0.21f * k, true);
                    AddFields("234", 0.459f * k, -0.232f * k, A(0));
                    AddFields("5", -0.196f * k, -0.453f * k, A(0));
                    break;
                case 36:
                    AddCities("0", 0f, 0.42f * k);
                    AddRoads("23", -0.141f * k, -0.176f * k, true);
                    AddFields("2347", 0.454f * k, -0.184f * k, A(0));
                    AddFields("56", -0.217f * k, -0.426f * k);
                    break;
                case 37:
                    AddCities("03", -0.345f * k, 0.336f * k, true);
                    AddRoads("12", 0.184f * k, -0.164f * k, true);
                    AddFields("25", -0.181f * k, -0.435f * k, A(0));
                    AddFields("34", 0.356f * k, -0.391f * k);
                    break;
                case 38:
                    AddRoads("1", 0.293f * k, -0.032f * k, true);
                    AddRoads("2", 0f, -0.285f * k);
                    AddRoads("3", -0.261f * k, 0.027f * k);
                    AddFields("0127", -0.188f * k, 0.293f * k);
                    AddFields("34", 0.345f * k, -0.364f * k);
                    AddFields("56", -0.352f * k, -0.308f * k);
                    break;
                case 39:
                    AddRoads("13", 0f, 0f, true);
                    AddFields("0127", 0f, -0.3f * k);
                    AddFields("3456", 0.233f * k, 0.409f * k);
                    break;
                case 40:
                    AddRoads("23", -0.032f * k, 0f, true);
                    AddFields("012347", -0.218f * k, 0.297f * k);
                    AddFields("56", -0.275f * k, -0.281f * k);
                    break;
                case 41:
                    AddRoads("01", 0.157f * k, 0.064f * k);
                    AddRoads("23", -0.27f * k, -0.093f * k);
                    AddFields("12", 0.3f * k, 0.3f * k);
                    AddFields("0347", -0.272f * k, 0.258f * k);
                    AddFields("56", -0.375f * k, -0.402f * k);
                    break;


                // Kings
                case 52:
                    AddCities("0", -0.03f * k, 0.4f * k);
                    AddMonastery(-0.069f * k, -0.126f * k);
                    AddFields("234567", 0.358f * k, -0.208f * k, A(0));
                    break;
                case 53:
                    AddCities("1", 0.448f * k, 0.052f * k);
                    AddRoads("0", 0.022f * k, 0.213f * k);
                    AddRoads("23", -0.162f * k, -0.144f * k);
                    AddFields("1", 0.218f * k, 0.462f * k, A(0));
                    AddFields("047", -0.372f * k, 0.334f * k, A(0));
                    AddFields("56", -0.385f * k, -0.375f * k);
                    break;
                case 54:
                    AddCities("1", 0.448f * k, 0.052f * k);
                    AddRoads("0", 0.01f * k, 0.216f * k);
                    AddFields("1", 0.218f * k, 0.462f * k, A(0));
                    AddFields("04567", -0.186f * k, -0.111f * k, A(0));
                    break;
                case 55:
                    AddCities("03", -0.195f * k, 0.225f * k);
                    AddRoads("2", -0.02f * k, -0.264f * k);
                    AddRoads("1", 0.283f * k, 0.127f * k);
                    AddFields("2", 0.477f * k, 0.282f * k, A(0));
                    AddFields("3", 0.467f * k, -0.174f * k, A(0));
                    AddFields("4", 0.222f * k, -0.452f * k, A(0));
                    AddFields("5", -0.25f * k, -0.432f * k, A(0));
                    break;
                case 56:
                    AddCities("02", 0f, 0.445f * k);
                    AddCities("13", 0.03f * k, 0.114f * k);
                    break;

                //Traders And Builders
                case 57:
                    AddMonastery();
                    AddRoads("1", 0.396f * k, 0.041f * k);
                    AddRoads("2", -0.015f * k, -0.378f * k);
                    AddRoads("3", -0.43f * k, 0f);
                    break;
                case 58:
                    AddCities("0", 0f, 0.44f * k);
                    AddRoads("1", 0.321f * k, 0.03f * k);
                    AddFields("2", 0.472f * k, 0.264f * k, A(0));
                    AddFields("3456", -0.163f * k, -0.179f * k, A(0));
                    break;
                case 59:
                    AddCities("0", 0f, 0.44f * k);
                    AddRoads("2", 0.005f * k, -0.211f * k);
                    AddRoads("3", -0.382f * k, 0.055f * k);
                    AddFields("234", 0.295f * k, -0.3f * k);
                    AddFields("56", -0.279f * k, -0.284f * k);
                    AddFields("7", -0.476f * k, 0.28f * k);
                    break;
                case 60:
                    AddRoads("02", 0f, 0.44f * k);
                    AddRoads("13", 0.192f * k, 0.067f * k);
                    AddFields("12", 0.357f * k, 0.378f * k);
                    AddFields("34", 0.261f * k, -0.308f * k);
                    AddFields("56", -0.3f * k, -0.304f * k);
                    AddFields("70", -0.386f * k, 0.381f * k);
                    break;

                //Custom Tiles
                case 81:
                    AddRoads("0", 0.006f * k, 0.406f * k, true);
                    AddFields("01234567", 0.286f * k, -0.268f * k);
                    break;
                case 82:
                    AddRoads("0123", 0.006f * k, 0.406f * k);
                    AddFields("12", 0.266f * k, 0.302f * k);
                    AddFields("34", 0.286f * k, -0.268f * k);
                    AddFields("56", -0.31f * k, -0.283f * k);
                    AddFields("70", -0.293f * k, 0.372f * k);
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
