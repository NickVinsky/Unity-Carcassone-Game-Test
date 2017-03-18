using System;
using Code.Game.Data;
using Code.GameComponents;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.Game {
    public static class Deck {
        private static TilesPack<int> _tilesPack = new TilesPack<int>();
        private static int _lastPickedIndex;

        public static void Add(int type) {
            _tilesPack.Add(type);
        }

        public static int LastPickedIndex() { return _lastPickedIndex; }

        public static int GenerateIndex() {
            var maxIndex = _tilesPack.Count;
            var rnd = Random.Range(0, maxIndex);
            return rnd;
        }

        public static int GenerateIndexSafe() {
            var maxIndex = _tilesPack.Count;
            var rnd = Random.Range(0, maxIndex);

            return Tile.CannotBePlacedOnBoard(rnd) ? GenerateIndexSafe() : rnd;
        }

        public static int GetRandomTile() {
            var rnd = GenerateIndex();
            int result = _tilesPack[rnd];
            _lastPickedIndex = rnd;
            //_tilesPack.RemoveAt(rnd);
            return result;
        }

        public static int GetAndSaveIndex(int tileId) {
            for (int i = 0; i < _tilesPack.Count; i++) {
                if (_tilesPack[i] == tileId) {
                    _lastPickedIndex = i;
                    return i;
                }
            }
            return 0;
        }

        public static int GetTileByIndex(int index) {
            int result = _tilesPack[index];
            _lastPickedIndex = index;
            //_tilesPack.RemoveAt(index);
            return result;
        }

        public static void Delete(int tileId) {
            for (var i = 0; i < _tilesPack.Count; i++) {
                if (_tilesPack[i] != tileId) continue;
                _tilesPack.RemoveAt(i);
                return;
            }
        }

        public static void SetLastPickedIndex(int index) { _lastPickedIndex = index; }


        public static void DeleteLastPicked() { _tilesPack.RemoveAt(_lastPickedIndex); }

        public static void DeleteFirst() {
            _tilesPack.RemoveAt(0);
        }

        public static int DeckSize() {
            return _tilesPack.Count;
        }
        public static bool IsEmpty() {
            return _tilesPack.Count == 0;
        }
        public static bool LastTile() {
            return _tilesPack.Count == 1;
        }
        public static void InitVanillaDeck() {
            _tilesPack.OnAddOrRemove += EventOnAddOrRemove;
            _tilesPack.Clear();
            AddTilesToDeck(1, 4);
            AddTilesToDeck(2, 2);
            AddTileToDeck(3);
            AddTilesToDeck(4, 3);
            AddTileToDeck(5);
            AddTileToDeck(6);
            AddTilesToDeck(7, 2);
            AddTilesToDeck(8, 3);
            AddTilesToDeck(9, 2);
            AddTilesToDeck(10, 3);
            AddTilesToDeck(11, 2);
            AddTileToDeck(12);
            AddTilesToDeck(13, 2);
            AddTilesToDeck(14, 2);
            AddTilesToDeck(15, 3);
            AddTilesToDeck(16, 5);
            AddTilesToDeck(17, 3);
            AddTilesToDeck(18, 3);
            AddTilesToDeck(19, 3);
            AddTilesToDeck(20, 4); // Starting Tile
            AddTilesToDeck(21, 8);
            AddTilesToDeck(22, 9);
            AddTilesToDeck(23, 4);
            AddTilesToDeck(24, 1);

            // Ins And Cathedrals
            AddTilesToDeck(25, 1);
            AddTilesToDeck(26, 2);
            AddTilesToDeck(27, 2);
            AddTilesToDeck(28, 2);
            AddTilesToDeck(29, 3);
            AddTilesToDeck(30, 2);
            AddTilesToDeck(31, 2);
            AddTilesToDeck(32, 3);
            AddTilesToDeck(33, 3);
            AddTilesToDeck(34, 2);
            AddTilesToDeck(35, 1);
            AddTilesToDeck(36, 1);
            AddTilesToDeck(37, 1);
            AddTilesToDeck(38, 1);
            AddTilesToDeck(39, 1);
            AddTilesToDeck(40, 1);
            AddTilesToDeck(41, 5);

            // Kings
            AddTilesToDeck(52, 1);
            AddTilesToDeck(53, 4);
            AddTilesToDeck(54, 4);
            AddTilesToDeck(55, 2);
            AddTilesToDeck(56, 2);

            //Traders And Builders
            AddTilesToDeck(57, 1);
            AddTilesToDeck(58, 4);
            AddTilesToDeck(59, 3);
            AddTilesToDeck(60, 4);

            //Custom Tiles
            AddTilesToDeck(81, 4);
            AddTilesToDeck(82, 2);
        }

        private static void AddTilesToDeck(int type, int quantity) {
            for (int i = 0; i < quantity; i++) {
                AddTileToDeck(type);
            }
        }

        private static void AddTileToDeck(int type) {
            _tilesPack.Add(type);
        }

        public static void EventOnAddOrRemove(object sender, EventArgs e) {
            if (GameObject.Find(GameRegulars.DeckCounter) == null) return;
            GameObject.Find(GameRegulars.DeckCounter).GetComponent<Text>().text = DeckSize() + "x";
        }
    }
}
