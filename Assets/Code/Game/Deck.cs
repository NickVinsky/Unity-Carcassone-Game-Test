using System;
using Code.Game.Data;
using Code.GameComponents;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.Game {
    public static class Deck {
        private static TilesPack<int> _tilesPack = new TilesPack<int>();

        public static void Add(int type) {
            _tilesPack.Add(type);
        }

        public static int GenerateIndex() {
            var maxIndex = _tilesPack.Count - 1;
            var rnd = Random.Range(0, maxIndex);
            return rnd;
        }

        public static int GetRandomTile() {
            var rnd = GenerateIndex();
            int result = _tilesPack[rnd];
            _tilesPack.RemoveAt(rnd);
            return result;
        }

        public static int Get(int tile) {
            for (int i = 0; i < _tilesPack.Count; i++) {
                if (_tilesPack[i] == tile) return _tilesPack[i];
            }
            return 0;
        }

        public static int GetTile(int index) {
            int result = _tilesPack[index];
            _tilesPack.RemoveAt(index);
            return result;
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
            AddTilesToDeck(20, 3); // Starting Tile
            AddTilesToDeck(21, 8);
            AddTilesToDeck(22, 9);
            AddTilesToDeck(23, 4);
            AddTilesToDeck(24, 1);
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
