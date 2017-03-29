namespace Code.Game.Data {
    public class ReconstructionInfo {
        public ReconstructionInfo() {
            LocactionID = -1;
            LocationOwner = PlayerColor.NotPicked;
        }

        public ReconstructionInfo(Cell cell, int tileID, int tileIndex, byte rotation, sbyte locactionID, PlayerColor locationOwner, bool[] barnReady) {
            Cell = cell;
            TileID = tileID;
            TileIndex = tileIndex;
            Rotation = rotation;
            LocactionID = locactionID;
            LocationOwner = locationOwner;
            ReadyForBarn = barnReady;
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