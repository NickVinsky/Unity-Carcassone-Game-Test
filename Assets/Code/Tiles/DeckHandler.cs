using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.Tiles {
    public static class DeckHandler {
        public static Deck<int> Deck = new Deck<int>();

        public static int GenerateIndex() {
            var maxIndex = Deck.Count - 1;
            var rnd = Random.Range(0, maxIndex);
            return rnd;
        }

        public static int GetRandomTile() {
            var rnd = GenerateIndex();
            int result = Deck[rnd];
            Deck.RemoveAt(rnd);
            return result;
        }

        public static int GetTile(int index) {
            int result = Deck[index];
            Deck.RemoveAt(index);
            return result;
        }

        public static int DeckSize() {
            return Deck.Count;
        }
        public static bool DeckIsEmpty() {
            return Deck.Count == 0;
        }
        public static void InitVanillaDeck() {
            Deck.OnAddOrRemove += EventOnAddOrRemove;
            Deck.Clear();
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
            Deck.Add(type);
        }

        public static void EventOnAddOrRemove(object sender, EventArgs e) {
            if (GameObject.Find(GameRegulars.DeckCounter) == null) return;
            GameObject.Find(GameRegulars.DeckCounter).GetComponent<Text>().text = DeckSize() + "x";
        }
    }
}
