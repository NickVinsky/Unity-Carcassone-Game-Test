using System.Text;

namespace Code.Network {
    /*
    public struct PlayerInfo {
        public static string PlayerName;
        public static int ConnectionId;

        public static bool IsRegistred = false;
    }*/

    public enum PlayerColor {
        NotPicked = 0,
        Blue = 1,
        Yellow = 2,
        Green = 3,
        Red = 4,
        Black = 5
    }

    public struct PlayerInformation {
        public string PlayerName;
        public int ConnectionId;
        public PlayerColor Color;

        public bool IsRegistred;
        public bool IsReady;
        public bool WaitingForColorUpgrade;
        public int MySlotNumber;
        public int MySlotNumberInGame;

        public bool IsMyTurn;
    }
}
