namespace Code.Game.Data {
    public struct PlayerInfo {
        public string UniKey;

        public int ID;
        public string PlayerName;
        public PlayerColor Color;

        public GameStage Stage;
        public byte MeeplesQuantity;
        public byte BigMeeplesQuantity;
        public byte MayorsQuantity;
        public byte PigsQuantity;
        public byte BuildersQuantity;
        public byte BarnsQuantity;
        public byte WagonsQuantity;
        public int Score;

        public bool IsRegistred;
        public bool IsReady;
        public bool WaitingForColorUpgrade;

        public int MySlotNumber;
        public int MySlotNumberInGame;

        public bool IsMyTurn;
    }
}