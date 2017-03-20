using System.Collections.Generic;
using Code.Game.Data;
using Code.Game.TileSubs;
using Code.Network;
using Code.Network.Commands;
using UnityEngine;

namespace Code.Game {
    public static class Tile {
        public static OnMouseHandler OnMouse = new OnMouseHandler();
        public static RotateHandler Rotate = new RotateHandler();
        public static NearbyHandler Nearby = new NearbyHandler();

        public static List<ReconstructionInfo> Cache = new List<ReconstructionInfo>();

        public static int StartingTile { get; set; }

        public static GameObject LastPlacedTile { get; set; }
        private static bool LastPlacedTileFounderHided { get; set; }

        public static TileInfo LastCheckedTile { get; private set; }

        public static TileInfo LastPlaced() { return LastPlacedTile.GetComponent<TileInfo>(); }

        public static TileInfo GetParent(GameObject o) { return o.transform.parent.gameObject.GetComponent<TileInfo>(); }

        public static TileInfo GetStarting() { return GameObject.Find("cell#0:0").GetComponent<TileInfo>(); }

        public static TileInfo Get(GameObject o) { return o.GetComponent<TileInfo>(); }

        public static TileInfo Get(string name) { return GameObject.Find(name).GetComponent<TileInfo>(); }

        public static TileInfo Get(Cell cell) { return GameObject.Find("cell#" + cell.X + ":" + cell.Y).GetComponent<TileInfo>(); }

        public static GameObject GetGameObject(Cell cell) { return GameObject.Find("cell#" + cell.X + ":" + cell.Y); }

        public static Cell GetCoordinates(GameObject o) {
            var x = o.GetComponent<TileInfo>().X;
            var y = o.GetComponent<TileInfo>().Y;
            return new Cell(x, y);
        }

        public static bool Exist(GameObject cell) {
            return cell.GetComponent<TileInfo>().Type != 0;
        }

        public static bool Exist(Cell v) {
            var cell = GameObject.Find("cell#" + v.X + ":" + v.Y);
            if (cell == null) return false;
            LastCheckedTile = cell.GetComponent<TileInfo>();
            return LastCheckedTile.Type != 0;
        }

        public static void ShowLastPlaced() {
            if (LastPlacedTile == null) return;
            LastPlacedTile.GetComponent<SpriteRenderer>().color = Net.CombineColors(Net.Color(LastPlaced().Founder), new Color(0.8f,0.8f,0.8f,0.8f));
            LastPlacedTile.transform.localScale = MainGame.Grid.EnlargedScale;
            LastPlacedTile.GetComponent<SpriteRenderer>().sortingOrder = 2;
            LastPlacedTileFounderHided = false;
        }

        public static void HideLastPlaced() {
            if (LastPlacedTileFounderHided || LastPlacedTile == null) return;
            LastPlacedTile.GetComponent<SpriteRenderer>().color = GameRegulars.NormalColor;
            LastPlacedTile.transform.localScale = MainGame.Grid.BaseScale;
            LastPlacedTile.GetComponent<SpriteRenderer>().sortingOrder = 1;
            LastPlacedTileFounderHided = true;
        }

        public static bool CannotBePlacedOnBoard(int index) {
            var tester = new GameObject("Tester");
            var testerTileID = Deck.GetTileByIndex(index);

            tester.AddComponent<TileInfo>();
            var testerTile = Get(tester);
            testerTile.InitTile(testerTileID);

            foreach (var emptyCell in GameObject.FindGameObjectsWithTag(GameRegulars.EmptyCellTag)) {
                if (!Nearby.CanBeAttachedTo(emptyCell, testerTile)) continue;
                Object.Destroy(tester);
                return false;
            }

            //GameObject.FindGameObjectsWithTag(GameRegulars.EmptyCellTag).All(emptyCell => !Nearby.CanBeAttachedTo(emptyCell, testerTile))
            return true;
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

        public static string GetVariation(int tileId) {
            int rnd;
            var upperBound = 1;
            switch (tileId) {
                case 16:
                    upperBound += 2;
                    rnd = Random.Range(1, upperBound);
                    return tileId + "v" + rnd;
                case 21:
                    upperBound += 2;
                    rnd = Random.Range(1, upperBound);
                    return tileId + "v" + rnd;
                case 22:
                    upperBound += 3;
                    rnd = Random.Range(1, upperBound);
                    return tileId + "v" + rnd;
                default:
                    return tileId.ToString();
            }
        }

        public static void SetStarting(int startTileType) {
            StartingTile = startTileType;
            var gridCell = GameObject.Find("cell#0:0");
            Object.Destroy(gridCell.GetComponent<BoxCollider2D>());
            Deck.Delete(StartingTile);
            gridCell.tag = GameRegulars.TileTag;
            gridCell.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Tiles/" + StartingTile);
            gridCell.GetComponent<TileInfo>().Rotates = 0;
            gridCell.GetComponent<TileInfo>().InitTile(StartingTile);
            MainGame.Grid.Expand(new Cell(0, 0));
        }

        public static void AttachToMouse() {
            var tp = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            tp = new Vector3(tp.x, tp.y, 0f);
            OnMouse.SetPosition(tp);
            if (Net.Game.IsOnline()) Net.Client.Action(Command.MouseCoordinates, tp);
        }

        public static void AttachToCoordinates(Vector3 t) {
            if (OnMouse.Exist()) OnMouse.Get().transform.position = t;
        }

        public static void Pick() {
            var tileID = Deck.GetRandomTile();
            //pickedTile = Deck.Get(22);
            var rotates = Rotate.Random();
            ApplyPicking(tileID, rotates);
        }

        public static void Pick(int index, byte rotates) {
            var tileID = Deck.GetTileByIndex(index);
            ApplyPicking(tileID, rotates);
            OnMouse.Get().transform.position = new Vector3(Screen.width * 2, 0f, 0f);
        }

        public static void PickWithID(int tileId, byte rotates) {
            ApplyPicking(tileId, rotates);
            OnMouse.Get().transform.position = new Vector3(Screen.width * 2, 0f, 0f);
        }

        private static void ApplyPicking(int tileType, byte rotates) {
            if (Deck.IsEmpty()) return;
            OnMouse.Create();
            OnMouse.Get().transform.SetParent(GameObject.Find(GameRegulars.GameTable).transform);
            OnMouse.Get().AddComponent<SpriteRenderer>();
            OnMouse.Get().AddComponent<TileInfo>();
            OnMouse.GetTile().InitTile(tileType);
            OnMouse.GetTile().Rotates = (sbyte) rotates;
            OnMouse.GetSprite().sprite = Resources.Load<Sprite>("Tiles/" + GetVariation(tileType)); // 80-All, 24-Vanilla
            OnMouse.GetSprite().sortingOrder = 4;
            Rotate.Sprite(rotates, OnMouse.Get());
            if (Net.Game.IsOnline() && !Net.Game.MyTurn()) {
                Cursor.visible = true;
            } else {
                Cursor.visible = false;
            }
            Deck.DeleteLastPicked();
        }

        public static void Return() {
            var type = OnMouse.GetTile().Type;
            Deck.Add(type);
            OnMouse.Destroy();
            Cursor.visible = true;
        }

        public static void Reconstruct(Cell cell, int id, int tileIndex, PlayerColor founder, byte rotation, sbyte locId, PlayerColor owner, Follower followerType) {
            Cache.Add(new ReconstructionInfo(cell, id, tileIndex, rotation, locId, owner));
            Deck.SetLastPickedIndex(tileIndex);
            OnMouse.Destroy();
            PickWithID(id, rotation);
            OnMouse.Put(cell, founder);
            Get(cell).AssignOpponentFollower(owner, (byte) locId, followerType);
        }

        public static void ShowPossibleFollowersLocations(GameObject o, Follower type) {
            Get(o).ShowPossibleLocations(type);
        }
    }
}