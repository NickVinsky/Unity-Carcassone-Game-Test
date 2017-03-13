using Code.Game.Data;
using UnityEngine;

namespace Code.Game.TileSubs {
    public class NearbyHandler {
        private int _lastX, _lastY;
        private TileInfo _lastTile;

        private Side GetOppositeSide(Side side) {
            return (Side)Tile.Rotate.Set((int) side + 2);
        }

        public GameObject Left(Vector2 cellPos) {
            return GameObject.Find("cell#" + (int)(cellPos.x - 1) + ":" + (int)cellPos.y);
        }

        public GameObject Right(Vector2 cellPos) {
            return GameObject.Find("cell#" + (int)(cellPos.x + 1) + ":" + (int)cellPos.y);
        }

        public GameObject Top(Vector2 cellPos) {
            return GameObject.Find("cell#" + (int)cellPos.x + ":" + (int)(cellPos.y + 1));
        }

        public GameObject Bot(Vector2 cellPos) {
            return GameObject.Find("cell#" + (int)cellPos.x + ":" + (int)(cellPos.y - 1));
        }

        public TileInfo GetLast() { return _lastTile; }

        public bool Exist(Cell v, byte side) {
            var cell = new Cell(v);
            switch ((Side) side) {
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
            var tile = GameObject.Find("cell#" + cell.X + ":" + cell.Y);
            if (tile == null) return false;
            _lastTile = tile.GetComponent<TileInfo>();
            return _lastTile.Type != 0;
        }

        private GameObject GetBySide(Side side, GameObject parentCell) {
            var x = 0; var y = 0;
            if (side == Side.Top) y = 1;
            if (side == Side.Right) x = 1;
            if (side == Side.Bot) y = -1;
            if (side == Side.Left) x = -1;
            var cellPos = MainGame.Grid.GetCellCoordinates(parentCell);
            return GameObject.Find("cell#" + (cellPos.X + x) + ":" + (cellPos.Y + y));
        }

        private bool CompareBySides(GameObject mainTile, Side sideOfMainTile) {
            GameObject neighborTile = GetBySide(sideOfMainTile, mainTile);
            if (neighborTile == null) return true;
            if (neighborTile.GetComponent<TileInfo>().Type == 0) return true;
            return CheckAttachment(sideOfMainTile, neighborTile);
        }

        public bool CheckNeighborTiles(GameObject mainTile) {
            var r = true;
            var n = true;
            for (var i = 0; i < 4; i++) {
                n = n && NullCheck(mainTile, (Side) i);
                r = r && CompareBySides(mainTile, (Side) i);
            }
            return r && !n;
        }

        private bool CheckAttachment(Side sideOfMainTile, GameObject targetTile) {
            if (Tile.OnMouse.GetSide(Tile.Rotate.Set((int) sideOfMainTile - Tile.OnMouse.GetRotation())) ==
                targetTile.GetComponent<TileInfo>().GetSide(Tile.Rotate.Set((int) GetOppositeSide(sideOfMainTile) - targetTile.GetComponent<TileInfo>().Rotates))) {
                return true;
            }
            return false;
        }

        private bool NullCheck(GameObject mainTile, Side sideOfMainTile) {
            GameObject neighborTile = GetBySide(sideOfMainTile, mainTile);
            if (neighborTile == null) return true;
            return neighborTile.GetComponent<TileInfo>().Type == 0;
        }

        public bool CanBeConnectedTo(GameObject cell) {
            return CheckNeighborTiles(cell) && cell.GetComponent<TileInfo>().Type == 0;
        }

        public bool CanBeAttachedTo(GameObject cell) {
            return Tile.OnMouse.Exist() && !Tile.Exist(cell) && CanBeConnectedTo(cell);
        }
    }
}