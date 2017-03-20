using Code.Game.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Network.Commands {
    public enum ErrType {
        PlayerNameOccupied,
        LobbyIsFull,
        GameAlreadyStarted
    }
    public enum ConnInfo {
        PlayerRegistred,
        PlayerDisconnected
    }
    public enum Command {
        Start,
        PlayerReturn,
        NextTurnIs,
        RotateTile,
        PickTileFromDeck,
        ReturnTileToDeck,
        TilePicked,
        TileNotPicked,
        HighlightCell,
        PutTile,
        MouseCoordinates,
        CursorStreaming,
        CursorStopStreaming,
        Follower,
        FinishTurn,
        AssignFollower,
        RemovePlacement,
        RemovePlacementMonk,
        BuilderCheck,
        NextPlayer
}

    public class NetPackBlank : MessageBase {}

    public class NetPackFollower : MessageBase {
        public PlayerColor Color;
        public Follower FollowerType;
    }

    public class NetPackPlayer : MessageBase {
        public PlayerInfo Player;
    }

    public class NetPackPlayerInfo : MessageBase {
        public string UniKey;

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
        public bool Trigger = false;
        public string Text;
        public int Value;
        public byte Byte;
        public PlayerColor Color;
        public Follower Follower;
        public Vector3 Vect3;
        public Cell Vector;
        //public Vector2 mousePosition;
    }

    public class NetPackTileCache : MessageBase {
        public Cell Cell;
        public int TileID;
        public int TileIndex;
        public PlayerColor Founder;
        public byte Rotation;

        public sbyte LocactionID;
        public PlayerColor LocationOwner;
        public Follower FollowerType;
    }

    public class NetPackRespond : MessageBase {
        public bool Respond;
    }


}