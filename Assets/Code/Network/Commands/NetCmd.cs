namespace Code.Network.Commands {
    public struct NetCmd {
        public const short AddPlayer = 48;
        public const short RemovePlayer = 49;
        public const short NumberOfPlayers = 50;
        public const short FreeColorRequest = 51;
        // 57 - 60 FREE

        public const short ChatMessage = 61;
        public const short ChatHistory = 62;

        public const short Ready = 52;
        public const short ReformLobbyPlayersList = 53;
        public const short ReformPlayersListWithAdding = 54;
        public const short FormLobbyPlayersList = 63;
        public const short InGamePlayers = 56;
        public const short InitRegistration = 64;
        public const short RefreshPlayersInfoAndReformPlayersList = 65;

        public const short Countdown = 55;

        public const short Err = 66;
        public const short Inf = 67;
        public const short Game = 70;

        public const short UpdatePlayerInfo = 68;
        public const short UpdatePlayerInfoAndReformPList = 69;

        public const short TransferCache = 71;
        public const short RefreshScore = 72;
        public const short SubtractFollower = 73;
        // next 74

        // 80 +
        public const short RegisterMe = 80;
        public const short ReturnIntoGame = 81;
        public const short GameData = 82;
        public const short TileCache = 83;
        public const short TileCacheFinish = 84;
    }
}