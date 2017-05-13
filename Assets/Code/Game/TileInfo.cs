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
        private readonly District _follower = new District();

        private readonly Area[] _side = {Area.Empty, Area.Empty, Area.Empty, Area.Empty};

        public sbyte Rotates { get; set; } // 0 - 0 grad, 1 - 90 grad, 2 - 180 grad, 3 - 270 grad ; Clockwise

        //public bool[] Field = new bool[8];

        public Area GetSide(int side) => _side[side];

        private Placements CurrentPlacementState { get; set; }
        public bool[] PlacementBlocked { get; set; }

        public List<Location> GetLocations => _follower.LocationsList;
        public Location GetLocation(int id) => _follower.GetLocation((byte) id);

        public void BarnReadiness(byte id, bool[] barnReady) => _follower.BarnReadiness(id, barnReady);

        public Cell IntVector => new Cell(X, Y);

        private static byte[] A(params byte[] a) => a;

        public void InitTile(int type) {
            PlacementBlocked = new bool[GameRegulars.EnumPlacementsCount];
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
                    AddMonastery();
                    AddRoads("2", -0.0095f, -0.92f);
                    AddFields("01234567", 0.279f * k, 0.271f * k);
                    break;
                case 3:
                    AddCities("0123", 0f, 0f, true);
                    break;
                case 4:
                    AddCities("013", 0f, 0.58f); // id# 0
                    AddFields("45", 0.138f, -1.098f, A(0)); // linket to city#0 ^^
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
                    AddFields("34", 1.114f, -1.18f);
                    break;
                case 11:
                    AddCities("03", -0.144f * k, 0.395f * k, true);
                    AddRoads("12", 0.075f * k, -0.244f * k);
                    AddFields("25", 0.195f * k, 0.122f * k, A(0));
                    AddFields("34", 0.89f, -0.953f);
                    break;
                case 12:
                    AddCities("13", -0.064f * k, 0.083f * k);
                    AddFields("01", 0.078f * k, 0.464f * k, A(0));
                    AddFields("45", 0.042f * k, -0.31f * k, A(0));
                    break;
                case 13:
                    AddCities("13", 0.589f, 0.371f, true);
                    AddFields("01", 0.166f, 1.418f, A(0));
                    AddFields("45", 0f, -1.2f, A(0));
                    break;
                case 14:
                    AddCities("0", -0.021f * k, 0.423f * k);
                    AddCities("3", -0.477f * k, 0.05f * k);
                    AddFields("2345", 0.095f * k, -0.089f * k, A(0, 1));
                    break;
                case 15:
                    AddCities("0", 0.044f, 1.36f);
                    AddCities("2", -0.294f, -1.359f);
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
                    AddCities("0", -0.154f, 1.391f);
                    AddRoads("1", 0.958f, -0.063f);
                    AddRoads("2", 0f, -1.132f);
                    AddRoads("3", -1.142f, 0.082f);
                    AddFields("27", 0.084f, 0.224f, A(0));
                    AddFields("34", 0.279f * k, -0.284f * k);
                    AddFields("56", -0.302f * k, -0.268f * k);
                    break;
                case 20:
                    AddCities("0", 0.023f * k, 0.434f * k);
                    AddRoads("13", 0.035f, 0.087f);
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
                    AddRoads("1", 1.131f, 0.25f);
                    AddRoads("3", -1.2f, 0.08f);
                    AddFields("0127", 0.3f, 1.23f);
                    AddFields("3456", 0.03f, -1.14f);
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
                    AddCities("03", -0.097f, 1.375f, true);
                    AddCities("2", 0.039f, -1.25f);
                    AddFields("23", 0.58f, 0f, A(0, 1));
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
                    AddCities("13", 0f, 0.2f, true);
                    AddRoads("0", 0.063f, 1.437f);
                    AddRoads("2", -0.042f, -1.14f);
                    AddFields("0", -0.652f, 1.472f, A(0));
                    AddFields("1", 0.785f, 1.458f, A(0));
                    AddFields("4", 0.69f, -1.215f, A(0));
                    AddFields("5", -0.756f, -1.268f, A(0));
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
                    AddCities("03", -0.351f, 1.334f, true);
                    AddRoads("12", 0.184f * k, -0.164f * k, true);
                    AddFields("25", -0.181f * k, -0.435f * k, A(0));
                    AddFields("34", 0.356f * k, -0.391f * k);
                    break;
                case 38:
                    AddRoads("1", 1.19f, 0.088f, true);
                    AddRoads("2", -0.066f, -1.109f);
                    AddRoads("3", -0.85f, 0.037f);
                    AddFields("0127", -0.55f, 0.85f);
                    AddFields("34", 0.987f, -1.01f);
                    AddFields("56", -0.958f, -0.904f);
                    break;
                case 39:
                    AddRoads("13", 0f, 0f, true);
                    AddFields("0127", 0.946f, 0.995f);
                    AddFields("3456", 0f, -0.8f);
                    break;
                case 40:
                    AddRoads("23", -0.032f * k, 0f, true);
                    AddFields("012347", -0.218f * k, 0.297f * k);
                    AddFields("56", -0.275f * k, -0.281f * k);
                    break;
                case 41:
                    AddRoads("01", 0.85f, 0.377f);
                    AddRoads("23", -0.439f, -0.131f);
                    AddFields("12", 0.779f, 1.313f);
                    AddFields("0347", -0.55f, 0.744f);
                    AddFields("56", -0.947f, -0.909f);
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
                    AddRoads("1", 1.066f, 0.156f);
                    AddRoads("2", 0.061f, -0.931f);
                    AddRoads("3", -1.205f, -0.022f);
                    AddFields("0127", 0.544f, 1.169f);
                    AddFields("34", 0.961f, -0.938f);
                    AddFields("56", -0.961f, -0.946f);
                    break;
                case 58:
                    AddCities("0", 0.289f, 1.37f);
                    AddRoads("1", 0.414f, 0.19f);
                    AddFields("2", 1.273f, 0.647f, A(0));
                    AddFields("3456", -0.712f, 0.057f, A(0));
                    break;
                case 59:
                    AddCities("0", 0f, 0.44f * k);
                    AddRoads("2", 0.005f * k, -0.211f * k);
                    AddRoads("3", -0.382f * k, 0.055f * k);
                    AddFields("234", 0.295f * k, -0.3f * k, A(0));
                    AddFields("56", -0.279f * k, -0.284f * k);
                    AddFields("7", -0.476f * k, 0.28f * k, A(0));
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
                    AddRoads("0123", -0.524f, 0.023f);
                    AddFields("12", 0.266f * k, 0.302f * k);
                    AddFields("34", 0.286f * k, -0.268f * k);
                    AddFields("56", -0.31f * k, -0.283f * k);
                    AddFields("70", -0.293f * k, 0.372f * k);
                    break;

                case 83:
                    AddRoads("013", -0.557f, 0f);
                    AddFields("12", 1f, 1f);
                    AddFields("3456", 0.398f, -0.874f);
                    AddFields("70", -0.886f, 0.92f);
                    break;
                case 84:
                    AddRoads("2", 0.097f, -0.617f);
                    AddFields("01234567", 0f, 1f);
                    break;
                case 85:
                    AddMonastery();
                    AddRoads("0", 0f, 1.168f);
                    AddRoads("1", 1.22f, 0f);
                    AddRoads("2", 0f, -0.9f);
                    AddRoads("3", -1.1f, 0.226f);
                    AddFields("12", 1f, 1f);
                    AddFields("34", 1f, -1f);
                    AddFields("56", -1f, -1f);
                    AddFields("70", -1f, 1f);
                    break;
                case 86:
                    AddCities("0", 0.115f, 1.28f);
                    AddRoads("13", 0.378f, -0.247f);
                    AddFields("2", 1.2f, 0.589f, A(0));
                    AddFields("3456", -0.661f, -0.831f);
                    AddFields("7", -1.2f, 0.5f, A(0));
                    break;
                case 87:
                    AddCities("0", -0.05f, 1.417f);
                    AddRoads("1", 1.312f, 0.108f);
                    AddRoads("3", -1.215f, 0.15f);
                    AddFields("2", 1.365f, 0.824f, A(0));
                    AddFields("3456", 0.104f, -0.153f, A(0));
                    AddFields("7", -1.453f, 0.836f, A(0));
                    break;
                case 88:
                    AddCities("0", 0.108f, 1.439f);
                    AddRoads("1", 0.9f, 0f);
                    AddRoads("3", -0.735f, 0.205f);
                    AddFields("2", 1.316f, 0.644f, A(0));
                    AddFields("3456", -0.109f, -0.863f);
                    AddFields("7", -1.371f, 0.743f, A(0));
                    break;
                case 89:
                    AddCities("0", 0.108f, 1.439f);
                    AddRoads("1", 1.317f, 0.142f);
                    AddRoads("3", -1.156f, 0.243f);
                    AddFields("234567", 0f, -0.5f, A(0));
                    break;
                case 90:
                    AddCities("0", 0.108f, 1.439f);
                    AddRoads("1", 0.306f, -0.508f);
                    AddRoads("3", -0.46f, 0.25f);
                    AddFields("26", -1.227f, -0.522f);
                    AddFields("345", 1.229f, -0.877f);
                    AddFields("7", -1.251f, 0.661f, A(0));
                    break;
                case 91:
                    AddCities("0", 0.1f, 0.328f);
                    AddRoads("1", 1.27f, 0.115f);
                    AddRoads("3", -1.165f, 0.163f);
                    AddFields("2", 1.355f, 0.903f, A(0));
                    AddFields("3", 1.414f, -0.645f, A(0));
                    AddFields("45", 0.027f, -1.169f, A(0));
                    AddFields("6", -1.462f, -0.511f, A(0));
                    AddFields("7", -1.333f, 0.847f, A(0));
                    break;
                case 92:
                    AddCities("0", -0.221f, 1.398f);
                    AddRoads("13", -0.714f, 0.265f, true);
                    AddFields("27", 1.238f, 0.605f, A(0));
                    AddFields("3456", 0.644f, -0.717f);
                    break;
                case 93:
                    AddCities("13", 0.597f, 0.259f, true);
                    AddRoads("02", 0.037f, -1.439f);
                    AddFields("0", -0.679f, 1.462f, A(0));
                    AddFields("1", 0.813f, 1.455f, A(0));
                    AddFields("45", 0.667f, -1.064f, A(0));
                    break;
                case 94:
                    AddCities("3", -0.587f, 0.192f, true);
                    AddFields("01", 0f, 1.4f, A(0));
                    AddFields("23", 1.34f, 0.14f, A(0));
                    AddFields("45", -0.175f, -1.214f, A(0));
                    break;
                case 95:
                    AddCities("0", 0f, 1.4f);
                    AddCities("3", -1.443f, 0.058f);
                    AddRoads("1", 0.66f, 0.244f);
                    AddRoads("2", -0.205f, -0.559f);
                    AddFields("2", 1.317f, 0.733f, A(0));
                    AddFields("34", 0.772f, -0.711f, A(0, 1));
                    AddFields("5", -0.7f, -1.223f, A(1));
                    break;
                case 96:
                    AddCities("02", -0.166f, -1.253f, true);
                    AddCities("13", -1.124f, 0.232f);
                    break;
                case 97:
                    AddCities("0", -0.297f, 1.461f);
                    AddRoads("23", 0.184f, 0.012f);
                    AddFields("234", 1.166f, 0.273f, A(0));
                    AddFields("56", -0.718f, -0.555f);
                    AddFields("7", -1.373f, 0.681f, A(0));
                    break;
                case 98:
                    AddCities("0", -0.297f, 1.461f);
                    AddRoads("12", -0.183f, -0.239f);
                    AddFields("2", 1.374f, 0.709f, A(0));
                    AddFields("34", 0.789f, -0.624f);
                    AddFields("567", -1.117f, -0.08f, A(0));
                    break;
                case 99:
                    AddCities("0", 0.153f, 1.241f);
                    AddRoads("1", 0.846f, 0.11f);
                    AddRoads("2", -0.092f, -0.488f);
                    AddFields("2", 1.313f, 0.573f, A(0));
                    AddFields("34", 0.787f, -0.669f);
                    AddFields("567", -1.308f, -0.587f, A(0));
                    break;
                case 100:
                    AddCities("0", 0.044f, 1.271f);
                    AddRoads("1", 0.487f, 0.185f);
                    AddRoads("3", -0.157f, -0.602f);
                    AddFields("2", 1.395f, 0.641f, A(0));
                    AddFields("37", 0.925f, -0.517f);
                    AddFields("456", -1.139f, -1.047f);
                    break;
                case 101:
                    AddCities("3", -1.378f, 0.125f);
                    AddRoads("0", -0.043f, 0.514f);
                    AddRoads("12", 0.54f, -0.418f);
                    AddFields("0", -0.571f, 1.337f, A(0));
                    AddFields("125", 0.872f, 0.775f, A(0));
                    AddFields("34", 1.24f, -1.085f);
                    break;
                case 102:
                    AddCities("01", 0.955f, 0.792f);
                    AddRoads("3", -0.418f, -0.012f);
                    AddFields("456", -0.276f, -0.832f, A(0));
                    AddFields("7", -1.275f, 0.638f, A(0));
                    break;
                case 103:
                    AddCities("01", 0.5f, 0.6f);
                    AddRoads("2", 0.13f, -0.942f);
                    AddRoads("3", -1.16f, 0.204f);
                    AddFields("4", 0.74f, -1.3f, A(0));
                    AddFields("5", -0.533f, -1.328f, A(0));
                    AddFields("6", -1.358f, -0.464f, A(0));
                    AddFields("7", -1.424f, 0.804f, A(0));
                    break;
                case 104:
                    AddCities("0", -0.098f, 0.904f);
                    AddFields("2345", 0.829f, -0.691f, A(0));
                    AddFields("67", -1.353f, 0.155f, A(0));
                    break;
                case 105:
                    AddCities("01", 0.395f, 1.159f, true);
                    AddCities("2", -0.07f, -1.299f);
                    AddFields("67", -0.805f, 0.077f, A(0, 1));
                    break;
                case 106:
                    AddCities("01", 1.238f, 0.271f, true);
                    AddRoads("23", -0.618f, -0.474f, true);
                    AddFields("47", 0.241f, -0.549f, A(0));
                    AddFields("56", -1.054f, -1.064f);
                    break;
                case 107:
                    AddCities("01", 1.039f, 1.013f);
                    AddRoads("2", 0.041f, -0.485f, true);
                    AddFields("4", 0.64f, -1.147f, A(0));
                    AddFields("567", -1.342f, -0.477f, A(0));
                    break;
                case 108:
                    AddCities("0", 0f, 1.257f);
                    AddRoads("12", 0.417f, -0.208f, true);
                    AddFields("2567", -1.187f, 0.459f, A(0));
                    AddFields("34", 1.072f, -0.836f);
                    break;
                case 109:
                    AddRoads("12", 0.2f, -0.059f, true);
                    AddFields("012567", -0.778f, -0.57f);
                    AddFields("34", 0.877f, -0.736f);
                    break;
            }
        }

        private void AddMonastery(float x = 0f, float y = 0f) {
            var meeplePos = new Vector2(x, y);
            var newLoc = new LocationInfo {
                Type = Area.Monastery,
                MeeplePos = meeplePos
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
            //var o = gameObject;
            _follower.Opponent(owner, id, type);
        }

        public void HideAll() {
            _follower.HideAll();
        }

        public void RemovePlacement(int constructID) {
            _follower.RemovePlacement(constructID);
        }

        public void ShowPossibleLocations(Follower type) {
            CurrentPlacementState = Placements.AllRestricted;
            ShowNextPossiblePlacement();
        }

        public void ShowNextPossiblePlacement() {
            if (MainGame.Player.FollowersEmpty()) {
                TryToFinish();
                return;
            }

            CurrentPlacementState = Next(CurrentPlacementState);
            _follower.HideAll();
            switch (CurrentPlacementState) {
                case Placements.MeeplesPigsAndBuilders:
                    if (MainGame.Player.MeeplesQuantity > 0 || MainGame.Player.PigsQuantity > 0 || MainGame.Player.BuildersQuantity > 0)
                        _follower.Show(Rotates, Placements.MeeplesPigsAndBuilders, Follower.Meeple, Follower.Pig, Follower.Builder);
                    else {
                        PlacementBlocked[(int) Placements.MeeplesPigsAndBuilders] = true;
                        ShowNextPossiblePlacement();
                    }
                    break;
                case Placements.BigMeeples:
                    if (MainGame.Player.BigMeeplesQuantity > 0) _follower.Show(Rotates, Placements.BigMeeples, Follower.BigMeeple);
                    else {
                        PlacementBlocked[(int) Placements.BigMeeples] = true;
                        ShowNextPossiblePlacement();
                    }
                    break;
                case Placements.Mayor:
                    if (MainGame.Player.MayorsQuantity > 0) _follower.Show(Rotates, Placements.Mayor, Follower.Mayor);
                    else {
                        PlacementBlocked[(int) Placements.Mayor] = true;
                        ShowNextPossiblePlacement();
                    }
                    break;
                case Placements.BarnAndWagons:
                    if (MainGame.Player.BarnsQuantity > 0) _follower.Show(Rotates, Placements.BarnAndWagons, Follower.Barn);
                    else {
                        PlacementBlocked[(int) Placements.BarnAndWagons] = true;
                        ShowNextPossiblePlacement();
                    }
                    break;
                case Placements.AllRestricted:
                    TryToFinish();
                    return;
            }
        }

        private Placements Next(Placements state) {
            var hasFreePlacements = false;

            for (var i = 1; i < PlacementBlocked.Length; i++) {
                if (!PlacementBlocked[i]) {
                    hasFreePlacements = true;
                }
            }
            if (!hasFreePlacements) return Placements.AllRestricted;

            while (true) {
                var length = GameRegulars.EnumPlacementsCount;
                var curPlacementInt = (int) state;
                var nextPlacementInt = curPlacementInt + 1;
                if (nextPlacementInt >= length) nextPlacementInt = 1;

                if (!PlacementBlocked[nextPlacementInt]) return (Placements) nextPlacementInt;
                state = (Placements) nextPlacementInt;
            }
        }

        public void TryToFinish() {
            MainGame.ChangeGameStage(GameStage.Finish);
        }

        public void ApplyRotation() {
            _follower.RotateNodes((byte) Rotates);
        }
    }
}
