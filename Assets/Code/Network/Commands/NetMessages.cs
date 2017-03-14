using Code.Game.Data;
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
        Follower,
        FinishTurn,
        AssignFollower,
        RemovePlacement,
        NextPlayer
}

    public class NetPackBlank : MessageBase {}

    public class NetPackPlayerColor : MessageBase {
        public PlayerColor Color;
    }

    public class NetPackPlayerInfo : MessageBase {
        public bool IsRegistred = false;
        public bool IsReady = false;

        public string PlayerName;
        public int ID;
        public PlayerColor Color;
        public byte FollowersNumber;
        public int Score;
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
        public byte Byte;
        public PlayerColor Color;
        public Vector3 Vect3;
        public Cell Vector;
        //public Vector2 mousePosition;
    }
}