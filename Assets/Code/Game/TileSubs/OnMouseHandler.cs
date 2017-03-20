using Code.Game.Data;
using UnityEngine;

namespace Code.Game.TileSubs {
    public class OnMouseHandler {
        private GameObject _tileOnMouse;

        public void Create() {
            if (!Exist()) _tileOnMouse = new GameObject(GameRegulars.TileOnMouseName);
        }

        public GameObject Get() { return _tileOnMouse; }

        public SpriteRenderer GetSprite() {
            return _tileOnMouse.GetComponent<SpriteRenderer>();
        }

        public TileInfo GetTile() {
            return _tileOnMouse.GetComponent<TileInfo>();
        }

        public sbyte GetRotation() {
            return GetTile().Rotates;
        }

        public Area GetSide(int side) {
            return GetTile().GetSide(side);
        }

        public void SetPosition(Vector3 pos) {
            if (!Exist()) return;
            _tileOnMouse.transform.position = pos;
        }

        public void SetRotation(int r) {
            Tile.Rotate.Sprite(r, _tileOnMouse);
            GetTile().Rotates = (sbyte)r;
        }

        public bool Exist() {
            return _tileOnMouse != null;
        }

        public void Destroy() {
            Object.DestroyImmediate(_tileOnMouse);
        }

        // Вызывается из локальной игры
        public void Put(GameObject gridCell) {
            ApplyPuting(gridCell, MainGame.Player.Color);
            MainGame.UpdateLocalPlayer();
        }

        // Вызывается из сетевой игры
        public void Put(Cell v, PlayerColor founder) {
            var gridCell  = GameObject.Find("cell#" + v.X + ":" + v.Y);
            ApplyPuting(gridCell, founder);
        }

        private void ApplyPuting(GameObject gridCell, PlayerColor founder) {
            gridCell.tag = GameRegulars.TileTag;
            Object.Destroy(gridCell.GetComponent<BoxCollider2D>());
            var cSprite = gridCell.GetComponent<SpriteRenderer>();
            cSprite.sprite = GetSprite().sprite;
            cSprite.transform.Rotate(Vector3.back * Tile.Rotate.GetAngle(_tileOnMouse));
            cSprite.color = GameRegulars.NormalColor;
            var cTileInfo = gridCell.GetComponent<TileInfo>();
            cTileInfo.Founder = founder;
            cTileInfo.Rotates = GetTile().Rotates;
            cTileInfo.InitTile(GetTile().Type);
            cTileInfo.ApplyRotation();
            Destroy();

            var reconstructInfo = new ReconstructionInfo {
                Cell = cTileInfo.IntVector(),
                TileID = cTileInfo.Type,
                TileIndex = Deck.LastPickedIndex(),
                Rotation = (byte) cTileInfo.Rotates,
                Founder = founder
            };
            Tile.Cache.Add(reconstructInfo);

            Cursor.visible = true;
            //MainGame.Grid.CheckBounds(gridCell);
            ScoreCalc.Count(gridCell);
            Tile.LastPlacedTile = gridCell;
            MainGame.Grid.Expand(Tile.GetCoordinates(gridCell));
        }
    }
}