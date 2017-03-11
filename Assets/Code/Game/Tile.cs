using Code.Game.Data;
using Code.Game.TileSubs;
using UnityEngine;

namespace Code.Game {
    public static class Tile {
        public static OnMouseHandler OnMouse = new OnMouseHandler();
        public static RotateHandler Rotate = new RotateHandler();
        public static NearbyHandler Nearby = new NearbyHandler();

        public static GameObject LastPlacedTile { get; set; }

        public static TileInfo LastPlaced() { return LastPlacedTile.GetComponent<TileInfo>(); }

        public static TileInfo GetParent(GameObject o) {
            return o.transform.parent.gameObject.GetComponent<TileInfo>();
        }

        public static TileInfo Get(string name) { return GameObject.Find(name).GetComponent<TileInfo>(); }

        public static bool Exist(GameObject cell) {
            return cell.GetComponent<TileInfo>().Type != 0;
        }

        public static void Highlight(string name, int color) {
            var cell = GameObject.Find(name);
            switch (color) {
                case 0:
                    cell.GetComponent<SpriteRenderer>().color = GameRegulars.NormalColor;
                    break;
                case 1:
                    cell.GetComponent<SpriteRenderer>().color = GameRegulars.CanAttachColor;
                    break;
                case 2:
                    cell.GetComponent<SpriteRenderer>().color = GameRegulars.CantAttachlColor;
                    break;
            }
        }

        public static void SetStarting(int startTileType) {
            var gridCell = GameObject.Find("cell#0:0");
            gridCell.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Tiles/" + startTileType);
            gridCell.GetComponent<TileInfo>().Rotates = 0;
            gridCell.GetComponent<TileInfo>().InitTile(startTileType);
        }

        public static void AttachToMouse() {
            var tp = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            tp = new Vector3(tp.x, tp.y, 0f);
            OnMouse.SetPosition(tp);
        }

        public static void AttachToCoordinates(Vector3 t) {
            if (OnMouse.Exist()) OnMouse.Get().transform.position = t;
        }

        public static void Pick() {
            if (Deck.DeckIsEmpty()) return;
            OnMouse.Create();
            OnMouse.Get().transform.SetParent(GameObject.Find(GameRegulars.GameTable).transform);
            OnMouse.Get().AddComponent<SpriteRenderer>();
            OnMouse.Get().AddComponent<TileInfo>();
            Cursor.visible = false;

            var pickedTile = Deck.GetRandomTile();
            var rotates = Rotate.Random();
            OnMouse.GetTile().InitTile(pickedTile);
            OnMouse.GetTile().Rotates = (sbyte) rotates;
            OnMouse.GetSprite().sprite = Resources.Load<Sprite>("Tiles/" + pickedTile); // 80-All, 24-Vanilla
            OnMouse.GetSprite().sortingOrder = 3;
            Rotate.Sprite(rotates, OnMouse.Get());
        }

        public static void Pick(int index, byte rotates) {
            if (Deck.DeckIsEmpty()) return;
            OnMouse.Create();
            OnMouse.Get().transform.SetParent(GameObject.Find(GameRegulars.GameTable).transform);
            OnMouse.Get().AddComponent<SpriteRenderer>();
            OnMouse.Get().AddComponent<TileInfo>();
            OnMouse.Get().transform.position = new Vector3(Screen.width * 2, 0f, 0f);
            Cursor.visible = false;

            var pickedTile = Deck.GetTile(index);
            OnMouse.GetTile().InitTile(pickedTile);
            OnMouse.GetTile().Rotates = (sbyte) rotates;
            OnMouse.GetSprite().sprite = Resources.Load<Sprite>("Tiles/" + pickedTile); // 80-All, 24-Vanilla
            OnMouse.GetSprite().sortingOrder = 3;
            Rotate.Sprite(rotates, OnMouse.Get());
        }

        public static void Return() {
            var type = OnMouse.GetTile().Type;
            Deck.Add(type);
            OnMouse.Destroy();
            Cursor.visible = true;
        }

        public static void ShowPossibleFollowersLocations(GameObject o) {
            o.GetComponent<TileInfo>().ShowPossibleFollowersLocations(o);
        }
    }
}