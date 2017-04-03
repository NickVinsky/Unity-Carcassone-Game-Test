using System;
using Code.Game.Data;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.Game {
    public static class Deck {
        private static TilesPack<int> _tilesPack = new TilesPack<int>();

        public static void Add(int type) {
            _tilesPack.Add(type);
        }

        public static int LastPickedIndex { get; private set; }

        public static int GenerateIndex => Random.Range(0, _tilesPack.Count);

        public static int GenerateIndexSafe() {
            var rnd = Random.Range(0, _tilesPack.Count);
            return Tile.CannotBePlacedOnBoard(rnd) ? GenerateIndexSafe() : rnd;
        }

        public static int GetRandomTile {
            get {
                var rnd = GenerateIndex;
                var result = _tilesPack[rnd];
                LastPickedIndex = rnd;
                return result;
            }
        }

        public static int GetAndSaveIndex(int tileId) {
            for (var i = 0; i < _tilesPack.Count; i++) {
                if (_tilesPack[i] != tileId) continue;
                LastPickedIndex = i;
                return i;
            }
            return 0;
        }

        public static int GetTileByIndex(int index) {
            var result = _tilesPack[index];
            LastPickedIndex = index;
            return result;
        }

        public static void Delete(int tileId) {
            for (var i = 0; i < _tilesPack.Count; i++) {
                if (_tilesPack[i] != tileId) continue;
                _tilesPack.RemoveAt(i);
                return;
            }
        }

        public static void SetLastPickedIndex(int index) { LastPickedIndex = index; }

        public static void DeleteLastPicked() { _tilesPack.RemoveAt(LastPickedIndex); }

        public static void DeleteFirst() {
            _tilesPack.RemoveAt(0);
        }

        public static int DeckSize => _tilesPack.Count;
        public static bool IsEmpty => _tilesPack.Count == 0;
        public static bool LastTile => _tilesPack.Count == 1;

        public static void InitVanillaDeck() {
            _tilesPack.OnAddOrRemove += EventOnAddOrRemove;
            _tilesPack.Clear();

            AddVanillaTiles();

            AddInnsAndCathedralsTiles();

            AddKingsTiles();

            AddTradersAndBuildersTiles();

            AddCustomTiles();
        }

        private static void AddVanillaTiles(int multiplier = 1) {
            AddTilesToDeck(1, 4, multiplier);
            AddTilesToDeck(2, 2, multiplier);
            AddTilesToDeck(3, 1, multiplier);
            AddTilesToDeck(4, 3, multiplier);
            AddTilesToDeck(5, 1, multiplier);
            AddTilesToDeck(6, 1, multiplier);
            AddTilesToDeck(7, 2, multiplier);
            AddTilesToDeck(8, 3, multiplier);
            AddTilesToDeck(9, 2, multiplier);
            AddTilesToDeck(10, 3, multiplier);
            AddTilesToDeck(11, 2, multiplier);
            AddTilesToDeck(12, 1, multiplier);
            AddTilesToDeck(13, 2, multiplier);
            AddTilesToDeck(14, 2, multiplier);
            AddTilesToDeck(15, 3, multiplier);
            AddTilesToDeck(16, 5, multiplier);
            AddTilesToDeck(17, 3, multiplier);
            AddTilesToDeck(18, 3, multiplier);
            AddTilesToDeck(19, 3, multiplier);
            AddTilesToDeck(20, 4, multiplier); // Starting Tile
            AddTilesToDeck(21, 8, multiplier);
            AddTilesToDeck(22, 9, multiplier);
            AddTilesToDeck(23, 4, multiplier);
            AddTilesToDeck(24, 1, multiplier);
        }

        private static void AddInnsAndCathedralsTiles(int multiplier = 1) {
            // В оригинале всех тайлов по одной штуке, кроме собора (#26), их два
            AddTilesToDeck(25, 1, multiplier);
            AddTilesToDeck(26, 2, multiplier);
            AddTilesToDeck(27, 1, multiplier);
            AddTilesToDeck(28, 1, multiplier);
            AddTilesToDeck(29, 1, multiplier);
            AddTilesToDeck(30, 1, multiplier);
            AddTilesToDeck(31, 1, multiplier);
            AddTilesToDeck(32, 1, multiplier);
            AddTilesToDeck(33, 1, multiplier);
            AddTilesToDeck(34, 1, multiplier);
            AddTilesToDeck(35, 1, multiplier);
            AddTilesToDeck(36, 1, multiplier);
            AddTilesToDeck(37, 1, multiplier);
            AddTilesToDeck(38, 1, multiplier);
            AddTilesToDeck(39, 1, multiplier);
            AddTilesToDeck(40, 1, multiplier);
            AddTilesToDeck(41, 1, multiplier);
        }

        private static void AddKingsTiles(int multiplier = 1) {
            // В оригинале всех тайлов по одной штуке
            AddTilesToDeck(52, 1, multiplier);
            AddTilesToDeck(53, 1, multiplier);
            AddTilesToDeck(54, 1, multiplier);
            AddTilesToDeck(55, 1, multiplier);
            AddTilesToDeck(56, 1, multiplier);
        }

        private static void AddTradersAndBuildersTiles(int multiplier = 1) {
            // В оригинале всех тайлов по одной штуке
            AddTilesToDeck(57, 1, multiplier);
            AddTilesToDeck(58, 1, multiplier);
            AddTilesToDeck(59, 1, multiplier);
            AddTilesToDeck(60, 1, multiplier);
        }

        private static void AddCustomTiles(int multiplier = 1) {
            AddTilesToDeck(81, 1, multiplier);
            AddTilesToDeck(82, 1, multiplier);
            AddTilesToDeck(83, 1, multiplier);
            AddTilesToDeck(84, 4, multiplier);
            AddTilesToDeck(85, 1, multiplier);
            AddTilesToDeck(86, 1, multiplier);
            AddTilesToDeck(87, 1, multiplier);
            AddTilesToDeck(88, 1, multiplier);
            AddTilesToDeck(89, 1, multiplier);
            AddTilesToDeck(90, 1, multiplier);
            AddTilesToDeck(91, 1, multiplier);
            AddTilesToDeck(92, 1, multiplier);
            AddTilesToDeck(93, 1, multiplier);
            AddTilesToDeck(94, 1, multiplier);
            AddTilesToDeck(95, 1, multiplier);
            AddTilesToDeck(96, 1, multiplier);
            AddTilesToDeck(97, 1, multiplier);
            AddTilesToDeck(98, 1, multiplier);
            AddTilesToDeck(99, 1, multiplier);
            AddTilesToDeck(100, 1, multiplier);
            AddTilesToDeck(101, 1, multiplier);
            AddTilesToDeck(102, 1, multiplier);
            AddTilesToDeck(103, 1, multiplier);
            AddTilesToDeck(104, 1, multiplier);
            AddTilesToDeck(105, 1, multiplier);
            AddTilesToDeck(106, 1, multiplier);
            AddTilesToDeck(107, 1, multiplier);
            AddTilesToDeck(108, 1, multiplier);
            AddTilesToDeck(109, 1, multiplier);

        }

        private static void AddTilesToDeck(int type, int quantity, int multiplier = 1) {
            var totalQuantity = quantity * multiplier;
            for (var i = 0; i < totalQuantity; i++) {
                AddTileToDeck(type);
            }
        }

        private static void AddTileToDeck(int type) {
            _tilesPack.Add(type);
        }

        public static void EventOnAddOrRemove(object sender, EventArgs e) {
            if (GameObject.Find(GameRegulars.DeckCounter) == null) return;
            GameObject.Find(GameRegulars.DeckCounter).GetComponent<Text>().text = DeckSize + "x";
        }
    }
}
