using UnityEngine;
using UnityEngine.Networking;

namespace Code.Network.Commands {
    public enum ErrType {
        PlayerNameOccupied,
        LobbyIsFull
    }
    public enum ConnInfo {
        PlayerRegistred,
        PlayerDisconnected
    }
    public enum Command {
        Start,
        NextTurnIs,
        RotateTile,
        PickTileFromDeck,
        ReturnTileToDeck,
        TilePicked,
        TileNotPicked,
        HighlightCell,
        PutTile,
        MouseCoordinates,
        NextPlayer
}

    public class NetPackBlank : MessageBase {}

    public class NetPackPlayerInfo : MessageBase, IPlayerInfo {
        public bool IsRegistred = false;
        public bool IsReady = false;

        public string PlayerName { get; set; }
        public int ID { get; set; }
        public PlayerColor Color { get; set; }
        public byte FollowersNumber { get; set; }
        public int Score { get; set; }

        public NetPackPlayerInfo() {
            ID = -10;
            PlayerName = string.Empty;
            Color = PlayerColor.NotPicked;
        }
    }

    public class NetPackChatMessage : MessageBase {
        public string Player;
        public string Message;
        public bool IsInfoMessage = false;
        public int RequesterID = -10;
    }

    public class NetPackMessage : MessageBase {
        public string Message;
    }

    public class NetPackErr : MessageBase {
        public ErrType Err;
    }

    public class NetPackInf : MessageBase {
        public ConnInfo Inf;
    }

    public class NetPackGame : MessageBase {
        public Command Command;
        public bool CommandDone = false;
        public string Text;
        public int Value;
        public PlayerColor Color;
        public Vector3 Vect3;
        //public Vector2 mousePosition;
    }
}