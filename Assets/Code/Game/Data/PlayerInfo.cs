namespace Code.Game.Data {
    public class PlayerInfo {
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

        public PlayerInfo() {
            InitGameRules();
            Score = 0;
        }

        public PlayerInfo(string uniKey, int id, string playerName) {
            UniKey = uniKey;
            ID = id;
            PlayerName = playerName;
            Score = 0;
            InitGameRules();
        }

        public void InitGameRules() {
            MeeplesQuantity = 8;
            BigMeeplesQuantity = 1;
            MayorsQuantity = 1;
            PigsQuantity = 6;
            BuildersQuantity = 6;
            BarnsQuantity = 1;
            WagonsQuantity = 1;
        }
    }
}