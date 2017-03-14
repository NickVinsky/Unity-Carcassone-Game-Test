using Code.Game.Data;
using Code.Network;
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
            Object.Destroy(_tileOnMouse);
        }

        // Вызывается из локальной игры
        public void Put(GameObject gridCell) {
            ApplyPuting(gridCell);
            MainGame.UpdateLocalPlayer();
        }

        // Вызывается из сетевой игры
        public void Put(Cell v) {
            var gridCell  = GameObject.Find("cell#" + v.X + ":" + v.Y);
            ApplyPuting(gridCell);
        }

        private void ApplyPuting(GameObject gridCell) {
            gridCell.tag = GameRegulars.TileTag;
            var cSprite = gridCell.GetComponent<SpriteRenderer>();
            cSprite.sprite = GetSprite().sprite;
            cSprite.transform.Rotate(Vector3.back * Tile.Rotate.GetAngle(_tileOnMouse));
            cSprite.color = GameRegulars.NormalColor;
            var cTileInfo = gridCell.GetComponent<TileInfo>();
            cTileInfo.Rotates = GetTile().Rotates;
            cTileInfo.InitTile(GetTile().Type);
            cTileInfo.ApplyRotation();
            Destroy();
            Cursor.visible = true;
            //MainGame.Grid.CheckBounds(gridCell);
            ScoreCalc.Count(gridCell);
            Tile.LastPlacedTile = gridCell;
            MainGame.Grid.Expand(Tile.GetCoordinates(gridCell));
        }
    }
}