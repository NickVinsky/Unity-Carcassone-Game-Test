using Code.Game.Data;
using UnityEngine;

namespace Code.Game.TileSubs {
    public class NearbyHandler {
        private int _lastX, _lastY;

        public Side GetOppositeSide(Side side) => (Side)Tile.Rotate.Set((int) side + 2);

        public TileInfo GetLast { get; private set; }

        public bool Exist(Cell v, Side side) {
            var cell = new Cell(v);
            switch (side) {
                case Side.Top:
                    cell.OffsetTop();
                    break;
                case Side.Right:
                    cell.OffsetRight();
                    break;
                case Side.Bot:
                    cell.OffsetBot();
                    break;
                case Side.Left:
                    cell.OffsetLeft();
                    break;
            }
            var tile = Tile.GetGameObject(cell);
            if (tile == null) return false;
            GetLast = tile.GetComponent<TileInfo>();
            return GetLast.Type != 0;
        }

        private static GameObject GetBySide(Side side, GameObject parentCell) {
            var x = 0; var y = 0;
            if (side == Side.Top) y = 1;
            if (side == Side.Right) x = 1;
            if (side == Side.Bot) y = -1;
            if (side == Side.Left) x = -1;
            var cellPos = MainGame.Grid.GetCellCoordinates(parentCell);
            return Tile.GetGameObject(new Cell(cellPos, x, y));
            //return GameObject.Find("cell#" + (cellPos.X + x) + ":" + (cellPos.Y + y));
        }

        public bool CanBeAttachedTo(GameObject cell, TileInfo tester) {
            var r = true;
            var n = true;
            for (var i = 0; i < 4; i++) {
                var neighborTile = GetBySide((Side) i, cell);
                n = n && NullCheck(neighborTile);
                r = r && CompareBySides(tester, neighborTile, (Side) i);
            }
            return r && !n;
        }

        public bool TileOnMouseCanBeAttachedTo(GameObject cell) => Tile.OnMouse.Exist && !Tile.Exist(cell) && CanBeConnectedTo(cell);

        public bool CanBeConnectedTo(GameObject cell) => CheckNeighborTiles(cell) && cell.GetComponent<TileInfo>().Type == 0;

        public bool CheckNeighborTiles(GameObject mainTile) {
            var r = true;
            var n = true;
            for (var i = 0; i < 4; i++) {
                var neighborTile = GetBySide((Side) i, mainTile);
                n = n && NullCheck(neighborTile);
                r = r && CompareBySides(Tile.OnMouse.Get, neighborTile, (Side) i);
            }
            return r && !n;
        }

        private static bool NullCheck(GameObject neighborTile) {
            if (neighborTile == null) return true;
            return !Tile.Exist(neighborTile);
        }

        private bool CompareBySides(TileInfo attachableTile, GameObject neighborTile, Side sideOfMainTile) {
            if (neighborTile == null) return true;
            if (!Tile.Exist(neighborTile)) return true;
            return CheckAttachment(sideOfMainTile, attachableTile, Tile.Get(neighborTile));
        }

        private bool CheckAttachment(Side sideOfMainTile, TileInfo mainTile, TileInfo targetTile) =>
            mainTile.GetSide(Tile.Rotate.Set((int) sideOfMainTile - mainTile.Rotates)) ==
            targetTile.GetSide(Tile.Rotate.Set((int) GetOppositeSide(sideOfMainTile) - targetTile.Rotates));
    }
}