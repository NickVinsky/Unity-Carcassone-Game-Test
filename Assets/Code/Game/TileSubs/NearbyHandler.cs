using UnityEngine;

namespace Code.Game.TileSubs {
    public class NearbyHandler {
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

        private GameObject GetBySide(Side side, GameObject parentCell) {
            var x = 0; var y = 0;
            if (side == Side.Top) y = 1;
            if (side == Side.Right) x = 1;
            if (side == Side.Bot) y = -1;
            if (side == Side.Left) x = -1;
            var cellPos = MainGame.Grid.GetCellCoordinates(parentCell);
            return GameObject.Find("cell#" + (cellPos.x + x) + ":" + (cellPos.y + y));
        }

        private bool CompareBySides(GameObject tileOnMouse, GameObject mainTile, Side sideOfMainTile) {
            GameObject neighborTile = GetBySide(sideOfMainTile, mainTile);
            if (neighborTile == null) return true;
            if (neighborTile.GetComponent<TileInfo>().Type == 0) return true;
            return CheckAttachment(tileOnMouse, sideOfMainTile, neighborTile);
        }

        public bool CheckNeighborTiles(GameObject mainTile) {
            var r = true;
            var n = true;
            for (var i = 0; i < 4; i++) {
                n = n && NullCheck(mainTile, (Side) i);
                r = r && CompareBySides(Tile.OnMouse.Get(), mainTile, (Side) i);
            }
            return r && !n;
        }

        private bool CheckAttachment(GameObject tileOnMouse, Side sideOfMainTile, GameObject targetTile) {
            if (tileOnMouse.GetComponent<TileInfo>().This[Tile.Rotate.Set((int) sideOfMainTile - tileOnMouse.GetComponent<TileInfo>().Rotates)] ==
                targetTile.GetComponent<TileInfo>().This[Tile.Rotate.Set((int) GetOppositeSide(sideOfMainTile) - targetTile.GetComponent<TileInfo>().Rotates)]) {
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