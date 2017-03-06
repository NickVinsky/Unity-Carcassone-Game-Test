using UnityEngine;

namespace Code.Tiles
{
    internal class Tile : MonoBehaviour {

        public int Type;

        public Area[] This = {Area.Field, Area.Field, Area.Field, Area.Field};
        public sbyte Rotates { get; set; } // 0 - 0 grad, 1 - 90 grad, 2 - 180 grad, 3 - 270 grad ; Clockwise

        public bool[] Field = new bool[8];

        /*public Tile(Area top, Area right, Area bot, Area left) {
            Rotates = 0;
            This[(int) Side.Top] = top;
            This[(int) Side.Right] = right;
            This[(int) Side.Bot] = bot;
            This[(int) Side.Left] = left;
        }*/

        public void InitTile(int type) {
            Type = type;
            This[(int) Side.Top] = Area.Field;
            This[(int) Side.Right] = Area.Field;
            This[(int) Side.Bot] = Area.Field;
            This[(int) Side.Left] = Area.Field;
            switch (Type) {
                case 0:
                    This[(int) Side.Top] = Area.Empty;
                    This[(int) Side.Right] = Area.Empty;
                    This[(int) Side.Bot] = Area.Empty;
                    This[(int) Side.Left] = Area.Empty;
                    break;
                case 2:
                    This[(int) Side.Bot] = Area.Road;
                    break;
                case 3:
                    This[(int) Side.Top] = Area.Town;
                    This[(int) Side.Right] = Area.Town;
                    This[(int) Side.Bot] = Area.Town;
                    This[(int) Side.Left] = Area.Town;
                    break;
                case 4:
                case 5:
                    This[(int) Side.Top] = Area.Town;
                    This[(int) Side.Right] = Area.Town;
                    This[(int) Side.Left] = Area.Town;
                    break;
                case 6:
                case 7:
                    This[(int) Side.Top] = Area.Town;
                    This[(int) Side.Right] = Area.Town;
                    This[(int) Side.Bot] = Area.Road;
                    This[(int) Side.Left] = Area.Town;
                    break;
                case 8:
                case 9:
                case 14:
                    This[(int) Side.Top] = Area.Town;
                    This[(int) Side.Left] = Area.Town;
                    break;
                case 10:
                case 11:
                    This[(int) Side.Top] = Area.Town;
                    This[(int) Side.Right] = Area.Road;
                    This[(int) Side.Bot] = Area.Road;
                    This[(int) Side.Left] = Area.Town;
                    break;
                case 12:
                case 13:
                    This[(int) Side.Right] = Area.Town;
                    This[(int) Side.Left] = Area.Town;
                    break;
                case 15:
                    This[(int) Side.Top] = Area.Town;
                    This[(int) Side.Bot] = Area.Town;
                    break;
                case 16:
                    This[(int) Side.Top] = Area.Town;
                    break;
                case 17:
                    This[(int) Side.Top] = Area.Town;
                    This[(int) Side.Bot] = Area.Road;
                    This[(int) Side.Left] = Area.Road;
                    break;
                case 18:
                    This[(int) Side.Top] = Area.Town;
                    This[(int) Side.Right] = Area.Road;
                    This[(int) Side.Bot] = Area.Road;
                    break;
                case 19:
                    This[(int) Side.Top] = Area.Town;
                    This[(int) Side.Right] = Area.Road;
                    This[(int) Side.Bot] = Area.Road;
                    This[(int) Side.Left] = Area.Road;
                    break;
                case 20:
                    This[(int) Side.Top] = Area.Town;
                    This[(int) Side.Right] = Area.Road;
                    This[(int) Side.Left] = Area.Road;
                    break;
                case 21:
                    This[(int) Side.Top] = Area.Road;
                    This[(int) Side.Bot] = Area.Road;
                    break;
                case 22:
                    This[(int) Side.Bot] = Area.Road;
                    This[(int) Side.Left] = Area.Road;
                    break;
                case 23:
                    This[(int) Side.Right] = Area.Road;
                    This[(int) Side.Bot] = Area.Road;
                    This[(int) Side.Left] = Area.Road;
                    break;
                case 24:
                    This[(int) Side.Top] = Area.Road;
                    This[(int) Side.Right] = Area.Road;
                    This[(int) Side.Bot] = Area.Road;
                    This[(int) Side.Left] = Area.Road;
                    break;
            }
        }
    }
}
