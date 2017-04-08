using Code.Network.Commands;

namespace Code.Game.Data {
    public class ReconstructionInfo {
        public ReconstructionInfo() {
            LocactionID = -1;
            LocationOwner = PlayerColor.NotPicked;
        }

        public ReconstructionInfo(NetPackTileCache cache) {
            Cell = cache.Cell;
            TileID = cache.TileID;
            TileIndex = cache.TileIndex;
            Founder = cache.Founder;
            Rotation = cache.Rotation;
            LocactionID = cache.LocactionID;
            LocationOwner = cache.LocationOwner;
            FollowerType = cache.FollowerType;
            ReadyForBarn = cache.ReadyForBarn;
        }

        public Cell Cell { get; set; }
        public int TileID { get; set; }
        public int TileIndex { get; set; }
        public PlayerColor Founder { get; set; }
        public byte Rotation { get; set; }

        public sbyte LocactionID { get; set; }
        public PlayerColor LocationOwner { get; set; }
        public Follower FollowerType { get; set; }
        public bool[] ReadyForBarn = new bool[4];
    }
}