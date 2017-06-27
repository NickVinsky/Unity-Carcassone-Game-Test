using Code.Game.Data;
using UnityEngine;

namespace Code.Game.TileSubs {
    public class OnMouseHandler {
        public void Create() {
            if (!Exist) GetGameObject = new GameObject(GameRegulars.TileOnMouseName);
        }

        public GameObject GetGameObject { get; private set; }

        public SpriteRenderer GetSpriteRenderer => GetGameObject.GetComponent<SpriteRenderer>();

        public TileInfo Get => GetGameObject.GetComponent<TileInfo>();

        public sbyte GetRotation => Get.Rotates;

        public Area GetSide(int side) => Get.GetSide(side);

        public void SetPosition(Vector3 pos) {
            if (!Exist) return;
            GetGameObject.transform.position = pos;
        }

        public void SetRotation(int r) {
            Tile.Rotate.Sprite(r, GetGameObject);
            Get.Rotates = (sbyte)r;
        }

        public bool Exist => GetGameObject != null;

        public void Focus() {
            if (!Exist) return;
            var tomPos = GetGameObject.transform.localPosition;
            var locScale = GetGameObject.transform.localScale;
            var newPos = new Vector3(tomPos.x / locScale.x, tomPos.y / locScale.y, -1f);
            Camera.main.transform.position = newPos;
        }

        public void Destroy() {
            Object.DestroyImmediate(GetGameObject);
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
            cSprite.sprite = GetSpriteRenderer.sprite;
            cSprite.transform.Rotate(Vector3.back * Tile.Rotate.GetAngle(GetGameObject));
            cSprite.color = GameRegulars.NormalColor;
            var cTileInfo = gridCell.GetComponent<TileInfo>();
            cTileInfo.Founder = founder;
            cTileInfo.Rotates = Get.Rotates;
            cTileInfo.InitTile(Get.Type);
            cTileInfo.ApplyRotation();
            Destroy();

            var reconstructInfo = new ReconstructionInfo {
                Cell = cTileInfo.IntVector,
                TileID = cTileInfo.Type,
                TileIndex = Deck.LastPickedIndex,
                Rotation = (byte) cTileInfo.Rotates,
                Founder = founder
            };
            Tile.Cache.Add(reconstructInfo);

            Cursor.visible = true;
            //MainGame.Grid.CheckBounds(gridCell);
            Tile.HideLastPlaced();
            Tile.LastPlacedTile = gridCell;
            ScoreCalc.Count(gridCell, founder);
            MainGame.Grid.Expand(Tile.GetCoordinates(gridCell));
        }
    }
}