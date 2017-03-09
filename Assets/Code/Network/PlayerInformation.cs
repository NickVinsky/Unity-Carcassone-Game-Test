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

    public interface IPlayerInfo {
        string PlayerName { get; set;}
        int ID { get; set;} // ConnectionId of player
        PlayerColor Color { get; set;}
        byte FollowersNumber { get; set;}
        int Score { get; set;}
    }

    public struct PlayerInformation : IPlayerInfo{
        public string PlayerName { get; set;}
        public int ID { get; set;}
        public PlayerColor Color { get; set;}
        public byte FollowersNumber { get; set;}
        public int Score { get; set;}

        public bool IsRegistred;
        public bool IsReady;
        public bool WaitingForColorUpgrade;
        public int MySlotNumber;
        public int MySlotNumberInGame;

        public bool IsMyTurn;
    }
}
