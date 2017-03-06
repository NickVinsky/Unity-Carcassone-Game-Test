using System;
using UnityEngine;
using Object = UnityEngine.Object;
using static Code.GameRegulars;
using Random = UnityEngine.Random;

namespace Code.Tiles
{
    internal static class TilesHandler {
        public static GameObject TileOnMouse;

        #region Rotations
        public static float GetRotateAngle(GameObject tile) {
            var rotates = tile.GetComponent<Tile>().Rotates;
            return Convert.ToSingle(rotates) * 90;
        }
        private static sbyte RotateHandler(sbyte turnsToDo, sbyte rotates) {
            sbyte r = Convert.ToSByte(rotates + turnsToDo);
            while (r > 3) r -= 4;
            while (r < 0) r += 4;
            return r;
        }
        private static int RotateHandler(int n) {
            int r = n;
            while (r > 3) r -= 4;
            while (r < 0) r += 4;
            return r;
        }
        private static void RotateGameObject(int r, GameObject o) {
            var angle = 0f;
            switch (r) {
                case 0:
                    break;
                case 1:
                    angle = -90f;
                    break;
                case 2:
                    angle = -180f;
                    break;
                case 3:
                    angle = 90f;
                    break;
            }
            o.GetComponent<SpriteRenderer>().transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        public static void RotateClockwise() {
            TileOnMouse.GetComponent<SpriteRenderer>().transform.Rotate(Vector3.back * 90);
            TileOnMouse.GetComponent<Tile>().Rotates = RotateHandler(1, TileOnMouse.GetComponent<Tile>().Rotates);
        }
        public static void RotateClockwise(GameObject tile) {
            tile.GetComponent<SpriteRenderer>().transform.Rotate(Vector3.back * 90);
            tile.GetComponent<Tile>().Rotates = RotateHandler(1, tile.GetComponent<Tile>().Rotates);
        }
        public static void RotateCounterClockwise(){
            TileOnMouse.GetComponent<SpriteRenderer>().transform.Rotate(Vector3.back * -90);
            TileOnMouse.GetComponent<Tile>().Rotates = RotateHandler(-1, TileOnMouse.GetComponent<Tile>().Rotates);
        }
        public static void RotateCounterClockwise(GameObject tile){
            tile.GetComponent<SpriteRenderer>().transform.Rotate(Vector3.back * -90);
            tile.GetComponent<Tile>().Rotates = RotateHandler(-1, tile.GetComponent<Tile>().Rotates);
        }
        public static void SetTileOnMouseRotation(int r) {
            RotateGameObject(r, TileOnMouse);
            TileOnMouse.GetComponent<Tile>().Rotates = (sbyte)r;
        }
        public static sbyte GetTileOnMouseRotation() {
            return TileOnMouse.GetComponent<Tile>().Rotates;
        }
        #endregion
        #region Neighbors
        private static Side GetOppositeSide(Side side) {
            return (Side)RotateHandler((int) side + 2);
        }
        public static GameObject TileToTheLeft(Vector2 cellPos) {
            return GameObject.Find("cell#" + (int)(cellPos.x - 1) + ":" + (int)cellPos.y);
        }
        public static GameObject TileToTheRight(Vector2 cellPos) {
            return GameObject.Find("cell#" + (int)(cellPos.x + 1) + ":" + (int)cellPos.y);
        }
        public static GameObject TileToTheTop(Vector2 cellPos) {
            return GameObject.Find("cell#" + (int)cellPos.x + ":" + (int)(cellPos.y + 1));
        }
        public static GameObject TileToTheBot(Vector2 cellPos) {
            return GameObject.Find("cell#" + (int)cellPos.x + ":" + (int)(cellPos.y - 1));
        }
        private static GameObject GetNeighborBySide(Side side, GameObject parentCell) {
            int x = 0; int y = 0;
            if (side == Side.Top) y = 1;
            if (side == Side.Right) x = 1;
            if (side == Side.Bot) y = -1;
            if (side == Side.Left) x = -1;
            Vector2 cellPos = Game.Grid.GetCellCoordinates(parentCell);
            return GameObject.Find("cell#" + (cellPos.x + x) + ":" + (cellPos.y + y));
        }
        private static bool CheckAttachment(GameObject tileOnMouse, Side sideOfMainTile, GameObject targetTile) {
            if (tileOnMouse.GetComponent<Tile>().This[RotateHandler((int) sideOfMainTile - tileOnMouse.GetComponent<Tile>().Rotates)] ==
                targetTile.GetComponent<Tile>().This[RotateHandler((int) GetOppositeSide(sideOfMainTile) - targetTile.GetComponent<Tile>().Rotates)]) {
                return true;
            }
            return false;
        }
        private static bool NullCheck(GameObject mainTile, Side sideOfMainTile) {
            GameObject neighborTile = GetNeighborBySide(sideOfMainTile, mainTile);
            if (neighborTile == null) return true;
            return neighborTile.GetComponent<Tile>().Type == 0;
        }
        private static bool CompareTilesBySides(GameObject tileOnMouse, GameObject mainTile, Side sideOfMainTile) {
            GameObject neighborTile = GetNeighborBySide(sideOfMainTile, mainTile);
            if (neighborTile == null) return true;
            if (neighborTile.GetComponent<Tile>().Type == 0) return true;
            return CheckAttachment(tileOnMouse, sideOfMainTile, neighborTile);
        }
        public static bool CheckNeighborTiles(GameObject mainTile) {
            bool r = true;
            bool n = true;
            for (var i = 0; i < 4; i++) {
                n = n && NullCheck(mainTile, (Side) i);
                r = r && CompareTilesBySides(GetTileOnMouse(), mainTile, (Side) i);
            }
            return r && !n;
        }
        #endregion

        public static void HighlightCell(string name, int color) {
            var cell = GameObject.Find(name);
            switch (color) {
                case 0:
                    cell.GetComponent<SpriteRenderer>().color = NormalColor;
                    break;
                case 1:
                    cell.GetComponent<SpriteRenderer>().color = CanAttachColor;
                    break;
                case 2:
                    cell.GetComponent<SpriteRenderer>().color = CantAttachlColor;
                    break;
            }
        }

        public static GameObject GetTileOnMouse() {
            return TileOnMouse;
        }

        public static void PutTileFromMouse(GameObject gridCell) {
            gridCell.GetComponent<SpriteRenderer>().sprite = TileOnMouse.GetComponent<SpriteRenderer>().sprite;
            gridCell.GetComponent<SpriteRenderer>().transform.Rotate(Vector3.back * GetRotateAngle(TileOnMouse));
            gridCell.GetComponent<Tile>().Rotates = TileOnMouse.GetComponent<Tile>().Rotates;
            gridCell.GetComponent<Tile>().InitTile(TileOnMouse.GetComponent<Tile>().Type);
            Object.Destroy(TileOnMouse);
            Cursor.visible = true;
            Game.Grid.CheckBounds(gridCell);
        }
        public static void SetStartTile(int startTileType) {
            var gridCell = GameObject.Find("cell#0:0");
            gridCell.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Tiles/" + startTileType);
            gridCell.AddComponent<Tile>();
            gridCell.GetComponent<Tile>().Rotates = 0;
            gridCell.GetComponent<Tile>().InitTile(startTileType);
        }

        public static void AttachTileToMouse() {
            //GameObject.Find(TileOnMouse).transform.position = Vector2.Lerp(transformPosition, mousePosition, 1.0f);
            var tp = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            tp = new Vector3(tp.x, tp.y, 0f);
            TileOnMouse.transform.position = tp;
        }

        public static void AttachTileToCoordinates(Vector3 t) {
            TileOnMouse.transform.position = t;
        }

        private static sbyte RandomRotate() {
            return (sbyte) Random.Range(0, 4);
        }

        public static void PickTileFromDeck() {
            if (DeckHandler.DeckIsEmpty()) return;
            TileOnMouse = new GameObject(TileOnMouseName);
            TileOnMouse.transform.SetParent(GameObject.Find(GameTable).transform);
            TileOnMouse.AddComponent<SpriteRenderer>();
            TileOnMouse.AddComponent<Tile>();
            Cursor.visible = false;

            var pickedTile = DeckHandler.GetRandomTile();
            var rotates = RandomRotate();
            TileOnMouse.GetComponent<Tile>().InitTile(pickedTile);
            TileOnMouse.GetComponent<Tile>().Rotates = rotates;
            TileOnMouse.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Tiles/" + pickedTile); // 80-All, 24-Vanilla
            TileOnMouse.GetComponent<SpriteRenderer>().sortingOrder = 3;
            RotateGameObject(rotates, TileOnMouse);
        }

        public static void PickTileFromDeck(int index) {
            if (DeckHandler.DeckIsEmpty()) return;
            TileOnMouse = new GameObject(TileOnMouseName);
            TileOnMouse.transform.SetParent(GameObject.Find(GameTable).transform);
            TileOnMouse.AddComponent<SpriteRenderer>();
            TileOnMouse.AddComponent<Tile>();
            Cursor.visible = false;

            var pickedTile = DeckHandler.GetTile(index);
            var rotates = RandomRotate();
            TileOnMouse.GetComponent<Tile>().InitTile(pickedTile);
            TileOnMouse.GetComponent<Tile>().Rotates = rotates;
            TileOnMouse.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Tiles/" + pickedTile); // 80-All, 24-Vanilla
            TileOnMouse.GetComponent<SpriteRenderer>().sortingOrder = 3;
            RotateGameObject(rotates, TileOnMouse);
        }

        public static void ReturnTileToDeck() {
            var type = TileOnMouse.GetComponent<Tile>().Type;
            DeckHandler.Deck.Add(type);
            Object.Destroy(TileOnMouse);
            Cursor.visible = true;
        }

        #region bools
        private static bool TileCanBeConnectedTo(GameObject cell) {
            return CheckNeighborTiles(cell) && cell.GetComponent<Tile>().Type == 0;
        }
        private static bool TileOnGridExist(GameObject cell) {
            return cell.GetComponent<Tile>().Type != 0;
        }
        public static bool TileOnMouseExist() {
            return TileOnMouse != null;
        }
        public static bool TileCanBeAttachedTo(GameObject cell) {
            return TileOnMouseExist() && !TileOnGridExist(cell) && TileCanBeConnectedTo(cell);
        }
        #endregion
    }
}
